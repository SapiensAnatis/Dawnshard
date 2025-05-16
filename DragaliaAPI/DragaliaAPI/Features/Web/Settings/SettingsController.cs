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
    public async Task SetSettings(
        UpdateSettingsRequest updateSettings,
        CancellationToken cancellationToken
    )
    {
        PlayerSettings internalSettings = SettingsMapper.MapToDatabaseSettings(updateSettings);

        await settingsService.SetSettings(internalSettings, cancellationToken);
    }
}
