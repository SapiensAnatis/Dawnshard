using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("/tutorial")]
public class TutorialController(
    ITutorialService tutorialService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    private readonly ITutorialService tutorialService = tutorialService;
    private readonly IUpdateDataService updateDataService = updateDataService;

    [HttpPost]
    [Route("update_step")]
    public async Task<DragaliaResult> UpdateStep(
        TutorialUpdateStepRequest request,
        CancellationToken cancellationToken
    )
    {
        int currentStep = await tutorialService.UpdateTutorialStatus(request.Step);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        return this.Ok(new TutorialUpdateStepResponse(currentStep, updateDataList, new()));
    }

    [HttpPost]
    [Route("update_flags")]
    public async Task<DragaliaResult> UpdateFlags(
        TutorialUpdateFlagsRequest flagRequest,
        CancellationToken cancellationToken
    )
    {
        List<int> currentFlags = await this.tutorialService.AddTutorialFlag(flagRequest.FlagId);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        return this.Ok(new TutorialUpdateFlagsResponse(currentFlags, updateDataList, new()));
    }
}
