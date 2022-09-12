using System;
using Xunit;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Controllers.Nintendo;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Test.Controllers.Nintendo
{
    public class LoginTest
    {
        private readonly Mock<ILogger<NintendoLoginController>> _mockLogger = new(MockBehavior.Strict);
        private readonly Mock<ILoginService> _mockLoginService = new(MockBehavior.Strict);

        public LoginTest()
        {
            
        }

        [Fact]
        public async Task LoginController_NullDeviceAccount_ReturnsCreatedDeviceAccount()
        {
            DeviceAccount deviceAccount = new("test id", "test password");

            LoginResponse createDeviceAccountResponse = new("idToken", deviceAccount)
            {
                createdDeviceAccount = deviceAccount
            };

            _mockLoginService.Setup(x => x.DeviceAccountFactory()).ReturnsAsync(deviceAccount);
            _mockLoginService.Setup(x => x.Login(deviceAccount)).ReturnsAsync(createDeviceAccountResponse);

            LoginRequest request = new(null);
            NintendoLoginController nintendoLoginController = new(_mockLogger.Object, _mockLoginService.Object);

            OkObjectResult? response = (await nintendoLoginController.Post(request)).Result as OkObjectResult;

            response.Should().NotBeNull();
            response!.Value.Should().Be(createDeviceAccountResponse);
            _mockLoginService.VerifyAll();
        }
    }
}
