using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Shared.MasterAsset;

public static class MasterAssetUtils
{
    public static int GetPlantDetailId(FortPlants id, int level) =>
        int.Parse($"{(int)id}{level:00}");

    public static FortPlantDetail GetFortPlant(FortPlants id, int level) =>
        MasterAsset.FortPlant.Get(GetPlantDetailId(id, level));
}
