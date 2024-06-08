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

        string key = await this.StartDungeon(mockSession);

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
            [
                new DbWallRewardDate() { LastClaimDate = DateTimeOffset.UnixEpoch, },
                new DbPlayerQuestWall()
                {
                    ViewerId = ViewerId,
                    WallId = 216010001,
                    WallLevel = 1,
                },
                new DbPlayerQuestWall()
                {
                    ViewerId = ViewerId,
                    WallId = 216010002,
                    WallLevel = 2,
                },
                new DbPlayerQuestWall()
                {
                    ViewerId = ViewerId,
                    WallId = 216010003,
                    WallLevel = 3,
                },
                new DbPlayerQuestWall()
                {
                    ViewerId = ViewerId,
                    WallId = 216010004,
                    WallLevel = 4,
                },
                new DbPlayerQuestWall()
                {
                    ViewerId = ViewerId,
                    WallId = 216010005,
                    WallLevel = 5,
                }
            ]
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
                    LastRewardDate = DateTimeOffset.UnixEpoch,
                    RewardStatus = RewardStatus.Available
                }
            );
    }

    [Fact]
    public async Task ReceiveMonthlyRewards_ReceivesRewards()
    {
        await this.AddToDatabase(
            new DbWallRewardDate() { LastClaimDate = DateTimeOffset.UnixEpoch }
        );

        DbPlayerUserData oldUserData = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        int oldTwinklingSand = this
            .ApiContext.PlayerMaterials.AsNoTracking()
            .First(x => x.MaterialId == Materials.TwinklingSand)
            .Quantity;

        int expectedMana = 25_000;
        int expectedCoin = 40_000;
        int expectedDewPoint = 9_000;
        int expectedTwinklingSand = 1;

        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010001,
                    WallLevel = 5,
                },
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010002,
                    WallLevel = 5,
                },
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010003,
                    WallLevel = 5,
                },
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010004,
                    WallLevel = 5,
                },
                new()
                {
                    ViewerId = ViewerId,
                    WallId = 216010005,
                    WallLevel = 5,
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
                    SumWallLevel = 5 * 5,
                    LastRewardDate = DateTimeOffset.UnixEpoch,
                    RewardStatus = RewardStatus.Available
                }
            );

        response
            .UpdateDataList.UserData.DewPoint.Should()
            .Be(oldUserData.DewPoint + expectedDewPoint);

        response.UpdateDataList.UserData.Coin.Should().Be(oldUserData.Coin + expectedCoin);

        response
            .UpdateDataList.UserData.ManaPoint.Should()
            .Be(oldUserData.ManaPoint + expectedMana);

        response
            .UpdateDataList.MaterialList.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new MaterialList()
                {
                    MaterialId = Materials.TwinklingSand,
                    Quantity = oldTwinklingSand + expectedTwinklingSand
                }
            );
    }

    [Fact]
    public async Task ReceiveMonthlyRewawrds_NotInitialized_ReturnsInvalidArgument()
    {
        DragaliaResponse<ResultCodeResponse> response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "wall/receive_monthly_reward",
                new WallGetMonthlyRewardRequest() { QuestGroupId = 21601 },
                ensureSuccessHeader: false
            )
        );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.CommonInvalidArgument);
    }

    [Fact]
    public async Task ReceiveMonthlyRewawrds_NotEligible_ReturnsInvalidArgument()
    {
        await this.AddRangeToDatabase(
            [
                new DbWallRewardDate() { LastClaimDate = DateTimeOffset.UtcNow },
                new DbPlayerQuestWall()
                {
                    ViewerId = ViewerId,
                    WallId = 216010001,
                    WallLevel = 5,
                },
            ]
        );

        DragaliaResponse<ResultCodeResponse> response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "wall/receive_monthly_reward",
                new WallGetMonthlyRewardRequest() { QuestGroupId = 21601 },
                ensureSuccessHeader: false
            )
        );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.CommonInvalidArgument);
    }

    private async Task<string> StartDungeon(DungeonSession session)
    {
        string key = this.DungeonService.CreateSession(session);
        await this.DungeonService.SaveSession(CancellationToken.None);

        return key;
    }
}
