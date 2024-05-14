using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Login;
using DragaliaAPI.Features.Login.Actions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;

namespace DragaliaAPI.Test.Features.Login;

public class LoginControllerTest
{
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IDailyResetAction> mockDailyResetAction;
    private readonly Mock<ILogger<LoginController>> mockLogger;
    private readonly Mock<ILoginService> mockLoginService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IDragonService> mockDragonService;
    private readonly FakeTimeProvider mockDateTimeProvider;

    private readonly LoginController loginController;

    public LoginControllerTest()
    {
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockDailyResetAction = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);
        this.mockLoginService = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Strict);
        this.mockDragonService = new(MockBehavior.Loose);
        this.mockDateTimeProvider = new FakeTimeProvider();

        this.loginController = new(
            this.mockUserDataRepository.Object,
            this.mockUpdateDataService.Object,
            new List<IDailyResetAction>() { mockDailyResetAction.Object },
            this.mockLogger.Object,
            this.mockLoginService.Object,
            this.mockRewardService.Object,
            this.mockDateTimeProvider,
            this.mockDragonService.Object
        );

        this.mockDateTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);

        this.mockRewardService.Setup(x => x.GetEntityResult()).Returns(new EntityResult());
    }

    [Fact]
    public async Task Index_LastLoginBeforeReset_CallsDailyResetAction()
    {
        this.mockUserDataRepository.SetupUserData(
            new DbPlayerUserData() { ViewerId = 1, LastLoginTime = DateTimeOffset.UnixEpoch }
        );

        this.mockDateTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);

        this.mockLoginService.Setup(x => x.RewardLoginBonus())
            .ReturnsAsync(Enumerable.Empty<AtgenLoginBonusList>());

        this.mockDailyResetAction.Setup(x => x.Apply()).Returns(Task.CompletedTask);

        this.mockLoginService.Setup(x => x.GetWallMonthlyReceiveList()).ReturnsAsync([]);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync(default(CancellationToken)))
            .ReturnsAsync(new UpdateDataList());

        await this.loginController.Index(default(CancellationToken));

        this.mockRewardService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockDailyResetAction.VerifyAll();
    }

    [Fact]
    public async Task Index_LastLoginAfterReset_DoesNotCallDailyResetAction()
    {
        DateTimeOffset timeAfterReset = DateTimeOffset.Parse("2049-05-13T17:55:52Z");

        this.mockUserDataRepository.SetupUserData(
            new DbPlayerUserData() { ViewerId = 1, LastLoginTime = timeAfterReset }
        );

        this.mockDateTimeProvider.SetUtcNow(timeAfterReset);

        this.mockLoginService.Setup(x => x.GetWallMonthlyReceiveList()).ReturnsAsync([]);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync(default(CancellationToken)))
            .ReturnsAsync(new UpdateDataList());

        await this.loginController.Index(default(CancellationToken));

        this.mockRewardService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockDailyResetAction.Verify(x => x.Apply(), Times.Never);
    }
}
