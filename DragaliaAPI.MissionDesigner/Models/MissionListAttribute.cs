namespace DragaliaAPI.MissionDesigner.Models;

[AttributeUsage(AttributeTargets.Class)]
public class MissionListAttribute : Attribute
{
    public MissionType Type { get; set; }
}
