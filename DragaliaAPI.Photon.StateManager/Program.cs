using System.Security.Cryptography.X509Certificates;
using DragaliaAPI.Photon.StateManager;
using DragaliaAPI.Photon.StateManager.Authentication;
using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Authentication;
using Redis.OM;
using Redis.OM.Contracts;
using Serilog;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

if (Environment.GetEnvironmentVariable("ENABLE_HTTPS") != null)
{
    X509Certificate2 certificate = X509Certificate2.CreateFromPemFile("cert.pem", "cert.key");

    builder.WebHost.ConfigureKestrel(serverOptions =>
        serverOptions.ConfigureHttpsDefaults(options => options.ServerCertificate = certificate)
    );
}

builder.Services.AddControllers();

builder.Host.UseSerilog(
    (context, config) =>
    {
        config.ReadFrom.Configuration(context.Configuration);
        config.Enrich.FromLogContext();

        config.Filter.ByExcluding(
            "EndsWith(RequestPath, '/health') and Coalesce(StatusCode, 200) = 200"
        );
        config.Filter.ByExcluding(
            "EndsWith(RequestPath, '/ping') and Coalesce(StatusCode, 200) = 200"
        );
    }
);

builder
    .Services.AddOptions<RedisCachingOptions>()
    .BindConfiguration(nameof(RedisCachingOptions))
    .Validate(x => x.KeyExpiryTimeMins > 0)
    .ValidateOnStart();

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
            Description = "API for storing room state received from Photon webhooks."
        }
    );
});

Log.Logger.Information("App environment {@env}", builder.Environment);

RedisOptions redisOptions =
    builder.Configuration.GetRequiredSection(nameof(RedisOptions)).Get<RedisOptions>()
    ?? throw new InvalidOperationException("Failed to deserialize Redis configuration");

// Don't attempt to connect to Redis when running tests
if (builder.Environment.EnvironmentName != "Testing")
{
    IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(
        new ConfigurationOptions()
        {
            EndPoints = new() { { redisOptions.Hostname, redisOptions.Port } },
            Password = redisOptions.Password,
        }
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
    public class Program { }
}
