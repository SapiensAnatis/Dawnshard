//#define TEST

using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models;
using MessagePack;
using MessagePack.Resolvers;
using DragaliaAPI.Shared.Definitions.Enums;
using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using System;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("load")]
public class LoadController : DragaliaControllerBase
{
    private readonly ISavefileService savefileService;
    private readonly IBonusService bonusService;
    private readonly IMapper mapper;
    private readonly ILogger<LoadController> logger;

    public LoadController(
        ISavefileService savefileService,
        IBonusService bonusService,
        IMapper mapper,
        ILogger<LoadController> logger
    )
    {
        this.savefileService = savefileService;
        this.bonusService = bonusService;
        this.mapper = mapper;
        this.logger = logger;
    }

#if !TEST
    [Route("index")]
    [HttpPost]
    public async Task<DragaliaResult> Index()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        DbPlayer savefile = await this.savefileService.Load(this.DeviceAccountId).SingleAsync();

        this.logger.LogInformation("{time} ms: Load query complete", stopwatch.ElapsedMilliseconds);

        FortBonusList bonusList = await bonusService.GetBonusList();

        this.logger.LogInformation("{time} ms: Bonus list acquired", stopwatch.ElapsedMilliseconds);

        LoadIndexData data =
            new()
            {
                build_list = savefile.BuildList.Select(this.mapper.Map<BuildList>),
                user_data = this.mapper.Map<UserData>(savefile.UserData),
                chara_list = savefile.CharaList.Select(this.mapper.Map<CharaList>),
                dragon_list = savefile.DragonList.Select(this.mapper.Map<DragonList>),
                dragon_reliability_list = savefile.DragonReliabilityList.Select(
                    this.mapper.Map<DragonReliabilityList>
                ),
                ability_crest_list = savefile.AbilityCrestList.Select(
                    this.mapper.Map<AbilityCrestList>
                ),
                dragon_gift_list = savefile.DragonGiftList
                    .Where(x => x.DragonGiftId > DragonGifts.GoldenChalice)
                    .Select(this.mapper.Map<DragonGiftList>),
                talisman_list = savefile.TalismanList.Select(this.mapper.Map<TalismanList>),
                weapon_body_list = savefile.WeaponBodyList.Select(this.mapper.Map<WeaponBodyList>),
                party_list = savefile.PartyList.Select(this.mapper.Map<PartyList>),
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
                material_list = savefile.MaterialList.Select(mapper.Map<MaterialList>),
                weapon_skin_list = savefile.WeaponSkinList.Select(mapper.Map<WeaponSkinList>),
                weapon_passive_ability_list = savefile.WeaponPassiveAbilityList.Select(
                    mapper.Map<WeaponPassiveAbilityList>
                ),
                fort_bonus_list = bonusList,
                party_power_data = new(999999),
                friend_notice = new(0, 0),
                present_notice = new(0, 0),
                guild_notice = new(0, 0, 0, 0, 0),
                //fort_plant_list = buildSummary,
                shop_notice = new ShopNotice(0),
                server_time = DateTimeOffset.UtcNow,
                stamina_multi_system_max = 99,
                stamina_multi_user_max = 12,
                quest_skip_point_system_max = 400,
                quest_skip_point_use_limit_max = 30,
                functional_maintenance_list = new List<FunctionalMaintenanceList>(),
                multi_server = new()
                {
                    host = string.Empty,
                    app_id = "4a84ccbe-ecd3-42b4-9570-4a8aa526a139"
                }
            };

        this.logger.LogInformation("{time} ms: Mapping complete", stopwatch.ElapsedMilliseconds);

        return this.Ok(data);
    }
#endif

    // Testing method: returns preset savefile to check what properties are needed
#if TEST
    [Route("index")]
    [HttpPost]
    public ActionResult<object> Index()
    {
        byte[] blob = System.IO.File.ReadAllBytes("Resources/endgame_savefile");
        dynamic preset_savefile = MessagePackSerializer.Deserialize<dynamic>(
            blob,
            ContractlessStandardResolver.Options
        );
        /*
                // Comment out properties that should be kept
                preset_savefile["data"].Remove("quest_bonus");
                preset_savefile["data"].Remove("special_shop_purchase");
                preset_savefile["data"].Remove("user_treasure_trade_list");
                preset_savefile["data"].Remove("treasure_trade_all_list");
                //preset_savefile["data"].Remove("user_data"); // Existing implementation
                preset_savefile["data"].Remove("party_power_data");
                //preset_savefile["data"].Remove("party_list"); // Existing implementation
                //preset_savefile["data"].Remove("chara_list"); // Existing implementation
                //preset_savefile["data"].Remove("dragon_list"); // Existing implementation
                preset_savefile["data"].Remove("dragon_gift_list");
                preset_savefile["data"].Remove("dragon_reliability_list");
                preset_savefile["data"].Remove("material_list");
                preset_savefile["data"].Remove("fort_bonus_list");
                preset_savefile["data"].Remove("fort_plant_list");
                preset_savefile["data"].Remove("build_list");
                preset_savefile["data"].Remove("equip_stamp_list");
                preset_savefile["data"].Remove("unit_story_list");
                preset_savefile["data"].Remove("castle_story_list");
                preset_savefile["data"].Remove("quest_list");
                preset_savefile["data"].Remove("quest_event_list");
                //preset_savefile["data"].Remove("quest_story_list");
                preset_savefile["data"].Remove("quest_treasure_list");
                preset_savefile["data"].Remove("quest_carry_list");
                preset_savefile["data"].Remove("quest_entry_condition_list");
                preset_savefile["data"].Remove("summon_ticket_list");
                preset_savefile["data"].Remove("summon_point_list");
                preset_savefile["data"].Remove("present_notice");
                preset_savefile["data"].Remove("friend_notice");
                preset_savefile["data"].Remove("mission_notice");
                //preset_savefile["data"].Remove("current_main_story_mission");
                preset_savefile["data"].Remove("guild_notice");
                preset_savefile["data"].Remove("shop_notice");
                preset_savefile["data"].Remove("album_passive_notice");
                preset_savefile["data"].Remove("functional_maintenance_list");
                preset_savefile["data"].Remove("quest_wall_list");
                preset_savefile["data"].Remove("astral_item_list");
                preset_savefile["data"].Remove("user_guild_data");
                preset_savefile["data"].Remove("guild_data");
                preset_savefile["data"].Remove("lottery_ticket_list");
                preset_savefile["data"].Remove("gather_item_list");
                preset_savefile["data"].Remove("weapon_skin_list");
                preset_savefile["data"].Remove("weapon_body_list");
                preset_savefile["data"].Remove("weapon_passive_ability_list");
                //preset_savefile["data"].Remove("ability_crest_list"); // Existing implementation (sort of)
                preset_savefile["data"].Remove("exchange_ticket_list");
                preset_savefile["data"].Remove("album_dragon_list");
                preset_savefile["data"].Remove("talisman_list");
                preset_savefile["data"].Remove("user_summon_list");
                //preset_savefile["data"].Remove("server_time"); //1
                //preset_savefile["data"].Remove("stamina_multi_user_max"); //1
                //preset_savefile["data"].Remove("stamina_multi_system_max"); //1
                //preset_savefile["data"].Remove("quest_bonus_stack_base_time");
                //preset_savefile["data"].Remove("spec_upgrade_time"); //1
                preset_savefile["data"].Remove("quest_skip_point_use_limit_max");
                preset_savefile["data"].Remove("quest_skip_point_system_max");
                preset_savefile["data"].Remove("multi_server");
                preset_savefile["data"].Remove("walker_data");
                preset_savefile["data"].Remove("update_data_list");
        */
        //preset_savefile["data"]["user_data"]["tutorial_status"] = 10301;

        return preset_savefile;
    }
#endif
}
