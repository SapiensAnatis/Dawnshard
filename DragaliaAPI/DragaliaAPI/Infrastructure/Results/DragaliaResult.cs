using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DragaliaAPI.Infrastructure.Results;

public class DragaliaResult<TValue> : IConvertToActionResult
    where TValue : class
{
    private readonly ResultCode resultCode = ResultCode.Success;

    public DragaliaResult(TValue value, ResultCode resultCode = ResultCode.Success)
    {
        this.Value = value;
        this.resultCode = resultCode;
    }

    public DragaliaResult(ActionResult result)
    {
        this.Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    public ActionResult? Result { get; }

    public TValue? Value { get; }

    public static implicit operator DragaliaResult<TValue>(TValue value) => new(value);

    public static implicit operator DragaliaResult<TValue>(ActionResult result) => new(result);

    public IActionResult Convert()
    {
        if (this.Result != null)
        {
            return this.Result;
        }

        ArgumentNullException.ThrowIfNull(this.Value);

        return new ObjectResult(new DragaliaResponse<TValue>(this.Value, this.resultCode))
        {
            DeclaredType = typeof(DragaliaResponse<TValue>),
            StatusCode = StatusCodes.Status200OK,
        };
    }
}

public class DragaliaResult : DragaliaResult<object>
{
    public DragaliaResult(object value)
        : base(value) { }

    public DragaliaResult(ActionResult result)
        : base(result) { }

    public static implicit operator DragaliaResult(ActionResult result) => new(result);
}
