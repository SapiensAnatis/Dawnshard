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
        DragaliaResponse<DmodeGetDataData> resp = await Client.PostMsgpack<DmodeGetDataData>(
            "dmode/get_data",
            new DmodeGetDataRequest()
        );

        resp.data.dmode_info.is_entry.Should().BeTrue();
        resp.data.dmode_chara_list.Should().NotBeNull();
        resp.data.dmode_dungeon_info.state.Should().Be(DungeonState.Waiting);
        resp.data.dmode_expedition.state.Should().Be(ExpeditionState.Waiting);
        resp.data.dmode_servitor_passive_list.Should().NotBeNull();
        resp.data.dmode_story_list.Should().NotBeNull();
    }

    [Fact]
    public async Task ReadStory_ReadsStory()
    {
        int oldWyrmite = this.ApiContext.PlayerUserData.AsNoTracking()
            .Single(x => x.ViewerId == ViewerId)
            .Crystal;

        DragaliaResponse<DmodeReadStoryData> resp = await Client.PostMsgpack<DmodeReadStoryData>(
            "dmode/read_story",
            new DmodeReadStoryRequest() { dmode_story_id = 1 }
        );

        resp.data.dmode_story_reward_list.Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new()
                    {
                        entity_type = EntityTypes.Wyrmite,
                        entity_id = 0,
                        entity_quantity = 25
                    },
                    new()
                    {
                        entity_type = EntityTypes.DmodePoint,
                        entity_id = (int)DmodePoint.Point1,
                        entity_quantity = 5000,
                    },
                    new()
                    {
                        entity_type = EntityTypes.DmodePoint,
                        entity_id = (int)DmodePoint.Point2,
                        entity_quantity = 1000,
                    }
                }
            );
        resp.data.update_data_list.user_data.crystal.Should().Be(oldWyrmite + 25);
        resp.data.update_data_list.dmode_story_list.Should()
            .ContainEquivalentOf(new DmodeStoryList() { dmode_story_id = 1, is_read = 1 });
    }

    [Fact]
    public async Task BuildupServitorPassive_BuildsUp()
    {
        DragaliaResponse<DmodeBuildupServitorPassiveData> resp =
            await Client.PostMsgpack<DmodeBuildupServitorPassiveData>(
                "dmode/buildup_servitor_passive",
                new DmodeBuildupServitorPassiveRequest()
                {
                    request_buildup_passive_list = new List<DmodeServitorPassiveList>()
                    {
                        new()
                        {
                            passive_no = DmodeServitorPassiveType.BurstDamage,
                            passive_level = 2
                        },
                        new()
                        {
                            passive_no = DmodeServitorPassiveType.ResistUndead,
                            passive_level = 10
                        },
                        new()
                        {
                            passive_no = DmodeServitorPassiveType.ResistNatural,
                            passive_level = 2
                        }
                    }
                }
            );

        resp.data.dmode_servitor_passive_list.Should()
            .Contain(
                x => x.passive_no == DmodeServitorPassiveType.BurstDamage && x.passive_level == 2
            );
        resp.data.dmode_servitor_passive_list.Should()
            .Contain(
                x => x.passive_no == DmodeServitorPassiveType.ResistUndead && x.passive_level == 10
            );
        resp.data.dmode_servitor_passive_list.Should()
            .Contain(
                x => x.passive_no == DmodeServitorPassiveType.ResistNatural && x.passive_level == 2
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
            await this.Client.PostMsgpack<DmodeExpeditionStartData>(
                "dmode/expedition_start",
                new DmodeExpeditionStartRequest()
                {
                    chara_id_list = new[]
                    {
                        Charas.HunterBerserker,
                        Charas.Empty,
                        Charas.Empty,
                        Charas.Empty
                    },
                    target_floor_num = 30
                }
            );

        resp.data.dmode_expedition.Should()
            .BeEquivalentTo(
                new DmodeExpedition()
                {
                    chara_id_1 = Charas.HunterBerserker,
                    chara_id_2 = Charas.Empty,
                    chara_id_3 = Charas.Empty,
                    chara_id_4 = Charas.Empty,
                    target_floor_num = 30,
                    state = ExpeditionState.Playing,
                    start_time = DateTimeOffset.UtcNow
                }
            );

        DragaliaResponse<DmodeExpeditionForceFinishData> finishResp =
            await this.Client.PostMsgpack<DmodeExpeditionForceFinishData>(
                "dmode/expedition_force_finish",
                new DmodeExpeditionForceFinishRequest() { }
            );

        finishResp
            .data.dmode_expedition.Should()
            .BeEquivalentTo(
                new DmodeExpedition()
                {
                    chara_id_1 = Charas.HunterBerserker,
                    chara_id_2 = Charas.Empty,
                    chara_id_3 = Charas.Empty,
                    chara_id_4 = Charas.Empty,
                    target_floor_num = 30,
                    state = ExpeditionState.Waiting,
                    start_time = resp.data.dmode_expedition.start_time
                }
            );
        finishResp
            .data.dmode_ingame_result.chara_id_list.Should()
            .BeEquivalentTo(
                new[] { Charas.HunterBerserker, Charas.Empty, Charas.Empty, Charas.Empty }
            );
        finishResp.data.dmode_ingame_result.reward_talisman_list.Should().BeEmpty();
        finishResp.data.dmode_ingame_result.take_dmode_point_1.Should().Be(0);
        finishResp.data.dmode_ingame_result.take_dmode_point_2.Should().Be(0);
    }

    [Fact]
    public async Task Expedition_CanStartAndFinish()
    {
        DateTimeOffset startTime = DateTimeOffset.UtcNow.AddDays(-1);

        this.MockDateTimeProvider.Setup(x => x.UtcNow).Returns(startTime);

        DragaliaResponse<DmodeExpeditionStartData> resp =
            await this.Client.PostMsgpack<DmodeExpeditionStartData>(
                "dmode/expedition_start",
                new DmodeExpeditionStartRequest()
                {
                    chara_id_list = new[]
                    {
                        Charas.HunterBerserker,
                        Charas.Chrom,
                        Charas.Cassandra,
                        Charas.GalaMym
                    },
                    target_floor_num = 30
                }
            );

        resp.data.dmode_expedition.Should()
            .BeEquivalentTo(
                new DmodeExpedition()
                {
                    chara_id_1 = Charas.HunterBerserker,
                    chara_id_2 = Charas.Chrom,
                    chara_id_3 = Charas.Cassandra,
                    chara_id_4 = Charas.GalaMym,
                    target_floor_num = 30,
                    state = ExpeditionState.Playing,
                    start_time = startTime
                }
            );

        this.MockDateTimeProvider.Setup(x => x.UtcNow).Returns(DateTimeOffset.UtcNow);

        DragaliaResponse<DmodeExpeditionFinishData> finishResp =
            await this.Client.PostMsgpack<DmodeExpeditionFinishData>(
                "dmode/expedition_finish",
                new DmodeExpeditionFinishRequest() { }
            );

        finishResp
            .data.dmode_expedition.Should()
            .BeEquivalentTo(
                new DmodeExpedition()
                {
                    chara_id_1 = Charas.HunterBerserker,
                    chara_id_2 = Charas.Chrom,
                    chara_id_3 = Charas.Cassandra,
                    chara_id_4 = Charas.GalaMym,
                    target_floor_num = 30,
                    state = ExpeditionState.Waiting,
                    start_time = startTime
                }
            );

        finishResp.data.dmode_ingame_result.take_dmode_point_1.Should().BeGreaterThan(0);
        finishResp.data.dmode_ingame_result.take_dmode_point_2.Should().Be(0);
        finishResp.data.dmode_ingame_result.reward_talisman_list.Should().NotBeEmpty();
        finishResp
            .data.dmode_ingame_result.reward_talisman_list.Should()
            .Contain(x => x.talisman_id == Talismans.HunterBerserker);
        finishResp
            .data.dmode_ingame_result.reward_talisman_list.Should()
            .Contain(x => x.talisman_id == Talismans.Chrom);
        finishResp
            .data.dmode_ingame_result.reward_talisman_list.Should()
            .Contain(x => x.talisman_id == Talismans.Cassandra);
        finishResp
            .data.dmode_ingame_result.reward_talisman_list.Should()
            .Contain(x => x.talisman_id == Talismans.GalaMym);
    }
}
