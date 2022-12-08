using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

//TODO: proper implementation
[Route("present")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class PresentController : DragaliaControllerBase
{
    private readonly IUpdateDataService _updateDataService;

    public PresentController(IUpdateDataService updateDataService)
    {
        _updateDataService = updateDataService;
    }

    [Route("get_present_list")]
    [HttpPost]
    public async Task<DragaliaResult> GetPresentList(
        [FromBody] PresentGetPresentListRequest request
    )
    {
        return Ok(
            new PresentGetPresentListData(
                new List<PresentDetailList>()
                {
                    new PresentDetailList()
                    {
                        present_id = 1,
                        state = 0,
                        entity_type = EntityTypes.PaidDiamantium,
                        entity_id = 0,
                        message_id = 2,
                        entity_quantity = 1200000,
                        message_param_value_1 = 1,
                        message_param_value_2 = 1,
                        message_param_value_3 = 1,
                        message_param_value_4 = 1,
                        create_time = DateTimeOffset.UtcNow,
                        receive_limit_time = DateTimeOffset.UtcNow.AddDays(7)
                    }
                },
                new List<PresentDetailList>(),
                new() { present_notice = new(1, 1) },
                new()
            )
        );
    }

    [Route("receive")]
    [HttpPost]
    public async Task<DragaliaResult> Receive([FromBody] PresentReceiveRequest request)
    {
        UpdateDataList ul = _updateDataService.GetUpdateDataList(this.DeviceAccountId);
        return Ok(
            new PresentReceiveData()
            {
                receive_present_id_list = new List<ulong>(),
                delete_present_id_list = new List<ulong>(),
                present_list = new List<PresentDetailList>()
                {
                    new PresentDetailList()
                    {
                        present_id = 1,
                        state = 0,
                        entity_type = EntityTypes.PaidDiamantium,
                        entity_id = 0,
                        entity_quantity = 1200000,
                        message_id = 2,
                        message_param_value_1 = 1,
                        message_param_value_2 = 1,
                        message_param_value_3 = 1,
                        message_param_value_4 = 1,
                        create_time = DateTimeOffset.UtcNow,
                        receive_limit_time = DateTimeOffset.UtcNow.AddDays(7)
                    }
                },
                present_limit_list = new List<PresentDetailList>(),
                limit_over_present_id_list = request.present_id_list,
                not_receive_present_id_list = request.present_id_list,
                update_data_list = ul
            }
        );
    }

    [MessagePackObject(true)]
    public record GetHistoryListRequest(int present_history_id);

    [Route("get_history_list")]
    [HttpPost]
    public async Task<DragaliaResult> GetHistoryList([FromBody] GetHistoryListRequest request)
    {
        return Ok(
            new PresentGetHistoryListData(
                new List<PresentHistoryList>()
                {
                    new PresentHistoryList()
                    {
                        id = 1,
                        message_id = 2,
                        entity_type = EntityTypes.Wyrmite,
                        entity_id = 0,
                        message_param_value_1 = 1,
                        message_param_value_2 = 1,
                        message_param_value_3 = 1,
                        message_param_value_4 = 1,
                        entity_quantity = 1200,
                        create_time = DateTimeOffset.UtcNow
                    }
                }
            )
        );
    }
}
