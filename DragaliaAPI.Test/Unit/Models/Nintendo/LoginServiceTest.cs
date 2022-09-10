using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Castle.Core.Logging;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Nintendo;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DragaliaAPI.Test.Unit.Models.Nintendo
{
    public class LoginServiceTest
    {
        private static string sessionId = "session id";
        private readonly Mock<ISessionService> mockSessionService = new(MockBehavior.Strict);
        private readonly Mock<ILogger<LoginService>> mockLogger = new(MockBehavior.Strict);

        private readonly LoginService loginService;

        public LoginServiceTest()
        {
            loginService = new(mockSessionService.Object, mockLogger.Object);
            mockSessionService.Setup(a => a.CreateNewSession(It.IsAny<DeviceAccount>())).Returns(sessionId);
            mockLogger.Setup(x => x.Log(
                It.Is<LogLevel>(x => x == LogLevel.Information),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()));
        }

        [Fact]
        public void LoginResponseFactory_NoDeviceAccount_CreatesDeviceAccountAndSession()
        {
            LoginResponse loginResponse = loginService.LoginResponseFactory();

            loginResponse.deviceAccount.Should().NotBeNull();
            loginResponse.sessionId.Should().Be(sessionId);
            mockSessionService.VerifyAll();
        }

        [Fact]
        public void LoginResponseFactory_DeviceAccount_CreatesSession()
        {
            DeviceAccount deviceAccount = new("id", "password");

            LoginResponse loginResponse = loginService.LoginResponseFactory(deviceAccount);

            loginResponse.deviceAccount.Should().Be(deviceAccount);
            loginResponse.sessionId.Should().Be(sessionId);
            mockSessionService.VerifyAll();
        }
    }
}
