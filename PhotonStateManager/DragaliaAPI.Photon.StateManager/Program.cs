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

RedisOptions redisOptions =
    builder.Configuration.GetRequiredSection(nameof(RedisOptions)).Get<RedisOptions>()
    ?? throw new InvalidOperationException("Failed to deserialize Redis configuration");

Log.Logger.Information(
    "Connecting to Redis at {Hostname}:{Port}",
    redisOptions.Hostname,
    redisOptions.Port
);

IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(
    new ConfigurationOptions()
    {
        EndPoints = new() { { redisOptions.Hostname, redisOptions.Port } },
        Password = redisOptions.Password,
        AbortOnConnectFail = false,
    }
);

RedisConnectionProvider provider = new(multiplexer);
builder.Services.AddSingleton<IRedisConnectionProvider>(provider);

WebApplication app = builder.Build();

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
