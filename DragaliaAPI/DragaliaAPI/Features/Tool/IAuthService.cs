namespace DragaliaAPI.Features.Tool;

public interface IAuthService
{
    Task<(long viewerId, string sessionId)> DoAuth(string idToken);
}
