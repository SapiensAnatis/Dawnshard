using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Services;

public class TutorialServiceTest
{
    private readonly Mock<ILogger<TutorialService>> mockLogger;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IAbilityCrestRepository> mockAbilityCrestRepository;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IFortRepository> mockFortRepository;

    private readonly ITutorialService tutorialService;

    public TutorialServiceTest()
    {
        mockLogger = new(MockBehavior.Loose);
        mockInventoryRepository = new(MockBehavior.Strict);
        mockAbilityCrestRepository = new(MockBehavior.Strict);
        mockUserDataRepository = new(MockBehavior.Strict);
        mockFortRepository = new(MockBehavior.Strict);

        tutorialService = new TutorialService(
            mockLogger.Object,
            mockInventoryRepository.Object,
            mockAbilityCrestRepository.Object,
            mockUserDataRepository.Object,
            mockFortRepository.Object
        );
    }

    [Fact]
    public async Task UpdateTutorialStatus_UpdatesTutorialStatus()
    {
        this.mockUserDataRepository
            .Setup(x => x.LookupUserData())
            .ReturnsAsync(new DbPlayerUserData { DeviceAccountId = "aa", TutorialStatus = 1 });

        int currentStatus = await this.tutorialService.UpdateTutorialStatus(80000);

        currentStatus.Should().Be(80000);
    }

    [Fact]
    public async Task UpdateTutorialStatus_LowerStatus_DoesNotUpdateTutorialStatus()
    {
        this.mockUserDataRepository
            .Setup(x => x.LookupUserData())
            .ReturnsAsync(new DbPlayerUserData { DeviceAccountId = "aa", TutorialStatus = 99999 });

        int currentStatus = await this.tutorialService.UpdateTutorialStatus(80000);

        currentStatus.Should().Be(99999);
    }
}
