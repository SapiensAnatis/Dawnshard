using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Shared.MasterAsset;

/// <summary>
/// Provides access to instances of <see cref="MasterAssetData{TKey,TItem}"/> to retrieve internal game data.
/// </summary>
public static class MasterAsset
{
    /// <summary>
    /// Contains information about characters.
    /// </summary>
    public static readonly MasterAssetData<Charas, CharaData> CharaData =
        new("CharaData.json", x => x.Id);

    /// <summary>
    /// Contains information about dragons.
    /// </summary>
    public static readonly MasterAssetData<Dragons, DragonData> DragonData =
        new("DragonData.json", x => x.Id);

    /// <summary>
    /// Contains information about mana circle nodes.
    /// </summary>
    public static readonly MasterAssetData<int, ManaNode> ManaNode = new("MC.json", x => x.MC_0);

    /// <summary>
    /// Contains information about quests.
    /// </summary>
    public static readonly MasterAssetData<int, QuestData> QuestData =
        new("QuestData.json", x => x.Id);

    /// <summary>
    /// Contains information about Halidom buildings.
    /// </summary>
    /// <remarks>
    /// To generate keys, use <see cref="MasterAssetUtils.GetPlantDetailId"/>
    /// </remarks>
    public static readonly MasterAssetData<int, FortPlantDetail> FortPlant =
        new("FortPlantDetail.json", x => x.Id);

    /// <summary>
    /// Contains information about weapons.
    /// </summary>
    public static readonly MasterAssetData<WeaponBodies, WeaponBody> WeaponBody =
        new("WeaponBody.json", x => x.Id);

    /// <summary>
    /// Contains information about miscellaneous weapon upgrade steps, i.e. anything not passive abilities or stat upgrades.
    /// </summary>
    public static readonly MasterAssetData<int, WeaponBodyBuildupGroup> WeaponBodyBuildupGroup =
        new("WeaponBodyBuildupGroup.json", x => x.Id);

    /// <summary>
    /// Contains information about stat weapon upgrade steps.
    /// </summary>
    public static readonly MasterAssetData<int, WeaponBodyBuildupLevel> WeaponBodyBuildupLevel =
        new("WeaponBodyBuildupLevel.json", x => x.Id);

    /// <summary>
    /// Contains information about weapon passive ability unlocks.
    /// </summary>
    public static readonly MasterAssetData<int, WeaponPassiveAbility> WeaponPassiveAbility =
        new("WeaponPassiveAbility.json", x => x.Id);

    /// <summary>
    /// Contains information about rewards from quests.
    /// </summary>
    public static readonly MasterAssetData<int, QuestDropInfo> QuestDrops =
        new("QuestDrops.json", x => x.QuestId);

    /// <summary>
    /// Dragon StoryId Arrays indexed by DragonId
    /// </summary>
    public static MasterAssetData<int, StoryData> DragonStories =>
        new("DragonStories.json", x => x.id);

    /// <summary>
    /// Character StoryId Arrays indexed by CharaId
    /// </summary>
    public static MasterAssetData<int, StoryData> CharaStories =>
        new("CharaStories.json", x => x.id);

    public static MasterAssetData<int, UnitStory> UnitStory => new("UnitStory.json", x => x.Id);
}
