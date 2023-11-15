namespace DragaliaAPI.Services.Exceptions;

public class SessionException : DragaliaException
{
    public SessionException(string sessionId)
        : base(ResultCode.SessionSessionNotFound, $"Failed to lookup session: {sessionId}") { }

    public SessionException()
        : base(ResultCode.SessionSessionNotFound, string.Empty) { }
}
