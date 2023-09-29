using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

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
            await this.Client.PostMsgpack<TimeAttackRankingGetDataData>(
                "/time_attack_ranking/get_data",
                new TimeAttackRankingGetDataRequest() { }
            )
        ).data.ranking_tier_reward_list
            .Should()
            .NotBeEmpty();
    }

    [Fact]
    public async Task ReceiveTierReward_GrantsRewardsOnce()
    {
        DbPlayerUserData oldUserData = await this.ApiContext.PlayerUserData
            .AsNoTracking()
            .FirstAsync(x => x.DeviceAccountId == DeviceAccountId);

        int questId = 227010101; // First Volk TA quest

        this.ApiContext.PlayerQuests.Add(
            new DbQuest()
            {
                DeviceAccountId = DeviceAccountId,
                BestClearTime = 200f,
                QuestId = questId
            }
        );

        await this.ApiContext.SaveChangesAsync();

        TimeAttackRankingReceiveTierRewardData rewardResponse = (
            await this.Client.PostMsgpack<TimeAttackRankingReceiveTierRewardData>(
                "/time_attack_ranking/receive_tier_reward",
                new TimeAttackRankingReceiveTierRewardRequest() { quest_id = questId }
            )
        ).data;

        rewardResponse.ranking_tier_reward_list.Should().NotBeEmpty();

        rewardResponse.ranking_tier_reward_entity_list
            .Should()
            .ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList()
                {
                    entity_id = 0,
                    entity_type = EntityTypes.Dew,
                    entity_quantity = 7000
                }
            );

        rewardResponse.entity_result.new_get_entity_list
            .Should()
            .ContainEquivalentOf(
                new AtgenDuplicateEntityList() { entity_id = 0, entity_type = EntityTypes.Dew }
            );

        rewardResponse.update_data_list.user_data.dew_point
            .Should()
            .Be(oldUserData.DewPoint + 7000);

        TimeAttackRankingReceiveTierRewardData secondRewardResponse = (
            await this.Client.PostMsgpack<TimeAttackRankingReceiveTierRewardData>(
                "/time_attack_ranking/receive_tier_reward",
                new TimeAttackRankingReceiveTierRewardRequest() { quest_id = questId }
            )
        ).data;

        secondRewardResponse.ranking_tier_reward_entity_list.Should().BeEmpty();
        secondRewardResponse.update_data_list.user_data.Should().BeNull();
    }
}
