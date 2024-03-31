using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Shared.MasterAsset.Models.Wall;

namespace DragaliaAPI.Shared.MasterAsset;

public static class MasterAssetUtils
{
    public static int GetPlantDetailId(FortPlants id, int level) =>
        int.Parse($"{(int)id}{level:00}");

    public static FortPlantDetail GetFortPlant(FortPlants id, int level) =>
        MasterAsset.FortPlantDetail.Get(GetPlantDetailId(id, level));

    public static int GetQuestWallDetailId(int wallId, int wallLevel)
    {
        int element = (wallId - 216010000);
        return Convert.ToInt32($"10{element}{wallLevel:000}");
    }

    public static QuestWallDetail GetQuestWallDetail(int wallId, int wallLevel)
    {
        return MasterAsset.QuestWallDetail.Get(GetQuestWallDetailId(wallId, wallLevel));
    }

    /// <summary>
    /// Make a material map's quantities negative for the purpose of subtracting rather than checking materials.
    /// </summary>
    /// <param name="map">The material map.</param>
    /// <returns>The inverted material map.</returns>
    public static Dictionary<Materials, int> Invert(this IDictionary<Materials, int> map) =>
        map.ToDictionary(x => x.Key, x => -x.Value);

    public static FortPlantDetail GetInitialFortPlant(FortPlants id) =>
        MasterAsset
            .FortPlantDetail.Enumerable.Where(x => x.AssetGroup == id)
            .OrderBy(x => x.Level)
            .First();

    public static int GetMissionProgressionId(int missionId, MissionType type) =>
        int.Parse($"{missionId}{(int)type:00}");
}
