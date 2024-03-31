using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace DragaliaAPI.Shared.Serialization;

/// <summary>
/// Savefile options for Dragalia communication.
/// <remarks>Mainly for savefile import; most of the game uses msgpack instead.</remarks>
/// </summary>
public class ApiJsonOptions
{
    public static readonly JsonSerializerOptions Instance;

    public static readonly Action<JsonSerializerOptions> Action = options =>
    {
        options.Converters.Add(new DateTimeUnixJsonConverter());
        options.Converters.Add(new TimeSpanUnixJsonConverter());
        options.Converters.Add(new BoolIntJsonConverter());
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.PropertyNamingPolicy = CustomSnakeCaseNamingPolicy.Instance;
        options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        // GraphQL
        options.IncludeFields = true;
    };

    static ApiJsonOptions()
    {
        Instance = new();
        Action.Invoke(Instance);
    }
}
