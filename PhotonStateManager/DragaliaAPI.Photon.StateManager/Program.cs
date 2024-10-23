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

builder.Host.UseSerilog(
    (context, config) =>
    {
        config.ReadFrom.Configuration(context.Configuration);
        config.Enrich.FromLogContext();

        config.Filter.ByExcluding(
            "EndsWith(RequestPath, '/health') and @l in ['verbose', 'debug', 'information'] ci"
        );
        config.Filter.ByExcluding(
            "EndsWith(RequestPath, '/ping') and @l in ['verbose', 'debug', 'information'] ci"
        );
    }
);

builder
    .Services.AddOptions<RedisCachingOptions>()
    .BindConfiguration(nameof(RedisCachingOptions))
    .Validate(
        x => x.KeyExpiryTimeMins > 0,
        "RedisCachingOptions.KeyExpiryTime must be greater than 0!"
    )
    .ValidateOnStart();

builder
    .Services.AddOptions<PhotonOptions>()
    .BindConfiguration(nameof(PhotonOptions))
    .Validate(x => !string.IsNullOrEmpty(x.Token), "Must specify a value for PhotonOptions.Token!")
    .ValidateOnStart();

builder.Services.AddOptions<RedisOptions>().BindConfiguration(nameof(RedisOptions));

builder
    .Services.AddAuthentication()
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
            Description = "API for storing room state received from Photon webhooks.",
        }
    );
});

builder.Services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>(static sp =>
{
    string connectionString =
        sp.GetRequiredService<IConfiguration>().GetConnectionString("redis")
        ?? throw new InvalidOperationException("Missing Redis connection string!");

    IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(connectionString);

    return new RedisConnectionProvider(multiplexer);
});

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
app.MapHealthChecks("/health");

app.Run();

namespace DragaliaAPI.Photon.StateManager
{
    // Needed for creating test fixture
    public class Program { }
}
