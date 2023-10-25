using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.MissionDesigner.Models;

public record ClearQuestMission : Mission
{
    public required int QuestId { get; init; }

    public override MissionCompleteType CompleteType => MissionCompleteType.QuestCleared;

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(this.Id, this.Type, this.MissionId, this.CompleteType, true, this.QuestId);
}
