using DragaliaAPI.Models.Nintendo;

namespace DragaliaAPI.Services;

public interface ISessionService
{
    /// <summary>
    /// Pre-register a session and associate it with the idToken in-memory.
    /// </summary>
    /// <param name="deviceAccount">The device account to associate with the new session.</param>
    /// <param name="idToken">The id token to use as the key in the database.</param>
    [Obsolete("Used for pre-BaaS login flow")]
    Task PrepareSession(DeviceAccount deviceAccount, string idToken);

    /// <summary>
    /// Complete registration of a session and issue its ID to a client.
    /// </summary>
    /// <param name="idToken">The ID token to use to look up the pre-registered session</param>
    /// <returns>The session id.</returns>
    [Obsolete("Used for pre-BaaS login flow")]
    Task<string> ActivateSession(string idToken);

    /// <summary>
    /// Get a queryable for a player's savefile from an id token.
    /// Warning: Will fail if ActivateSession() has been called.
    /// </summary>
    /// <param name="idToken"></param>
    /// <returns></returns>
    Task<string> GetDeviceAccountId_IdToken(string idToken);

    /// <summary>
    /// Get a queryable for a player's savefile from a session id.
    /// </summary>
    /// <param name="sessionId">The session id.</param>
    /// <returns></returns>
    Task<string> GetDeviceAccountId_SessionId(string sessionId);
    Task<string> CreateSession(string accountId, string idToken);
}
