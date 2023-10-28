using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Wall;

public class WallTest : TestFixture
{
    public WallTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions(toleranceSec: 2);
    }

    [Fact]
    public async Task ReceiveMonthlyRewards_ReceivesRewards()
    {
        DbPlayerUserData oldUserData = this.ApiContext.PlayerUserData
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId);

        int expectedMana = 15_000;
        int expectedCoin = 20_000;
        int expectedDewPoint = 5_400;

        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    WallId = 216010001,
                    WallLevel = 6,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    WallId = 216010002,
                    WallLevel = 2,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    WallId = 216010003,
                    WallLevel = 3,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    WallId = 216010004,
                    WallLevel = 2,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    WallId = 216010005,
                    WallLevel = 1,
                }
            }
        );

        WallReceiveMonthlyRewardData response = (
            await this.Client.PostMsgpack<WallReceiveMonthlyRewardData>(
                "wall/receive_monthly_reward",
                new WallGetMonthlyRewardRequest() { quest_group_id = 21601 }
            )
        ).data;

        response.user_wall_reward_list
            .Should()
            .ContainEquivalentOf(
                new AtgenUserWallRewardList()
                {
                    quest_group_id = 21601,
                    sum_wall_level = 6 + 2 + 3 + 2 + 1,
                    last_reward_date = DateTimeOffset.UtcNow,
                    reward_status = RewardStatus.Received
                }
            );

        response.update_data_list.user_data.dew_point
            .Should()
            .Be(oldUserData.DewPoint + expectedDewPoint);

        response.update_data_list.user_data.coin.Should().Be(oldUserData.Coin + expectedCoin);

        response.update_data_list.user_data.mana_point
            .Should()
            .Be(oldUserData.ManaPoint + expectedMana);
    }
}
