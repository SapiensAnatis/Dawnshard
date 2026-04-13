namespace DragaliaAPI.MissionDesigner.Models.RegularMission;

/// <summary>
/// Mission completed by performing an Item Summon.
/// Example: "Perform an Item Summon".
/// </summary>
public class ItemSummonMission : Mission
{
    protected override MissionCompleteType CompleteType => MissionCompleteType.ItemSummon;
}
