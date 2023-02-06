using System.Runtime.CompilerServices;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Shared.MasterAsset;

public static class MasterAsset
{
    private static readonly Lazy<MasterAssetData<Charas, CharaData>> _CharaData =
        new(() => new("CharaData.json", x => x.Id));
    public static MasterAssetData<Charas, CharaData> CharaData => _CharaData.Value;

    private static readonly Lazy<MasterAssetData<Dragons, DragonData>> _DragonData =
        new(() => new("DragonData.json", x => x.Id));
    public static MasterAssetData<Dragons, DragonData> DragonData => _DragonData.Value;

    private static readonly Lazy<MasterAssetData<int, ManaNode>> _ManaNode =
        new(() => new("MC.json", x => x.MC_0));
    public static MasterAssetData<int, ManaNode> ManaNode => _ManaNode.Value;

    private static readonly Lazy<MasterAssetData<int, QuestData>> _QuestData =
        new(() => new("QuestData.json", x => x.Id));
    public static MasterAssetData<int, QuestData> QuestData => _QuestData.Value;

    private static readonly Lazy<MasterAssetData<int, FortPlantDetail>> _FortPlant =
        new(() => new("FortPlantDetail.json", x => x.Id));
    public static MasterAssetData<int, FortPlantDetail> FortPlant => _FortPlant.Value;

    private static readonly Lazy<MasterAssetData<WeaponBodies, WeaponBody>> _WeaponBody =
        new(() => new("WeaponBody.json", x => x.Id));
    public static MasterAssetData<WeaponBodies, WeaponBody> WeaponBody => _WeaponBody.Value;

    private static readonly Lazy<
        MasterAssetData<int, WeaponBodyBuildupGroup>
    > _WeaponBodyBuildupGroup = new(() => new("WeaponBodyBuildupGroup.json", x => x.Id));
    public static MasterAssetData<int, WeaponBodyBuildupGroup> WeaponBodyBuildupGroup =>
        _WeaponBodyBuildupGroup.Value;

    private static readonly Lazy<
        MasterAssetData<int, WeaponBodyBuildupLevel>
    > _WeaponBodyBuildupLevel = new(() => new("WeaponBodyBuildupLevel.json", x => x.Id));
    public static MasterAssetData<int, WeaponBodyBuildupLevel> WeaponBodyBuildupLevel =>
        _WeaponBodyBuildupLevel.Value;

    private static readonly Lazy<MasterAssetData<int, WeaponPassiveAbility>> _WeaponPassiveAbility =
        new(() => new("WeaponPassiveAbility.json", x => x.Id));
    public static MasterAssetData<int, WeaponPassiveAbility> WeaponPassiveAbility =>
        _WeaponPassiveAbility.Value;

    private static readonly Lazy<
        Dictionary<int, List<WeaponPassiveAbility>>
    > _WeaponPassiveAbilityGroups =
        new(
            () =>
                WeaponPassiveAbility.Enumerable
                    .GroupBy(x => x.WeaponPassiveAbilityGroupId)
                    .ToDictionary(x => x.Key, x => x.ToList())
        );
    public static Dictionary<int, List<WeaponPassiveAbility>> WeaponPassiveAbilityGroups =>
        _WeaponPassiveAbilityGroups.Value;
}
