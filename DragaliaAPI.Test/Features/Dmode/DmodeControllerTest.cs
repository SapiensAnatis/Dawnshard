using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using MockQueryable.Moq;

namespace DragaliaAPI.Test.Features.Dmode;

public class DmodeControllerTest
{
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IStoryRepository> mockStoryRepository;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IStoryService> mockStoryService;
    private readonly Mock<IDateTimeProvider> mockDateTimeProvider;
    private readonly Mock<IDmodeService> mockDmodeService;
    private readonly Mock<IDmodeRepository> mockDmodeRepository;

    private readonly DmodeController dmodeController;

    private readonly DateTimeOffset FixedTime = DateTimeOffset.UtcNow;

    public DmodeControllerTest()
    {
        mockUpdateDataService = new(MockBehavior.Strict);
        mockStoryRepository = new(MockBehavior.Strict);
        mockRewardService = new(MockBehavior.Strict);
        mockStoryService = new(MockBehavior.Strict);
        mockDateTimeProvider = new(MockBehavior.Strict);
        mockDmodeService = new(MockBehavior.Strict);
        mockDmodeRepository = new(MockBehavior.Strict);

        dmodeController = new DmodeController(
            mockUpdateDataService.Object,
            mockStoryRepository.Object,
            mockRewardService.Object,
            mockStoryService.Object,
            mockDateTimeProvider.Object,
            mockDmodeService.Object,
            mockDmodeRepository.Object
        );

        mockDateTimeProvider.SetupGet(x => x.UtcNow).Returns(FixedTime);
    }

    [Fact]
    public async Task GetData_ReturnsData()
    {
        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

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

        List<DmodeStoryList> stories = new() { new DmodeStoryList(1000, 1) };
        mockStoryRepository
            .SetupGet(x => x.DmodeStories)
            .Returns(
                new List<DbPlayerStoryState>()
                {
                    new()
                    {
                        DeviceAccountId = UnitTestUtils.DeviceAccountId,
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

        DmodeGetDataData? resp = (await dmodeController.GetData()).GetData<DmodeGetDataData>();

        resp.Should().NotBeNull();

        resp!.current_server_time.Should().Be(FixedTime);
        resp.dmode_info.Should().BeEquivalentTo(info);
        resp.dmode_chara_list.Should().BeEquivalentTo(charaList);
        resp.dmode_expedition.Should().BeEquivalentTo(expedition);
        resp.dmode_dungeon_info.Should().BeEquivalentTo(dungeonInfo);
        resp.dmode_story_list.Should().BeEquivalentTo(stories);
        resp.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockDmodeService.VerifyAll();
        mockUpdateDataService.VerifyAll();
        mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task Entry_InitializesData()
    {
        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

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

        List<DmodeStoryList> stories = new() { new DmodeStoryList(1000, 1) };
        mockStoryRepository
            .SetupGet(x => x.DmodeStories)
            .Returns(
                new List<DbPlayerStoryState>()
                {
                    new()
                    {
                        DeviceAccountId = UnitTestUtils.DeviceAccountId,
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

        DmodeGetDataData? resp = (await dmodeController.Entry()).GetData<DmodeGetDataData>();

        resp.Should().NotBeNull();

        resp!.current_server_time.Should().Be(FixedTime);
        resp.dmode_info.Should().BeEquivalentTo(info);
        resp.dmode_chara_list.Should().BeEquivalentTo(charaList);
        resp.dmode_expedition.Should().BeEquivalentTo(expedition);
        resp.dmode_dungeon_info.Should().BeEquivalentTo(dungeonInfo);
        resp.dmode_story_list.Should().BeEquivalentTo(stories);
        resp.update_data_list.Should().BeEquivalentTo(updateDataList);

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
        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        DmodeReadStoryData? resp = (
            await dmodeController.ReadStory(new DmodeReadStoryRequest(1000))
        ).GetData<DmodeReadStoryData>();

        resp.Should().NotBeNull();
        resp.dmode_story_reward_list.Should().BeEquivalentTo(rewards);
        resp.duplicate_entity_list.Should().NotBeNull();
        resp.entity_result.Should().BeEquivalentTo(entityResult);
        resp.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockStoryService.VerifyAll();
        mockRewardService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildupServitorPassive_BuildsupServitor()
    {
        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        List<DmodeServitorPassiveList> passiveList =
            new() { new DmodeServitorPassiveList(DmodeServitorPassiveType.Exp, 5) };

        mockDmodeService
            .Setup(x => x.BuildupServitorPassive(It.IsAny<IEnumerable<DmodeServitorPassiveList>>()))
            .ReturnsAsync(passiveList);

        DmodeBuildupServitorPassiveData? resp = (
            await dmodeController.BuildupServitorPassive(
                new DmodeBuildupServitorPassiveRequest(passiveList)
            )
        ).GetData<DmodeBuildupServitorPassiveData>();

        resp.Should().NotBeNull();
        resp.dmode_servitor_passive_list.Should().BeEquivalentTo(passiveList);
        resp.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockUpdateDataService.VerifyAll();
        mockDmodeService.VerifyAll();
    }

    [Fact]
    public async Task ExpeditionStart_StartsExpedition()
    {
        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        List<Charas> charaIdList = new() { Charas.ThePrince, 0, 0, 0 };

        DmodeExpedition expedition =
            new(Charas.ThePrince, 0, 0, 0, FixedTime, 10, ExpeditionState.Playing);
        mockDmodeService
            .Setup(x => x.StartExpedition(10, It.IsAny<IEnumerable<Charas>>()))
            .ReturnsAsync(expedition);

        DmodeExpeditionStartData? resp = (
            await dmodeController.ExpeditionStart(new DmodeExpeditionStartRequest(10, charaIdList))
        ).GetData<DmodeExpeditionStartData>();

        mockUpdateDataService.VerifyAll();
        mockDmodeService.VerifyAll();
    }

    [Fact]
    public async Task ExpeditionFinish_FinishesExpedition()
    {
        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        EntityResult entityResult = new();
        mockRewardService.Setup(x => x.GetEntityResult()).Returns(entityResult);

        DmodeExpedition expedition =
            new(Charas.ThePrince, 0, 0, 0, FixedTime, 10, ExpeditionState.Waiting);
        DmodeIngameResult ingameResult =
            new() { chara_id_list = new Charas[] { Charas.ThePrince, 0, 0, 0 }, floor_num = 10 };

        mockDmodeService
            .Setup(x => x.FinishExpedition(false))
            .ReturnsAsync((expedition, ingameResult));

        DmodeExpeditionFinishData? resp = (
            await dmodeController.ExpeditionFinish()
        ).GetData<DmodeExpeditionFinishData>();

        resp.Should().NotBeNull();
        resp.dmode_expedition.Should().BeEquivalentTo(expedition);
        resp.dmode_ingame_result.Should().BeEquivalentTo(ingameResult);
        resp.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockRewardService.VerifyAll();
        mockUpdateDataService.VerifyAll();
        mockDmodeService.VerifyAll();
    }

    [Fact]
    public async Task ExpeditionForceFinish_FinishesExpeditionForced()
    {
        UpdateDataList updateDataList = new();
        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        EntityResult entityResult = new();
        mockRewardService.Setup(x => x.GetEntityResult()).Returns(entityResult);

        DmodeExpedition expedition =
            new(Charas.ThePrince, 0, 0, 0, FixedTime, 10, ExpeditionState.Waiting);
        DmodeIngameResult ingameResult =
            new() { chara_id_list = new Charas[] { Charas.ThePrince, 0, 0, 0 } };

        mockDmodeService
            .Setup(x => x.FinishExpedition(true))
            .ReturnsAsync((expedition, ingameResult));

        DmodeExpeditionForceFinishData? resp = (
            await dmodeController.ExpeditionForceFinish()
        ).GetData<DmodeExpeditionForceFinishData>();

        resp.Should().NotBeNull();
        resp.dmode_expedition.Should().BeEquivalentTo(expedition);
        resp.dmode_ingame_result.Should().BeEquivalentTo(ingameResult);
        resp.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockRewardService.VerifyAll();
        mockUpdateDataService.VerifyAll();
        mockDmodeService.VerifyAll();
    }
}
