using DragaliaAPI.Controllers.Nintendo;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Test.Controllers.Nintendo
{
    public class LoginTest
    {
        private readonly Mock<ILogger<NintendoLoginController>> mockLogger;
        private readonly Mock<ISessionService> mockSessionService;
        private readonly Mock<IDeviceAccountService> mockDeviceAccountService;
        private readonly NintendoLoginController nintendoLoginController;

        private readonly DeviceAccount deviceAccount = new("test id", "test password");

        public LoginTest()
        {
            mockLogger = new(MockBehavior.Loose);
            mockSessionService = new(MockBehavior.Strict);
            mockDeviceAccountService = new(MockBehavior.Strict);

            mockSessionService.Setup(x => x.CreateNewSession(deviceAccount, It.IsAny<string>())).Returns("session id");

            nintendoLoginController = new(
                mockLogger.Object,
                mockSessionService.Object,
                mockDeviceAccountService.Object);
        }

        [Fact]
        public async Task LoginController_NullDeviceAccount_ReturnsOKAndCreatedDeviceAccount()
        {
            mockDeviceAccountService.Setup(x => x.RegisterDeviceAccount()).ReturnsAsync(deviceAccount);
            mockDeviceAccountService.Setup(x => x.AuthenticateDeviceAccount(deviceAccount)).ReturnsAsync(true);
            LoginRequest request = new(null);

            ActionResult<LoginResponse> response = await nintendoLoginController.Post(request);

            OkObjectResult goodResponse = Assert.IsType<OkObjectResult>(response.Result);
            LoginResponse responseObject = Assert.IsType<LoginResponse>(goodResponse.Value);
            responseObject.user.deviceAccounts.Should().BeEquivalentTo(new List<DeviceAccount> { deviceAccount });
            responseObject.createdDeviceAccount.Should().BeEquivalentTo(deviceAccount);
        }

        [Fact]
        public async Task LoginController_ExistingDeviceAccount_ReturnsOKAndDeviceAccount()
        {
            mockDeviceAccountService.Setup(x => x.AuthenticateDeviceAccount(deviceAccount)).ReturnsAsync(true);
            LoginRequest request = new(deviceAccount);

            ActionResult<LoginResponse> response = await nintendoLoginController.Post(request);

            OkObjectResult goodResponse = Assert.IsType<OkObjectResult>(response.Result);
            LoginResponse responseObject = Assert.IsType<LoginResponse>(goodResponse.Value);
            responseObject.user.deviceAccounts.Should().BeEquivalentTo(new List<DeviceAccount> { deviceAccount });
            responseObject.createdDeviceAccount.Should().BeNull();
        }

        [Fact]
        public async Task LoginController_DeviceAccountUnauthorized_ReturnsUnauthorized()
        {
            mockDeviceAccountService.Setup(x => x.AuthenticateDeviceAccount(deviceAccount)).ReturnsAsync(false);
            LoginRequest request = new(deviceAccount);

            ActionResult<LoginResponse> response = await nintendoLoginController.Post(request);

            Assert.IsType<UnauthorizedResult>(response.Result);
        }
    }
}
