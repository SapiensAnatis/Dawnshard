using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("quest")]
[ApiController]
public class QuestController : DragaliaControllerBase
{
    private readonly IStoryService storyService;
    private readonly IHelperService helperService;
    private readonly IQuestRewardService questRewardService;
    private readonly IUpdateDataService updateDataService;
    private readonly ILogger<QuestController> logger;

    public QuestController(
        IStoryService storyService,
        IHelperService helperService,
        IQuestRewardService questRewardService,
        IUpdateDataService updateDataService,
        ILogger<QuestController> logger
    )
    {
        this.storyService = storyService;
        this.helperService = helperService;
        this.questRewardService = questRewardService;
        this.updateDataService = updateDataService;
        this.logger = logger;
    }

    [HttpPost]
    [Route("read_story")]
    public async Task<DragaliaResult> ReadStory(QuestReadStoryRequest request)
    {
        IEnumerable<AtgenBuildEventRewardEntityList> rewardList = await this.storyService.ReadStory(
            StoryTypes.Quest,
            request.quest_story_id
        );

        EntityResult entityResult = StoryService.GetEntityResult(rewardList);
        IEnumerable<AtgenQuestStoryRewardList> questRewardList = rewardList.Select(
            StoryService.ToQuestStoryReward
        );

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return this.Ok(
            new QuestReadStoryData()
            {
                quest_story_reward_list = questRewardList,
                entity_result = entityResult,
                update_data_list = updateDataList
            }
        );
    }

    [HttpPost("get_support_user_list")]
    public async Task<DragaliaResult> GetUserSupportList()
    {
        // TODO: this is actually going to be a pretty complicated system
        QuestGetSupportUserListData response = await this.helperService.GetHelpers();
        return Ok(response);
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

    [HttpPost("drop_list")]
    public DragaliaResult DropList(QuestDropListRequest request)
    {
        IEnumerable<Materials> drops = this.questRewardService.GetDrops(request.quest_id);

        return Ok(
            new QuestDropListData()
            {
                quest_drop_info = new()
                {
                    drop_info_list = drops.Select(
                        x =>
                            new AtgenDuplicateEntityList()
                            {
                                entity_id = (int)x,
                                entity_type = EntityTypes.Material
                            }
                    )
                }
            }
        );
    }

    private static class StubData
    {
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
}
