using DragaliaAPI.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Services;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Features.Fort;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("update")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class UpdateController(
    ILogger<UpdateController> logger,
    IUserDataRepository userDataRepository,
    IUpdateDataService updateDataService,
    IFortService fortService,
    IEmblemRepository emblemRepository
) : DragaliaControllerBase
{
    [HttpPost]
    [Route("namechange")]
    public async Task<DragaliaResult> Post(UpdateNamechangeRequest request)
    {
        await userDataRepository.UpdateName(request.name);

        await updateDataService.SaveChangesAsync();

        return this.Ok(new UpdateNamechangeData(request.name));
    }

    [HttpPost]
    [Route("reset_new")]
    public async Task<DragaliaResult> ResetNew(UpdateResetNewRequest request)
    {
        foreach (AtgenTargetList target in request.target_list)
        {
            logger.LogDebug("reset_new target: {@target}", target);
            target.target_id_list ??= Enumerable.Empty<long>();

            switch (target.target_name)
            {
                case "friend":
                case "friend_apply":
                case "stamp":
                    // TODO
                    logger.LogInformation(
                        "Unhandled type {resetType} in update/reset_new",
                        target.target_name
                    );
                    break;
                case "emblem":
                    foreach (
                        DbEmblem emblem in await emblemRepository.Emblems
                            .Where(x => target.target_id_list.Contains((long)x.EmblemId))
                            .ToListAsync()
                    )
                    {
                        emblem.IsNew = false;
                    }

                    break;
                case "fort":
                    await fortService.ClearPlantNewStatuses(target.target_id_list);
                    break;
                default:
                    throw new DragaliaException(
                        ResultCode.ModelUpdateTargetNotFound,
                        "Invalid target_name"
                    );
            }
        }

        await updateDataService.SaveChangesAsync();

        return this.Ok(new UpdateResetNewData(1));
    }
}
