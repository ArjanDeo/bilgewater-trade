using BilgewaterTrade.Core.Interfaces;
using BilgewaterTrade.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace BilgewaterTrade.Api.Endpoints;

public static class ListingEndpoints
{
    public static IEndpointRouteBuilder MapListingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/listings");

        group.MapGet("/", async ([FromQuery]
            int connectedRealmId, [FromQuery]
            string item, [FromServices]
            IListingService listingService) =>
        {
            var result = await listingService.SearchListings(
                connectedRealmId,
                item);

            if (result.IsFailure)
            {
                return result.Error == ListingErrors.NoListingsFoundError
                    ? Results.NotFound(result.Error.Message)
                    : Results.BadRequest(result.Error.Message);
            }

            return Results.Ok(result.Value);
        });

        return app;
    }
}