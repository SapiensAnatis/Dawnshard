using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Web.Savefile;

[ApiController]
[Route("/api/savefile/edit")]
[Authorize(Policy = AuthConstants.PolicyNames.RequireDawnshardIdentity)]
internal sealed class SavefileEditController(SavefileEditService savefileEditService)
    : ControllerBase
{
    [SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "Controller method must be non-static to be registered"
    )]
    [HttpGet("widgets/present")]
    public ActionResult<PresentWidgetData> GetPresentWidgetData() =>
        EditorWidgetsService.GetPresentWidgetData();

    [HttpPost]
    public async Task<ActionResult> DoEdit(SavefileEditRequest request)
    {
        if (!savefileEditService.ValidateEdits(request))
        {
            return this.BadRequest();
        }

        await savefileEditService.PerformEdits(request);

        return this.Ok();
    }
}
