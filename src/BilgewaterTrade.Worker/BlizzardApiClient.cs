using BilgewaterTrade.Worker.Dtos;
using Pathoschild.Http.Client;

namespace BilgewaterTrade.Worker;

public class BlizzardApiClient(FluentClient fluentClient, BlizzardAuthClient authClient)
{
    public async Task<CommoditiesResponseDto> GetCommoditiesAsync() =>
        await fluentClient
            .GetAsync("https://us.api.blizzard.com/data/wow/auctions/commodities")
            .WithBearerAuthentication(await authClient.GetAccessTokenAsync())
            .WithArgument("namespace", "dynamic-us")
            .As<CommoditiesResponseDto>();

    public async Task<RealmAuctionResponseDto> GetRealmAuctionsAsync(int connectedRealmId) =>
        await fluentClient
            .GetAsync($"https://us.api.blizzard.com/data/wow/connected-realm/{connectedRealmId}/auctions")
            .WithBearerAuthentication(await authClient.GetAccessTokenAsync())
            .WithArgument("namespace", "dynamic-us")
            .As<RealmAuctionResponseDto>();
    public async Task<ConnectedRealmIndexResponseDto> GetConnectedRealmIndexAsync() =>
        await fluentClient
            .GetAsync("https://us.api.blizzard.com/data/wow/connected-realm/index")
            .WithBearerAuthentication(await authClient.GetAccessTokenAsync())
            .WithArgument("namespace", "dynamic-us")
            .As<ConnectedRealmIndexResponseDto>();
    public async Task<ConnectedRealmResponseDto> GetConnectedRealmAsync(int connectedRealmId) =>
        await fluentClient
            .GetAsync($"https://us.api.blizzard.com/data/wow/connected-realm/{connectedRealmId}")
            .WithBearerAuthentication(await authClient.GetAccessTokenAsync())
            .WithArgument("namespace", "dynamic-us")
            .As<ConnectedRealmResponseDto>();
}