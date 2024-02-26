using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Maintenance;

[Route("maintenance")]
public class MaintenanceController(
    MaintenanceService maintenanceService,
    ILogger<MaintenanceService> logger
) : DragaliaControllerBaseCore
{
    [HttpPost("get_text")]
    public DragaliaResult<MaintenanceGetTextResponse> GetText(MaintenanceGetTextRequest request)
    {
        if (!maintenanceService.CheckIsMaintenance())
        {
            logger.LogError("Invalid call to get maintenance text: maintenance is not active");
            return this.Code(ResultCode.CommonServerError);
        }

        return new MaintenanceGetTextResponse()
        {
            MaintenanceText = maintenanceService.GetMaintenanceText()
        };
    }
}
