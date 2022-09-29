using DragaliaAPI.Models.Database;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Test.Unit.Models;

public class SessionServiceTest
{
    private readonly Mock<IApiRepository> mockRepository;
    private readonly SessionService sessionService;
    private readonly DeviceAccount deviceAccount = new("id", "password");

    public SessionServiceTest()
    {
        mockRepository = new(MockBehavior.Strict);
        mockRepository.Setup(x => x.GetSavefileByDeviceAccountId("id")).ReturnsAsync(
            new DbPlayerSavefile() { DeviceAccountId = deviceAccount.id, ViewerId = 1 });

        var opts = Options.Create(new MemoryDistributedCacheOptions());
        IDistributedCache testCache = new MemoryDistributedCache(opts);

        sessionService = new(mockRepository.Object, testCache);

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