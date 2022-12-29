using System.Text.Json.Serialization;

namespace DragaliaAPI.Models;

public class DragalipatchConfig
{
    [JsonPropertyName("mode")]
    public required string Mode { get; set; }

    [JsonPropertyName("cdnUrl")]
    public string? CdnUrl { get; set; }

    [JsonPropertyName("coneshellKey")]
    public string? ConeshellKey { get; set; }

    [JsonPropertyName("useUnifiedLogin")]
    public bool UseUnifiedLogin { get; set; }
}
