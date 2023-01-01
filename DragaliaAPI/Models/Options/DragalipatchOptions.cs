using System.Text.Json.Serialization;

namespace DragaliaAPI.Models.Options;

/// <summary>
/// Options model for Dragalipatch config section.
/// </summary>
public class DragalipatchOptions
{
    [JsonRequired]
    public string Mode { get; set; } = string.Empty;

    public string? CdnUrl { get; set; }

    public string? ConeshellKey { get; set; }
}
