namespace BilgewaterTrade.DataAccess.Tables;

public class Item
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int ItemLevel { get; set; }
    public bool IsCommodity { get; set; }
}