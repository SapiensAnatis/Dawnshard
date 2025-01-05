using DragaliaAPI.Photon.StateManager.Authentication;
using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Authentication;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Redis.OM;
using Redis.OM.Contracts;
using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager;

internal static class ServiceConfiguration
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.ConfigureOptions();
        builder.ConfigureObservability();

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

        builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            string connectionString =
                serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("redis")
                ?? throw new InvalidOperationException("Missing Redis connection string!");

            return ConnectionMultiplexer.Connect(connectionString);
        });
        builder.Services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();

        return builder;
    }

    private static WebApplicationBuilder ConfigureOptions(this WebApplicationBuilder builder)
    {
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
            .Validate(
                x => !string.IsNullOrEmpty(x.Token),
                "Must specify a value for PhotonOptions.Token!"
            )
            .ValidateOnStart();

        builder.Services.AddOptions<RedisOptions>().BindConfiguration(nameof(RedisOptions));

        return builder;
    }

    private static WebApplicationBuilder ConfigureObservability(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddOpenTelemetry()
            .ConfigureResource(cfg =>
            {
                cfg.AddService(
                    serviceName: "photon-state-manager",
                    autoGenerateServiceInstanceId: false
                );
            });

        if (builder.HasOtlpTracesEndpoint())
        {
            builder
                .Services.AddOpenTelemetry()
                .WithTracing(tracing => tracing.AddRedisInstrumentation());
        }

        builder.Services.AddSingleton<PhotonStateManagerMetrics>();
        builder
            .Services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics.AddMeter(PhotonStateManagerMetrics.MeterName));

        return builder;
    }
}
