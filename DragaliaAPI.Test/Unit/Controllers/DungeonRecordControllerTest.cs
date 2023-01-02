using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using static DragaliaAPI.Test.TestUtils;

namespace DragaliaAPI.Test.Unit.Controllers;

public class DungeonRecordControllerTest
{
    private readonly DungeonRecordController dungeonRecordController;
    private readonly Mock<IQuestRepository> mockQuestRepository;
    private readonly Mock<IDungeonService> mockDungeonService;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;

    private const string dungeonKey = "key";
    private const int questId = 320180103;
    private const int clearTime = 60;

    public DungeonRecordControllerTest()
    {
        this.mockQuestRepository = new(MockBehavior.Strict);
        this.mockDungeonService = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockInventoryRepository = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);

        this.dungeonRecordController = new(
            mockQuestRepository.Object,
            mockDungeonService.Object,
            mockUserDataRepository.Object,
            mockInventoryRepository.Object,
            mockUpdateDataService.Object
        );

        this.dungeonRecordController.SetupMockContext();

        this.mockDungeonService
            .Setup(x => x.FinishDungeon(dungeonKey))
            .ReturnsAsync(
                new DungeonSession()
                {
                    Party = new List<PartySettingList>(),
                    QuestData = new QuestData(
                        Id: questId,
                        QuestPlayModeType: QuestPlayModeTypes.None,
                        LimitedElementalType: 0,
                        LimitedElementalType2: 0,
                        LimitedWeaponTypePatternId: 0,
                        PayStaminaSingle: 0,
                        PayStaminaMulti: 0,
                        DungeonType: DungeonTypes.None,
                        Scene01: "",
                        AreaName01: "",
                        Scene02: "",
                        AreaName02: "",
                        Scene03: "",
                        AreaName03: "",
                        Scene04: "",
                        AreaName04: "",
                        Scene05: "",
                        AreaName05: "",
                        Scene06: "",
                        AreaName06: ""
                    )
                }
            );

        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new() { DeviceAccountId = DeviceAccountId, QuestId = questId }
                }.AsQueryable().BuildMock());

        this.mockUserDataRepository
            .Setup(x => x.AddTutorialFlag(DeviceAccountId, 1022))
            .ReturnsAsync(new DbPlayerUserData { });

        this.mockQuestRepository
            .Setup(x => x.CompleteQuest(DeviceAccountId, questId, It.IsAny<float>()))
            .ReturnsAsync(
                new DbQuest()
                {
                    DeviceAccountId = DeviceAccountId,
                    QuestId = questId,
                    IsMissionClear1 = true,
                    IsMissionClear2 = true,
                    IsMissionClear3 = true
                }
            );

        this.mockUserDataRepository
            .Setup(x => x.GetUserData(DeviceAccountId))
            .Returns(new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        Name = "Euden",
                        ViewerId = 1
                    }
                }.AsQueryable().BuildMock());

        this.mockInventoryRepository
            .Setup(
                x => x.AddMaterials(DeviceAccountId, It.IsAny<List<Materials>>(), It.IsAny<int>())
            )
            .Returns(Task.FromResult(0));

        this.mockUpdateDataService
            .Setup(x => x.GetUpdateDataList(DeviceAccountId))
            .Returns(new UpdateDataList() { });

        this.mockQuestRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);
    }

    // Tests that CompleteQuest returning the same clear time as GetQuests marks time as best clear time
    [Fact]
    public async Task BestClearDisplaysTimeAsBestTime()
    {
        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        BestClearTime = clearTime
                    }
                }.AsQueryable().BuildMock());

        this.mockQuestRepository
            .Setup(x => x.CompleteQuest(DeviceAccountId, questId, It.IsAny<float>()))
            .ReturnsAsync(
                new DbQuest()
                {
                    DeviceAccountId = DeviceAccountId,
                    QuestId = questId,
                    BestClearTime = clearTime
                }
            );

        DungeonRecordRecordRequest request =
            new()
            {
                dungeon_key = dungeonKey,
                play_record = new() { time = clearTime }
            };

        ActionResult<DragaliaResponse<object>> response = await this.dungeonRecordController.Record(
            request
        );

        DungeonRecordRecordData? data = response.GetData<DungeonRecordRecordData>();
        data.Should().NotBeNull();

        data!.ingame_result_data.clear_time.Should().Be(clearTime);
        data!.ingame_result_data.is_best_clear_time.Should().BeTrue();

        this.mockQuestRepository.VerifyAll();
        this.mockDungeonService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    // Tests that CompleteQuest returning a different time as GetQuests doesn't mark as best clear time
    // (in theory shouldn't have case where GetQuests gives faster time than CompleteQuests)
    [Fact]
    public async Task SlowerClearDoesNotDisplayTimeAsBestTime()
    {
        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        BestClearTime = clearTime
                    }
                }.AsQueryable().BuildMock());

        this.mockQuestRepository
            .Setup(x => x.CompleteQuest(DeviceAccountId, questId, It.IsAny<float>()))
            .ReturnsAsync(
                new DbQuest()
                {
                    DeviceAccountId = DeviceAccountId,
                    QuestId = questId,
                    BestClearTime = clearTime - 1
                }
            );

        DungeonRecordRecordRequest request =
            new()
            {
                dungeon_key = dungeonKey,
                play_record = new() { time = clearTime }
            };

        ActionResult<DragaliaResponse<object>> response = await this.dungeonRecordController.Record(
            request
        );

        DungeonRecordRecordData? data = response.GetData<DungeonRecordRecordData>();
        data.Should().NotBeNull();

        data!.ingame_result_data.clear_time.Should().Be(clearTime);
        data!.ingame_result_data.is_best_clear_time.Should().BeFalse();

        this.mockQuestRepository.VerifyAll();
        this.mockDungeonService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    // Tests first time clear with all missions complete gives full rewards of 25 wyrmite total
    [Fact]
    public async Task FirstClearFullClearGivesAllMissions()
    {
        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        PlayCount = 0,
                        IsMissionClear1 = false,
                        IsMissionClear2 = false,
                        IsMissionClear3 = false
                    }
                }.AsQueryable().BuildMock());

        DungeonRecordRecordRequest request = new() { dungeon_key = dungeonKey };

        ActionResult<DragaliaResponse<object>> response = await this.dungeonRecordController.Record(
            request
        );

        DungeonRecordRecordData? data = response.GetData<DungeonRecordRecordData>();
        data.Should().NotBeNull();

        data!.ingame_result_data.reward_record.first_clear_set
            .Should()
            .BeEquivalentTo(new List<AtgenFirstClearSet>() { this.CreateClearReward() });

        data!.ingame_result_data.reward_record.missions_clear_set
            .Should()
            .BeEquivalentTo(
                new List<AtgenMissionsClearSet>()
                {
                    this.CreateMissionReward(1),
                    this.CreateMissionReward(2),
                    this.CreateMissionReward(3)
                }
            );

        data!.ingame_result_data.reward_record.mission_complete
            .Should()
            .BeEquivalentTo(new List<AtgenFirstClearSet>() { this.CreateClearReward() });

        this.mockQuestRepository.VerifyAll();
        this.mockDungeonService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    // Tests that first clearing all missions but not first clear gives all missions but not first
    // clear bonus for a total of 20 wyrmite
    [Fact]
    public async Task NotFirstClearFullClearGivesAllMissionsButNotFirstClear()
    {
        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        PlayCount = 1,
                        IsMissionClear1 = false,
                        IsMissionClear2 = false,
                        IsMissionClear3 = false
                    }
                }.AsQueryable().BuildMock());

        DungeonRecordRecordRequest request = new() { dungeon_key = dungeonKey };

        ActionResult<DragaliaResponse<object>> response = await this.dungeonRecordController.Record(
            request
        );

        DungeonRecordRecordData? data = response.GetData<DungeonRecordRecordData>();
        data.Should().NotBeNull();

        data!.ingame_result_data.reward_record.first_clear_set
            .Should()
            .BeEquivalentTo(new List<AtgenFirstClearSet>() { });

        data!.ingame_result_data.reward_record.missions_clear_set
            .Should()
            .BeEquivalentTo(
                new List<AtgenMissionsClearSet>()
                {
                    this.CreateMissionReward(1),
                    this.CreateMissionReward(2),
                    this.CreateMissionReward(3)
                }
            );

        data!.ingame_result_data.reward_record.mission_complete
            .Should()
            .BeEquivalentTo(new List<AtgenFirstClearSet>() { this.CreateClearReward() });

        this.mockQuestRepository.VerifyAll();
        this.mockDungeonService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    // Tests that when previous missions have been completed that the right amount of wyrmite is
    // given as well as their corresponding mission numbers in missions_clear_set
    [Fact]
    public async Task VariousPreviousMissionClearStatusesGiveCorrectMissions()
    {
        DungeonRecordRecordRequest request = new() { dungeon_key = dungeonKey };

        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        PlayCount = 1,
                        IsMissionClear1 = true,
                        IsMissionClear2 = false,
                        IsMissionClear3 = false
                    }
                }.AsQueryable().BuildMock());

        ActionResult<DragaliaResponse<object>> response = await this.dungeonRecordController.Record(
            request
        );

        DungeonRecordRecordData? missionTwoThreeCleared =
            response.GetData<DungeonRecordRecordData>();
        missionTwoThreeCleared.Should().NotBeNull();

        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        PlayCount = 1,
                        IsMissionClear1 = true,
                        IsMissionClear2 = true,
                        IsMissionClear3 = false
                    }
                }.AsQueryable().BuildMock());

        ActionResult<DragaliaResponse<object>> response2 =
            await this.dungeonRecordController.Record(request);

        DungeonRecordRecordData? missionThreeCleared = response2.GetData<DungeonRecordRecordData>();
        missionThreeCleared.Should().NotBeNull();

        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        PlayCount = 1,
                        IsMissionClear1 = true,
                        IsMissionClear2 = false,
                        IsMissionClear3 = true
                    }
                }.AsQueryable().BuildMock());

        ActionResult<DragaliaResponse<object>> response3 =
            await this.dungeonRecordController.Record(request);

        DungeonRecordRecordData? missionTwoCleared = response3.GetData<DungeonRecordRecordData>();
        missionTwoCleared.Should().NotBeNull();

        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        PlayCount = 1,
                        IsMissionClear1 = false,
                        IsMissionClear2 = true,
                        IsMissionClear3 = false
                    }
                }.AsQueryable().BuildMock());

        ActionResult<DragaliaResponse<object>> response4 =
            await this.dungeonRecordController.Record(request);

        DungeonRecordRecordData? missionOneThreeCleared =
            response4.GetData<DungeonRecordRecordData>();
        missionOneThreeCleared.Should().NotBeNull();

        using (new AssertionScope())
        {
            missionTwoThreeCleared!.ingame_result_data.reward_record.first_clear_set
                .Should()
                .BeEquivalentTo(new List<AtgenFirstClearSet>() { });

            missionTwoThreeCleared!.ingame_result_data.reward_record.missions_clear_set
                .Should()
                .BeEquivalentTo(
                    new List<AtgenMissionsClearSet>()
                    {
                        this.CreateMissionReward(2),
                        this.CreateMissionReward(3)
                    }
                );

            missionTwoThreeCleared!.ingame_result_data.reward_record.mission_complete
                .Should()
                .BeEquivalentTo(new List<AtgenFirstClearSet>() { this.CreateClearReward() });

            missionThreeCleared!.ingame_result_data.reward_record.first_clear_set
                .Should()
                .BeEquivalentTo(new List<AtgenFirstClearSet>() { });

            missionThreeCleared!.ingame_result_data.reward_record.missions_clear_set
                .Should()
                .BeEquivalentTo(new List<AtgenMissionsClearSet>() { this.CreateMissionReward(3) });

            missionThreeCleared!.ingame_result_data.reward_record.mission_complete
                .Should()
                .BeEquivalentTo(new List<AtgenFirstClearSet>() { this.CreateClearReward() });

            missionTwoCleared!.ingame_result_data.reward_record.first_clear_set
                .Should()
                .BeEquivalentTo(new List<AtgenFirstClearSet>() { });

            missionTwoCleared!.ingame_result_data.reward_record.missions_clear_set
                .Should()
                .BeEquivalentTo(new List<AtgenMissionsClearSet>() { this.CreateMissionReward(2) });

            missionTwoCleared!.ingame_result_data.reward_record.mission_complete
                .Should()
                .BeEquivalentTo(new List<AtgenFirstClearSet>() { this.CreateClearReward() });

            missionOneThreeCleared!.ingame_result_data.reward_record.first_clear_set
                .Should()
                .BeEquivalentTo(new List<AtgenFirstClearSet>() { });

            missionOneThreeCleared!.ingame_result_data.reward_record.missions_clear_set
                .Should()
                .BeEquivalentTo(
                    new List<AtgenMissionsClearSet>()
                    {
                        this.CreateMissionReward(1),
                        this.CreateMissionReward(3)
                    }
                );

            missionOneThreeCleared!.ingame_result_data.reward_record.mission_complete
                .Should()
                .BeEquivalentTo(new List<AtgenFirstClearSet>() { this.CreateClearReward() });
        }

        this.mockQuestRepository.VerifyAll();
        this.mockDungeonService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    // Tests that mission rewards aren't given again if missions have previously been cleared
    [Fact]
    public async Task AllMissionsPreviouslyClearedDoesntGiveRewards()
    {
        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        PlayCount = 1,
                        IsMissionClear1 = true,
                        IsMissionClear2 = true,
                        IsMissionClear3 = true
                    }
                }.AsQueryable().BuildMock());

        DungeonRecordRecordRequest request = new() { dungeon_key = dungeonKey };

        ActionResult<DragaliaResponse<object>> response = await this.dungeonRecordController.Record(
            request
        );

        DungeonRecordRecordData? data = response.GetData<DungeonRecordRecordData>();
        data.Should().NotBeNull();

        data!.ingame_result_data.reward_record.first_clear_set
            .Should()
            .BeEquivalentTo(new List<AtgenFirstClearSet>() { });

        data!.ingame_result_data.reward_record.missions_clear_set
            .Should()
            .BeEquivalentTo(new List<AtgenMissionsClearSet>() { });

        data!.ingame_result_data.reward_record.mission_complete
            .Should()
            .BeEquivalentTo(new List<AtgenFirstClearSet>() { });

        this.mockQuestRepository.VerifyAll();
        this.mockDungeonService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    private AtgenFirstClearSet CreateClearReward(
        int type = (int)EntityTypes.Wyrmite,
        int id = 0,
        int quantity = 5
    )
    {
        return new()
        {
            type = type,
            id = id,
            quantity = quantity
        };
    }

    private AtgenMissionsClearSet CreateMissionReward(
        int index,
        int type = (int)EntityTypes.Wyrmite,
        int id = 0,
        int quantity = 5
    )
    {
        return new()
        {
            type = type,
            id = id,
            quantity = quantity,
            mission_no = index
        };
    }
}
