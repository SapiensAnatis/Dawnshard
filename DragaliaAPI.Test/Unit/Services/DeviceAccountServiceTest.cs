using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Unit.Services;

public class DeviceAccountServiceTest
{
    private readonly Mock<ILogger<DeviceAccountService>> mockLogger;
    private readonly Mock<IDeviceAccountRepository> mockRepository;

    private readonly DeviceAccountService deviceAccountService;

    public DeviceAccountServiceTest()
    {
        mockLogger = new(MockBehavior.Loose);
        mockRepository = new(MockBehavior.Strict);

        var inMemoryConfiguration = new Dictionary<string, string> { { "HashSalt", "dragalia" }, };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfiguration)
            .Build();

        deviceAccountService = new(mockRepository.Object, configuration, mockLogger.Object);
    }

    [Fact]
    public async Task AuthenticateDeviceAccount_CorrectCredentials_ReturnsTrue()
    {
        DeviceAccount deviceAccount = new("id", "password");
        DbDeviceAccount dbDeviceAccount = new("id", "mZlZ+wpg+n3l63y9D25f93v0KLM=");
        mockRepository
            .Setup(x => x.GetDeviceAccountById(deviceAccount.id))
            .ReturnsAsync(dbDeviceAccount);

        bool result = await deviceAccountService.AuthenticateDeviceAccount(deviceAccount);

        result.Should().BeTrue();
        mockRepository.VerifyAll();
    }

    [Fact]
    public async Task AuthenticateDeviceAccount_IncorrectCredentials_ReturnsFalse()
    {
        DeviceAccount deviceAccount = new("id", "password");
        DbDeviceAccount dbDeviceAccount = new("id", "non-matching hash");
        mockRepository
            .Setup(x => x.GetDeviceAccountById(deviceAccount.id))
            .ReturnsAsync(dbDeviceAccount);

        bool result = await deviceAccountService.AuthenticateDeviceAccount(deviceAccount);

        result.Should().BeFalse();
        mockRepository.VerifyAll();
    }

    [Fact]
    public async Task AuthenticateDeviceAccount_NoPassword_ThrowsException()
    {
        DeviceAccount deviceAccount = new("id", null);

        Func<Task> act = async () =>
        {
            await deviceAccountService.AuthenticateDeviceAccount(deviceAccount);
        };

        await act.Should().ThrowAsync<ArgumentNullException>();
        mockRepository.VerifyAll();
    }

    [Fact]
    public async Task CreateDeviceAccount_CallsAddNewDeviceAccount()
    {
        mockRepository
            .Setup(x => x.AddNewDeviceAccount(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        mockRepository
            .Setup(x => x.CreateNewSavefile(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        mockRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        await deviceAccountService.RegisterDeviceAccount();

        mockRepository.VerifyAll();
    }
}
