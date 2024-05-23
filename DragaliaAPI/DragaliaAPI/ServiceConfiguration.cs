using DragaliaAPI.Authentication;
using DragaliaAPI.Database;
using DragaliaAPI.Features.Blazor;
using DragaliaAPI.Features.Chara;
using DragaliaAPI.Features.ClearParty;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.DmodeDungeon;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Dungeon.AutoRepeat;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Dungeon.Start;
using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Item;
using DragaliaAPI.Features.Login.Actions;
using DragaliaAPI.Features.Maintenance;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.PartyPower;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Features.Shared.Options;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Features.Stamp;
using DragaliaAPI.Features.Story;
using DragaliaAPI.Features.StorySkip;
using DragaliaAPI.Features.Talisman;
using DragaliaAPI.Features.TimeAttack;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.Features.Version;
using DragaliaAPI.Features.Zena;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Services.Health;
using DragaliaAPI.Services.Photon;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MudBlazor;
using MudBlazor.Services;

namespace DragaliaAPI;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureGameServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddScoped<ISessionService, SessionService>()
#pragma warning disable CS0618 // Type or member is obsolete
            .AddScoped<IDeviceAccountService, DeviceAccountService>()
#pragma warning restore CS0618 // Type or member is obsolete
            .AddScoped<IUpdateDataService, UpdateDataService>()
            .AddScoped<IDragonService, DragonService>()
            .AddScoped<ISavefileService, SavefileService>()
            .AddScoped<IHelperService, HelperService>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IBonusService, BonusService>()
            .AddScoped<IWeaponService, WeaponService>()
            .AddScoped<IMatchingService, MatchingService>()
            .AddScoped<IAbilityCrestService, AbilityCrestService>()
            .AddScoped<IHeroParamService, HeroParamService>()
            .AddScoped<ITutorialService, TutorialService>()
            .AddScoped<ILoadService, LoadService>()
            .AddScoped<IStampService, StampService>()
            .AddScoped<IStampRepository, StampRepository>()
            .AddScoped<ISavefileUpdateService, SavefileUpdateService>()
            .AddTransient<PlayerIdentityLoggingMiddleware>();

        services
            .AddSummoningFeature()
            .AddRewardFeature()
            .AddLoginFeature()
            .AddWallFeature()
            .AddPresentFeature()
            .AddQuestFeature()
            .AddStoryFeature()
            .AddWebFeature();

        services
            .RegisterMissionServices()
            // Shop Feature
            .AddScoped<IShopRepository, ShopRepository>()
            .AddScoped<IItemSummonService, ItemSummonService>()
            .AddScoped<IPaymentService, PaymentService>()
            .AddScoped<IShopService, ShopService>()
            // Treasure Trade Feature
            .AddScoped<ITradeRepository, TradeRepository>()
            .AddScoped<ITradeService, TradeService>()
            // Fort Feature
            .AddScoped<IFortService, FortService>()
            .AddScoped<IFortRepository, FortRepository>()
            // Dungeon Feature
            .AddScoped<IDungeonService, DungeonService>()
            .AddScoped<IDungeonStartService, DungeonStartService>()
            .AddScoped<IDungeonRepository, DungeonRepository>()
            .AddScoped<IQuestEnemyService, QuestEnemyService>()
            .AddScoped<IOddsInfoService, OddsInfoService>()
            .AddScoped<IDungeonRecordService, DungeonRecordService>()
            .AddScoped<IDungeonRecordHelperService, DungeonRecordHelperService>()
            .AddScoped<IDungeonRecordRewardService, DungeonRecordRewardService>()
            .AddScoped<IDungeonRecordDamageService, DungeonRecordDamageService>()
            .AddScoped<IQuestCompletionService, QuestCompletionService>()
            .AddScoped<IAbilityCrestMultiplierService, AbilityCrestMultiplierService>()
            .AddScoped<IAutoRepeatService, AutoRepeatService>()
            // Event Feature
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IEventService, EventService>()
            .AddScoped<IEventDropService, EventDropService>()
            .AddScoped<ITimeAttackService, TimeAttackService>()
            .AddScoped<ITimeAttackRepository, TimeAttackRepository>()
            .AddScoped<ITimeAttackCacheService, TimeAttackCacheService>()
            // Clear party feature
            .AddScoped<IClearPartyRepository, ClearPartyRepository>()
            .AddScoped<IClearPartyService, ClearPartyService>()
            // Dmode feature
            .AddScoped<IDmodeRepository, DmodeRepository>()
            .AddScoped<IDmodeCacheService, DmodeCacheService>()
            .AddScoped<IDmodeService, DmodeService>()
            .AddScoped<IDmodeDungeonService, DmodeDungeonService>()
            // Item feature
            .AddScoped<IItemRepository, ItemRepository>()
            .AddScoped<IItemService, ItemService>()
            // User feature
            .AddScoped<IUserService, UserService>()
            // Talisman feature
            .AddScoped<ITalismanService, TalismanService>()
            // Emblem feature
            .AddScoped<IEmblemRepository, EmblemRepository>()
            // Quest feature
            // Party power feature
            .AddScoped<IPartyPowerService, PartyPowerService>()
            .AddScoped<IPartyPowerRepository, PartyPowerRepository>()
            // Chara feature
            .AddScoped<ICharaService, CharaService>()
            .AddScoped<IResourceVersionService, ResourceVersionService>()
            .AddScoped<ICharaService, CharaService>()
            // Zena feature
            .AddScoped<IZenaService, ZenaService>()
            // Story skip feature
            .AddScoped<StorySkipService>()
            // Maintenance feature
            .AddScoped<MaintenanceService>();

        services.AddHandlers();

        services.AddScoped<IBlazorIdentityService, BlazorIdentityService>();

        services.AddAllOfType<ISavefileUpdate>();

        services.AddHttpClient<IBaasApi, BaasApi>();

        services.AddHttpClient<IPhotonStateApi, PhotonStateApi>(client =>
        {
            PhotonOptions? options = configuration
                .GetRequiredSection(nameof(PhotonOptions))
                .Get<PhotonOptions>();
            ArgumentNullException.ThrowIfNull(options);

            client.BaseAddress = new(options.StateManagerUrl);
        });
        services.AddScoped<IMatchingService, MatchingService>();

        services.AddScoped<ResourceVersionActionFilter>().AddScoped<MaintenanceActionFilter>();

        return services;
    }

    public static IServiceCollection ConfigureGameOptions(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services
            .Configure<BaasOptions>(config.GetRequiredSection("Baas"))
            .Configure<LoginOptions>(config.GetRequiredSection("Login"))
            .Configure<DragalipatchOptions>(config.GetRequiredSection("Dragalipatch"))
            .Configure<RedisCachingOptions>(config.GetRequiredSection(nameof(RedisCachingOptions)))
            .Configure<PhotonOptions>(config.GetRequiredSection(nameof(PhotonOptions)))
            .Configure<ItemSummonConfig>(config)
            .Configure<DragonfruitConfig>(config)
            .Configure<TimeAttackOptions>(config.GetRequiredSection(nameof(TimeAttackOptions)))
            .Configure<ResourceVersionOptions>(
                config.GetRequiredSection(nameof(ResourceVersionOptions))
            )
            .Configure<BlazorOptions>(config.GetRequiredSection(nameof(BlazorOptions)))
            .Configure<EventOptions>(config.GetRequiredSection(nameof(EventOptions)))
            .Configure<MaintenanceOptions>(config.GetRequiredSection(nameof(MaintenanceOptions)));

        services.AddSummoningOptions(config);

        // Ensure item summon weightings add to 100%
        services
            .AddOptions<ItemSummonConfig>()
            .Validate(x => x.Odds.Sum(y => y.Rate) == 100_000)
            .ValidateOnStart();

        services
            .AddOptions<DragonfruitConfig>()
            .Validate(x => x.FruitOdds.Values.All(y => y.Normal + y.Ripe + y.Succulent == 100))
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection ConfigureBlazorFrontend(this IServiceCollection services)
    {
        services.AddServerSideBlazor();
        services.AddMudServices(options =>
        {
            options.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            options.SnackbarConfiguration.VisibleStateDuration = 5000;
            options.SnackbarConfiguration.ShowTransitionDuration = 500;
            options.SnackbarConfiguration.HideTransitionDuration = 500;
        });
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddRazorPages();

        return services;
    }

    public static IServiceCollection ConfigureHealthchecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<ApiContext>()
            .AddCheck<RedisHealthCheck>("Redis", failureStatus: HealthStatus.Unhealthy);

        return services;
    }

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(opts =>
            {
                opts.AddScheme<SessionAuthenticationHandler>(SchemeName.Session, null);
                opts.AddScheme<DeveloperAuthenticationHandler>(SchemeName.Developer, null);
                opts.AddScheme<PhotonAuthenticationHandler>(
                    nameof(PhotonAuthenticationHandler),
                    null
                );
                opts.AddScheme<ZenaAuthenticationHandler>(SchemeName.Zena, null);

                opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(opts =>
            {
                opts.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                opts.SlidingExpiration = true;
            });

        return services;
    }

    public static IServiceCollection ConfigureHangfire(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHangfire(
            (serviceProvider, cfg) =>
            {
                PostgresOptions postgresOptions = serviceProvider
                    .GetRequiredService<IOptions<PostgresOptions>>()
                    .Value;

                cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(pgCfg =>
                        pgCfg.UseNpgsqlConnection(postgresOptions.GetConnectionString("Hangfire"))
                    );
            }
        );

        serviceCollection.AddHangfireServer();

        return serviceCollection;
    }
}
