using DragaliaAPI.Features.Shared.Models.Generated;
using DragaliaAPI.Infrastructure.Results;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("push_notification")]
public class PushNotificationController : DragaliaControllerBase
{
    [HttpPost("update_setting")]
    public DragaliaResult UpdateSetting() =>
        this.Ok(new PushNotificationUpdateSettingResponse() { Result = 1 });
}
