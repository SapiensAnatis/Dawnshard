using System.Diagnostics;
using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Services.Game;

public class LoadService(
    ISavefileService savefileService,
    ApiContext apiContext,
    IBonusService bonusService,
    IMapper mapper,
    ILogger<LoadService> logger,
    IOptionsMonitor<PhotonOptions> photonOptions,
    IMissionService missionService,
    IPresentService presentService,
    ITradeService tradeService,
    IShopRepository shopRepository,
    IUserService userService,
    IWallService wallService,
    TimeProvider timeProvider
) : ILoadService
{
    public async Task<LoadIndexResponse> BuildIndexData()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        DbPlayer savefile = await savefileService.Load().AsNoTracking().FirstAsync();

        logger.LogInformation("{time} ms: Load query complete", stopwatch.ElapsedMilliseconds);

        FortBonusList bonusList = await bonusService.GetBonusList();

        logger.LogInformation("{time} ms: Bonus list acquired", stopwatch.ElapsedMilliseconds);

        // TODO/NOTE: special shop purchase list is not set here. maybe change once that fully works?

        LoadIndexResponse data =
            new()
            {
                BuildList = savefile.BuildList.Select(mapper.Map<BuildList>),
                UserData = mapper.Map<UserData>(savefile.UserData),
                CharaList = savefile.CharaList.Select(mapper.Map<CharaList>),
                DragonList = savefile.DragonList.Select(mapper.Map<DragonList>),
                DragonReliabilityList = savefile.DragonReliabilityList.Select(
                    mapper.Map<DragonReliabilityList>
                ),
                AbilityCrestList = savefile.AbilityCrestList.Select(mapper.Map<AbilityCrestList>),
                DragonGiftList = savefile
                    .DragonGiftList.Where(x => x.DragonGiftId > DragonGifts.GoldenChalice)
                    .Select(mapper.Map<DragonGiftList>),
                TalismanList = savefile.TalismanList.Select(mapper.Map<TalismanList>),
                WeaponBodyList = savefile.WeaponBodyList.Select(mapper.Map<WeaponBodyList>),
                PartyList = savefile.PartyList.Select(mapper.Map<PartyList>),
                QuestStoryList = savefile
                    .StoryStates.Where(x => x.StoryType == StoryTypes.Quest)
                    .Select(mapper.Map<QuestStoryList>),
                UnitStoryList = savefile
                    .StoryStates.Where(x =>
                        x.StoryType == StoryTypes.Chara || x.StoryType == StoryTypes.Dragon
                    )
                    .Select(mapper.Map<UnitStoryList>),
                CastleStoryList = savefile
                    .StoryStates.Where(x => x.StoryType == StoryTypes.Castle)
                    .Select(mapper.Map<CastleStoryList>),
                QuestList = savefile.QuestList.Select(mapper.Map<QuestList>),
                QuestEventList = savefile.QuestEvents.Select(mapper.Map<QuestEventList>),
                QuestTreasureList = savefile.QuestTreasureList.Select(
                    mapper.Map<QuestTreasureList>
                ),
                MaterialList = savefile.MaterialList.Select(mapper.Map<MaterialList>),
                WeaponSkinList = savefile.WeaponSkinList.Select(mapper.Map<WeaponSkinList>),
                WeaponPassiveAbilityList = savefile.WeaponPassiveAbilityList.Select(
                    mapper.Map<WeaponPassiveAbilityList>
                ),
                FortBonusList = bonusList,
                PartyPowerData = mapper.Map<PartyPowerData>(savefile.PartyPower),
                FriendNotice = new(0, 0),
                PresentNotice = await presentService.GetPresentNotice(),
                GuildNotice = new(0, false, false, false, false),
                //fort_plant_list = buildSummary,
                ServerTime = timeProvider.GetUtcNow(),
                StaminaMultiSystemMax = userService.StaminaMultiMax,
                StaminaMultiUserMax = 12,
                QuestSkipPointSystemMax = userService.QuestSkipPointMax,
                QuestSkipPointUseLimitMax = 30,
                FunctionalMaintenanceList = new List<FunctionalMaintenanceList>(),
                MultiServer = new()
                {
                    Host = photonOptions.CurrentValue.ServerUrl,
                    AppId = string.Empty
                },
                MissionNotice = await missionService.GetMissionNotice(null),
                EquipStampList = savefile
                    .EquippedStampList.Select(mapper.Map<DbEquippedStamp, EquipStampList>)
                    .OrderBy(x => x.Slot),
                QuestEntryConditionList = await missionService.GetEntryConditions(),
                UserTreasureTradeList = await tradeService.GetUserTreasureTradeList(),
                TreasureTradeAllList = tradeService.GetCurrentTreasureTradeList(),
                ShopNotice = new ShopNotice(await shopRepository.GetDailySummonCountAsync() == 0),
                SummonTicketList = await apiContext
                    .PlayerSummonTickets.ProjectToSummonTicketList()
                    .ToListAsync(),
                QuestBonusStackBaseTime = new DateTimeOffset(
                    2021,
                    04,
                    07,
                    06,
                    00,
                    00,
                    TimeSpan.Zero
                ), // 7. April 2017
                AlbumDragonList = Enumerable.Empty<AlbumDragonData>(),
                QuestWallList = await wallService.GetQuestWallList()
            };

        logger.LogInformation("{time} ms: Mapping complete", stopwatch.ElapsedMilliseconds);
        return data;
    }
}
