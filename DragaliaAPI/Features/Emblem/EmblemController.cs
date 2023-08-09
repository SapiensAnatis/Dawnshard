using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
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
        EmblemGetListData resp = new();

        resp.emblem_list = (await emblemRepository.GetEmblemsAsync()).Select(
            x => new EmblemList(x.EmblemId, x.IsNew, x.GetTime)
        );

        return Ok(resp);
    }

    [HttpPost("set")]
    public async Task<DragaliaResult> Set(EmblemSetRequest request)
    {
        EmblemSetData resp = new();

        if (!await emblemRepository.HasEmblem(request.emblem_id))
        {
            throw new DragaliaException(ResultCode.CommonInvalidArgument, "Unowned emblem id");
        }

        (await userDataRepository.GetUserDataAsync()).EmblemId = request.emblem_id;

        await updateDataService.SaveChangesAsync();

        resp.result = 1;

        return Ok(resp);
    }
}
