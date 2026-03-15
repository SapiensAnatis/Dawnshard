using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Maintenance;

[Route("maintenance")]
public partial class MaintenanceController(
    MaintenanceService maintenanceService,
    ILogger<MaintenanceService> logger
) : DragaliaControllerBaseCore
{
    [HttpPost("get_text")]
    public DragaliaResult<MaintenanceGetTextResponse> GetText(MaintenanceGetTextRequest request)
    {
        if (!maintenanceService.CheckIsMaintenance())
        {
            Log.InvalidCallToGetMaintenanceTextMaintenanceIsNotActive(logger);
            return this.Code(ResultCode.CommonServerError);
        }

        return new MaintenanceGetTextResponse()
        {
            MaintenanceText = maintenanceService.GetMaintenanceText(),
        };
    }

    private static partial class Log
    {
        [LoggerMessage(
            LogLevel.Error,
            "Invalid call to get maintenance text: maintenance is not active"
        )]
        public static partial void InvalidCallToGetMaintenanceTextMaintenanceIsNotActive(
            ILogger logger
        );
    }
}
