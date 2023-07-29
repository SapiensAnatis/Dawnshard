using DragaliaAPI.Features.DmodeDungeon;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Test.Features.Dmode;

public class DmodeDungeonControllerTest
{
    private readonly Mock<IDmodeDungeonService> mockDmodeDungeonService;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IRewardService> mockRewardService;

    private readonly DmodeDungeonController dmodeDungeonController;

    private readonly UpdateDataList updateDataList =
        new() { material_list = new List<MaterialList>() { new(Materials.Squishums, 5000) } };

    public DmodeDungeonControllerTest()
    {
        mockDmodeDungeonService = new(MockBehavior.Strict);
        mockUpdateDataService = new(MockBehavior.Strict);
        mockRewardService = new(MockBehavior.Strict);

        dmodeDungeonController = new(
            mockDmodeDungeonService.Object,
            mockUpdateDataService.Object,
            mockRewardService.Object
        );

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);
    }

    [Fact]
    public async Task Start_StartsDungeon()
    {
        DungeonState state = DungeonState.WaitingInitEnd;
        DmodeIngameData ingameData = new() { unique_key = "unique" };

        DmodeDungeonStartRequest request = new(Charas.ThePrince, 0, 0, new List<Charas>());

        mockDmodeDungeonService
            .Setup(
                x =>
                    x.StartDungeon(
                        request.chara_id,
                        request.start_floor_num,
                        request.servitor_id,
                        request.bring_edit_skill_chara_id_list
                    )
            )
            .ReturnsAsync((state, ingameData));

        DmodeDungeonStartData? resp = (
            await dmodeDungeonController.Start(request)
        ).GetData<DmodeDungeonStartData>();

        resp.Should().NotBeNull();
        resp!.dmode_dungeon_state.Should().Be(state);
        resp.dmode_ingame_data.Should().Be(ingameData);

        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task Restart_RestartsDungeon()
    {
        DungeonState state = DungeonState.RestartEnd;
        DmodeIngameData ingameData = new() { unique_key = "unique" };

        mockDmodeDungeonService.Setup(x => x.RestartDungeon()).ReturnsAsync((state, ingameData));

        DmodeDungeonRestartData? resp = (
            await dmodeDungeonController.Restart()
        ).GetData<DmodeDungeonRestartData>();

        resp.Should().NotBeNull();
        resp!.dmode_dungeon_state.Should().Be(state);
        resp.dmode_ingame_data.Should().Be(ingameData);

        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task Floor_GeneratesFloor()
    {
        DungeonState state = DungeonState.Playing;
        DmodeFloorData floorData =
            new()
            {
                dmode_area_info = new AtgenDmodeAreaInfo()
                {
                    floor_num = 50,
                    current_area_id = 10,
                    current_area_theme_id = 100
                }
            };

        DmodePlayRecord playRecord = new();

        mockDmodeDungeonService
            .Setup(x => x.ProgressToNextFloor(It.IsAny<DmodePlayRecord>()))
            .ReturnsAsync((state, floorData));

        DmodeDungeonFloorData? resp = (
            await dmodeDungeonController.Floor(new DmodeDungeonFloorRequest(playRecord))
        ).GetData<DmodeDungeonFloorData>();

        resp.Should().NotBeNull();
        resp!.dmode_dungeon_state.Should().Be(state);
        resp.dmode_floor_data.Should().BeEquivalentTo(floorData);
        resp.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Finish_FinishesDungeon(bool isGameOver)
    {
        DungeonState state = DungeonState.Waiting;
        DmodeIngameResult ingameResult = new() { floor_num = 50, take_dmode_point_1 = 5000 };

        mockDmodeDungeonService
            .Setup(x => x.FinishDungeon(isGameOver))
            .ReturnsAsync((state, ingameResult));

        EntityResult entityResult = new();
        mockRewardService.Setup(x => x.GetEntityResult()).Returns(entityResult);

        DmodeDungeonFinishData? resp = (
            await dmodeDungeonController.Finish(new DmodeDungeonFinishRequest(isGameOver))
        ).GetData<DmodeDungeonFinishData>();

        resp.Should().NotBeNull();
        resp!.dmode_dungeon_state.Should().Be(state);
        resp.dmode_ingame_result.Should().Be(ingameResult);
        resp.entity_result.Should().Be(entityResult);
        resp.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockRewardService.VerifyAll();
        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task FloorSkip_SkipsFloor()
    {
        DungeonState state = DungeonState.WaitingSkipEnd;

        mockDmodeDungeonService.Setup(x => x.SkipFloor()).ReturnsAsync(state);

        DmodeDungeonFloorSkipData? resp = (
            await dmodeDungeonController.FloorSkip()
        ).GetData<DmodeDungeonFloorSkipData>();

        resp.Should().NotBeNull();
        resp!.dmode_dungeon_state.Should().Be(state);
        resp.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task UserHalt_HaltsDungeon()
    {
        DungeonState state = DungeonState.Halting;

        mockDmodeDungeonService.Setup(x => x.HaltDungeon(true)).ReturnsAsync(state);

        DmodeDungeonUserHaltData? resp = (
            await dmodeDungeonController.UserHalt()
        ).GetData<DmodeDungeonUserHaltData>();

        resp.Should().NotBeNull();
        resp!.dmode_dungeon_state.Should().Be(state);
        resp.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task SystemHalt_HaltsDungeon()
    {
        DungeonState state = DungeonState.Halting;

        mockDmodeDungeonService.Setup(x => x.HaltDungeon(false)).ReturnsAsync(state);

        DmodeDungeonSystemHaltData? resp = (
            await dmodeDungeonController.SystemHalt()
        ).GetData<DmodeDungeonSystemHaltData>();

        resp.Should().NotBeNull();
        resp!.dmode_dungeon_state.Should().Be(state);
        resp.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }
}
