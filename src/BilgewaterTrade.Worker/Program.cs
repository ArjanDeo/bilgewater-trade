using BilgewaterTrade.DataAccess;
using BilgewaterTrade.Worker;
using Microsoft.EntityFrameworkCore;
using Pathoschild.Http.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<BilgewaterTradeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BilgewaterTrade-Dev")));

builder.Services.AddSingleton<FluentClient>();
builder.Services.AddSingleton<BlizzardAuthClient>();
builder.Services.AddSingleton<BlizzardApiClient>();

var host = builder.Build();
host.Run();
