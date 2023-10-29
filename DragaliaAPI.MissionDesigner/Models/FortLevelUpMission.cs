using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.MissionDesigner.Models;

public class FortLevelUpMission : Mission
{
    public required int RequiredTotalHalidomLevel { get; init; }

    public override MissionCompleteType CompleteType => MissionCompleteType.FortLevelUp;

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(this.Id, this.Type, this.MissionId, MissionCompleteType.FortLevelUp, true);
}
