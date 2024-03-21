using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Time.Testing;
using MockQueryable.Moq;

namespace DragaliaAPI.Test.Features.Dmode;

public class DmodeControllerTest
{
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IStoryRepository> mockStoryRepository;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IStoryService> mockStoryService;
    private readonly FakeTimeProvider mockDateTimeProvider;
    private readonly Mock<IDmodeService> mockDmodeService;
    private readonly Mock<IDmodeRepository> mockDmodeRepository;

    private readonly DmodeController dmodeController;

    private readonly DateTimeOffset fixedTime = DateTimeOffset.UtcNow;

    public DmodeControllerTest()
    {
        mockUpdateDataService = new(MockBehavior.Strict);
        mockStoryRepository = new(MockBehavior.Strict);
        mockRewardService = new(MockBehavior.Strict);
        mockStoryService = new(MockBehavior.Strict);
        mockDateTimeProvider = new();
        mockDmodeService = new(MockBehavior.Strict);
        mockDmodeRepository = new(MockBehavior.Strict);

        dmodeController = new DmodeController(
            mockUpdateDataService.Object,
            mockStoryRepository.Object,
            mockRewardService.Object,
            mockStoryService.Object,
            mockDateTimeProvider,
            mockDmodeService.Object,
            mockDmodeRepository.Object
        );

        mockDateTimeProvider.SetUtcNow(this.fixedTime);
    }

    [Fact]
    public async Task GetData_ReturnsData()
    {
        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        DmodeInfo info =
            new(50, 0, DateTimeOffset.UnixEpoch, 0, DateTimeOffset.UnixEpoch, 100, 50, true);
        mockDmodeService.Setup(x => x.GetInfo()).ReturnsAsync(info);

        List<DmodeCharaList> charaList =
            new()
            {
                new DmodeCharaList(Charas.ThePrince, 50, 1, Charas.Nadine, Charas.Nadine, 0, 1000)
            };
        mockDmodeService.Setup(x => x.GetCharaList()).ReturnsAsync(charaList);

        DmodeExpedition expedition =
            new(Charas.ThePrince, 0, 0, 0, DateTimeOffset.UnixEpoch, 0, ExpeditionState.Waiting);
        mockDmodeService.Setup(x => x.GetExpedition()).ReturnsAsync(expedition);

        DmodeDungeonInfo dungeonInfo = new(0, 0, 0, 0, false, DungeonState.Waiting);
        mockDmodeService.Setup(x => x.GetDungeonInfo()).ReturnsAsync(dungeonInfo);

        List<DmodeStoryList> stories = new() { new DmodeStoryList(1000, true) };
        mockStoryRepository
            .SetupGet(x => x.DmodeStories)
            .Returns(
                new List<DbPlayerStoryState>()
                {
                    new()
                    {
                        ViewerId = UnitTestUtils.ViewerId,
                        StoryId = 1000,
                        StoryType = StoryTypes.DungeonMode,
                        State = StoryState.Read
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        List<DmodeServitorPassiveList> passiveList =
            new() { new DmodeServitorPassiveList(DmodeServitorPassiveType.Exp, 5) };
        mockDmodeService.Setup(x => x.GetServitorPassiveList()).ReturnsAsync(passiveList);

        DmodeGetDataResponse? resp = (
            await dmodeController.GetData(default)
        ).GetData<DmodeGetDataResponse>();

        resp.Should().NotBeNull();

        resp!.CurrentServerTime.Should().Be(this.fixedTime);
        resp.DmodeInfo.Should().BeEquivalentTo(info);
        resp.DmodeCharaList.Should().BeEquivalentTo(charaList);
        resp.DmodeExpedition.Should().BeEquivalentTo(expedition);
        resp.DmodeDungeonInfo.Should().BeEquivalentTo(dungeonInfo);
        resp.DmodeStoryList.Should().BeEquivalentTo(stories);
        resp.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockDmodeService.VerifyAll();
        mockUpdateDataService.VerifyAll();
        mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task Entry_InitializesData()
    {
        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        mockDmodeRepository.Setup(x => x.InitializeForPlayer());

        DmodeInfo info =
            new(50, 0, DateTimeOffset.UnixEpoch, 0, DateTimeOffset.UnixEpoch, 100, 50, true);
        mockDmodeService.Setup(x => x.GetInfo()).ReturnsAsync(info);

        List<DmodeCharaList> charaList =
            new()
            {
                new DmodeCharaList(Charas.ThePrince, 50, 1, Charas.Nadine, Charas.Nadine, 0, 1000)
            };
        mockDmodeService.Setup(x => x.GetCharaList()).ReturnsAsync(charaList);

        DmodeExpedition expedition =
            new(Charas.ThePrince, 0, 0, 0, DateTimeOffset.UnixEpoch, 0, ExpeditionState.Waiting);
        mockDmodeService.Setup(x => x.GetExpedition()).ReturnsAsync(expedition);

        DmodeDungeonInfo dungeonInfo = new(0, 0, 0, 0, false, DungeonState.Waiting);
        mockDmodeService.Setup(x => x.GetDungeonInfo()).ReturnsAsync(dungeonInfo);

        List<DmodeStoryList> stories = new() { new DmodeStoryList(1000, true) };
        mockStoryRepository
            .SetupGet(x => x.DmodeStories)
            .Returns(
                new List<DbPlayerStoryState>()
                {
                    new()
                    {
                        ViewerId = UnitTestUtils.ViewerId,
                        StoryId = 1000,
                        StoryType = StoryTypes.DungeonMode,
                        State = StoryState.Read
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        List<DmodeServitorPassiveList> passiveList =
            new() { new DmodeServitorPassiveList(DmodeServitorPassiveType.Exp, 5) };
        mockDmodeService.Setup(x => x.GetServitorPassiveList()).ReturnsAsync(passiveList);

        DmodeGetDataResponse? resp = (
            await dmodeController.Entry(default)
        ).GetData<DmodeGetDataResponse>();

        resp.Should().NotBeNull();

        resp!.CurrentServerTime.Should().Be(this.fixedTime);
        resp.DmodeInfo.Should().BeEquivalentTo(info);
        resp.DmodeCharaList.Should().BeEquivalentTo(charaList);
        resp.DmodeExpedition.Should().BeEquivalentTo(expedition);
        resp.DmodeDungeonInfo.Should().BeEquivalentTo(dungeonInfo);
        resp.DmodeStoryList.Should().BeEquivalentTo(stories);
        resp.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockDmodeService.VerifyAll();
        mockUpdateDataService.VerifyAll();
        mockStoryRepository.VerifyAll();
        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task ReadStory_ReadsStory()
    {
        List<AtgenBuildEventRewardEntityList> rewards =
            new() { new(EntityTypes.Material, 1000, 10) };

        mockStoryService
            .Setup(x => x.ReadStory(StoryTypes.DungeonMode, 1000))
            .ReturnsAsync(rewards);

        EntityResult entityResult = new();
        mockRewardService.Setup(x => x.GetEntityResult()).Returns(entityResult);

        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        DmodeReadStoryResponse? resp = (
            await dmodeController.ReadStory(new DmodeReadStoryRequest(1000), default)
        ).GetData<DmodeReadStoryResponse>();

        resp.Should().NotBeNull();
        resp!.DmodeStoryRewardList.Should().BeEquivalentTo(rewards);
        resp.DuplicateEntityList.Should().NotBeNull();
        resp.EntityResult.Should().BeEquivalentTo(entityResult);
        resp.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockStoryService.VerifyAll();
        mockRewardService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildupServitorPassive_BuildsupServitor()
    {
        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        List<DmodeServitorPassiveList> passiveList =
            new() { new DmodeServitorPassiveList(DmodeServitorPassiveType.Exp, 5) };

        mockDmodeService
            .Setup(x => x.BuildupServitorPassive(It.IsAny<IEnumerable<DmodeServitorPassiveList>>()))
            .ReturnsAsync(passiveList);

        DmodeBuildupServitorPassiveResponse? resp = (
            await dmodeController.BuildupServitorPassive(
                new DmodeBuildupServitorPassiveRequest(passiveList),
                default
            )
        ).GetData<DmodeBuildupServitorPassiveResponse>();

        resp.Should().NotBeNull();
        resp!.DmodeServitorPassiveList.Should().BeEquivalentTo(passiveList);
        resp.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockUpdateDataService.VerifyAll();
        mockDmodeService.VerifyAll();
    }

    [Fact]
    public async Task ExpeditionStart_StartsExpedition()
    {
        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        List<Charas> charaIdList = new() { Charas.ThePrince, 0, 0, 0 };

        DmodeExpedition expedition =
            new(Charas.ThePrince, 0, 0, 0, this.fixedTime, 10, ExpeditionState.Playing);
        mockDmodeService
            .Setup(x => x.StartExpedition(10, It.IsAny<IEnumerable<Charas>>()))
            .ReturnsAsync(expedition);

        DmodeExpeditionStartResponse? resp = (
            await dmodeController.ExpeditionStart(
                new DmodeExpeditionStartRequest(10, charaIdList),
                default
            )
        ).GetData<DmodeExpeditionStartResponse>();

        mockUpdateDataService.VerifyAll();
        mockDmodeService.VerifyAll();
    }

    [Fact]
    public async Task ExpeditionFinish_FinishesExpedition()
    {
        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        EntityResult entityResult = new();
        mockRewardService.Setup(x => x.GetEntityResult()).Returns(entityResult);

        DmodeExpedition expedition =
            new(Charas.ThePrince, 0, 0, 0, this.fixedTime, 10, ExpeditionState.Waiting);
        DmodeIngameResult ingameResult =
            new() { CharaIdList = new Charas[] { Charas.ThePrince, 0, 0, 0 }, FloorNum = 10 };

        mockDmodeService
            .Setup(x => x.FinishExpedition(false))
            .ReturnsAsync((expedition, ingameResult));

        DmodeExpeditionFinishResponse? resp = (
            await dmodeController.ExpeditionFinish(default)
        ).GetData<DmodeExpeditionFinishResponse>();

        resp.Should().NotBeNull();
        resp!.DmodeExpedition.Should().BeEquivalentTo(expedition);
        resp.DmodeIngameResult.Should().BeEquivalentTo(ingameResult);
        resp.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockRewardService.VerifyAll();
        mockUpdateDataService.VerifyAll();
        mockDmodeService.VerifyAll();
    }

    [Fact]
    public async Task ExpeditionForceFinish_FinishesExpeditionForced()
    {
        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        EntityResult entityResult = new();
        mockRewardService.Setup(x => x.GetEntityResult()).Returns(entityResult);

        DmodeExpedition expedition =
            new(Charas.ThePrince, 0, 0, 0, this.fixedTime, 10, ExpeditionState.Waiting);
        DmodeIngameResult ingameResult =
            new() { CharaIdList = new Charas[] { Charas.ThePrince, 0, 0, 0 } };

        mockDmodeService
            .Setup(x => x.FinishExpedition(true))
            .ReturnsAsync((expedition, ingameResult));

        DmodeExpeditionForceFinishResponse? resp = (
            await dmodeController.ExpeditionForceFinish(default)
        ).GetData<DmodeExpeditionForceFinishResponse>();

        resp.Should().NotBeNull();
        resp!.DmodeExpedition.Should().BeEquivalentTo(expedition);
        resp.DmodeIngameResult.Should().BeEquivalentTo(ingameResult);
        resp.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockRewardService.VerifyAll();
        mockUpdateDataService.VerifyAll();
        mockDmodeService.VerifyAll();
    }
}
