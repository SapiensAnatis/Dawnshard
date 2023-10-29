using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.MissionDesigner.Models;

public class ClearQuestMission : Mission
{
    public required int QuestId { get; init; }

    public required bool UseTotalValue { get; init; }

    public override MissionCompleteType CompleteType => MissionCompleteType.QuestCleared;

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(
            this.Id,
            this.Type,
            this.MissionId,
            this.CompleteType,
            this.UseTotalValue,
            this.QuestId
        );
}
