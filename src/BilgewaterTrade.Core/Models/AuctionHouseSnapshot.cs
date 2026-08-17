namespace BilgewaterTrade.Core.Models;

public enum SnapshotSource
{
    Commodities,
    RealmAuctions
}

public class AuctionHouseSnapshot
{
    public int Id { get; set; }
    public SnapshotSource Source { get; set; }
    public string? Realm { get; set; }   // null for commodity snapshots (region-wide)
    public required string Region { get; set; }
    public DateTimeOffset FetchedAt { get; set; }
}