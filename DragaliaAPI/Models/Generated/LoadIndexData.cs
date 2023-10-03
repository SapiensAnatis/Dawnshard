using System.Text.Json.Serialization;
using MessagePack;

// ReSharper disable InconsistentNaming

namespace DragaliaAPI.Models.Generated;

[MessagePackObject(true)]
public class LoadIndexData
{
    [JsonPropertyName("origin")]
    public string Origin => "dawnshard";

    [JsonRequired]
    public required UserData user_data { get; set; }

    public PartyPowerData party_power_data { get; set; } = new();

    [JsonRequired]
    public IEnumerable<PartyList> party_list { get; set; } = Enumerable.Empty<PartyList>();

    public IEnumerable<CharaList> chara_list { get; set; } = Enumerable.Empty<CharaList>();

    public IEnumerable<DragonList> dragon_list { get; set; } = Enumerable.Empty<DragonList>();

    public IEnumerable<QuestList> quest_list { get; set; } = Enumerable.Empty<QuestList>();
    public IEnumerable<QuestEventList> quest_event_list { get; set; } =
        Enumerable.Empty<QuestEventList>();

    public IEnumerable<MaterialList> material_list { get; set; } = Enumerable.Empty<MaterialList>();
    public IEnumerable<AstralItemList> astral_item_list { get; set; } =
        Enumerable.Empty<AstralItemList>();

    [Obsolete("weapon_body_list is used post-2.0")]
    public IEnumerable<WeaponList> weapon_list { get; set; } = Enumerable.Empty<WeaponList>();
    public IEnumerable<AlbumWeaponList>? album_weapon_list { get; set; } =
        Enumerable.Empty<AlbumWeaponList>();

    [Obsolete("ability_crest_list is used post-2.0")]
    public IEnumerable<AmuletList> amulet_list { get; set; } = Enumerable.Empty<AmuletList>();
    public IEnumerable<WeaponSkinList> weapon_skin_list { get; set; } =
        Enumerable.Empty<WeaponSkinList>();

    public IEnumerable<WeaponBodyList> weapon_body_list { get; set; } =
        Enumerable.Empty<WeaponBodyList>();
    public IEnumerable<WeaponPassiveAbilityList> weapon_passive_ability_list { get; set; } =
        Enumerable.Empty<WeaponPassiveAbilityList>();

    public IEnumerable<AbilityCrestList> ability_crest_list { get; set; } =
        Enumerable.Empty<AbilityCrestList>();

    public IEnumerable<TalismanList> talisman_list { get; set; } = Enumerable.Empty<TalismanList>();

    public IEnumerable<DragonReliabilityList> dragon_reliability_list { get; set; } =
        Enumerable.Empty<DragonReliabilityList>();
    public IEnumerable<DragonGiftList> dragon_gift_list { get; set; } =
        Enumerable.Empty<DragonGiftList>();
    public IEnumerable<AlbumDragonData> album_dragon_list { get; set; } =
        Enumerable.Empty<AlbumDragonData>();
    public IEnumerable<EquipStampList> equip_stamp_list { get; set; } =
        Enumerable.Empty<EquipStampList>();
    public IEnumerable<UnitStoryList> unit_story_list { get; set; } =
        Enumerable.Empty<UnitStoryList>();

    public IEnumerable<CastleStoryList> castle_story_list { get; set; } =
        Enumerable.Empty<CastleStoryList>();

    public IEnumerable<QuestStoryList> quest_story_list { get; set; } =
        Enumerable.Empty<QuestStoryList>();
    public IEnumerable<QuestTreasureList> quest_treasure_list { get; set; } =
        Enumerable.Empty<QuestTreasureList>();
    public IEnumerable<QuestWallList> quest_wall_list { get; set; } =
        Enumerable.Empty<QuestWallList>();
    public IEnumerable<QuestCarryList> quest_carry_list { get; set; } =
        Enumerable.Empty<QuestCarryList>();
    public IEnumerable<QuestEntryConditionList> quest_entry_condition_list { get; set; } =
        Enumerable.Empty<QuestEntryConditionList>();

    public required FortBonusList fort_bonus_list { get; set; }

    public IEnumerable<BuildList> build_list { get; set; } = Enumerable.Empty<BuildList>();
    public IEnumerable<CraftList> craft_list { get; set; } = Enumerable.Empty<CraftList>();
    public IEnumerable<UserSummonList> user_summon_list { get; set; } =
        Enumerable.Empty<UserSummonList>();

    public IEnumerable<SummonTicketList> summon_ticket_list { get; set; } =
        Enumerable.Empty<SummonTicketList>();
    public IEnumerable<SummonPointList> summon_point_list { get; set; } =
        Enumerable.Empty<SummonPointList>();
    public IEnumerable<LotteryTicketList> lottery_ticket_list { get; set; } =
        Enumerable.Empty<LotteryTicketList>();
    public IEnumerable<ExchangeTicketList> exchange_ticket_list { get; set; } =
        Enumerable.Empty<ExchangeTicketList>();
    public IEnumerable<GatherItemList> gather_item_list { get; set; } =
        Enumerable.Empty<GatherItemList>();
    public IEnumerable<FortPlantList> fort_plant_list { get; set; } =
        Enumerable.Empty<FortPlantList>();

    [JsonIgnore] // This is sometimes [] which fucks up the parsing and it isn't even added yet
    public UserGuildData? user_guild_data { get; set; } = new();

    [JsonIgnore] // This is sometimes [] which fucks up the parsing and it isn't even added yet
    public GuildData? guild_data { get; set; } = new();

    public PresentNotice present_notice { get; set; } = new();

    public FriendNotice friend_notice { get; set; } = new();

    public MissionNotice mission_notice { get; set; } = new();

    public GuildNotice guild_notice { get; set; } = new();

    public ShopNotice shop_notice { get; set; } = new();

    public AlbumPassiveNotice album_passive_notice { get; set; } = new();
    public IEnumerable<FunctionalMaintenanceList> functional_maintenance_list { get; set; } =
        Enumerable.Empty<FunctionalMaintenanceList>();
    public IEnumerable<TreasureTradeList> treasure_trade_all_list { get; set; } =
        Enumerable.Empty<TreasureTradeList>();
    public IEnumerable<UserTreasureTradeList> user_treasure_trade_list { get; set; } =
        Enumerable.Empty<UserTreasureTradeList>();
    public IEnumerable<ShopPurchaseList> special_shop_purchase { get; set; } =
        Enumerable.Empty<ShopPurchaseList>();
    public int stamina_single_recover_second { get; set; }
    public int stamina_multi_user_max { get; set; }
    public int stamina_multi_system_max { get; set; }
    public int quest_skip_point_system_max { get; set; }
    public int quest_skip_point_use_limit_max { get; set; }
    public int spec_upgrade_time { get; set; }
    public DateTimeOffset server_time { get; set; } = DateTimeOffset.UnixEpoch;
    public int quest_bonus_stack_base_time { get; set; }
    public IEnumerable<AtgenQuestBonus> quest_bonus { get; set; } =
        Enumerable.Empty<AtgenQuestBonus>();

    [JsonIgnore]
    public AtgenMultiServer? multi_server { get; set; }

    public AtgenWalkerData? walker_data { get; set; } = new();

    public LoadIndexData(
        UserData user_data,
        PartyPowerData party_power_data,
        IEnumerable<PartyList> party_list,
        IEnumerable<CharaList> chara_list,
        IEnumerable<DragonList> dragon_list,
        IEnumerable<QuestList> quest_list,
        IEnumerable<QuestEventList> quest_event_list,
        IEnumerable<MaterialList> material_list,
        IEnumerable<AstralItemList> astral_item_list,
        IEnumerable<WeaponList> weapon_list,
        IEnumerable<AlbumWeaponList> album_weapon_list,
        IEnumerable<AmuletList> amulet_list,
        IEnumerable<WeaponSkinList> weapon_skin_list,
        IEnumerable<WeaponBodyList> weapon_body_list,
        IEnumerable<WeaponPassiveAbilityList> weapon_passive_ability_list,
        IEnumerable<AbilityCrestList> ability_crest_list,
        IEnumerable<TalismanList> talisman_list,
        IEnumerable<DragonReliabilityList> dragon_reliability_list,
        IEnumerable<DragonGiftList> dragon_gift_list,
        IEnumerable<AlbumDragonData> album_dragon_list,
        IEnumerable<EquipStampList> equip_stamp_list,
        IEnumerable<UnitStoryList> unit_story_list,
        IEnumerable<CastleStoryList> castle_story_list,
        IEnumerable<QuestStoryList> quest_story_list,
        IEnumerable<QuestTreasureList> quest_treasure_list,
        IEnumerable<QuestWallList> quest_wall_list,
        IEnumerable<QuestCarryList> quest_carry_list,
        IEnumerable<QuestEntryConditionList> quest_entry_condition_list,
        FortBonusList fort_bonus_list,
        IEnumerable<BuildList> build_list,
        IEnumerable<CraftList> craft_list,
        IEnumerable<UserSummonList> user_summon_list,
        IEnumerable<SummonTicketList> summon_ticket_list,
        IEnumerable<SummonPointList> summon_point_list,
        IEnumerable<LotteryTicketList> lottery_ticket_list,
        IEnumerable<ExchangeTicketList> exchange_ticket_list,
        IEnumerable<GatherItemList> gather_item_list,
        IEnumerable<FortPlantList> fort_plant_list,
        UserGuildData user_guild_data,
        GuildData guild_data,
        PresentNotice present_notice,
        FriendNotice friend_notice,
        MissionNotice mission_notice,
        GuildNotice guild_notice,
        ShopNotice shop_notice,
        AlbumPassiveNotice album_passive_notice,
        IEnumerable<FunctionalMaintenanceList> functional_maintenance_list,
        IEnumerable<TreasureTradeList> treasure_trade_all_list,
        IEnumerable<UserTreasureTradeList> user_treasure_trade_list,
        IEnumerable<ShopPurchaseList> special_shop_purchase,
        int stamina_single_recover_second,
        int stamina_multi_user_max,
        int stamina_multi_system_max,
        int quest_skip_point_system_max,
        int quest_skip_point_use_limit_max,
        int spec_upgrade_time,
        DateTimeOffset server_time,
        int quest_bonus_stack_base_time,
        IEnumerable<AtgenQuestBonus> quest_bonus,
        AtgenMultiServer multi_server,
        AtgenWalkerData walker_data
    )
    {
        this.user_data = user_data;
        this.party_power_data = party_power_data;
        this.party_list = party_list;
        this.chara_list = chara_list;
        this.dragon_list = dragon_list;
        this.quest_list = quest_list;
        this.quest_event_list = quest_event_list;
        this.material_list = material_list;
        this.astral_item_list = astral_item_list;
#pragma warning disable CS0618 // Type or member is obsolete
        this.weapon_list = weapon_list;
#pragma warning restore CS0618 // Type or member is obsolete
        this.album_weapon_list = album_weapon_list;
#pragma warning disable CS0618 // Type or member is obsolete
        this.amulet_list = amulet_list;
#pragma warning restore CS0618 // Type or member is obsolete
        this.weapon_skin_list = weapon_skin_list;
        this.weapon_body_list = weapon_body_list;
        this.weapon_passive_ability_list = weapon_passive_ability_list;
        this.ability_crest_list = ability_crest_list;
        this.talisman_list = talisman_list;
        this.dragon_reliability_list = dragon_reliability_list;
        this.dragon_gift_list = dragon_gift_list;
        this.album_dragon_list = album_dragon_list;
        this.equip_stamp_list = equip_stamp_list;
        this.unit_story_list = unit_story_list;
        this.castle_story_list = castle_story_list;
        this.quest_story_list = quest_story_list;
        this.quest_treasure_list = quest_treasure_list;
        this.quest_wall_list = quest_wall_list;
        this.quest_carry_list = quest_carry_list;
        this.quest_entry_condition_list = quest_entry_condition_list;
        this.fort_bonus_list = fort_bonus_list;
        this.build_list = build_list;
        this.craft_list = craft_list;
        this.user_summon_list = user_summon_list;
        this.summon_ticket_list = summon_ticket_list;
        this.summon_point_list = summon_point_list;
        this.lottery_ticket_list = lottery_ticket_list;
        this.exchange_ticket_list = exchange_ticket_list;
        this.gather_item_list = gather_item_list;
        this.fort_plant_list = fort_plant_list;
        this.user_guild_data = user_guild_data;
        this.guild_data = guild_data;
        this.present_notice = present_notice;
        this.friend_notice = friend_notice;
        this.mission_notice = mission_notice;
        this.guild_notice = guild_notice;
        this.shop_notice = shop_notice;
        this.album_passive_notice = album_passive_notice;
        this.functional_maintenance_list = functional_maintenance_list;
        this.treasure_trade_all_list = treasure_trade_all_list;
        this.user_treasure_trade_list = user_treasure_trade_list;
        this.special_shop_purchase = special_shop_purchase;
        this.stamina_single_recover_second = stamina_single_recover_second;
        this.stamina_multi_user_max = stamina_multi_user_max;
        this.stamina_multi_system_max = stamina_multi_system_max;
        this.quest_skip_point_system_max = quest_skip_point_system_max;
        this.quest_skip_point_use_limit_max = quest_skip_point_use_limit_max;
        this.spec_upgrade_time = spec_upgrade_time;
        this.server_time = server_time;
        this.quest_bonus_stack_base_time = quest_bonus_stack_base_time;
        this.quest_bonus = quest_bonus;
        this.multi_server = multi_server;
        this.walker_data = walker_data;
    }

    [JsonConstructor]
    public LoadIndexData() { }
}
