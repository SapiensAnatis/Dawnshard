using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Dungeon.AutoRepeat;

[Route("repeat")]
public class RepeatController(
    IAutoRepeatService autoRepeatService,
    ILogger<RepeatController> logger
) : DragaliaControllerBase
{
    private readonly IAutoRepeatService autoRepeatService = autoRepeatService;
    private readonly ILogger<RepeatController> logger = logger;

    [HttpPost("end")]
    public async Task<DragaliaResult<RepeatEndResponse>> End()
    {
        RepeatInfo? info = await this.autoRepeatService.ClearRepeatInfo();
        if (info == null)
        {
            throw new DragaliaException(
                ResultCode.DungeonRepeatNotPlayable,
                "Failed to retrieve repeat data"
            );
        }

        RepeatEndResponse response =
            new()
            {
                IngameResultData = info.IngameResultData,
                UpdateDataList = info.UpdateDataList,
                EntityResult = new()
                {
                    ConvertedEntityList = [],
                    NewGetEntityList = [],
                    OverDiscardEntityList = [],
                    OverPresentLimitEntityList = [],
                    OverPresentEntityList = []
                },
                RepeatData = new(info.Key.ToString(), info.CurrentCount, 1)
            };

        return response;
    }
}
