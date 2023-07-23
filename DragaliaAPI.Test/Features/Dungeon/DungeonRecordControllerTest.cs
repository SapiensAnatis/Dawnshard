using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using static DragaliaAPI.Test.UnitTestUtils;

namespace DragaliaAPI.Test.Controllers;

public class DungeonRecordControllerTest
{
    private readonly DungeonRecordController dungeonRecordController;
    private readonly Mock<IQuestRepository> mockQuestRepository;
    private readonly Mock<IDungeonService> mockDungeonService;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<ITutorialService> mockTutorialService;
    private readonly Mock<IMissionProgressionService> mockMissionProgressionService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<ILogger<DungeonRecordController>> mockLogger;
    private readonly Mock<IQuestCompletionService> mockQuestCompletionService;
    private readonly Mock<IEventDropService> mockEventDropService;
    private readonly Mock<IAbilityCrestMultiplierService> mockCrestMultiplierService;
    private readonly Mock<IUserService> mockUserService;

    private const string dungeonKey = "key";
    private const int questId = 100010101;
    private const int clearTime = 60;

    private readonly List<PartySettingList> party =
        new()
        {
            new() { unit_no = 1, chara_id = Charas.ThePrince }
        };
    private readonly QuestData questData = MasterAsset.QuestData.Get(questId);

    public DungeonRecordControllerTest()
    {
        this.mockQuestRepository = new(MockBehavior.Strict);
        this.mockDungeonService = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockInventoryRepository = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockTutorialService = new(MockBehavior.Strict);
        this.mockMissionProgressionService = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Loose); // This file is about to be overhauled anyway
        this.mockLogger = new(MockBehavior.Loose);
        this.mockQuestCompletionService = new(MockBehavior.Strict);
        this.mockEventDropService = new(MockBehavior.Strict);
        this.mockCrestMultiplierService = new(MockBehavior.Strict);
        this.mockUserService = new(MockBehavior.Loose); // yes loose

        this.dungeonRecordController = new(
            this.mockQuestRepository.Object,
            this.mockDungeonService.Object,
            this.mockUserDataRepository.Object,
            this.mockInventoryRepository.Object,
            this.mockUpdateDataService.Object,
            this.mockTutorialService.Object,
            this.mockMissionProgressionService.Object,
            this.mockLogger.Object,
            this.mockQuestCompletionService.Object,
            this.mockEventDropService.Object,
            this.mockRewardService.Object,
            this.mockCrestMultiplierService.Object,
            this.mockUserService.Object
        );

        this.dungeonRecordController.SetupMockContext();

        this.mockDungeonService
            .Setup(x => x.FinishDungeon(dungeonKey))
            .ReturnsAsync(new DungeonSession() { Party = party, QuestData = questData });

        this.mockTutorialService
            .Setup(x => x.AddTutorialFlag(1022))
            .ReturnsAsync(new List<int> { 1022 });

        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        Name = "Euden",
                        ViewerId = 1
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockInventoryRepository
            .Setup(x => x.UpdateQuantity(It.IsAny<IEnumerable<KeyValuePair<Materials, int>>>()))
            .Returns(Task.CompletedTask);

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        this.mockCrestMultiplierService
            .Setup(x => x.GetFacilityEventMultiplier(party, questData.Gid))
            .ReturnsAsync(1);
    }

    // Tests that QuestId and party data show up in response
    [Fact]
    public async Task QuestIdAndPartyDataAppearInResponse()
    {
        this.mockMissionProgressionService.Setup(x => x.OnQuestCleared(100010101));

        this.mockQuestRepository
            .SetupGet(x => x.Quests)
            .Returns(
                new List<DbQuest>()
                {
                    new() { DeviceAccountId = DeviceAccountId, QuestId = questId }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockQuestRepository
            .Setup(x => x.CompleteQuest(questId, It.IsAny<float>()))
            .ReturnsAsync(new DbQuest() { DeviceAccountId = DeviceAccountId, QuestId = questId });

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestMissions(
                        It.IsAny<DungeonSession>(),
                        new[] { false, false, false },
                        It.IsAny<PlayRecord>()
                    )
            )
            .ReturnsAsync(
                new QuestMissionStatus(
                    new[] { true, true, true },
                    new List<AtgenMissionsClearSet>(),
                    new List<AtgenFirstClearSet>()
                )
            );

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestScoreMissions(It.IsAny<DungeonSession>(), It.IsAny<PlayRecord>())
            )
            .ReturnsAsync((new List<AtgenScoreMissionSuccessList>(), 0));

        this.mockQuestCompletionService
            .Setup(x => x.GrantFirstClearRewards(questId))
            .ReturnsAsync(new List<AtgenFirstClearSet>());

        this.mockEventDropService
            .Setup(x => x.ProcessEventPassiveDrops(It.IsAny<QuestData>()))
            .ReturnsAsync(new List<AtgenEventPassiveUpList>());

        this.mockCrestMultiplierService
            .Setup(x => x.GetFacilityEventMultiplier(party, questData.Gid))
            .ReturnsAsync(4);

        this.mockEventDropService
            .Setup(
                x => x.ProcessEventMaterialDrops(It.IsAny<QuestData>(), It.IsAny<PlayRecord>(), 4)
            )
            .ReturnsAsync(new List<AtgenDropAll>());

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

        this.mockQuestRepository.VerifyAll();
        this.mockDungeonService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    // Tests that CompleteQuest returning the same clear time as GetQuests marks time as best clear time
    [Fact]
    public async Task BestClearDisplaysTimeAsBestTime()
    {
        this.mockMissionProgressionService.Setup(x => x.OnQuestCleared(100010101));

        this.mockQuestRepository
            .SetupGet(x => x.Quests)
            .Returns(
                new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        BestClearTime = clearTime
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockQuestRepository
            .Setup(x => x.CompleteQuest(questId, clearTime))
            .ReturnsAsync(
                new DbQuest()
                {
                    DeviceAccountId = DeviceAccountId,
                    QuestId = questId,
                    BestClearTime = clearTime
                }
            );

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestMissions(
                        It.IsAny<DungeonSession>(),
                        new[] { false, false, false },
                        It.IsAny<PlayRecord>()
                    )
            )
            .ReturnsAsync(
                new QuestMissionStatus(
                    new[] { true, true, true },
                    new List<AtgenMissionsClearSet>(),
                    new List<AtgenFirstClearSet>()
                )
            );

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestScoreMissions(It.IsAny<DungeonSession>(), It.IsAny<PlayRecord>())
            )
            .ReturnsAsync((new List<AtgenScoreMissionSuccessList>(), 0));

        this.mockQuestCompletionService
            .Setup(x => x.GrantFirstClearRewards(questId))
            .ReturnsAsync(new List<AtgenFirstClearSet>());

        this.mockEventDropService
            .Setup(x => x.ProcessEventPassiveDrops(It.IsAny<QuestData>()))
            .ReturnsAsync(new List<AtgenEventPassiveUpList>());

        this.mockEventDropService
            .Setup(
                x => x.ProcessEventMaterialDrops(It.IsAny<QuestData>(), It.IsAny<PlayRecord>(), 1)
            )
            .ReturnsAsync(new List<AtgenDropAll>());

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
        this.mockMissionProgressionService.Setup(x => x.OnQuestCleared(100010101));

        this.mockQuestRepository
            .SetupGet(x => x.Quests)
            .Returns(
                new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        BestClearTime = clearTime
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockQuestRepository
            .Setup(x => x.CompleteQuest(questId, clearTime))
            .ReturnsAsync(
                new DbQuest()
                {
                    DeviceAccountId = DeviceAccountId,
                    QuestId = questId,
                    BestClearTime = clearTime - 1
                }
            );

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestMissions(
                        It.IsAny<DungeonSession>(),
                        new[] { false, false, false },
                        It.IsAny<PlayRecord>()
                    )
            )
            .ReturnsAsync(
                new QuestMissionStatus(
                    new[] { true, true, true },
                    new List<AtgenMissionsClearSet>(),
                    new List<AtgenFirstClearSet>()
                )
            );

        this.mockQuestCompletionService
            .Setup(x => x.GrantFirstClearRewards(questId))
            .ReturnsAsync(new List<AtgenFirstClearSet>());

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestScoreMissions(It.IsAny<DungeonSession>(), It.IsAny<PlayRecord>())
            )
            .ReturnsAsync((new List<AtgenScoreMissionSuccessList>(), 0));

        this.mockEventDropService
            .Setup(x => x.ProcessEventPassiveDrops(It.IsAny<QuestData>()))
            .ReturnsAsync(new List<AtgenEventPassiveUpList>());

        this.mockEventDropService
            .Setup(
                x => x.ProcessEventMaterialDrops(It.IsAny<QuestData>(), It.IsAny<PlayRecord>(), 1)
            )
            .ReturnsAsync(new List<AtgenDropAll>());

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
        this.mockMissionProgressionService.Setup(x => x.OnQuestCleared(100010101));

        this.mockQuestRepository
            .SetupGet(x => x.Quests)
            .Returns(
                new List<DbQuest>()
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
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockQuestRepository
            .Setup(x => x.CompleteQuest(questId, It.IsAny<float>()))
            .ReturnsAsync(
                new DbQuest()
                {
                    DeviceAccountId = DeviceAccountId,
                    QuestId = questId,
                    IsMissionClear1 = false,
                    IsMissionClear2 = false,
                    IsMissionClear3 = false
                }
            );

        List<AtgenMissionsClearSet> clearRewards =
            new()
            {
                this.CreateMissionReward(1),
                this.CreateMissionReward(2),
                this.CreateMissionReward(3)
            };

        List<AtgenFirstClearSet> firstClearReward = new() { this.CreateClearReward() };

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestMissions(
                        It.IsAny<DungeonSession>(),
                        new[] { false, false, false },
                        It.IsAny<PlayRecord>()
                    )
            )
            .ReturnsAsync(
                new QuestMissionStatus(new[] { true, true, true }, clearRewards, firstClearReward)
            );

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestScoreMissions(It.IsAny<DungeonSession>(), It.IsAny<PlayRecord>())
            )
            .ReturnsAsync((new List<AtgenScoreMissionSuccessList>(), 0));

        this.mockQuestCompletionService
            .Setup(x => x.GrantFirstClearRewards(questId))
            .ReturnsAsync(firstClearReward);

        this.mockEventDropService
            .Setup(x => x.ProcessEventPassiveDrops(It.IsAny<QuestData>()))
            .ReturnsAsync(new List<AtgenEventPassiveUpList>());

        this.mockEventDropService
            .Setup(
                x => x.ProcessEventMaterialDrops(It.IsAny<QuestData>(), It.IsAny<PlayRecord>(), 1)
            )
            .ReturnsAsync(new List<AtgenDropAll>());

        DungeonRecordRecordRequest request = new() { dungeon_key = dungeonKey };

        ActionResult<DragaliaResponse<object>> response = await this.dungeonRecordController.Record(
            request
        );

        DungeonRecordRecordData? data = response.GetData<DungeonRecordRecordData>();
        data.Should().NotBeNull();

        data!.ingame_result_data.reward_record.first_clear_set
            .Should()
            .BeEquivalentTo(firstClearReward);

        data!.ingame_result_data.reward_record.missions_clear_set
            .Should()
            .BeEquivalentTo(clearRewards);

        data!.ingame_result_data.reward_record.mission_complete
            .Should()
            .BeEquivalentTo(firstClearReward);

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
        this.mockMissionProgressionService.Setup(x => x.OnQuestCleared(100010101));

        this.mockQuestRepository
            .SetupGet(x => x.Quests)
            .Returns(
                new List<DbQuest>()
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
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockQuestRepository
            .Setup(x => x.CompleteQuest(questId, It.IsAny<float>()))
            .ReturnsAsync(
                new DbQuest()
                {
                    DeviceAccountId = DeviceAccountId,
                    QuestId = questId,
                    IsMissionClear1 = false,
                    IsMissionClear2 = false,
                    IsMissionClear3 = false
                }
            );

        List<AtgenMissionsClearSet> clearRewards =
            new()
            {
                this.CreateMissionReward(1),
                this.CreateMissionReward(2),
                this.CreateMissionReward(3)
            };

        List<AtgenFirstClearSet> missionCompleteReward = new() { this.CreateClearReward() };

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestMissions(
                        It.IsAny<DungeonSession>(),
                        new[] { false, false, false },
                        It.IsAny<PlayRecord>()
                    )
            )
            .ReturnsAsync(
                new QuestMissionStatus(
                    new[] { true, true, true },
                    clearRewards,
                    missionCompleteReward
                )
            );

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestScoreMissions(It.IsAny<DungeonSession>(), It.IsAny<PlayRecord>())
            )
            .ReturnsAsync((new List<AtgenScoreMissionSuccessList>(), 0));

        this.mockEventDropService
            .Setup(x => x.ProcessEventPassiveDrops(It.IsAny<QuestData>()))
            .ReturnsAsync(new List<AtgenEventPassiveUpList>());

        this.mockEventDropService
            .Setup(
                x => x.ProcessEventMaterialDrops(It.IsAny<QuestData>(), It.IsAny<PlayRecord>(), 1)
            )
            .ReturnsAsync(new List<AtgenDropAll>());

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
            .BeEquivalentTo(clearRewards);

        data!.ingame_result_data.reward_record.mission_complete
            .Should()
            .BeEquivalentTo(missionCompleteReward);

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
        this.mockMissionProgressionService.Setup(x => x.OnQuestCleared(100010101));

        this.mockQuestRepository
            .SetupGet(x => x.Quests)
            .Returns(
                new List<DbQuest>()
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
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockQuestRepository
            .Setup(x => x.CompleteQuest(questId, It.IsAny<float>()))
            .ReturnsAsync(
                new DbQuest()
                {
                    DeviceAccountId = DeviceAccountId,
                    QuestId = questId,
                    IsMissionClear1 = missions[0],
                    IsMissionClear2 = missions[1],
                    IsMissionClear3 = missions[2]
                }
            );

        List<AtgenMissionsClearSet> missionsCleared = new();

        for (int i = 0; i < 3; i++)
        {
            if (!missions[i])
            {
                missionsCleared.Add(this.CreateMissionReward(i + 1));
            }
        }

        List<AtgenFirstClearSet> missionCompleteReward = new() { this.CreateClearReward() };

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestMissions(
                        It.IsAny<DungeonSession>(),
                        missions,
                        It.IsAny<PlayRecord>()
                    )
            )
            .ReturnsAsync(
                new QuestMissionStatus(
                    new[] { true, true, true },
                    missionsCleared,
                    missionCompleteReward
                )
            );

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestScoreMissions(It.IsAny<DungeonSession>(), It.IsAny<PlayRecord>())
            )
            .ReturnsAsync((new List<AtgenScoreMissionSuccessList>(), 0));

        this.mockEventDropService
            .Setup(x => x.ProcessEventPassiveDrops(It.IsAny<QuestData>()))
            .ReturnsAsync(new List<AtgenEventPassiveUpList>());

        this.mockEventDropService
            .Setup(
                x => x.ProcessEventMaterialDrops(It.IsAny<QuestData>(), It.IsAny<PlayRecord>(), 1)
            )
            .ReturnsAsync(new List<AtgenDropAll>());

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
            .BeEquivalentTo(missionsCleared);

        data!.ingame_result_data.reward_record.mission_complete
            .Should()
            .BeEquivalentTo(missionCompleteReward);

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
        this.mockMissionProgressionService.Setup(x => x.OnQuestCleared(100010101));

        this.mockQuestRepository
            .SetupGet(x => x.Quests)
            .Returns(
                new List<DbQuest>()
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
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockQuestRepository
            .Setup(x => x.CompleteQuest(questId, It.IsAny<float>()))
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

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestMissions(
                        It.IsAny<DungeonSession>(),
                        new[] { true, true, true },
                        It.IsAny<PlayRecord>()
                    )
            )
            .ReturnsAsync(
                new QuestMissionStatus(
                    new[] { true, true, true },
                    new List<AtgenMissionsClearSet>(),
                    new List<AtgenFirstClearSet>()
                )
            );

        this.mockQuestCompletionService
            .Setup(
                x =>
                    x.CompleteQuestScoreMissions(It.IsAny<DungeonSession>(), It.IsAny<PlayRecord>())
            )
            .ReturnsAsync((new List<AtgenScoreMissionSuccessList>(), 0));

        this.mockEventDropService
            .Setup(x => x.ProcessEventPassiveDrops(It.IsAny<QuestData>()))
            .ReturnsAsync(new List<AtgenEventPassiveUpList>());

        this.mockEventDropService
            .Setup(
                x => x.ProcessEventMaterialDrops(It.IsAny<QuestData>(), It.IsAny<PlayRecord>(), 1)
            )
            .ReturnsAsync(new List<AtgenDropAll>());

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
        EntityTypes type = EntityTypes.Wyrmite,
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
        EntityTypes type = EntityTypes.Wyrmite,
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
