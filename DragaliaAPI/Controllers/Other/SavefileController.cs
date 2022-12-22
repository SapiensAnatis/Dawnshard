using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Other;

[Route("savefile")]
[NoSession]
[Authorize(AuthenticationSchemes = "DeveloperAuthentication")]
[ApiController]
public class SavefileController : ControllerBase
{
    private readonly ISavefileService savefileService;

    public SavefileController(ISavefileService savefileImportService)
    {
        this.savefileService = savefileImportService;
    }

    [HttpPost("import/{viewerId}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Import(
        long viewerId,
        [FromBody] DragaliaResponse<LoadIndexData> loadIndexResponse
    )
    {
        await this.savefileService.Import(viewerId, loadIndexResponse.data);

        return this.NoContent();
    }

    [HttpDelete("delete/{viewerId}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Delete(long viewerId)
    {
        await this.savefileService.Reset(viewerId);

        return this.NoContent();
    }
}
