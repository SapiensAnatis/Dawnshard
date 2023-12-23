using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Dungeon.Start;
using DragaliaAPI.Models.Generated;
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
        RepeatInfo? info = await this.autoRepeatService.GetRepeatInfo();
        if (info == null)
        {
            this.logger.LogWarning("Failed to retrieve repeat data");
            return this.Code(ResultCode.DungeonRepeatNotPlayable);
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
                }
            };

        return response;
    }
}
