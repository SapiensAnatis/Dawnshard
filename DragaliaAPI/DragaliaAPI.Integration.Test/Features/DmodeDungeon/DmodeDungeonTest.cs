using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.DmodeDungeon;

public class DmodeDungeonTest : TestFixture
{
    public DmodeDungeonTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task Start_CanCallFloor()
    {
        this.AddCharacter(Charas.Shingen);

        DragaliaResponse<DmodeDungeonStartResponse> startResponse =
            await this.Client.PostMsgpack<DmodeDungeonStartResponse>(
                "dmode_dungeon/start",
                new DmodeDungeonStartRequest()
                {
                    CharaId = Charas.Shingen,
                    StartFloorNum = 1,
                    ServitorId = 1,
                    BringEditSkillCharaIdList = new List<Charas>()
                    {
                        Charas.Ranzal,
                        Charas.GalaCleo
                    }
                }
            );

        (await this.GetDungeonState()).Should().Be(DungeonState.WaitingInitEnd);

        DragaliaResponse<DmodeDungeonFloorResponse> floorResponse =
            await this.Client.PostMsgpack<DmodeDungeonFloorResponse>(
                "dmode_dungeon/floor",
                new DmodeDungeonFloorRequest() { DmodePlayRecord = null }
            );

        floorResponse.Data.DmodeFloorData.DmodeAreaInfo.FloorNum.Should().Be(1);
        floorResponse
            .Data.DmodeFloorData.DmodeDungeonOdds.DmodeDungeonItemList.Should()
            .OnlyHaveUniqueItems(item => item.ItemNo);
    }

    [Fact]
    public async Task Start_Skip_FloorHasEquipment()
    {
        this.AddCharacter(Charas.Shingen);

        DbPlayerDmodeInfo oldInfo = this
            .ApiContext.PlayerDmodeInfos.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        await this.Client.PostMsgpack<DmodeDungeonStartResponse>(
            "dmode_dungeon/start",
            new DmodeDungeonStartRequest()
            {
                CharaId = Charas.Shingen,
                StartFloorNum = 30,
                ServitorId = 1,
                BringEditSkillCharaIdList = new List<Charas>() { Charas.Ranzal, Charas.GalaCleo }
            }
        );

        await this.Client.PostMsgpack<DmodeDungeonFloorSkipResponse>("dmode_dungeon/floor_skip");

        (await this.GetDungeonState()).Should().Be(DungeonState.WaitingSkipEnd);

        DragaliaResponse<DmodeDungeonFloorResponse> floorResponse =
            await this.Client.PostMsgpack<DmodeDungeonFloorResponse>(
                "dmode_dungeon/floor",
                new DmodeDungeonFloorRequest() { DmodePlayRecord = null }
            );

        floorResponse.Data.DmodeFloorData.DmodeAreaInfo.FloorNum.Should().Be(30);
        floorResponse
            .Data.DmodeFloorData.DmodeDungeonOdds.DmodeDungeonItemList.Should()
            .OnlyHaveUniqueItems(item => item.ItemNo);
        floorResponse.Data.DmodeFloorData.DmodeUnitInfo.DmodeHoldDragonList.Should().NotBeEmpty();
        floorResponse
            .Data.DmodeFloorData.DmodeDungeonOdds.DmodeDungeonItemList.Should()
            .Contain(x => x.ItemState == DmodeDungeonItemState.EquipWeapon);
        floorResponse
            .Data.DmodeFloorData.DmodeDungeonOdds.DmodeDungeonItemList.Should()
            .Contain(x => x.ItemState == DmodeDungeonItemState.EquipCrest);

        this.ApiContext.PlayerDmodeInfos.AsNoTracking()
            .First(x => x.ViewerId == ViewerId)
            .FloorSkipCount.Should()
            .Be(oldInfo.FloorSkipCount + 1);
    }

    [Fact]
    public async Task Start_Halt_CanResume()
    {
        this.AddCharacter(Charas.Shingen);

        DbPlayerDmodeInfo oldInfo = this
            .ApiContext.PlayerDmodeInfos.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        await this.Client.PostMsgpack<DmodeDungeonStartResponse>(
            "dmode_dungeon/start",
            new DmodeDungeonStartRequest()
            {
                CharaId = Charas.Shingen,
                StartFloorNum = 1,
                ServitorId = 1,
                BringEditSkillCharaIdList = new List<Charas>() { Charas.Ranzal, Charas.GalaCleo }
            }
        );

        await this.Client.PostMsgpack<DmodeDungeonUserHaltResponse>("dmode_dungeon/user_halt");

        this.ApiContext.PlayerDmodeDungeons.AsNoTracking()
            .First(x => x.ViewerId == ViewerId)
            .State.Should()
            .Be(DungeonState.Halting);

        await this.Client.PostMsgpack<DmodeDungeonRestartResponse>("dmode_dungeon/restart");

        (await this.GetDungeonState()).Should().Be(DungeonState.RestartEnd);

        this.ApiContext.PlayerDmodeInfos.AsNoTracking()
            .First(x => x.ViewerId == ViewerId)
            .RecoveryCount.Should()
            .Be(oldInfo.RecoveryCount + 1);
    }

    [Fact]
    public async Task Finish_NoRedisData_ClearsDbEntry()
    {
        await this
            .ApiContext.PlayerDmodeDungeons.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdateAsync(e =>
                e.SetProperty(d => d.Floor, 55)
                    .SetProperty(d => d.State, DungeonState.Halting)
                    .SetProperty(d => d.CharaId, Charas.Berserker)
            );

        DmodeDungeonFinishResponse response = (
            await this.Client.PostMsgpack<DmodeDungeonFinishResponse>(
                "dmode_dungeon/finish",
                new DmodeDungeonFinishRequest() { IsGameOver = false }
            )
        ).Data;

        response.DmodeDungeonState.Should().Be(DungeonState.Waiting);
        response.DmodeIngameResult.Should().BeEquivalentTo(new DmodeIngameResult());
    }

    private async Task<DungeonState> GetDungeonState()
    {
        return (
            await this
                .ApiContext.PlayerDmodeDungeons.AsNoTracking()
                .FirstAsync(x => x.ViewerId == ViewerId)
        ).State;
    }
}
