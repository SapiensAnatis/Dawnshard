using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Models.Generated;
using MessagePack;

namespace DragaliaAPI.Models;

[MessagePackObject(true)]
public class LoadIndexData
{
    public UserData? user_data { get; init; }
    public PartyPowerData? party_power_data { get; init; }
    public IEnumerable<PartyList>? party_list { get; init; }
    public IEnumerable<CharaList>? chara_list { get; init; }
    public IEnumerable<DragonList>? dragon_list { get; init; }
    public IEnumerable<QuestList>? quest_list { get; init; }
    public IEnumerable<QuestEventList>? quest_event_list { get; init; }
    public IEnumerable<MaterialList>? material_list { get; init; }
    public IEnumerable<AstralItemList>? astral_item_list { get; init; }
    public IEnumerable<WeaponList>? weapon_list { get; init; }
    public IEnumerable<AlbumWeaponList>? album_weapon_list { get; init; }
    public IEnumerable<AmuletList>? amulet_list { get; init; }
    public IEnumerable<WeaponSkinList>? weapon_skin_list { get; init; }
    public IEnumerable<WeaponBodyList>? weapon_body_list { get; init; }
    public IEnumerable<WeaponPassiveAbilityList>? weapon_passive_ability_list { get; init; }
    public IEnumerable<AbilityCrestList>? ability_crest_list { get; init; }
    public IEnumerable<TalismanList>? talisman_list { get; init; }
    public IEnumerable<DragonReliabilityList>? dragon_reliability_list { get; init; }
    public IEnumerable<DragonGiftList>? dragon_gift_list { get; init; }
    public IEnumerable<AlbumDragonData>? album_dragon_list { get; init; }
    public IEnumerable<EquipStampList>? equip_stamp_list { get; init; }
    public IEnumerable<UnitStoryList>? unit_story_list { get; init; }
    public IEnumerable<CastleStoryList>? castle_story_list { get; init; }
    public IEnumerable<QuestStoryList>? quest_story_list { get; init; }
    public IEnumerable<QuestTreasureList>? quest_treasure_list { get; init; }
    public IEnumerable<QuestWallList>? quest_wall_list { get; init; }
    public IEnumerable<QuestCarryList>? quest_carry_list { get; init; }
    public IEnumerable<QuestEntryConditionList>? quest_entry_condition_list { get; init; }
    public FortBonusList fort_bonus_list { get; init; }
    public IEnumerable<BuildList>? build_list { get; init; }
    public IEnumerable<CraftList>? craft_list { get; init; }
    public IEnumerable<UserSummonList>? user_summon_list { get; init; }
    public IEnumerable<SummonTicketList>? summon_ticket_list { get; init; }
    public IEnumerable<SummonPointList>? summon_point_list { get; init; }
    public IEnumerable<LotteryTicketList>? lottery_ticket_list { get; init; }
    public IEnumerable<ExchangeTicketList>? exchange_ticket_list { get; init; }
    public IEnumerable<GatherItemList>? gather_item_list { get; init; }
    public IEnumerable<FortPlantList>? fort_plant_list { get; init; }
    public UserGuildData? user_guild_data { get; init; }
    public GuildData? guild_data { get; init; }
    public PresentNotice? present_notice { get; init; }
    public FriendNotice? friend_notice { get; init; }
    public MissionNotice? mission_notice { get; init; }
    public GuildNotice? guild_notice { get; init; }
    public ShopNotice? shop_notice { get; init; }
    public AlbumPassiveNotice? album_passive_notice { get; init; }
    public IEnumerable<FunctionalMaintenanceList>? functional_maintenance_list { get; init; }
    public IEnumerable<TreasureTradeList>? treasure_trade_all_list { get; init; }
    public IEnumerable<UserTreasureTradeList>? user_treasure_trade_list { get; init; }
    public IEnumerable<ShopPurchaseList>? special_shop_purchase { get; init; }
    public int? stamina_single_recover_second { get; init; }
    public int? stamina_multi_user_max { get; init; }
    public int? stamina_multi_system_max { get; init; }
    public int? quest_skip_point_system_max { get; init; }
    public int? quest_skip_point_use_limit_max { get; init; }
    public int? spec_upgrade_time { get; init; }

    [MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
    public DateTimeOffset? server_time { get; init; }
    public int? quest_bonus_stack_base_time { get; init; }
    public IEnumerable<AtgenQuestBonus>? quest_bonus { get; init; }
    public AtgenMultiServer? multi_server { get; init; }
    public AtgenWalkerData? walker_data { get; init; }
}
