using System;
using System.Collections.Generic;
using System.Linq;
using DragaliaAPI.Models.Nintendo;

namespace DragaliaAPI.Models
{
    public class SessionService : ISessionService
    {
        private class Session
        {
            public string Id { get; }
            public Nintendo.DeviceAccount deviceAccount { get; set; }
            public string IdToken { get; init; }

            public Session(Nintendo.DeviceAccount deviceAccount, string idToken)
            {
                this.Id = Guid.NewGuid().ToString();
                this.IdToken = idToken;
                this.deviceAccount = deviceAccount;
            }
        }

        private readonly List<Session> _sessions = new();

        public string CreateNewSession(Nintendo.DeviceAccount deviceAccount, string idToken)
        {
            Session? existingSession = _sessions.SingleOrDefault(x => x.deviceAccount.id == deviceAccount.id);
            if (existingSession != null)
                _sessions.Remove(existingSession);
            
            Session session = new(deviceAccount, idToken);
            _sessions.Add(session);

            return session.Id;
        }

        public bool ValidateSession(Nintendo.DeviceAccount deviceAccount, string sessionId)
        {
            return _sessions.Any(x => x.deviceAccount.id == deviceAccount.id && x.Id == sessionId);
        }

        public string? SessionIdFromIdToken(string idToken)
        {
            return _sessions.FirstOrDefault(x => x.IdToken == idToken)?.Id;
        }
    }
}
