using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DragaliaAPI.Infrastructure.Middleware;

/// <summary>
/// Service filter for populating <see cref="HttpContext.Items"/> with the result code of the response,
/// so that it can be logged out in <see cref="ResultCodeLoggingMiddleware"/>.
/// </summary>
public class SetResultCodeActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is not ObjectResult { Value: DragaliaResponse dragaliaResponse })
        {
            return;
        }

        context.HttpContext.Items[nameof(ResultCode)] = dragaliaResponse.DataHeaders.ResultCode;
    }
}
