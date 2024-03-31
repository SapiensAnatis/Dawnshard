using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Serialization;
using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class DragonList
{
    /// <summary>
    /// The time the dragon was obtained at.
    /// </summary>
    /// <remarks>
    /// <see cref="CustomSnakeCaseNamingPolicy"/> special-cases properties called
    /// "GetTime" into "gettime", which is true for all other types aside from this one, where it actually
    /// needs to be "get_time"
    /// </remarks>
    [Key("get_time")]
    [JsonPropertyName("get_time")]
    public DateTimeOffset GetTime { get; set; }
}
