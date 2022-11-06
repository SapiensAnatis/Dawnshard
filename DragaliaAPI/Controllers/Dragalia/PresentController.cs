using DragaliaAPI.Database;
using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("present")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class PresentController : ControllerBase
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
            new GetPresentListResponse(
                new GetPresentListData(
                    new(),
                    new()
                    {
                        new Present(
                            122414502,
                            1,
                            0,
                            0,
                            24,
                            0,
                            120,
                            0,
                            0,
                            0,
                            2010003,
                            100010101,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            DateTimeOffset.UtcNow,
                            DateTimeOffset.UtcNow.AddDays(7)
                        )
                    },
                    new(new PresentNotice(0, 1), new()),
                    new(new List<BaseNewEntity>(), new List<BaseNewEntity>())
                )
            )
        );
    }
}
