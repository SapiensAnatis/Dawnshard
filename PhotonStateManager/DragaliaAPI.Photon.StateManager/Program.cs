using DragaliaAPI.Photon.StateManager;
using DragaliaAPI.Photon.StateManager.Models;
using Redis.OM;
using Redis.OM.Contracts;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddServiceDefaults();
builder.ConfigureServices();

WebApplication app = builder.Build();

IRedisConnectionProvider provider = app.Services.GetRequiredService<IRedisConnectionProvider>();

bool created = await provider.Connection.CreateIndexAsync(typeof(RedisGame));
RedisIndexInfo? info = await provider.Connection.GetIndexInfoAsync(typeof(RedisGame));

Log.IndexCreated(app.Logger, created);
Log.IndexInfo(app.Logger, info);

if (builder.Environment.IsDevelopment())
{
    Log.DevelopmentModeClearing(app.Logger);
    await provider.RedisCollection<RedisGame>().DeleteAsync(provider.RedisCollection<RedisGame>());
}

// Force this to be instantiated as it is not injected anywhere at the moment
app.Services.GetRequiredService<PhotonStateManagerMetrics>();

app.UseSerilogRequestLogging();

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

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Index created: {Created}")]
    public static partial void IndexCreated(ILogger logger, bool created);

    [LoggerMessage(Level = LogLevel.Information, Message = "Index info: {@Info}")]
    public static partial void IndexInfo(ILogger logger, RedisIndexInfo? info);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "App is in development mode -- clearing all pre-existing games"
    )]
    public static partial void DevelopmentModeClearing(ILogger logger);
}
