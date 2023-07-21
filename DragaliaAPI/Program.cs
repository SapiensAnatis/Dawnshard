using System.Reflection;
using DragaliaAPI.Database;
using DragaliaAPI.Features.GraphQL;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Health;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using DragaliaAPI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using MudBlazor.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration
    .AddJsonFile("itemSummonOdds.json", optional: false, reloadOnChange: true)
    .AddJsonFile("dragonfruitOdds.json", optional: false, reloadOnChange: true)
    .Build();

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

builder.Services
    .Configure<BaasOptions>(config.GetRequiredSection("Baas"))
    .Configure<LoginOptions>(config.GetRequiredSection("Login"))
    .Configure<DragalipatchOptions>(config.GetRequiredSection("Dragalipatch"))
    .Configure<RedisOptions>(config.GetRequiredSection("Redis"))
    .Configure<PhotonOptions>(config.GetRequiredSection(nameof(PhotonOptions)))
    .Configure<ItemSummonConfig>(config)
    .Configure<DragonfruitConfig>(config);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

// Ensure item summon weightings add to 100%
builder.Services.AddOptions<ItemSummonConfig>().Validate(x => x.Odds.Sum(y => y.Rate) == 100_000);
builder.Services
    .AddOptions<DragonfruitConfig>()
    .Validate(x => x.FruitOdds.Values.All(y => y.Normal + y.Ripe + y.Succulent == 100));

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Host.UseSerilog(
    (context, services, loggerConfig) =>
        loggerConfig.ReadFrom
            .Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
);

// Add services to the container.

builder.Services.AddControllers();
builder.Services
    .AddMvc()
    .AddMvcOptions(option =>
    {
        option.OutputFormatters.Add(new CustomMessagePackOutputFormatter(CustomResolver.Options));
        option.InputFormatters.Add(new CustomMessagePackInputFormatter(CustomResolver.Options));
    })
    .AddJsonOptions(options => ApiJsonOptions.Action.Invoke(options.JsonSerializerOptions));

builder.Services.AddRazorPages();
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ApiContext>()
    .AddCheck<RedisHealthCheck>("Redis", failureStatus: HealthStatus.Unhealthy);

builder.Services
    .AddAuthentication(opts =>
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

builder.Services
    .AddResponseCompression()
    .ConfigureDatabaseServices(builder.Configuration.GetConnectionString("PostgresHost"))
    .ConfigureSharedServices()
    .AddAutoMapper(Assembly.GetExecutingAssembly())
    .AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("RedisHost");
        options.InstanceName = "RedisInstance";
    })
    .AddHttpContextAccessor();

builder.Services.ConfigureGameServices(builder.Configuration);
builder.Services.ConfigureGraphQlSchema();

WebApplication app = builder.Build();

Log.Logger.Debug("App environment: {@env}", app.Environment);

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    app.MigrateDatabase();

app.UseStaticFiles();

app.UseSerilogRequestLogging();

app.UsePathBase("/2.19.0_20220714193707"); // Latest Android app version
app.UsePathBase("/2.19.0_20220719103923"); // Latest iOS app version

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<NotFoundHandlerMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<DailyResetMiddleware>();

app.MapControllers();
app.UseResponseCompression();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapHealthChecks("/health");

app.Run();

namespace DragaliaAPI
{
    public partial class Program { }
}
