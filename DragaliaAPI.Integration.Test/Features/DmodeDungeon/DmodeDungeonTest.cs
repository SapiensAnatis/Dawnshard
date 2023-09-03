using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
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

        DragaliaResponse<DmodeDungeonStartData> startResponse =
            await this.Client.PostMsgpack<DmodeDungeonStartData>(
                "dmode_dungeon/start",
                new DmodeDungeonStartRequest()
                {
                    chara_id = Charas.Shingen,
                    start_floor_num = 1,
                    servitor_id = 1,
                    bring_edit_skill_chara_id_list = new List<Charas>()
                    {
                        Charas.Ranzal,
                        Charas.GalaCleo
                    }
                }
            );

        (await this.GetDungeonState()).Should().Be(DungeonState.WaitingInitEnd);

        DragaliaResponse<DmodeDungeonFloorData> floorResponse =
            await this.Client.PostMsgpack<DmodeDungeonFloorData>(
                "dmode_dungeon/floor",
                new DmodeDungeonFloorRequest() { dmode_play_record = null }
            );

        floorResponse.data.dmode_floor_data.dmode_area_info.floor_num.Should().Be(1);
        floorResponse.data.dmode_floor_data.dmode_dungeon_odds.dmode_dungeon_item_list
            .Should()
            .OnlyHaveUniqueItems(item => item.item_no);
    }

    [Fact]
    public async Task Start_Skip_FloorHasEquipment()
    {
        this.AddCharacter(Charas.Shingen);

        DbPlayerDmodeInfo oldInfo = this.ApiContext.PlayerDmodeInfos
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId);

        await this.Client.PostMsgpack<DmodeDungeonStartData>(
            "dmode_dungeon/start",
            new DmodeDungeonStartRequest()
            {
                chara_id = Charas.Shingen,
                start_floor_num = 30,
                servitor_id = 1,
                bring_edit_skill_chara_id_list = new List<Charas>()
                {
                    Charas.Ranzal,
                    Charas.GalaCleo
                }
            }
        );

        await this.Client.PostMsgpack<DmodeDungeonFloorSkipData>(
            "dmode_dungeon/floor_skip",
            new DmodeDungeonFloorSkipRequest() { }
        );

        (await this.GetDungeonState()).Should().Be(DungeonState.WaitingSkipEnd);

        DragaliaResponse<DmodeDungeonFloorData> floorResponse =
            await this.Client.PostMsgpack<DmodeDungeonFloorData>(
                "dmode_dungeon/floor",
                new DmodeDungeonFloorRequest() { dmode_play_record = null }
            );

        floorResponse.data.dmode_floor_data.dmode_area_info.floor_num.Should().Be(30);
        floorResponse.data.dmode_floor_data.dmode_dungeon_odds.dmode_dungeon_item_list
            .Should()
            .OnlyHaveUniqueItems(item => item.item_no);
        floorResponse.data.dmode_floor_data.dmode_unit_info.dmode_hold_dragon_list
            .Should()
            .NotBeEmpty();
        floorResponse.data.dmode_floor_data.dmode_dungeon_odds.dmode_dungeon_item_list
            .Should()
            .Contain(x => x.item_state == DmodeDungeonItemState.EquipWeapon);
        floorResponse.data.dmode_floor_data.dmode_dungeon_odds.dmode_dungeon_item_list
            .Should()
            .Contain(x => x.item_state == DmodeDungeonItemState.EquipCrest);

        this.ApiContext.PlayerDmodeInfos
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId)
            .FloorSkipCount.Should()
            .Be(oldInfo.FloorSkipCount + 1);
    }

    [Fact]
    public async Task Start_Halt_CanResume()
    {
        this.AddCharacter(Charas.Shingen);

        DbPlayerDmodeInfo oldInfo = this.ApiContext.PlayerDmodeInfos
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId);

        await this.Client.PostMsgpack<DmodeDungeonStartData>(
            "dmode_dungeon/start",
            new DmodeDungeonStartRequest()
            {
                chara_id = Charas.Shingen,
                start_floor_num = 1,
                servitor_id = 1,
                bring_edit_skill_chara_id_list = new List<Charas>()
                {
                    Charas.Ranzal,
                    Charas.GalaCleo
                }
            }
        );

        await this.Client.PostMsgpack<DmodeDungeonUserHaltData>(
            "dmode_dungeon/user_halt",
            new DmodeDungeonUserHaltRequest() { }
        );

        this.ApiContext.PlayerDmodeDungeons
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId)
            .State.Should()
            .Be(DungeonState.Halting);

        await this.Client.PostMsgpack<DmodeDungeonRestartData>(
            "dmode_dungeon/restart",
            new DmodeDungeonRestartRequest() { }
        );

        (await this.GetDungeonState()).Should().Be(DungeonState.RestartEnd);

        this.ApiContext.PlayerDmodeInfos
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId)
            .RecoveryCount.Should()
            .Be(oldInfo.RecoveryCount + 1);
    }

    private async Task<DungeonState> GetDungeonState()
    {
        return (
            await this.ApiContext.PlayerDmodeDungeons
                .AsNoTracking()
                .FirstAsync(x => x.DeviceAccountId == DeviceAccountId)
        ).State;
    }
}
