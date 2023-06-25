using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Services;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Features.Fort;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("update")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class UpdateController : DragaliaControllerBase
{
    private readonly ILogger<UpdateController> logger;
    private readonly IUserDataRepository userDataRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IFortService fortService;

    public UpdateController(
        ILogger<UpdateController> logger,
        IUserDataRepository userDataRepository,
        IUpdateDataService updateDataService,
        IFortService fortService
    )
    {
        this.logger = logger;
        this.userDataRepository = userDataRepository;
        this.updateDataService = updateDataService;
        this.fortService = fortService;
    }

    [HttpPost]
    [Route("namechange")]
    public async Task<DragaliaResult> Post(UpdateNamechangeRequest request)
    {
        await userDataRepository.UpdateName(request.name);
        await userDataRepository.SaveChangesAsync();

        return this.Ok(new UpdateNamechangeData(request.name));
    }

    [HttpPost]
    [Route("reset_new")]
    public async Task<DragaliaResult> ResetNew(UpdateResetNewRequest request)
    {
        foreach (AtgenTargetList target in request.target_list)
        {
            switch (target.target_name)
            {
                case "friend":
                case "friend_apply":
                case "stamp":
                case "emblem":
                    // TODO
                    logger.LogInformation(
                        "Unhandled type {resetType} in update/reset_new",
                        target.target_name
                    );
                    break;
                case "fort":
                    await this.fortService.ClearPlantNewStatuses(target.target_id_list);
                    break;
                default:
                    throw new DragaliaException(
                        ResultCode.ModelUpdateTargetNotFound,
                        "Invalid target_name"
                    );
                //case "fort" :
                //    fortService.RemoveNew(target.target_id_list);
                //    break;
            }
        }

        await updateDataService.SaveChangesAsync();

        return this.Ok(new UpdateResetNewData(1));
    }
}
