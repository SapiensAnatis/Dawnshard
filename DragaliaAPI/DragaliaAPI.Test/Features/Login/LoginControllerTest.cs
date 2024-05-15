using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Login;
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
    private readonly Mock<ILoginBonusService> loginBonusService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly FakeTimeProvider mockDateTimeProvider;

    private readonly LoginController loginController;

    public LoginControllerTest()
    {
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockDailyResetAction = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);
        this.loginBonusService = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Strict);
        this.mockDateTimeProvider = new FakeTimeProvider();

        this.loginController = new(
            this.mockUserDataRepository.Object,
            this.mockUpdateDataService.Object,
            new List<IDailyResetAction>() { mockDailyResetAction.Object },
            this.mockLogger.Object,
            this.loginBonusService.Object,
            this.mockRewardService.Object,
            this.mockDateTimeProvider
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

        this.loginBonusService.Setup(x => x.RewardLoginBonus())
            .ReturnsAsync(Enumerable.Empty<AtgenLoginBonusList>());

        this.mockDailyResetAction.Setup(x => x.Apply()).Returns(Task.CompletedTask);

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

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync(default(CancellationToken)))
            .ReturnsAsync(new UpdateDataList());

        await this.loginController.Index(default(CancellationToken));

        this.mockRewardService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockDailyResetAction.Verify(x => x.Apply(), Times.Never);
    }
}
