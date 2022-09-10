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
        private readonly Mock<ILoginFactory> _mockLoginFactory = new(MockBehavior.Strict);

        public LoginTest()
        {
            
        }

        #nullable enable
        private static LoginRequest TestLoginRequestFactory(DeviceAccount? deviceAccount)
        {
            return new()
            {
                appVersion = "2.19.0",
                assertion = "assertion",
                carrier = "giffgaff",
                deviceAccount = deviceAccount,
                deviceAnalyticsId = "id",
                deviceName = "ONEPLUS A6003",
                locale = "en-US",
                manufacturer = "OnePlus",
                networkType = "wifi",
                osType = "Android",
                osVersion = "11",
                sdkVersion = "Unity-2.33.0-0a4be7c8",
                sessionId = "",
                timeZone = "Europe/London",
                timeZoneOffset = 3600000
            };
        }
        #nullable restore

        [Fact]
        public void LoginController_NullDeviceAccount_ReturnsCreatedDeviceAccount()
        {
            DeviceAccount deviceAccount = new()
            {
                id = "test id",
                password = "test password",
            };

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
