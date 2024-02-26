using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.TimeAttack;

public class TimeAttackRankingTest : TestFixture
{
    public TimeAttackRankingTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetData_ReturnsRewards()
    {
        (
            await this.Client.PostMsgpack<TimeAttackRankingGetDataResponse>(
                "/time_attack_ranking/get_data"
            )
        )
            .Data.RankingTierRewardList.Should()
            .NotBeEmpty();
    }

    [Fact]
    public async Task ReceiveTierReward_GrantsRewardsOnce()
    {
        DbPlayerUserData oldUserData = await this
            .ApiContext.PlayerUserData.AsNoTracking()
            .FirstAsync(x => x.ViewerId == ViewerId);

        int questId = 227010101; // First Volk TA quest

        this.ApiContext.PlayerQuests.Add(
            new DbQuest()
            {
                ViewerId = ViewerId,
                BestClearTime = 200f,
                QuestId = questId
            }
        );

        await this.ApiContext.SaveChangesAsync();

        TimeAttackRankingReceiveTierRewardResponse rewardResponse = (
            await this.Client.PostMsgpack<TimeAttackRankingReceiveTierRewardResponse>(
                "/time_attack_ranking/receive_tier_reward",
                new TimeAttackRankingReceiveTierRewardRequest() { QuestId = questId }
            )
        ).Data;

        rewardResponse.RankingTierRewardList.Should().NotBeEmpty();

        rewardResponse
            .RankingTierRewardEntityList.Should()
            .ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList()
                {
                    EntityId = 0,
                    EntityType = EntityTypes.Dew,
                    EntityQuantity = 7000
                }
            );

        rewardResponse
            .EntityResult.NewGetEntityList.Should()
            .ContainEquivalentOf(
                new AtgenDuplicateEntityList() { EntityId = 0, EntityType = EntityTypes.Dew }
            );

        rewardResponse.UpdateDataList.UserData.DewPoint.Should().Be(oldUserData.DewPoint + 7000);

        TimeAttackRankingReceiveTierRewardResponse secondRewardResponse = (
            await this.Client.PostMsgpack<TimeAttackRankingReceiveTierRewardResponse>(
                "/time_attack_ranking/receive_tier_reward",
                new TimeAttackRankingReceiveTierRewardRequest() { QuestId = questId }
            )
        ).Data;

        secondRewardResponse.RankingTierRewardEntityList.Should().BeEmpty();
        secondRewardResponse.UpdateDataList.UserData.Should().BeNull();
    }
}
