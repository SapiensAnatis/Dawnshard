namespace DragaliaAPI.MissionDesigner.Models.EventMission;

public class EventQuestClearWithCrestMission : Mission
{
    public override MissionCompleteType CompleteType =>
        MissionCompleteType.EventQuestClearWithCrest;

    public required int EventId { get; init; }

    public required AbilityCrests Crest { get; init; }

    public override MissionProgressionInfo ToMissionProgressionInfo() =>
        new(
            this.Id,
            this.Type,
            this.MissionId,
            this.CompleteType,
            false,
            this.EventId,
            (int)this.Crest
        );
}
