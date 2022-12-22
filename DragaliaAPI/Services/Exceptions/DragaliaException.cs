using DragaliaAPI.Models;

namespace DragaliaAPI.Services.Exceptions;

public class DragaliaException : Exception
{
    public ResultCode Code { get; }

    public DragaliaException(ResultCode code, string message) : base(message)
    {
        this.Code = code;
    }
}
