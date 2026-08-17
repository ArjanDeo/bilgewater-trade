using BilgewaterTrade.Core.Models;
using BilgewaterTrade.DataAccess;
using BilgewaterTrade.Worker.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BilgewaterTrade.Worker;

public class Worker(ILogger<Worker> logger, BlizzardApiClient apiClient, IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Fetching commodities at: {time}", DateTimeOffset.Now);
            var commodities = await apiClient.GetCommoditiesAsync();

            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BilgewaterTradeDbContext>();

                var snapshot = new AuctionHouseSnapshot
                {
                    Source = SnapshotSource.Commodities,
                    Realm = null,
                    Region = "us",
                    FetchedAt = DateTimeOffset.UtcNow
                };
                dbContext.AuctionHouseSnapshots.Add(snapshot);
                await dbContext.SaveChangesAsync(stoppingToken);
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("New commodities snapshot saved at: {time}", DateTimeOffset.Now);

                var commodityIdHashSet = commodities.Auctions
                    .Select(a => a.Item.Id)
                    .ToHashSet();
                
                var existingItemIds = await dbContext.Items
                    .Where(i => commodityIdHashSet.Contains(i.Id))
                    .Select(i => i.Id)
                    .ToHashSetAsync(stoppingToken);
                var newItemIds = commodityIdHashSet.Except(existingItemIds);
                
                var newItems = newItemIds.Select(id => new Item
                {
                    Id = id,
                    Name = "Unknown Item",
                    ItemLevel = 0,
                    IsCommodity = true
                }).ToList();
                var first = commodities.Auctions.First();
                var listings = commodities.Auctions.Select(auction =>
                {
                    if (Enum.TryParse<Common.TimeLeft>(auction.TimeLeft, ignoreCase: true, out var timeLeft))
                        return new CommodityListing
                        {
                            ItemId = auction.Item.Id,
                            AuctionHouseSnapshotId = snapshot.Id,
                            UnitPriceCopper = auction.UnitPrice,
                            Quantity = auction.Quantity,
                            TimeLeft = timeLeft
                        };
                    logger.LogWarning("Auction {AuctionId} had missing/invalid TimeLeft: {Raw}", auction.Id, auction.TimeLeft);
                    timeLeft = Common.TimeLeft.Short; // fallback default

                    return new CommodityListing
                    {
                        ItemId = auction.Item.Id,
                        AuctionHouseSnapshotId = snapshot.Id,
                        UnitPriceCopper = auction.UnitPrice,
                        Quantity = auction.Quantity,
                        TimeLeft = timeLeft
                    };
                }).ToList();

                dbContext.Items.AddRange(newItems);
                await dbContext.SaveChangesAsync(stoppingToken);

                dbContext.CommodityListings.AddRange(listings);
                await dbContext.SaveChangesAsync(stoppingToken);
                
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}