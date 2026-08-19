using System.Text.Json;
using BilgewaterTrade.Core.Dtos;
using BilgewaterTrade.Core.Interfaces;
using BilgewaterTrade.DataAccess;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace BilgewaterTrade.Core.Services;

public class ListingService(BilgewaterTradeDbContext context, IConnectionMultiplexer redis) : IListingService
{
    public async Task<Result<List<SearchListingsDto>>> SearchListings(int connectedRealmId, string searchQuery)
    {
        var cache = redis.GetDatabase();

            var matchingItems = await context.Items
                .AsNoTracking()
                .Where(x => EF.Functions.ILike(x.Name, $"%{searchQuery}%"))
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.IsCommodity,
                })
                .ToListAsync();

            if (matchingItems.Count == 0)
                return Result<List<SearchListingsDto>>.Failure(new Error("404", "No listings found"));

            var itemIds = matchingItems
                .Select(x => x.Id)
                .ToArray();

            var cacheKeys = itemIds
                .Select(id => (RedisKey)$"listing:{connectedRealmId}:{id}")
                .ToArray();

            var cachedValues = await cache.StringGetAsync(cacheKeys);

            var cachedResults = new List<SearchListingsDto>();
            var missedItemIds = new List<int>();

            for (int i = 0; i < itemIds.Length; i++)
            {
                if (cachedValues[i].HasValue)
                {
                    var list = JsonSerializer.Deserialize<List<SearchListingsDto>>((string)cachedValues[i]!)!;
                    cachedResults.AddRange(list);
                }
                else
                {
                    missedItemIds.Add(itemIds[i]);
                }
            }

            var itemLookup = matchingItems.ToDictionary(x => x.Id);

            var freshResults = new List<SearchListingsDto>();

            if (missedItemIds.Count > 0)
            {
                // Realm auctions — only for cache misses
                var realmAuctions = await context.RealmListings
                    .AsNoTracking()
                    .Where(x =>
                        x.AuctionHouseSnapshot.ConnectedRealmId == connectedRealmId &&
                        missedItemIds.Contains(x.ItemId))
                    .Select(x => new SearchListingsDto()
                    {
                        CheapestBuyoutCopper = x.BuyoutCopper,
                        IsCommodity = false,
                        ItemId = x.ItemId,
                        ItemName = x.Item.Name,
                        Quantity = x.Quantity,
                        CheapestUnitPriceCopper = x.BuyoutCopper
                    })
                    .ToListAsync();

                // Commodities — only for cache misses
                var commodityAuctions = await context.CommodityListings
                    .AsNoTracking()
                    .Where(x =>
                        x.AuctionHouseSnapshot.ConnectedRealmId == null &&
                        missedItemIds.Contains(x.ItemId))
                    .Select(x => new SearchListingsDto()
                    {
                        ItemId = x.ItemId,
                        ItemName = x.Item.Name,
                        IsCommodity = true,
                        CheapestBuyoutCopper = null,
                        CheapestUnitPriceCopper = x.UnitPriceCopper,
                        Quantity = x.Quantity,
                    })
                    .ToListAsync();

                freshResults = [.. realmAuctions, .. commodityAuctions];

                // Cache all listings for each searchQuery together, grouped by ItemId —
                // an searchQuery can have many listings, so one key must hold a list,
                // not a single result (otherwise later writes overwrite earlier ones).
                var groupedByItem = freshResults.GroupBy(r => r.ItemId);

                foreach (var group in groupedByItem)
                {
                    var key = $"listing:{connectedRealmId}:{group.Key}";
                    await cache.StringSetAsync(key, JsonSerializer.Serialize(group.ToList()), TimeSpan.FromHours(1));
                }
            }

            var allResults = cachedResults.Concat(freshResults);

            var aggregated = new Dictionary<int, SearchListingsDto>();

            foreach (var listing in allResults)
            {
                if (aggregated.TryGetValue(listing.ItemId, out var existing))
                {
                    existing.Quantity += listing.Quantity;

                    if (listing.CheapestUnitPriceCopper is { } unitPrice &&
                        (existing.CheapestUnitPriceCopper is null || unitPrice < existing.CheapestUnitPriceCopper))
                    {
                        existing.CheapestUnitPriceCopper = unitPrice;
                    }

                    if (listing.CheapestBuyoutCopper is { } buyoutPrice &&
                        (existing.CheapestBuyoutCopper is null || buyoutPrice < existing.CheapestBuyoutCopper))
                    {
                        existing.CheapestBuyoutCopper = buyoutPrice;
                    }
                }
                else
                {
                    // new SearchListingsDto instance per item, so we don't mutate cached/fresh source objects
                    aggregated[listing.ItemId] = new SearchListingsDto
                    {
                        ItemId = listing.ItemId,
                        ItemName = listing.ItemName,
                        IsCommodity = listing.IsCommodity,
                        Quantity = listing.Quantity,
                        CheapestUnitPriceCopper = listing.CheapestUnitPriceCopper,
                        CheapestBuyoutCopper = listing.CheapestBuyoutCopper
                    };
                }
            }

            var results = aggregated.Values
                .OrderBy(x => x.CheapestUnitPriceCopper ?? x.CheapestBuyoutCopper)
                .ToList();

            return Result<List<SearchListingsDto>>.Success(results);
    }
    
}