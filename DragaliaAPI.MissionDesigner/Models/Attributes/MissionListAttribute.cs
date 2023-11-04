namespace DragaliaAPI.MissionDesigner.Models.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class MissionListAttribute : Attribute
{
    public MissionType Type { get; set; }
}
