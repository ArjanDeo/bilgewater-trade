using BilgewaterTrade.Api.Endpoints;
using BilgewaterTrade.Core.Interfaces;
using BilgewaterTrade.Core.Services;
using BilgewaterTrade.DataAccess;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<BilgewaterTradeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BilgewaterTrade-Dev")));

// Redis
var redisService = ConnectionMultiplexer
    .Connect(builder.Configuration.GetConnectionString("Redis-Dev")!);

builder.Services.AddSingleton<IConnectionMultiplexer>(redisService);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // SvelteKit dev server
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddScoped<IListingService, ListingService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowFrontendDev");
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Bilgewater.Trade API v1");
    });
}

await using var scope = app.Services.CreateAsyncScope();
var db = scope.ServiceProvider.GetRequiredService<BilgewaterTradeDbContext>();

var pendingMigrations = await db.Database.GetPendingMigrationsAsync();

if (pendingMigrations.Any())
{
    Console.Error.WriteLine("ERROR: Database migrations are pending.\nRun 'dotnet ef database update' before starting the application.");
    return;
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
    app.Environment.IsDevelopment()
        ? Results.Redirect("/swagger/index.html")
        : Results.Ok("API Running.")
);

app.MapListingEndpoints();

app.Run();

