using System.Collections.Frozen;
using System.Diagnostics;
using System.Reflection;
using DragaliaAPI;
using DragaliaAPI.Authentication;
using DragaliaAPI.Database;
using DragaliaAPI.Features.GraphQL;
using DragaliaAPI.Infrastructure.Hangfire;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Health;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.MasterAsset;
using EntityGraphQL.AspNet;
using Hangfire;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

builder.WebHost.UseStaticWebAssets();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Host.UseSerilog(
    (context, services, loggerConfig) =>
        loggerConfig
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            // Blazor keeps throwing these errors from MudBlazor internals; there is nothing we can do about them
            .Filter.ByExcluding(evt => evt.Exception is JSDisconnectedException)
);

builder
    .Services.AddControllers()
    .AddMvcOptions(option =>
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
    .AddResponseCompression()
    .ConfigureHealthchecks()
    .AddAutoMapper(Assembly.GetExecutingAssembly());

builder
    .Services.ConfigureGameServices(builder.Configuration)
    .ConfigureGameOptions(builder.Configuration)
    .ConfigureSharedServices()
    .ConfigureGraphQLSchema()
    .ConfigureBlazorFrontend();

WebApplication app = builder.Build();

Stopwatch watch = new();
app.Logger.LogInformation("Loading MasterAsset data.");

watch.Start();
await MasterAsset.LoadAsync();
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
    app.MigrateDatabase();

app.UseStaticFiles();
app.UseAuthentication();
app.UseResponseCompression();

#pragma warning disable CA1861 // Avoid constant arrays as arguments. Only created once as top-level statement.
FrozenSet<string> apiRoutePrefixes = new[]
{
    "/2.19.0_20220714193707",
    "/2.19.0_20220719103923"
}.ToFrozenSet();
#pragma warning restore CA1861

// Game endpoints
app.MapWhen(
    ctx => apiRoutePrefixes.Any(prefix => ctx.Request.Path.StartsWithSegments(prefix)),
    applicationBuilder =>
    {
        foreach (string prefix in apiRoutePrefixes)
            applicationBuilder.UsePathBase(prefix);

        applicationBuilder.UseRouting();
        applicationBuilder.UseAuthorization();
        applicationBuilder.UseMiddleware<PlayerIdentityLoggingMiddleware>();
        applicationBuilder.UseSerilogRequestLogging();
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
    applicationBuilder =>
    {
        // todo unfuck cors
        applicationBuilder.UseCors(cors =>
            cors.WithOrigins("http://localhost:3001")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
        );
        applicationBuilder.UseRouting();
        applicationBuilder.UseSerilogRequestLogging();
#pragma warning disable ASP0001
        applicationBuilder.UseAuthorization();
#pragma warning restore ASP0001
        applicationBuilder.UseAntiforgery();
        applicationBuilder.UseMiddleware<PlayerIdentityLoggingMiddleware>();
        applicationBuilder.UseEndpoints(endpoints =>
        {
            endpoints.MapDragaliaAPIEndpoints();
        });
    }
);

// Blazor website
app.MapWhen(
    static ctx => !ctx.Request.Path.StartsWithSegments("/api"),
    applicationBuilder =>
    {
        {
            applicationBuilder.UseRouting();
#pragma warning disable ASP0001
            applicationBuilder.UseAuthorization();
#pragma warning restore ASP0001
            applicationBuilder.UseAntiforgery();
            applicationBuilder.UseMiddleware<PlayerIdentityLoggingMiddleware>();
            applicationBuilder.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapRazorComponents<App>().AddInteractiveServerRenderMode();
            });
        }
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
            UseUnifiedLogin = loginOptions.CurrentValue.UseBaasLogin
        }
);

app.Run();

namespace DragaliaAPI
{
    public class Program;
}
