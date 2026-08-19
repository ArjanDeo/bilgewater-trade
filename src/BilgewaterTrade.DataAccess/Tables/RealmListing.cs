namespace BilgewaterTrade.DataAccess.Tables;


public class RealmListing
{
    public long Id { get; set; }

    public int ItemId { get; set; }
    public Item Item { get; set; }

    public int AuctionHouseSnapshotId { get; set; }
    public AuctionHouseSnapshot AuctionHouseSnapshot { get; set; }

    public long? BuyoutCopper { get; set; }

    public int Quantity { get; set; }

    public Common.TimeLeft TimeLeft { get; set; }
}