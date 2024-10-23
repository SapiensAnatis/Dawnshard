using DragaliaAPI.Photon.StateManager;
using DragaliaAPI.Photon.StateManager.Authentication;
using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Redis.OM;
using Redis.OM.Contracts;
using Serilog;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddServiceDefaults();
builder.ConfigureServices();

WebApplication app = builder.Build();

RedisOptions redisOptions = app.Services.GetRequiredService<IOptions<RedisOptions>>().Value;

app.Logger.LogInformation(
    "Connecting to Redis at {Hostname}:{Port}",
    redisOptions.Hostname,
    redisOptions.Port
);

IRedisConnectionProvider provider = app.Services.GetRequiredService<IRedisConnectionProvider>();

bool created = await provider.Connection.CreateIndexAsync(typeof(RedisGame));
RedisIndexInfo? info = await provider.Connection.GetIndexInfoAsync(typeof(RedisGame));

app.Logger.LogInformation("Index created: {Created}", created);
app.Logger.LogInformation("Index info: {@Info}", info);

if (builder.Environment.IsDevelopment())
{
    app.Logger.LogInformation("App is in development mode -- clearing all pre-existing games");
    await provider.RedisCollection<RedisGame>().DeleteAsync(provider.RedisCollection<RedisGame>());
}

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();

namespace DragaliaAPI.Photon.StateManager
{
    // Needed for creating test fixture
    public class Program { }
}
