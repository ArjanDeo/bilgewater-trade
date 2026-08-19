using BilgewaterTrade.DataAccess.Tables;
using BilgewaterTrade.DataAccess;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pathoschild.Http.Client;

namespace BilgewaterTrade.Worker;

public class ItemSearchResponseDto
{
    [JsonProperty("results")]
    public List<ItemSearchResultDto> Results { get; set; } = [];
}

public class ItemSearchResultDto
{
    [JsonProperty("data")]
    public ItemSearchDataDto? Data { get; set; }
}

public class ItemSearchDataDto
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public Dictionary<string, string> Name { get; set; } = [];

    [JsonProperty("level")]
    public int Level { get; set; }
}

public class SeedData(
    BlizzardApiClient blizzardApiClient,
    BlizzardAuthClient authClient,
    BilgewaterTradeDbContext db)
{
    public async Task SeedRealmsAsync()
    {
        var index = await blizzardApiClient.GetConnectedRealmIndexAsync();

        var existingRealmIds = await db.Realms
            .Select(x => x.Id)
            .ToHashSetAsync();

        foreach (var reference in index.ConnectedRealms)
        {
            var connectedRealm =
                await blizzardApiClient.GetConnectedRealmAsync(reference.Id);

            foreach (var realm in connectedRealm.Realms)
            {
                if (!existingRealmIds.Add(realm.Id))
                    continue;

                db.Realms.Add(new Realm
                {
                    Id = realm.Id,
                    ConnectedRealmId = connectedRealm.Id,
                    Name = realm.Name["en_US"],
                    Slug = realm.Slug
                });
                Console.WriteLine($"Realm {realm.Name["en_US"]} inserted.");
            }
        }

        await db.SaveChangesAsync();
        Console.WriteLine("Finished seeding realms.");
    }

    public async Task SeedItemsAsync()
    {
        var token = await authClient.GetAccessTokenAsync();

        const int pageSize = 1000;
        var minId = 0;
        var totalInserted = 0;

        using var client = new FluentClient();

        while (true)
        {
            var response = await client
                .GetAsync("https://us.api.blizzard.com/data/wow/search/item")
                .WithBearerAuthentication(token)
                .WithArgument("namespace", "static-us")
                .WithArgument("orderby", "id")
                .WithArgument("id", $"[{minId},]")
                .WithArgument("_pageSize", pageSize)
                .As<ItemSearchResponseDto>();

            if (response.Results.Count == 0)
                break;

            var items = response.Results
                .Where(x => x.Data is not null)
                .Select(x =>
                {
                    var name =
                        x.Data.Name.GetValueOrDefault("en_US") ??
                        x.Data.Name.Values.FirstOrDefault();

                    return string.IsNullOrWhiteSpace(name)
                        ? null
                        : new
                        {
                            x.Data.Id,
                            Name = name,
                            ItemLevel = x.Data.Level
                        };
                })
                .Where(x => x is not null)
                .ToList();

            var ids = items
                .Select(x => x.Id)
                .ToList();

            var existingItems = await db.Items
                .Where(x => ids.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);

            foreach (var item in items)
            {
                if (existingItems.TryGetValue(item.Id, out var existing))
                {
                    existing.Name = item.Name;
                    existing.ItemLevel = item.ItemLevel;
                }
                else
                {
                    db.Items.Add(new Item
                    {
                        Id = item.Id,
                        Name = item.Name,
                        ItemLevel = item.ItemLevel,
                        IsCommodity = false
                    });
                }
            }

            await db.SaveChangesAsync();

            totalInserted += items.Count;

            Console.WriteLine(
                $"Upserted {items.Count} items (total: {totalInserted})");

            var highestId = items.Count > 0
                ? items.Max(x => x.Id)
                : minId;

            if (highestId < minId + 1)
            {
                Console.WriteLine(
                    "No forward progress made. Stopping to avoid an infinite loop.");

                break;
            }

            minId = highestId + 1;

            if (response.Results.Count < pageSize)
            {
                Console.WriteLine("Received a partial page. Done.");
                break;
            }

            await Task.Delay(50);
        }

        Console.WriteLine(
            $"Finished. Total items inserted: {totalInserted}");
    }
}