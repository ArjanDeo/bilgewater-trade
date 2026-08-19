using BilgewaterTrade.DataAccess;
using BilgewaterTrade.Worker;
using Microsoft.EntityFrameworkCore;
using Pathoschild.Http.Client;

// Run the worker with these set to true once to seed DB info if it is empty. (remember migrations as well)
// Seeding takes approximately 3 minutes as of 08/26. (Realms is fast, but there are ±170,000 items to insert)
const bool seedRealms = false;
const bool seedItems = false;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<BilgewaterTradeDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("BilgewaterTrade-Dev")));

// Stops spam like:
// INSERT INTO "Items" ("Id", "IsCommodity", "ItemLevel", "Name")
// VALUES (@p3956, @p3957, @p3958, @p3959);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);

builder.Services.AddSingleton<FluentClient>();
builder.Services.AddSingleton<BlizzardAuthClient>();
builder.Services.AddSingleton<BlizzardApiClient>();
builder.Services.AddScoped<SeedData>();

var host = builder.Build();

if (seedRealms || seedItems)
{
    using var scope = host.Services.CreateScope();

    var seedData = scope.ServiceProvider.GetRequiredService<SeedData>();

    if (seedRealms)
        await seedData.SeedRealmsAsync();

    if (seedItems)
        await seedData.SeedItemsAsync();
    
    Console.WriteLine("Database Seeded. Exiting...");
    return;
}


await host.RunAsync();