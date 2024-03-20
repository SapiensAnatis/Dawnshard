using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Emblem;

[Route("emblem")]
public class EmblemController(
    IEmblemRepository emblemRepository,
    IUserDataRepository userDataRepository,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [HttpPost("get_list")]
    public async Task<DragaliaResult> GetList()
    {
        EmblemGetListResponse resp = new();

        resp.EmblemList = (await emblemRepository.GetEmblemsAsync()).Select(x => new EmblemList(
            x.EmblemId,
            x.IsNew,
            x.GetTime
        ));

        return Ok(resp);
    }

    [HttpPost("set")]
    public async Task<DragaliaResult> Set(
        EmblemSetRequest request,
        CancellationToken cancellationToken
    )
    {
        EmblemSetResponse resp = new();

        if (!await emblemRepository.HasEmblem(request.EmblemId))
        {
            throw new DragaliaException(ResultCode.CommonInvalidArgument, "Unowned emblem id");
        }

        (await userDataRepository.GetUserDataAsync()).EmblemId = request.EmblemId;

        await updateDataService.SaveChangesAsync(cancellationToken);

        resp.Result = 1;

        return Ok(resp);
    }
}
