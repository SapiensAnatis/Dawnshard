namespace DragaliaAPI.MissionDesigner.Models.EventMission;

/// <summary>
/// Mission completed by clearing any of the event's raid battles.
/// To target a specific raid difficulty, use a <see cref="RegularMission.ClearQuestMission"/> with
/// the relevant quest ID instead, as a raid and difficulty uniquely identify a single quest.
/// Example: "Clear a Raid Battle".
/// </summary>
public class ClearRaidMission : Mission
{
    protected override MissionCompleteType CompleteType => MissionCompleteType.EventRaidClear;

    public int EventId { get; set; }

    protected override int? Parameter => this.EventId;
}
