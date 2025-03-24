using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Friends;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Configuration;

[Route("update")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
internal sealed class UpdateController(
    ILogger<UpdateController> logger,
    IUserDataRepository userDataRepository,
    IUpdateDataService updateDataService,
    IFortService fortService,
    IEmblemRepository emblemRepository,
    FriendService friendService
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
            IEnumerable<long> targetList = target.TargetIdList ?? [];
            logger.LogDebug("reset_new target: {@target}", target);

            switch (target.TargetName)
            {
                case "friend":
                {
                    await friendService.ResetNewFriends(targetList, cancellationToken);
                    break;
                }
                case "friend_apply":
                {
                    Debug.Assert(target.TargetIdList is null);

                    await friendService.ResetNewRequests(cancellationToken);
                    break;
                }
                case "stamp":
                {
                    // TODO
                    logger.LogInformation(
                        "Unhandled type {resetType} in update/reset_new",
                        target.TargetName
                    );
                    break;
                }
                case "emblem":
                {
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
                }
                case "fort":
                {
                    await fortService.ClearPlantNewStatuses(targetList);
                    break;
                }
                default:
                {
                    throw new DragaliaException(
                        ResultCode.ModelUpdateTargetNotFound,
                        "Invalid target_name"
                    );
                }
            }
        }

        await updateDataService.SaveChangesAsync(cancellationToken);

        return this.Ok(new UpdateResetNewResponse(1));
    }
}
