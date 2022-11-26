using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Models.Generated;

[MessagePackObject(true)]
public record AbilityCrestList(
    int ability_crest_id,
    int buildup_count,
    int limit_break_count,
    int equipable_count,
    int hp_plus_count,
    int attack_plus_count,
    int is_favorite,
    int is_new,
    int gettime,
    int ability_1_leve,
    int ability_2_leve
);

[MessagePackObject(true)]
public record AbilityCrestSetList(
    int ability_crest_set_no,
    string ability_crest_set_name,
    int crest_slot_type_1_crest_id_1,
    int crest_slot_type_1_crest_id_2,
    int crest_slot_type_1_crest_id_3,
    int crest_slot_type_2_crest_id_1,
    int crest_slot_type_2_crest_id_2,
    int crest_slot_type_3_crest_id_1,
    int crest_slot_type_3_crest_id_2,
    ulong talisman_key_id
);

[MessagePackObject(true)]
public record AbilityCrestTradeList(
    int ability_crest_trade_id,
    int ability_crest_id,
    int need_dew_point,
    int priority,
    int complete_date,
    int pickup_view_start_date,
    int pickup_view_end_date
);

[MessagePackObject(true)]
public record AchievementList(int achievement_id, int progress, int state);

[MessagePackObject(true)]
public record AlbumDragonData(int dragon_id, int max_level, int max_limit_break_count);

[MessagePackObject(true)]
public record AlbumMissionList(
    int album_mission_id,
    int progress,
    int state,
    int end_date,
    int start_date
);

[MessagePackObject(true)]
public record AlbumPassiveNotice(int is_update_chara, int is_update_dragon);

[MessagePackObject(true)]
public record AlbumWeaponList(int weapon_id, int gettime);

[MessagePackObject(true)]
public record AmuletList(
    int amulet_id,
    ulong amulet_key_id,
    int is_lock,
    int is_new,
    int gettime,
    int ability_1_level,
    int ability_2_level,
    int ability_3_level,
    int hp_plus_count,
    int attack_plus_count,
    int status_plus_count,
    int level,
    int limit_break_count,
    int exp
);

[MessagePackObject(true)]
public record AmuletTradeList(
    int amulet_trade_id,
    int amulet_id,
    int need_dew_point_1,
    int need_dew_point_2,
    int need_dew_point_3,
    int need_dew_point_4,
    int need_dew_point_5,
    int priority,
    int limit,
    int complete_date,
    int pickup_view_start_date,
    int pickup_view_end_date
);

[MessagePackObject(true)]
public record ApiTest;

[MessagePackObject(true)]
public record AreaInfoList(string scene_path, string area_name);

[MessagePackObject(true)]
public record AstralItemList(int astral_item_id, int quantity);

[MessagePackObject(true)]
public record AtgenAddCoinList(ulong build_id, int add_coin);

[MessagePackObject(true)]
public record AtgenAddHarvestList(int material_id, int add_num);

[MessagePackObject(true)]
public record AtgenAddStaminaList(ulong build_id, int add_stamina);

[MessagePackObject(true)]
public record AtgenAlbumQuestPlayRecordList(int quest_play_record_id, int quest_play_record_value);

[MessagePackObject(true)]
public record AtgenAllBonus(int hp, int attack);

[MessagePackObject(true)]
public record AtgenArchivePartyCharaList(int unit_no, Charas chara_id);

[MessagePackObject(true)]
public record AtgenArchivePartyUnitList(
    int unit_no,
    Charas chara_id,
    int equip_dragon_id,
    int equip_weapon_body_id,
    int equip_crest_slot_type_1_crest_id_1,
    int equip_crest_slot_type_1_crest_id_2,
    int equip_crest_slot_type_1_crest_id_3,
    int equip_crest_slot_type_2_crest_id_1,
    int equip_crest_slot_type_2_crest_id_2,
    int equip_crest_slot_type_3_crest_id_1,
    int equip_crest_slot_type_3_crest_id_2,
    int equip_talisman_id,
    int equip_talisman_ability_id_1,
    int equip_talisman_ability_id_2,
    int equip_talisman_ability_id_3,
    int edit_skill_1_chara_id,
    int edit_skill_2_chara_id,
    int ex_ability_1_chara_id,
    int ex_ability_2_chara_id,
    int ex_ability_3_chara_id,
    int ex_ability_4_chara_id
);

[MessagePackObject(true)]
public record AtgenBattleRoyalData(
    int event_cycle_id,
    Charas chara_id,
    int dragon_id,
    int weapon_skin_id,
    int special_skill_id,
    string dungeon_key
);

[MessagePackObject(true)]
public record AtgenBattleRoyalHistoryList(
    int id,
    int event_id,
    Charas chara_id,
    int use_weapon_type,
    int ranking,
    int kill_count,
    int assist_count,
    int battle_royal_point,
    int start_time
);

[MessagePackObject(true)]
public record AtgenBattleRoyalRecord(int ranking, int kill_count, int assist_count);

[MessagePackObject(true)]
public record AtgenBonusFactorList(int factor_type, float factor_value);

[MessagePackObject(true)]
public record AtgenBoxSummonData(
    int event_id,
    int event_point,
    int box_summon_seq,
    int reset_possible,
    int remaining_quantity,
    int max_exec_count,
    IEnumerable<AtgenBoxSummonDetail> box_summon_detail
);

[MessagePackObject(true)]
public record AtgenBoxSummonDetail(
    int id,
    int entity_type,
    int entity_id,
    int entity_quantity,
    int limit,
    int pickup_item_state,
    int reset_item_flag,
    int total_count,
    int two_step_id
);

[MessagePackObject(true)]
public record AtgenBoxSummonResult(
    int event_id,
    int box_summon_seq,
    int reset_possible,
    int remaining_quantity,
    int max_exec_count,
    int is_stopped_by_target,
    IEnumerable<AtgenDrawDetails> draw_details,
    IEnumerable<AtgenBoxSummonDetail> box_summon_detail,
    int event_point
);

[MessagePackObject(true)]
public record AtgenBuildEventRewardEntityList(int entity_type, int entity_id, int entity_quantity);

[MessagePackObject(true)]
public record AtgenBuildupAbilityCrestPieceList(
    int buildup_piece_type,
    int step,
    int is_use_dedicated_material
);

[MessagePackObject(true)]
public record AtgenBuildupWeaponBodyPieceList(
    int buildup_piece_type,
    int buildup_piece_no,
    int step,
    int is_use_dedicated_material
);

[MessagePackObject(true)]
public record AtgenCategoryList(int category_id, string name);

[MessagePackObject(true)]
public record AtgenCharaGrowRecord(Charas chara_id, int take_exp);

[MessagePackObject(true)]
public record AtgenCharaHonorList(Charas chara_id, IEnumerable<AtgenHonorList> honor_list);

[MessagePackObject(true)]
public record AtgenCharaUnitSetDetailList(
    int unit_set_no,
    string unit_set_name,
    ulong dragon_key_id,
    int weapon_body_id,
    int crest_slot_type_1_crest_id_1,
    int crest_slot_type_1_crest_id_2,
    int crest_slot_type_1_crest_id_3,
    int crest_slot_type_2_crest_id_1,
    int crest_slot_type_2_crest_id_2,
    int crest_slot_type_3_crest_id_1,
    int crest_slot_type_3_crest_id_2,
    ulong talisman_key_id
);

[MessagePackObject(true)]
public record AtgenCommentList(string comment_text, string author_type, int comment_created_at);

[MessagePackObject(true)]
public record AtgenCsSummonList(
    IEnumerable<SummonList> summon_list,
    IEnumerable<SummonList> campaign_summon_list,
    IEnumerable<SummonList> campaign_ssr_summon_list,
    IEnumerable<SummonList> platinum_summon_list,
    IEnumerable<SummonList> exclude_summon_list
);

[MessagePackObject(true)]
public record AtgenDamageRecord(int total, int skill, int dot, int critical, int enchant);

[MessagePackObject(true)]
public record AtgenDebugDamageRecordLog(
    Charas chara_id,
    string name,
    string second_name,
    int weapon_type,
    int elemental_type,
    int level,
    int hp,
    int attack,
    int ex_ability_level,
    int ex_ability_2_level,
    int ability_1_level,
    int ability_2_level,
    int ability_3_level,
    int skill_1_level,
    int skill_2_level,
    int burst_attack_level,
    int combo_buildup_count,
    int hp_plus_count,
    int attack_plus_count,
    int total,
    int skill,
    int dot,
    int critical,
    int enchant,
    int dragon_total,
    int dragon_skill,
    int dragon_dot,
    int dragon_critical,
    int dragon_enchant,
    string build_number,
    string resource_version,
    string server_id,
    string device_name
);

[MessagePackObject(true)]
public record AtgenDebugDebugPartyList(
    int id,
    int party_no,
    Charas chara_id,
    int chara_level,
    int chara_rarity,
    int chara_hp_plus_count,
    int chara_attack_plus_count,
    int release_mana_circle,
    int dragon_id,
    int dragon_level,
    int dragon_limit,
    int dragon_hp_plus_count,
    int dragon_attack_plus_count,
    int dragon_reliability_level,
    int weapon_body_id,
    int weapon_body_buildup_count,
    int weapon_body_limit_break_count,
    int weapon_body_limit_over_count,
    int crest_slot_type_1_ability_crest_id_1,
    int crest_slot_type_1_ability_crest_buildup_count_1,
    int crest_slot_type_1_ability_crest_limit_break_count_1,
    int crest_slot_type_1_ability_crest_hp_plus_count_1,
    int crest_slot_type_1_ability_crest_attack_plus_count_1,
    int crest_slot_type_1_ability_crest_id_2,
    int crest_slot_type_1_ability_crest_buildup_count_2,
    int crest_slot_type_1_ability_crest_limit_break_count_2,
    int crest_slot_type_1_ability_crest_hp_plus_count_2,
    int crest_slot_type_1_ability_crest_attack_plus_count_2,
    int crest_slot_type_1_ability_crest_id_3,
    int crest_slot_type_1_ability_crest_buildup_count_3,
    int crest_slot_type_1_ability_crest_limit_break_count_3,
    int crest_slot_type_1_ability_crest_hp_plus_count_3,
    int crest_slot_type_1_ability_crest_attack_plus_count_3,
    int crest_slot_type_2_ability_crest_id_1,
    int crest_slot_type_2_ability_crest_buildup_count_1,
    int crest_slot_type_2_ability_crest_limit_break_count_1,
    int crest_slot_type_2_ability_crest_hp_plus_count_1,
    int crest_slot_type_2_ability_crest_attack_plus_count_1,
    int crest_slot_type_2_ability_crest_id_2,
    int crest_slot_type_2_ability_crest_buildup_count_2,
    int crest_slot_type_2_ability_crest_limit_break_count_2,
    int crest_slot_type_2_ability_crest_hp_plus_count_2,
    int crest_slot_type_2_ability_crest_attack_plus_count_2,
    int crest_slot_type_3_ability_crest_id_1,
    int crest_slot_type_3_ability_crest_buildup_count_1,
    int crest_slot_type_3_ability_crest_limit_break_count_1,
    int crest_slot_type_3_ability_crest_hp_plus_count_1,
    int crest_slot_type_3_ability_crest_attack_plus_count_1,
    int crest_slot_type_3_ability_crest_id_2,
    int crest_slot_type_3_ability_crest_buildup_count_2,
    int crest_slot_type_3_ability_crest_limit_break_count_2,
    int crest_slot_type_3_ability_crest_hp_plus_count_2,
    int crest_slot_type_3_ability_crest_attack_plus_count_2,
    string title
);

[MessagePackObject(true)]
public record AtgenDebugUnitDataList(
    Charas chara_id,
    int chara_lv,
    int chara_hp_plus_count,
    int chara_attack_plus_count,
    int chara_rarity,
    int release_mana_circle,
    int weapon_body_id,
    int weapon_body_buildup_count,
    int weapon_body_limit_break_count,
    int weapon_body_limit_over_count,
    int dragon_id,
    int dragon_lv,
    int dragon_hp_plus_count,
    int dragon_attack_plus_count,
    int dragon_limit,
    int dragon_reliability_level,
    int crest_slot_type_1_ability_crest_id_1,
    int crest_slot_type_1_ability_crest_buildup_count_1,
    int crest_slot_type_1_ability_crest_limit_break_count_1,
    int crest_slot_type_1_ability_crest_hp_plus_count_1,
    int crest_slot_type_1_ability_crest_attack_plus_count_1,
    int crest_slot_type_1_ability_crest_id_2,
    int crest_slot_type_1_ability_crest_buildup_count_2,
    int crest_slot_type_1_ability_crest_limit_break_count_2,
    int crest_slot_type_1_ability_crest_hp_plus_count_2,
    int crest_slot_type_1_ability_crest_attack_plus_count_2,
    int crest_slot_type_1_ability_crest_id_3,
    int crest_slot_type_1_ability_crest_buildup_count_3,
    int crest_slot_type_1_ability_crest_limit_break_count_3,
    int crest_slot_type_1_ability_crest_hp_plus_count_3,
    int crest_slot_type_1_ability_crest_attack_plus_count_3,
    int crest_slot_type_2_ability_crest_id_1,
    int crest_slot_type_2_ability_crest_buildup_count_1,
    int crest_slot_type_2_ability_crest_limit_break_count_1,
    int crest_slot_type_2_ability_crest_hp_plus_count_1,
    int crest_slot_type_2_ability_crest_attack_plus_count_1,
    int crest_slot_type_2_ability_crest_id_2,
    int crest_slot_type_2_ability_crest_buildup_count_2,
    int crest_slot_type_2_ability_crest_limit_break_count_2,
    int crest_slot_type_2_ability_crest_hp_plus_count_2,
    int crest_slot_type_2_ability_crest_attack_plus_count_2,
    int crest_slot_type_3_ability_crest_id_1,
    int crest_slot_type_3_ability_crest_buildup_count_1,
    int crest_slot_type_3_ability_crest_limit_break_count_1,
    int crest_slot_type_3_ability_crest_hp_plus_count_1,
    int crest_slot_type_3_ability_crest_attack_plus_count_1,
    int crest_slot_type_3_ability_crest_id_2,
    int crest_slot_type_3_ability_crest_buildup_count_2,
    int crest_slot_type_3_ability_crest_limit_break_count_2,
    int crest_slot_type_3_ability_crest_hp_plus_count_2,
    int crest_slot_type_3_ability_crest_attack_plus_count_2
);

[MessagePackObject(true)]
public record AtgenDeleteAmuletList(ulong amulet_key_id);

[MessagePackObject(true)]
public record AtgenDeleteDragonList(ulong dragon_key_id);

[MessagePackObject(true)]
public record AtgenDeleteTalismanList(ulong talisman_key_id);

[MessagePackObject(true)]
public record AtgenDeleteWeaponList(ulong weapon_key_id);

[MessagePackObject(true)]
public record AtgenDmodeAreaInfo(
    int floor_num,
    float quest_time,
    int dmode_score,
    int current_area_theme_id,
    int current_area_id
);

[MessagePackObject(true)]
public record AtgenDmodeDragonUseList(int dragon_id, int use_count);

[MessagePackObject(true)]
public record AtgenDmodeDropList(int type, int id, int quantity);

[MessagePackObject(true)]
public record AtgenDmodeDropObj(
    int obj_id,
    int obj_type,
    IEnumerable<AtgenDmodeDropList> dmode_drop_list
);

[MessagePackObject(true)]
public record AtgenDmodeDungeonItemOptionList(int item_no, int abnormal_status_invalid_count);

[MessagePackObject(true)]
public record AtgenDmodeDungeonItemStateList(int item_no, int state);

[MessagePackObject(true)]
public record AtgenDmodeDungeonOdds(
    IEnumerable<AtgenDmodeSelectDragonList> dmode_select_dragon_list,
    IEnumerable<DmodeDungeonItemList> dmode_dungeon_item_list,
    DmodeOddsInfo dmode_odds_info
);

[MessagePackObject(true)]
public record AtgenDmodeEnemy(
    int enemy_idx,
    int is_pop,
    int level,
    int param_id,
    IEnumerable<AtgenDmodeDropList> dmode_drop_list
);

[MessagePackObject(true)]
public record AtgenDmodeHoldDragonList(int dragon_id, int count);

[MessagePackObject(true)]
public record AtgenDmodeSelectDragonList(
    int select_dragon_no,
    int dragon_id,
    int is_rare,
    int pay_dmode_point_1,
    int pay_dmode_point_2
);

[MessagePackObject(true)]
public record AtgenDmodeTreasureRecord(IEnumerable<int> drop_obj, IEnumerable<int> enemy);

[MessagePackObject(true)]
public record AtgenDmodeUnitInfo(
    int level,
    int exp,
    IEnumerable<int> equip_crest_item_no_sort_list,
    IEnumerable<int> bag_item_no_sort_list,
    IEnumerable<int> skill_bag_item_no_sort_list,
    IEnumerable<AtgenDmodeHoldDragonList> dmode_hold_dragon_list,
    int take_dmode_point_1,
    int take_dmode_point_2
);

[MessagePackObject(true)]
public record AtgenDragonBonus(int elemental_type, float dragon_bonus, float hp, float attack);

[MessagePackObject(true)]
public record AtgenDragonGiftRewardList(
    int dragon_gift_id,
    int is_favorite,
    IEnumerable<DragonRewardEntityList> return_gift_list,
    IEnumerable<RewardReliabilityList> reward_reliability_list
);

[MessagePackObject(true)]
public record AtgenDragonTimeBonus(float dragon_time_bonus);

[MessagePackObject(true)]
public record AtgenDrawDetails(
    int id,
    int is_new,
    int entity_type,
    int entity_id,
    int entity_quantity,
    int view_rarity
);

[MessagePackObject(true)]
public record AtgenDropAll(int id, int type, int quantity, int place, float factor);

[MessagePackObject(true)]
public record AtgenDropList(int type, int id, int quantity, int place);

[MessagePackObject(true)]
public record AtgenDropObj(
    int obj_id,
    int obj_type,
    int is_rare,
    IEnumerable<AtgenDropList> drop_list
);

[MessagePackObject(true)]
public record AtgenDuplicateEntityList(int entity_type, int entity_id);

[MessagePackObject(true)]
public record AtgenElementBonus(int elemental_type, float hp, float attack);

[MessagePackObject(true)]
public record AtgenEnemy(
    int piece,
    int enemy_idx,
    int is_pop,
    int is_rare,
    int param_id,
    IEnumerable<EnemyDropList> enemy_drop_list
);

[MessagePackObject(true)]
public record AtgenEnemyPiece(Materials id, int quantity);

[MessagePackObject(true)]
public record AtgenEnemySmash(int count);

[MessagePackObject(true)]
public record AtgenEntryConditions(
    IEnumerable<int> unaccepted_element_type_list,
    IEnumerable<int> unaccepted_weapon_type_list,
    int required_party_power,
    int objective_text_id,
    string objective_free_text
);

[MessagePackObject(true)]
public record AtgenEventBoost(int event_effect, float effect_value);

[MessagePackObject(true)]
public record AtgenEventDamageData(
    long user_damage_value,
    int user_target_time,
    long total_damage_value,
    int total_target_time,
    int total_aggregate_time
);

[MessagePackObject(true)]
public record AtgenEventDamageHistoryList(int target_time, long total_damage_value);

[MessagePackObject(true)]
public record AtgenEventDamageRewardList(
    int target_time,
    IEnumerable<AtgenBuildEventRewardEntityList> reward_entity_list
);

[MessagePackObject(true)]
public record AtgenEventFortData(int plant_id, int level);

[MessagePackObject(true)]
public record AtgenEventPassiveUpList(int passive_id, int progress);

[MessagePackObject(true)]
public record AtgenExchangeSummomPointList(
    int summon_point_id,
    int summon_point,
    int cs_summon_point
);

[MessagePackObject(true)]
public record AtgenFailQuestDetail(int quest_id, int wall_id, int wall_level, int is_host);

[MessagePackObject(true)]
public record AtgenFirstClearSet(int id, int type, int quantity);

[MessagePackObject(true)]
public record AtgenFirstMeeting(int headcount, int id, int type, int total_quantity);

[MessagePackObject(true)]
public record AtgenGrade(
    int min_value,
    int max_value,
    int grade_num,
    IEnumerable<AtgenDropList> drop_list
);

[MessagePackObject(true)]
public record AtgenGuild(
    int guild_id,
    int guild_emblem_id,
    string guild_name,
    int is_penalty_guild_name
);

[MessagePackObject(true)]
public record AtgenGuildInviteParamsList(int guild_id, ulong guild_invite_id);

[MessagePackObject(true)]
public record AtgenHarvestBuildList(
    ulong build_id,
    IEnumerable<AtgenAddHarvestList> add_harvest_list
);

[MessagePackObject(true)]
public record AtgenHelperDetailList(
    ulong viewer_id,
    int is_friend,
    int get_mana_point,
    int apply_send_status
);

[MessagePackObject(true)]
public record AtgenHonorList(int honor_id);

[MessagePackObject(true)]
public record AtgenIngameWalker(int skill_2_level);

[MessagePackObject(true)]
public record AtgenInquiryFaqList(int id, string question, string answer);

[MessagePackObject(true)]
public record AtgenItemSummonRateList(
    int entity_type,
    int entity_id,
    int entity_quantity,
    string entity_rate
);

[MessagePackObject(true)]
public record AtgenLatest(int episode);

[MessagePackObject(true)]
public record AtgenLoginBonusList(
    int reward_code,
    int login_bonus_id,
    int total_login_day,
    int reward_day,
    int entity_type,
    int entity_id,
    int entity_quantity,
    int entity_level,
    int entity_limit_break_count
);

[MessagePackObject(true)]
public record AtgenLoginLotteryRewardList(
    int login_lottery_id,
    int entity_type,
    int entity_id,
    int entity_quantity,
    int is_pickup,
    int is_guaranteed
);

[MessagePackObject(true)]
public record AtgenLostUnitList(int unit_no, int entity_type, int entity_id);

[MessagePackObject(true)]
public record AtgenLotteryEntitySetList(
    int lottery_prize_rank,
    string rate,
    IEnumerable<AtgenBuildEventRewardEntityList> entity_list
);

[MessagePackObject(true)]
public record AtgenLotteryPrizeRankList(int lottery_prize_rank, string total_rate);

[MessagePackObject(true)]
public record AtgenLotteryResultList(
    int lottery_rank,
    int rank_entiry_quantity,
    IEnumerable<AtgenBuildEventRewardEntityList> entity_list
);

[MessagePackObject(true)]
public record AtgenMainStoryMissionStateList(int main_story_mission_id, int state);

[MessagePackObject(true)]
public record AtgenMissionParamsList(int daily_mission_id, int day_no);

[MessagePackObject(true)]
public record AtgenMissionsClearSet(int id, int type, int quantity, int mission_no);

[MessagePackObject(true)]
public record AtgenMonthlyWallReceiveList(int quest_group_id, int is_receive_reward);

[MessagePackObject(true)]
public record AtgenMultiServer(string host, string app_id);

[MessagePackObject(true)]
public record AtgenNAccountInfo(string email, string nickname);

[MessagePackObject(true)]
public record AtgenNeedTradeEntityList(
    int entity_type,
    int entity_id,
    int entity_quantity,
    int limit_break_count
);

[MessagePackObject(true)]
public record AtgenNeedUnitList(int type, ulong key_id);

[MessagePackObject(true)]
public record AtgenNormalMissionNotice(
    int is_update,
    int all_mission_count,
    int completed_mission_count,
    int receivable_reward_count,
    int pickup_mission_count,
    int current_mission_id,
    IEnumerable<int> new_complete_mission_id_list
);

[MessagePackObject(true)]
public record AtgenNotReceivedMissionIdListWithDayNo(
    int day_no,
    IEnumerable<int> not_received_mission_id_list
);

[MessagePackObject(true)]
public record AtgenOpinionList(
    string opinion_id,
    string opinion_text,
    int created_at,
    int updated_at
);

[MessagePackObject(true)]
public record AtgenOpinionTypeList(int opinion_type, string name);

[MessagePackObject(true)]
public record AtgenOption(
    int strength_param_id,
    int strength_ability_id,
    int strength_skill_id,
    int abnormal_status_invalid_count
);

[MessagePackObject(true)]
public record AtgenOwnDamageRankingList(int rank, int is_new, Charas chara_id, long damage_value);

[MessagePackObject(true)]
public record AtgenOwnRankingList(
    int rank,
    int is_new,
    int ranking_id,
    ulong viewer_id,
    int quest_id,
    float clear_time,
    int start_time,
    int end_time,
    string party_hash,
    ulong viewer_id_1,
    ulong viewer_id_2,
    ulong viewer_id_3,
    ulong viewer_id_4,
    int chara_id_1,
    int chara_id_2,
    int chara_id_3,
    int chara_id_4,
    int chara_rarity_1,
    int chara_rarity_2,
    int chara_rarity_3,
    int chara_rarity_4,
    int chara_level_1,
    int chara_level_2,
    int chara_level_3,
    int chara_level_4
);

[MessagePackObject(true)]
public record AtgenPaid(string code, int total);

[MessagePackObject(true)]
public record AtgenParamBonus(int weapon_type, float hp, float attack);

[MessagePackObject(true)]
public record AtgenPenaltyData(
    int report_id,
    int point,
    int penalty_type,
    int penalty_text_type,
    string penalty_body
);

[MessagePackObject(true)]
public record AtgenPlayWallDetail(int wall_id, int after_wall_level, int before_wall_level);

[MessagePackObject(true)]
public record AtgenPlusCountParamsList(int plus_count_type, int plus_count);

[MessagePackObject(true)]
public record AtgenProductionRp(float speed, int max);

[MessagePackObject(true)]
public record AtgenProductLockList(int shop_type, int goods_id, int is_lock, int expire_time);

[MessagePackObject(true)]
public record AtgenQuestBonus(int goods_id, int effect_start_time, int effect_end_time);

[MessagePackObject(true)]
public record AtgenQuestDropInfo(
    IEnumerable<AtgenDuplicateEntityList> drop_info_list,
    IEnumerable<AtgenDuplicateEntityList> host_drop_info_list,
    IEnumerable<AtgenDuplicateEntityList> fever_drop_info_list,
    IEnumerable<AtgenDuplicateEntityList> quest_bonus_info_list,
    IEnumerable<AtgenDuplicateEntityList> campaign_extra_reward_info_list,
    IEnumerable<AtgenQuestRebornBonusInfoList> quest_reborn_bonus_info_list
);

[MessagePackObject(true)]
public record AtgenQuestRebornBonusInfoList(
    int reborn_count,
    IEnumerable<AtgenDuplicateEntityList> bonus_info_list
);

[MessagePackObject(true)]
public record AtgenQuestStoryRewardList(
    int entity_type,
    int entity_id,
    int entity_quantity,
    int entity_level,
    int entity_limit_break_count
);

[MessagePackObject(true)]
public record AtgenRarityGroupList(
    bool pickup,
    int rarity,
    string total_rate,
    string chara_rate,
    string dragon_rate,
    string amulet_rate
);

[MessagePackObject(true)]
public record AtgenRarityList(int rarity, string total_rate);

[MessagePackObject(true)]
public record AtgenReceiveQuestBonus(
    int target_quest_id,
    int receive_bonus_count,
    float bonus_factor,
    IEnumerable<AtgenBuildEventRewardEntityList> quest_bonus_entity_list
);

[MessagePackObject(true)]
public record AtgenRecoverData(int recover_stamina_type, int recover_stamina_point);

[MessagePackObject(true)]
public record AtgenRedoableSummonResultUnitList(int entity_type, int id, int rarity)
    : SimpleSummonReward(entity_type, id, rarity);

[MessagePackObject(true)]
public record AtgenRequestAbilityCrestSetData(
    int crest_slot_type_1_crest_id_1,
    int crest_slot_type_1_crest_id_2,
    int crest_slot_type_1_crest_id_3,
    int crest_slot_type_2_crest_id_1,
    int crest_slot_type_2_crest_id_2,
    int crest_slot_type_3_crest_id_1,
    int crest_slot_type_3_crest_id_2,
    ulong talisman_key_id
);

[MessagePackObject(true)]
public record AtgenRequestCharaUnitSetData(
    ulong dragon_key_id,
    int weapon_body_id,
    int crest_slot_type_1_crest_id_1,
    int crest_slot_type_1_crest_id_2,
    int crest_slot_type_1_crest_id_3,
    int crest_slot_type_2_crest_id_1,
    int crest_slot_type_2_crest_id_2,
    int crest_slot_type_3_crest_id_1,
    int crest_slot_type_3_crest_id_2,
    ulong talisman_key_id
);

[MessagePackObject(true)]
public record AtgenRequestQuestMultipleList(int quest_id, int play_count, int bet_count);

[MessagePackObject(true)]
public record AtgenResultPrizeList(
    int summon_prize_rank,
    IEnumerable<AtgenBuildEventRewardEntityList> entity_list
);

[MessagePackObject(true)]
public record AtgenResultUnitList(int entity_type, int id, int rarity, bool is_new, int dew_point);

[MessagePackObject(true)]
public record AtgenRewardTalismanList(
    int talisman_id,
    int talisman_ability_id_1,
    int talisman_ability_id_2,
    int talisman_ability_id_3,
    int additional_hp,
    int additional_attack
);

[MessagePackObject(true)]
public record AtgenRoomMemberList(ulong viewer_id);

[MessagePackObject(true)]
public record AtgenScoreMissionSuccessList(
    int score_mission_complete_type,
    int score_target_value,
    float correction_value
);

[MessagePackObject(true)]
public record AtgenScoringEnemyPointList(int scoring_enemy_id, int smash_count, int point);

[MessagePackObject(true)]
public record AtgenShopGiftList(int dragon_gift_id, int price, int is_buy);

[MessagePackObject(true)]
public record AtgenStaminaBonus(
    int goods_id,
    int last_bonus_time,
    int effect_start_time,
    int effect_end_time
);

[MessagePackObject(true)]
public record AtgenStoneBonus(int goods_id, int bonus_count, int last_bonus_time);

[MessagePackObject(true)]
public record AtgenSummonPointTradeList(int trade_id, int entity_type, int entity_id);

[MessagePackObject(true)]
public record AtgenSummonPrizeEntitySetList(
    int summon_prize_rank,
    string rate,
    IEnumerable<AtgenBuildEventRewardEntityList> entity_list
);

[MessagePackObject(true)]
public record AtgenSummonPrizeRankList(int summon_prize_rank, string total_rate);

[MessagePackObject(true)]
public record AtgenSupportAmulet(
    ulong amulet_key_id,
    int amulet_id,
    int level,
    int attack,
    int ability_1_level,
    int ability_2_level,
    int ability_3_level,
    int hp_plus_count,
    int attack_plus_count,
    int status_plus_count,
    int limit_break_count
);

[MessagePackObject(true)]
public record AtgenSupportChara(
    Charas chara_id,
    int level,
    int additional_max_level,
    int rarity,
    int hp,
    int attack,
    int hp_plus_count,
    int attack_plus_count,
    int status_plus_count,
    int ability_1_level,
    int ability_2_level,
    int ability_3_level,
    int ex_ability_level,
    int ex_ability_2_level,
    int skill_1_level,
    int skill_2_level,
    int is_unlock_edit_skill
);

[MessagePackObject(true)]
public record AtgenSupportCrestSlotType1List(
    int ability_crest_id,
    int buildup_count,
    int limit_break_count,
    int hp_plus_count,
    int attack_plus_count,
    int equipable_count
);

[MessagePackObject(true)]
public record AtgenSupportData(
    ulong viewer_id,
    string name,
    int is_friend,
    CharaList chara_data,
    DragonList dragon_data,
    WeaponList weapon_data,
    AmuletList amulet_data,
    AmuletList amulet_2_data,
    GameWeaponSkin weapon_skin_data,
    GameWeaponBody weapon_body_data,
    IEnumerable<GameAbilityCrest> crest_slot_type_1_crest_list,
    IEnumerable<GameAbilityCrest> crest_slot_type_2_crest_list,
    IEnumerable<GameAbilityCrest> crest_slot_type_3_crest_list,
    TalismanList talisman_data,
    IEnumerable<WeaponPassiveAbilityList> game_weapon_passive_ability_list
);

[MessagePackObject(true)]
public record AtgenSupportDragon(
    ulong dragon_key_id,
    int dragon_id,
    int level,
    int hp,
    int attack,
    int skill_1_level,
    int ability_1_level,
    int ability_2_level,
    int hp_plus_count,
    int attack_plus_count,
    int status_plus_count,
    int limit_break_count
);

[MessagePackObject(true)]
public record AtgenSupportReward(int serve_count, int mana_point);

[MessagePackObject(true)]
public record AtgenSupportTalisman(
    ulong talisman_key_id,
    int talisman_id,
    int talisman_ability_id_1,
    int talisman_ability_id_2,
    int talisman_ability_id_3,
    int additional_hp,
    int additional_attack
);

[MessagePackObject(true)]
public record AtgenSupportUserDataDetail(
    UserSupportList user_support_data,
    FortBonusList fort_bonus_list,
    IEnumerable<int> mana_circle_piece_id_list,
    int dragon_reliability_level,
    int is_friend,
    int apply_send_status
);

[MessagePackObject(true)]
public record AtgenSupportUserDetailList(ulong viewer_id, int gettable_mana_point, int is_friend);

[MessagePackObject(true)]
public record AtgenSupportWeapon(
    ulong weapon_key_id,
    int weapon_id,
    int level,
    int attack,
    int skill_level,
    int hp_plus_count,
    int attack_plus_count,
    int status_plus_count,
    int limit_break_count
);

[MessagePackObject(true)]
public record AtgenSupportWeaponBody(
    int weapon_body_id,
    int buildup_count,
    int limit_break_count,
    int limit_over_count,
    int additional_effect_count,
    int equipable_count,
    int additional_crest_slot_type_1_count,
    int additional_crest_slot_type_2_count,
    int additional_crest_slot_type_3_count
);

[MessagePackObject(true)]
public record AtgenTargetList(string target_name, IEnumerable<ulong> target_id_list);

[MessagePackObject(true)]
public record AtgenTransitionResultData(ulong abolished_viewer_id, ulong linked_viewer_id);

[MessagePackObject(true)]
public record AtgenTreasureRecord(
    int area_idx,
    IEnumerable<int> drop_obj,
    IEnumerable<int> enemy,
    IEnumerable<AtgenEnemySmash> enemy_smash
);

[MessagePackObject(true)]
public record AtgenUnit(
    IEnumerable<OddsUnitDetail> chara_odds_list,
    IEnumerable<OddsUnitDetail> dragon_odds_list,
    IEnumerable<OddsUnitDetail> amulet_odds_list
);

[MessagePackObject(true)]
public record AtgenUnitData(
    Charas chara_id,
    int skill_1_level,
    int skill_2_level,
    int ability_1_level,
    int ability_2_level,
    int ability_3_level,
    int ex_ability_level,
    int ex_ability_2_level,
    int burst_attack_level,
    int combo_buildup_count
);

[MessagePackObject(true)]
public record AtgenUnitList(int id, string rate);

[MessagePackObject(true)]
public record AtgenUseItemList(int item_id, int item_quantity);

[MessagePackObject(true)]
public record AtgenUserBuildEventItemList(int user_build_event_item, int event_item_value);

[MessagePackObject(true)]
public record AtgenUserClb01EventItemList(int user_clb_01_event_item, int event_item_value);

[MessagePackObject(true)]
public record AtgenUserCollectEventItemList(int user_collect_event_item, int event_item_value);

[MessagePackObject(true)]
public record AtgenUserEventTradeList(int event_trade_id, int trade_count);

[MessagePackObject(true)]
public record AtgenUserItemSummon(int daily_summon_count, int last_summon_time);

[MessagePackObject(true)]
public record AtgenUserMazeEventItemList(int user_maze_event_item, int event_item_value);

[MessagePackObject(true)]
public record AtgenUserMazeEventItemList__2(int event_item_id, int quantity);

[MessagePackObject(true)]
public record AtgenUserWallRewardList(
    int quest_group_id,
    int sum_wall_level,
    int last_reward_date,
    int reward_status
);

[MessagePackObject(true)]
public record EulaVersion(string region, string lang, int eula_version, int privacy_policy_version);

[MessagePackObject(true)]
public record AtgenWalkerData(
    int reliability_level,
    int reliability_total_exp,
    int last_contact_time,
    int skill_2_level
);

[MessagePackObject(true)]
public record AtgenWallDropReward(
    IEnumerable<AtgenBuildEventRewardEntityList> reward_entity_list,
    int take_coin,
    int take_mana
);

[MessagePackObject(true)]
public record AtgenWallUnitInfo(
    IEnumerable<PartySettingList> quest_party_setting_list,
    IEnumerable<UserSupportList> helper_list,
    IEnumerable<AtgenHelperDetailList> helper_detail_list
);

[MessagePackObject(true)]
public record AtgenWeaponKeyDataList(ulong key_id, int target_set_num, int target_weapon_num);

[MessagePackObject(true)]
public record AtgenWeaponSetList(
    int select_weapon_id,
    IEnumerable<AtgenWeaponKeyDataList> weapon_key_data_list
);

[MessagePackObject(true)]
public record AtgenWebviewUrlList(string function_name, string url);

[MessagePackObject(true)]
public record BattleRoyalCharaSkinList(int battle_royal_chara_skin_id, int gettime);

[MessagePackObject(true)]
public record BattleRoyalCycleUserRecord(
    int event_id,
    int event_cycle_id,
    int cycle_total_battle_royal_point
);

[MessagePackObject(true)]
public record BattleRoyalEventItemList(int event_id, int item_id, int quantity);

[MessagePackObject(true)]
public record BattleRoyalEventUserRecord(
    int event_id,
    int total_battle_royal_point,
    int rank_top_count,
    int top_four_count,
    int total_kill_count,
    int max_kill_count,
    int total_assist_count,
    int max_assist_count,
    int last_use_weapon_type,
    int use_swd_count,
    int use_kat_count,
    int use_dag_count,
    int use_axe_count,
    int use_lan_count,
    int use_bow_count,
    int use_rod_count,
    int use_can_count,
    int use_gun_count
);

[MessagePackObject(true)]
public record BattleRoyalResult(
    int battle_royal_cycle_id,
    Charas chara_id,
    int weapon_skin_id,
    int ranking,
    int kill_count,
    int assist_count,
    int take_exp,
    int player_level_up_fstone,
    int take_accumulate_point,
    int take_battle_royal_point
);

[MessagePackObject(true)]
public record BeginnerMissionList(
    int beginner_mission_id,
    int progress,
    int state,
    int end_date,
    int start_date
);

[MessagePackObject(true)]
public record BuildEventRewardList(int event_id, int event_reward_id);

[MessagePackObject(true)]
public record BuildEventUserList(
    int build_event_id,
    IEnumerable<AtgenUserBuildEventItemList> user_build_event_item_list
);

[MessagePackObject(true)]
public record BuildList(
    ulong build_id,
    int plant_id,
    int level,
    int fort_plant_detail_id,
    int position_x,
    int position_z,
    int build_status,
    int build_start_date,
    int build_end_date,
    int remain_time,
    int last_income_time,
    int is_new
);

[MessagePackObject(true)]
public record CastleStoryList(int castle_story_id, int is_read);

[MessagePackObject(true)]
public record CharaFriendshipList(
    Charas chara_id,
    int add_point,
    int total_point,
    int is_temporary
);

[MessagePackObject(true)]
public record CharaList(
    Charas chara_id,
    int exp,
    int level,
    int additional_max_level,
    int hp,
    int attack,
    int ex_ability_level,
    int ex_ability_2_level,
    int ability_1_level,
    int ability_2_level,
    int ability_3_level,
    int is_new,
    int skill_1_level,
    int skill_2_level,
    int burst_attack_level,
    int rarity,
    int limit_break_count,
    int hp_plus_count,
    int attack_plus_count,
    int status_plus_count,
    int combo_buildup_count,
    int is_unlock_edit_skill,
    int gettime,
    IEnumerable<int> mana_circle_piece_id_list,
    int is_temporary,
    int list_view_flag
);

[MessagePackObject(true)]
public record CharaUnitSetList(
    Charas chara_id,
    IEnumerable<AtgenCharaUnitSetDetailList> chara_unit_set_detail_list
);

[MessagePackObject(true)]
public record Clb01EventUserList(
    int event_id,
    IEnumerable<AtgenUserClb01EventItemList> user_clb_01_event_item_list
);

[MessagePackObject(true)]
public record CollectEventUserList(
    int event_id,
    IEnumerable<AtgenUserCollectEventItemList> user_collect_event_item_list
);

[MessagePackObject(true)]
public record CombatEventUserList(
    int event_id,
    int event_point,
    int exchange_item_01,
    int quest_unlock_item_01,
    int story_unlock_item_01,
    int advent_item_01
);

[MessagePackObject(true)]
public record ConvertedEntityList(
    int before_entity_type,
    int before_entity_id,
    int before_entity_quantity,
    int after_entity_type,
    int after_entity_id,
    int after_entity_quantity
);

[MessagePackObject(true)]
public record CraftList(int weapon_id, int is_new);

[MessagePackObject(true)]
public record CurrentMainStoryMission(
    int main_story_mission_group_id,
    IEnumerable<AtgenMainStoryMissionStateList> main_story_mission_state_list
);

[MessagePackObject(true)]
public record DailyMissionList(
    int daily_mission_id,
    int progress,
    int state,
    int day_no,
    int weekly_mission_id,
    int week_no,
    int end_date,
    int start_date,
    int is_lock_receive_reward,
    int is_pickup
);

[MessagePackObject(true)]
public record DataHeader(int result_code);

[MessagePackObject(true)]
public record DeleteDataList(
    IEnumerable<AtgenDeleteDragonList> delete_dragon_list,
    IEnumerable<AtgenDeleteTalismanList> delete_talisman_list,
    IEnumerable<AtgenDeleteWeaponList> delete_weapon_list,
    IEnumerable<AtgenDeleteAmuletList> delete_amulet_list
);

[MessagePackObject(true)]
public record DiamondData(int paid_diamond, int free_diamond);

[MessagePackObject(true)]
public record DmodeCharaList(
    Charas chara_id,
    int max_floor_num,
    int select_servitor_id,
    int select_edit_skill_chara_id_1,
    int select_edit_skill_chara_id_2,
    int select_edit_skill_chara_id_3,
    int max_dmode_score
);

[MessagePackObject(true)]
public record DmodeDungeonInfo(
    Charas chara_id,
    int floor_num,
    int quest_time,
    int dungeon_score,
    int is_play_end,
    int state
);

[MessagePackObject(true)]
public record DmodeDungeonItemList(int item_no, int item_id, int item_state, AtgenOption option);

[MessagePackObject(true)]
public record DmodeExpedition(
    int chara_id_1,
    int chara_id_2,
    int chara_id_3,
    int chara_id_4,
    int start_time,
    int target_floor_num,
    int state
);

[MessagePackObject(true)]
public record DmodeFloorData(
    string unique_key,
    string floor_key,
    int is_end,
    int is_play_end,
    int is_view_area_start_equipment,
    AtgenDmodeAreaInfo dmode_area_info,
    AtgenDmodeUnitInfo dmode_unit_info,
    AtgenDmodeDungeonOdds dmode_dungeon_odds
);

[MessagePackObject(true)]
public record DmodeInfo(
    int total_max_floor_num,
    int recovery_count,
    int recovery_time,
    int floor_skip_count,
    int floor_skip_time,
    int dmode_point_1,
    int dmode_point_2,
    int is_entry
);

[MessagePackObject(true)]
public record DmodeIngameData(
    string unique_key,
    int start_floor_num,
    int target_floor_num,
    int recovery_count,
    int recovery_time,
    int servitor_id,
    int dmode_level_group_id,
    AtgenUnitData unit_data,
    IEnumerable<DmodeServitorPassiveList> dmode_servitor_passive_list
);

[MessagePackObject(true)]
public record DmodeIngameResult(
    int floor_num,
    int is_record_floor_num,
    IEnumerable<int> chara_id_list,
    float quest_time,
    int is_view_quest_time,
    int dmode_score,
    IEnumerable<AtgenRewardTalismanList> reward_talisman_list,
    int take_dmode_point_1,
    int take_dmode_point_2,
    int take_player_exp,
    int player_level_up_fstone,
    int clear_state
);

[MessagePackObject(true)]
public record DmodeOddsInfo(
    IEnumerable<AtgenDmodeDropObj> dmode_drop_obj,
    IEnumerable<AtgenDmodeEnemy> dmode_enemy
);

[MessagePackObject(true)]
public record DmodePlayRecord(
    string unique_key,
    string floor_key,
    int floor_num,
    int is_floor_incomplete,
    AtgenDmodeTreasureRecord dmode_treasure_record,
    IEnumerable<AtgenDmodeDungeonItemStateList> dmode_dungeon_item_state_list,
    IEnumerable<AtgenDmodeDungeonItemOptionList> dmode_dungeon_item_option_list,
    IEnumerable<AtgenDmodeDragonUseList> dmode_dragon_use_list,
    IEnumerable<int> equip_crest_item_no_sort_list,
    IEnumerable<int> bag_item_no_sort_list,
    IEnumerable<int> skill_bag_item_no_sort_list,
    float quest_time,
    int select_dragon_no
);

[MessagePackObject(true)]
public record DmodeServitorPassiveList(int passive_no, int passive_level);

[MessagePackObject(true)]
public record DmodeStoryList(int dmode_story_id, int is_read);

[MessagePackObject(true)]
public record DragonGiftList(int dragon_gift_id, int quantity);

[MessagePackObject(true)]
public record DragonList(
    int dragon_id,
    ulong dragon_key_id,
    int level,
    int exp,
    int is_lock,
    int is_new,
    int get_time,
    int skill_1_level,
    int ability_1_level,
    int ability_2_level,
    int limit_break_count,
    int hp_plus_count,
    int attack_plus_count,
    int status_plus_count
);

[MessagePackObject(true)]
public record DragonReliabilityList(
    int dragon_id,
    int reliability_level,
    int reliability_total_exp,
    int gettime,
    int last_contact_time
);

[MessagePackObject(true)]
public record DragonRewardEntityList(
    int entity_type,
    int entity_id,
    int entity_quantity,
    int is_over
);

[MessagePackObject(true)]
public record DrillMissionGroupList(int drill_mission_group_id);

[MessagePackObject(true)]
public record DrillMissionList(
    int drill_mission_id,
    int progress,
    int state,
    int end_date,
    int start_date
);

[MessagePackObject(true)]
public record dummy;

[MessagePackObject(true)]
public record EarnEventUserList(
    int event_id,
    int event_point,
    int exchange_item_01,
    int exchange_item_02,
    int advent_item_quantity_01
);

[MessagePackObject(true)]
public record EditSkillCharaData(Charas chara_id, int edit_skill_level);

[MessagePackObject(true)]
public record EmblemList(int emblem_id, int is_new, int gettime);

[MessagePackObject(true)]
public record EnemyBookList(int enemy_book_id, int piece_count, int kill_count);

[MessagePackObject(true)]
public record EnemyDamageHistory(IEnumerable<int> damage, IEnumerable<int> combo);

[MessagePackObject(true)]
public record EnemyDropList(int coin, int mana, IEnumerable<AtgenDropList> drop_list);

[MessagePackObject(true)]
public record EquipStampList(int slot, int stamp_id);

[MessagePackObject(true)]
public record EventAbilityCharaList(Charas chara_id, int ability_id_1, int ability_id_2);

[MessagePackObject(true)]
public record EventCycleRewardList(int event_cycle_id, int event_cycle_reward_id);

[MessagePackObject(true)]
public record EventDamageRanking(
    int event_id,
    IEnumerable<AtgenOwnDamageRankingList> own_damage_ranking_list
);

[MessagePackObject(true)]
public record EventPassiveList(
    int event_id,
    IEnumerable<AtgenEventPassiveUpList> event_passive_grow_list
);

[MessagePackObject(true)]
public record EventStoryList(int event_story_id, int state);

[MessagePackObject(true)]
public record EventTradeList(
    int event_trade_id,
    int trade_group_id,
    int tab_group_id,
    int priority,
    int is_lock_view,
    int commence_date,
    int complete_date,
    int reset_type,
    int limit,
    int read_story_count,
    int clear_target_quest_id,
    int destination_entity_type,
    int destination_entity_id,
    int destination_entity_quantity,
    IEnumerable<AtgenBuildEventRewardEntityList> need_entity_list
);

[MessagePackObject(true)]
public record ExchangeTicketList(int exchange_ticket_id, int quantity);

[MessagePackObject(true)]
public record ExHunterEventUserList(
    int event_id,
    int box_summon_point,
    int ex_hunter_point_1,
    int ex_hunter_point_2,
    int ex_hunter_point_3,
    int advent_item_quantity_1,
    int advent_item_quantity_2,
    int ultimate_key_count,
    int exchange_item_1,
    int exchange_item_2
);

[MessagePackObject(true)]
public record ExRushEventUserList(int event_id, int ex_rush_item_1, int ex_rush_item_2);

[MessagePackObject(true)]
public record FortBonusList(
    IEnumerable<AtgenParamBonus> param_bonus,
    IEnumerable<AtgenParamBonus> param_bonus_by_weapon,
    IEnumerable<AtgenElementBonus> element_bonus,
    AtgenAllBonus all_bonus,
    IEnumerable<AtgenElementBonus> chara_bonus_by_album,
    IEnumerable<AtgenDragonBonus> dragon_bonus,
    AtgenDragonTimeBonus dragon_time_bonus,
    IEnumerable<AtgenElementBonus> dragon_bonus_by_album
);

[MessagePackObject(true)]
public record FortDetail(int max_carpenter_count, int carpenter_num, int working_carpenter_num);

[MessagePackObject(true)]
public record FortPlantList(int plant_id, int is_new);

[MessagePackObject(true)]
public record FriendNotice(int friend_new_count, int apply_new_count);

[MessagePackObject(true)]
public record FunctionalMaintenanceList(int functional_maintenance_type);

[MessagePackObject(true)]
public record GameAbilityCrest(
    int ability_crest_id,
    int buildup_count,
    int limit_break_count,
    int equipable_count,
    int ability_1_level,
    int ability_2_level,
    int hp_plus_count,
    int attack_plus_count
);

[MessagePackObject(true)]
public record GameWeaponBody(
    int weapon_body_id,
    int buildup_count,
    int limit_break_count,
    int limit_over_count,
    int skill_no,
    int skill_level,
    int ability_1_level,
    int ability_2_level,
    int equipable_count,
    int additional_crest_slot_type_1_count,
    int additional_crest_slot_type_2_count,
    int additional_crest_slot_type_3_count,
    int additional_effect_count
);

[MessagePackObject(true)]
public record GameWeaponSkin(int weapon_skin_id);

[MessagePackObject(true)]
public record GatherItemList(
    int gather_item_id,
    int quantity,
    int quest_take_weekly_quantity,
    int quest_last_weekly_reset_time
);

[MessagePackObject(true)]
public record GrowMaterialList(int type, ulong id, int quantity);

[MessagePackObject(true)]
public record GrowRecord(
    int take_player_exp,
    int take_chara_exp,
    int take_mana,
    float bonus_factor,
    float mana_bonus_factor,
    IEnumerable<AtgenCharaGrowRecord> chara_grow_record,
    IEnumerable<CharaFriendshipList> chara_friendship_list
);

[MessagePackObject(true)]
public record GuildApplyList(
    ulong viewer_id,
    string user_name,
    int user_level,
    int max_party_power,
    int profile_entity_type,
    int profile_entity_id,
    int profile_entity_rarity,
    ulong guild_apply_id,
    int last_active_time
);

[MessagePackObject(true)]
public record GuildChatMessageList(
    ulong chat_message_id,
    ulong viewer_id,
    string user_name,
    int profile_entity_type,
    int profile_entity_id,
    int profile_entity_rarity,
    int chat_message_type,
    string chat_message_text,
    int chat_message_stamp_id,
    int chat_message_system_message_id,
    int chat_message_param_value_1,
    int chat_message_param_value_2,
    int chat_message_param_value_3,
    int chat_message_param_value_4,
    int create_time
);

[MessagePackObject(true)]
public record GuildData(
    int guild_id,
    string guild_name,
    int guild_emblem_id,
    string guild_introduction,
    int joining_condition_type,
    int activity_policy_type,
    string guild_board,
    int guild_member_count,
    int is_penalty_guild_name,
    int is_penalty_guild_introduction,
    int is_penalty_guild_board
);

[MessagePackObject(true)]
public record GuildInviteReceiveList(
    ulong guild_invite_id,
    ulong send_viewer_id,
    string send_user_name,
    int send_max_party_power,
    int send_profile_entity_type,
    int send_profile_entity_id,
    int send_profile_entity_rarity,
    int send_last_active_time,
    int guild_invite_message_id,
    GuildData guild_data
);

[MessagePackObject(true)]
public record GuildInviteSendList(
    ulong guild_invite_id,
    ulong send_viewer_id,
    string send_user_name,
    ulong receive_viewer_id,
    string receive_user_name,
    int receive_user_level,
    int receive_max_party_power,
    int receive_profile_entity_type,
    int receive_profile_entity_id,
    int receive_profile_entity_rarity,
    int receive_last_active_time,
    int guild_invite_message_id,
    int limit_time
);

[MessagePackObject(true)]
public record GuildMemberList(
    ulong viewer_id,
    string user_name,
    int user_level,
    int max_party_power,
    int profile_entity_type,
    int profile_entity_id,
    int profile_entity_rarity,
    int last_active_time,
    int last_guild_active_time,
    int last_attend_time,
    int guild_position_type,
    int temporary_end_time
);

[MessagePackObject(true)]
public record GuildNotice(
    int guild_apply_count,
    int is_update_guild_board,
    int is_update_guild_apply_reply,
    int is_update_guild,
    int is_update_guild_invite
);

[MessagePackObject(true)]
public record IngameData(
    ulong viewer_id,
    string dungeon_key,
    int dungeon_type,
    int play_type,
    int quest_id,
    int bonus_type,
    int continue_limit,
    int continue_count,
    int reborn_limit,
    int start_time,
    PartyInfo party_info,
    IEnumerable<AreaInfoList> area_info_list,
    int use_stone,
    int is_host,
    int is_fever_time,
    int is_bot_tutorial,
    int is_receivable_carry_bonus,
    int is_use_event_chara_ability,
    IEnumerable<EventAbilityCharaList> event_ability_chara_list,
    IEnumerable<ulong> first_clear_viewer_id_list,
    int multi_disconnect_type,
    int repeat_state,
    AtgenIngameWalker ingame_walker
);

[MessagePackObject(true)]
public record IngameQuestData(
    int quest_id,
    int play_count,
    int is_mission_clear_1,
    int is_mission_clear_2,
    int is_mission_clear_3
);

[MessagePackObject(true)]
public record IngameResultData(
    string dungeon_key,
    int play_type,
    int quest_id,
    RewardRecord reward_record,
    GrowRecord grow_record,
    int start_time,
    int end_time,
    int is_clear,
    int state,
    int dungeon_skip_type,
    int is_host,
    int is_fever_time,
    int wave_count,
    int current_play_count,
    int reborn_count,
    IEnumerable<PartySettingList> quest_party_setting_list,
    IEnumerable<UserSupportList> helper_list,
    IEnumerable<AtgenScoringEnemyPointList> scoring_enemy_point_list,
    IEnumerable<AtgenHelperDetailList> helper_detail_list,
    IEnumerable<AtgenScoreMissionSuccessList> score_mission_success_list,
    IEnumerable<AtgenBonusFactorList> bonus_factor_list,
    IEnumerable<AtgenEventPassiveUpList> event_passive_up_list,
    float clear_time,
    int is_best_clear_time,
    long total_play_damage,
    IEnumerable<ConvertedEntityList> converted_entity_list
);

[MessagePackObject(true)]
public record IngameWallData(int wall_id, int wall_level);

[MessagePackObject(true)]
public record ItemList(int item_id, int quantity);

[MessagePackObject(true)]
public record LimitBreakGrowList(int limit_break_count, int limit_break_item_type, ulong target_id);

[MessagePackObject(true)]
public record LotteryOddsRate(
    IEnumerable<AtgenLotteryPrizeRankList> lottery_prize_rank_list,
    IEnumerable<AtgenLotteryEntitySetList> lottery_entity_set_list
);

[MessagePackObject(true)]
public record LotteryOddsRateList(LotteryOddsRate normal, LotteryOddsRate guarantee);

[MessagePackObject(true)]
public record LotteryTicketList(int lottery_ticket_id, int quantity);

[MessagePackObject(true)]
public record MainStoryMissionList(
    int main_story_mission_id,
    int progress,
    int state,
    int end_date,
    int start_date
);

[MessagePackObject(true)]
public record MaterialList(int material_id, int quantity);

[MessagePackObject(true)]
public record MazeEventUserList(
    int event_id,
    IEnumerable<AtgenUserMazeEventItemList> user_maze_event_item_list
);

[MessagePackObject(true)]
public record MemoryEventMissionList(
    int memory_event_mission_id,
    int progress,
    int state,
    int end_date,
    int start_date
);

[MessagePackObject(true)]
public record MissionNotice(
    AtgenNormalMissionNotice normal_mission_notice,
    AtgenNormalMissionNotice daily_mission_notice,
    AtgenNormalMissionNotice period_mission_notice,
    AtgenNormalMissionNotice beginner_mission_notice,
    AtgenNormalMissionNotice special_mission_notice,
    AtgenNormalMissionNotice main_story_mission_notice,
    AtgenNormalMissionNotice memory_event_mission_notice,
    AtgenNormalMissionNotice drill_mission_notice,
    AtgenNormalMissionNotice album_mission_notice
);

[MessagePackObject(true)]
public record MuseumDragonList(int state, int dragon_id);

[MessagePackObject(true)]
public record MuseumList(int state, Charas chara_id);

[MessagePackObject(true)]
public record NormalMissionList(
    int normal_mission_id,
    int progress,
    int state,
    int end_date,
    int start_date
);

[MessagePackObject(true)]
public record OddsInfo(
    int area_index,
    int reaction_obj_count,
    IEnumerable<AtgenDropObj> drop_obj,
    IEnumerable<AtgenEnemy> enemy,
    IEnumerable<AtgenGrade> grade
);

[MessagePackObject(true)]
public record OddsRate(
    IEnumerable<AtgenRarityList> rarity_list,
    IEnumerable<AtgenRarityGroupList> rarity_group_list,
    AtgenUnit unit
);

[MessagePackObject(true)]
public record OddsRateList(int required_count_to_next, OddsRate normal, OddsRate guarantee);

[MessagePackObject(true)]
public record OddsUnitDetail(bool pickup, int rarity, IEnumerable<AtgenUnitList> unit_list);

[MessagePackObject(true)]
public record OptionData(
    int is_enable_auto_lock_unit,
    int is_auto_lock_dragon_sr,
    int is_auto_lock_dragon_ssr,
    int is_auto_lock_weapon_sr,
    int is_auto_lock_weapon_ssr,
    int is_auto_lock_weapon_sssr,
    int is_auto_lock_amulet_sr,
    int is_auto_lock_amulet_ssr
);

[MessagePackObject(true)]
public record PartyInfo(
    IEnumerable<PartyUnitList> party_unit_list,
    FortBonusList fort_bonus_list,
    AtgenEventBoost event_boost,
    AtgenSupportData support_data,
    IEnumerable<AtgenEventPassiveUpList> event_passive_grow_list
);

[MessagePackObject(true)]
public record PartyList(
    int party_no,
    string party_name,
    IEnumerable<PartySettingList> party_setting_list
);

[MessagePackObject(true)]
public record PartyPowerData(int max_party_power);

[MessagePackObject(true)]
public record PartySettingList(
    int unit_no,
    Charas chara_id,
    ulong equip_weapon_key_id,
    ulong equip_dragon_key_id,
    ulong equip_amulet_key_id,
    ulong equip_amulet_2_key_id,
    int equip_skin_weapon_id,
    int equip_weapon_body_id,
    int equip_weapon_skin_id,
    int equip_crest_slot_type_1_crest_id_1,
    int equip_crest_slot_type_1_crest_id_2,
    int equip_crest_slot_type_1_crest_id_3,
    int equip_crest_slot_type_2_crest_id_1,
    int equip_crest_slot_type_2_crest_id_2,
    int equip_crest_slot_type_3_crest_id_1,
    int equip_crest_slot_type_3_crest_id_2,
    ulong equip_talisman_key_id,
    int edit_skill_1_chara_id,
    int edit_skill_2_chara_id
);

[MessagePackObject(true)]
public record PartyUnitList(
    int position,
    CharaList chara_data,
    DragonList dragon_data,
    GameWeaponSkin weapon_skin_data,
    GameWeaponBody weapon_body_data,
    IEnumerable<GameAbilityCrest> crest_slot_type_1_crest_list,
    IEnumerable<GameAbilityCrest> crest_slot_type_2_crest_list,
    IEnumerable<GameAbilityCrest> crest_slot_type_3_crest_list,
    TalismanList talisman_data,
    EditSkillCharaData edit_skill_1_chara_data,
    EditSkillCharaData edit_skill_2_chara_data,
    IEnumerable<WeaponPassiveAbilityList> game_weapon_passive_ability_list,
    int dragon_reliability_level
);

[MessagePackObject(true)]
public record PaymentTarget(int target_hold_quantity, int target_cost);

[MessagePackObject(true)]
public record PeriodMissionList(
    int period_mission_id,
    int progress,
    int state,
    int end_date,
    int start_date
);

[MessagePackObject(true)]
public record PlayRecord(
    IEnumerable<AtgenTreasureRecord> treasure_record,
    float time,
    int down_count,
    int trap_count,
    int bad_status,
    int dragon_pillar_count,
    int dragon_transform_count,
    int damage_count,
    int skill_count,
    int guard_broken_count,
    int break_count,
    int give_damage,
    int max_combo_count,
    int is_clear,
    int clear_state,
    int wave,
    int reaction_touch_cnt,
    int grade_point,
    int reborn_count,
    int visit_private_house,
    int protection_damage,
    int remaining_time,
    int lower_drawbridge_count,
    IEnumerable<int> live_unit_no_list,
    long total_play_damage,
    IEnumerable<AtgenDamageRecord> damage_record,
    IEnumerable<AtgenDamageRecord> dragon_damage_record,
    AtgenBattleRoyalRecord battle_royal_record,
    int max_damage,
    int max_critical_damage,
    int dps,
    int play_continue_count
);

[MessagePackObject(true)]
public record PresentDetailList(
    ulong present_id,
    int master_id,
    int state,
    int entity_type,
    int entity_id,
    int entity_quantity,
    int entity_level,
    int entity_limit_break_count,
    int entity_status_plus_count,
    int message_id,
    int message_param_value_1,
    int message_param_value_2,
    int message_param_value_3,
    int message_param_value_4,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset receive_limit_time,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset create_time
);

[MessagePackObject(true)]
public record PresentHistoryList(
    ulong id,
    int entity_type,
    int entity_id,
    int entity_quantity,
    int entity_level,
    int entity_limit_break_count,
    int entity_status_plus_count,
    int message_id,
    int message_param_value_1,
    int message_param_value_2,
    int message_param_value_3,
    int message_param_value_4,
    int create_time
);

[MessagePackObject(true)]
public record PresentNotice(int present_limit_count, int present_count);

[MessagePackObject(true)]
public record ProductList(
    int id,
    string sku,
    int paid_diamond_quantity,
    int free_diamond_quantity,
    int price
);

[MessagePackObject(true)]
public record QuestCarryList(int quest_carry_id, int receive_count, int last_receive_time);

[MessagePackObject(true)]
public record QuestEntryConditionList(int quest_entry_condition_id);

[MessagePackObject(true)]
public record QuestEventList(
    int quest_event_id,
    int daily_play_count,
    int weekly_play_count,
    int quest_bonus_receive_count,
    int quest_bonus_stack_count,
    int quest_bonus_stack_time,
    int quest_bonus_reserve_count,
    int quest_bonus_reserve_time,
    int last_daily_reset_time,
    int last_weekly_reset_time
);

[MessagePackObject(true)]
public record QuestEventScheduleList(
    int quest_group_id,
    int event_schedule_type,
    int start_date,
    int end_date,
    int interval_type,
    string fever_time_start_1,
    string fever_time_end_1,
    string fever_time_start_2,
    string fever_time_end_2,
    string fever_time_start_3,
    string fever_time_end_3
);

[MessagePackObject(true)]
public record QuestList(
    int quest_id,
    int state,
    int is_mission_clear_1,
    int is_mission_clear_2,
    int is_mission_clear_3,
    int daily_play_count,
    int weekly_play_count,
    int play_count,
    int last_daily_reset_time,
    int last_weekly_reset_time,
    int is_appear,
    float best_clear_time
);

[MessagePackObject(true)]
public record QuestScheduleDetailList(
    int schedule_detail_id,
    int schedule_group_id,
    int drop_bonus_percent,
    int limit_shop_goods_type,
    int interval_type,
    int start_date,
    int end_date
);

[MessagePackObject(true)]
public record QuestStoryList(int quest_story_id, int state);

[MessagePackObject(true)]
public record QuestTreasureList(int quest_treasure_id);

[MessagePackObject(true)]
public record QuestWallList(int wall_id, int wall_level, int is_start_next_level);

[MessagePackObject(true)]
public record RaidEventRewardList(int raid_event_id, int raid_event_reward_id);

[MessagePackObject(true)]
public record RaidEventUserList(
    int raid_event_id,
    int box_summon_point,
    int raid_point_1,
    int raid_point_2,
    int raid_point_3,
    int advent_item_quantity_1,
    int advent_item_quantity_2,
    int ultimate_key_count,
    int exchange_item_count,
    int exchange_item_count_2
);

[MessagePackObject(true)]
public record RankingTierRewardList(int ranking_group_id, int tier_reward_id);

[MessagePackObject(true)]
public record RedoableSummonOddsRateList(OddsRate normal, OddsRate guarantee);

[MessagePackObject(true)]
public record RepeatData(string repeat_key, int repeat_count, int repeat_state);

[MessagePackObject(true)]
public record RepeatSetting(int repeat_type, int repeat_count, IEnumerable<int> use_item_list);

[MessagePackObject(true)]
public record ResponseCommon(DataHeader data_headers);

[MessagePackObject(true)]
public record RewardRecord(
    IEnumerable<AtgenDropAll> drop_all,
    IEnumerable<AtgenFirstClearSet> first_clear_set,
    IEnumerable<AtgenFirstClearSet> mission_complete,
    IEnumerable<AtgenMissionsClearSet> missions_clear_set,
    IEnumerable<AtgenFirstClearSet> quest_bonus_list,
    IEnumerable<AtgenFirstClearSet> challenge_quest_bonus_list,
    IEnumerable<AtgenFirstClearSet> campaign_extra_reward_list,
    IEnumerable<AtgenEnemyPiece> enemy_piece,
    AtgenFirstMeeting first_meeting,
    IEnumerable<AtgenFirstClearSet> carry_bonus,
    IEnumerable<AtgenFirstClearSet> reborn_bonus,
    IEnumerable<AtgenFirstClearSet> weekly_limit_reward_list,
    int take_coin,
    float shop_quest_bonus_factor,
    int player_level_up_fstone,
    int take_accumulate_point,
    int take_boost_accumulate_point,
    int take_astral_item_quantity
);

[MessagePackObject(true)]
public record RewardReliabilityList(
    IEnumerable<DragonRewardEntityList> levelup_entity_list,
    int level,
    int is_release_story
);

[MessagePackObject(true)]
public record RoomList(
    int room_id,
    string room_name,
    string region,
    string cluster_name,
    string language,
    int status,
    int entry_type,
    int entry_guild_id,
    ulong host_viewer_id,
    string host_name,
    int host_level,
    int leader_chara_id,
    int leader_chara_level,
    int leader_chara_rarity,
    int quest_id,
    int quest_type,
    IEnumerable<AtgenRoomMemberList> room_member_list,
    int start_entry_time,
    int member_num,
    AtgenEntryConditions entry_conditions,
    int compatible_id
);

[MessagePackObject(true)]
public record SearchClearPartyCharaList(
    int quest_id,
    IEnumerable<AtgenArchivePartyCharaList> archive_party_chara_list
);

[MessagePackObject(true)]
public record SearchClearPartyList(IEnumerable<AtgenArchivePartyUnitList> archive_party_unit_list);

[MessagePackObject(true)]
public record SettingSupport(
    Charas chara_id,
    ulong equip_dragon_key_id,
    ulong equip_weapon_key_id,
    ulong equip_amulet_key_id,
    ulong equip_amulet_2_key_id,
    int equip_weapon_body_id,
    int equip_crest_slot_type_1_crest_id_1,
    int equip_crest_slot_type_1_crest_id_2,
    int equip_crest_slot_type_1_crest_id_3,
    int equip_crest_slot_type_2_crest_id_1,
    int equip_crest_slot_type_2_crest_id_2,
    int equip_crest_slot_type_3_crest_id_1,
    int equip_crest_slot_type_3_crest_id_2,
    ulong equip_talisman_key_id
);

[MessagePackObject(true)]
public record ShopNotice(int is_shop_notification);

[MessagePackObject(true)]
public record ShopPurchaseList(
    int goods_id,
    int last_buy_time,
    int effect_start_time,
    int effect_end_time,
    int buy_count
);

[MessagePackObject(true)]
public record SimpleEventUserList(
    int event_id,
    int simple_event_item_1,
    int simple_event_item_2,
    int simple_event_item_3
);

[MessagePackObject(true)]
public record SkinWeaponData(int weapon_id);

[MessagePackObject(true)]
public record SpecialMissionList(
    int special_mission_id,
    int progress,
    int state,
    int end_date,
    int start_date
);

[MessagePackObject(true)]
public record StampList(int stamp_id, int is_new);

[MessagePackObject(true)]
public record SummonHistoryList(
    int key_id,
    int summon_id,
    int summon_exec_type,
    int exec_date,
    int payment_type,
    int entity_type,
    int entity_id,
    int entity_quantity,
    int entity_level,
    int entity_rarity,
    int entity_limit_break_count,
    int entity_hp_plus_count,
    int entity_attack_plus_count,
    int summon_prize_rank,
    int summon_point_id,
    int summon_point,
    int get_dew_point_quantity
);

[MessagePackObject(true)]
public record SummonList(
    int summon_id,
    int summon_type,
    int summon_group_id,
    int single_crystal,
    int single_diamond,
    int multi_crystal,
    int multi_diamond,
    int limited_crystal,
    int limited_diamond,
    int summon_point_id,
    int add_summon_point,
    int add_summon_point_stone,
    int exchange_summon_point,
    int status,
    int commence_date,
    int complete_date,
    int daily_count,
    int daily_limit,
    int total_count,
    int total_limit,
    int campaign_type,
    int free_count_rest,
    int is_beginner_campaign,
    int beginner_campaign_count_rest,
    int consecution_campaign_count_rest
);

[MessagePackObject(true)]
public record SummonPointList(
    int summon_point_id,
    int summon_point,
    int cs_summon_point,
    int cs_point_term_min_date,
    int cs_point_term_max_date
);

[MessagePackObject(true)]
public record SummonPrizeOddsRate(
    IEnumerable<AtgenSummonPrizeRankList> summon_prize_rank_list,
    IEnumerable<AtgenSummonPrizeEntitySetList> summon_prize_entity_set_list
);

[MessagePackObject(true)]
public record SummonPrizeOddsRateList(SummonPrizeOddsRate normal, SummonPrizeOddsRate guarantee);

[MessagePackObject(true)]
public record SummonTicketList(
    ulong key_id,
    int summon_ticket_id,
    int quantity,
    int use_limit_time
);

[MessagePackObject(true)]
public record TalismanList(
    ulong talisman_key_id,
    int talisman_id,
    int talisman_ability_id_1,
    int talisman_ability_id_2,
    int talisman_ability_id_3,
    int additional_hp,
    int additional_attack,
    int is_new,
    int is_lock,
    int gettime
);

[MessagePackObject(true)]
public record TimeAttackRankingData(
    int ranking_id,
    IEnumerable<AtgenOwnRankingList> own_ranking_list
);

[MessagePackObject(true)]
public record TreasureTradeList(
    int priority,
    int treasure_trade_id,
    int tab_group_id,
    int is_lock_view,
    int commence_date,
    int complete_date,
    int reset_type,
    int limit,
    IEnumerable<AtgenNeedTradeEntityList> need_trade_entity_list,
    int destination_entity_type,
    int destination_entity_id,
    int destination_entity_quantity,
    int destination_limit_break_count
);

[MessagePackObject(true)]
public record UnitStoryList(int unit_story_id, int is_read);

[MessagePackObject(true)]
public record UnusedType;

[MessagePackObject(true)]
public record UserAbilityCrestTradeList(int ability_crest_trade_id, int trade_count);

[MessagePackObject(true)]
public record UserAmuletTradeList(int amulet_trade_id, int trade_count);

[MessagePackObject(true)]
public record UserData(
    ulong viewer_id,
    string name,
    int level,
    int exp,
    long coin,
    int crystal,
    int dew_point,
    int stamina_single,
    int last_stamina_single_update_time,
    int stamina_single_surplus_second,
    int stamina_multi,
    int last_stamina_multi_update_time,
    int stamina_multi_surplus_second,
    int max_dragon_quantity,
    int max_weapon_quantity,
    int max_amulet_quantity,
    int quest_skip_point,
    int build_time_point,
    int age_group,
    int main_party_no,
    int emblem_id,
    int active_memory_event_id,
    int mana_point,
    int last_login_time,
    int tutorial_status,
    IEnumerable<int> tutorial_flag_list,
    int prologue_end_time,
    int fort_open_time,
    int create_time,
    int is_optin
);

[MessagePackObject(true)]
public record UserEventItemData(
    IEnumerable<AtgenUserMazeEventItemList__2> user_maze_event_item_list
);

[MessagePackObject(true)]
public record UserEventLocationRewardList(int event_id, int location_reward_id);

[MessagePackObject(true)]
public record UserGuildData(
    int guild_id,
    ulong guild_apply_id,
    int penalty_end_time,
    int guild_push_notification_type_running,
    int guild_push_notification_type_suspending,
    int profile_entity_type,
    int profile_entity_id,
    int profile_entity_rarity,
    int is_enable_invite_receive,
    int is_enable_invite_send,
    int last_attend_time
);

[MessagePackObject(true)]
public record UserRedoableSummonData(
    int is_fixed_result,
    IEnumerable<AtgenRedoableSummonResultUnitList> redoable_summon_result_unit_list
);

[MessagePackObject(true)]
public record UserSummonList(
    int summon_id,
    int summon_count,
    int campaign_type,
    int free_count_rest,
    int is_beginner_campaign,
    int beginner_campaign_count_rest,
    int consecution_campaign_count_rest
);

[MessagePackObject(true)]
public record UserSupportList(
    ulong viewer_id,
    string name,
    int level,
    int last_login_date,
    int emblem_id,
    int max_party_power,
    AtgenGuild guild,
    AtgenSupportChara support_chara,
    AtgenSupportWeapon support_weapon,
    AtgenSupportDragon support_dragon,
    AtgenSupportWeaponBody support_weapon_body,
    IEnumerable<AtgenSupportCrestSlotType1List> support_crest_slot_type_1_list,
    IEnumerable<AtgenSupportCrestSlotType1List> support_crest_slot_type_2_list,
    IEnumerable<AtgenSupportCrestSlotType1List> support_crest_slot_type_3_list,
    AtgenSupportTalisman support_talisman,
    AtgenSupportAmulet support_amulet,
    AtgenSupportAmulet support_amulet_2
);

[MessagePackObject(true)]
public record UserTreasureTradeList(int treasure_trade_id, int trade_count, int last_reset_time);

[MessagePackObject(true)]
public record WalletBalance(int total, int free, IEnumerable<AtgenPaid> paid);

[MessagePackObject(true)]
public record WeaponBodyList(
    int weapon_body_id,
    int buildup_count,
    int limit_break_count,
    int limit_over_count,
    int equipable_count,
    int additional_crest_slot_type_1_count,
    int additional_crest_slot_type_2_count,
    int additional_crest_slot_type_3_count,
    int additional_effect_count,
    IEnumerable<int> unlock_weapon_passive_ability_no_list,
    int fort_passive_chara_weapon_buildup_count,
    int is_new,
    int gettime,
    int skill_n,
    int skill_leve,
    int ability_1_leve,
    int ability_2_level
);

[MessagePackObject(true)]
public record WeaponList(
    int weapon_id,
    ulong weapon_key_id,
    int is_lock,
    int is_new,
    int gettime,
    int skill_level,
    int hp_plus_count,
    int attack_plus_count,
    int status_plus_count,
    int level,
    int limit_break_count,
    int exp
);

[MessagePackObject(true)]
public record WeaponPassiveAbilityList(int weapon_passive_ability_id);

[MessagePackObject(true)]
public record WeaponSkinList(int weapon_skin_id, int is_new, int gettime);
