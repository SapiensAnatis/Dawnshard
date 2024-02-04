using DragaliaAPI.Models;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Services.Game;

/// <summary>
/// SessionService interfaces with Redis to store the information about current sessions in-memory.
/// The basic flow looks like this:
///
/// 1. User comes to /tool/auth from BaaS with ID token
/// 2. SessionService creates a session which contains the claims from the ID token, such as viewer ID and account ID.
/// 3. This SID is sent back from the response in /tool/auth.
/// 4. All subsequent requests will contain the session ID in the header, and this can be used to
///    retrieve the savefile and update it if necessary.
/// </summary>
public class SessionService : ISessionService
{
    private readonly IDistributedCache cache;
    private readonly IOptionsMonitor<RedisCachingOptions> options;
    private readonly ILogger<SessionService> logger;
    private readonly TimeProvider dateTimeProvider;
    private readonly IPlayerIdentityService playerIdentityService;

    private DistributedCacheEntryOptions CacheOptions =>
        new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(options.CurrentValue.SessionExpiryTimeMinutes)
        };

    public SessionService(
        IDistributedCache cache,
        IOptionsMonitor<RedisCachingOptions> options,
        ILogger<SessionService> logger,
        TimeProvider dateTimeProvider,
        IPlayerIdentityService playerIdentityService
    )
    {
        this.cache = cache;
        this.options = options;
        this.logger = logger;
        this.dateTimeProvider = dateTimeProvider;
        this.playerIdentityService = playerIdentityService;
    }

    private static class Schema
    {
        public static string Session_IdToken(string idToken) => $":session:id_token:{idToken}";

        public static string Session_SessionId(string sessionId) =>
            $":session:session_id:{sessionId}";

        public static string SessionId_DeviceAccountId(string deviceAccountId) =>
            $":session:device_account_id:{deviceAccountId}";

        public static string ImpersonatedSession_DeviceAccountId(string deviceAccountId) =>
            $":impersonated_session:device_account_id:{deviceAccountId}";
    }

    [Obsolete(ObsoleteReasons.BaaS)]
    public async Task PrepareSession(DeviceAccount deviceAccount, string idToken)
    {
        // Check if there is an existing session, and if so, remove it
        string? existingSessionId = await cache.GetStringAsync(
            Schema.SessionId_DeviceAccountId(deviceAccount.id)
        );

        if (!string.IsNullOrEmpty(existingSessionId))
        {
            await cache.RemoveAsync(Schema.Session_SessionId(existingSessionId));
            await cache.RemoveAsync(Schema.SessionId_DeviceAccountId(deviceAccount.id));
        }

        string sessionId = Guid.NewGuid().ToString();

        // Filler viewerid for session as this flow is deprecated
        Session session =
            new(sessionId, idToken, deviceAccount.id, 47337, this.dateTimeProvider.GetUtcNow());
        await cache.SetStringAsync(
            Schema.Session_IdToken(idToken),
            JsonSerializer.Serialize(session),
            CacheOptions
        );

        logger.LogInformation(
            "Preparing session: DeviceAccount '{id}', id-token '{id_token}'",
            deviceAccount.id,
            idToken
        );
    }

    [Obsolete(ObsoleteReasons.BaaS)]
    public async Task<string> ActivateSession(string idToken)
    {
        Session session = await LoadSession(Schema.Session_IdToken(idToken));

        // Move key to sessionId
        // Don't remove -- sometimes /tool/auth is called multiple times consecutively?
        // await _cache.RemoveAsync(Schema.Session_IdToken(idToken));
        string? sessionJson = await cache.GetStringAsync(
            Schema.Session_SessionId(session.SessionId)
        );

        if (!string.IsNullOrEmpty(sessionJson))
        {
            // Issue existing session ID if session has already been activated
            Session existingSession =
                JsonSerializer.Deserialize<Session>(sessionJson)
                ?? throw new JsonException(
                    $"Loaded session JSON {sessionJson} could not be deserialized."
                );

            return existingSession.SessionId;
        }

        // Register in sessions
        await cache.SetStringAsync(
            Schema.Session_SessionId(session.SessionId),
            JsonSerializer.Serialize(session),
            CacheOptions
        );

        // Register in existent sessions
        await cache.SetStringAsync(
            Schema.SessionId_DeviceAccountId(session.DeviceAccountId),
            session.SessionId,
            CacheOptions
        );

        this.logger.LogInformation(
            "Activated session: id-token '{id_token}', issued session id '{session_id}'",
            idToken,
            session.SessionId
        );

        return session.SessionId;
    }

    public async Task<string> CreateSession(
        string idToken,
        string accountId,
        long viewerId,
        DateTimeOffset loginTime
    )
    {
        // Check for existing session
        Session? session = await this.TryLoadSession(Schema.Session_IdToken(idToken));
        if (session is null)
        {
            string sessionId = Guid.NewGuid().ToString();
            session = new(sessionId, idToken, accountId, viewerId, loginTime);
        }
        else
        {
            session = session with { LoginTime = loginTime };
        }

        // Register in sessions by id token (for reauth)
        await cache.SetStringAsync(
            Schema.Session_IdToken(idToken),
            JsonSerializer.Serialize(session),
            CacheOptions
        );

        // Register in sessions by session id (for all other endpoints)
        await cache.SetStringAsync(
            Schema.Session_SessionId(session.SessionId),
            JsonSerializer.Serialize(session),
            CacheOptions
        );

        return session.SessionId;
    }

    public async Task<Session> LoadSessionIdToken(string idToken) =>
        await this.LoadSession(Schema.Session_IdToken(idToken));

    public async Task<Session> LoadSessionSessionId(string sessionId) =>
        await this.LoadSession(Schema.Session_SessionId(sessionId));

    public async Task StartUserImpersonation(string targetAccountId, long targetViewerId)
    {
        Session session = new(string.Empty, string.Empty, targetAccountId, targetViewerId);

        // Set permanent key
        // We can use the identity service here as this is called from GraphQL mutations which
        // set the context via the other kind of user impersonation.
        await this.cache.SetStringAsync(
            Schema.ImpersonatedSession_DeviceAccountId(this.playerIdentityService.AccountId),
            JsonSerializer.Serialize(session)
        );
    }

    public async Task ClearUserImpersonation() =>
        await this.cache.RemoveAsync(
            Schema.ImpersonatedSession_DeviceAccountId(this.playerIdentityService.AccountId)
        );

    public async Task<Session?> LoadImpersonationSession(string deviceAccountId) =>
        await this.TryLoadSession(Schema.ImpersonatedSession_DeviceAccountId(deviceAccountId));

    private async Task<Session> LoadSession(string key) =>
        await this.TryLoadSession(key) ?? throw new SessionException(key);

    private async Task<Session?> TryLoadSession(string key)
    {
        string? json = await this.cache.GetStringAsync(key);

        if (json is null)
            return null;

        return JsonSerializer.Deserialize<Session>(json);
    }
}
