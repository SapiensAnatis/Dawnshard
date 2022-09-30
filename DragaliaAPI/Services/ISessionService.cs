using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Nintendo;

namespace DragaliaAPI.Services;

public interface ISessionService
{
    /// <summary>
    /// Pre-register a session and associate it with the idToken in-memory.
    /// </summary>
    /// <param name="deviceAccount">The device account to associate with the new session.</param>
    /// <param name="idToken">The id token to use as the key in the database.</param>
    Task PrepareSession(DeviceAccount deviceAccount, string idToken);

    /// <summary>
    /// Complete registration of a session and issue its ID to a client.
    /// </summary>
    /// <param name="idToken">The ID token to use to look up the pre-registered session</param>
    /// <returns>The session id.</returns>
    Task<string> ActivateSession(string idToken);

    /// <summary>
    /// Check if a session with the given device account and id exists.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns>true if the session exists, false if it does not.</returns>
    Task<bool> ValidateSession(string sessionId);

    /// <summary>
    /// Get a queryable for a player's savefile from an id token.
    /// Warning: Will fail if ActivateSession() has been called.
    /// </summary>
    /// <param name="idToken"></param>
    /// <returns></returns>
    Task<IQueryable<DbPlayerSavefile>> GetSavefile_IdToken(string idToken);

    /// <summary>
    /// Get a queryable for a player's savefile from a session id.
    /// </summary>
    /// <param name="sessionId">The session id.</param>
    /// <returns></returns>
    Task<IQueryable<DbPlayerSavefile>> GetSavefile_SessionId(string sessionId);
}