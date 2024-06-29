using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Web.Savefile;

public class SavefileController(ILoadService loadService) : ControllerBase
{
    [HttpGet("export")]
    [Authorize(Policy = AuthConstants.PolicyNames.RequireDawnshardIdentity)]
    public async Task<FileResult> GetSavefile(CancellationToken cancellationToken)
    {
        LoadIndexResponse savefile = await loadService.BuildIndexData(cancellationToken);

        return this.File(
            JsonSerializer.SerializeToUtf8Bytes(savefile, ApiJsonOptions.Instance),
            "application/json",
            "savedata.txt"
        );
    }
}
