using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.TimeAttack;

[Route("time_attack_ranking")]
public class TimeAttackController(
    ITimeAttackService timeAttackService,
    IRewardService rewardService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [Route("get_data")]
    [HttpPost]
    public DragaliaResult<TimeAttackRankingGetDataResponse> GetData()
    {
        IEnumerable<RankingTierRewardList> rewardList = timeAttackService
            .GetRewards()
            .Select(ToRankingTierRewardList);

        return new TimeAttackRankingGetDataResponse() { RankingTierRewardList = rewardList };
    }

    [Route("receive_tier_reward")]
    [HttpPost]
    public async Task<DragaliaResult> ReceiveTierReward(
        TimeAttackRankingReceiveTierRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<RankingTierReward> receivedRewards = await timeAttackService.ReceiveTierReward(
            request.QuestId
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        EntityResult entityResult = rewardService.GetEntityResult();

        IEnumerable<RankingTierReward> rewardList = timeAttackService.GetRewards();

        return this.Ok(
            new TimeAttackRankingReceiveTierRewardResponse()
            {
                UpdateDataList = updateDataList,
                EntityResult = entityResult,
                RankingTierRewardList = rewardList.Select(ToRankingTierRewardList),
                RankingTierRewardEntityList = receivedRewards.Select(ToRewardEntityList)
            }
        );
    }

    private static RankingTierRewardList ToRankingTierRewardList(RankingTierReward reward) =>
        new(reward.GroupId, reward.Id);

    private static AtgenBuildEventRewardEntityList ToRewardEntityList(RankingTierReward reward) =>
        new(
            reward.RankingRewardEntityType,
            reward.RankingRewardEntityId,
            reward.RankingRewardEntityQuantity
        );
}
