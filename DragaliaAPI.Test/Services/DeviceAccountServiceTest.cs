using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Services;

[Obsolete(ObsoleteReasons.BaaS)]
public class DeviceAccountServiceTest
{
    private readonly Mock<ILogger<DeviceAccountService>> mockLogger;
    private readonly Mock<IDeviceAccountRepository> mockRepository;
    private readonly Mock<ISavefileService> mockSavefileService;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;

    private readonly DeviceAccountService deviceAccountService;

    public DeviceAccountServiceTest()
    {
        this.mockLogger = new(MockBehavior.Loose);
        this.mockRepository = new(MockBehavior.Strict);
        this.mockSavefileService = new(MockBehavior.Strict);
        this.mockPlayerIdentityService = new(MockBehavior.Strict);

        Dictionary<string, string?> inMemoryConfiguration = new() { { "HashSalt", "dragalia" }, };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfiguration)
            .Build();

        deviceAccountService = new(
            mockRepository.Object,
            mockSavefileService.Object,
            configuration,
            mockPlayerIdentityService.Object,
            mockLogger.Object
        );
    }

    [Fact]
    public async Task AuthenticateDeviceAccount_CorrectCredentials_ReturnsTrue()
    {
        DeviceAccount deviceAccount = new("id", "password");
        DbDeviceAccount dbDeviceAccount = new("id", "mZlZ+wpg+n3l63y9D25f93v0KLM=");
        this.mockRepository.Setup(x => x.GetDeviceAccountById(deviceAccount.id))
            .ReturnsAsync(dbDeviceAccount);

        bool result = await deviceAccountService.AuthenticateDeviceAccount(deviceAccount);

        result.Should().BeTrue();
        this.mockRepository.VerifyAll();
    }

    [Fact]
    public async Task AuthenticateDeviceAccount_IncorrectCredentials_ReturnsFalse()
    {
        DeviceAccount deviceAccount = new("id", "password");
        DbDeviceAccount dbDeviceAccount = new("id", "non-matching hash");
        this.mockRepository.Setup(x => x.GetDeviceAccountById(deviceAccount.id))
            .ReturnsAsync(dbDeviceAccount);

        bool result = await deviceAccountService.AuthenticateDeviceAccount(deviceAccount);

        result.Should().BeFalse();
        this.mockRepository.VerifyAll();
    }

    [Fact]
    public async Task AuthenticateDeviceAccount_ForeignDeviceAccount_CallsAddNewDeviceAccount()
    {
        this.mockRepository.Setup(x => x.AddNewDeviceAccount("foreign id", It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        this.mockRepository.Setup(x => x.GetDeviceAccountById("foreign id"))
            .ReturnsAsync((DbDeviceAccount?)null);
        this.mockRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        this.mockSavefileService.Setup(x => x.Create())
            .ReturnsAsync(new DbPlayer() { AccountId = "foreign id", ViewerId = 1 });

        this.mockPlayerIdentityService.Setup(x => x.StartUserImpersonation(null, "foreign id"))
            .Returns(new Mock<IDisposable>(MockBehavior.Loose).Object);

        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("foreign id");

        bool result = await deviceAccountService.AuthenticateDeviceAccount(
            new DeviceAccount("foreign id", "password")
        );

        result.Should().BeTrue();
        this.mockRepository.VerifyAll();
    }

    [Fact]
    public async Task CreateDeviceAccount_CallsAddNewDeviceAccount()
    {
        this.mockRepository.Setup(
            x => x.AddNewDeviceAccount(It.IsAny<string>(), It.IsAny<string>())
        )
            .Returns(Task.CompletedTask);
        this.mockRepository.Setup(x => x.GetDeviceAccountById(It.IsAny<string>()))
            .ReturnsAsync((DbDeviceAccount?)null);
        this.mockRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        this.mockPlayerIdentityService.Setup(
            x => x.StartUserImpersonation(null, It.IsAny<string>())
        )
            .Returns(new Mock<IDisposable>(MockBehavior.Loose).Object);
        this.mockSavefileService.Setup(x => x.Create())
            .ReturnsAsync(new DbPlayer() { AccountId = "foreign id", ViewerId = 1 });

        await deviceAccountService.RegisterDeviceAccount();

        this.mockRepository.VerifyAll();
    }
}
