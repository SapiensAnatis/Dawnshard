using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Login;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Controllers;

public class LoginControllerTest
{
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IDailyResetAction> mockDailyResetAction;
    private readonly Mock<IResetHelper> mockResetHelper;
    private readonly Mock<ILogger<LoginController>> mockLogger;

    private readonly LoginController loginController;

    public LoginControllerTest()
    {
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockDailyResetAction = new(MockBehavior.Strict);
        this.mockResetHelper = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.loginController = new(
            this.mockUserDataRepository.Object,
            this.mockUpdateDataService.Object,
            new List<IDailyResetAction>() { mockDailyResetAction.Object },
            this.mockResetHelper.Object,
            this.mockLogger.Object
        );
    }

    [Fact]
    public async Task Index_LastLoginBeforeReset_CallsDailyResetAction()
    {
        this.mockUserDataRepository.SetupUserData(
            new DbPlayerUserData()
            {
                DeviceAccountId = "id",
                LastLoginTime = DateTimeOffset.UnixEpoch
            }
        );

        this.mockResetHelper.SetupGet(x => x.LastDailyReset).Returns(DateTimeOffset.UtcNow);

        this.mockDailyResetAction.Setup(x => x.Apply()).Returns(Task.CompletedTask);

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        await this.loginController.Index();

        this.mockUserDataRepository.VerifyAll();
        this.mockResetHelper.VerifyAll();
        this.mockDailyResetAction.VerifyAll();
    }

    [Fact]
    public async Task Index_LastLoginAfterReset_DoesNotCallDailyResetAction()
    {
        this.mockUserDataRepository.SetupUserData(
            new DbPlayerUserData() { DeviceAccountId = "id", LastLoginTime = DateTimeOffset.UtcNow }
        );

        this.mockResetHelper
            .SetupGet(x => x.LastDailyReset)
            .Returns(DateTimeOffset.UtcNow.AddHours(-1));

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        await this.loginController.Index();

        this.mockUserDataRepository.VerifyAll();
        this.mockResetHelper.VerifyAll();
        this.mockDailyResetAction.Verify(x => x.Apply(), Times.Never);
    }
}
