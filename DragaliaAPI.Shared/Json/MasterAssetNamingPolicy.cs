using System.Text.Json;

namespace DragaliaAPI.Shared.Json;

internal class MasterAssetNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return $"_{name}";
    }
}
