namespace DragaliaAPI.MissionDesigner.Models.Attributes;

public sealed class EventIdAttribute(int eventId) : ImplicitPropertyAttribute
{
    public int EventId { get; } = eventId;

    public override string Property => "EventId";

    public override object Value => this.EventId;
}
