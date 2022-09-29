
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Dragalia;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Models
{
    public class SessionService : ISessionService
    {
        private readonly IApiRepository _repository;

        public SessionService(IApiRepository repository)
        {
            _repository = repository;
        }

        // TODO: Implement Redis for session state management
        private readonly List<Session> _sessions = new();

        public async Task<string> CreateNewSession(DeviceAccount deviceAccount, string idToken)
        {
            Session? existingSession = _sessions.SingleOrDefault(x => x.deviceAccount.id == deviceAccount.id);
            if (existingSession != null)
                _sessions.Remove(existingSession);

            long viewerId = (await _repository.GetSavefileByDeviceAccountId(deviceAccount.id)).ViewerId;

            Session session = new(idToken, deviceAccount, viewerId);
            _sessions.Add(session);

            return session.Id;
        }

        public bool ValidateSession(string sessionId)
        {
            return _sessions.Any(x => x.Id == sessionId);
        }

        public string? GetSessionIdFromIdToken(string idToken)
        {
            return _sessions.FirstOrDefault(x => x.IdToken == idToken)?.Id;
        }

        public async Task<DbPlayerSavefile> GetSavefile(string sessionId)
        {
            Session session = _sessions.First(x => x.Id == sessionId);

            return await _repository.GetSavefileByDeviceAccountId(session.deviceAccount.id);
        }

        private class Session
        {
            public string Id { get; }
            public DeviceAccount deviceAccount { get; set; }
            public string IdToken { get; init; }
            public long ViewerId { get; init; }

            public Session(string idToken, DeviceAccount deviceAccount, long ViewerId)
            {
                this.Id = Guid.NewGuid().ToString();
                this.IdToken = idToken;
                this.deviceAccount = deviceAccount;
                this.ViewerId = ViewerId;
            }
        }
    }
}
