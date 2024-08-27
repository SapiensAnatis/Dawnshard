using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Web.Savefile;

public class SavefileEditRequest
{
    public required List<PresentFormSubmission> Presents { get; init; }
}

public class PresentFormSubmission
{
    [JsonConverter(typeof(JsonStringEnumConverter<EntityTypes>))]
    public EntityTypes Type { get; init; }

    public int Item { get; init; }

    public int Quantity { get; init; }
}
