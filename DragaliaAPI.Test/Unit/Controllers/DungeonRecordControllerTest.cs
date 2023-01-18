using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
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
    private const int questId = 100010101;
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
                    Party = new List<PartySettingList>()
                    {
                        new() { unit_no = 1, chara_id = Charas.ThePrince }
                    },
                    QuestData = MasterAsset.QuestData.Get(questId)
                }
            );

        this.mockUserDataRepository
            .Setup(x => x.AddTutorialFlag(DeviceAccountId, 1022))
            .ReturnsAsync(new DbPlayerUserData { DeviceAccountId = DeviceAccountId });

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
                x =>
                    x.AddMaterialQuantity(
                        DeviceAccountId,
                        It.IsAny<List<Materials>>(),
                        It.IsAny<int>()
                    )
            )
            .Returns(Task.CompletedTask);

        this.mockUpdateDataService
            .Setup(x => x.GetUpdateDataList(DeviceAccountId))
            .Returns(new UpdateDataList() { });

        this.mockQuestRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);
    }

    // Tests that QuestId and party data show up in response
    [Fact]
    public async Task QuestIdAndPartyDataAppearInResponse()
    {
        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new() { DeviceAccountId = DeviceAccountId, QuestId = questId }
                }.AsQueryable().BuildMock());

        this.mockQuestRepository
            .Setup(x => x.CompleteQuest(DeviceAccountId, questId, It.IsAny<float>()))
            .ReturnsAsync(new DbQuest() { DeviceAccountId = DeviceAccountId, QuestId = questId });

        DungeonRecordRecordRequest request = new() { dungeon_key = dungeonKey };

        ActionResult<DragaliaResponse<object>> response = await this.dungeonRecordController.Record(
            request
        );

        DungeonRecordRecordData? data = response.GetData<DungeonRecordRecordData>();
        data.Should().NotBeNull();

        data!.ingame_result_data.quest_id.Should().Be(questId);
        data!.ingame_result_data.quest_party_setting_list
            .Should()
            .BeEquivalentTo(
                new List<PartySettingList>()
                {
                    new() { unit_no = 1, chara_id = Charas.ThePrince }
                }
            );
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
            .Setup(x => x.CompleteQuest(DeviceAccountId, questId, clearTime))
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
            .Setup(x => x.CompleteQuest(DeviceAccountId, questId, clearTime))
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
    [Theory]
    [ClassData(typeof(MissionCompletionGenerator))]
    public async Task VariousPreviousMissionClearStatusesGiveCorrectMissions(bool[] missions)
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
                        IsMissionClear1 = missions[0],
                        IsMissionClear2 = missions[1],
                        IsMissionClear3 = missions[2]
                    }
                }.AsQueryable().BuildMock());

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

        DungeonRecordRecordRequest request = new() { dungeon_key = dungeonKey };

        ActionResult<DragaliaResponse<object>> response = await this.dungeonRecordController.Record(
            request
        );

        DungeonRecordRecordData? data = response.GetData<DungeonRecordRecordData>();
        data.Should().NotBeNull();

        List<AtgenMissionsClearSet> missionsCleared = new();

        for (int i = 0; i < 3; i++)
        {
            if (!missions[i])
            {
                missionsCleared.Add(this.CreateMissionReward(i + 1));
            }
        }

        data!.ingame_result_data.reward_record.first_clear_set
            .Should()
            .BeEquivalentTo(new List<AtgenFirstClearSet>() { });

        data!.ingame_result_data.reward_record.missions_clear_set
            .Should()
            .BeEquivalentTo(missionsCleared);

        data!.ingame_result_data.reward_record.mission_complete
            .Should()
            .BeEquivalentTo(new List<AtgenFirstClearSet>() { this.CreateClearReward() });

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

    public class MissionCompletionGenerator : TheoryData<bool[]>
    {
        public MissionCompletionGenerator()
        {
            Add(new bool[] { true, false, false });
            Add(new bool[] { false, true, false });
            Add(new bool[] { false, false, true });
            Add(new bool[] { true, true, false });
            Add(new bool[] { true, false, true });
            Add(new bool[] { false, true, true });
        }
    }
}
