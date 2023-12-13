namespace DragaliaAPI.MissionDesigner.Models;

public class ClearQuestMission : Mission
{
    public int? QuestId { get; init; }

    public int? QuestGroupId { get; init; }

    public QuestPlayModeTypes? PlayMode { get; init; }

    protected override MissionCompleteType CompleteType => MissionCompleteType.QuestCleared;

    protected override int? Parameter => this.QuestId;

    protected override int? Parameter2 => this.QuestGroupId;

    protected override int? Parameter3 => (int?)this.PlayMode;
}
