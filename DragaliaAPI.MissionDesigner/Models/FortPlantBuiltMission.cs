namespace DragaliaAPI.MissionDesigner.Models;

public class FortPlantBuiltMission : Mission
{
    public FortPlants Plant { get; init; }

    public override MissionCompleteType CompleteType => MissionCompleteType.FortPlantBuilt;

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(this.Id, this.Type, this.MissionId, this.CompleteType, true, (int)this.Plant);
}
