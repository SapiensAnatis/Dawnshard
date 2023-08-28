using DragaliaAPI.Blazor.Authentication;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.Chara;
using DragaliaAPI.Features.ClearParty;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.DmodeDungeon;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Dungeon.Start;
using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Item;
using DragaliaAPI.Features.Login;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.PartyPower;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Features.Stamp;
using DragaliaAPI.Features.Talisman;
using DragaliaAPI.Features.Tickets;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.Helpers;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Services.Photon;
using DragaliaAPI.Shared.PlayerDetails;

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
            .AddScoped<ISummonService, SummonService>()
            .AddScoped<IUpdateDataService, UpdateDataService>()
            .AddScoped<IDragonService, DragonService>()
            .AddScoped<ISavefileService, SavefileService>()
            .AddScoped<IHelperService, HelperService>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IBonusService, BonusService>()
            .AddScoped<IWeaponService, WeaponService>()
            .AddScoped<IStoryService, StoryService>()
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
            // Mission Feature
            .AddScoped<IMissionRepository, MissionRepository>()
            .AddScoped<IMissionService, MissionService>()
            .AddScoped<IRewardService, RewardService>()
            .AddScoped<IMissionProgressionService, MissionProgressionService>()
            .AddScoped<IMissionInitialProgressionService, MissionInitialProgressionService>()
            .AddScoped<IFortMissionProgressionService, FortMissionProgressionService>()
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
            .AddScoped<ITradeService, TradeService>()
            // Fort Feature
            .AddScoped<IFortService, FortService>()
            .AddScoped<IFortRepository, FortRepository>()
            // Login feature
            .AddScoped<IResetHelper, ResetHelper>()
            .AddScoped<IDateTimeProvider, DateTimeProvider>()
            .AddScoped<ILoginBonusService, LoginBonusService>()
            .AddScoped<ILoginBonusRepository, LoginBonusRepository>()
            // Dungeon Feature
            .AddScoped<IDungeonService, DungeonService>()
            .AddScoped<IDungeonStartService, DungeonStartService>()
            .AddScoped<IDungeonRepository, DungeonRepository>()
            .AddScoped<IQuestDropService, QuestDropService>()
            .AddScoped<IQuestEnemyService, QuestEnemyService>()
            .AddScoped<IOddsInfoService, OddsInfoService>()
            .AddScoped<IDungeonRecordService, DungeonRecordService>()
            .AddScoped<IDungeonRecordHelperService, DungeonRecordHelperService>()
            .AddScoped<IDungeonRecordRewardService, DungeonRecordRewardService>()
            .AddScoped<IDungeonRecordDamageService, DungeonRecordDamageService>()
            .AddScoped<IQuestCompletionService, QuestCompletionService>()
            .AddScoped<IAbilityCrestMultiplierService, AbilityCrestMultiplierService>()
            // Event Feature
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IEventService, EventService>()
            .AddScoped<IEventDropService, EventDropService>()
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
            // Tickets feature
            .AddScoped<ITicketRepository, TicketRepository>()
            // Emblem feature
            .AddScoped<IEmblemRepository, EmblemRepository>()
            // Quest feature
            .AddScoped<IQuestService, QuestService>()
            .AddScoped<IQuestCacheService, QuestCacheService>()
            // Party power feature
            .AddScoped<IPartyPowerService, PartyPowerService>()
            .AddScoped<IPartyPowerRepository, PartyPowerRepository>()
            // Chara feature
            .AddScoped<ICharaService, CharaService>();

        services.AddScoped<IBlazorIdentityService, BlazorIdentityService>();

        services.AddAllOfType<ISavefileUpdate>();
        services.AddAllOfType<IDailyResetAction>();

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

        return services;
    }
}
