using DragaliaAPI.Models;

namespace DragaliaAPI.Services.Exceptions;

public class SessionException : DragaliaException
{
    public SessionException(string sessionId)
        : base(ResultCode.SESSION_SESSION_NOT_FOUND, $"Failed to lookup session: {sessionId}") { }

    public SessionException() : base(ResultCode.SESSION_SESSION_NOT_FOUND, "") { }
}
