using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Nintendo;

namespace DragaliaAPI.Models;

public interface ISessionService
{
    /// <summary>
    /// Create a new session.
    /// </summary>
    /// <param name="deviceAccount">The device account to associate with the new session.</param>
    /// <returns>The session id.</returns>
    Task<string> CreateNewSession(DeviceAccount deviceAccount, string idToken);

    /// <summary>
    /// Check if a session with the given device account and id exists.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns>true if the session exists, false if it does not.</returns>
    bool ValidateSession(string sessionId);

    string? GetSessionIdFromIdToken(string idToken);

    Task<DbPlayerSavefile> GetSavefile(string sessionId);
}