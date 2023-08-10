using DragaliaAPI.Photon.StateManager;
using DragaliaAPI.Photon.StateManager.Authentication;
using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Authentication;
using Redis.OM;
using Redis.OM.Contracts;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

builder.Services.AddControllers();

builder.Host.UseSerilog(
    (context, config) =>
    {
        config.WriteTo.Console();

        SeqOptions seqOptions =
            builder.Configuration.GetSection(nameof(SeqOptions)).Get<SeqOptions>()
            ?? throw new NullReferenceException("Failed to get seq config!");

        if (seqOptions.Enabled)
            config.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.Key);

        config.MinimumLevel.Debug();
        config.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);
        config.Filter.ByExcluding(
            "EndsWith(RequestPath, '/health') and Coalesce(StatusCode, 200) = 200"
        );
        config.Filter.ByExcluding(
            "EndsWith(RequestPath, '/ping') and Coalesce(StatusCode, 200) = 200"
        );
    }
);

builder.Services
    .AddOptions<RedisOptions>()
    .BindConfiguration(nameof(RedisOptions))
    .Validate(x => x.KeyExpiryTimeMins > 0)
    .ValidateOnStart();

builder.Services
    .AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, PhotonAuthenticationHandler>(
        nameof(PhotonAuthenticationHandler),
        null
    );

builder.Services.AddHealthChecks().AddCheck<RedisHealthCheck>("Redis");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc(
        "v1",
        new()
        {
            Version = "v1",
            Title = "Photon State Manager",
            Description = "API for storing room state received from Photon webhooks."
        }
    );
});

Log.Logger.Information("App environment {@env}", builder.Environment);

// Don't attempt to connect to Redis when running tests
if (builder.Environment.EnvironmentName != "Testing")
{
    IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(
        builder.Configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Missing required Redis connection string!")
    );

    IRedisConnectionProvider provider = new RedisConnectionProvider(multiplexer);
    builder.Services.AddSingleton(provider);

    bool created = await provider.Connection.CreateIndexAsync(typeof(RedisGame));

    RedisIndexInfo? info = await provider.Connection.GetIndexInfoAsync(typeof(RedisGame));
    Log.Logger.Information("Index created: {created}", created);
    Log.Logger.Information("Index info: {@info}", info);

    if (builder.Environment.IsDevelopment())
    {
        Log.Logger.Information("App is in development mode -- clearing all pre-existing games");

        await provider
            .RedisCollection<RedisGame>()
            .DeleteAsync(provider.RedisCollection<RedisGame>());
    }
}

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

namespace DragaliaAPI.Photon.StateManager
{
    // Needed for creating test fixture
    public partial class Program { }
}
