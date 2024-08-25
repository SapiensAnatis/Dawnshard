using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Tutorial;

[Route("/tutorial")]
public class TutorialController(
    ITutorialService tutorialService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [HttpPost]
    [Route("update_step")]
    public async Task<DragaliaResult> UpdateStep(
        TutorialUpdateStepRequest request,
        CancellationToken cancellationToken
    )
    {
        int currentStep = await tutorialService.UpdateTutorialStatus(request.Step);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return this.Ok(new TutorialUpdateStepResponse(currentStep, updateDataList, new()));
    }

    [HttpPost]
    [Route("update_flags")]
    public async Task<DragaliaResult> UpdateFlags(
        TutorialUpdateFlagsRequest flagRequest,
        CancellationToken cancellationToken
    )
    {
        List<int> currentFlags = await tutorialService.AddTutorialFlag(flagRequest.FlagId);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return this.Ok(new TutorialUpdateFlagsResponse(currentFlags, updateDataList, new()));
    }
}
