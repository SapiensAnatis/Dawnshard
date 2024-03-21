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
public class QuestController(
    IStoryService storyService,
    IHelperService helperService,
    IUpdateDataService updateDataService,
    IClearPartyService clearPartyService,
    IQuestTreasureService questTreasureService
) : DragaliaControllerBase
{
    private readonly IStoryService storyService = storyService;
    private readonly IHelperService helperService = helperService;
    private readonly IUpdateDataService updateDataService = updateDataService;
    private readonly IClearPartyService clearPartyService = clearPartyService;
    private readonly IQuestTreasureService questTreasureService = questTreasureService;

    [HttpPost]
    [Route("read_story")]
    public async Task<DragaliaResult> ReadStory(
        QuestReadStoryRequest request,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<AtgenBuildEventRewardEntityList> rewardList = await this.storyService.ReadStory(
            StoryTypes.Quest,
            request.QuestStoryId
        );

        EntityResult entityResult = StoryService.GetEntityResult(rewardList);
        IEnumerable<AtgenQuestStoryRewardList> questRewardList = rewardList.Select(
            StoryService.ToQuestStoryReward
        );

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        return this.Ok(
            new QuestReadStoryResponse()
            {
                QuestStoryRewardList = questRewardList,
                EntityResult = entityResult,
                UpdateDataList = updateDataList
            }
        );
    }

    [HttpPost("get_support_user_list")]
    public async Task<DragaliaResult> GetUserSupportList()
    {
        // TODO: this is actually going to be a pretty complicated system
        QuestGetSupportUserListResponse response = await this.helperService.GetHelpers();
        return Ok(response);
    }

    [HttpPost("get_quest_clear_party")]
    public async Task<DragaliaResult> GetQuestClearParty(
        QuestGetQuestClearPartyRequest request,
        CancellationToken cancellationToken
    )
    {
        (IEnumerable<PartySettingList> clearParty, IEnumerable<AtgenLostUnitList> lostUnitList) =
            await this.clearPartyService.GetQuestClearParty(request.QuestId, false);

        await this.updateDataService.SaveChangesAsync(cancellationToken);
        // Updated lost entities

        return Ok(
            new QuestGetQuestClearPartyResponse()
            {
                QuestClearPartySettingList = clearParty,
                LostUnitList = lostUnitList
            }
        );
    }

    [HttpPost("get_quest_clear_party_multi")]
    public async Task<DragaliaResult> GetQuestClearPartyMulti(
        QuestGetQuestClearPartyMultiRequest request,
        CancellationToken cancellationToken
    )
    {
        (IEnumerable<PartySettingList> clearParty, IEnumerable<AtgenLostUnitList> lostUnitList) =
            await this.clearPartyService.GetQuestClearParty(request.QuestId, true);

        await this.updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(
            new QuestGetQuestClearPartyMultiResponse()
            {
                QuestMultiClearPartySettingList = clearParty,
                LostUnitList = lostUnitList
            }
        );
    }

    [HttpPost("open_treasure")]
    public async Task<DragaliaResult> OpenTreasure(
        QuestOpenTreasureRequest request,
        CancellationToken cancellationToken
    )
    {
        QuestOpenTreasureResponse response = await this.questTreasureService.DoOpenTreasure(
            request,
            cancellationToken
        );
        return Ok(response);
    }

    [HttpPost("set_quest_clear_party")]
    public async Task<DragaliaResult> SetQuestClearParty(
        QuestSetQuestClearPartyRequest request,
        CancellationToken cancellationToken
    )
    {
        await this.clearPartyService.SetQuestClearParty(
            request.QuestId,
            false,
            request.RequestPartySettingList
        );

        await this.updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(new QuestSetQuestClearPartyResponse() { Result = 1 });
    }

    [HttpPost("set_quest_clear_party_multi")]
    public async Task<DragaliaResult> SetQuestClearParty(
        QuestSetQuestClearPartyMultiRequest request,
        CancellationToken cancellationToken
    )
    {
        await this.clearPartyService.SetQuestClearParty(
            request.QuestId,
            true,
            request.RequestPartySettingList
        );

        await this.updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(new QuestSetQuestClearPartyMultiResponse() { Result = 1 });
    }

    [HttpPost("drop_list")]
    public DragaliaResult DropList(QuestDropListRequest request)
    {
        IEnumerable<DropEntity> drops = Enumerable.Empty<DropEntity>();
        if (MasterAsset.QuestDrops.TryGetValue(request.QuestId, out QuestDropInfo? dropInfo))
            drops = dropInfo.Drops;

        return Ok(
            new QuestDropListResponse()
            {
                QuestDropInfo = new()
                {
                    DropInfoList = drops.Select(x => new AtgenDuplicateEntityList()
                    {
                        EntityId = x.Id,
                        EntityType = x.EntityType,
                    })
                }
            }
        );
    }
}
