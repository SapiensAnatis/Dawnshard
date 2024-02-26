using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class UpdateDataList
{
    [Key("chara_list")]
    public IEnumerable<CharaList> CharaList { get; set; }

    [Key("dragon_list")]
    public IEnumerable<DragonList> DragonList { get; set; }

    [Key("weapon_list")]
    public IEnumerable<WeaponList> WeaponList { get; set; }

    [Key("amulet_list")]
    public IEnumerable<AmuletList> AmuletList { get; set; }

    [Key("weapon_skin_list")]
    public IEnumerable<WeaponSkinList> WeaponSkinList { get; set; }

    [Key("weapon_body_list")]
    public IEnumerable<WeaponBodyList> WeaponBodyList { get; set; }

    [Key("weapon_passive_ability_list")]
    public IEnumerable<WeaponPassiveAbilityList> WeaponPassiveAbilityList { get; set; }

    [Key("ability_crest_list")]
    public IEnumerable<AbilityCrestList> AbilityCrestList { get; set; }

    [Key("ability_crest_set_list")]
    public IEnumerable<AbilityCrestSetList> AbilityCrestSetList { get; set; }

    [Key("talisman_list")]
    public IEnumerable<TalismanList> TalismanList { get; set; }

    [Key("museum_list")]
    public IEnumerable<MuseumList> MuseumList { get; set; }

    [Key("album_dragon_list")]
    public IEnumerable<AlbumDragonData> AlbumDragonList { get; set; }

    [Key("album_weapon_list")]
    public IEnumerable<AlbumWeaponList> AlbumWeaponList { get; set; }

    [Key("enemy_book_list")]
    public IEnumerable<EnemyBookList> EnemyBookList { get; set; }

    [Key("item_list")]
    public IEnumerable<ItemList> ItemList { get; set; }

    [Key("astral_item_list")]
    public IEnumerable<AstralItemList> AstralItemList { get; set; }

    [Key("material_list")]
    public IEnumerable<MaterialList> MaterialList { get; set; }

    [Key("quest_list")]
    public IEnumerable<QuestList> QuestList { get; set; }

    [Key("quest_event_list")]
    public IEnumerable<QuestEventList> QuestEventList { get; set; }

    [Key("dragon_gift_list")]
    public IEnumerable<DragonGiftList> DragonGiftList { get; set; }

    [Key("dragon_reliability_list")]
    public IEnumerable<DragonReliabilityList> DragonReliabilityList { get; set; }

    [Key("unit_story_list")]
    public IEnumerable<UnitStoryList> UnitStoryList { get; set; }

    [Key("castle_story_list")]
    public IEnumerable<CastleStoryList> CastleStoryList { get; set; }

    [Key("quest_story_list")]
    public IEnumerable<QuestStoryList> QuestStoryList { get; set; }

    [Key("quest_treasure_list")]
    public IEnumerable<QuestTreasureList> QuestTreasureList { get; set; }

    [Key("quest_wall_list")]
    public IEnumerable<QuestWallList> QuestWallList { get; set; }

    [Key("quest_carry_list")]
    public IEnumerable<QuestCarryList> QuestCarryList { get; set; }

    [Key("quest_entry_condition_list")]
    public IEnumerable<QuestEntryConditionList> QuestEntryConditionList { get; set; }

    [Key("summon_ticket_list")]
    public IEnumerable<SummonTicketList> SummonTicketList { get; set; }

    [Key("summon_point_list")]
    public IEnumerable<SummonPointList> SummonPointList { get; set; }

    [Key("lottery_ticket_list")]
    public IEnumerable<LotteryTicketList> LotteryTicketList { get; set; }

    [Key("exchange_ticket_list")]
    public IEnumerable<ExchangeTicketList> ExchangeTicketList { get; set; }

    [Key("gather_item_list")]
    public IEnumerable<GatherItemList> GatherItemList { get; set; }

    [Key("build_list")]
    public IEnumerable<BuildList> BuildList { get; set; }

    [Key("fort_plant_list")]
    public IEnumerable<FortPlantList> FortPlantList { get; set; }

    [Key("craft_list")]
    public IEnumerable<CraftList> CraftList { get; set; }

    [Key("chara_unit_set_list")]
    public IEnumerable<CharaUnitSetList> CharaUnitSetList { get; set; }

    [Key("battle_royal_chara_skin_list")]
    public IEnumerable<BattleRoyalCharaSkinList> BattleRoyalCharaSkinList { get; set; }

    [Key("dmode_story_list")]
    public IEnumerable<DmodeStoryList> DmodeStoryList { get; set; }

    [Key("raid_event_user_list")]
    public IEnumerable<RaidEventUserList> RaidEventUserList { get; set; }

    [Key("maze_event_user_list")]
    public IEnumerable<MazeEventUserList> MazeEventUserList { get; set; }

    [Key("build_event_user_list")]
    public IEnumerable<BuildEventUserList> BuildEventUserList { get; set; }

    [Key("collect_event_user_list")]
    public IEnumerable<CollectEventUserList> CollectEventUserList { get; set; }

    [Key("clb_01_event_user_list")]
    public IEnumerable<Clb01EventUserList> Clb01EventUserList { get; set; }

    [Key("ex_rush_event_user_list")]
    public IEnumerable<ExRushEventUserList> ExRushEventUserList { get; set; }

    [Key("simple_event_user_list")]
    public IEnumerable<SimpleEventUserList> SimpleEventUserList { get; set; }

    [Key("ex_hunter_event_user_list")]
    public IEnumerable<ExHunterEventUserList> ExHunterEventUserList { get; set; }

    [Key("combat_event_user_list")]
    public IEnumerable<CombatEventUserList> CombatEventUserList { get; set; }

    [Key("battle_royal_event_item_list")]
    public IEnumerable<BattleRoyalEventItemList> BattleRoyalEventItemList { get; set; }

    [Key("battle_royal_event_user_record")]
    public IEnumerable<BattleRoyalEventUserRecord> BattleRoyalEventUserRecord { get; set; }

    [Key("battle_royal_cycle_user_record")]
    public IEnumerable<BattleRoyalCycleUserRecord> BattleRoyalCycleUserRecord { get; set; }

    [Key("earn_event_user_list")]
    public IEnumerable<EarnEventUserList> EarnEventUserList { get; set; }

    [Key("event_passive_list")]
    public IEnumerable<EventPassiveList> EventPassiveList { get; set; }

    [Key("functional_maintenance_list")]
    public IEnumerable<FunctionalMaintenanceList> FunctionalMaintenanceList { get; set; }

    private IEnumerable<PartyList> partyList;

    [Key("party_list")]
    public IEnumerable<PartyList> PartyList
    {
        get => this.partyList;
        set
        {
            if (value is null)
                return;

            this.partyList = value.Select(x => new PartyList()
            {
                PartyName = x.PartyName,
                PartyNo = x.PartyNo,
                PartySettingList = x.PartySettingList.OrderBy(y => y.UnitNo)
            });
        }
    }
}
