//#define TEST

using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("load")]
public class LoadController : DragaliaControllerBase
{
    private readonly ILoadService loadService;
    private readonly ISavefileUpdateService savefileUpdateService;

    public LoadController(ILoadService loadService, ISavefileUpdateService savefileUpdateService)
    {
        this.loadService = loadService;
        this.savefileUpdateService = savefileUpdateService;
    }

#if !TEST
    [Route("index")]
    [HttpPost]
    public async Task<DragaliaResult> Index()
    {
        await this.savefileUpdateService.UpdateSavefile();

        LoadIndexData data = await loadService.BuildIndexData();
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
