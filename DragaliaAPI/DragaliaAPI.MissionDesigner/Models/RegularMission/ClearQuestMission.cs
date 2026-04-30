namespace DragaliaAPI.MissionDesigner.Models.RegularMission;

/// <summary>
/// Mission completed by clearing a quest. Can be scoped to a specific quest via
/// <see cref="QuestId"/>, a quest group via <see cref="QuestGroupId"/>, or a play mode via
/// <see cref="PlayMode"/>. With no parameters set, any quest clear counts.
/// Example: "Clear a Quest", "Clear Three Extra Boss Battles".
/// </summary>
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
