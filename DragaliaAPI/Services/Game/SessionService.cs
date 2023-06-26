using DragaliaAPI.Helpers;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Services.Game;

/// <summary>
/// SessionService interfaces with Redis to store the information about current sessions in-memory.
/// The basic flow looks like this:
///
/// 1. The NintendoLoginController calls PrepareSession with DeviceAccount information and an ID
///    token, and a session is created and stored in the cache indexed by the ID token. The
///    controller sends back the ID token.
///
/// 2. The client *may* later send that ID token in a request to SignupController, in which case it
///    just needs to be sent the ViewerId of the DeviceAccount's associated savefile. This does not
///    involve any cache writes.
///
/// 3. The client will later send the ID token in a request to AuthController, where ActivateSession
///    is called, which moves the key of the session from the id_token (hereafter unused) to the
///    session ID. The session ID is returned and sent in the response from AuthController.
///
/// 4. All subsequent requests will contain the session ID in the header, and this can be used to
///    retrieve the savefile and update it if necessary.
/// </summary>
public class SessionService : ISessionService
{
    private readonly IDistributedCache cache;
    private readonly IOptionsMonitor<RedisOptions> options;
    private readonly ILogger<SessionService> logger;
    private readonly IDateTimeProvider dateTimeProvider;

    private DistributedCacheEntryOptions CacheOptions =>
        new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(options.CurrentValue.SessionExpiryTimeMinutes)
        };

    public SessionService(
        IDistributedCache cache,
        IOptionsMonitor<RedisOptions> options,
        ILogger<SessionService> logger,
        IDateTimeProvider dateTimeProvider
    )
    {
        this.cache = cache;
        this.options = options;
        this.logger = logger;
        this.dateTimeProvider = dateTimeProvider;
    }

    private static class Schema
    {
        public static string Session_IdToken(string idToken) => $":session:id_token:{idToken}";

        public static string Session_SessionId(string sessionId) =>
            $":session:session_id:{sessionId}";

        public static string SessionId_DeviceAccountId(string deviceAccountId) =>
            $":session:device_account_id:{deviceAccountId}";
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
            new(sessionId, idToken, deviceAccount.id, 47337, this.dateTimeProvider.UtcNow);
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
