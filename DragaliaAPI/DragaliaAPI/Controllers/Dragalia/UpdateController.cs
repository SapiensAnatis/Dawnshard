using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<DragaliaResult> Post(
        UpdateNamechangeRequest request,
        CancellationToken cancellationToken
    )
    {
        await userDataRepository.UpdateName(request.Name);

        await updateDataService.SaveChangesAsync(cancellationToken);

        return this.Ok(new UpdateNamechangeResponse(request.Name));
    }

    [HttpPost]
    [Route("reset_new")]
    public async Task<DragaliaResult> ResetNew(
        UpdateResetNewRequest request,
        CancellationToken cancellationToken
    )
    {
        foreach (AtgenTargetList target in request.TargetList)
        {
            logger.LogDebug("reset_new target: {@target}", target);
            target.TargetIdList ??= Enumerable.Empty<long>();

            switch (target.TargetName)
            {
                case "friend":
                case "friend_apply":
                case "stamp":
                    // TODO
                    logger.LogInformation(
                        "Unhandled type {resetType} in update/reset_new",
                        target.TargetName
                    );
                    break;
                case "emblem":
                    await foreach (
                        DbEmblem emblem in emblemRepository
                            .Emblems.Where(x => x.IsNew)
                            .AsAsyncEnumerable()
                            .WithCancellation(cancellationToken)
                    )
                    {
                        emblem.IsNew = false;
                    }

                    break;
                case "fort":
                    await fortService.ClearPlantNewStatuses(target.TargetIdList);
                    break;
                default:
                    throw new DragaliaException(
                        ResultCode.ModelUpdateTargetNotFound,
                        "Invalid target_name"
                    );
            }
        }

        await updateDataService.SaveChangesAsync(cancellationToken);

        return this.Ok(new UpdateResetNewResponse(1));
    }
}
