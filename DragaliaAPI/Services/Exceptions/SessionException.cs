namespace DragaliaAPI.Services.Exceptions;

public class SessionException : Exception
{
    public SessionException(string sessionId) : base($"Failed to lookup session: {sessionId}") { }
}
