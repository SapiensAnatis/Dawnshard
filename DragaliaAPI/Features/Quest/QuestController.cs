using DragaliaAPI.Controllers;
using DragaliaAPI.Features.ClearParty;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Quest;

[Route("quest")]
[ApiController]
public class QuestController : DragaliaControllerBase
{
    private readonly IStoryService storyService;
    private readonly IHelperService helperService;
    private readonly IQuestDropService questRewardService;
    private readonly IUpdateDataService updateDataService;
    private readonly IClearPartyService clearPartyService;
    private readonly IRewardService rewardService;
    private readonly ILogger<QuestController> logger;

    public QuestController(
        IStoryService storyService,
        IHelperService helperService,
        IQuestDropService questRewardService,
        IUpdateDataService updateDataService,
        IClearPartyService clearPartyService,
        IRewardService rewardService,
        ILogger<QuestController> logger
    )
    {
        this.storyService = storyService;
        this.helperService = helperService;
        this.questRewardService = questRewardService;
        this.updateDataService = updateDataService;
        this.clearPartyService = clearPartyService;
        this.rewardService = rewardService;
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
    public async Task<DragaliaResult> GetQuestClearParty(QuestGetQuestClearPartyRequest request)
    {
        (IEnumerable<PartySettingList> clearParty, IEnumerable<AtgenLostUnitList> lostUnitList) =
            await this.clearPartyService.GetQuestClearParty(request.quest_id, false);

        await this.updateDataService.SaveChangesAsync(); // Updated lost entities

        return Ok(
            new QuestGetQuestClearPartyData()
            {
                quest_clear_party_setting_list = clearParty,
                lost_unit_list = lostUnitList
            }
        );
    }

    [HttpPost("get_quest_clear_party_multi")]
    public async Task<DragaliaResult> GetQuestClearPartyMulti(
        QuestGetQuestClearPartyMultiRequest request
    )
    {
        (IEnumerable<PartySettingList> clearParty, IEnumerable<AtgenLostUnitList> lostUnitList) =
            await this.clearPartyService.GetQuestClearParty(request.quest_id, true);

        await this.updateDataService.SaveChangesAsync();

        return Ok(
            new QuestGetQuestClearPartyMultiData()
            {
                quest_multi_clear_party_setting_list = clearParty,
                lost_unit_list = lostUnitList
            }
        );
    }

    [HttpPost("get_quest_open_treasure")]
    public async Task<DragaliaResult> GetQuestOpenTreasure(QuestOpenTreasureRequest request)
    {
        QuestTreasureData questTreasureData = MasterAsset.QuestTreasureData[
            request.quest_treasure_id
        ];

        await rewardService.GrantReward(
            new Entity(
                questTreasureData.EntityType,
                questTreasureData.EntityId,
                questTreasureData.EntityQuantity
            )
        );

        List<AtgenBuildEventRewardEntityList> rewards = new();

        if (questTreasureData.EntityType != EntityTypes.None)
        {
            // find entity type for dragon storage
            rewards.Add(
                new AtgenBuildEventRewardEntityList(
                    questTreasureData.EntityType,
                    questTreasureData.EntityId,
                    questTreasureData.EntityQuantity
                )
            );
        }

        IEnumerable<AtgenBuildEventRewardEntityList> quest_treasure_reward_list = rewards;

        IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list =
            new List<AtgenDuplicateEntityList>();
        EntityResult entityResult = rewardService.GetEntityResult();

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return Ok(
            new QuestOpenTreasureData()
            {
                update_data_list = updateDataList,
                entity_result = entityResult,
                quest_treasure_reward_list = quest_treasure_reward_list,
                duplicate_entity_list = duplicate_entity_list,
                add_max_dragon_quantity = questTreasureData.AddMaxDragonStorage,
                add_max_weapon_quantity = 0,
                add_max_amulet_quantity = 0
            }
        );
    }

    [HttpPost("set_quest_clear_party")]
    public async Task<DragaliaResult> SetQuestClearParty(QuestSetQuestClearPartyRequest request)
    {
        await this.clearPartyService.SetQuestClearParty(
            request.quest_id,
            false,
            request.request_party_setting_list
        );

        await this.updateDataService.SaveChangesAsync();

        return Ok(new QuestSetQuestClearPartyData() { result = 1 });
    }

    [HttpPost("set_quest_clear_party_multi")]
    public async Task<DragaliaResult> SetQuestClearParty(
        QuestSetQuestClearPartyMultiRequest request
    )
    {
        await this.clearPartyService.SetQuestClearParty(
            request.quest_id,
            true,
            request.request_party_setting_list
        );

        await this.updateDataService.SaveChangesAsync();

        return Ok(new QuestSetQuestClearPartyMultiData() { result = 1 });
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
}
