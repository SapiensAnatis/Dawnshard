using DragaliaAPI.Features.Maintenance;
using DragaliaAPI.Infrastructure.Results;
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
    private readonly IOptionsMonitor<MaintenanceOptions> options = options;
    private readonly ILogger<MaintenanceActionFilter> logger = logger;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!this.options.CurrentValue.Enabled)
            return;

        this.logger.LogInformation("Aborting request due to active maintenance.");

        context.Result = new OkObjectResult(
            new DragaliaResponse<object>(
                dataHeaders: new DataHeaders(ResultCode.CommonMaintenance),
                new ResultCodeResponse(ResultCode.CommonMaintenance)
            )
        );
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
