namespace DragaliaAPI.MissionDesigner.Models;

[AttributeUsage(AttributeTargets.Property)]
public class MissionListAttribute : Attribute
{
    public MissionType Type { get; set; }
}
