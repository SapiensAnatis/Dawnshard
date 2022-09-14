using System;
using Xunit;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Controllers.Nintendo;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models;

namespace DragaliaAPI.Test.Controllers.Nintendo
{
    public class LoginTest
    {
        private readonly Mock<ILogger<NintendoLoginController>> mockLogger = new(MockBehavior.Strict);
        private readonly Mock<ISessionService> mockSessionService = new(MockBehavior.Strict);
        private readonly Mock<DeviceAccountContext> mockDeviceAccountContext = new(MockBehavior.Strict);
        private NintendoLoginController nintendoLoginController;

        private readonly DeviceAccount deviceAccount = new("test id", "test password");

        public LoginTest()
        {
            mockSessionService.Setup(x => x.CreateNewSession(deviceAccount, It.IsAny<string>())).Returns("session id");

            nintendoLoginController = new(
                mockLogger.Object,
                mockSessionService.Object,
                mockDeviceAccountContext.Object);
        }

        [Fact]
        public async Task LoginController_NullDeviceAccount_ReturnsOKAndCreatedDeviceAccount()
        {
            mockDeviceAccountContext.Setup(x => x.RegisterDeviceAccount()).ReturnsAsync(deviceAccount);
            mockDeviceAccountContext.Setup(x => x.AuthenticateDeviceAccount(deviceAccount)).ReturnsAsync(true);
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
            mockDeviceAccountContext.Setup(x => x.AuthenticateDeviceAccount(deviceAccount)).ReturnsAsync(true);
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
            mockDeviceAccountContext.Setup(x => x.AuthenticateDeviceAccount(deviceAccount)).ReturnsAsync(false);
            LoginRequest request = new(deviceAccount);

            ActionResult<LoginResponse> response = await nintendoLoginController.Post(request);

            Assert.IsType<UnauthorizedResult>(response.Result);
        }
    }
}
