using DragaliaAPI.Controllers;
using DragaliaAPI.Features.ClearParty;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Quest;

[Route("quest")]
[ApiController]
public class QuestController : DragaliaControllerBase
{
    private readonly IStoryService storyService;
    private readonly IHelperService helperService;
    private readonly IUpdateDataService updateDataService;
    private readonly IClearPartyService clearPartyService;
    private readonly IQuestTreasureService questTreasureService;
    private readonly ILogger<QuestController> logger;

    public QuestController(
        IStoryService storyService,
        IHelperService helperService,
        IUpdateDataService updateDataService,
        IClearPartyService clearPartyService,
        IQuestTreasureService questTreasureService,
        ILogger<QuestController> logger
    )
    {
        this.storyService = storyService;
        this.helperService = helperService;
        this.updateDataService = updateDataService;
        this.clearPartyService = clearPartyService;
        this.questTreasureService = questTreasureService;
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

    [HttpPost("open_treasure")]
    public async Task<DragaliaResult> OpenTreasure(QuestOpenTreasureRequest request)
    {
        QuestOpenTreasureData response = await this.questTreasureService.DoOpenTreasure(request);
        return Ok(response);
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
        IEnumerable<DropEntity> drops = Enumerable.Empty<DropEntity>();
        if (MasterAsset.QuestDrops.TryGetValue(request.quest_id, out QuestDropInfo? dropInfo))
            drops = dropInfo.Drops;

        return Ok(
            new QuestDropListData()
            {
                quest_drop_info = new()
                {
                    drop_info_list = drops.Select(
                        x =>
                            new AtgenDuplicateEntityList()
                            {
                                entity_id = x.Id,
                                entity_type = x.EntityType,
                            }
                    )
                }
            }
        );
    }
}
