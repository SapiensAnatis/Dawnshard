using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Other;

[Route("savefile")]
[Consumes("application/json")]
[Authorize(AuthenticationSchemes = SchemeName.Developer)]
[ApiController]
[ApiJsonOutput]
public class SavefileController : ControllerBase
{
    private readonly ISavefileService savefileService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IPlayerIdentityService playerIdentityService;

    public SavefileController(
        ISavefileService savefileImportService,
        IUserDataRepository userDataRepository,
        IPlayerIdentityService playerIdentityService
    )
    {
        this.savefileService = savefileImportService;
        this.userDataRepository = userDataRepository;
        this.playerIdentityService = playerIdentityService;
    }

    [HttpPost("import/{viewerId:long}")]
    public async Task<IActionResult> Import(
        long viewerId,
        [FromBodyUsingApiJson] DragaliaResponse<LoadIndexResponse> loadIndexResponse
    )
    {
        string accountId = await LookupAccountId(viewerId);
        using IDisposable ctx = this.playerIdentityService.StartUserImpersonation(
            viewerId,
            accountId
        );

        await this.savefileService.ThreadSafeImport(loadIndexResponse.Data);

        return this.NoContent();
    }

    [HttpGet("export/{viewerId:long}")]
    public async Task<IActionResult> Export(long viewerId, [FromServices] ILoadService loadService)
    {
        string accountId = await LookupAccountId(viewerId);
        using IDisposable ctx = this.playerIdentityService.StartUserImpersonation(
            viewerId,
            accountId
        );

        DragaliaResponse<LoadIndexResponse> result = new(await loadService.BuildIndexData());
        return Ok(result);
    }

    private async Task<string> LookupAccountId(long viewerId)
    {
        // Note that unlike in AuthService, a savefile must already exist here, hence no OrDefault
        return await this
            .userDataRepository.GetViewerData(viewerId)
            .Select(x => x.Owner!.AccountId)
            .SingleAsync();
    }
}
