using DragaliaAPI.Database;
using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Models.Generated;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("present")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class PresentController : DragaliaControllerBase
{
    private ApiContext _apiContext;

    public PresentController(ApiContext apiContext)
    {
        _apiContext = apiContext;
    }

    [MessagePackObject(true)]
    public record GetPresentListRequest(
        [property: MessagePackFormatter(typeof(BoolToIntFormatter))] bool is_limit,
        int present_id
    );

    [Route("get_present_list")]
    [HttpPost]
    public async Task<DragaliaResult> GetPresentList(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] GetPresentListRequest request
    )
    {
        int val = int.Parse(
            (await _apiContext.DeviceAccounts.FirstAsync(x => x.Id == "1")).HashedPassword
        );
        return Ok(
            new PresentGetPresentListData(
                new List<PresentDetailList>(),
                new List<PresentDetailList>(),
                new() { present_notice = new(1, 1) },
                new()
            )
        );
    }
}
