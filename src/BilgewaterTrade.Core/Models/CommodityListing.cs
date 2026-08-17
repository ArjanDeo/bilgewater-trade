namespace BilgewaterTrade.Core.Models;

public class CommodityListing
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public required Item Item { get; set; }
    public int AuctionHouseSnapshotId { get; set; }
    public required AuctionHouseSnapshot AuctionHouseSnapshot { get; set; }
    public int UnitPriceCopper { get; set; }
    public int Quantity {get; set;}
    public Common.TimeLeft TimeLeft { get; set; }
}