using DragaliaAPI.Models.Nintendo;
using Microsoft.Extensions.Caching.Distributed;
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
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _cacheOptions;
    private readonly ILogger<SessionService> _logger;

    public SessionService(
        IDistributedCache cache,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        ILogger<SessionService> logger
    )
    {
        _cache = cache;
        _logger = logger;

        int expiryTimeMinutes = configuration.GetValue<int>("SessionExpiryTimeMinutes");
        _cacheOptions = new() { SlidingExpiration = TimeSpan.FromMinutes(expiryTimeMinutes) };

        if (environment.IsDevelopment())
        {
            // Create non-expiring 'development' session for manual testing
            Session devSession = new("session id", "id");
            string devSessionSchema1 = Schema.Session_SessionId(devSession.SessionId);
            string devSessionSchema2 = Schema.SessionId_DeviceAccountId(devSession.DeviceAccountId);

            // This is a scoped service -- so make sure it doesn't already exist
            if (string.IsNullOrEmpty(_cache.GetString(devSessionSchema1)))
                _cache.SetString(devSessionSchema1, JsonSerializer.Serialize(devSession));

            if (string.IsNullOrEmpty(_cache.GetString(devSessionSchema2)))
                _cache.SetString(devSessionSchema2, devSession.SessionId);
        }
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

    public async Task PrepareSession(DeviceAccount deviceAccount, string idToken)
    {
        // Check if there is an existing session, and if so, remove it
        string existingSessionId = await _cache.GetStringAsync(
            Schema.SessionId_DeviceAccountId(deviceAccount.id)
        );
        if (!string.IsNullOrEmpty(existingSessionId))
        {
            // TODO: Consider abstracting this into a RemoveSession method, in case it needs to be done elsewhere
            await _cache.RemoveAsync(Schema.Session_SessionId(existingSessionId));
            await _cache.RemoveAsync(Schema.SessionId_DeviceAccountId(deviceAccount.id));
        }

        string sessionId = Guid.NewGuid().ToString();

        Session session = new(sessionId, deviceAccount.id);
        await _cache.SetStringAsync(
            Schema.Session_IdToken(idToken),
            JsonSerializer.Serialize(session),
            _cacheOptions
        );
        _logger.LogInformation(
            "Preparing session: DeviceAccount '{id}', id-token '{id_token}'",
            deviceAccount.id,
            idToken
        );
    }

    public async Task<string> ActivateSession(string idToken)
    {
        Session session = await LoadSession(Schema.Session_IdToken(idToken));

        // Move key to sessionId
        // Don't remove -- sometimes /tool/auth is called multiple times consecutively?
        // await _cache.RemoveAsync(Schema.Session_IdToken(idToken));
        string? sessionJson = await _cache.GetStringAsync(
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

        await _cache.SetStringAsync(
            Schema.Session_SessionId(session.SessionId),
            JsonSerializer.Serialize(session),
            _cacheOptions
        );

        // Register in existent sessions
        await _cache.SetStringAsync(
            Schema.SessionId_DeviceAccountId(session.DeviceAccountId),
            session.SessionId,
            _cacheOptions
        );

        _logger.LogInformation(
            "Activated session: id-token '{id_token}', issued session id '{session_id}'",
            idToken,
            session.SessionId
        );

        return session.SessionId;
    }

    public async Task<bool> ValidateSession(string sessionId)
    {
        string? sessionJson = await _cache.GetStringAsync(Schema.Session_SessionId(sessionId));
        return !string.IsNullOrEmpty(sessionJson);
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
        string? sessionJson = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(sessionJson))
        {
            throw new SessionException(key);
        }

        return JsonSerializer.Deserialize<Session>(sessionJson)
            ?? throw new JsonException(
                $"Loaded session JSON {sessionJson} could not be deserialized."
            );
    }

    private record Session(string SessionId, string DeviceAccountId);
}
