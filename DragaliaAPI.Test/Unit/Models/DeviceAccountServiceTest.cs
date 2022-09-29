using DragaliaAPI.Models.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Unit.Models
{
    public class DeviceAccountServiceTest
    {
        private readonly Mock<ILogger<DeviceAccountService>> mockLogger;
        private readonly Mock<IApiRepository> mockRepository;

        private readonly DeviceAccountService deviceAccountService;

        public DeviceAccountServiceTest()
        {
            mockLogger = new(MockBehavior.Loose);
            mockRepository = new(MockBehavior.Strict);

            var inMemoryConfiguration = new Dictionary<string, string> {
                {"HashSalt", "dragalia"},
            };

            mockRepository = new(MockBehavior.Strict);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfiguration)
                .Build();

            deviceAccountService = new(mockRepository.Object, configuration, mockLogger.Object);
        }

        [Fact]
        public async Task AuthenticateDeviceAccount_CorrectCredentials_ReturnsTrue()
        {
            DeviceAccount deviceAccount = new("id", "password");
            DbDeviceAccount dbDeviceAccount = new("id", "NMvdakTznEF6khwWcz17i6GTnDA=");
            mockRepository.Setup(x => x.GetDeviceAccountById(deviceAccount.id)).ReturnsAsync(dbDeviceAccount);

            bool result = await deviceAccountService.AuthenticateDeviceAccount(deviceAccount);

            result.Should().BeTrue();
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task AuthenticateDeviceAccount_InCorrectCredentials_ReturnsFalse()
        {
            DeviceAccount deviceAccount = new("id", "password");
            DbDeviceAccount dbDeviceAccount = new("id", "non-matching hash");
            mockRepository.Setup(x => x.GetDeviceAccountById(deviceAccount.id)).ReturnsAsync(dbDeviceAccount);

            bool result = await deviceAccountService.AuthenticateDeviceAccount(deviceAccount);

            result.Should().BeFalse();
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task AuthenticateDeviceAccount_NoPassword_ThrowsException()
        {
            DeviceAccount deviceAccount = new("id", null);

            Func<Task> act = async () => { await deviceAccountService.AuthenticateDeviceAccount(deviceAccount); };

            await act.Should().ThrowAsync<ArgumentNullException>();
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateDeviceAccount_CallsAddNewDeviceAccount()
        {
            mockRepository.Setup(x => x.AddNewDeviceAccount(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            mockRepository.Setup(x => x.AddNewPlayerSavefile(It.IsAny<string>())).Returns(Task.CompletedTask);

            await deviceAccountService.RegisterDeviceAccount();

            mockRepository.VerifyAll();
        }
    }
}
