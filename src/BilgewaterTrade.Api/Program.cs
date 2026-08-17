using BilgewaterTrade.DataAccess;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<BilgewaterTradeDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("BilgewaterTrade-Dev"));
});

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
    BilgewaterTradeDbContext db) =>
{
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
        .ToHashSet();

    var itemLookup = matchingItems
        .ToDictionary(x => x.Id);

    // Realm auctions
    var realmAuctions = await db.RealmListings
        .AsNoTracking()
        .Where(x =>
            x.AuctionHouseSnapshot.ConnectedRealmId == connectedRealmId &&
            itemIds.Contains(x.ItemId))
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

    // Commodities
    var commodityAuctions = await db.CommodityListings
        .AsNoTracking()
        .Where(x =>
            x.AuctionHouseSnapshot.ConnectedRealmId == null &&
            itemIds.Contains(x.ItemId))
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

    var results = realmAuctions
        .Concat(commodityAuctions)
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