using System.Reflection;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Stamp;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.GraphQL;
using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Services.Health;
using DragaliaAPI.Services.Photon;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Json;
using EntityGraphQL.AspNet;
using EntityGraphQL.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration
    .AddJsonFile("itemSummonOdds.json", optional: false, reloadOnChange: true)
    .Build();

builder.Services
    .Configure<BaasOptions>(config.GetRequiredSection("Baas"))
    .Configure<LoginOptions>(config.GetRequiredSection("Login"))
    .Configure<DragalipatchOptions>(config.GetRequiredSection("Dragalipatch"))
    .Configure<RedisOptions>(config.GetRequiredSection("Redis"))
    .Configure<PhotonOptions>(config.GetRequiredSection(nameof(PhotonOptions)))
    .Configure<ItemSummonConfig>(config);

// Ensure item summon weightings add to 100%
builder.Services.AddOptions<ItemSummonConfig>().Validate(x => x.Odds.Sum(y => y.Rate) == 100_000);

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

builder.Services.AddRazorPages(
    options =>
        // Make root URL redirect to news instead of 404
        options.Conventions.AddPageRoute("/News", "~/")
);
builder.Services.AddServerSideBlazor();
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ApiContext>()
    .AddCheck<RedisHealthCheck>("Redis", failureStatus: HealthStatus.Unhealthy);

builder.Services.AddAuthentication(opts =>
{
    opts.AddScheme<SessionAuthenticationHandler>(SchemeName.Session, null);
    opts.AddScheme<DeveloperAuthenticationHandler>(SchemeName.Developer, null);
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

builder.Services
    .AddScoped<ISessionService, SessionService>()
#pragma warning disable CS0618 // Type or member is obsolete
    .AddScoped<IDeviceAccountService, DeviceAccountService>()
#pragma warning restore CS0618 // Type or member is obsolete
    .AddScoped<ISummonService, SummonService>()
    .AddScoped<IUpdateDataService, UpdateDataService>()
    .AddScoped<IDungeonService, DungeonService>()
    .AddScoped<IDragonService, DragonService>()
    .AddScoped<ISavefileService, SavefileService>()
    .AddScoped<IHelperService, HelperService>()
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<IBonusService, BonusService>()
    .AddScoped<IWeaponService, WeaponService>()
    .AddScoped<IFortService, FortService>()
    .AddScoped<IQuestRewardService, QuestRewardService>()
    .AddScoped<IStoryService, StoryService>()
    .AddScoped<IMatchingService, MatchingService>()
    .AddScoped<IAbilityCrestService, AbilityCrestService>()
    .AddScoped<IHeroParamService, HeroParamService>()
    .AddScoped<ITutorialService, TutorialService>()
    .AddScoped<ILoadService, LoadService>()
    .AddScoped<IStampService, StampService>()
    .AddScoped<IStampRepository, StampRepository>()
    .AddScoped<ISavefileUpdateService, SavefileUpdateService>();

builder.Services
    .AddTransient<ILogEventEnricher, AccountIdEnricher>()
    .AddTransient<ILogEventEnricher, PodNameEnricher>()
    // Mission Feature
    .AddScoped<IMissionRepository, MissionRepository>()
    .AddScoped<IMissionService, MissionService>()
    .AddScoped<IRewardService, RewardService>()
    .AddScoped<IMissionProgressionService, MissionProgressionService>()
    .AddScoped<IMissionInitialProgressionService, MissionInitialProgressionService>()
    // Shop Feature
    .AddScoped<IShopRepository, ShopRepository>()
    .AddScoped<IItemSummonService, ItemSummonService>()
    .AddScoped<IPaymentService, PaymentService>()
    .AddScoped<IShopService, ShopService>()
    // Present feature
    .AddScoped<IPresentService, PresentService>()
    .AddScoped<IPresentControllerService, PresentControllerService>()
    .AddScoped<IPresentRepository, PresentRepository>()
    // Treasure Trade Feature
    .AddScoped<ITradeRepository, TradeRepository>()
    .AddScoped<ITradeService, TradeService>();

builder.Services.AddAllOfType<ISavefileUpdate>();

builder.Services.AddHttpClient<IBaasApi, BaasApi>();
builder.Services.AddHttpClient<IPhotonStateApi, PhotonStateApi>(client =>
{
    PhotonOptions? options = builder.Configuration
        .GetRequiredSection(nameof(PhotonOptions))
        .Get<PhotonOptions>();
    ArgumentNullException.ThrowIfNull(options);

    client.BaseAddress = new(options.StateManagerUrl);
});

builder.Services.ConfigureGraphQlSchema();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();

Log.Logger.Debug("App environment: {@env}", app.Environment);

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    app.MigrateDatabase();

app.MapRazorPages();

app.UsePathBase("/2.19.0_20220714193707"); // Latest Android app version
app.UsePathBase("/2.19.0_20220719103923"); // Latest iOS app version

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<NotFoundHandlerMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseResponseCompression();
app.MapHealthChecks("/health");

app.Run();

namespace DragaliaAPI
{
    public partial class Program { }
}
