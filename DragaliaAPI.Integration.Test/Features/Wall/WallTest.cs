using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.Wall;

public class WallTest : TestFixture
{
    public WallTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions(toleranceSec: 2);
    }

    [Fact]
    public async Task Fail_ReturnsExpectedResponse()
    {
        int questId = 0;
        int expectedWallId = 216010001;
        int expectedWallLevel = 10;

        await AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                State = 0,
                ViewerId = ViewerId
            }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                WallId = expectedWallId,
                WallLevel = expectedWallLevel
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        WallFailData response = (
            await Client.PostMsgpack<WallFailData>(
                "/wall/fail",
                new WallFailRequest() { dungeon_key = key, fail_state = 0 }
            )
        ).data;

        response
            .fail_quest_detail.Should()
            .BeEquivalentTo(
                new AtgenFailQuestDetail()
                {
                    wall_id = expectedWallId,
                    wall_level = expectedWallLevel,
                    is_host = true
                }
            );
    }

    [Fact]
    public async Task GetMonthlyReward_ReturnsExpectedResponse()
    {
        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010001,
                    WallLevel = 1,
                },
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010002,
                    WallLevel = 2,
                },
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010003,
                    WallLevel = 3,
                },
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010004,
                    WallLevel = 4,
                },
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010005,
                    WallLevel = 5,
                }
            }
        );

        WallGetMonthlyRewardData response = (
            await this.Client.PostMsgpack<WallGetMonthlyRewardData>(
                "wall/get_monthly_reward",
                new WallGetMonthlyRewardData() { }
            )
        ).data;

        response
            .user_wall_reward_list.Should()
            .ContainEquivalentOf(
                new AtgenUserWallRewardList()
                {
                    quest_group_id = 21601,
                    sum_wall_level = 1 + 2 + 3 + 4 + 5,
                    last_reward_date = DateTimeOffset.UtcNow,
                    reward_status = RewardStatus.Received
                }
            );
    }

    [Fact]
    public async Task ReceiveMonthlyRewards_ReceivesRewards()
    {
        DbPlayerUserData oldUserData = this.ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        int expectedMana = 15_000;
        int expectedCoin = 20_000;
        int expectedDewPoint = 5_400;

        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010001,
                    WallLevel = 6,
                },
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010002,
                    WallLevel = 2,
                },
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010003,
                    WallLevel = 3,
                },
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010004,
                    WallLevel = 2,
                },
                new()
                {
                    ViewerId = ViewerId,
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

        response
            .user_wall_reward_list.Should()
            .ContainEquivalentOf(
                new AtgenUserWallRewardList()
                {
                    quest_group_id = 21601,
                    sum_wall_level = 6 + 2 + 3 + 2 + 1,
                    last_reward_date = DateTimeOffset.UtcNow,
                    reward_status = RewardStatus.Received
                }
            );

        response
            .update_data_list.user_data.dew_point.Should()
            .Be(oldUserData.DewPoint + expectedDewPoint);

        response.update_data_list.user_data.coin.Should().Be(oldUserData.Coin + expectedCoin);

        response
            .update_data_list.user_data.mana_point.Should()
            .Be(oldUserData.ManaPoint + expectedMana);
    }
}
