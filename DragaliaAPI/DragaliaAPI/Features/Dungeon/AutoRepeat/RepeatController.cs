using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Dungeon.Start;
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
    public async Task<DragaliaResult<RepeatEndData>> End()
    {
        RepeatInfo? info = await this.autoRepeatService.ClearRepeatInfo();
        if (info == null)
        {
            throw new DragaliaException(
                ResultCode.DungeonRepeatNotPlayable,
                "Failed to retrieve repeat data"
            );
        }

        RepeatEndData response =
            new()
            {
                ingame_result_data = info.IngameResultData,
                update_data_list = info.UpdateDataList,
                entity_result = new()
                {
                    converted_entity_list = [],
                    new_get_entity_list = [],
                    over_discard_entity_list = [],
                    over_present_limit_entity_list = [],
                    over_present_entity_list = []
                },
                repeat_data = new(info.Key.ToString(), info.CurrentCount, 1)
            };

        return response;
    }
}
