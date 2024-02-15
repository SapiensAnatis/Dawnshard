using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("/tutorial")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public partial class TutorialController : DragaliaControllerBase
{
    private readonly ITutorialService tutorialService;
    private readonly IUpdateDataService updateDataService;

    public TutorialController(
        ITutorialService tutorialService,
        IUpdateDataService updateDataService
    )
    {
        this.tutorialService = tutorialService;
        this.updateDataService = updateDataService;
    }

    [HttpPost]
    [Route("update_step")]
    public async Task<DragaliaResult> UpdateStep(TutorialUpdateStepRequest request)
    {
        int currentStep = await tutorialService.UpdateTutorialStatus(request.step);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return this.Ok(new TutorialUpdateStepData(currentStep, updateDataList, new()));
    }

    [HttpPost]
    [Route("update_flags")]
    public async Task<DragaliaResult> UpdateFlags(TutorialUpdateFlagsRequest flagRequest)
    {
        List<int> currentFlags = await this.tutorialService.AddTutorialFlag(flagRequest.flag_id);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return this.Ok(new TutorialUpdateFlagsData(currentFlags, updateDataList, new()));
    }
}
