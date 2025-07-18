﻿using DragaliaAPI.Database;
using DragaliaAPI.Features.Chara;
using DragaliaAPI.Features.ClearParty;
using DragaliaAPI.Features.CoOp;
using DragaliaAPI.Features.CoOp.Stamps;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.DmodeDungeon;
using DragaliaAPI.Features.Dragons;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Dungeon.AutoRepeat;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Dungeon.Start;
using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Friends;
using DragaliaAPI.Features.Item;
using DragaliaAPI.Features.Login.Auth;
using DragaliaAPI.Features.Login.Savefile;
using DragaliaAPI.Features.Login.SavefileUpdate;
using DragaliaAPI.Features.Maintenance;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Parties;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Shared.Options;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Features.Story.Skip;
using DragaliaAPI.Features.Talisman;
using DragaliaAPI.Features.TimeAttack;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.Features.Version;
using DragaliaAPI.Features.Weapons;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Infrastructure.Authentication;
using DragaliaAPI.Infrastructure.Metrics;
using DragaliaAPI.Infrastructure.Middleware;
using DragaliaAPI.Models.Generated;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using static DragaliaAPI.Infrastructure.Authentication.AuthConstants;
using AuthService = DragaliaAPI.Features.Login.Auth.AuthService;
using BaasApi = DragaliaAPI.Features.Shared.BaasApi;
using DragonService = DragaliaAPI.Features.Dragons.DragonService;

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
            .AddScoped<IUpdateDataService, UpdateDataService>()
            .AddScoped<IDragonService, DragonService>()
            .AddScoped<ISavefileService, SavefileService>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IBonusService, BonusService>()
            .AddScoped<IWeaponService, WeaponService>()
            .AddScoped<IMatchingService, MatchingService>()
            .AddScoped<IHeroParamService, HeroParamService>()
            .AddScoped<ILoadService, LoadService>()
            .AddScoped<IStampService, StampService>()
            .AddScoped<IStampRepository, StampRepository>()
            .AddScoped<ISavefileUpdateService, SavefileUpdateService>()
            .AddTransient<IdentityLogContextMiddleware>()
            .AddTransient<HeaderLogContextMiddleware>()
            .AddTransient<ResultCodeLoggingMiddleware>();

        services
            .AddSummoningFeature()
            .AddRewardFeature()
            .AddLoginFeature()
            .AddWallFeature()
            .AddPresentFeature()
            .AddQuestFeature()
            .AddStoryFeature()
            .AddWebFeature()
            .AddAbilityCrestFeature()
            .AddTutorialFeature()
            .AddZenaFeature()
            .AddFriendFeature()
            .AddDungeonFeature();

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
            // Event Feature
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IEventService, EventService>()
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
            // Party power feature
            .AddScoped<IPartyPowerService, PartyPowerService>()
            .AddScoped<IPartyPowerRepository, PartyPowerRepository>()
            // Chara feature
            .AddScoped<ICharaService, CharaService>()
            .AddScoped<IResourceVersionService, ResourceVersionService>()
            .AddScoped<ICharaService, CharaService>()
            // Story skip feature
            .AddScoped<StorySkipService>()
            // Maintenance feature
            .AddScoped<MaintenanceService>();

        services.AddAllOfType<ISavefileUpdate>();

        services.AddHttpClient<IBaasApi, BaasApi>(
            (sp, client) =>
            {
                IOptionsMonitor<BaasOptions> options = sp.GetRequiredService<
                    IOptionsMonitor<BaasOptions>
                >();

                client.BaseAddress = options.CurrentValue.BaasUrlParsed;
            }
        );

        services.AddHttpClient<IPhotonStateApi, PhotonStateApi>(
            (sp, client) =>
            {
                IOptionsMonitor<PhotonOptions> options = sp.GetRequiredService<
                    IOptionsMonitor<PhotonOptions>
                >();

                client.BaseAddress = new(options.CurrentValue.StateManagerUrl);
            }
        );
        services.AddScoped<IMatchingService, MatchingService>();

        services
            .AddScoped<ResourceVersionActionFilter>()
            .AddScoped<MaintenanceActionFilter>()
            .AddScoped<SetResultCodeActionFilter>();

        return services;
    }

    public static IServiceCollection ConfigureGameOptions(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services
            .Configure<BaasOptions>(config.GetRequiredSection("Baas"))
            .Configure<DragalipatchOptions>(config.GetRequiredSection("Dragalipatch"))
            .Configure<RedisCachingOptions>(config.GetRequiredSection(nameof(RedisCachingOptions)))
            .Configure<PhotonOptions>(config.GetRequiredSection(nameof(PhotonOptions)))
            .Configure<ItemSummonConfig>(config)
            .Configure<DragonfruitConfig>(config)
            .Configure<TimeAttackOptions>(config.GetRequiredSection(nameof(TimeAttackOptions)))
            .Configure<ResourceVersionOptions>(
                config.GetRequiredSection(nameof(ResourceVersionOptions))
            )
            .Configure<EventOptions>(config.GetRequiredSection(nameof(EventOptions)))
            .Configure<MaintenanceOptions>(config.GetRequiredSection(nameof(MaintenanceOptions)));

        services.AddTransient<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

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
                opts.AddScheme<SessionAuthenticationHandler>(SchemeNames.Session, null);
                opts.AddScheme<DeveloperAuthenticationHandler>(SchemeNames.Developer, null);
                opts.AddScheme<PhotonAuthenticationHandler>(
                    nameof(PhotonAuthenticationHandler),
                    null
                );
            })
            .AddJwtBearer(
                SchemeNames.GameJwt,
                options =>
                {
                    options.Events = new()
                    {
                        OnMessageReceived = GameJwtAuthenticationCallbacks.OnMessageReceived,
                        OnTokenValidated = GameJwtAuthenticationCallbacks.OnTokenValidated,
                        OnChallenge = GameJwtAuthenticationCallbacks.OnChallenge,
                    };
                    // Other options configured in ConfigureJwtBearerOptions.cs after the ServiceProvider is built.
                }
            );

        return services;
    }

    public static IServiceCollection ConfigureHangfire(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHangfire(
            (serviceProvider, cfg) =>
            {
                IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

                cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(pgCfg =>
                        pgCfg.UseNpgsqlConnection(configuration.GetConnectionString("postgres"))
                    );
            }
        );

        serviceCollection.AddHangfireServer();

        return serviceCollection;
    }

    public static WebApplicationBuilder ConfigureObservability(this WebApplicationBuilder builder)
    {
        // Custom config on top of ServiceDefaults

        builder
            .Services.AddOpenTelemetry()
            .ConfigureResource(cfg =>
            {
                cfg.AddService(serviceName: "dragalia-api", autoGenerateServiceInstanceId: false);
            });

        if (builder.HasOtlpTracesEndpoint())
        {
            builder
                .Services.AddOpenTelemetry()
                .WithTracing(tracing =>
                    tracing.AddEntityFrameworkCoreInstrumentation(options =>
                        options.SetDbStatementForText = true
                    )
                //  Not compatible with IDistributedCache as requires IConnectionMultiplexer
                // .AddRedisInstrumentation()
                );
        }

        builder.Services.AddSingleton<IDragaliaApiMetrics, DragaliaApiMetrics>();
        builder
            .Services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics.AddMeter(DragaliaApiMetrics.MeterName));

        return builder;
    }
}
