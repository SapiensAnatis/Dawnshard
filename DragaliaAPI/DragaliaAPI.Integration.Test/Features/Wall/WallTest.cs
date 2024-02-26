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
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                WallId = expectedWallId,
                WallLevel = expectedWallLevel
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        WallFailResponse response = (
            await Client.PostMsgpack<WallFailResponse>(
                "/wall/fail",
                new WallFailRequest() { DungeonKey = key, FailState = 0 }
            )
        ).Data;

        response
            .FailQuestDetail.Should()
            .BeEquivalentTo(
                new AtgenFailQuestDetail()
                {
                    WallId = expectedWallId,
                    WallLevel = expectedWallLevel,
                    IsHost = true
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

        WallGetMonthlyRewardResponse response = (
            await this.Client.PostMsgpack<WallGetMonthlyRewardResponse>(
                "wall/get_monthly_reward",
                new WallGetMonthlyRewardResponse() { }
            )
        ).Data;

        response
            .UserWallRewardList.Should()
            .ContainEquivalentOf(
                new AtgenUserWallRewardList()
                {
                    QuestGroupId = 21601,
                    SumWallLevel = 1 + 2 + 3 + 4 + 5,
                    LastRewardDate = DateTimeOffset.UtcNow,
                    RewardStatus = RewardStatus.Received
                }
            );
    }

    [Fact]
    public async Task ReceiveMonthlyRewards_ReceivesRewards()
    {
        DbPlayerUserData oldUserData = this
            .ApiContext.PlayerUserData.AsNoTracking()
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

        WallReceiveMonthlyRewardResponse response = (
            await this.Client.PostMsgpack<WallReceiveMonthlyRewardResponse>(
                "wall/receive_monthly_reward",
                new WallGetMonthlyRewardRequest() { QuestGroupId = 21601 }
            )
        ).Data;

        response
            .UserWallRewardList.Should()
            .ContainEquivalentOf(
                new AtgenUserWallRewardList()
                {
                    QuestGroupId = 21601,
                    SumWallLevel = 6 + 2 + 3 + 2 + 1,
                    LastRewardDate = DateTimeOffset.UtcNow,
                    RewardStatus = RewardStatus.Received
                }
            );

        response
            .UpdateDataList.UserData.DewPoint.Should()
            .Be(oldUserData.DewPoint + expectedDewPoint);

        response.UpdateDataList.UserData.Coin.Should().Be(oldUserData.Coin + expectedCoin);

        response
            .UpdateDataList.UserData.ManaPoint.Should()
            .Be(oldUserData.ManaPoint + expectedMana);
    }
}
