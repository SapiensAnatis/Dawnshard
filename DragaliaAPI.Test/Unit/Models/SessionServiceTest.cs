using DragaliaAPI.Models.Database;

namespace DragaliaAPI.Test.Unit.Models
{
    public class SessionServiceTest
    {
        private readonly Mock<IApiRepository> mockRepository;
        private readonly SessionService sessionService;
        private readonly DeviceAccount deviceAccount = new("id", "password");

        public SessionServiceTest()
        {
            mockRepository = new(MockBehavior.Strict);
            sessionService = new(mockRepository.Object);
        }

        [Fact]
        public async Task CreateNewSession_NewSession_CreatesValidSession()
        {
            string sessionId = await sessionService.CreateNewSession(deviceAccount, "idToken");

            sessionService.ValidateSession(sessionId).Should().Be(true);
        }

        [Fact]
        public async Task CreateNewSession_ExistingSession_ReplacesOldSession()
        {
            string firstSessionId = await sessionService.CreateNewSession(deviceAccount, "idToken");
            string secondSessionId = await sessionService.CreateNewSession(deviceAccount, "idToken");

            sessionService.ValidateSession(secondSessionId).Should().Be(true);
            secondSessionId.Should().NotBeEquivalentTo(firstSessionId);
        }

        [Fact]
        public void ValidateSession_NonExistentSession_ReturnsFalse()
        {
            bool result = sessionService.ValidateSession("sessionId");
            result.Should().Be(false);
        }
    }


}
