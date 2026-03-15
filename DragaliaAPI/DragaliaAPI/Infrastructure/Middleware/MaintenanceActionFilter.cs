using DragaliaAPI.Features.Shared.Options;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Infrastructure.Middleware;

[UsedImplicitly]
public partial class MaintenanceActionFilter(
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

        Log.RewritingResponseDueToActiveMaintenance(logger);

        context.HttpContext.Items[nameof(ResultCode)] = MaintenanceCode;

        context.Result = new OkObjectResult(
            new DragaliaResponse<object>(
                new ResultCodeResponse(MaintenanceCode),
                dataHeaders: new(MaintenanceCode)
            )
        );
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Rewriting response due to active maintenance.")]
        public static partial void RewritingResponseDueToActiveMaintenance(ILogger logger);
    }
}
