using DragaliaAPI.Models;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services;
using DragaliaAPI.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

namespace DragaliaAPI.Test.Unit.Services;

public class SessionServiceTest
{
    // TODO: Refactor this file to use the new methods
    private readonly Mock<ILogger<SessionService>> mockLogger;
    private readonly Mock<IOptionsMonitor<RedisOptions>> mockOptions;
    private readonly SessionService sessionService;

    [Obsolete(ObsoleteReasons.BaaS)]
    private readonly DeviceAccount deviceAccount = new("id", "password");

    [Obsolete(ObsoleteReasons.BaaS)]
    private readonly DeviceAccount deviceAccountTwo = new("id 2", "password 2");

    private readonly IDistributedCache testCache;

    public SessionServiceTest()
    {
        this.mockLogger = new(MockBehavior.Loose);
        this.mockOptions = new(MockBehavior.Strict);

        IOptions<MemoryDistributedCacheOptions> opts = Options.Create(
            new MemoryDistributedCacheOptions()
        );

        this.testCache = new MemoryDistributedCache(opts);
        this.mockOptions
            .SetupGet(x => x.CurrentValue)
            .Returns(new RedisOptions() { SessionExpiryTimeMinutes = 1 });

        sessionService = new(testCache, mockOptions.Object, mockLogger.Object);
    }

    [Fact]
    public async Task CreateSession_CanGetAfterwards()
    {
        string sessionId = await this.sessionService.CreateSession("token", "id", 1);

        Session session = await this.sessionService.LoadSessionSessionId(sessionId);

        session.SessionId.Should().Be(sessionId);
        session.IdToken.Should().Be("token");
        session.DeviceAccountId.Should().Be("id");
        session.ViewerId.Should().Be(1);
    }

    [Fact]
    public async Task CreateSession_SetsExpectedKeys()
    {
        string sessionId = await this.sessionService.CreateSession("token", "id", 1);

        this.testCache.GetString($":session:session_id:{sessionId}").Should().NotBeNull();
        this.testCache.GetString($":session:id_token:token").Should().NotBeNull();
    }

    [Obsolete(ObsoleteReasons.BaaS)]
    [Fact]
    public async Task NewSession_CreatesValidSession()
    {
        string sessionId = await PrepareAndRegisterSession("id_token", deviceAccount);

        this.testCache.GetString($":session:session_id:{sessionId}").Should().NotBeNull();
    }

    [Obsolete(ObsoleteReasons.BaaS)]
    [Fact]
    public async Task NewSession_ExistingSession_ReplacesOldSession()
    {
        string firstSessionId = await PrepareAndRegisterSession("id_token", deviceAccount);
        string secondSessionId = await PrepareAndRegisterSession("id_token", deviceAccount);

        this.testCache.GetString($":session:session_id:{firstSessionId}").Should().BeNull();
        this.testCache.GetString($":session:session_id:{secondSessionId}").Should().NotBeNull();

        secondSessionId.Should().NotBeEquivalentTo(firstSessionId);
    }

    [Obsolete(ObsoleteReasons.BaaS)]
    [Fact]
    public async Task NewSession_TwoCreated_BothValid()
    {
        string firstSessionId = await PrepareAndRegisterSession("id_token_1", deviceAccount);
        string secondSessionId = await PrepareAndRegisterSession("id_token_2", deviceAccountTwo);

        this.testCache.GetString($":session:session_id:{firstSessionId}").Should().NotBeNull();
        this.testCache.GetString($":session:session_id:{secondSessionId}").Should().NotBeNull();

        secondSessionId.Should().NotBeEquivalentTo(firstSessionId);
    }

    [Obsolete(ObsoleteReasons.BaaS)]
    private async Task<string> PrepareAndRegisterSession(
        string idToken,
        DeviceAccount deviceAccount
    )
    {
        await sessionService.PrepareSession(deviceAccount, idToken);
        return await sessionService.ActivateSession(idToken);
    }
}
