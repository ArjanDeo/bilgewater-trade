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
    #endregion Tables

}