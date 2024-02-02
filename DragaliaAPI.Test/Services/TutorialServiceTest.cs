using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;

namespace DragaliaAPI.Test.Services;

public class TutorialServiceTest
{
    private readonly Mock<ILogger<TutorialService>> mockLogger;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IAbilityCrestRepository> mockAbilityCrestRepository;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IFortRepository> mockFortRepository;
    private readonly Mock<IWallService> mockWallService;

    private readonly ITutorialService tutorialService;

    public TutorialServiceTest()
    {
        mockLogger = new(MockBehavior.Loose);
        mockInventoryRepository = new(MockBehavior.Strict);
        mockAbilityCrestRepository = new(MockBehavior.Strict);
        mockUserDataRepository = new(MockBehavior.Strict);
        mockFortRepository = new(MockBehavior.Strict);
        this.mockWallService = new(MockBehavior.Strict);

        tutorialService = new TutorialService(
            mockLogger.Object,
            mockInventoryRepository.Object,
            mockAbilityCrestRepository.Object,
            mockUserDataRepository.Object,
            mockFortRepository.Object,
            this.mockWallService.Object
        );
    }

    [Fact]
    public async Task UpdateTutorialStatus_UpdatesTutorialStatus()
    {
        this.mockUserDataRepository.SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>
                {
                    new() { ViewerId = 1, TutorialStatus = 1 }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        int currentStatus = await this.tutorialService.UpdateTutorialStatus(80000);

        currentStatus.Should().Be(80000);
    }

    [Fact]
    public async Task UpdateTutorialStatus_LowerStatus_DoesNotUpdateTutorialStatus()
    {
        this.mockUserDataRepository.SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>
                {
                    new() { ViewerId = 1, TutorialStatus = 99999 }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        int currentStatus = await this.tutorialService.UpdateTutorialStatus(80000);

        currentStatus.Should().Be(99999);
    }

    [Fact]
    public async Task UpdateTutorialStatus_DojoStatus_AddsDojos()
    {
        this.mockUserDataRepository.SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>
                {
                    new() { ViewerId = 1, TutorialStatus = 1 }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockFortRepository.Setup(x => x.AddDojos()).Returns(Task.CompletedTask);

        int currentStatus = await this.tutorialService.UpdateTutorialStatus(60999);

        currentStatus.Should().Be(60999);
        this.mockFortRepository.VerifyAll();
    }
}
