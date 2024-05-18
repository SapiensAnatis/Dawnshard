using DragaliaAPI.Features.Present;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;

namespace DragaliaAPI.Features.Login.Actions;

public class LoginGiftResetAction : IDailyResetAction
{
    private readonly IPresentService presentService;

    public LoginGiftResetAction(IPresentService presentService)
    {
        this.presentService = presentService;
    }

    public Task Apply()
    {
        this.presentService.AddPresent(
            new Present.Present(
                PresentMessage.DragaliaLostTeamMessage,
                EntityTypes.Material,
                (int)Materials.ChampionsTestament,
                5
            )
        );

        this.presentService.AddPresent(
            new Present.Present(
                PresentMessage.DragaliaLostTeamMessage,
                EntityTypes.Material,
                (int)Materials.KnightsTestament,
                5
            )
        );

        this.presentService.AddPresent(
            new Present.Present(
                PresentMessage.DragaliaLostTeamMessage,
                EntityTypes.Material,
                (int)Materials.Omnicite
            )
        );

        this.presentService.AddPresent(
            new Present.Present(
                PresentMessage.DragaliaLostTeamMessage,
                EntityTypes.Material,
                (int)Materials.TwinklingSand,
                10
            )
        );

        this.presentService.AddPresent(
            new Materials[]
            {
                Materials.FlameTome,
                Materials.WindTome,
                Materials.WaterTome,
                Materials.ShadowTome,
                Materials.LightTome
            }.Select(x => new Present.Present(
                PresentMessage.DragaliaLostTeamMessage,
                EntityTypes.Material,
                (int)x,
                5
            ))
        );

        return Task.CompletedTask;
    }
}
