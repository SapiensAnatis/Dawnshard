using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.MissionDesigner.Models;

public record FortPlantLevelUpMission : Mission
{
    public required int RequiredHalidomLevel { get; init; }

    public required FortPlants FortPlant { get; init; }

    public override MissionCompleteType CompleteType => MissionCompleteType.FortPlantLevelUp;

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(this.Id, this.Type, this.MissionId, this.CompleteType, true, (int)this.FortPlant);
}
