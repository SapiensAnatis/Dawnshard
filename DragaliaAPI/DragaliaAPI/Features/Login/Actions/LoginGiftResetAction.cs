using DragaliaAPI.Database.Entities.Owned;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Web.Settings;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;

namespace DragaliaAPI.Features.Login.Actions;

internal sealed class LoginGiftResetAction(
    IPresentService presentService,
    SettingsService settingsService
) : IDailyResetAction
{
    public async Task Apply()
    {
        PlayerSettings settings = await settingsService.GetSettings(CancellationToken.None);

        if (!settings.DailyGifts)
        {
            return;
        }

        presentService.AddPresent(
            new Present.Present(
                PresentMessage.DragaliaLostTeamMessage,
                EntityTypes.Material,
                (int)Materials.ChampionsTestament,
                5
            )
        );

        presentService.AddPresent(
            new Present.Present(
                PresentMessage.DragaliaLostTeamMessage,
                EntityTypes.Material,
                (int)Materials.KnightsTestament,
                5
            )
        );

        presentService.AddPresent(
            new Present.Present(
                PresentMessage.DragaliaLostTeamMessage,
                EntityTypes.Material,
                (int)Materials.Omnicite
            )
        );

        presentService.AddPresent(
            new Present.Present(
                PresentMessage.DragaliaLostTeamMessage,
                EntityTypes.Material,
                (int)Materials.TwinklingSand,
                10
            )
        );

        presentService.AddPresent(
            new Materials[]
            {
                Materials.FlameTome,
                Materials.WindTome,
                Materials.WaterTome,
                Materials.ShadowTome,
                Materials.LightTome,
            }.Select(x => new Present.Present(
                PresentMessage.DragaliaLostTeamMessage,
                EntityTypes.Material,
                (int)x,
                5
            ))
        );
    }
}
