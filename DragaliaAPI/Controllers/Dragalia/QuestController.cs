using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("quest")]
[Consumes("application/octet-stream")]
[Produces("application/x-msgpack")]
[ApiController]
public class QuestController : DragaliaControllerBase
{
    private readonly IQuestRepository questRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IUpdateDataService updateDataService;
    private const int ReadStoryState = 1;
    private const int MaxStoryId = 1000103;

    private static class StubData
    {
        public static QuestGetSupportUserListData SupportUserData =
            new()
            {
                support_user_list = new List<UserSupportList>()
                {
                    new()
                    {
                        viewer_id = 100,
                        name = "dreadfullydistinct",
                        level = 400,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 40000002,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.DragonyuleIlia,
                            level = 10,
                            additional_max_level = 0,
                            rarity = 5,
                            hp = 60,
                            attack = 40,
                            hp_plus_count = 0,
                            attack_plus_count = 0,
                            ability_1_level = 0,
                            ability_2_level = 0,
                            ability_3_level = 0,
                            ex_ability_level = 1,
                            ex_ability_2_level = 1,
                            skill_1_level = 1,
                            skill_2_level = 0,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new() { dragon_key_id = 0, },
                        support_weapon_body = new() { weapon_body_id = 0, },
                        support_talisman = new() { talisman_key_id = 0, },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new() { ability_crest_id = 0 },
                            new() { ability_crest_id = 0 },
                            new() { ability_crest_id = 0 },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new() { ability_crest_id = 0 },
                            new() { ability_crest_id = 0 },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new() { ability_crest_id = 0 },
                            new() { ability_crest_id = 0 },
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    }
                },
                support_user_detail_list = new List<AtgenSupportUserDetailList>()
                {
                    new()
                    {
                        viewer_id = 100,
                        gettable_mana_point = 25,
                        is_friend = 0,
                    }
                }
            };

        public static QuestGetQuestClearPartyData ClearPartyData =
            new()
            {
                quest_clear_party_setting_list = new List<PartySettingList>()
                {
                    new()
                    {
                        unit_no = 1,
                        chara_id = Charas.ThePrince,
                        equip_dragon_key_id = 0,
                        equip_weapon_body_id = 0,
                        equip_weapon_skin_id = 0,
                        equip_crest_slot_type_1_crest_id_1 = 0,
                        equip_crest_slot_type_1_crest_id_2 = 0,
                        equip_crest_slot_type_1_crest_id_3 = 0,
                        equip_crest_slot_type_2_crest_id_1 = 0,
                        equip_crest_slot_type_2_crest_id_2 = 0,
                        equip_crest_slot_type_3_crest_id_1 = 0,
                        equip_crest_slot_type_3_crest_id_2 = 0,
                        equip_talisman_key_id = 0,
                        edit_skill_1_chara_id = 0,
                        edit_skill_2_chara_id = 0,
                    },
                    new()
                    {
                        unit_no = 2,
                        chara_id = Charas.Empty,
                        equip_dragon_key_id = 0,
                        equip_weapon_body_id = 0,
                        equip_weapon_skin_id = 0,
                        equip_crest_slot_type_1_crest_id_1 = 0,
                        equip_crest_slot_type_1_crest_id_2 = 0,
                        equip_crest_slot_type_1_crest_id_3 = 0,
                        equip_crest_slot_type_2_crest_id_1 = 0,
                        equip_crest_slot_type_2_crest_id_2 = 0,
                        equip_crest_slot_type_3_crest_id_1 = 0,
                        equip_crest_slot_type_3_crest_id_2 = 0,
                        equip_talisman_key_id = 0,
                        edit_skill_1_chara_id = 0,
                        edit_skill_2_chara_id = 0,
                    },
                    new()
                    {
                        unit_no = 3,
                        chara_id = Charas.Empty,
                        equip_dragon_key_id = 0,
                        equip_weapon_body_id = 0,
                        equip_weapon_skin_id = 0,
                        equip_crest_slot_type_1_crest_id_1 = 0,
                        equip_crest_slot_type_1_crest_id_2 = 0,
                        equip_crest_slot_type_1_crest_id_3 = 0,
                        equip_crest_slot_type_2_crest_id_1 = 0,
                        equip_crest_slot_type_2_crest_id_2 = 0,
                        equip_crest_slot_type_3_crest_id_1 = 0,
                        equip_crest_slot_type_3_crest_id_2 = 0,
                        equip_talisman_key_id = 0,
                        edit_skill_1_chara_id = 0,
                        edit_skill_2_chara_id = 0,
                    },
                    new()
                    {
                        unit_no = 4,
                        chara_id = Charas.Empty,
                        equip_dragon_key_id = 0,
                        equip_weapon_body_id = 0,
                        equip_weapon_skin_id = 0,
                        equip_crest_slot_type_1_crest_id_1 = 0,
                        equip_crest_slot_type_1_crest_id_2 = 0,
                        equip_crest_slot_type_1_crest_id_3 = 0,
                        equip_crest_slot_type_2_crest_id_1 = 0,
                        equip_crest_slot_type_2_crest_id_2 = 0,
                        equip_crest_slot_type_3_crest_id_1 = 0,
                        equip_crest_slot_type_3_crest_id_2 = 0,
                        equip_talisman_key_id = 0,
                        edit_skill_1_chara_id = 0,
                        edit_skill_2_chara_id = 0,
                    },
                },
                lost_unit_list = new List<AtgenLostUnitList>(),
            };
    }

    public QuestController(
        IQuestRepository questRepository,
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IUpdateDataService updateDataService
    )
    {
        this.questRepository = questRepository;
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.updateDataService = updateDataService;
    }

    [HttpPost]
    [Route("read_story")]
    public async Task<DragaliaResult> ReadStory(QuestReadStoryRequest request)
    {
        await this.questRepository.UpdateQuestStory(
            this.DeviceAccountId,
            request.quest_story_id,
            ReadStoryState
        );

        List<AtgenDuplicateEntityList> newGetEntityList = new();
        List<AtgenQuestStoryRewardList> rewardList = new();

        if (request.quest_story_id == MaxStoryId)
        {
            await this.userDataRepository.SkipTutorial(this.DeviceAccountId);

            await this.unitRepository.AddCharas(
                this.DeviceAccountId,
                new List<Charas>() { Charas.Elisanne }
            );

            newGetEntityList.Add(
                new() { entity_id = (int)Charas.Elisanne, entity_type = (int)EntityTypes.Chara }
            );
            rewardList.Add(
                new()
                {
                    entity_id = (int)Charas.Elisanne,
                    entity_level = 1,
                    entity_limit_break_count = 0,
                    entity_quantity = 1,
                    entity_type = (int)EntityTypes.Chara
                }
            );
        }

        UpdateDataList updateData = this.updateDataService.GetUpdateDataList(this.DeviceAccountId);

        await this.questRepository.SaveChangesAsync();

        QuestReadStoryData responseData =
            new(
                updateData,
                new() { new_get_entity_list = newGetEntityList },
                quest_story_reward_list: rewardList,
                new List<ConvertedEntityList>()
            );

        return this.Ok(responseData);
    }

    [HttpPost("get_support_user_list")]
    public DragaliaResult GetUserSupportList()
    {
        // TODO: this is actually going to be a pretty complicated system
        return Ok(StubData.SupportUserData);
    }

    [HttpPost("get_quest_clear_party")]
    public DragaliaResult GetQuestClearParty()
    {
        // TODO: Retrieve from database
        return Ok(StubData.ClearPartyData);
    }

    [HttpPost("set_quest_clear_party")]
    public DragaliaResult SetQuestClearParty()
    {
        // TODO: Store in database
        return Ok(new QuestSetQuestClearPartyData() { result = 1 });
    }
}
