using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Test.Unit.Services;

public class SessionServiceTest
{
    // TODO: Refactor this file to use the new methods
    private readonly Mock<ILogger<SessionService>> mockLogger;
    private readonly SessionService sessionService;

    private readonly DeviceAccount deviceAccount = new("id", "password");
    private readonly DeviceAccount deviceAccountTwo = new("id 2", "password 2");

    private readonly IDistributedCache testCache;

    public SessionServiceTest()
    {
        mockLogger = new(MockBehavior.Loose);

        IOptions<MemoryDistributedCacheOptions> opts = Options.Create(
            new MemoryDistributedCacheOptions()
        );
        this.testCache = new MemoryDistributedCache(opts);

        Dictionary<string, string?> inMemoryConfiguration =
            new() { { "SessionExpiryTimeMinutes", "5" }, };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfiguration)
            .Build();

        sessionService = new(testCache, configuration, mockLogger.Object);
    }

    [Fact]
    public async Task NewSession_CreatesValidSession()
    {
        string sessionId = await PrepareAndRegisterSession("id_token", deviceAccount);

        this.testCache.GetString($":session:session_id:{sessionId}").Should().NotBeNull();
    }

    [Fact]
    public async Task NewSession_ExistingSession_ReplacesOldSession()
    {
        string firstSessionId = await PrepareAndRegisterSession("id_token", deviceAccount);
        string secondSessionId = await PrepareAndRegisterSession("id_token", deviceAccount);

        this.testCache.GetString($":session:session_id:{firstSessionId}").Should().BeNull();
        this.testCache.GetString($":session:session_id:{secondSessionId}").Should().NotBeNull();

        secondSessionId.Should().NotBeEquivalentTo(firstSessionId);
    }

    [Fact]
    public async Task NewSession_TwoCreated_BothValid()
    {
        string firstSessionId = await PrepareAndRegisterSession("id_token_1", deviceAccount);
        string secondSessionId = await PrepareAndRegisterSession("id_token_2", deviceAccountTwo);

        this.testCache.GetString($":session:session_id:{firstSessionId}").Should().NotBeNull();
        this.testCache.GetString($":session:session_id:{secondSessionId}").Should().NotBeNull();

        secondSessionId.Should().NotBeEquivalentTo(firstSessionId);
    }

    private async Task<string> PrepareAndRegisterSession(
        string idToken,
        DeviceAccount deviceAccount
    )
    {
        await sessionService.PrepareSession(deviceAccount, idToken);
        return await sessionService.ActivateSession(idToken);
    }
}
