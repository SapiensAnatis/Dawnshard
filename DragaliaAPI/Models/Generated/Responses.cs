using MessagePack;

namespace DragaliaAPI.Models.Generated;

[MessagePackObject(true)]
public record AbilityCrestBuildupPieceData(
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record AbilityCrestBuildupPlusCountData(
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record AbilityCrestGetAbilityCrestSetListData(
    IEnumerable<AbilityCrestSetList> ability_crest_set_list
);

[MessagePackObject(true)]
public record AbilityCrestResetPlusCountData(
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record AbilityCrestSetAbilityCrestSetData(
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record AbilityCrestSetFavoriteData(
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record AbilityCrestTradeGetListData(
    IEnumerable<UserAbilityCrestTradeList> user_ability_crest_trade_list,
    IEnumerable<AbilityCrestTradeList> ability_crest_trade_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record AbilityCrestTradeTradeData(
    IEnumerable<UserAbilityCrestTradeList> user_ability_crest_trade_list,
    IEnumerable<AbilityCrestTradeList> ability_crest_trade_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record AbilityCrestUpdateAbilityCrestSetNameData(
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record AlbumIndexData(
    IEnumerable<AtgenAlbumQuestPlayRecordList> album_quest_play_record_list,
    IEnumerable<AlbumDragonData> album_dragon_list,
    AlbumPassiveNotice album_passive_update_result,
    IEnumerable<AtgenCharaHonorList> chara_honor_list,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record AmuletBuildupData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    DeleteDataList delete_data_list
);

[MessagePackObject(true)]
public record AmuletLimitBreakData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    DeleteDataList delete_data_list
);

[MessagePackObject(true)]
public record AmuletResetPlusCountData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record AmuletSellData(
    DeleteDataList delete_data_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record AmuletSetLockData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record AmuletTradeGetListData(
    IEnumerable<UserAmuletTradeList> user_amulet_trade_list,
    IEnumerable<AmuletTradeList> amulet_trade_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record AmuletTradeTradeData(
    IEnumerable<UserAmuletTradeList> user_amulet_trade_list,
    IEnumerable<AmuletTradeList> amulet_trade_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record BattleRoyalEventEntryData(
    BattleRoyalEventUserRecord battle_royal_event_user_record,
    BattleRoyalCycleUserRecord battle_royal_cycle_user_record,
    IEnumerable<BattleRoyalEventItemList> battle_royal_event_item_list,
    IEnumerable<EventCycleRewardList> event_cycle_reward_list,
    IEnumerable<BattleRoyalCharaSkinList> battle_royal_chara_skin_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record BattleRoyalEventGetEventDataData(
    BattleRoyalEventUserRecord battle_royal_event_user_record,
    BattleRoyalCycleUserRecord battle_royal_cycle_user_record,
    IEnumerable<BattleRoyalEventItemList> battle_royal_event_item_list,
    IEnumerable<EventCycleRewardList> event_cycle_reward_list,
    IEnumerable<BattleRoyalCharaSkinList> battle_royal_chara_skin_list,
    IEnumerable<EventTradeList> event_trade_list
);

[MessagePackObject(true)]
public record BattleRoyalEventReceiveEventCyclePointRewardData(
    IEnumerable<EventCycleRewardList> event_cycle_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenBuildEventRewardEntityList> event_cycle_reward_entity_list
);

[MessagePackObject(true)]
public record BattleRoyalEventReleaseCharaSkinData(
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record BattleRoyalFailData(int result);

[MessagePackObject(true)]
public record BattleRoyalGetBattleRoyalHistoryData(
    IEnumerable<AtgenBattleRoyalHistoryList> battle_royal_history_list
);

[MessagePackObject(true)]
public record BattleRoyalRecordRoyalRecordMultiData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    BattleRoyalResult battle_royal_result,
    IEnumerable<AtgenBuildEventRewardEntityList> event_cycle_reward_entity_list
);

[MessagePackObject(true)]
public record BattleRoyalStartMultiData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    AtgenBattleRoyalData battle_royal_data
);

[MessagePackObject(true)]
public record BattleRoyalStartRoyalMultiData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    AtgenBattleRoyalData battle_royal_data
);

[MessagePackObject(true)]
public record BuildEventEntryData(
    BuildEventUserList build_event_user_data,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    int is_receivable_event_daily_bonus
);

[MessagePackObject(true)]
public record BuildEventGetEventDataData(
    BuildEventUserList build_event_user_data,
    int is_receivable_event_daily_bonus,
    IEnumerable<BuildEventRewardList> build_event_reward_list,
    IEnumerable<EventTradeList> event_trade_list,
    AtgenEventFortData event_fort_data
);

[MessagePackObject(true)]
public record BuildEventReceiveBuildPointRewardData(
    IEnumerable<BuildEventRewardList> build_event_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenBuildEventRewardEntityList> build_event_reward_entity_list
);

[MessagePackObject(true)]
public record BuildEventReceiveDailyBonusData(
    IEnumerable<AtgenBuildEventRewardEntityList> event_daily_bonus_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record CartoonLatestData(AtgenLatest latest);

[MessagePackObject(true)]
public record CastleStoryReadData(
    IEnumerable<AtgenBuildEventRewardEntityList> castle_story_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list
);

[MessagePackObject(true)]
public record CharaAwakeData(UpdateDataList update_data_list);

[MessagePackObject(true)]
public record CharaBuildupManaData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record CharaBuildupPlatinumData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record CharaBuildupData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record CharaGetCharaUnitSetData(IEnumerable<CharaUnitSetList> chara_unit_set_list);

[MessagePackObject(true)]
public record CharaGetListData(IEnumerable<CharaList> chara_list);

[MessagePackObject(true)]
public record CharaLimitBreakAndBuildupManaData(
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record CharaLimitBreakData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record CharaResetPlusCountData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record CharaSetCharaUnitSetData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record CharaUnlockEditSkillData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record Clb01EventEntryData(
    Clb01EventUserList clb_01_event_user_data,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record Clb01EventGetEventDataData(
    Clb01EventUserList clb_01_event_user_data,
    IEnumerable<BuildEventRewardList> clb_01_event_reward_list,
    IEnumerable<CharaFriendshipList> chara_friendship_list
);

[MessagePackObject(true)]
public record Clb01EventReceiveClb01PointRewardData(
    IEnumerable<BuildEventRewardList> clb_01_event_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenBuildEventRewardEntityList> clb_01_event_reward_entity_list
);

[MessagePackObject(true)]
public record CollectEventEntryData(
    CollectEventUserList collect_event_user_data,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record CollectEventGetEventDataData(
    CollectEventUserList collect_event_user_data,
    IEnumerable<EventStoryList> event_story_list
);

[MessagePackObject(true)]
public record CombatEventEntryData(
    CombatEventUserList combat_event_user_data,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record CombatEventGetEventDataData(
    CombatEventUserList combat_event_user_data,
    IEnumerable<EventTradeList> event_trade_list,
    IEnumerable<BuildEventRewardList> event_reward_list,
    IEnumerable<UserEventLocationRewardList> user_event_location_reward_list
);

[MessagePackObject(true)]
public record CombatEventReceiveEventLocationRewardData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<UserEventLocationRewardList> user_event_location_reward_list,
    IEnumerable<AtgenBuildEventRewardEntityList> event_location_reward_entity_list
);

[MessagePackObject(true)]
public record CombatEventReceiveEventPointRewardData(
    IEnumerable<BuildEventRewardList> event_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenBuildEventRewardEntityList> event_reward_entity_list
);

[MessagePackObject(true)]
public record CraftAssembleData(
    UpdateDataList update_data_list,
    DeleteDataList delete_data_list,
    SettingSupport setting_support
);

[MessagePackObject(true)]
public record CraftCreateData(
    UpdateDataList update_data_list,
    DeleteDataList delete_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record CraftDisassembleData(
    UpdateDataList update_data_list,
    DeleteDataList delete_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record CraftResetNewData(UpdateDataList update_data_list);

[MessagePackObject(true)]
public record DeployGetDeployVersionData(string deploy_hash);

[MessagePackObject(true)]
public record DmodeBuildupServitorPassiveData(
    IEnumerable<DmodeServitorPassiveList> dmode_servitor_passive_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DmodeDungeonFinishData(
    int dmode_dungeon_state,
    DmodeIngameResult dmode_ingame_result,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DmodeDungeonFloorData(
    int dmode_dungeon_state,
    DmodeFloorData dmode_floor_data,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DmodeDungeonFloorSkipData(
    int dmode_dungeon_state,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DmodeDungeonRestartData(DmodeIngameData dmode_ingame_data, int dmode_dungeon_state);

[MessagePackObject(true)]
public record DmodeDungeonStartData(DmodeIngameData dmode_ingame_data, int dmode_dungeon_state);

[MessagePackObject(true)]
public record DmodeDungeonSystemHaltData(
    int dmode_dungeon_state,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DmodeDungeonUserHaltData(
    int dmode_dungeon_state,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DmodeEntryData(
    DmodeInfo dmode_info,
    IEnumerable<DmodeCharaList> dmode_chara_list,
    IEnumerable<DmodeServitorPassiveList> dmode_servitor_passive_list,
    DmodeDungeonInfo dmode_dungeon_info,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DmodeExpeditionFinishData(
    DmodeIngameResult dmode_ingame_result,
    DmodeExpedition dmode_expedition,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DmodeExpeditionForceFinishData(
    DmodeIngameResult dmode_ingame_result,
    DmodeExpedition dmode_expedition,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DmodeExpeditionStartData(
    DmodeExpedition dmode_expedition,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DmodeGetDataData(
    DmodeInfo dmode_info,
    IEnumerable<DmodeCharaList> dmode_chara_list,
    IEnumerable<DmodeServitorPassiveList> dmode_servitor_passive_list,
    DmodeDungeonInfo dmode_dungeon_info,
    IEnumerable<DmodeStoryList> dmode_story_list,
    DmodeExpedition dmode_expedition,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    int current_server_time
);

[MessagePackObject(true)]
public record DmodeReadStoryData(
    IEnumerable<AtgenBuildEventRewardEntityList> dmode_story_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list
);

[MessagePackObject(true)]
public record DragonBuildupData(
    UpdateDataList update_data_list,
    DeleteDataList delete_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DragonBuyGiftToSendMultipleData(
    IEnumerable<AtgenShopGiftList> shop_gift_list,
    IEnumerable<AtgenDragonGiftRewardList> dragon_gift_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    int dragon_contact_free_gift_count
);

[MessagePackObject(true)]
public record DragonBuyGiftToSendData(
    IEnumerable<AtgenShopGiftList> shop_gift_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    int is_favorite,
    IEnumerable<DragonRewardEntityList> return_gift_list,
    IEnumerable<RewardReliabilityList> reward_reliability_list,
    int dragon_contact_free_gift_count
);

[MessagePackObject(true)]
public record DragonGetContactDataData(IEnumerable<AtgenShopGiftList> shop_gift_list);

[MessagePackObject(true)]
public record DragonLimitBreakData(
    UpdateDataList update_data_list,
    DeleteDataList delete_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DragonResetPlusCountData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record DragonSellData(
    DeleteDataList delete_data_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record DragonSendGiftMultipleData(
    UpdateDataList update_data_list,
    int is_favorite,
    IEnumerable<DragonRewardEntityList> return_gift_list,
    IEnumerable<RewardReliabilityList> reward_reliability_list
);

[MessagePackObject(true)]
public record DragonSendGiftData(
    UpdateDataList update_data_list,
    int is_favorite,
    IEnumerable<DragonRewardEntityList> return_gift_list,
    IEnumerable<RewardReliabilityList> reward_reliability_list
);

[MessagePackObject(true)]
public record DragonSetLockData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record DreamAdventureClearData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    int result
);

[MessagePackObject(true)]
public record DreamAdventurePlayData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    int result
);

[MessagePackObject(true)]
public record DungeonFailData(
    int result,
    IEnumerable<UserSupportList> fail_helper_list,
    IEnumerable<AtgenHelperDetailList> fail_helper_detail_list,
    AtgenFailQuestDetail fail_quest_detail
);

[MessagePackObject(true)]
public record DungeonGetAreaOddsData(OddsInfo odds_info);

[MessagePackObject(true)]
public record DungeonReceiveQuestBonusData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    AtgenReceiveQuestBonus receive_quest_bonus
);

[MessagePackObject(true)]
public record DungeonRecordRecordMultiData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IngameResultData ingame_result_data,
    TimeAttackRankingData time_attack_ranking_data,
    EventDamageRanking event_damage_ranking
);

[MessagePackObject(true)]
public record DungeonRecordRecordData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IngameResultData ingame_result_data,
    TimeAttackRankingData time_attack_ranking_data,
    RepeatData repeat_data,
    EventDamageRanking event_damage_ranking
);

[MessagePackObject(true)]
public record DungeonRetryData(int continue_count, UpdateDataList update_data_list);

[MessagePackObject(true)]
public record DungeonSkipStartAssignUnitData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IngameResultData ingame_result_data,
    TimeAttackRankingData time_attack_ranking_data
);

[MessagePackObject(true)]
public record DungeonSkipStartMultipleQuestAssignUnitData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IngameResultData ingame_result_data,
    TimeAttackRankingData time_attack_ranking_data
);

[MessagePackObject(true)]
public record DungeonSkipStartMultipleQuestData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IngameResultData ingame_result_data,
    TimeAttackRankingData time_attack_ranking_data
);

[MessagePackObject(true)]
public record DungeonSkipStartData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IngameResultData ingame_result_data,
    TimeAttackRankingData time_attack_ranking_data
);

[MessagePackObject(true)]
public record DungeonStartStartAssignUnitData(
    IngameData ingame_data,
    IngameQuestData ingame_quest_data,
    OddsInfo odds_info,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record DungeonStartStartMultiAssignUnitData(
    IngameData ingame_data,
    IngameQuestData ingame_quest_data,
    OddsInfo odds_info,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record DungeonStartStartMultiData(
    IngameData ingame_data,
    IngameQuestData ingame_quest_data,
    OddsInfo odds_info,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record DungeonStartStartData(
    IngameData ingame_data,
    IngameQuestData ingame_quest_data,
    OddsInfo odds_info,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record EarnEventEntryData(
    EarnEventUserList earn_event_user_data,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record EarnEventGetEventDataData(
    EarnEventUserList earn_event_user_data,
    IEnumerable<EventTradeList> event_trade_list,
    IEnumerable<BuildEventRewardList> event_reward_list
);

[MessagePackObject(true)]
public record EarnEventReceiveEventPointRewardData(
    IEnumerable<BuildEventRewardList> event_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenBuildEventRewardEntityList> event_reward_entity_list
);

[MessagePackObject(true)]
public record EmblemGetListData(IEnumerable<EmblemList> emblem_list);

[MessagePackObject(true)]
public record EmblemSetData(int result);

[MessagePackObject(true)]
public record EulaAgreeAgreeData(EulaVersion version_hash, int is_optin);

[MessagePackObject(true)]
public record EulaGetVersionListData(IEnumerable<EulaVersion> version_hash_list);

[MessagePackObject(true)]
public record EulaGetVersionData(
    EulaVersion version_hash,
    bool is_required_agree,
    int agreement_status
);

[MessagePackObject(true)]
public record EventDamageGetTotalDamageHistoryData(
    IEnumerable<AtgenEventDamageHistoryList> event_damage_history_list
);

[MessagePackObject(true)]
public record EventDamageReceiveDamageRewardData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenEventDamageRewardList> event_damage_reward_list
);

[MessagePackObject(true)]
public record EventStoryReadData(
    IEnumerable<AtgenBuildEventRewardEntityList> event_story_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record EventSummonExecData(
    AtgenBoxSummonResult box_summon_result,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record EventSummonGetDataData(AtgenBoxSummonData box_summon_data);

[MessagePackObject(true)]
public record EventSummonResetData(AtgenBoxSummonData box_summon_data);

[MessagePackObject(true)]
public record EventTradeGetListData(
    IEnumerable<AtgenUserEventTradeList> user_event_trade_list,
    IEnumerable<EventTradeList> event_trade_list,
    IEnumerable<MaterialList> material_list,
    UserEventItemData user_event_item_data
);

[MessagePackObject(true)]
public record EventTradeTradeData(
    IEnumerable<AtgenUserEventTradeList> user_event_trade_list,
    IEnumerable<EventTradeList> event_trade_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<MaterialList> material_list,
    UserEventItemData user_event_item_data
);

[MessagePackObject(true)]
public record ExchangeGetUnitListData(IEnumerable<AtgenDuplicateEntityList> select_unit_list);

[MessagePackObject(true)]
public record ExchangeSelectUnitData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record ExHunterEventEntryData(
    ExHunterEventUserList ex_hunter_event_user_data,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record ExHunterEventGetEventDataData(
    ExHunterEventUserList ex_hunter_event_user_data,
    IEnumerable<BuildEventRewardList> ex_hunter_event_reward_list,
    IEnumerable<EventTradeList> event_trade_list,
    IEnumerable<EventPassiveList> event_passive_list
);

[MessagePackObject(true)]
public record ExHunterEventReceiveExHunterPointRewardData(
    IEnumerable<BuildEventRewardList> ex_hunter_event_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record ExRushEventEntryData(
    ExRushEventUserList ex_rush_event_user_data,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record ExRushEventGetEventDataData(
    ExRushEventUserList ex_rush_event_user_data,
    IEnumerable<CharaFriendshipList> chara_friendship_list,
    IEnumerable<EventTradeList> event_trade_list
);

[MessagePackObject(true)]
public record FortAddCarpenterData(
    int result,
    UpdateDataList update_data_list,
    FortDetail fort_detail
);

[MessagePackObject(true)]
public record FortBuildAtOnceData(
    int result,
    UpdateDataList update_data_list,
    ulong build_id,
    FortDetail fort_detail,
    FortBonusList fort_bonus_list,
    AtgenProductionRp production_rp,
    AtgenProductionRp production_df,
    AtgenProductionRp production_st
);

[MessagePackObject(true)]
public record FortBuildCancelData(
    int result,
    ulong build_id,
    FortDetail fort_detail,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record FortBuildEndData(
    int result,
    ulong build_id,
    FortBonusList fort_bonus_list,
    FortDetail fort_detail,
    AtgenProductionRp production_rp,
    AtgenProductionRp production_df,
    AtgenProductionRp production_st,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record FortBuildStartData(
    int result,
    ulong build_id,
    int build_start_date,
    int build_end_date,
    int remain_time,
    FortDetail fort_detail,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record FortGetDataData(
    FortDetail fort_detail,
    IEnumerable<BuildList> build_list,
    FortBonusList fort_bonus_list,
    AtgenProductionRp production_rp,
    AtgenProductionRp production_df,
    AtgenProductionRp production_st,
    int dragon_contact_free_gift_count,
    int current_server_time
);

[MessagePackObject(true)]
public record FortGetMultiIncomeData(
    int result,
    IEnumerable<AtgenHarvestBuildList> harvest_build_list,
    IEnumerable<AtgenAddCoinList> add_coin_list,
    IEnumerable<AtgenAddStaminaList> add_stamina_list,
    int is_over_coin,
    int is_over_material,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record FortLevelupAtOnceData(
    int result,
    ulong build_id,
    FortDetail fort_detail,
    FortBonusList fort_bonus_list,
    int current_fort_level,
    int current_fort_craft_level,
    AtgenProductionRp production_rp,
    AtgenProductionRp production_df,
    AtgenProductionRp production_st,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record FortLevelupCancelData(
    int result,
    ulong build_id,
    FortDetail fort_detail,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record FortLevelupEndData(
    int result,
    ulong build_id,
    FortDetail fort_detail,
    FortBonusList fort_bonus_list,
    int current_fort_level,
    int current_fort_craft_level,
    AtgenProductionRp production_rp,
    AtgenProductionRp production_df,
    AtgenProductionRp production_st,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record FortLevelupStartData(
    int result,
    int levelup_start_date,
    int levelup_end_date,
    int remain_time,
    ulong build_id,
    FortDetail fort_detail,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record FortMoveData(
    int result,
    ulong build_id,
    FortBonusList fort_bonus_list,
    AtgenProductionRp production_rp,
    AtgenProductionRp production_df,
    AtgenProductionRp production_st,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record FortSetNewFortPlantData(int result, UpdateDataList update_data_list);

[MessagePackObject(true)]
public record FriendAllReplyDenyData(
    int result,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record FriendApplyListData(
    int result,
    IEnumerable<UserSupportList> friend_apply,
    IEnumerable<ulong> new_apply_viewer_id_list
);

[MessagePackObject(true)]
public record FriendAutoSearchData(int result, IEnumerable<UserSupportList> search_list);

[MessagePackObject(true)]
public record FriendDeleteData(int result);

[MessagePackObject(true)]
public record FriendFriendIndexData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    int friend_count
);

[MessagePackObject(true)]
public record FriendFriendListData(
    int result,
    IEnumerable<UserSupportList> friend_list,
    IEnumerable<ulong> new_friend_viewer_id_list
);

[MessagePackObject(true)]
public record FriendGetSupportCharaDetailData(AtgenSupportUserDataDetail support_user_data_detail);

[MessagePackObject(true)]
public record FriendGetSupportCharaData(int result, SettingSupport setting_support);

[MessagePackObject(true)]
public record FriendIdSearchData(int result, UserSupportList search_user);

[MessagePackObject(true)]
public record FriendReplyData(
    int result,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record FriendRequestCancelData(int result);

[MessagePackObject(true)]
public record FriendRequestListData(int result, IEnumerable<UserSupportList> request_list);

[MessagePackObject(true)]
public record FriendRequestData(int result, UpdateDataList update_data_list);

[MessagePackObject(true)]
public record FriendSetSupportCharaData(
    int result,
    UpdateDataList update_data_list,
    SettingSupport setting_support
);

[MessagePackObject(true)]
public record GuildChatGetNewMessageListData(
    IEnumerable<GuildChatMessageList> guild_chat_message_list,
    int polling_interval
);

[MessagePackObject(true)]
public record GuildChatGetOldMessageListData(
    IEnumerable<GuildChatMessageList> guild_chat_message_list
);

[MessagePackObject(true)]
public record GuildChatPostMessageStampData(
    IEnumerable<GuildChatMessageList> guild_chat_message_list,
    int polling_interval
);

[MessagePackObject(true)]
public record GuildChatPostMessageTextData(
    IEnumerable<GuildChatMessageList> guild_chat_message_list,
    int polling_interval
);

[MessagePackObject(true)]
public record GuildChatPostReportData(int result);

[MessagePackObject(true)]
public record GuildDisbandData(UpdateDataList update_data_list);

[MessagePackObject(true)]
public record GuildDropMemberData(IEnumerable<GuildMemberList> guild_member_list);

[MessagePackObject(true)]
public record GuildEstablishData(
    UpdateDataList update_data_list,
    IEnumerable<GuildMemberList> guild_member_list
);

[MessagePackObject(true)]
public record GuildGetGuildApplyDataData(IEnumerable<GuildApplyList> guild_apply_list);

[MessagePackObject(true)]
public record GuildGetGuildMemberDataData(IEnumerable<GuildMemberList> guild_member_list);

[MessagePackObject(true)]
public record GuildIndexData(
    UpdateDataList update_data_list,
    IEnumerable<GuildMemberList> guild_member_list,
    IEnumerable<GuildChatMessageList> guild_chat_message_list,
    IEnumerable<GuildApplyList> guild_apply_list,
    int is_update_guild_position_type,
    IEnumerable<AtgenBuildEventRewardEntityList> attend_bonus_list,
    int polling_interval,
    int current_server_time,
    IEnumerable<GuildInviteSendList> guild_invite_send_list,
    int guild_invite_receive_count
);

[MessagePackObject(true)]
public record GuildInviteGetGuildInviteReceiveDataData(
    IEnumerable<GuildInviteReceiveList> guild_invite_receive_list
);

[MessagePackObject(true)]
public record GuildInviteGetGuildInviteSendDataData(
    IEnumerable<GuildInviteSendList> guild_invite_send_list
);

[MessagePackObject(true)]
public record GuildInviteInviteCancelData(IEnumerable<GuildInviteSendList> guild_invite_send_list);

[MessagePackObject(true)]
public record GuildInviteInviteReplyAllDenyData(
    UpdateDataList update_data_list,
    IEnumerable<GuildInviteReceiveList> guild_invite_receive_list
);

[MessagePackObject(true)]
public record GuildInviteInviteReplyData(
    UpdateDataList update_data_list,
    IEnumerable<GuildInviteReceiveList> guild_invite_receive_list
);

[MessagePackObject(true)]
public record GuildInviteInviteSendData(IEnumerable<GuildInviteSendList> guild_invite_send_list);

[MessagePackObject(true)]
public record GuildJoinReplyAllDenyData(IEnumerable<GuildApplyList> guild_apply_list);

[MessagePackObject(true)]
public record GuildJoinReplyData(
    IEnumerable<GuildMemberList> guild_member_list,
    IEnumerable<GuildApplyList> guild_apply_list
);

[MessagePackObject(true)]
public record GuildJoinRequestCancelData(UpdateDataList update_data_list);

[MessagePackObject(true)]
public record GuildJoinRequestData(UpdateDataList update_data_list);

[MessagePackObject(true)]
public record GuildJoinData(
    UpdateDataList update_data_list,
    IEnumerable<GuildMemberList> guild_member_list
);

[MessagePackObject(true)]
public record GuildPostReportData(int result);

[MessagePackObject(true)]
public record GuildResignData(UpdateDataList update_data_list);

[MessagePackObject(true)]
public record GuildSearchAutoSearchData(IEnumerable<GuildData> auto_search_guild_list);

[MessagePackObject(true)]
public record GuildSearchGetGuildDetailData(IEnumerable<GuildData> search_guild_list);

[MessagePackObject(true)]
public record GuildSearchIdSearchData(IEnumerable<GuildData> search_guild_list);

[MessagePackObject(true)]
public record GuildSearchNameSearchData(IEnumerable<GuildData> search_guild_list);

[MessagePackObject(true)]
public record GuildUpdateGuildPositionTypeData(IEnumerable<GuildMemberList> guild_member_list);

[MessagePackObject(true)]
public record GuildUpdateGuildSettingData(UpdateDataList update_data_list);

[MessagePackObject(true)]
public record GuildUpdateUserSettingData(UpdateDataList update_data_list);

[MessagePackObject(true)]
public record InquiryAggregationData(int result);

[MessagePackObject(true)]
public record InquiryDetailData(
    string opinion_id,
    int opinion_type,
    string opinion_text,
    IEnumerable<AtgenCommentList> comment_list,
    int occurred_at,
    int created_at
);

[MessagePackObject(true)]
public record InquiryReplyData;

[MessagePackObject(true)]
public record InquirySendData;

[MessagePackObject(true)]
public record InquiryTopData(
    IEnumerable<AtgenOpinionList> opinion_list,
    IEnumerable<AtgenOpinionTypeList> opinion_type_list,
    IEnumerable<AtgenInquiryFaqList> inquiry_faq_list
);

[MessagePackObject(true)]
public record ItemGetListData(IEnumerable<ItemList> item_list);

[MessagePackObject(true)]
public record ItemUseRecoveryStaminaData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    AtgenRecoverData recover_data
);

[MessagePackObject(true)]
public record LoginIndexData(
    IEnumerable<AtgenLoginBonusList> login_bonus_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    AtgenSupportReward support_reward,
    int dragon_contact_free_gift_count,
    IEnumerable<AtgenMonthlyWallReceiveList> monthly_wall_receive_list,
    IEnumerable<AtgenLoginLotteryRewardList> login_lottery_reward_list,
    AtgenPenaltyData penalty_data,
    IEnumerable<AtgenExchangeSummomPointList> exchange_summom_point_list,
    int before_exchange_summon_item_quantity,
    int server_time
);

[MessagePackObject(true)]
public record LoginPenaltyConfirmData(int result, AtgenPenaltyData penalty_data);

[MessagePackObject(true)]
public record LoginVerifyJwsData;

[MessagePackObject(true)]
public record LotteryGetOddsDataData(LotteryOddsRateList lottery_odds_rate_list);

[MessagePackObject(true)]
public record LotteryLotteryExecData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenLotteryResultList> lottery_result_list
);

[MessagePackObject(true)]
public record LotteryResultData(IEnumerable<AtgenLotteryResultList> lottery_result_list);

[MessagePackObject(true)]
public record MaintenanceGetTextData(string maintenance_text);

[MessagePackObject(true)]
public record MatchingCheckPenaltyUserData(int result);

[MessagePackObject(true)]
public record MatchingGetRoomListByGuildData(IEnumerable<RoomList> room_list);

[MessagePackObject(true)]
public record MatchingGetRoomListByLocationData(IEnumerable<RoomList> room_list);

[MessagePackObject(true)]
public record MatchingGetRoomListByQuestIdData(IEnumerable<RoomList> room_list);

[MessagePackObject(true)]
public record MatchingGetRoomListData(
    IEnumerable<RoomList> room_list,
    IEnumerable<RoomList> friend_room_list,
    IEnumerable<RoomList> event_room_list,
    IEnumerable<RoomList> event_friend_room_list
);

[MessagePackObject(true)]
public record MatchingGetRoomNameData(
    string room_name,
    int quest_id,
    string cluster_name,
    RoomList room_data,
    int is_friend
);

[MessagePackObject(true)]
public record MazeEventEntryData(MazeEventUserList maze_event_user_data);

[MessagePackObject(true)]
public record MazeEventGetEventDataData(
    MazeEventUserList maze_event_user_data,
    IEnumerable<BuildEventRewardList> maze_event_reward_list,
    IEnumerable<EventTradeList> event_trade_list
);

[MessagePackObject(true)]
public record MazeEventReceiveMazePointRewardData(
    IEnumerable<BuildEventRewardList> maze_event_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenBuildEventRewardEntityList> maze_event_reward_entity_list
);

[MessagePackObject(true)]
public record MemoryEventActivateData(
    int result,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record MissionGetDrillMissionListData(
    IEnumerable<DrillMissionList> drill_mission_list,
    IEnumerable<DrillMissionGroupList> drill_mission_group_list,
    MissionNotice mission_notice
);

[MessagePackObject(true)]
public record MissionGetMissionListData(
    IEnumerable<NormalMissionList> normal_mission_list,
    IEnumerable<DailyMissionList> daily_mission_list,
    IEnumerable<PeriodMissionList> period_mission_list,
    IEnumerable<BeginnerMissionList> beginner_mission_list,
    IEnumerable<SpecialMissionList> special_mission_list,
    IEnumerable<int> special_mission_purchased_group_id_list,
    IEnumerable<MainStoryMissionList> main_story_mission_list,
    CurrentMainStoryMission current_main_story_mission,
    IEnumerable<MemoryEventMissionList> memory_event_mission_list,
    IEnumerable<AlbumMissionList> album_mission_list,
    MissionNotice mission_notice
);

[MessagePackObject(true)]
public record MissionReceiveAlbumRewardData(
    IEnumerable<NormalMissionList> normal_mission_list,
    IEnumerable<DailyMissionList> daily_mission_list,
    IEnumerable<PeriodMissionList> period_mission_list,
    IEnumerable<BeginnerMissionList> beginner_mission_list,
    IEnumerable<SpecialMissionList> special_mission_list,
    IEnumerable<MainStoryMissionList> main_story_mission_list,
    IEnumerable<MemoryEventMissionList> memory_event_mission_list,
    IEnumerable<AlbumMissionList> album_mission_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<int> not_received_mission_id_list,
    IEnumerable<int> need_entry_event_id_list,
    IEnumerable<ConvertedEntityList> converted_entity_list
);

[MessagePackObject(true)]
public record MissionReceiveBeginnerRewardData(
    IEnumerable<NormalMissionList> normal_mission_list,
    IEnumerable<DailyMissionList> daily_mission_list,
    IEnumerable<PeriodMissionList> period_mission_list,
    IEnumerable<BeginnerMissionList> beginner_mission_list,
    IEnumerable<SpecialMissionList> special_mission_list,
    IEnumerable<MainStoryMissionList> main_story_mission_list,
    IEnumerable<MemoryEventMissionList> memory_event_mission_list,
    IEnumerable<AlbumMissionList> album_mission_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<int> not_received_mission_id_list,
    IEnumerable<int> need_entry_event_id_list,
    IEnumerable<ConvertedEntityList> converted_entity_list
);

[MessagePackObject(true)]
public record MissionReceiveDailyRewardData(
    IEnumerable<NormalMissionList> normal_mission_list,
    IEnumerable<DailyMissionList> daily_mission_list,
    IEnumerable<PeriodMissionList> period_mission_list,
    IEnumerable<BeginnerMissionList> beginner_mission_list,
    IEnumerable<SpecialMissionList> special_mission_list,
    IEnumerable<MainStoryMissionList> main_story_mission_list,
    IEnumerable<MemoryEventMissionList> memory_event_mission_list,
    IEnumerable<AlbumMissionList> album_mission_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenNotReceivedMissionIdListWithDayNo> not_received_mission_id_list_with_day_no,
    IEnumerable<int> need_entry_event_id_list,
    IEnumerable<ConvertedEntityList> converted_entity_list
);

[MessagePackObject(true)]
public record MissionReceiveDrillRewardData(
    IEnumerable<DrillMissionList> drill_mission_list,
    IEnumerable<DrillMissionGroupList> drill_mission_group_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<int> not_received_mission_id_list,
    IEnumerable<int> need_entry_event_id_list,
    IEnumerable<ConvertedEntityList> converted_entity_list,
    IEnumerable<AtgenBuildEventRewardEntityList> drill_mission_group_complete_reward_list
);

[MessagePackObject(true)]
public record MissionReceiveMainStoryRewardData(
    IEnumerable<NormalMissionList> normal_mission_list,
    IEnumerable<DailyMissionList> daily_mission_list,
    IEnumerable<PeriodMissionList> period_mission_list,
    IEnumerable<BeginnerMissionList> beginner_mission_list,
    IEnumerable<SpecialMissionList> special_mission_list,
    IEnumerable<MainStoryMissionList> main_story_mission_list,
    IEnumerable<MemoryEventMissionList> memory_event_mission_list,
    IEnumerable<AlbumMissionList> album_mission_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<int> not_received_mission_id_list,
    IEnumerable<int> need_entry_event_id_list,
    IEnumerable<ConvertedEntityList> converted_entity_list
);

[MessagePackObject(true)]
public record MissionReceiveMemoryEventRewardData(
    IEnumerable<NormalMissionList> normal_mission_list,
    IEnumerable<DailyMissionList> daily_mission_list,
    IEnumerable<PeriodMissionList> period_mission_list,
    IEnumerable<BeginnerMissionList> beginner_mission_list,
    IEnumerable<SpecialMissionList> special_mission_list,
    IEnumerable<MainStoryMissionList> main_story_mission_list,
    IEnumerable<MemoryEventMissionList> memory_event_mission_list,
    IEnumerable<AlbumMissionList> album_mission_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<int> not_received_mission_id_list,
    IEnumerable<int> need_entry_event_id_list,
    IEnumerable<ConvertedEntityList> converted_entity_list
);

[MessagePackObject(true)]
public record MissionReceiveNormalRewardData(
    IEnumerable<NormalMissionList> normal_mission_list,
    IEnumerable<DailyMissionList> daily_mission_list,
    IEnumerable<PeriodMissionList> period_mission_list,
    IEnumerable<BeginnerMissionList> beginner_mission_list,
    IEnumerable<SpecialMissionList> special_mission_list,
    IEnumerable<MainStoryMissionList> main_story_mission_list,
    IEnumerable<MemoryEventMissionList> memory_event_mission_list,
    IEnumerable<AlbumMissionList> album_mission_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<int> not_received_mission_id_list,
    IEnumerable<int> need_entry_event_id_list,
    IEnumerable<ConvertedEntityList> converted_entity_list
);

[MessagePackObject(true)]
public record MissionReceivePeriodRewardData(
    IEnumerable<NormalMissionList> normal_mission_list,
    IEnumerable<DailyMissionList> daily_mission_list,
    IEnumerable<PeriodMissionList> period_mission_list,
    IEnumerable<BeginnerMissionList> beginner_mission_list,
    IEnumerable<SpecialMissionList> special_mission_list,
    IEnumerable<MainStoryMissionList> main_story_mission_list,
    IEnumerable<MemoryEventMissionList> memory_event_mission_list,
    IEnumerable<AlbumMissionList> album_mission_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<int> not_received_mission_id_list,
    IEnumerable<int> need_entry_event_id_list,
    IEnumerable<ConvertedEntityList> converted_entity_list
);

[MessagePackObject(true)]
public record MissionReceiveSpecialRewardData(
    IEnumerable<NormalMissionList> normal_mission_list,
    IEnumerable<DailyMissionList> daily_mission_list,
    IEnumerable<PeriodMissionList> period_mission_list,
    IEnumerable<BeginnerMissionList> beginner_mission_list,
    IEnumerable<SpecialMissionList> special_mission_list,
    IEnumerable<MainStoryMissionList> main_story_mission_list,
    IEnumerable<MemoryEventMissionList> memory_event_mission_list,
    IEnumerable<AlbumMissionList> album_mission_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<int> not_received_mission_id_list,
    IEnumerable<int> need_entry_event_id_list,
    IEnumerable<ConvertedEntityList> converted_entity_list
);

[MessagePackObject(true)]
public record MissionUnlockDrillMissionGroupData(
    IEnumerable<DrillMissionList> drill_mission_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record MissionUnlockMainStoryGroupData(
    IEnumerable<MainStoryMissionList> main_story_mission_list,
    UpdateDataList update_data_list,
    IEnumerable<AtgenBuildEventRewardEntityList> main_story_mission_unlock_bonus_list
);

[MessagePackObject(true)]
public record MypageInfoData(
    int present_cnt,
    int friend_apply,
    bool friend,
    int achievement_cnt,
    UpdateDataList update_data_list,
    int is_receive_event_damage_reward,
    int is_view_start_dash,
    int is_view_dream_select,
    int is_shop_notification,
    RepeatData repeat_data,
    IEnumerable<UserSummonList> user_summon_list,
    IEnumerable<QuestEventScheduleList> quest_event_schedule_list,
    IEnumerable<QuestScheduleDetailList> quest_schedule_detail_list
);

[MessagePackObject(true)]
public record OptionGetOptionData(OptionData option_data);

[MessagePackObject(true)]
public record OptionSetOptionData(OptionData option_data);

[MessagePackObject(true)]
public record PartyIndexData(IEnumerable<BuildList> build_list);

[MessagePackObject(true)]
public record PartySetMainPartyNoData(int main_party_no);

[MessagePackObject(true)]
public record PartySetPartySettingData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record PartyUpdatePartyNameData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record PlatformAchievementGetPlatformAchievementListData(
    IEnumerable<AchievementList> achievement_list
);

[MessagePackObject(true)]
public record PresentGetHistoryListData(IEnumerable<PresentHistoryList> present_history_list);

[MessagePackObject(true)]
public record PresentGetPresentListData(
    IEnumerable<PresentDetailList> present_list,
    IEnumerable<PresentDetailList> present_limit_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record PresentReceiveData(
    IEnumerable<ulong> receive_present_id_list,
    IEnumerable<ulong> not_receive_present_id_list,
    IEnumerable<ulong> delete_present_id_list,
    IEnumerable<ulong> limit_over_present_id_list,
    IEnumerable<PresentDetailList> present_list,
    IEnumerable<PresentDetailList> present_limit_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<ConvertedEntityList> converted_entity_list
);

[MessagePackObject(true)]
public record PushNotificationUpdateSettingData(int result);

[MessagePackObject(true)]
public record QuestDropListData(AtgenQuestDropInfo quest_drop_info);

[MessagePackObject(true)]
public record QuestGetQuestClearPartyMultiData(
    IEnumerable<PartySettingList> quest_multi_clear_party_setting_list,
    IEnumerable<AtgenLostUnitList> lost_unit_list
);

[MessagePackObject(true)]
public record QuestGetQuestClearPartyData(
    IEnumerable<PartySettingList> quest_clear_party_setting_list,
    IEnumerable<AtgenLostUnitList> lost_unit_list
);

[MessagePackObject(true)]
public record QuestGetSupportUserListData(
    IEnumerable<UserSupportList> support_user_list,
    IEnumerable<AtgenSupportUserDetailList> support_user_detail_list
);

[MessagePackObject(true)]
public record QuestOpenTreasureData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenBuildEventRewardEntityList> quest_treasure_reward_list,
    IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list,
    int add_max_dragon_quantity,
    int add_max_weapon_quantity,
    int add_max_amulet_quantity
);

[MessagePackObject(true)]
public record QuestReadStoryData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenQuestStoryRewardList> quest_story_reward_list,
    IEnumerable<ConvertedEntityList> converted_entity_list
);

[MessagePackObject(true)]
public record QuestSearchQuestClearPartyCharaMultiData(
    IEnumerable<SearchClearPartyCharaList> search_quest_clear_party_chara_list
);

[MessagePackObject(true)]
public record QuestSearchQuestClearPartyCharaData(
    IEnumerable<SearchClearPartyCharaList> search_quest_clear_party_chara_list
);

[MessagePackObject(true)]
public record QuestSearchQuestClearPartyMultiData(
    IEnumerable<SearchClearPartyList> search_quest_clear_party_list
);

[MessagePackObject(true)]
public record QuestSearchQuestClearPartyData(
    IEnumerable<SearchClearPartyList> search_quest_clear_party_list
);

[MessagePackObject(true)]
public record QuestSetQuestClearPartyMultiData(int result);

[MessagePackObject(true)]
public record QuestSetQuestClearPartyData(int result);

[MessagePackObject(true)]
public record RaidEventEntryData(
    RaidEventUserList raid_event_user_data,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record RaidEventGetEventDataData(
    RaidEventUserList raid_event_user_data,
    IEnumerable<RaidEventRewardList> raid_event_reward_list,
    IEnumerable<CharaFriendshipList> chara_friendship_list,
    IEnumerable<EventTradeList> event_trade_list,
    IEnumerable<EventPassiveList> event_passive_list,
    IEnumerable<EventAbilityCharaList> event_ability_chara_list,
    int is_receive_event_damage_reward,
    AtgenEventDamageData event_damage_data
);

[MessagePackObject(true)]
public record RaidEventReceiveRaidPointRewardData(
    IEnumerable<RaidEventRewardList> raid_event_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record RedoableSummonFixExecData(
    UserRedoableSummonData user_redoable_summon_data,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record RedoableSummonGetDataData(
    UserRedoableSummonData? user_redoable_summon_data,
    RedoableSummonOddsRateList redoable_summon_odds_rate_list
);

[MessagePackObject(true)]
public record RedoableSummonPreExecData(UserRedoableSummonData user_redoable_summon_data);

[MessagePackObject(true)]
public record RepeatEndData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IngameResultData ingame_result_data,
    RepeatData repeat_data
);

[MessagePackObject(true)]
public record ShopChargeCancelData(
    int is_quest_bonus,
    int is_stone_bonus,
    int is_stamina_bonus,
    IEnumerable<ShopPurchaseList> material_shop_purchase,
    IEnumerable<ShopPurchaseList> normal_shop_purchase,
    IEnumerable<ShopPurchaseList> special_shop_purchase,
    IEnumerable<AtgenStoneBonus> stone_bonus,
    IEnumerable<AtgenStaminaBonus> stamina_bonus,
    IEnumerable<AtgenQuestBonus> quest_bonus,
    IEnumerable<AtgenProductLockList> product_lock_list,
    IEnumerable<ProductList> product_list,
    UpdateDataList update_data_list,
    AtgenUserItemSummon user_item_summon,
    int infancy_paid_diamond_limit
);

[MessagePackObject(true)]
public record ShopGetBonusData(
    int is_quest_bonus,
    int is_stone_bonus,
    int is_stamina_bonus,
    IEnumerable<AtgenStoneBonus> stone_bonus,
    IEnumerable<AtgenStaminaBonus> stamina_bonus,
    IEnumerable<AtgenQuestBonus> quest_bonus,
    IEnumerable<AtgenBuildEventRewardEntityList> stone_bonus_entity_list,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record ShopGetDreamSelectUnitListData(
    IEnumerable<AtgenDuplicateEntityList> dream_select_unit_list
);

[MessagePackObject(true)]
public record ShopGetListData(
    int is_quest_bonus,
    int is_stone_bonus,
    int is_stamina_bonus,
    IEnumerable<ShopPurchaseList> material_shop_purchase,
    IEnumerable<ShopPurchaseList> normal_shop_purchase,
    IEnumerable<ShopPurchaseList> special_shop_purchase,
    IEnumerable<AtgenStoneBonus> stone_bonus,
    IEnumerable<AtgenStaminaBonus> stamina_bonus,
    IEnumerable<AtgenQuestBonus> quest_bonus,
    IEnumerable<AtgenProductLockList> product_lock_list,
    IEnumerable<ProductList> product_list,
    UpdateDataList update_data_list,
    AtgenUserItemSummon user_item_summon,
    int infancy_paid_diamond_limit
);

[MessagePackObject(true)]
public record ShopGetProductListData(
    IEnumerable<ProductList> product_list,
    UpdateDataList update_data_list,
    int infancy_paid_diamond_limit
);

[MessagePackObject(true)]
public record ShopItemSummonExecData(
    AtgenUserItemSummon user_item_summon,
    IEnumerable<AtgenBuildEventRewardEntityList> item_summon_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record ShopItemSummonOddData(IEnumerable<AtgenItemSummonRateList> item_summon_rate_list);

[MessagePackObject(true)]
public record ShopMaterialShopPurchaseData(
    int is_quest_bonus,
    int is_stone_bonus,
    int is_stamina_bonus,
    IEnumerable<ShopPurchaseList> material_shop_purchase,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record ShopNormalShopPurchaseData(
    int is_quest_bonus,
    int is_stone_bonus,
    int is_stamina_bonus,
    IEnumerable<ShopPurchaseList> normal_shop_purchase,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record ShopPreChargeCheckData(
    int is_quest_bonus,
    int is_stone_bonus,
    int is_stamina_bonus,
    IEnumerable<ShopPurchaseList> material_shop_purchase,
    IEnumerable<ShopPurchaseList> normal_shop_purchase,
    IEnumerable<ShopPurchaseList> special_shop_purchase,
    IEnumerable<AtgenStoneBonus> stone_bonus,
    IEnumerable<AtgenStaminaBonus> stamina_bonus,
    IEnumerable<AtgenQuestBonus> quest_bonus,
    IEnumerable<AtgenProductLockList> product_lock_list,
    IEnumerable<ProductList> product_list,
    UpdateDataList update_data_list,
    AtgenUserItemSummon user_item_summon,
    int infancy_paid_diamond_limit,
    int is_purchase
);

[MessagePackObject(true)]
public record ShopSpecialShopPurchaseData(
    int is_quest_bonus,
    int is_stone_bonus,
    int is_stamina_bonus,
    IEnumerable<ShopPurchaseList> special_shop_purchase,
    IEnumerable<AtgenStoneBonus> stone_bonus,
    IEnumerable<AtgenStaminaBonus> stamina_bonus,
    IEnumerable<AtgenQuestBonus> quest_bonus,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record SimpleEventEntryData(
    SimpleEventUserList simple_event_user_data,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record SimpleEventGetEventDataData(
    SimpleEventUserList simple_event_user_data,
    IEnumerable<EventTradeList> event_trade_list
);

[MessagePackObject(true)]
public record StampGetStampData(IEnumerable<StampList> stamp_list);

[MessagePackObject(true)]
public record StampSetEquipStampData(int result, IEnumerable<EquipStampList> equip_stamp_list);

[MessagePackObject(true)]
public record StoryReadData(
    IEnumerable<AtgenBuildEventRewardEntityList> unit_story_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list
);

[MessagePackObject(true)]
public record StorySkipSkipData(int result_state);

[MessagePackObject(true)]
public record SuggestionGetCategoryListData(IEnumerable<AtgenCategoryList> category_list);

[MessagePackObject(true)]
public record SuggestionSetData;

[MessagePackObject(true)]
public record SummonExcludeGetListData(
    IEnumerable<AtgenDuplicateEntityList> summon_exclude_unit_list
);

[MessagePackObject(true)]
public record SummonExcludeGetOddsDataData(
    OddsRateList odds_rate_list,
    SummonPrizeOddsRateList summon_prize_odds_rate_list
);

[MessagePackObject(true)]
public record SummonExcludeRequestData(
    IEnumerable<AtgenResultUnitList> result_unit_list,
    IEnumerable<AtgenResultPrizeList> result_prize_list,
    IEnumerable<int> presage_effect_list,
    int reversal_effect_index,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<SummonTicketList> summon_ticket_list,
    int result_summon_point,
    IEnumerable<UserSummonList> user_summon_list
);

[MessagePackObject(true)]
public record SummonGetOddsDataData(
    OddsRateList odds_rate_list,
    SummonPrizeOddsRateList summon_prize_odds_rate_list
);

[MessagePackObject(true)]
public record SummonGetSummonHistoryData(IEnumerable<SummonHistoryList> summon_history_list);

[MessagePackObject(true)]
public record SummonGetSummonListData(
    IEnumerable<SummonList> summon_list,
    IEnumerable<SummonList> chara_ssr_summon_list,
    IEnumerable<SummonList> dragon_ssr_summon_list,
    IEnumerable<SummonList> chara_ssr_update_summon_list,
    IEnumerable<SummonList> dragon_ssr_update_summon_list,
    IEnumerable<SummonList> campaign_summon_list,
    IEnumerable<SummonList> campaign_ssr_summon_list,
    IEnumerable<SummonList> platinum_summon_list,
    IEnumerable<SummonList> exclude_summon_list,
    AtgenCsSummonList cs_summon_list,
    IEnumerable<SummonTicketList> summon_ticket_list,
    IEnumerable<SummonPointList> summon_point_list
);

[MessagePackObject(true)]
public record SummonGetSummonPointTradeData(
    IEnumerable<AtgenSummonPointTradeList> summon_point_trade_list,
    IEnumerable<SummonPointList> summon_point_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record SummonRequestData(
    IEnumerable<AtgenResultUnitList> result_unit_list,
    IEnumerable<AtgenResultPrizeList> result_prize_list,
    IEnumerable<int> presage_effect_list,
    int reversal_effect_index,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<SummonTicketList> summon_ticket_list,
    int result_summon_point,
    IEnumerable<UserSummonList> user_summon_list
);

[MessagePackObject(true)]
public record SummonSummonPointTradeData(
    IEnumerable<AtgenBuildEventRewardEntityList> exchange_entity_list,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record TalismanSellData(
    DeleteDataList delete_data_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record TalismanSetLockData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record TimeAttackRankingGetDataData(
    IEnumerable<RankingTierRewardList> ranking_tier_reward_list
);

[MessagePackObject(true)]
public record TimeAttackRankingReceiveTierRewardData(
    IEnumerable<RankingTierRewardList> ranking_tier_reward_list,
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenBuildEventRewardEntityList> ranking_tier_reward_entity_list
);

[MessagePackObject(true)]
public record ToolAuthData(ulong viewer_id, string session_id, string nonce);

[MessagePackObject(true)]
public record ToolGetMaintenanceTimeData(int maintenance_start_time, int maintenance_end_time);

[MessagePackObject(true)]
public record ToolGetServiceStatusData(int service_status);

[MessagePackObject(true)]
public record ToolReauthData(ulong viewer_id, string session_id, string nonce);

[MessagePackObject(true)]
public record ToolSignupData(ulong viewer_id, int servertime);

[MessagePackObject(true)]
public record TrackRecordUpdateProgressData(UpdateDataList update_data_list);

[MessagePackObject(true)]
public record TransitionTransitionByNAccountData(AtgenTransitionResultData transition_result_data);

[MessagePackObject(true)]
public record TreasureTradeGetListAllData(
    IEnumerable<UserTreasureTradeList> user_treasure_trade_list,
    IEnumerable<TreasureTradeList> treasure_trade_list,
    IEnumerable<TreasureTradeList> treasure_trade_all_list,
    DmodeInfo dmode_info
);

[MessagePackObject(true)]
public record TreasureTradeGetListData(
    IEnumerable<UserTreasureTradeList> user_treasure_trade_list,
    IEnumerable<TreasureTradeList> treasure_trade_list,
    IEnumerable<TreasureTradeList> treasure_trade_all_list,
    DmodeInfo dmode_info
);

[MessagePackObject(true)]
public record TreasureTradeTradeData(
    IEnumerable<UserTreasureTradeList> user_treasure_trade_list,
    IEnumerable<TreasureTradeList> treasure_trade_list,
    IEnumerable<TreasureTradeList> treasure_trade_all_list,
    UpdateDataList update_data_list,
    DeleteDataList delete_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record TutorialUpdateFlagsData(
    IEnumerable<int> tutorial_flag_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record TutorialUpdateStepData(
    int step,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record UpdateNamechangeData(string checked_name);

[MessagePackObject(true)]
public record UpdateResetNewData(int result);

[MessagePackObject(true)]
public record UserGetNAccountInfoData(
    UpdateDataList update_data_list,
    AtgenNAccountInfo n_account_info
);

[MessagePackObject(true)]
public record UserGetWalletBalanceData(WalletBalance wallet_balance);

[MessagePackObject(true)]
public record UserLinkedNAccountData(UpdateDataList update_data_list);

[MessagePackObject(true)]
public record UserOptInSettingData(int is_optin);

[MessagePackObject(true)]
public record UserRecoverStaminaByStoneData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    AtgenRecoverData recover_data
);

[MessagePackObject(true)]
public record UserWithdrawalData(int result);

[MessagePackObject(true)]
public record VersionGetResourceVersionData(string resource_version);

[MessagePackObject(true)]
public record WalkerSendGiftMultipleData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    int is_favorite,
    IEnumerable<DragonRewardEntityList> return_gift_list,
    IEnumerable<RewardReliabilityList> reward_reliability_list,
    AtgenWalkerData walker_data
);

[MessagePackObject(true)]
public record WallFailData(
    int result,
    IEnumerable<UserSupportList> fail_helper_list,
    IEnumerable<AtgenHelperDetailList> fail_helper_detail_list,
    AtgenFailQuestDetail fail_quest_detail
);

[MessagePackObject(true)]
public record WallGetMonthlyRewardData(IEnumerable<AtgenUserWallRewardList> user_wall_reward_list);

[MessagePackObject(true)]
public record WallGetWallClearPartyData(
    IEnumerable<PartySettingList> wall_clear_party_setting_list,
    IEnumerable<AtgenLostUnitList> lost_unit_list
);

[MessagePackObject(true)]
public record WallReceiveMonthlyRewardData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    IEnumerable<AtgenBuildEventRewardEntityList> wall_monthly_reward_list,
    IEnumerable<AtgenUserWallRewardList> user_wall_reward_list,
    IEnumerable<AtgenMonthlyWallReceiveList> monthly_wall_receive_list
);

[MessagePackObject(true)]
public record WallRecordRecordData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    AtgenPlayWallDetail play_wall_detail,
    IEnumerable<AtgenBuildEventRewardEntityList> wall_clear_reward_list,
    AtgenWallDropReward wall_drop_reward,
    AtgenWallUnitInfo wall_unit_info
);

[MessagePackObject(true)]
public record WallSetWallClearPartyData(int result);

[MessagePackObject(true)]
public record WallStartStartAssignUnitData(
    IngameData ingame_data,
    IngameWallData ingame_wall_data,
    OddsInfo odds_info,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record WallStartStartData(
    IngameData ingame_data,
    IngameWallData ingame_wall_data,
    OddsInfo odds_info,
    UpdateDataList update_data_list
);

[MessagePackObject(true)]
public record WeaponBodyBuildupPieceData(
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record WeaponBodyCraftData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record WeaponBuildupData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    DeleteDataList delete_data_list
);

[MessagePackObject(true)]
public record WeaponLimitBreakData(
    UpdateDataList update_data_list,
    EntityResult entity_result,
    DeleteDataList delete_data_list
);

[MessagePackObject(true)]
public record WeaponResetPlusCountData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record WeaponSellData(
    DeleteDataList delete_data_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);

[MessagePackObject(true)]
public record WeaponSetLockData(UpdateDataList update_data_list, EntityResult entity_result);

[MessagePackObject(true)]
public record WebviewVersionUrlListData(IEnumerable<AtgenWebviewUrlList> webview_url_list);
