using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Dmode;

public class DmodeTest : TestFixture
{
    public DmodeTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions(toleranceSec: 2);
        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task GetData_ReturnsData()
    {
        DragaliaResponse<DmodeGetDataResponse> resp =
            await Client.PostMsgpack<DmodeGetDataResponse>("dmode/get_data");

        resp.Data.DmodeInfo.IsEntry.Should().BeTrue();
        resp.Data.DmodeCharaList.Should().NotBeNull();
        resp.Data.DmodeDungeonInfo.State.Should().Be(DungeonState.Waiting);
        resp.Data.DmodeExpedition.State.Should().Be(ExpeditionState.Waiting);
        resp.Data.DmodeServitorPassiveList.Should().NotBeNull();
        resp.Data.DmodeStoryList.Should().NotBeNull();
    }

    [Fact]
    public async Task ReadStory_ReadsStory()
    {
        int oldWyrmite = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Single(x => x.ViewerId == ViewerId)
            .Crystal;

        DragaliaResponse<DmodeReadStoryResponse> resp =
            await Client.PostMsgpack<DmodeReadStoryResponse>(
                "dmode/read_story",
                new DmodeReadStoryRequest() { DmodeStoryId = 1 }
            );

        resp.Data.DmodeStoryRewardList.Should()
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
        resp.Data.UpdateDataList.UserData.Crystal.Should().Be(oldWyrmite + 25);
        resp.Data.UpdateDataList.DmodeStoryList.Should()
            .ContainEquivalentOf(new DmodeStoryList() { DmodeStoryId = 1, IsRead = true });
    }

    [Fact]
    public async Task BuildupServitorPassive_BuildsUp()
    {
        DragaliaResponse<DmodeBuildupServitorPassiveResponse> resp =
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

        resp.Data.DmodeServitorPassiveList.Should()
            .Contain(x =>
                x.PassiveNo == DmodeServitorPassiveType.BurstDamage && x.PassiveLevel == 2
            );
        resp.Data.DmodeServitorPassiveList.Should()
            .Contain(x =>
                x.PassiveNo == DmodeServitorPassiveType.ResistUndead && x.PassiveLevel == 10
            );
        resp.Data.DmodeServitorPassiveList.Should()
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
        DragaliaResponse<DmodeExpeditionStartResponse> resp =
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

        resp.Data.DmodeExpedition.Should()
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

        DragaliaResponse<DmodeExpeditionForceFinishResponse> finishResp =
            await this.Client.PostMsgpack<DmodeExpeditionForceFinishResponse>(
                "dmode/expedition_force_finish"
            );

        finishResp
            .Data.DmodeExpedition.Should()
            .BeEquivalentTo(
                new DmodeExpedition()
                {
                    CharaId1 = Charas.HunterBerserker,
                    CharaId2 = Charas.Empty,
                    CharaId3 = Charas.Empty,
                    CharaId4 = Charas.Empty,
                    TargetFloorNum = 30,
                    State = ExpeditionState.Waiting,
                    StartTime = resp.Data.DmodeExpedition.StartTime
                }
            );
        finishResp
            .Data.DmodeIngameResult.CharaIdList.Should()
            .BeEquivalentTo(
                new[] { Charas.HunterBerserker, Charas.Empty, Charas.Empty, Charas.Empty }
            );
        finishResp.Data.DmodeIngameResult.RewardTalismanList.Should().BeEmpty();
        finishResp.Data.DmodeIngameResult.TakeDmodePoint1.Should().Be(0);
        finishResp.Data.DmodeIngameResult.TakeDmodePoint2.Should().Be(0);
    }

    [Fact]
    public async Task Expedition_CanStartAndFinish()
    {
        DateTimeOffset startTime = DateTimeOffset.UtcNow;

        DragaliaResponse<DmodeExpeditionStartResponse> resp =
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

        resp.Data.DmodeExpedition.Should()
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

        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow.AddDays(1));

        DragaliaResponse<DmodeExpeditionFinishResponse> finishResp =
            await this.Client.PostMsgpack<DmodeExpeditionFinishResponse>("dmode/expedition_finish");

        finishResp
            .Data.DmodeExpedition.Should()
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

        finishResp.Data.DmodeIngameResult.TakeDmodePoint1.Should().BeGreaterThan(0);
        finishResp.Data.DmodeIngameResult.TakeDmodePoint2.Should().Be(0);
        finishResp.Data.DmodeIngameResult.RewardTalismanList.Should().NotBeEmpty();
        finishResp
            .Data.DmodeIngameResult.RewardTalismanList.Should()
            .Contain(x => x.TalismanId == Talismans.HunterBerserker);
        finishResp
            .Data.DmodeIngameResult.RewardTalismanList.Should()
            .Contain(x => x.TalismanId == Talismans.Chrom);
        finishResp
            .Data.DmodeIngameResult.RewardTalismanList.Should()
            .Contain(x => x.TalismanId == Talismans.Cassandra);
        finishResp
            .Data.DmodeIngameResult.RewardTalismanList.Should()
            .Contain(x => x.TalismanId == Talismans.GalaMym);
    }
}
