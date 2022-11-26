namespace DragaliaAPI.Services;

public class SessionException : Exception
{
    public SessionException(string sessionId) : base($"Failed to lookup session: {sessionId}") { }
}
