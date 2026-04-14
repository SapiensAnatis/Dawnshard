namespace DragaliaAPI.MissionDesigner.Models.EventMission;

/// <summary>
/// Mission completed by defeating a cumulative number of enemies in an event.
/// Example: "Defeat 1,000 Enemies in Invasions".
/// </summary>
public class EarnEnemiesKilledMission : Mission
{
    public int EventId { get; set; }

    protected override MissionCompleteType CompleteType => MissionCompleteType.EarnEnemiesKilled;

    protected override int? Parameter => this.EventId;
}
