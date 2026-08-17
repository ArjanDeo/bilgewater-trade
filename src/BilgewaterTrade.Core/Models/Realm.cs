namespace BilgewaterTrade.Core.Models;

public class Realm
{
    public int Id { get; set; }
    public int ConnectedRealmId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
}