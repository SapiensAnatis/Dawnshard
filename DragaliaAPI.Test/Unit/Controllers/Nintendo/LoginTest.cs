using System;
using Xunit;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Controllers.Nintendo;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using FluentAssertions;

namespace DragaliaAPI.Test.Controllers.Nintendo
{
    public class LoginTest
    {
        private readonly Mock<ILogger<NintendoLoginController>> _mockLogger = new(MockBehavior.Strict);
        private readonly Mock<ILoginService> _mockLoginFactory = new(MockBehavior.Strict);

        public LoginTest()
        {
            
        }

        private static LoginRequest TestLoginRequestFactory(DeviceAccount? deviceAccount)
        {
            return new LoginRequest(deviceAccount);
        }

        [Fact]
        public void LoginController_NullDeviceAccount_ReturnsCreatedDeviceAccount()
        {
            DeviceAccount deviceAccount = new("test id", "test password");

            LoginResponse createDeviceAccountResponse = new("accessToken", "idToken", "sessionId", deviceAccount)
            {
                createdDeviceAccount = deviceAccount
            };

            _mockLoginFactory.Setup(x => x.LoginResponseFactory()).Returns(createDeviceAccountResponse);

            LoginRequest request = TestLoginRequestFactory(null);
            NintendoLoginController nintendoLoginController = new(_mockLogger.Object, _mockLoginFactory.Object);
            
            LoginResponse response = nintendoLoginController.Post(request);

            response.Should().Be(createDeviceAccountResponse);
            _mockLoginFactory.VerifyAll();
        }
    }
}
