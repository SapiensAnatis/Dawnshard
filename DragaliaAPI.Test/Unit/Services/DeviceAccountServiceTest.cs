using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Services;
using DragaliaAPI.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Unit.Services;

[Obsolete(ObsoleteReasons.BaaS)]
public class DeviceAccountServiceTest
{
    private readonly Mock<ILogger<DeviceAccountService>> mockLogger;
    private readonly Mock<IDeviceAccountRepository> mockRepository;
    private readonly Mock<ISavefileService> mockSavefileService;

    private readonly DeviceAccountService deviceAccountService;

    public DeviceAccountServiceTest()
    {
        this.mockLogger = new(MockBehavior.Loose);
        this.mockRepository = new(MockBehavior.Strict);
        this.mockSavefileService = new(MockBehavior.Strict);

        Dictionary<string, string?> inMemoryConfiguration = new() { { "HashSalt", "dragalia" }, };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfiguration)
            .Build();

        deviceAccountService = new(
            mockRepository.Object,
            mockSavefileService.Object,
            configuration,
            mockLogger.Object
        );
    }

    [Fact]
    public async Task AuthenticateDeviceAccount_CorrectCredentials_ReturnsTrue()
    {
        DeviceAccount deviceAccount = new("id", "password");
        DbDeviceAccount dbDeviceAccount = new("id", "mZlZ+wpg+n3l63y9D25f93v0KLM=");
        this.mockRepository
            .Setup(x => x.GetDeviceAccountById(deviceAccount.id))
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
        this.mockRepository
            .Setup(x => x.GetDeviceAccountById(deviceAccount.id))
            .ReturnsAsync(dbDeviceAccount);

        bool result = await deviceAccountService.AuthenticateDeviceAccount(deviceAccount);

        result.Should().BeFalse();
        this.mockRepository.VerifyAll();
    }

    [Fact]
    public async Task AuthenticateDeviceAccount_ForeignDeviceAccount_CallsAddNewDeviceAccount()
    {
        this.mockRepository
            .Setup(x => x.AddNewDeviceAccount("foreign id", It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        this.mockRepository
            .Setup(x => x.GetDeviceAccountById("foreign id"))
            .ReturnsAsync((DbDeviceAccount?)null);
        this.mockRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        this.mockSavefileService.Setup(x => x.Create("foreign id")).Returns(Task.CompletedTask);

        bool result = await deviceAccountService.AuthenticateDeviceAccount(
            new DeviceAccount("foreign id", "password")
        );

        result.Should().BeTrue();
        this.mockRepository.VerifyAll();
    }

    [Fact]
    public async Task CreateDeviceAccount_CallsAddNewDeviceAccount()
    {
        this.mockRepository
            .Setup(x => x.AddNewDeviceAccount(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        this.mockRepository
            .Setup(x => x.GetDeviceAccountById(It.IsAny<string>()))
            .ReturnsAsync((DbDeviceAccount?)null);
        this.mockRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        this.mockSavefileService
            .Setup(x => x.Create(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        await deviceAccountService.RegisterDeviceAccount();

        this.mockRepository.VerifyAll();
    }
}
