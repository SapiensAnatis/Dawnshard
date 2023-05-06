using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("present")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class PresentController : DragaliaControllerBase
{
    private readonly IPresentService presentService;

    public PresentController(IPresentService presentService)
    {
        this.presentService = presentService;
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
        return Ok(await presentService.GetPresentList(request, DeviceAccountId, ViewerId));
    }

    [Route("receive")]
    [HttpPost]
    public async Task<DragaliaResult> Receive([FromBody] PresentReceiveRequest request)
    {
        return Ok(await presentService.ReceivePresent(request, DeviceAccountId, ViewerId));
    }

    [Route("get_history_list")]
    [HttpPost]
    public async Task<DragaliaResult> GetHistoryList(
        [FromBody] PresentGetHistoryListRequest request
    )
    {
        return Ok(await presentService.GetPresentHistoryList(request, DeviceAccountId, ViewerId));
    }
}
