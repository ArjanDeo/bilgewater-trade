using BilgewaterTrade.Core.Dtos;

namespace BilgewaterTrade.Core.Interfaces;

public interface IListingService
{
    public Task<Result<List<SearchListingsDto>>> SearchListings(int connectedRealmId, string searchQuery);
}