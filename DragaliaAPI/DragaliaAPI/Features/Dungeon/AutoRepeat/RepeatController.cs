using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Dungeon.AutoRepeat;

[Route("repeat")]
public class RepeatController(IAutoRepeatService autoRepeatService) : DragaliaControllerBase
{
    [HttpPost("end")]
    public async Task<DragaliaResult<RepeatEndResponse>> End()
    {
        RepeatInfo? info =
            await autoRepeatService.ClearRepeatInfo()
            ?? throw new DragaliaException(
                ResultCode.DungeonRepeatNotPlayable,
                "Failed to retrieve repeat data"
            );

        RepeatEndResponse response = new()
        {
            IngameResultData = info.IngameResultData,
            UpdateDataList = info.UpdateDataList,
            EntityResult = new()
            {
                ConvertedEntityList = [],
                NewGetEntityList = [],
                OverDiscardEntityList = [],
                OverPresentLimitEntityList = [],
                OverPresentEntityList = [],
            },
            RepeatData = null,
        };

        return response;
    }
}
