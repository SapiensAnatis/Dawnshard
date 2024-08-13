using DragaliaAPI.Models;
using DragaliaAPI.Models.Options;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Infrastructure.Middleware;

[UsedImplicitly]
public class MaintenanceActionFilter(
    IOptionsMonitor<MaintenanceOptions> options,
    ILogger<MaintenanceActionFilter> logger
) : IActionFilter
{
    private const ResultCode MaintenanceCode = ResultCode.CommonMaintenance;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (!options.CurrentValue.Enabled)
        {
            return;
        }

        logger.LogInformation("Rewriting response due to active maintenance.");

        context.HttpContext.Items[nameof(ResultCode)] = MaintenanceCode;

        context.Result = new OkObjectResult(
            new DragaliaResponse<object>(
                dataHeaders: new DataHeaders(MaintenanceCode),
                new ResultCodeResponse(MaintenanceCode)
            )
        );
    }
}
