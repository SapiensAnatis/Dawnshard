using DragaliaAPI.Models.Nintendo;

namespace DragaliaAPI.Models
{
    public interface ISessionService
    {
        /// <summary>
        /// Create a new session.
        /// </summary>
        /// <param name="deviceAccount">The device account to associate with the new session.</param>
        /// <returns>The session id.</returns>
        string CreateNewSession(DeviceAccount deviceAccount);

        /// <summary>
        /// Check if a session with the given device account and id exists.
        /// </summary>
        /// <param name="deviceAccount"></param>
        /// <param name="sessionId"></param>
        /// <returns>true if the session exists, false if it does not.</returns>
        bool ValidateSession(DeviceAccount deviceAccount, string sessionId);
    }
}