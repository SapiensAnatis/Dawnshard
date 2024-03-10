using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class UpdateDataList
{
    [Key("chara_list")]
    public List<CharaList>? CharaList { get; set; }

    [Key("dragon_list")]
    public List<DragonList>? DragonList { get; set; }

    [Key("weapon_list")]
    public List<WeaponList>? WeaponList { get; set; }

    [Key("amulet_list")]
    public List<AmuletList>? AmuletList { get; set; }

    [Key("weapon_skin_list")]
    public List<WeaponSkinList>? WeaponSkinList { get; set; }

    [Key("weapon_body_list")]
    public List<WeaponBodyList>? WeaponBodyList { get; set; }

    [Key("weapon_passive_ability_list")]
    public List<WeaponPassiveAbilityList>? WeaponPassiveAbilityList { get; set; }

    [Key("ability_crest_list")]
    public List<AbilityCrestList>? AbilityCrestList { get; set; }

    [Key("ability_crest_set_list")]
    public List<AbilityCrestSetList>? AbilityCrestSetList { get; set; }

    [Key("talisman_list")]
    public List<TalismanList>? TalismanList { get; set; }

    [Key("museum_list")]
    public List<MuseumList>? MuseumList { get; set; }

    [Key("album_dragon_list")]
    public List<AlbumDragonData>? AlbumDragonList { get; set; }

    [Key("album_weapon_list")]
    public List<AlbumWeaponList>? AlbumWeaponList { get; set; }

    [Key("enemy_book_list")]
    public List<EnemyBookList>? EnemyBookList { get; set; }

    [Key("item_list")]
    public List<ItemList>? ItemList { get; set; }

    [Key("astral_item_list")]
    public List<AstralItemList>? AstralItemList { get; set; }

    [Key("material_list")]
    public List<MaterialList>? MaterialList { get; set; }

    [Key("quest_list")]
    public List<QuestList>? QuestList { get; set; }

    [Key("quest_event_list")]
    public List<QuestEventList>? QuestEventList { get; set; }

    [Key("dragon_gift_list")]
    public List<DragonGiftList>? DragonGiftList { get; set; }

    [Key("dragon_reliability_list")]
    public List<DragonReliabilityList>? DragonReliabilityList { get; set; }

    [Key("unit_story_list")]
    public List<UnitStoryList>? UnitStoryList { get; set; }

    [Key("castle_story_list")]
    public List<CastleStoryList>? CastleStoryList { get; set; }

    [Key("quest_story_list")]
    public List<QuestStoryList>? QuestStoryList { get; set; }

    [Key("quest_treasure_list")]
    public List<QuestTreasureList>? QuestTreasureList { get; set; }

    [Key("quest_wall_list")]
    public List<QuestWallList>? QuestWallList { get; set; }

    [Key("quest_carry_list")]
    public List<QuestCarryList>? QuestCarryList { get; set; }

    [Key("quest_entry_condition_list")]
    public List<QuestEntryConditionList>? QuestEntryConditionList { get; set; }

    [Key("summon_ticket_list")]
    public List<SummonTicketList>? SummonTicketList { get; set; }

    [Key("summon_point_list")]
    public List<SummonPointList>? SummonPointList { get; set; }

    [Key("lottery_ticket_list")]
    public List<LotteryTicketList>? LotteryTicketList { get; set; }

    [Key("exchange_ticket_list")]
    public List<ExchangeTicketList>? ExchangeTicketList { get; set; }

    [Key("gather_item_list")]
    public List<GatherItemList>? GatherItemList { get; set; }

    [Key("build_list")]
    public List<BuildList>? BuildList { get; set; }

    [Key("fort_plant_list")]
    public List<FortPlantList>? FortPlantList { get; set; }

    [Key("craft_list")]
    public List<CraftList>? CraftList { get; set; }

    [Key("chara_unit_set_list")]
    public List<CharaUnitSetList>? CharaUnitSetList { get; set; }

    [Key("battle_royal_chara_skin_list")]
    public List<BattleRoyalCharaSkinList>? BattleRoyalCharaSkinList { get; set; }

    [Key("dmode_story_list")]
    public List<DmodeStoryList>? DmodeStoryList { get; set; }

    [Key("raid_event_user_list")]
    public List<RaidEventUserList>? RaidEventUserList { get; set; }

    [Key("maze_event_user_list")]
    public List<MazeEventUserList>? MazeEventUserList { get; set; }

    [Key("build_event_user_list")]
    public List<BuildEventUserList>? BuildEventUserList { get; set; }

    [Key("collect_event_user_list")]
    public List<CollectEventUserList>? CollectEventUserList { get; set; }

    [Key("clb_01_event_user_list")]
    public List<Clb01EventUserList>? Clb01EventUserList { get; set; }

    [Key("ex_rush_event_user_list")]
    public List<ExRushEventUserList>? ExRushEventUserList { get; set; }

    [Key("simple_event_user_list")]
    public List<SimpleEventUserList>? SimpleEventUserList { get; set; }

    [Key("ex_hunter_event_user_list")]
    public List<ExHunterEventUserList>? ExHunterEventUserList { get; set; }

    [Key("combat_event_user_list")]
    public List<CombatEventUserList>? CombatEventUserList { get; set; }

    [Key("battle_royal_event_item_list")]
    public List<BattleRoyalEventItemList>? BattleRoyalEventItemList { get; set; }

    [Key("battle_royal_event_user_record")]
    public List<BattleRoyalEventUserRecord>? BattleRoyalEventUserRecord { get; set; }

    [Key("battle_royal_cycle_user_record")]
    public List<BattleRoyalCycleUserRecord>? BattleRoyalCycleUserRecord { get; set; }

    [Key("earn_event_user_list")]
    public List<EarnEventUserList>? EarnEventUserList { get; set; }

    [Key("event_passive_list")]
    public List<EventPassiveList>? EventPassiveList { get; set; }

    [Key("functional_maintenance_list")]
    public List<FunctionalMaintenanceList>? FunctionalMaintenanceList { get; set; }

    private List<PartyList>? partyList;

    [Key("party_list")]
    public List<PartyList>? PartyList
    {
        get => this.partyList;
        set
        {
            if (value is null)
                return;

            this.partyList = value
                .Select(x => new PartyList()
                {
                    PartyName = x.PartyName,
                    PartyNo = x.PartyNo,
                    PartySettingList = x.PartySettingList.OrderBy(y => y.UnitNo)
                })
                .ToList();
        }
    }
}
