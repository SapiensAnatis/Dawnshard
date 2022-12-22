using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Database.Repositories;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("/tutorial")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public partial class TutorialController : DragaliaControllerBase
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

        return this.Ok(new TutorialUpdateStepData(request.step, updateDataList, new()));
    }

    [HttpPost]
    [Route("update_flags")]
    public async Task<DragaliaResult> UpdateFlags(TutorialUpdateFlagsRequest flagRequest)
    {
        int flag_id = flagRequest.flag_id;

        await this.userDataRepository.AddTutorialFlag(this.DeviceAccountId, flag_id);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.userDataRepository.SaveChangesAsync();

        return this.Ok(
            new TutorialUpdateFlagsData(new List<int>() { flag_id }, updateDataList, new())
        );
    }
}
