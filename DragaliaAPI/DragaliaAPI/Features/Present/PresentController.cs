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
        PresentGetPresentListResponse data = new();

        if (request.IsLimit)
        {
            data.PresentLimitList = await this.presentControllerService.GetLimitPresentList(
                request.PresentId
            );
        }
        else
        {
            data.PresentList = await this.presentControllerService.GetPresentList(
                request.PresentId
            );
        }

        data.UpdateDataList = new()
        {
            PresentNotice = await this.presentService.GetPresentNotice()
        };

        return Ok(data);
    }

    [Route("receive")]
    [HttpPost]
    public async Task<DragaliaResult> Receive(
        [FromBody] PresentReceiveRequest request,
        CancellationToken cancellationToken
    )
    {
        (
            IEnumerable<long> receivedPresents,
            IEnumerable<long> notReceivedPresents,
            IEnumerable<long> deletedPresents
        ) = await presentControllerService.ReceivePresent(request.PresentIdList, request.IsLimit);

        EntityResult entityResult = this.rewardService.GetEntityResult();
        // Prevent double messages popping up about discarded entities
        entityResult.OverDiscardEntityList = Enumerable.Empty<AtgenBuildEventRewardEntityList>();

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        IEnumerable<PresentDetailList> presentList =
            await this.presentControllerService.GetPresentList(0);
        IEnumerable<PresentDetailList> limitPresentList =
            await this.presentControllerService.GetLimitPresentList(0);

        PresentReceiveResponse data = new PresentReceiveResponse()
        {
            ReceivePresentIdList = receivedPresents.Select(x => (ulong)x),
            NotReceivePresentIdList = notReceivedPresents.Select(x => (ulong)x),
            DeletePresentIdList = deletedPresents.Select(x => (ulong)x),
            LimitOverPresentIdList = Enumerable.Empty<ulong>(), // TODO
            PresentList = presentList,
            PresentLimitList = limitPresentList,
            UpdateDataList = updateDataList,
            EntityResult = entityResult,
            ConvertedEntityList = entityResult.ConvertedEntityList,
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
            request.PresentHistoryId
        );

        return Ok(new PresentGetHistoryListResponse() { PresentHistoryList = list, });
    }
}
