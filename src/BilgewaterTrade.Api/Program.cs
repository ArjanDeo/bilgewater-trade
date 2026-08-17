using System.Text.Json;
using BilgewaterTrade.DataAccess;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<BilgewaterTradeDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("BilgewaterTrade-Dev"));
});

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Bilgewater.Trade API v1");
    });
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
    app.Environment.IsDevelopment()
        ? Results.Redirect("/swagger/index.html")
        : Results.Ok("API Running.")
);

app.MapGet("/api/auctions/{connectedRealmId:int}", async (
    int connectedRealmId,
    string item,
    BilgewaterTradeDbContext db,
    IConnectionMultiplexer redis) =>
{
    var cache = redis.GetDatabase();

    var matchingItems = await db.Items
        .AsNoTracking()
        .Where(x => EF.Functions.ILike(x.Name, $"%{item}%"))
        .Select(x => new
        {
            x.Id,
            x.Name,
            x.IsCommodity
        })
        .ToListAsync();

    if (matchingItems.Count == 0)
    {
        return Results.Ok(Array.Empty<AuctionResult>());
    }

    var itemIds = matchingItems
        .Select(x => x.Id)
        .ToArray();

    var cacheKeys = itemIds
        .Select(id => (RedisKey)$"auction:{connectedRealmId}:{id}")
        .ToArray();

    var cachedValues = await cache.StringGetAsync(cacheKeys);

    var cachedResults = new List<AuctionResult>();
    var missedItemIds = new List<int>();

    for (int i = 0; i < itemIds.Length; i++)
    {
        if (cachedValues[i].HasValue)
        {
            var list = JsonSerializer.Deserialize<List<AuctionResult>>((string)cachedValues[i]!)!;
            cachedResults.AddRange(list);
        }
        else
        {
            missedItemIds.Add(itemIds[i]);
        }
    }

    var itemLookup = matchingItems.ToDictionary(x => x.Id);

    var freshResults = new List<AuctionResult>();

    if (missedItemIds.Count > 0)
    {
        // Realm auctions — only for cache misses
        var realmAuctions = await db.RealmListings
            .AsNoTracking()
            .Where(x =>
                x.AuctionHouseSnapshot.ConnectedRealmId == connectedRealmId &&
                missedItemIds.Contains(x.ItemId))
            .Select(x => new AuctionResult
            {
                ItemId = x.ItemId,
                ItemName = x.Item.Name,
                IsCommodity = false,
                BuyoutCopper = x.BuyoutCopper,
                UnitPriceCopper = null,
                Quantity = x.Quantity,
                TimeLeft = x.TimeLeft
            })
            .ToListAsync();

        // Commodities — only for cache misses
        var commodityAuctions = await db.CommodityListings
            .AsNoTracking()
            .Where(x =>
                x.AuctionHouseSnapshot.ConnectedRealmId == null &&
                missedItemIds.Contains(x.ItemId))
            .Select(x => new AuctionResult
            {
                ItemId = x.ItemId,
                ItemName = x.Item.Name,
                IsCommodity = true,
                BuyoutCopper = null,
                UnitPriceCopper = x.UnitPriceCopper,
                Quantity = x.Quantity,
                TimeLeft = x.TimeLeft
            })
            .ToListAsync();

        freshResults = realmAuctions.Concat(commodityAuctions).ToList();

        // Cache all listings for each item together, grouped by ItemId —
        // an item can have many listings, so one key must hold a list,
        // not a single result (otherwise later writes overwrite earlier ones).
        var groupedByItem = freshResults.GroupBy(r => r.ItemId);

        foreach (var group in groupedByItem)
        {
            var key = $"auction:{connectedRealmId}:{group.Key}";
            await cache.StringSetAsync(key, JsonSerializer.Serialize(group.ToList()), TimeSpan.FromHours(1));
        }
    }

    var results = cachedResults
        .Concat(freshResults)
        .OrderBy(x => x.BuyoutCopper ?? x.UnitPriceCopper)
        .ToList();

    return Results.Ok(results);
});

app.Run();

public class AuctionResult
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public bool IsCommodity { get; set; }

    public long? BuyoutCopper { get; set; }

    public long? UnitPriceCopper { get; set; }

    public int Quantity { get; set; }

    public BilgewaterTrade.Core.Models.Common.TimeLeft TimeLeft { get; set; }
}