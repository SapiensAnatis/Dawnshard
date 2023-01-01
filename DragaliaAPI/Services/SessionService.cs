using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Services.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace DragaliaAPI.Services;

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
    private readonly DistributedCacheEntryOptions cacheOptions;
    private readonly ILogger<SessionService> logger;

    public SessionService(
        IDistributedCache cache,
        IConfiguration configuration,
        ILogger<SessionService> logger
    )
    {
        this.cache = cache;
        this.logger = logger;

        int expiryTimeMinutes = configuration.GetValue<int>("SessionExpiryTimeMinutes");
        cacheOptions = new() { SlidingExpiration = TimeSpan.FromMinutes(expiryTimeMinutes) };
    }

    private static class Schema
    {
        public static string Session_IdToken(string idToken)
        {
            return $":session:id_token:{idToken}";
        }

        public static string Session_SessionId(string sessionId) =>
            $":session:session_id:{sessionId}";

        public static string SessionId_DeviceAccountId(string deviceAccountId) =>
            $":session_id:device_account_id:{deviceAccountId}";
    }

    [Obsolete("Used for old pre-BaaS login flow")]
    public async Task PrepareSession(DeviceAccount deviceAccount, string idToken)
    {
        // Check if there is an existing session, and if so, remove it
        string? existingSessionId = await cache.GetStringAsync(
            Schema.SessionId_DeviceAccountId(deviceAccount.id)
        );

        if (!string.IsNullOrEmpty(existingSessionId))
        {
            // TODO: Consider abstracting this into a RemoveSession method, in case it needs to be done elsewhere
            await cache.RemoveAsync(Schema.Session_SessionId(existingSessionId));
            await cache.RemoveAsync(Schema.SessionId_DeviceAccountId(deviceAccount.id));
        }

        string sessionId = Guid.NewGuid().ToString();

        Session session = new(sessionId, deviceAccount.id);
        await cache.SetStringAsync(
            Schema.Session_IdToken(idToken),
            JsonSerializer.Serialize(session),
            cacheOptions
        );

        logger.LogInformation(
            "Preparing session: DeviceAccount '{id}', id-token '{id_token}'",
            deviceAccount.id,
            idToken
        );
    }

    [Obsolete("Used for old pre-BaaS login flow")]
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
            cacheOptions
        );

        // Register in existent sessions
        await cache.SetStringAsync(
            Schema.SessionId_DeviceAccountId(session.DeviceAccountId),
            session.SessionId,
            cacheOptions
        );

        this.logger.LogInformation(
            "Activated session: id-token '{id_token}', issued session id '{session_id}'",
            idToken,
            session.SessionId
        );

        return session.SessionId;
    }

    public async Task<string> CreateSession(string accountId, string idToken)
    {
        // Check for existing session
        Session? existingSession = await this.TryLoadSession(Schema.Session_IdToken(idToken));
        if (existingSession is not null)
            return existingSession.SessionId;

        string sessionId = Guid.NewGuid().ToString();
        Session session = new(sessionId, accountId);

        // Register in sessions by id token (for reauth)
        await cache.SetStringAsync(
            Schema.Session_IdToken(idToken),
            JsonSerializer.Serialize(session),
            cacheOptions
        );

        // Register in sessions by session id (for account id retrieval)
        await cache.SetStringAsync(
            Schema.Session_SessionId(sessionId),
            JsonSerializer.Serialize(session)
        );

        return sessionId;
    }

    public async Task<string> GetDeviceAccountId_SessionId(string sessionId)
    {
        Session session = await LoadSession(Schema.Session_SessionId(sessionId));

        return session.DeviceAccountId;
    }

    public async Task<string> GetDeviceAccountId_IdToken(string idToken)
    {
        Session session = await LoadSession(Schema.Session_IdToken(idToken));

        return session.DeviceAccountId;
    }

    private async Task<Session> LoadSession(string key)
    {
        string? sessionJson = await cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(sessionJson))
        {
            throw new SessionException(key);
        }

        return JsonSerializer.Deserialize<Session>(sessionJson)
            ?? throw new JsonException(
                $"Loaded session JSON {sessionJson} could not be deserialized."
            );
    }

    private async Task<Session?> TryLoadSession(string key)
    {
        string? json = await this.cache.GetStringAsync(key);

        if (json is null)
            return null;

        return JsonSerializer.Deserialize<Session>(json);
    }

    private record Session(string SessionId, string DeviceAccountId);
}
