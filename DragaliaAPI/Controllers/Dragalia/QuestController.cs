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
    private readonly IHelperService helperService;
    private readonly IUpdateDataService updateDataService;
    private const int ReadStoryState = 1;
    private const int MaxStoryId = 1000103;

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

    public QuestController(
        IQuestRepository questRepository,
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IHelperService helperService,
        IUpdateDataService updateDataService
    )
    {
        this.questRepository = questRepository;
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.helperService = helperService;
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
                new() { entity_id = (int)Charas.Elisanne, entity_type = EntityTypes.Chara }
            );
            rewardList.Add(
                new()
                {
                    entity_id = (int)Charas.Elisanne,
                    entity_level = 1,
                    entity_limit_break_count = 0,
                    entity_quantity = 1,
                    entity_type = EntityTypes.Chara
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
}
