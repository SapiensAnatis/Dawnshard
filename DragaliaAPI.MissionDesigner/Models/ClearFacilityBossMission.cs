namespace DragaliaAPI.MissionDesigner.Models;

public class ClearFacilityBossMission : Mission
{
    public override MissionCompleteType CompleteType => MissionCompleteType.QuestCleared;

    public required int QuestGroupId { get; init; }
    
    public required string AreaName { get; init; }
    
    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(this.Id, this.Type, this.MissionId, this.CompleteType, false, null, this.QuestGroupId, null, )
}
