using BilgewaterTrade.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BilgewaterTrade.DataAccess;

public class BilgewaterTradeDbContext : DbContext
{
    public BilgewaterTradeDbContext(DbContextOptions<BilgewaterTradeDbContext> options) : base(options)
    {
        //...
    }
    #region Tables
    public DbSet<Item> Items => Set<Item>();
    public DbSet<AuctionHouseSnapshot> AuctionHouseSnapshots => Set<AuctionHouseSnapshot>();
    public DbSet<CommodityListing> CommodityListings => Set<CommodityListing>();
    public DbSet<RealmListing> RealmListings => Set<RealmListing>();
    
    #endregion Tables

}