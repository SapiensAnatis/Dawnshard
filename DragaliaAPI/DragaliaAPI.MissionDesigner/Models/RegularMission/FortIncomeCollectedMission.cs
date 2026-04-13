namespace DragaliaAPI.MissionDesigner.Models.RegularMission;

/// <summary>
/// Mission completed by collecting a resource produced by a Halidom facility.
/// The <see cref="EntityType"/> specifies which resource to collect (e.g., Rupies, Mana).
/// Example: "Collect Rupies from a Facility".
/// </summary>
public class FortIncomeCollectedMission : Mission
{
    public required EntityTypes EntityType { get; init; }

    protected override MissionCompleteType CompleteType => MissionCompleteType.FortIncomeCollected;

    protected override int? Parameter => (int)this.EntityType;
}
