using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using DragaliaAPI;
using DragaliaAPI.Database;
using DragaliaAPI.Features.Blazor;
using DragaliaAPI.Features.GraphQL;
using DragaliaAPI.Features.Shared.Options;
using DragaliaAPI.Features.TimeAttack;
using DragaliaAPI.Features.Version;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Health;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Json;
using DragaliaAPI.Shared.MasterAsset;
using EntityGraphQL.AspNet;
using EntityGraphQL.Schema;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfiguration config = builder
    .Configuration.AddJsonFile("itemSummonOdds.json", optional: false, reloadOnChange: true)
    .AddJsonFile("dragonfruitOdds.json", optional: false, reloadOnChange: true)
    .Build();

builder.WebHost.UseStaticWebAssets();

builder
    .Services.Configure<BaasOptions>(config.GetRequiredSection("Baas"))
    .Configure<LoginOptions>(config.GetRequiredSection("Login"))
    .Configure<DragalipatchOptions>(config.GetRequiredSection("Dragalipatch"))
    .Configure<RedisOptions>(config.GetRequiredSection("Redis"))
    .Configure<PhotonOptions>(config.GetRequiredSection(nameof(PhotonOptions)))
    .Configure<ItemSummonConfig>(config)
    .Configure<DragonfruitConfig>(config)
    .Configure<TimeAttackOptions>(config.GetRequiredSection(nameof(TimeAttackOptions)))
    .Configure<ResourceVersionOptions>(config.GetRequiredSection(nameof(ResourceVersionOptions)))
    .Configure<BlazorOptions>(config.GetRequiredSection(nameof(BlazorOptions)))
    .Configure<EventOptions>(config.GetRequiredSection(nameof(EventOptions)));

builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(options =>
{
    options.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    options.SnackbarConfiguration.VisibleStateDuration = 5000;
    options.SnackbarConfiguration.ShowTransitionDuration = 500;
    options.SnackbarConfiguration.HideTransitionDuration = 500;
});

// Ensure item summon weightings add to 100%
builder.Services.AddOptions<ItemSummonConfig>().Validate(x => x.Odds.Sum(y => y.Rate) == 100_000);
builder
    .Services.AddOptions<DragonfruitConfig>()
    .Validate(x => x.FruitOdds.Values.All(y => y.Normal + y.Ripe + y.Succulent == 100));

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

// Add services to the container.

builder.Services.AddControllers();
builder
    .Services.AddMvc()
    .AddMvcOptions(option =>
    {
        option.OutputFormatters.Add(new CustomMessagePackOutputFormatter(CustomResolver.Options));
        option.InputFormatters.Add(new CustomMessagePackInputFormatter(CustomResolver.Options));
    })
    .AddJsonOptions(options => ApiJsonOptions.Action.Invoke(options.JsonSerializerOptions));

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder
    .Services.AddHealthChecks()
    .AddDbContextCheck<ApiContext>()
    .AddCheck<RedisHealthCheck>("Redis", failureStatus: HealthStatus.Unhealthy);

builder
    .Services.AddAuthentication(opts =>
    {
        opts.AddScheme<SessionAuthenticationHandler>(SchemeName.Session, null);
        opts.AddScheme<DeveloperAuthenticationHandler>(SchemeName.Developer, null);
        opts.AddScheme<PhotonAuthenticationHandler>(nameof(PhotonAuthenticationHandler), null);

        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(opts =>
    {
        opts.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        opts.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder
    .Services.AddResponseCompression()
    .ConfigureDatabaseServices(builder.Configuration.GetConnectionString("PostgresHost"))
    .ConfigureSharedServices()
    .AddAutoMapper(Assembly.GetExecutingAssembly())
    .AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("RedisHost");
        options.InstanceName = "RedisInstance";
    })
    .AddHttpContextAccessor();

builder.Services.AddDataProtection().PersistKeysToDbContext<ApiContext>();

builder.Services.ConfigureGameServices(builder.Configuration);
builder.Services.ConfigureGraphQlSchema();

WebApplication app = builder.Build();

Stopwatch watch = new();
app.Logger.LogInformation("Loading MasterAsset data.");

watch.Start();
RuntimeHelpers.RunClassConstructor(typeof(MasterAsset).TypeHandle);
watch.Stop();

app.Logger.LogInformation("Loaded MasterAsset in {time}.", watch.Elapsed);

if (Environment.GetEnvironmentVariable("DISABLE_AUTO_MIGRATION") == null)
    app.MigrateDatabase();

app.UseStaticFiles();
app.UseAuthentication();
app.UseResponseCompression();

ImmutableArray<string> apiRoutePrefixes = new[]
{
    "/api",
    "/2.19.0_20220714193707",
    "/2.19.0_20220719103923"
}.ToImmutableArray();

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
            endpoints.MapGraphQL<ApiContext>(
                configureEndpoint: endpoint =>
                    endpoint.RequireAuthorization(
                        policy =>
                            policy
                                .RequireAuthenticatedUser()
                                .AddAuthenticationSchemes(SchemeName.Developer)
                    )
            );
        });
    }
);

app.MapWhen(
    ctx => !apiRoutePrefixes.Any(prefix => ctx.Request.Path.StartsWithSegments(prefix)),
    applicationBuilder =>
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
);

app.MapHealthChecks("/health"); // Kubernetes readiness check
app.MapGet("/ping", () => Results.Ok()); // Kubernetes liveness check
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
    public partial class Program { }
}
