using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;

namespace DragaliaAPI.Test.Controllers;

public class LoginControllerTest
{
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<ILogger<LoginController>> mockLogger;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;

    private readonly LoginController loginController;

    public LoginControllerTest()
    {
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockInventoryRepository = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.loginController = new(
            this.mockUserDataRepository.Object,
            this.mockInventoryRepository.Object,
            this.mockUpdateDataService.Object,
            new Mock<IPresentService>().Object,
            this.mockLogger.Object
        );
    }

    [Fact]
    public async Task Index_RefreshesDragonGifts()
    {
        this.mockUserDataRepository.SetupUserData(
            new DbPlayerUserData()
            {
                DeviceAccountId = "id",
                LastLoginTime = DateTimeOffset.UnixEpoch
            }
        );

        this.mockInventoryRepository
            .Setup(x => x.RefreshPurchasableDragonGiftCounts())
            .Returns(Task.CompletedTask);

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        await this.loginController.Index();

        this.mockUserDataRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
    }
}
