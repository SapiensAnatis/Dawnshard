using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using DragaliaAPI.Services;
using MessagePack.Resolvers;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia.Load;

[Route("load/index")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class IndexController : ControllerBase
{
    private readonly IApiRepository _apiRepository;
    private readonly ISessionService _sessionService;

    public IndexController(IApiRepository apiRepository, ISessionService sessionService)
    {
        _apiRepository = apiRepository;
        _sessionService = sessionService;
    }

#if !TEST
    [HttpPost]
    public async Task<DragaliaResult> Post([FromHeader(Name = "SID")] string sessionId)
    {
        string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);

        DbPlayerUserData dbUserData = await _apiRepository
            .GetPlayerInfo(deviceAccountId)
            .SingleAsync();
        IEnumerable<DbPlayerCharaData> dbCharaData = await _apiRepository
            .GetCharaData(deviceAccountId)
            .ToListAsync();
        IEnumerable<DbPlayerDragonData> dbDragonData = await _apiRepository
            .GetDragonData(deviceAccountId)
            .ToListAsync();
        IEnumerable<DbParty> dbParties = await _apiRepository
            .GetParties(deviceAccountId)
            .ToListAsync();
        IEnumerable<DbPlayerMaterial> dbMaterials = await _apiRepository
            .GetMaterials(deviceAccountId)
            .ToListAsync();

        UserData userData = SavefileUserDataFactory.Create(dbUserData);
        IEnumerable<Chara> charas = dbCharaData.Select(CharaFactory.Create);
        IEnumerable<Dragon> dragons = dbDragonData.Select(DragonFactory.Create);
        IEnumerable<Party> parties = dbParties.Select(PartyFactory.CreateDto);
        IEnumerable<Material> materials = dbMaterials.Select(MaterialFactory.Create);

        LoadIndexData data = new LoadIndexData(
            userData,
            charas,
            dragons,
            parties,
            materials,
            new List<object>()
        );

        LoadIndexResponse response = new(data);

        return Ok(response);
    }
#endif

    // Testing method: returns preset savefile to check what properties are needed
#if TEST
    [HttpPost]
    [Produces("application/octet-stream")]
    public ActionResult<object> Post()
    {
        byte[] blob = System.IO.File.ReadAllBytes("Resources/new_savefile");
        dynamic preset_savefile = MessagePackSerializer.Deserialize<dynamic>(
            blob,
            ContractlessStandardResolver.Options
        );

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
        preset_savefile["data"].Remove("quest_story_list");
        preset_savefile["data"].Remove("quest_treasure_list");
        preset_savefile["data"].Remove("quest_carry_list");
        preset_savefile["data"].Remove("quest_entry_condition_list");
        preset_savefile["data"].Remove("summon_ticket_list");
        preset_savefile["data"].Remove("summon_point_list");
        preset_savefile["data"].Remove("present_notice");
        preset_savefile["data"].Remove("friend_notice");
        preset_savefile["data"].Remove("mission_notice");
        preset_savefile["data"].Remove("current_main_story_mission");
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
        //preset_savefile["data"].Remove("ability_crest_list"); // Needed for teams
        preset_savefile["data"].Remove("exchange_ticket_list");
        preset_savefile["data"].Remove("album_dragon_list");
        preset_savefile["data"].Remove("talisman_list");
        preset_savefile["data"].Remove("user_summon_list");
        preset_savefile["data"].Remove("server_time");
        preset_savefile["data"].Remove("stamina_multi_user_max");
        preset_savefile["data"].Remove("stamina_multi_system_max");
        preset_savefile["data"].Remove("quest_bonus_stack_base_time");
        preset_savefile["data"].Remove("spec_upgrade_time");
        preset_savefile["data"].Remove("quest_skip_point_use_limit_max");
        preset_savefile["data"].Remove("quest_skip_point_system_max");
        preset_savefile["data"].Remove("multi_server");
        preset_savefile["data"].Remove("walker_data");
        preset_savefile["data"].Remove("update_data_list");

        return Ok(preset_savefile);
    }
#endif
}
