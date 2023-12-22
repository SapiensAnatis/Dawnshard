using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Login;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Features.Login;

public class LoginControllerTest
{
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IDailyResetAction> mockDailyResetAction;
    private readonly Mock<IResetHelper> mockResetHelper;
    private readonly Mock<ILogger<LoginController>> mockLogger;
    private readonly Mock<ILoginBonusService> loginBonusService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IDateTimeProvider> mockDateTimeProvider;

    private readonly LoginController loginController;

    public LoginControllerTest()
    {
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockDailyResetAction = new(MockBehavior.Strict);
        this.mockResetHelper = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);
        this.loginBonusService = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Strict);
        this.mockDateTimeProvider = new(MockBehavior.Strict);

        this.loginController = new(
            this.mockUserDataRepository.Object,
            this.mockUpdateDataService.Object,
            new List<IDailyResetAction>() { mockDailyResetAction.Object },
            this.mockResetHelper.Object,
            this.mockLogger.Object,
            this.loginBonusService.Object,
            this.mockRewardService.Object,
            this.mockDateTimeProvider.Object
        );

        this.mockDateTimeProvider.SetupGet(x => x.UtcNow).Returns(DateTimeOffset.UtcNow);

        this.mockRewardService.Setup(x => x.GetEntityResult()).Returns(new EntityResult());
    }

    [Fact]
    public async Task Index_LastLoginBeforeReset_CallsDailyResetAction()
    {
        this.mockUserDataRepository.SetupUserData(
            new DbPlayerUserData() { ViewerId = 1, LastLoginTime = DateTimeOffset.UnixEpoch }
        );

        this.mockResetHelper.SetupGet(x => x.LastDailyReset).Returns(DateTimeOffset.UtcNow);

        this.loginBonusService.Setup(x => x.RewardLoginBonus())
            .ReturnsAsync(Enumerable.Empty<AtgenLoginBonusList>());

        this.mockDailyResetAction.Setup(x => x.Apply()).Returns(Task.CompletedTask);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        await this.loginController.Index();

        this.mockDateTimeProvider.VerifyAll();
        this.mockRewardService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockResetHelper.VerifyAll();
        this.mockDailyResetAction.VerifyAll();
    }

    [Fact]
    public async Task Index_LastLoginAfterReset_DoesNotCallDailyResetAction()
    {
        this.mockUserDataRepository.SetupUserData(
            new DbPlayerUserData() { ViewerId = 1, LastLoginTime = DateTimeOffset.UtcNow }
        );

        this.mockResetHelper.SetupGet(x => x.LastDailyReset)
            .Returns(DateTimeOffset.UtcNow.AddHours(-1));

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        await this.loginController.Index();

        this.mockDateTimeProvider.VerifyAll();
        this.mockRewardService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockResetHelper.VerifyAll();
        this.mockDailyResetAction.Verify(x => x.Apply(), Times.Never);
    }
}
