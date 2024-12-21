using DragaliaAPI.Features.Login.Savefile;
using DragaliaAPI.Infrastructure.Authentication;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Web.Savefile;

[ApiController]
[Route("/api/savefile")]
[Authorize(Policy = AuthConstants.PolicyNames.RequireDawnshardIdentity)]
internal sealed class SavefileController(ILoadService loadService) : ControllerBase
{
    [HttpGet("export")]
    public async Task<FileResult> GetSavefile(CancellationToken cancellationToken)
    {
        LoadIndexResponse loadIndexResponse = await loadService.BuildIndexData(cancellationToken);
        LoadIndexResponse sanitizedResponse = loadService.SanitizeIndexData(loadIndexResponse);

        DragaliaResponse<LoadIndexResponse> savefile = new(
            sanitizedResponse,
            new DataHeaders(ResultCode.Success)
        );

        return this.File(
            JsonSerializer.SerializeToUtf8Bytes(savefile, ApiJsonOptions.Instance),
            "text/plain",
            "savedata.txt"
        );
    }
}
