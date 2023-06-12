using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Other;

[Route("savefile")]
[Consumes("application/json")]
[Authorize(AuthenticationSchemes = SchemeName.Developer)]
[ApiController]
public class SavefileController : ControllerBase
{
    private readonly ISavefileService savefileService;
    private readonly IUserDataRepository userDataRepository;

    public SavefileController(
        ISavefileService savefileImportService,
        IUserDataRepository userDataRepository
    )
    {
        this.savefileService = savefileImportService;
        this.userDataRepository = userDataRepository;
    }

    [HttpPost("import/{viewerId:long}")]
    public async Task<IActionResult> Import(
        long viewerId,
        [FromBody] DragaliaResponse<LoadIndexData> loadIndexResponse
    )
    {
        await this.savefileService.ThreadSafeImport(
            await this.LookupAccountId(viewerId),
            loadIndexResponse.data
        );

        return this.NoContent();
    }

    [HttpGet("export/{viewerId:long}")]
    public async Task<IActionResult> Export(long viewerId, [FromServices] ILoadService loadService)
    {
        string accountId = await LookupAccountId(viewerId);
        DragaliaResponse<LoadIndexData> result = new(await loadService.BuildIndexData(accountId));
        return Ok(result);
    }

    [HttpDelete("delete/{viewerId:long}")]
    public async Task<IActionResult> Delete(long viewerId)
    {
        await this.savefileService.Reset(await this.LookupAccountId(viewerId));

        return this.NoContent();
    }

    private async Task<string> LookupAccountId(long viewerId)
    {
        // Note that unlike in AuthService, a savefile must already exist here, hence no OrDefault
        return await this.userDataRepository
            .GetUserData(viewerId)
            .Select(x => x.DeviceAccountId)
            .SingleAsync();
    }
}
