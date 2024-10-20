using System.Diagnostics;
using System.Reflection;
using DragaliaAPI;
using DragaliaAPI.Authentication;
using DragaliaAPI.Database;
using DragaliaAPI.Features.GraphQL;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Infrastructure.Hangfire;
using DragaliaAPI.Infrastructure.Middleware;
using DragaliaAPI.Infrastructure.OutputCaching;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Health;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.MasterAsset;
using EntityGraphQL.AspNet;
using Hangfire;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.JSInterop;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .Configuration.AddJsonFile(
        Path.Join("Resources", "itemSummonOdds.json"),
        optional: false,
        reloadOnChange: true
    )
    .AddJsonFile(
        Path.Join("Resources", "dragonfruitOdds.json"),
        optional: false,
        reloadOnChange: true
    )
    .AddJsonFile(
        Path.Join("Resources", "bannerConfig.json"),
        optional: false,
        reloadOnChange: true
    );

string kpfPath = Path.Combine(Directory.GetCurrentDirectory(), "config");

builder.Configuration.AddKeyPerFile(directoryPath: kpfPath, optional: true, reloadOnChange: true);

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Host.UseSerilog(
    static (context, services, loggerConfig) =>
        loggerConfig
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
);

builder
    .Services.AddControllers()
    .ConfigureApplicationPartManager(static manager =>
        manager.FeatureProviders.Add(new CustomControllerFeatureProvider())
    )
    .AddMvcOptions(static option =>
    {
        option.OutputFormatters.Add(new CustomMessagePackOutputFormatter(CustomResolver.Options));
        option.InputFormatters.Add(new CustomMessagePackInputFormatter(CustomResolver.Options));
    });

RedisOptions redisOptions =
    builder.Configuration.GetSection(nameof(RedisOptions)).Get<RedisOptions>()
    ?? throw new InvalidOperationException("Failed to get Redis config");
HangfireOptions hangfireOptions =
    builder.Configuration.GetSection(nameof(HangfireOptions)).Get<HangfireOptions>()
    ?? new() { Enabled = false };

builder.Services.ConfigureDatabaseServices(builder.Configuration);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = new()
    {
        EndPoints = new() { { redisOptions.Hostname, redisOptions.Port } },
        Password = redisOptions.Password,
    };
    options.InstanceName = "RedisInstance";
});

if (hangfireOptions.Enabled)
{
    builder.Services.ConfigureHangfire();
}

builder.Services.AddDataProtection().PersistKeysToDbContext<ApiContext>();

builder
    .Services.AddAuthorization()
    .ConfigureAuthentication()
    .AddOutputCache(static opts =>
    {
        opts.AddBasePolicy(
            static cachePolicyBuilder =>
                cachePolicyBuilder
                    .AddPolicy<RepeatedRequestPolicy>()
                    .Expire(TimeSpan.FromMinutes(2)),
            excludeDefaultPolicy: true
        );
    })
    .ConfigureHealthchecks()
    .AddAutoMapper(Assembly.GetExecutingAssembly())
    .AddFeatureManagement();

builder
    .Services.ConfigureGameServices(builder.Configuration)
    .ConfigureGameOptions(builder.Configuration)
    .ConfigureSharedServices()
    .ConfigureGraphQLSchema();

WebApplication app = builder.Build();

app.Logger.LogDebug("Using key-per-file configuration from path {KpfPath}", kpfPath);

Stopwatch watch = new();
app.Logger.LogInformation("Loading MasterAsset data.");

watch.Start();
await MasterAsset.LoadAsync(app.Services.GetRequiredService<IFeatureManager>());
watch.Stop();

app.Logger.LogInformation("Loaded MasterAsset in {Time} ms.", watch.ElapsedMilliseconds);

PostgresOptions postgresOptions = app
    .Services.GetRequiredService<IOptions<PostgresOptions>>()
    .Value;

app.Logger.LogDebug(
    "Using PostgreSQL connection {Host}:{Port}",
    postgresOptions.Hostname,
    postgresOptions.Port
);

app.Logger.LogDebug(
    "Using Redis connection {Host}:{Port}",
    redisOptions.Hostname,
    redisOptions.Port
);

if (!postgresOptions.DisableAutoMigration)
{
    app.MigrateDatabase();
}

// Game endpoints
app.MapWhen(
    static ctx =>
        DragaliaHttpConstants.RoutePrefixes.List.Any(prefix =>
            ctx.Request.Path.StartsWithSegments(prefix)
        ),
    static applicationBuilder =>
    {
        foreach (string prefix in DragaliaHttpConstants.RoutePrefixes.List)
        {
            applicationBuilder.UsePathBase(prefix);
        }

        applicationBuilder.UseMiddleware<HeaderLogContextMiddleware>();
        applicationBuilder.UseSerilogRequestLogging();
        applicationBuilder.UseAuthentication();
        applicationBuilder.UseRouting();
        applicationBuilder.UseAuthorization();
        applicationBuilder.UseMiddleware<IdentityLogContextMiddleware>();
        applicationBuilder.UseMiddleware<ResultCodeLoggingMiddleware>();
        applicationBuilder.UseOutputCache();
        applicationBuilder.UseMiddleware<NotFoundHandlerMiddleware>();
        applicationBuilder.UseMiddleware<ExceptionHandlerMiddleware>();
        applicationBuilder.UseMiddleware<DailyResetMiddleware>();
        applicationBuilder.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGraphQL<ApiContext>(configureEndpoint: endpoint =>
                endpoint.RequireAuthorization(policy =>
                    policy.RequireAuthenticatedUser().AddAuthenticationSchemes(SchemeName.Developer)
                )
            );
        });
    }
);

// Svelte website API
app.MapWhen(
    static ctx => ctx.Request.Path.StartsWithSegments("/api"),
    static applicationBuilder =>
    {
        applicationBuilder.UseRouting();
        applicationBuilder.UseSerilogRequestLogging();
#pragma warning disable ASP0001
        applicationBuilder.UseAuthorization();
#pragma warning restore ASP0001
        applicationBuilder.UseMiddleware<IdentityLogContextMiddleware>();
        applicationBuilder.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
);

if (hangfireOptions.Enabled)
{
    app.AddHangfireJobs();
    app.UseHangfireDashboard();
    app.MapHangfireDashboard()
        .RequireAuthorization(policy =>
        {
            policy
                .RequireAuthenticatedUser()
                .RequireRole(Constants.Roles.Developer)
                .AddAuthenticationSchemes(SchemeName.Developer);
        });
}

app.MapHealthChecks(
    "/health",
    new HealthCheckOptions() { ResponseWriter = HealthCheckWriter.WriteResponse }
);
app.MapGet("/ping", () => Results.Ok());
app.MapGet(
    "/dragalipatch/config",
    (
        [FromServices] IOptionsMonitor<LoginOptions> loginOptions,
        [FromServices] IOptionsMonitor<DragalipatchOptions> patchOptions
    ) =>
        new DragalipatchResponse()
        {
            Mode = patchOptions.CurrentValue.Mode,
            ConeshellKey = patchOptions.CurrentValue.ConeshellKey,
            CdnUrl = patchOptions.CurrentValue.CdnUrl,
            UseUnifiedLogin = loginOptions.CurrentValue.UseBaasLogin,
        }
);

LinqToDBForEFTools.Initialize();
DataConnection.TurnTraceSwitchOn();

app.Run();

namespace DragaliaAPI
{
    public class Program;
}
