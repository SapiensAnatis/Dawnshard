using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("push_notification")]
public class PushNotificationController : DragaliaControllerBase
{
    [HttpPost("update_setting")]
    public DragaliaResult UpdateSetting() =>
        this.Ok(new PushNotificationUpdateSettingResponse() { Result = 1 });
}
