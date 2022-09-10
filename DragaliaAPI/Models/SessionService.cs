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
            public string Id { get; set; }
            public DeviceAccount deviceAccount { get; set; }

            public Session(DeviceAccount deviceAccount)
            {
                Id = Guid.NewGuid().ToString();
                this.deviceAccount = deviceAccount;
            }
        }

        private List<Session> _sessions = new List<Session>();

        public string CreateNewSession(DeviceAccount deviceAccount)
        {
            Session? existingSession = _sessions.SingleOrDefault(x => x.deviceAccount.id == deviceAccount.id);
            if (existingSession != null)
                _sessions.Remove(existingSession);
            
            Session session = new(deviceAccount);
            _sessions.Add(session);

            return session.Id;
        }

        public bool ValidateSession(DeviceAccount deviceAccount, string sessionId)
        {
            return _sessions.Any(x => x.deviceAccount.id == deviceAccount.id && x.Id == sessionId);
        }
    }
}
