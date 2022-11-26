using DragaliaAPI.Models.Generated;
using MessagePack;

namespace DragaliaAPI.Models;

[MessagePackObject(true)]
public class UpdateDataList
{
    public UserData? user_data { get; set; }
    public DiamondData? diamond_data { get; set; }
    public PartyPowerData? party_power_data { get; set; }
    public IEnumerable<CharaList>? chara_list { get; set; }
    public IEnumerable<DragonList>? dragon_list { get; set; }
    public IEnumerable<WeaponList>? weapon_list { get; set; }
    public IEnumerable<AmuletList>? amulet_list { get; set; }
    public IEnumerable<WeaponSkinList>? weapon_skin_list { get; set; }
    public IEnumerable<WeaponBodyList>? weapon_body_list { get; set; }
    public IEnumerable<WeaponPassiveAbilityList>? weapon_passive_ability_list { get; set; }
    public IEnumerable<AbilityCrestList>? ability_crest_list { get; set; }
    public IEnumerable<AbilityCrestSetList>? ability_crest_set_list { get; set; }
    public IEnumerable<TalismanList>? talisman_list { get; set; }
    public IEnumerable<PartyList>? party_list { get; set; }
    public IEnumerable<MuseumList>? museum_list { get; set; }
    public IEnumerable<AlbumDragonData>? album_dragon_list { get; set; }
    public IEnumerable<AlbumWeaponList>? album_weapon_list { get; set; }
    public IEnumerable<EnemyBookList>? enemy_book_list { get; set; }
    public IEnumerable<ItemList>? item_list { get; set; }
    public IEnumerable<AstralItemList>? astral_item_list { get; set; }
    public IEnumerable<MaterialList>? material_list { get; set; }
    public IEnumerable<QuestList>? quest_list { get; set; }
    public IEnumerable<QuestEventList>? quest_event_list { get; set; }
    public IEnumerable<DragonGiftList>? dragon_gift_list { get; set; }
    public IEnumerable<DragonReliabilityList>? dragon_reliability_list { get; set; }
    public IEnumerable<UnitStoryList>? unit_story_list { get; set; }
    public IEnumerable<CastleStoryList>? castle_story_list { get; set; }
    public IEnumerable<QuestStoryList>? quest_story_list { get; set; }
    public IEnumerable<QuestTreasureList>? quest_treasure_list { get; set; }
    public IEnumerable<QuestWallList>? quest_wall_list { get; set; }
    public IEnumerable<QuestCarryList>? quest_carry_list { get; set; }
    public IEnumerable<QuestEntryConditionList>? quest_entry_condition_list { get; set; }
    public IEnumerable<SummonTicketList>? summon_ticket_list { get; set; }
    public IEnumerable<SummonPointList>? summon_point_list { get; set; }
    public IEnumerable<LotteryTicketList>? lottery_ticket_list { get; set; }
    public IEnumerable<ExchangeTicketList>? exchange_ticket_list { get; set; }
    public IEnumerable<GatherItemList>? gather_item_list { get; set; }
    public IEnumerable<BuildList>? build_list { get; set; }
    public IEnumerable<FortPlantList>? fort_plant_list { get; set; }
    public FortBonusList? fort_bonus_list { get; set; }
    public IEnumerable<CraftList>? craft_list { get; set; }
    public CurrentMainStoryMission? current_main_story_mission { get; set; }
    public IEnumerable<CharaUnitSetList>? chara_unit_set_list { get; set; }
    public UserGuildData? user_guild_data { get; set; }
    public GuildData? guild_data { get; set; }
    public IEnumerable<BattleRoyalCharaSkinList>? battle_royal_chara_skin_list { get; set; }
    public DmodeInfo? dmode_info { get; set; }
    public IEnumerable<DmodeStoryList>? dmode_story_list { get; set; }
    public PresentNotice? present_notice { get; set; }
    public FriendNotice? friend_notice { get; set; }
    public MissionNotice? mission_notice { get; set; }
    public GuildNotice? guild_notice { get; set; }
    public ShopNotice? shop_notice { get; set; }
    public AlbumPassiveNotice? album_passive_notice { get; set; }
    public IEnumerable<RaidEventUserList>? raid_event_user_list { get; set; }
    public IEnumerable<MazeEventUserList>? maze_event_user_list { get; set; }
    public IEnumerable<BuildEventUserList>? build_event_user_list { get; set; }
    public IEnumerable<CollectEventUserList>? collect_event_user_list { get; set; }
    public IEnumerable<Clb01EventUserList>? clb_01_event_user_list { get; set; }
    public IEnumerable<ExRushEventUserList>? ex_rush_event_user_list { get; set; }
    public IEnumerable<SimpleEventUserList>? simple_event_user_list { get; set; }
    public IEnumerable<ExHunterEventUserList>? ex_hunter_event_user_list { get; set; }
    public IEnumerable<CombatEventUserList>? combat_event_user_list { get; set; }
    public IEnumerable<BattleRoyalEventItemList>? battle_royal_event_item_list { get; set; }
    public IEnumerable<BattleRoyalEventUserRecord>? battle_royal_event_user_record { get; set; }
    public IEnumerable<BattleRoyalCycleUserRecord>? battle_royal_cycle_user_record { get; set; }
    public IEnumerable<EarnEventUserList>? earn_event_user_list { get; set; }
    public IEnumerable<EventPassiveList>? event_passive_list { get; set; }
    public IEnumerable<FunctionalMaintenanceList>? functional_maintenance_list { get; set; } =
        new List<FunctionalMaintenanceList>();
}
