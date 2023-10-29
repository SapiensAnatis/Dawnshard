using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.MissionDesigner.Models;

public class CraftWeaponMission : Mission
{
    public required WeaponBodies WeaponBody { get; init; }

    public required UnitElement Element { get; init; }

    public required int Rarity { get; init; }

    public required WeaponSeries Series { get; init; }

    public override MissionCompleteType CompleteType => MissionCompleteType.WeaponEarned;

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(
            this.Id,
            this.Type,
            this.MissionId,
            this.CompleteType,
            true,
            (int)this.WeaponBody,
            (int)this.Element,
            this.Rarity,
            (int)this.Series
        );
}
