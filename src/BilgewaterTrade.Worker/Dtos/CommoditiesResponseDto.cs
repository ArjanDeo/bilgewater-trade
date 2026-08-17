using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace BilgewaterTrade.Worker.Dtos;

public class CommoditiesResponseDto
{
    [JsonProperty("auctions")]
    public required List<CommodityAuctionDto> Auctions { get; set; }
}

public class CommodityAuctionDto
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("item")]
    public required ItemRefDto Item { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("unit_price")]
    public long UnitPrice { get; set; }

    [JsonProperty("time_left")]
    public required string TimeLeft { get; set; }
}

public class ItemRefDto
{
    [JsonProperty("id")]
    public int Id { get; set; }
}