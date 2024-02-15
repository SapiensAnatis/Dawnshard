using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Present;

[Route("present")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class PresentController : DragaliaControllerBase
{
    private readonly IPresentControllerService presentControllerService;
    private readonly IPresentService presentService;
    private readonly IRewardService rewardService;
    private readonly IUpdateDataService updateDataService;

    public PresentController(
        IPresentControllerService presentControllerService,
        IPresentService presentService,
        IRewardService rewardService,
        IUpdateDataService updateDataService
    )
    {
        this.presentControllerService = presentControllerService;
        this.presentService = presentService;
        this.rewardService = rewardService;
        this.updateDataService = updateDataService;
    }

    /// <summary>
    /// Gets limited/non-limited presents
    /// </summary>
    /// <param name="request"><seealso cref="PresentGetPresentListRequest"/></param>
    /// <returns></returns>
    [Route("get_present_list")]
    [HttpPost]
    public async Task<DragaliaResult> GetPresentList(
        [FromBody] PresentGetPresentListRequest request
    )
    {
        PresentGetPresentListData data = new();

        if (request.is_limit)
        {
            data.present_limit_list = await this.presentControllerService.GetLimitPresentList(
                request.present_id
            );
        }
        else
        {
            data.present_list = await this.presentControllerService.GetPresentList(
                request.present_id
            );
        }

        data.update_data_list = new()
        {
            present_notice = await this.presentService.GetPresentNotice()
        };

        return Ok(data);
    }

    [Route("receive")]
    [HttpPost]
    public async Task<DragaliaResult> Receive([FromBody] PresentReceiveRequest request)
    {
        (
            IEnumerable<long> receivedPresents,
            IEnumerable<long> notReceivedPresents,
            IEnumerable<long> deletedPresents
        ) = await presentControllerService.ReceivePresent(
            request.present_id_list,
            request.is_limit
        );

        EntityResult entityResult = this.rewardService.GetEntityResult();
        // Prevent double messages popping up about discarded entities
        entityResult.over_discard_entity_list = Enumerable.Empty<AtgenBuildEventRewardEntityList>();

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        IEnumerable<PresentDetailList> presentList =
            await this.presentControllerService.GetPresentList(0);
        IEnumerable<PresentDetailList> limitPresentList =
            await this.presentControllerService.GetLimitPresentList(0);

        PresentReceiveData data = new PresentReceiveData()
        {
            receive_present_id_list = receivedPresents.Select(x => (ulong)x),
            not_receive_present_id_list = notReceivedPresents.Select(x => (ulong)x),
            delete_present_id_list = deletedPresents.Select(x => (ulong)x),
            limit_over_present_id_list = Enumerable.Empty<ulong>(), // TODO
            present_list = presentList,
            present_limit_list = limitPresentList,
            update_data_list = updateDataList,
            entity_result = entityResult,
            converted_entity_list = entityResult.converted_entity_list,
        };

        return Ok(data);
    }

    [Route("get_history_list")]
    [HttpPost]
    public async Task<DragaliaResult> GetHistoryList(
        [FromBody] PresentGetHistoryListRequest request
    )
    {
        IEnumerable<PresentHistoryList> list = await presentControllerService.GetPresentHistoryList(
            request.present_history_id
        );

        return Ok(new PresentGetHistoryListData() { present_history_list = list, });
    }
}
