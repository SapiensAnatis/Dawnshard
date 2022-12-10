using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers;

[Route("savefile_import")]
[NoSession]
[Authorize(AuthenticationSchemes = "DeveloperAuthentication")]
[ApiController]
public class SavefileImportController : ControllerBase
{
    private readonly ISavefileImportService savefileImportService;

    public SavefileImportController(ISavefileImportService savefileImportService)
    {
        this.savefileImportService = savefileImportService;
    }

    [HttpPost("import/{viewerId}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Import(
        long viewerId,
        [FromBody] DragaliaResponse<LoadIndexData> loadIndexResponse
    )
    {
        await this.savefileImportService.Import(viewerId, loadIndexResponse.data);

        return this.NoContent();
    }
}
