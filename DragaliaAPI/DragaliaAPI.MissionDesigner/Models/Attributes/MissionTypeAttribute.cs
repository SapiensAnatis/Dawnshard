namespace DragaliaAPI.MissionDesigner.Models.Attributes;

public sealed class MissionTypeAttribute(MissionType type) : ImplicitPropertyAttribute
{
    public MissionType Type { get; } = type;

    public override string Property => "Type";

    public override object Value => this.Type;
}
