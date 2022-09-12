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
        private readonly Mock<ISessionService> mockSessionService = new(MockBehavior.Strict);
        private readonly Mock<ILogger<LoginService>> mockLogger = new(MockBehavior.Loose);
        private readonly DeviceAccount deviceAccount = new("id", "password");

        private readonly LoginService loginService;

        public LoginServiceTest()
        {
            loginService = new(mockLogger.Object, mockSessionService.Object);
            mockSessionService.Setup(x => x.CreateNewSession(deviceAccount, It.IsAny<string>())).Returns("session id");
        }

        [Fact]
        public async Task Login_DeviceAccountCorrectPassword_CreatesSession()
        {
            LoginResponse loginResponse = await loginService.Login(deviceAccount);

            loginResponse.deviceAccount.Should().Be(deviceAccount);
            mockSessionService.VerifyAll();
        }

        [Fact]
        public async Task Login_DeviceAccountIncorrectPassword_ReturnsFalse()
        {
            // mockDatabase.Authenticate.Returns(false)
            LoginResponse loginResponse = await loginService.Login(deviceAccount);
        }
    }
}
