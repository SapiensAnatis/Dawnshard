using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;

namespace DragaliaAPI.Test.Services;

public class StoryServiceTest
{
    private readonly Mock<IStoryRepository> mockStoryRepository;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<ILogger<StoryService>> mockLogger;
    private readonly Mock<ITutorialService> mockTutorialService;
    private readonly Mock<IFortRepository> mockFortRepository;
    private readonly Mock<IMissionProgressionService> mockMissionProgressionService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IPaymentService> mockPaymentService;

    private readonly IStoryService storyService;

    public StoryServiceTest()
    {
        this.mockStoryRepository = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockInventoryRepository = new(MockBehavior.Strict);
        this.mockLogger = new();
        this.mockTutorialService = new(MockBehavior.Strict);
        this.mockFortRepository = new(MockBehavior.Strict);
        this.mockMissionProgressionService = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Strict);
        this.mockPaymentService = new(MockBehavior.Strict);

        this.storyService = new StoryService(
            mockStoryRepository.Object,
            mockLogger.Object,
            mockUserDataRepository.Object,
            mockInventoryRepository.Object,
            mockTutorialService.Object,
            mockFortRepository.Object,
            mockMissionProgressionService.Object,
            mockRewardService.Object,
            mockPaymentService.Object
        );
    }

    [Fact]
    public async Task CheckUnitStoryEligibility_InvalidStoryId_ReturnsFalse()
    {
        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(StoryTypes.Chara, 8))
            .ReturnsAsync(new DbPlayerStoryState() { DeviceAccountId = string.Empty });

        (await this.storyService.CheckStoryEligibility(StoryTypes.Chara, 8)).Should().BeFalse();
    }

    [Fact]
    public async Task CheckUnitStoryEligibility_MissingQuestStory_ReturnsFalse()
    {
        this.mockStoryRepository
            .SetupGet(x => x.QuestStories)
            .Returns(new List<DbPlayerStoryState>().AsQueryable().BuildMock());

        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(StoryTypes.Chara, 100004101))
            .ReturnsAsync(
                new DbPlayerStoryState()
                {
                    DeviceAccountId = string.Empty,
                    State = StoryState.Unlocked
                }
            );

        (await this.storyService.CheckStoryEligibility(StoryTypes.Chara, 100004101))
            .Should()
            .BeFalse();

        this.mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task CheckUnitStoryEligibility_MissingUnitStory_ReturnsFalse()
    {
        this.mockStoryRepository
            .SetupGet(x => x.UnitStories)
            .Returns(new List<DbPlayerStoryState>().AsQueryable().BuildMock());

        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(StoryTypes.Chara, 110013012))
            .ReturnsAsync(
                new DbPlayerStoryState()
                {
                    DeviceAccountId = string.Empty,
                    State = StoryState.Unlocked
                }
            );

        (await this.storyService.CheckStoryEligibility(StoryTypes.Chara, 110013012))
            .Should()
            .BeFalse();

        this.mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task CheckUnitStoryEligibility_Eligible_ReturnsTrue()
    {
        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(StoryTypes.Chara, 110013013))
            .ReturnsAsync(
                new DbPlayerStoryState()
                {
                    DeviceAccountId = string.Empty,
                    State = StoryState.Unlocked
                }
            );

        this.mockStoryRepository
            .SetupGet(x => x.UnitStories)
            .Returns(
                new List<DbPlayerStoryState>()
                {
                    new()
                    {
                        DeviceAccountId = "whatever",
                        StoryId = 110013012,
                        State = StoryState.Read,
                        StoryType = StoryTypes.Chara
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        (await this.storyService.CheckStoryEligibility(StoryTypes.Chara, 110013013))
            .Should()
            .BeTrue();

        this.mockStoryRepository.VerifyAll();
    }

    [Theory]
    [ClassData(typeof(UnitStoryTheoryData))]
    public async Task ReadUnitStory_ReturnsExpectedReward(
        DbPlayerStoryState state,
        int expectedWyrmite
    )
    {
        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(state.StoryType, state.StoryId))
            .ReturnsAsync(state);

        this.mockUserDataRepository
            .Setup(x => x.GiveWyrmite(expectedWyrmite))
            .Returns(Task.CompletedTask);

        (await this.storyService.ReadStory(state.StoryType, state.StoryId))
            .Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new() { entity_type = EntityTypes.Wyrmite, entity_quantity = expectedWyrmite }
                }
            );

        state.State.Should().Be(StoryState.Read);

        this.mockStoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Theory]
    [InlineData(StoryTypes.Dragon, 210016011)]
    [InlineData(StoryTypes.Chara, 100003101)]
    public async Task ReadUnitStory_StoryRead_ReturnsExpectedReward(StoryTypes type, int storyId)
    {
        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(type, storyId))
            .ReturnsAsync(
                new DbPlayerStoryState() { DeviceAccountId = string.Empty, State = StoryState.Read }
            );

        (await this.storyService.ReadStory(type, storyId))
            .Should()
            .BeEquivalentTo(new List<AtgenBuildEventRewardEntityList>() { });

        this.mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task ReadQuestStory_Read_ReturnsNoRewards()
    {
        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(StoryTypes.Quest, 1))
            .ReturnsAsync(
                new DbPlayerStoryState() { DeviceAccountId = string.Empty, State = StoryState.Read }
            );

        (await this.storyService.ReadStory(StoryTypes.Quest, 1))
            .Should()
            .BeEquivalentTo(new List<AtgenQuestStoryRewardList>());

        this.mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task ReadQuestStory_DragonReward_ReceivesReward()
    {
        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(StoryTypes.Quest, 1000311))
            .ReturnsAsync(
                new DbPlayerStoryState()
                {
                    DeviceAccountId = string.Empty,
                    State = StoryState.Unlocked
                }
            );

        this.mockUserDataRepository.Setup(x => x.GiveWyrmite(25)).Returns(Task.CompletedTask);
        this.mockTutorialService
            .Setup(x => x.OnStoryQuestRead(1000311))
            .Returns(Task.CompletedTask);
        this.mockMissionProgressionService.Setup(x => x.OnQuestStoryCleared(1000311));

        this.mockRewardService
            .Setup(
                x =>
                    x.GrantReward(
                        new Entity(EntityTypes.Dragon, (int)Dragons.Brunhilda, 1, null, null, null)
                    )
            )
            .ReturnsAsync(RewardGrantResult.Added);

        (await this.storyService.ReadStory(StoryTypes.Quest, 1000311))
            .Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new() { entity_type = EntityTypes.Wyrmite, entity_quantity = 25 },
                    new()
                    {
                        entity_type = EntityTypes.Dragon,
                        entity_id = (int)Dragons.Brunhilda,
                        entity_quantity = 1,
                    }
                }
            );

        this.mockUserDataRepository.VerifyAll();
        this.mockRewardService.VerifyAll();
        this.mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task CheckCastleStoryEligibility_Read_ReturnsExpectedResult()
    {
        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(StoryTypes.Castle, 1))
            .ReturnsAsync(
                new DbPlayerStoryState() { DeviceAccountId = "", State = StoryState.Read }
            );

        (await this.storyService.CheckStoryEligibility(StoryTypes.Castle, 1)).Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CheckCastleStoryEligibility_NotRead_ReturnsExpectedResult(
        bool materialCheckResult
    )
    {
        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(StoryTypes.Castle, 1))
            .ReturnsAsync(
                new DbPlayerStoryState() { DeviceAccountId = "", State = StoryState.Unlocked }
            );

        this.mockInventoryRepository
            .Setup(x => x.CheckQuantity(Materials.LookingGlass, 1))
            .ReturnsAsync(materialCheckResult);

        (await this.storyService.CheckStoryEligibility(StoryTypes.Castle, 1))
            .Should()
            .Be(materialCheckResult);
    }

    [Fact]
    public async Task ReadCastleStory_Read_ReturnsExpectedReward()
    {
        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(StoryTypes.Castle, 2))
            .ReturnsAsync(
                new DbPlayerStoryState() { DeviceAccountId = string.Empty, State = StoryState.Read }
            );

        (await this.storyService.ReadStory(StoryTypes.Castle, 2))
            .Should()
            .BeEquivalentTo(new List<AtgenBuildEventRewardEntityList>() { });

        this.mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task ReadCastleStory_Unread_ReturnsExpectedReward()
    {
        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(StoryTypes.Castle, 2))
            .ReturnsAsync(
                new DbPlayerStoryState()
                {
                    DeviceAccountId = string.Empty,
                    State = StoryState.Unlocked
                }
            );

        this.mockInventoryRepository
            .Setup(x => x.UpdateQuantity(Materials.LookingGlass, -1))
            .Returns(Task.CompletedTask);
        this.mockUserDataRepository.Setup(x => x.GiveWyrmite(50)).Returns(Task.CompletedTask);

        (await this.storyService.ReadStory(StoryTypes.Castle, 2))
            .Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new() { entity_type = EntityTypes.Wyrmite, entity_quantity = 50 }
                }
            );

        this.mockStoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task ReadQuestStory_FortBuildReward_ReceivesReward()
    {
        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(StoryTypes.Quest, 1000607))
            .ReturnsAsync(
                new DbPlayerStoryState()
                {
                    DeviceAccountId = string.Empty,
                    State = StoryState.Unlocked
                }
            );

        this.mockMissionProgressionService.Setup(x => x.OnQuestStoryCleared(1000607));

        this.mockUserDataRepository.Setup(x => x.GiveWyrmite(25)).Returns(Task.CompletedTask);
        this.mockTutorialService
            .Setup(x => x.OnStoryQuestRead(1000607))
            .Returns(Task.CompletedTask);

        this.mockFortRepository
            .Setup(x => x.AddToStorage(FortPlants.WindDracolith, 1, true, null))
            .Returns(Task.CompletedTask);

        (await this.storyService.ReadStory(StoryTypes.Quest, 1000607))
            .Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new() { entity_type = EntityTypes.Wyrmite, entity_quantity = 25 },
                    new()
                    {
                        entity_type = EntityTypes.FortPlant,
                        entity_id = (int)FortPlants.WindDracolith,
                        entity_quantity = 1,
                    }
                }
            );

        this.mockFortRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task ReadQuestStory_EventGuestUnlocked_GrantsCharacterReward()
    {
        int storyId = 2042704; // Fractured Futures compendium -- Audric join story

        this.mockStoryRepository
            .Setup(x => x.GetOrCreateStory(StoryTypes.Quest, storyId))
            .ReturnsAsync(
                new DbPlayerStoryState()
                {
                    DeviceAccountId = string.Empty,
                    State = StoryState.Unlocked
                }
            );

        this.mockMissionProgressionService.Setup(x => x.OnQuestStoryCleared(storyId));

        this.mockUserDataRepository.Setup(x => x.GiveWyrmite(25)).Returns(Task.CompletedTask);
        this.mockTutorialService
            .Setup(x => x.OnStoryQuestRead(storyId))
            .Returns(Task.CompletedTask);

        this.mockRewardService
            .Setup(
                x =>
                    x.GrantReward(
                        It.Is<Entity>(
                            y => y.Type == EntityTypes.Chara && y.Id == (int)Charas.Audric
                        )
                    )
            )
            .ReturnsAsync(RewardGrantResult.Added);

        (await this.storyService.ReadStory(StoryTypes.Quest, storyId))
            .Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new() { entity_type = EntityTypes.Wyrmite, entity_quantity = 25 },
                    new()
                    {
                        entity_type = EntityTypes.Chara,
                        entity_id = (int)Charas.Audric,
                        entity_quantity = 1,
                    }
                }
            );

        this.mockUserDataRepository.VerifyAll();
        this.mockStoryRepository.VerifyAll();
        this.mockRewardService.VerifyAll();
    }

    private class UnitStoryTheoryData : TheoryData<DbPlayerStoryState, int>
    {
        public UnitStoryTheoryData()
        {
            this.Add(
                new()
                {
                    DeviceAccountId = string.Empty,
                    StoryId = 100004011,
                    StoryType = StoryTypes.Chara,
                    State = StoryState.Unlocked,
                },
                25
            );

            this.Add(
                new()
                {
                    DeviceAccountId = string.Empty,
                    StoryId = 100004012,
                    StoryType = StoryTypes.Chara,
                    State = StoryState.Unlocked
                },
                10
            );

            this.Add(
                new()
                {
                    DeviceAccountId = string.Empty,
                    StoryId = 210143011,
                    StoryType = StoryTypes.Dragon,
                    State = StoryState.Unlocked
                },
                25
            );

            this.Add(
                new()
                {
                    DeviceAccountId = string.Empty,
                    StoryId = 210143011,
                    StoryType = StoryTypes.Dragon,
                    State = StoryState.Unlocked
                },
                25
            );

            this.Add(
                new()
                {
                    DeviceAccountId = string.Empty,
                    StoryId = 210143012,
                    StoryType = StoryTypes.Dragon,
                    State = StoryState.Unlocked
                },
                25
            );
        }
    }
}
