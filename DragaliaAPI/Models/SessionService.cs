
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Dragalia;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DragaliaAPI.Models;

public class SessionService : ISessionService
{
    private readonly IApiRepository _repository;
    private readonly IDistributedCache _cache;

    public SessionService(IApiRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }
    private List<Session> GetSessions()
    {
        string json = _cache.GetString(":sessions");
        return JsonSerializer.Deserialize<List<Session>>(json ?? "[]") ??
               throw new JsonException($"Failed to deserialize session list JSON {json} from Redis cache.");
    }

    // Readonly shortcut to be used instead of calling GetSessions() in every method
    private List<Session> Sessions { get { return GetSessions(); } }

    private void SetSessions(List<Session> sessions)
    {
        string json = JsonSerializer.Serialize(sessions);
        _cache.SetString(":sessions", json);
    }

    public async Task<string> CreateNewSession(DeviceAccount deviceAccount, string idToken)
    {
        List<Session> sessions = GetSessions();
        Session? existingSession = sessions.SingleOrDefault(x => x.DeviceAccountId == deviceAccount.id);
        if (existingSession != null)
            sessions.Remove(existingSession);

        long viewerId = (await _repository.GetSavefileByDeviceAccountId(deviceAccount.id)).ViewerId;
        string sessionId = Guid.NewGuid().ToString();

        Session session = new(sessionId, idToken, deviceAccount.id, viewerId);
        sessions.Add(session);
        SetSessions(sessions);

        return session.SessionId;
    }

    public bool ValidateSession(string sessionId)
    {
        return Sessions.Any(x => x.SessionId == sessionId);
    }

    public string? GetSessionIdFromIdToken(string idToken)
    {
        return Sessions.FirstOrDefault(x => x.IdToken == idToken)?.SessionId;
    }

    public async Task<DbPlayerSavefile> GetSavefile(string sessionId)
    {
        Session session = Sessions.First(x => x.SessionId == sessionId);

        return await _repository.GetSavefileByDeviceAccountId(session.DeviceAccountId);
    }

    private class Session
    {
        public string SessionId { get; }
        public string DeviceAccountId { get; set; }
        public string IdToken { get; init; }
        public long ViewerId { get; init; }

        public Session(string sessionId, string idToken, string deviceAccountId, long ViewerId)
        {
            this.SessionId = sessionId;
            this.IdToken = idToken;
            this.DeviceAccountId = deviceAccountId;
            this.ViewerId = ViewerId;
        }
    }
}