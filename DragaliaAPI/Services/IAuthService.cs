namespace DragaliaAPI.Services;

public interface IAuthService
{
    Task<(long viewerId, string sessionId)> DoAuth(string idToken);
}
