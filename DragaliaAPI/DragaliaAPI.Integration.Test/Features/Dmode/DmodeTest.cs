using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Dmode;

public class DmodeTest : TestFixture
{
    public DmodeTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions(toleranceSec: 2);
    }

    [Fact]
    public async Task GetData_ReturnsData()
    {
        DragaliaResponse<DmodeGetDataData> resp = await Client.PostMsgpack<DmodeGetDataResponse>(
            "dmode/get_data",
            new DmodeGetDataRequest()
        );

        resp.data.DmodeInfo.IsEntry.Should().BeTrue();
        resp.data.DmodeCharaList.Should().NotBeNull();
        resp.data.DmodeDungeonInfo.State.Should().Be(DungeonState.Waiting);
        resp.data.DmodeExpedition.State.Should().Be(ExpeditionState.Waiting);
        resp.data.DmodeServitorPassiveList.Should().NotBeNull();
        resp.data.DmodeStoryList.Should().NotBeNull();
    }

    [Fact]
    public async Task ReadStory_ReadsStory()
    {
        int oldWyrmite = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Single(x => x.ViewerId == ViewerId)
            .Crystal;

        DragaliaResponse<DmodeReadStoryData> resp =
            await Client.PostMsgpack<DmodeReadStoryResponse>(
                "dmode/read_story",
                new DmodeReadStoryRequest() { DmodeStoryId = 1 }
            );

        resp.data.DmodeStoryRewardList.Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new()
                    {
                        EntityType = EntityTypes.Wyrmite,
                        EntityId = 0,
                        EntityQuantity = 25
                    },
                    new()
                    {
                        EntityType = EntityTypes.DmodePoint,
                        EntityId = (int)DmodePoint.Point1,
                        EntityQuantity = 5000,
                    },
                    new()
                    {
                        EntityType = EntityTypes.DmodePoint,
                        EntityId = (int)DmodePoint.Point2,
                        EntityQuantity = 1000,
                    }
                }
            );
        resp.data.UpdateDataList.UserData.Crystal.Should().Be(oldWyrmite + 25);
        resp.data.UpdateDataList.DmodeStoryList.Should()
            .ContainEquivalentOf(new DmodeStoryList() { DmodeStoryId = 1, IsRead = 1 });
    }

    [Fact]
    public async Task BuildupServitorPassive_BuildsUp()
    {
        DragaliaResponse<DmodeBuildupServitorPassiveData> resp =
            await Client.PostMsgpack<DmodeBuildupServitorPassiveResponse>(
                "dmode/buildup_servitor_passive",
                new DmodeBuildupServitorPassiveRequest()
                {
                    RequestBuildupPassiveList = new List<DmodeServitorPassiveList>()
                    {
                        new()
                        {
                            PassiveNo = DmodeServitorPassiveType.BurstDamage,
                            PassiveLevel = 2
                        },
                        new()
                        {
                            PassiveNo = DmodeServitorPassiveType.ResistUndead,
                            PassiveLevel = 10
                        },
                        new()
                        {
                            PassiveNo = DmodeServitorPassiveType.ResistNatural,
                            PassiveLevel = 2
                        }
                    }
                }
            );

        resp.data.DmodeServitorPassiveList.Should()
            .Contain(x =>
                x.PassiveNo == DmodeServitorPassiveType.BurstDamage && x.PassiveLevel == 2
            );
        resp.data.DmodeServitorPassiveList.Should()
            .Contain(x =>
                x.PassiveNo == DmodeServitorPassiveType.ResistUndead && x.PassiveLevel == 10
            );
        resp.data.DmodeServitorPassiveList.Should()
            .Contain(x =>
                x.PassiveNo == DmodeServitorPassiveType.ResistNatural && x.PassiveLevel == 2
            );

        ApiContext
            .PlayerDmodeServitorPassives.AsNoTracking()
            .Should()
            .ContainEquivalentOf(
                new DbPlayerDmodeServitorPassive()
                {
                    ViewerId = ViewerId,
                    PassiveId = DmodeServitorPassiveType.ResistNatural,
                    Level = 2
                }
            );
        ApiContext
            .PlayerDmodeServitorPassives.AsNoTracking()
            .Should()
            .ContainEquivalentOf(
                new DbPlayerDmodeServitorPassive()
                {
                    ViewerId = ViewerId,
                    PassiveId = DmodeServitorPassiveType.ResistUndead,
                    Level = 10
                }
            );
        ApiContext
            .PlayerDmodeServitorPassives.AsNoTracking()
            .Should()
            .ContainEquivalentOf(
                new DbPlayerDmodeServitorPassive()
                {
                    ViewerId = ViewerId,
                    PassiveId = DmodeServitorPassiveType.BurstDamage,
                    Level = 2
                }
            );
    }

    [Fact]
    public async Task Expedition_CanStartAndForceFinish()
    {
        DragaliaResponse<DmodeExpeditionStartData> resp =
            await this.Client.PostMsgpack<DmodeExpeditionStartResponse>(
                "dmode/expedition_start",
                new DmodeExpeditionStartRequest()
                {
                    CharaIdList = new[]
                    {
                        Charas.HunterBerserker,
                        Charas.Empty,
                        Charas.Empty,
                        Charas.Empty
                    },
                    TargetFloorNum = 30
                }
            );

        resp.data.DmodeExpedition.Should()
            .BeEquivalentTo(
                new DmodeExpedition()
                {
                    CharaId1 = Charas.HunterBerserker,
                    CharaId2 = Charas.Empty,
                    CharaId3 = Charas.Empty,
                    CharaId4 = Charas.Empty,
                    TargetFloorNum = 30,
                    State = ExpeditionState.Playing,
                    StartTime = DateTimeOffset.UtcNow
                }
            );

        DragaliaResponse<DmodeExpeditionForceFinishData> finishResp =
            await this.Client.PostMsgpack<DmodeExpeditionForceFinishResponse>(
                "dmode/expedition_force_finish",
                new DmodeExpeditionForceFinishRequest() { }
            );

        finishResp
            .data.DmodeExpedition.Should()
            .BeEquivalentTo(
                new DmodeExpedition()
                {
                    CharaId1 = Charas.HunterBerserker,
                    CharaId2 = Charas.Empty,
                    CharaId3 = Charas.Empty,
                    CharaId4 = Charas.Empty,
                    TargetFloorNum = 30,
                    State = ExpeditionState.Waiting,
                    StartTime = resp.data.DmodeExpedition.StartTime
                }
            );
        finishResp
            .data.DmodeIngameResult.CharaIdList.Should()
            .BeEquivalentTo(
                new[] { Charas.HunterBerserker, Charas.Empty, Charas.Empty, Charas.Empty }
            );
        finishResp.data.DmodeIngameResult.RewardTalismanList.Should().BeEmpty();
        finishResp.data.DmodeIngameResult.TakeDmodePoint1.Should().Be(0);
        finishResp.data.DmodeIngameResult.TakeDmodePoint2.Should().Be(0);
    }

    [Fact]
    public async Task Expedition_CanStartAndFinish()
    {
        DateTimeOffset startTime = DateTimeOffset.UtcNow.AddDays(-1);

        this.MockDateTimeProvider.Setup(x => x.UtcNow).Returns(startTime);

        DragaliaResponse<DmodeExpeditionStartData> resp =
            await this.Client.PostMsgpack<DmodeExpeditionStartResponse>(
                "dmode/expedition_start",
                new DmodeExpeditionStartRequest()
                {
                    CharaIdList = new[]
                    {
                        Charas.HunterBerserker,
                        Charas.Chrom,
                        Charas.Cassandra,
                        Charas.GalaMym
                    },
                    TargetFloorNum = 30
                }
            );

        resp.data.DmodeExpedition.Should()
            .BeEquivalentTo(
                new DmodeExpedition()
                {
                    CharaId1 = Charas.HunterBerserker,
                    CharaId2 = Charas.Chrom,
                    CharaId3 = Charas.Cassandra,
                    CharaId4 = Charas.GalaMym,
                    TargetFloorNum = 30,
                    State = ExpeditionState.Playing,
                    StartTime = startTime
                }
            );

        this.MockDateTimeProvider.Setup(x => x.UtcNow).Returns(DateTimeOffset.UtcNow);

        DragaliaResponse<DmodeExpeditionFinishData> finishResp =
            await this.Client.PostMsgpack<DmodeExpeditionFinishResponse>(
                "dmode/expedition_finish",
                new DmodeExpeditionFinishRequest() { }
            );

        finishResp
            .data.DmodeExpedition.Should()
            .BeEquivalentTo(
                new DmodeExpedition()
                {
                    CharaId1 = Charas.HunterBerserker,
                    CharaId2 = Charas.Chrom,
                    CharaId3 = Charas.Cassandra,
                    CharaId4 = Charas.GalaMym,
                    TargetFloorNum = 30,
                    State = ExpeditionState.Waiting,
                    StartTime = startTime
                }
            );

        finishResp.data.DmodeIngameResult.TakeDmodePoint1.Should().BeGreaterThan(0);
        finishResp.data.DmodeIngameResult.TakeDmodePoint2.Should().Be(0);
        finishResp.data.DmodeIngameResult.RewardTalismanList.Should().NotBeEmpty();
        finishResp
            .data.DmodeIngameResult.RewardTalismanList.Should()
            .Contain(x => x.TalismanId == Talismans.HunterBerserker);
        finishResp
            .data.DmodeIngameResult.RewardTalismanList.Should()
            .Contain(x => x.TalismanId == Talismans.Chrom);
        finishResp
            .data.DmodeIngameResult.RewardTalismanList.Should()
            .Contain(x => x.TalismanId == Talismans.Cassandra);
        finishResp
            .data.DmodeIngameResult.RewardTalismanList.Should()
            .Contain(x => x.TalismanId == Talismans.GalaMym);
    }
}
