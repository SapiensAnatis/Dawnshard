using DragaliaAPI.Features.Login.Auth;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Test.Services;

public class SessionServiceTest
{
    // TODO: Refactor this file to use the new methods
    private readonly Mock<ILogger<SessionService>> mockLogger;
    private readonly Mock<IOptionsMonitor<RedisCachingOptions>> mockOptions;
    private readonly Mock<TimeProvider> mockTimeProvider;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;
    private readonly SessionService sessionService;

    private readonly IDistributedCache testCache;

    public SessionServiceTest()
    {
        this.mockLogger = new(MockBehavior.Loose);
        this.mockOptions = new(MockBehavior.Strict);
        this.mockTimeProvider = new(MockBehavior.Strict);
        this.mockPlayerIdentityService = new(MockBehavior.Strict);

        IOptions<MemoryDistributedCacheOptions> opts = Options.Create(
            new MemoryDistributedCacheOptions()
        );

        this.testCache = new MemoryDistributedCache(opts);
        this.mockOptions.SetupGet(x => x.CurrentValue)
            .Returns(new RedisCachingOptions() { SessionExpiryTimeMinutes = 1 });

        sessionService = new(
            this.testCache,
            this.mockOptions.Object,
            this.mockLogger.Object,
            this.mockTimeProvider.Object,
            this.mockPlayerIdentityService.Object
        );

        this.mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task CreateSession_CanGetAfterwards()
    {
        string sessionId = await this.sessionService.CreateSession(
            "token",
            "id",
            1,
            DateTimeOffset.UnixEpoch
        );

        Session session = await this.sessionService.LoadSessionSessionId(sessionId);

        session.SessionId.Should().Be(sessionId);
        session.IdToken.Should().Be("token");
        session.DeviceAccountId.Should().Be("id");
        session.ViewerId.Should().Be(1);
        session.LoginTime.Should().Be(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task CreateSession_SetsExpectedKeys()
    {
        string sessionId = await this.sessionService.CreateSession(
            "token",
            "id",
            1,
            DateTimeOffset.UnixEpoch
        );

        this.testCache.GetString($":session:session_id:{sessionId}").Should().NotBeNull();
        this.testCache.GetString($":session:id_token:token").Should().NotBeNull();
    }
}
