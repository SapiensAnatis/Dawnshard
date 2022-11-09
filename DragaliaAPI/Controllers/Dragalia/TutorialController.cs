using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Requests;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("/tutorial")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public partial class TutorialController : DragaliaController
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IUpdateDataService updateDataService;

    public TutorialController(
        IUserDataRepository userDataRepository,
        IUpdateDataService updateDataService
    )
    {
        this.userDataRepository = userDataRepository;
        this.updateDataService = updateDataService;
    }

    [HttpPost]
    [Route("update_step")]
    public async Task<DragaliaResult> UpdateStep(TutorialUpdateStepRequest request)
    {
        await userDataRepository.UpdateTutorialStatus(this.DeviceAccountId, request.step);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.userDataRepository.SaveChangesAsync();

        TutorialUpdateStepResponse response =
            new(new TutorialUpdateStepData(request.step, updateDataList));

        return this.Ok(response);
    }

    [HttpPost]
    [Route("update_flags")]
    public async Task<DragaliaResult> UpdateFlags(UpdateFlagsRequest flagRequest)
    {
        int flag_id = flagRequest.flag_id;

        DbPlayerUserData userData = await this.userDataRepository.AddTutorialFlag(
            this.DeviceAccountId,
            flag_id
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.userDataRepository.SaveChangesAsync();

        TutorialUpdateFlagsResponse response =
            new(
                new TutorialUpdateFlagsData(
                    new() { flag_id },
                    updateDataList,
                    new(new List<BaseNewEntity>(), new List<BaseNewEntity>())
                )
            );

        return this.Ok(response);
    }
}
