using System.Text.Json;

namespace DragaliaAPI.Shared.Json;

public class MasterAssetJsonOptions
{
    public static readonly JsonSerializerOptions Instance;

    static MasterAssetJsonOptions()
    {
        Instance = new();
        Instance.Converters.Add(new BoolIntJsonConverter());
        Instance.Converters.Add(new MasterAssetDateTimeOffsetConverter());
        Instance.PropertyNamingPolicy = new MasterAssetNamingPolicy();
    }
}
