using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Web.Savefile;

[Route("/api/savefile")]
[Authorize(Policy = AuthConstants.PolicyNames.RequireDawnshardIdentity)]
internal sealed class SavefileController(ILoadService loadService) : ControllerBase
{
    [HttpGet("export")]
    public async Task<FileResult> GetSavefile(CancellationToken cancellationToken)
    {
        DragaliaResponse<LoadIndexResponse> savefile =
            new(
                await loadService.BuildIndexData(cancellationToken),
                new DataHeaders(ResultCode.Success)
            );

        return this.File(
            JsonSerializer.SerializeToUtf8Bytes(savefile, ApiJsonOptions.Instance),
            "text/plain",
            "savedata.txt"
        );
    }
}
