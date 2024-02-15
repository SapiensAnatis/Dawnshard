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
    public DragaliaResult<TimeAttackRankingGetDataData> GetData(
        TimeAttackRankingGetDataRequest request
    )
    {
        IEnumerable<RankingTierRewardList> rewardList = timeAttackService
            .GetRewards()
            .Select(ToRankingTierRewardList);

        return new TimeAttackRankingGetDataData() { ranking_tier_reward_list = rewardList };
    }

    [Route("receive_tier_reward")]
    [HttpPost]
    public async Task<DragaliaResult> ReceiveTierReward(
        TimeAttackRankingReceiveTierRewardRequest request
    )
    {
        IEnumerable<RankingTierReward> receivedRewards = await timeAttackService.ReceiveTierReward(
            request.quest_id
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();
        EntityResult entityResult = rewardService.GetEntityResult();

        IEnumerable<RankingTierReward> rewardList = timeAttackService.GetRewards();

        return this.Ok(
            new TimeAttackRankingReceiveTierRewardData()
            {
                update_data_list = updateDataList,
                entity_result = entityResult,
                ranking_tier_reward_list = rewardList.Select(ToRankingTierRewardList),
                ranking_tier_reward_entity_list = receivedRewards.Select(ToRewardEntityList)
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
