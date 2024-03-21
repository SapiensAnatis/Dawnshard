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
        new() { MaterialList = new List<MaterialList>() { new(Materials.Squishums, 5000) } };

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

        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);
    }

    [Fact]
    public async Task Start_StartsDungeon()
    {
        DungeonState state = DungeonState.WaitingInitEnd;
        DmodeIngameData ingameData = new() { UniqueKey = "unique" };

        DmodeDungeonStartRequest request = new(Charas.ThePrince, 0, 0, new List<Charas>());

        mockDmodeDungeonService
            .Setup(x =>
                x.StartDungeon(
                    request.CharaId,
                    request.StartFloorNum,
                    request.ServitorId,
                    request.BringEditSkillCharaIdList
                )
            )
            .ReturnsAsync((state, ingameData));

        DmodeDungeonStartResponse? resp = (
            await dmodeDungeonController.Start(request, default)
        ).GetData<DmodeDungeonStartResponse>();

        resp.Should().NotBeNull();
        resp!.DmodeDungeonState.Should().Be(state);
        resp.DmodeIngameData.Should().Be(ingameData);

        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task Restart_RestartsDungeon()
    {
        DungeonState state = DungeonState.RestartEnd;
        DmodeIngameData ingameData = new() { UniqueKey = "unique" };

        mockDmodeDungeonService.Setup(x => x.RestartDungeon()).ReturnsAsync((state, ingameData));

        DmodeDungeonRestartResponse? resp = (
            await dmodeDungeonController.Restart(default)
        ).GetData<DmodeDungeonRestartResponse>();

        resp.Should().NotBeNull();
        resp!.DmodeDungeonState.Should().Be(state);
        resp.DmodeIngameData.Should().Be(ingameData);

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
                DmodeAreaInfo = new AtgenDmodeAreaInfo()
                {
                    FloorNum = 50,
                    CurrentAreaId = 10,
                    CurrentAreaThemeId = 100
                }
            };

        DmodePlayRecord playRecord = new();

        mockDmodeDungeonService
            .Setup(x => x.ProgressToNextFloor(It.IsAny<DmodePlayRecord>()))
            .ReturnsAsync((state, floorData));

        DmodeDungeonFloorResponse? resp = (
            await dmodeDungeonController.Floor(new DmodeDungeonFloorRequest(playRecord), default)
        ).GetData<DmodeDungeonFloorResponse>();

        resp.Should().NotBeNull();
        resp!.DmodeDungeonState.Should().Be(state);
        resp.DmodeFloorData.Should().BeEquivalentTo(floorData);
        resp.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Finish_FinishesDungeon(bool isGameOver)
    {
        DungeonState state = DungeonState.Waiting;
        DmodeIngameResult ingameResult = new() { FloorNum = 50, TakeDmodePoint1 = 5000 };

        mockDmodeDungeonService
            .Setup(x => x.FinishDungeon(isGameOver))
            .ReturnsAsync((state, ingameResult));

        EntityResult entityResult = new();
        mockRewardService.Setup(x => x.GetEntityResult()).Returns(entityResult);

        DmodeDungeonFinishResponse? resp = (
            await dmodeDungeonController.Finish(new DmodeDungeonFinishRequest(isGameOver), default)
        ).GetData<DmodeDungeonFinishResponse>();

        resp.Should().NotBeNull();
        resp!.DmodeDungeonState.Should().Be(state);
        resp.DmodeIngameResult.Should().Be(ingameResult);
        resp.EntityResult.Should().Be(entityResult);
        resp.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockRewardService.VerifyAll();
        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task FloorSkip_SkipsFloor()
    {
        DungeonState state = DungeonState.WaitingSkipEnd;

        mockDmodeDungeonService.Setup(x => x.SkipFloor()).ReturnsAsync(state);

        DmodeDungeonFloorSkipResponse? resp = (
            await dmodeDungeonController.FloorSkip(default)
        ).GetData<DmodeDungeonFloorSkipResponse>();

        resp.Should().NotBeNull();
        resp!.DmodeDungeonState.Should().Be(state);
        resp.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task UserHalt_HaltsDungeon()
    {
        DungeonState state = DungeonState.Halting;

        mockDmodeDungeonService.Setup(x => x.HaltDungeon(true)).ReturnsAsync(state);

        DmodeDungeonUserHaltResponse? resp = (
            await dmodeDungeonController.UserHalt(default)
        ).GetData<DmodeDungeonUserHaltResponse>();

        resp.Should().NotBeNull();
        resp!.DmodeDungeonState.Should().Be(state);
        resp.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task SystemHalt_HaltsDungeon()
    {
        DungeonState state = DungeonState.Halting;

        mockDmodeDungeonService.Setup(x => x.HaltDungeon(false)).ReturnsAsync(state);

        DmodeDungeonSystemHaltResponse? resp = (
            await dmodeDungeonController.SystemHalt(default)
        ).GetData<DmodeDungeonSystemHaltResponse>();

        resp.Should().NotBeNull();
        resp!.DmodeDungeonState.Should().Be(state);
        resp.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockDmodeDungeonService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }
}
