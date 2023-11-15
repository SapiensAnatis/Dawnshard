namespace DragaliaAPI.Services.Exceptions;

public class DragaliaException : Exception
{
    public ResultCode Code { get; }

    public DragaliaException(ResultCode code)
        : base()
    {
        this.Code = code;
    }

    public DragaliaException(ResultCode code, string message)
        : base(message)
    {
        this.Code = code;
    }

    public DragaliaException(ResultCode code, string message, Exception innerException)
        : base(message, innerException)
    {
        this.Code = code;
    }
}
