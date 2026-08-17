using BilgewaterTrade.Core.Models;
using BilgewaterTrade.DataAccess;
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
                // TODO: map commodities.Auctions -> CommodityListing rows, using snapshot.Id
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}