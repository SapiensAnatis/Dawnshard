using System.Diagnostics;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.PartyPower;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Features.Tickets;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Services.Game;

public class LoadService(
    ISavefileService savefileService,
    IBonusService bonusService,
    IMapper mapper,
    ILogger<LoadService> logger,
    IOptionsMonitor<PhotonOptions> photonOptions,
    IMissionService missionService,
    IPresentService presentService,
    ITradeService tradeService,
    IShopRepository shopRepository,
    IUserService userService,
    ITicketRepository ticketRepository,
    IDateTimeProvider dateTimeProvider
) : ILoadService
{
    public async Task<LoadIndexData> BuildIndexData()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        DbPlayer savefile = await savefileService.Load().SingleAsync();

        logger.LogInformation("{time} ms: Load query complete", stopwatch.ElapsedMilliseconds);

        FortBonusList bonusList = await bonusService.GetBonusList();

        logger.LogInformation("{time} ms: Bonus list acquired", stopwatch.ElapsedMilliseconds);

        // TODO/NOTE: special shop purchase list is not set here. maybe change once that fully works?

        LoadIndexData data =
            new()
            {
                build_list = savefile.BuildList.Select(mapper.Map<BuildList>),
                user_data = mapper.Map<UserData>(savefile.UserData),
                chara_list = savefile.CharaList.Select(mapper.Map<CharaList>),
                dragon_list = savefile.DragonList.Select(mapper.Map<DragonList>),
                dragon_reliability_list = savefile.DragonReliabilityList.Select(
                    mapper.Map<DragonReliabilityList>
                ),
                ability_crest_list = savefile.AbilityCrestList.Select(mapper.Map<AbilityCrestList>),
                dragon_gift_list = savefile.DragonGiftList
                    .Where(x => x.DragonGiftId > DragonGifts.GoldenChalice)
                    .Select(mapper.Map<DragonGiftList>),
                talisman_list = savefile.TalismanList.Select(mapper.Map<TalismanList>),
                weapon_body_list = savefile.WeaponBodyList.Select(mapper.Map<WeaponBodyList>),
                party_list = savefile.PartyList.Select(mapper.Map<PartyList>),
                quest_story_list = savefile.StoryStates
                    .Where(x => x.StoryType == StoryTypes.Quest)
                    .Select(mapper.Map<QuestStoryList>),
                unit_story_list = savefile.StoryStates
                    .Where(x => x.StoryType == StoryTypes.Chara || x.StoryType == StoryTypes.Dragon)
                    .Select(mapper.Map<UnitStoryList>),
                castle_story_list = savefile.StoryStates
                    .Where(x => x.StoryType == StoryTypes.Castle)
                    .Select(mapper.Map<CastleStoryList>),
                quest_list = savefile.QuestList.Select(mapper.Map<QuestList>),
                quest_event_list = savefile.QuestEvents.Select(mapper.Map<QuestEventList>),
                material_list = savefile.MaterialList.Select(mapper.Map<MaterialList>),
                weapon_skin_list = savefile.WeaponSkinList.Select(mapper.Map<WeaponSkinList>),
                weapon_passive_ability_list = savefile.WeaponPassiveAbilityList.Select(
                    mapper.Map<WeaponPassiveAbilityList>
                ),
                fort_bonus_list = bonusList,
                party_power_data = mapper.Map<PartyPowerData>(savefile.PartyPower),
                friend_notice = new(0, 0),
                present_notice = await presentService.GetPresentNotice(),
                guild_notice = new(0, 0, 0, 0, 0),
                //fort_plant_list = buildSummary,
                server_time = dateTimeProvider.UtcNow,
                stamina_multi_system_max = userService.StaminaMultiMax,
                stamina_multi_user_max = 12,
                quest_skip_point_system_max = userService.QuestSkipPointMax,
                quest_skip_point_use_limit_max = 30,
                functional_maintenance_list = new List<FunctionalMaintenanceList>(),
                multi_server = new()
                {
                    host = photonOptions.CurrentValue.ServerUrl,
                    app_id = string.Empty
                },
                mission_notice = await missionService.GetMissionNotice(null),
                equip_stamp_list = savefile.EquippedStampList
                    .Select(mapper.Map<DbEquippedStamp, EquipStampList>)
                    .OrderBy(x => x.slot),
                quest_entry_condition_list = await missionService.GetEntryConditions(),
                user_treasure_trade_list = await tradeService.GetUserTreasureTradeList(),
                treasure_trade_all_list = tradeService.GetCurrentTreasureTradeList(),
                shop_notice = new ShopNotice(await shopRepository.GetDailySummonCountAsync() == 0),
                summon_ticket_list = (await ticketRepository.GetTicketsAsync()).Select(
                    mapper.Map<SummonTicketList>
                ),
                quest_bonus_stack_base_time = 1617775200, // 7. April 2017
                album_dragon_list = Enumerable.Empty<AlbumDragonData>()
            };

        logger.LogInformation("{time} ms: Mapping complete", stopwatch.ElapsedMilliseconds);
        return data;
    }
}
