using System;
using Xunit;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Controllers.Nintendo;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace DragaliaAPI.Test.Controllers.Nintendo
{
    public class LoginTest
    {
        private readonly Mock<ILogger<NintendoLoginController>> _mockLogger = new(MockBehavior.Strict);
        public LoginTest()
        {
        }

        private static LoginRequest LoginRequestFactory(DeviceAccount? deviceAccount)
        {
            return new()
            {
                appVersion = "2.19.0",
                assertion = "eyJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJjb20ubmludGVuZG8uemFnYTowYzNkNzg5ZjVlZDIzZjJiMzRjNzk2NjBhMzcxOTBkMWM4NzNhM2YyIiwiaWF0IjoxNjYyODIwODU3LCJhdWQiOiJodHRwczpcL1wvNDhjYzgxY2RiOGRlMzBlMDYxOTI4ZjU2ZTliZDRiNGQuYmFhcy5uaW50ZW5kby5jb20ifQ==.iok6IkQfBzGLXjO1snF6Rk0nAQ5brU2oMBmXrlfJZ24=",
                carrier = "giffgaff",
                deviceAccount = deviceAccount,
                deviceAnalyticsId = "a2J1YmFhYWFERG1NamZtckpNTmVqSHZ6UGJWUE9FUwA=",
                deviceName = "ONEPLUS A6003",
                locale = "en-US",
                manufacturer = "OnePlus",
                networkType = "wifi",
                osType = "Android",
                osVersion = "11",
                sdkVersion = "Unity-2.33.0-0a4be7c8",
                sessionId = "271bd4fcd3d3e035-1662820849066",
                timeZone = "Europe/London",
                timeZoneOffset = 3600000
            };
        }

        [Fact]
        public void LoginController_NullDeviceAccount_ReturnsCreatedDeviceAccount()
        {
            LoginRequest request = LoginRequestFactory(null);
            NintendoLoginController nintendoLoginController = new(_mockLogger.Object);
            
            LoginResponse response = nintendoLoginController.Post(request);

            string jsonString = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });

            Assert.NotNull(response); 
        }
    }
}
