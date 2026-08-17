namespace BilgewaterTrade.Core.Models;

public class RealmListing
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public required Item Item { get; set; }
    public int AuctionHouseSnapshotId { get; set; }
    public required AuctionHouseSnapshot AuctionHouseSnapshot { get; set; }
    public long? BuyoutCopper { get; set; }
    public int Quantity {get; set;}
    public Common.TimeLeft TimeLeft { get; set; }
}