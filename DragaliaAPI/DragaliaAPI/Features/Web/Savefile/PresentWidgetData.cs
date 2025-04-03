using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Web.Savefile;

internal sealed class PresentWidgetData
{
    public required IReadOnlyList<EntityTypeInformation> Types { get; init; }

    public required IDictionary<
        EntityTypes,
        IReadOnlyList<EntityTypeItem>
    > AvailableItems { get; init; }
}

internal readonly struct EntityTypeInformation
{
    [JsonConverter(typeof(JsonStringEnumConverter<EntityTypes>))]
    public EntityTypes Type { get; init; }

    public int MaxQuantity { get; init; }
}

internal readonly struct EntityTypeItem
{
    public EntityTypeItem(int id)
    {
        this.Id = id;
    }

    public int Id { get; init; }
}
