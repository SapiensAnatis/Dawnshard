using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Dragalia;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DragaliaAPI.Services;

public class SessionService : ISessionService
{
    private readonly IApiRepository _apiRepository;
    private readonly IDistributedCache _cache;

    public SessionService(IApiRepository repository, IDistributedCache cache)
    {
        _apiRepository = repository;
        _cache = cache;
    }

    private static class Schema
    {
        public static string Session_IdToken(string idToken)
            => $":session:id_token:{idToken}";

        public static string Session_SessionId(string sessionId)
            => $":session:session_id:{sessionId}";
        
        public static string SessionId_DeviceAccountId(string deviceAccountId)
            => $":session_id:device_account_id:{deviceAccountId}";
    }
    
    public async Task PrepareSession(DeviceAccount deviceAccount, string idToken)
    {
        // Check if there is an existing session, and if so, remove it
        string existingSessionId = await _cache.GetStringAsync(Schema.SessionId_DeviceAccountId(deviceAccount.id));
        if (!string.IsNullOrEmpty(existingSessionId))
        {
            // TODO: Consider abstracting this into a RemoveSession method, in case it needs to be done elsewhere
            await _cache.RemoveAsync(Schema.Session_SessionId(existingSessionId));
            await _cache.RemoveAsync(Schema.SessionId_DeviceAccountId(deviceAccount.id));
        }

        IQueryable<DbPlayerSavefile> savefile = _apiRepository.GetSavefile(deviceAccount.id);
        long viewerId = await savefile.Select(x => x.ViewerId).SingleAsync();
        string sessionId = Guid.NewGuid().ToString();

        Session session = new(sessionId, deviceAccount.id, viewerId);
        await _cache.SetStringAsync(Schema.Session_IdToken(idToken), JsonSerializer.Serialize(session));
    }

    public async Task<string> ActivateSession(string idToken)
    {
        // Helper method not used as we want to retain the JSON to move it
        string sessionJson = await _cache.GetStringAsync(Schema.Session_IdToken(idToken));
        if (string.IsNullOrEmpty(sessionJson)) { throw new ArgumentException($"Could not load session for ID token {idToken}"); }

        Session session = JsonSerializer.Deserialize<Session>(sessionJson) ?? throw new JsonException();

        // Move key to sessionId
        await _cache.RemoveAsync(Schema.Session_IdToken(idToken));
        await _cache.SetStringAsync(Schema.Session_SessionId(session.SessionId), sessionJson);
        // Register in existent sessions
        await _cache.SetStringAsync(Schema.SessionId_DeviceAccountId(session.DeviceAccountId), session.SessionId);

        return session.SessionId;
    }

    public async Task<bool> ValidateSession(string sessionId)
    {
        string sessionJson = await _cache.GetStringAsync(Schema.Session_SessionId(sessionId));
        return !string.IsNullOrEmpty(sessionJson);
    }

    public async Task<IQueryable<DbPlayerSavefile>> GetSavefile_SessionId(string sessionId)
    {
        Session session = await LoadSession(Schema.Session_SessionId(sessionId));

        return _apiRepository.GetSavefile(session.DeviceAccountId);
    }

    public async Task<IQueryable<DbPlayerSavefile>> GetSavefile_IdToken(string idToken)
    {
        Session session = await LoadSession(Schema.Session_IdToken(idToken));

        return _apiRepository.GetSavefile(session.DeviceAccountId);
    }

    private async Task<Session> LoadSession(string key)
    {
        string sessionJson = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(sessionJson)) { throw new ArgumentException($"Could not load session for key {key}"); }

        return JsonSerializer.Deserialize<Session>(sessionJson) ?? throw new JsonException();
    }

    private record Session(string SessionId, string DeviceAccountId, long ViewerId);
}