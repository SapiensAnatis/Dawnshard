using Dawnshard.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

public static class Extensions
{
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();
        builder.AddDefaultHealthChecks();
        builder.ConfigureLogging();

        // Cannot add this as a transitive dependency upgrade breaks logging
        // https://github.com/dotnet/extensions/issues/5336
        // Re-evaluate when upgrading to .NET 9
        //
        // builder.Services.ConfigureHttpClientDefaults(http =>
        // {
        //     http.AddStandardResilienceHandler();
        // });

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks(
            "/health",
            new HealthCheckOptions() { ResponseWriter = HealthCheckWriter.WriteResponse }
        );
        app.MapHealthChecks(
            "/ping",
            new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") }
        );
        app.MapPrometheusScrapingEndpoint();

        return app;
    }

    private static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(
            static (context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
                config.Enrich.FromLogContext();

                config.Filter.ByExcluding(
                    "EndsWith(RequestPath, '/health') and @l in ['verbose', 'debug', 'information'] ci"
                );
                config.Filter.ByExcluding(
                    "EndsWith(RequestPath, '/ping') and @l in ['verbose', 'debug', 'information'] ci"
                );
                config.Filter.ByExcluding(
                    "EndsWith(RequestPath, '/metrics') and @l in ['verbose', 'debug', 'information'] ci"
                );

                if (context.HasOtlpLogsEndpoint())
                {
                    config.WriteTo.OpenTelemetry();
                }
            }
        );

        return builder;
    }

    private static IHostApplicationBuilder ConfigureOpenTelemetry(
        this IHostApplicationBuilder builder
    )
    {
        builder
            .Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            });

        if (builder.HasOtlpTracesEndpoint()) { }

        builder
            .Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddProcessor<FilteringProcessor>();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(
        this IHostApplicationBuilder builder
    )
    {
        if (builder.HasOtlpTracesEndpoint())
        {
            builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
                tracing.AddOtlpExporter()
            );
        }

        if (builder.HasOtlpMetricsEndpoint())
        {
            builder.Services.ConfigureOpenTelemetryMeterProvider(metrics =>
                metrics.AddOtlpExporter()
            );
        }

        if (builder.HasOtlpLogsEndpoint())
        {
            builder.Services.ConfigureOpenTelemetryLoggerProvider(logging =>
                logging.AddOtlpExporter()
            );
        }

        return builder;
    }

    private static IHostApplicationBuilder AddDefaultHealthChecks(
        this IHostApplicationBuilder builder
    )
    {
        builder
            .Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static bool HasOtlpTracesEndpoint(this IHostApplicationBuilder builder) =>
        !string.IsNullOrWhiteSpace(
            builder.Configuration.GetOtlpEndpoint("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT")
        );

    private static bool HasOtlpMetricsEndpoint(this IHostApplicationBuilder builder) =>
        !string.IsNullOrWhiteSpace(
            builder.Configuration.GetOtlpEndpoint("OTEL_EXPORTER_OTLP_METRICS_ENDPOINT")
        );

    private static bool HasOtlpLogsEndpoint(this IHostApplicationBuilder builder) =>
        !string.IsNullOrWhiteSpace(
            builder.Configuration.GetOtlpEndpoint("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT")
        );

    private static bool HasOtlpLogsEndpoint(this HostBuilderContext context) =>
        !string.IsNullOrWhiteSpace(
            context.Configuration.GetOtlpEndpoint("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT")
        );

    private static string? GetOtlpEndpoint(this IConfiguration configuration, string envVarName) =>
        configuration[envVarName] ?? configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
}
