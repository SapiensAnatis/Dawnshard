using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Shared.MasterAsset;

public static class MasterAssetUtils
{
    public static int GetPlantDetailId(FortPlants id, int level) =>
        int.Parse($"{(int)id}{level:00}");

    public static FortPlantDetail GetFortPlant(FortPlants id, int level) =>
        MasterAsset.FortPlant.Get(GetPlantDetailId(id, level));

    public static WeaponBodyBuildupLevel GetBuildupLevel(this WeaponBody body, int level) =>
        MasterAsset.WeaponBodyBuildupLevel.Get(int.Parse($"{body.Rarity}010{level:00}"));

    public static WeaponBodyBuildupGroup GetBuildupGroup(
        this WeaponBody body,
        BuildupPieceTypes type,
        int step
    ) =>
        MasterAsset.WeaponBodyBuildupGroup.Get(
            int.Parse($"{body.WeaponBodyBuildupGroupId}{(int)type:00}{step:00}")
        );

    public static WeaponPassiveAbility GetPassiveAbility(this WeaponBody body, int abilityNo) =>
        MasterAsset.WeaponPassiveAbility.Get(
            int.Parse($"{body.WeaponPassiveAbilityGroupId}{abilityNo:00}")
        );

    public static Dictionary<Materials, int> Invert(this Dictionary<Materials, int> map) =>
        map.ToDictionary(x => x.Key, x => -x.Value);
}
