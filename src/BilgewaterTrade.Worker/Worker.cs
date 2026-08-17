using BilgewaterTrade.Core.Models;
using BilgewaterTrade.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BilgewaterTrade.Worker;

public class Worker(
    ILogger<Worker> logger,
    BlizzardApiClient apiClient,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    // For testing only
    private const int ConnectedRealmId = 11;

    private Common.TimeLeft ParseCommodityTimeLeft(
        string value,
        long auctionId)
    {
        if (Enum.TryParse<Common.TimeLeft>(
                value,
                ignoreCase: true,
                out var timeLeft))
        {
            return timeLeft;
        }

        logger.LogWarning(
            "Auction {AuctionId} had missing/invalid TimeLeft: {Raw}",
            auctionId,
            value);

        return Common.TimeLeft.Short;
    }

    private async Task FetchCommoditiesAsync(
        CancellationToken stoppingToken)
    {
        var commodities = await apiClient.GetCommoditiesAsync();

        using var scope = scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<BilgewaterTradeDbContext>();

        var snapshot = new AuctionHouseSnapshot
        {
            Source = SnapshotSource.Commodities,
            ConnectedRealmId = null,
            Region = "us",
            FetchedAt = DateTimeOffset.UtcNow
        };

        dbContext.AuctionHouseSnapshots.Add(snapshot);

        await dbContext.SaveChangesAsync(stoppingToken);

        var commodityItemIds = commodities.Auctions
            .Select(a => a.Item.Id)
            .ToHashSet();

        var existingItemIds = await dbContext.Items
            .Where(i => commodityItemIds.Contains(i.Id))
            .Select(i => i.Id)
            .ToHashSetAsync(stoppingToken);

        var newItems = commodityItemIds
            .Except(existingItemIds)
            .Select(id => new Item
            {
                Id = id,
                Name = "Unknown Item",
                ItemLevel = 0,
                IsCommodity = true
            })
            .ToList();

        dbContext.Items.AddRange(newItems);

        await dbContext.SaveChangesAsync(stoppingToken);

        var listings = commodities.Auctions
            .Select(auction => new CommodityListing
            {
                ItemId = auction.Item.Id,
                AuctionHouseSnapshotId = snapshot.Id,
                UnitPriceCopper = auction.UnitPrice,
                Quantity = auction.Quantity,
                TimeLeft = ParseCommodityTimeLeft(
                    auction.TimeLeft,
                    auction.Id)
            })
            .ToList();

        dbContext.CommodityListings.AddRange(listings);

        await dbContext.SaveChangesAsync(stoppingToken);

        logger.LogInformation(
            "Saved {Count} commodity listings",
            listings.Count);
    }

    private async Task FetchRealmAuctionsAsync(
        int connectedRealmId,
        CancellationToken stoppingToken)
    {
        var auctions = await apiClient.GetRealmAuctionsAsync(connectedRealmId);

        using var scope = scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<BilgewaterTradeDbContext>();

        var snapshot = new AuctionHouseSnapshot
        {
            Source = SnapshotSource.RealmAuctions,
            ConnectedRealmId = connectedRealmId,
            Region = "us",
            FetchedAt = DateTimeOffset.UtcNow
        };

        dbContext.AuctionHouseSnapshots.Add(snapshot);

        await dbContext.SaveChangesAsync(stoppingToken);

        var itemIds = auctions.Auctions
            .Select(a => a.Item.Id)
            .ToHashSet();

        var existingItemIds = await dbContext.Items
            .Where(i => itemIds.Contains(i.Id))
            .Select(i => i.Id)
            .ToHashSetAsync(stoppingToken);

        var newItems = itemIds
            .Except(existingItemIds)
            .Select(id => new Item
            {
                Id = id,
                Name = "Unknown Item",
                ItemLevel = 0,
                IsCommodity = false
            })
            .ToList();

        dbContext.Items.AddRange(newItems);

        await dbContext.SaveChangesAsync(stoppingToken);

        var listings = auctions.Auctions
            .Select(auction => new RealmListing
            {
                Id = auction.Id,
                ItemId = auction.Item.Id,
                AuctionHouseSnapshotId = snapshot.Id,
                BuyoutCopper = auction.BuyoutCopper,
                Quantity = auction.Quantity,
                TimeLeft = auction.TimeLeft
            })
            .ToList();

        dbContext.RealmListings.AddRange(listings);

        await dbContext.SaveChangesAsync(stoppingToken);

        logger.LogInformation(
            "Saved {Count} realm auctions for connected realm {ConnectedRealmId}",
            listings.Count,
            connectedRealmId);
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "Starting auction house fetch at: {Time}",
                DateTimeOffset.Now);

            await FetchCommoditiesAsync(stoppingToken);

            await FetchRealmAuctionsAsync(
                ConnectedRealmId,
                stoppingToken);

            logger.LogInformation(
                "Auction house fetch completed at: {Time}",
                DateTimeOffset.Now);

            await Task.Delay(
                TimeSpan.FromHours(1),
                stoppingToken);
        }
    }
}