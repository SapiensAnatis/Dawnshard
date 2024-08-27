using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Web.Savefile;

[Route("/api/savefile/edit")]
[Authorize(Policy = AuthConstants.PolicyNames.RequireDawnshardIdentity)]
internal sealed class SavefileEditController : ControllerBase
{
    [SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "Controller method must be non-static to be registered"
    )]
    [HttpGet("widgets/present")]
    public ActionResult<PresentWidgetData> GetPresentWidgetData() =>
        EditorWidgetsService.GetPresentWidgetData();
}
