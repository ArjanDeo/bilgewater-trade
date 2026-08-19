using BilgewaterTrade.DataAccess;
using Newtonsoft.Json;
namespace BilgewaterTrade.Worker.Dtos;

public class RealmAuctionDto
{
    public long Id { get; set; }
    [JsonProperty("buyout")]
    public long? BuyoutCopper { get; set; }
    [JsonProperty("item")]
    public ItemRefDto Item { get; set; }
    public int Quantity { get; set; }
    [JsonProperty("time_left")]
    public Common.TimeLeft  TimeLeft { get; set; }
}