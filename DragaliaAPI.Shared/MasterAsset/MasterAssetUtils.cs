using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Shared.MasterAsset;

public static class MasterAssetUtils
{
    public static int GetPlantDetailId(FortPlants id, int level) =>
        int.Parse($"{(int)id}{level:00}");

    public static FortPlantDetail GetFortPlant(FortPlants id, int level) =>
        MasterAsset.FortPlant.Get(GetPlantDetailId(id, level));

    /// <summary>
    /// Make a material map's quantities negative for the purpose of subtracting rather than checking materials.
    /// </summary>
    /// <param name="map">The material map.</param>
    /// <returns>The inverted material map.</returns>
    public static Dictionary<Materials, int> Invert(this Dictionary<Materials, int> map) =>
        map.ToDictionary(x => x.Key, x => -x.Value);
}
