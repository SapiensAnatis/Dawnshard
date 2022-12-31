using System.Text.Json;

namespace DragaliaAPI.Shared.MasterAsset;

internal class MasterAssetNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return $"_{name}";
    }
}
