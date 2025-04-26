using DragaliaAPI.Database.Entities.Owned;
using DragaliaAPI.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Web.Settings;

[ApiController]
[Route("/api/settings")]
[Authorize(Policy = AuthConstants.PolicyNames.RequireDawnshardIdentity)]
internal sealed class SettingsController(SettingsService settingsService) : ControllerBase
{
    [HttpPut]
    public async Task SetSettings(PlayerSettings settings, CancellationToken cancellationToken)
    {
        await settingsService.SetSettings(settings, cancellationToken);
    }
}
