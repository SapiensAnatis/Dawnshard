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

namespace DragaliaAPI.Controllers.Dragalia;

[Route("load")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class LoadController : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IPartyRepository partyRepository;
    private readonly IQuestRepository questRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IFortRepository fortRepository;
    private readonly IMapper mapper;

    public LoadController(
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IPartyRepository partyRepository,
        IQuestRepository questRepository,
        IInventoryRepository inventoryRepository,
        IFortRepository fortRepository,
        IMapper mapper
    )
    {
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.partyRepository = partyRepository;
        this.questRepository = questRepository;
        this.inventoryRepository = inventoryRepository;
        this.fortRepository = fortRepository;
        this.mapper = mapper;
    }

#if !TEST
    [Route("index")]
    [HttpPost]
    public async Task<DragaliaResult> Index()
    {
        UserData userData = mapper.Map<UserData>(
            await this.userDataRepository.GetUserData(this.DeviceAccountId).SingleAsync()
        );

        IEnumerable<CharaList> charas = (
            await this.unitRepository.GetAllCharaData(this.DeviceAccountId).ToListAsync()
        ).Select(mapper.Map<CharaList>);

        IEnumerable<DragonList> dragons = (
            await this.unitRepository.GetAllDragonData(this.DeviceAccountId).ToListAsync()
        ).Select(mapper.Map<DragonList>);

        IEnumerable<DragonReliabilityList> dragonReliabilities = (
            await this.unitRepository
                .GetAllDragonReliabilityData(this.DeviceAccountId)
                .ToListAsync()
        ).Select(mapper.Map<DragonReliabilityList>);

        IEnumerable<AbilityCrestList> crests = (
            await this.unitRepository.GetAllAbilityCrestData(this.DeviceAccountId).ToListAsync()
        ).Select(mapper.Map<AbilityCrestList>);

        IEnumerable<WeaponBodyList> weapons = (
            await this.unitRepository.GetAllWeaponBodyData(this.DeviceAccountId).ToListAsync()
        ).Select(mapper.Map<WeaponBodyList>);

        IEnumerable<PartyList> parties = (
            await this.partyRepository.GetParties(this.DeviceAccountId).ToListAsync()
        )
            .Select(mapper.Map<PartyList>)
            .Select(
                x =>
                    new PartyList()
                    {
                        party_name = x.party_name,
                        party_no = x.party_no,
                        party_setting_list = x.party_setting_list.OrderBy(x => x.unit_no)
                    }
            );

        IEnumerable<QuestStoryList> questStories = (
            await this.questRepository
                .GetStories(this.DeviceAccountId, StoryTypes.Quest)
                .ToListAsync()
        ).Select(mapper.Map<QuestStoryList>);

        IEnumerable<CastleStoryList> castleStories = (
            await this.questRepository
                .GetStories(this.DeviceAccountId, StoryTypes.Castle)
                .ToListAsync()
        ).Select(mapper.Map<CastleStoryList>);

        IEnumerable<UnitStoryList> unitStories = (
            await this.questRepository
                .GetStories(this.DeviceAccountId, StoryTypes.Chara)
                .ToListAsync()
        ).Select(mapper.Map<UnitStoryList>);

        IEnumerable<QuestList> quests = (
            await this.questRepository.GetQuests(this.DeviceAccountId).ToListAsync()
        ).Select(mapper.Map<QuestList>);

        IEnumerable<MaterialList> materials = (
            await this.inventoryRepository.GetMaterials(this.DeviceAccountId).ToListAsync()
        ).Select(mapper.Map<MaterialList>);

        IEnumerable<TalismanList> talismans = (
            await this.unitRepository.GetAllTalismanData(this.DeviceAccountId).ToListAsync()
        ).Select(mapper.Map<TalismanList>);

        IEnumerable<BuildList> buildDetails = (
            await this.fortRepository.GetBuilds(this.DeviceAccountId).ToListAsync()
        ).Select(mapper.Map<BuildList>);
        //IEnumerable<FortPlantList> buildSummary = new FortPlants[] { FortPlants.RupieMine, FortPlants.FlameAltar, , 100403, 100404, 100405, 100701, 100702, 100703, 100704, 100705 }

        LoadIndexData data =
            new()
            {
                user_data = userData,
                chara_list = charas,
                dragon_list = dragons,
                dragon_reliability_list = dragonReliabilities,
                ability_crest_list = crests,
                talisman_list = talismans,
                weapon_body_list = weapons,
                party_list = parties,
                quest_story_list = questStories,
                unit_story_list = unitStories,
                castle_story_list = castleStories,
                quest_list = quests,
                material_list = materials,
                party_power_data = new(999999),
                friend_notice = new(0, 0),
                present_notice = new(0, 0),
                guild_notice = new(0, 0, 0, 0, 0),
                build_list = buildDetails,
                //fort_plant_list = buildSummary,
                shop_notice = new ShopNotice(0),
                server_time = DateTimeOffset.UtcNow,
                stamina_multi_system_max = 99,
                stamina_multi_user_max = 12,
                quest_skip_point_system_max = 400,
                quest_skip_point_use_limit_max = 30,
                functional_maintenance_list = new List<FunctionalMaintenanceList>(),
            };

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
