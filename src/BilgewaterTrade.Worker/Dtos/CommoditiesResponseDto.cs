using System.Text.Json.Serialization;

namespace BilgewaterTrade.Worker.Dtos;

public class CommoditiesResponseDto
{
    [JsonPropertyName("auctions")]
    public required List<CommodityAuctionDto> Auctions { get; set; }
}

public class CommodityAuctionDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("item")]
    public required ItemRefDto Item { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unit_price")]
    public int UnitPrice { get; set; }

    [JsonPropertyName("time_left")]
    public required string TimeLeft { get; set; }
}

public class ItemRefDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}