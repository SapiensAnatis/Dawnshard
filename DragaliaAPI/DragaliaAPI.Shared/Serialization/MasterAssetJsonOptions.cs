using System.Text.Json;
using System.Text.Json.Serialization;

namespace DragaliaAPI.Shared.Serialization;

public class MasterAssetJsonOptions
{
    public static readonly JsonSerializerOptions Instance;

    static MasterAssetJsonOptions()
    {
        Instance = new();
        Instance.Converters.Add(new BoolIntJsonConverter());
        Instance.Converters.Add(new MasterAssetDateTimeOffsetConverter());
        Instance.Converters.Add(new JsonStringEnumConverter());
        Instance.PropertyNamingPolicy = new MasterAssetNamingPolicy();
    }
}
