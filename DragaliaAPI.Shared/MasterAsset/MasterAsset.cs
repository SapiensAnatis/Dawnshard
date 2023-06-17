using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

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

    /// <summary>
    /// Contains information about ability crests in the shop.
    /// </summary>
    public static readonly MasterAssetData<int, AbilityCrestTrade> AbilityCrestTrade =
        new("AbilityCrestTrade.json", x => x.Id);

    /// <summary>
    /// Contains information about the materials required to unbind ability crests.
    /// </summary>
    public static readonly MasterAssetData<int, AbilityCrestBuildupGroup> AbilityCrestBuildupGroup =
        new("AbilityCrestBuildupGroup.json", x => x.Id);

    /// <summary>
    /// Contains information about the materials required to level up ability crests.
    /// </summary>
    public static readonly MasterAssetData<int, AbilityCrestBuildupLevel> AbilityCrestBuildupLevel =
        new("AbilityCrestBuildupLevel.json", x => x.Id);

    /// <summary>
    /// Contains information about the level limits of different ability crest rarities.
    /// </summary>
    public static readonly MasterAssetData<int, AbilityCrestRarity> AbilityCrestRarity =
        new("AbilityCrestRarity.json", x => x.Id);

    /// <summary>
    /// Contains information about ability crests.
    /// </summary>
    public static readonly MasterAssetData<AbilityCrests, AbilityCrest> AbilityCrest =
        new("AbilityCrest.json", x => x.Id);

    public static readonly MasterAssetData<int, QuestEventGroup> QuestEventGroup =
        new("QuestEventGroup.json", x => x.Id);

    public static readonly MasterAssetData<int, QuestEvent> QuestEvent =
        new("QuestEvent.json", x => x.Id);

    public static readonly MasterAssetData<int, AlbumMission> AlbumMission =
        new("Missions/MissionAlbumData.json", x => x.Id);

    public static readonly MasterAssetData<int, NormalMission> BeginnerMission =
        new("Missions/MissionBeginnerData.json", x => x.Id);

    public static readonly MasterAssetData<int, DailyMission> DailyMission =
        new("Missions/MissionDailyData.json", x => x.Id);

    public static readonly MasterAssetData<int, DrillMission> DrillMission =
        new("Missions/MissionDrillData.json", x => x.Id);

    public static readonly MasterAssetData<int, DrillMissionGroup> DrillMissionGroup =
        new("Missions/MissionDrillGroup.json", x => x.Id);

    public static readonly MasterAssetData<int, MainStoryMission> MainStoryMission =
        new("Missions/MissionMainStoryData.json", x => x.Id);

    public static readonly MasterAssetData<int, MainStoryMissionGroup> MainStoryMissionGroup =
        new("Missions/MissionMainStoryGroup.json", x => x.Id);

    public static readonly MasterAssetData<int, MemoryEventMission> MemoryEventMission =
        new("Missions/MissionMemoryEventData.json", x => x.Id);

    public static readonly MasterAssetData<int, NormalMission> NormalMission =
        new("Missions/MissionNormalData.json", x => x.Id);

    public static readonly MasterAssetData<int, PeriodMission> PeriodMission =
        new("Missions/MissionPeriodData.json", x => x.Id);

    public static readonly MasterAssetData<int, SpecialMission> SpecialMission =
        new("Missions/MissionSpecialData.json", x => x.Id);

    public static readonly MasterAssetData<int, SpecialMissionGroup> SpecialMissionGroup =
        new("Missions/MissionSpecialGroup.json", x => x.Id);

    public static readonly MasterAssetData<
        MissionProgressType,
        MissionProgressionInfo
    > MissionProgressionInfo = new("Missions/MissionProgressionInfo.json", x => x.Type);

    public static readonly MasterAssetData<
        int,
        MainStoryMissionGroupRewards
    > MainStoryMissionGroupRewards = new("Missions/MainStoryMissionGroupRewards.json", x => x.Id);
}
