using DragaliaAPI.Models;
using DragaliaAPI.Models.Options;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Middleware;

[UsedImplicitly]
public class MaintenanceActionFilter(
    IOptionsMonitor<MaintenanceOptions> options,
    ILogger<MaintenanceActionFilter> logger
) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (!options.CurrentValue.Enabled)
        {
            return;
        }

        logger.LogInformation("Rewriting response due to active maintenance.");

        context.Result = new OkObjectResult(
            new DragaliaResponse<object>(
                dataHeaders: new DataHeaders(ResultCode.CommonMaintenance),
                new ResultCodeResponse(ResultCode.CommonMaintenance)
            )
        );
    }
}
