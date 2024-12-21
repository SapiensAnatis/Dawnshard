using DragaliaAPI.Models;

namespace DragaliaAPI.Features.Login.Auth;

public interface ISessionService
{
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
    /// <exception cref="SessionException">A matching key was not found in the cache.</exception>
    Task<Session> LoadSessionSessionId(string sessionId);

    /// <summary>
    /// Get a session from an id token.
    /// </summary>
    /// <param name="idToken">The ID token.</param>
    /// <returns>The session object.</returns>
    /// <exception cref="SessionException">A matching key was not found in the cache.</exception>
    Task<Session> LoadSessionIdToken(string idToken);
    Task StartUserImpersonation(string targetAccountId, long targetViewerId);
    Task ClearUserImpersonation();
    Task<Session?> LoadImpersonationSession(string deviceAccountId);
}
