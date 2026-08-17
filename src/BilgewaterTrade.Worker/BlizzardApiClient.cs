using BilgewaterTrade.Worker.Dtos;
using Pathoschild.Http.Client;

namespace BilgewaterTrade.Worker;

public class BlizzardApiClient(FluentClient fluentClient, BlizzardAuthClient authClient)
{
    public async Task<CommoditiesResponseDto> GetCommoditiesAsync() =>
        await fluentClient
            .GetAsync("https://eu.api.blizzard.com/data/wow/auctions/commodities")
            .WithHeader("Authorization", $"Bearer {await authClient.GetAccessTokenAsync()}")
            .WithArgument("namespace", "dynamic-eu")
            .As<CommoditiesResponseDto>();
}