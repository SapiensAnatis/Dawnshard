using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Nintendo;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Xunit;

namespace DragaliaAPI.Test.Unit.Models
{
    public class SessionServiceTest
    {
        private readonly SessionService sessionService = new();
        private readonly DeviceAccount deviceAccount = new("id", "password");

        [Fact]
        public void CreateNewSession_NewSession_CreatesValidSession()
        {
            string sessionId = sessionService.CreateNewSession(deviceAccount, "idToken");

            sessionService.ValidateSession(deviceAccount, sessionId).Should().Be(true);
        }

        [Fact]
        public void CreateNewSession_ExistingSession_ReplacesOldSession()
        {
            string firstSessionId = sessionService.CreateNewSession(deviceAccount, "idToken");
            string secondSessionId = sessionService.CreateNewSession(deviceAccount, "idToken");

            sessionService.ValidateSession(deviceAccount, secondSessionId).Should().Be(true);
            secondSessionId.Should().NotBeEquivalentTo(firstSessionId);
        }

        [Fact]
        public void ValidateSession_NonExistentSession_ReturnsFalse()
        {
            bool result = sessionService.ValidateSession(deviceAccount, "sessionId");
            result.Should().Be(false);
        }
    }


}
