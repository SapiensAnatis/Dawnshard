using DragaliaAPI.Models;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Shared;

namespace DragaliaAPI.Services;

public interface ISessionService
{
    /// <summary>
    /// Pre-register a session and associate it with the idToken in-memory.
    /// </summary>
    /// <param name="deviceAccount">The device account to associate with the new session.</param>
    /// <param name="idToken">The id token to use as the key in the database.</param>
    [Obsolete(ObsoleteReasons.BaaS)]
    Task PrepareSession(DeviceAccount deviceAccount, string idToken);

    /// <summary>
    /// Complete registration of a session and issue its ID to a client.
    /// </summary>
    /// <param name="idToken">The ID token to use to look up the pre-registered session</param>
    /// <returns>The session id.</returns>
    [Obsolete(ObsoleteReasons.BaaS)]
    Task<string> ActivateSession(string idToken);

    /// <summary>
    /// Create a new session.
    /// </summary>
    /// <param name="idToken">The BaaS ID token/</param>
    /// <param name="accountId">The BaaS subject.</param>
    /// <param name="viewerId">The UserData ViewerID.</param>
    /// <param name="loginTime">The login time of the user.</param>
    /// <returns>The session ID.</returns>
    Task<string> CreateSession(
        string idToken,
        string accountId,
        long viewerId,
        DateTimeOffset loginTime
    );

    /// <summary>
    /// Get a session from an id.
    /// </summary>
    /// <param name="sessionId">The session id.</param>
    /// <returns>The session object.</returns>
    /// <exception cref="Exceptions.SessionException">A matching key was not found in the cache.</exception>
    Task<Session> LoadSessionSessionId(string sessionId);

    /// <summary>
    /// Get a session from an id token.
    /// </summary>
    /// <param name="idToken">The ID token.</param>
    /// <returns>The session object.</returns>
    /// <exception cref="Exceptions.SessionException">A matching key was not found in the cache.</exception>
    Task<Session> LoadSessionIdToken(string idToken);
    Task StartUserImpersonation(string targetAccountId, long targetViewerId);
    Task ClearUserImpersonation();
    Task<Session?> LoadImpersonationSession(string deviceAccountId);
}
