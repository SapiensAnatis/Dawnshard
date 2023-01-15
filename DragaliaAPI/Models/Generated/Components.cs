#nullable disable

using System.Text.Json.Serialization;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;
using MessagePack;

namespace DragaliaAPI.Models.Generated;

[MessagePackObject(true)]
public class AbilityCrestList
{
    public int ability_crest_id { get; set; }
    public int buildup_count { get; set; }
    public int limit_break_count { get; set; }
    public int equipable_count { get; set; }
    public int hp_plus_count { get; set; }
    public int attack_plus_count { get; set; }
    public int is_favorite { get; set; }
    public int is_new { get; set; }
    public DateTimeOffset gettime { get; set; }
    public int ability_1_level { get; set; }
    public int ability_2_level { get; set; }

    public AbilityCrestList(
        int ability_crest_id,
        int buildup_count,
        int limit_break_count,
        int equipable_count,
        int hp_plus_count,
        int attack_plus_count,
        int is_favorite,
        int is_new,
        DateTimeOffset gettime,
        int ability_1_level,
        int ability_2_level
    )
    {
        this.ability_crest_id = ability_crest_id;
        this.buildup_count = buildup_count;
        this.limit_break_count = limit_break_count;
        this.equipable_count = equipable_count;
        this.hp_plus_count = hp_plus_count;
        this.attack_plus_count = attack_plus_count;
        this.is_favorite = is_favorite;
        this.is_new = is_new;
        this.gettime = gettime;
        this.ability_1_level = ability_1_level;
        this.ability_2_level = ability_2_level;
    }

    public AbilityCrestList() { }
}

[MessagePackObject(true)]
public class AbilityCrestSetList
{
    public int ability_crest_set_no { get; set; }
    public string ability_crest_set_name { get; set; }
    public int crest_slot_type_1_crest_id_1 { get; set; }
    public int crest_slot_type_1_crest_id_2 { get; set; }
    public int crest_slot_type_1_crest_id_3 { get; set; }
    public int crest_slot_type_2_crest_id_1 { get; set; }
    public int crest_slot_type_2_crest_id_2 { get; set; }
    public int crest_slot_type_3_crest_id_1 { get; set; }
    public int crest_slot_type_3_crest_id_2 { get; set; }
    public ulong talisman_key_id { get; set; }

    public AbilityCrestSetList(
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
    )
    {
        this.ability_crest_set_no = ability_crest_set_no;
        this.ability_crest_set_name = ability_crest_set_name;
        this.crest_slot_type_1_crest_id_1 = crest_slot_type_1_crest_id_1;
        this.crest_slot_type_1_crest_id_2 = crest_slot_type_1_crest_id_2;
        this.crest_slot_type_1_crest_id_3 = crest_slot_type_1_crest_id_3;
        this.crest_slot_type_2_crest_id_1 = crest_slot_type_2_crest_id_1;
        this.crest_slot_type_2_crest_id_2 = crest_slot_type_2_crest_id_2;
        this.crest_slot_type_3_crest_id_1 = crest_slot_type_3_crest_id_1;
        this.crest_slot_type_3_crest_id_2 = crest_slot_type_3_crest_id_2;
        this.talisman_key_id = talisman_key_id;
    }

    public AbilityCrestSetList() { }
}

[MessagePackObject(true)]
public class AbilityCrestTradeList
{
    public int ability_crest_trade_id { get; set; }
    public int ability_crest_id { get; set; }
    public int need_dew_point { get; set; }
    public int priority { get; set; }
    public int complete_date { get; set; }
    public int pickup_view_start_date { get; set; }
    public int pickup_view_end_date { get; set; }

    public AbilityCrestTradeList(
        int ability_crest_trade_id,
        int ability_crest_id,
        int need_dew_point,
        int priority,
        int complete_date,
        int pickup_view_start_date,
        int pickup_view_end_date
    )
    {
        this.ability_crest_trade_id = ability_crest_trade_id;
        this.ability_crest_id = ability_crest_id;
        this.need_dew_point = need_dew_point;
        this.priority = priority;
        this.complete_date = complete_date;
        this.pickup_view_start_date = pickup_view_start_date;
        this.pickup_view_end_date = pickup_view_end_date;
    }

    public AbilityCrestTradeList() { }
}

[MessagePackObject(true)]
public class AchievementList
{
    public int achievement_id { get; set; }
    public int progress { get; set; }
    public int state { get; set; }

    public AchievementList(int achievement_id, int progress, int state)
    {
        this.achievement_id = achievement_id;
        this.progress = progress;
        this.state = state;
    }

    public AchievementList() { }
}

[MessagePackObject(true)]
public class AlbumDragonData
{
    public int dragon_id { get; set; }
    public int max_level { get; set; }
    public int max_limit_break_count { get; set; }

    public AlbumDragonData(int dragon_id, int max_level, int max_limit_break_count)
    {
        this.dragon_id = dragon_id;
        this.max_level = max_level;
        this.max_limit_break_count = max_limit_break_count;
    }

    public AlbumDragonData() { }
}

[MessagePackObject(true)]
public class AlbumMissionList
{
    public int album_mission_id { get; set; }
    public int progress { get; set; }
    public int state { get; set; }
    public int end_date { get; set; }
    public int start_date { get; set; }

    public AlbumMissionList(
        int album_mission_id,
        int progress,
        int state,
        int end_date,
        int start_date
    )
    {
        this.album_mission_id = album_mission_id;
        this.progress = progress;
        this.state = state;
        this.end_date = end_date;
        this.start_date = start_date;
    }

    public AlbumMissionList() { }
}

[MessagePackObject(true)]
public class AlbumPassiveNotice
{
    public int is_update_chara { get; set; }
    public int is_update_dragon { get; set; }

    public AlbumPassiveNotice(int is_update_chara, int is_update_dragon)
    {
        this.is_update_chara = is_update_chara;
        this.is_update_dragon = is_update_dragon;
    }

    public AlbumPassiveNotice() { }
}

[MessagePackObject(true)]
public class AlbumWeaponList
{
    public int weapon_id { get; set; }
    public int gettime { get; set; }

    public AlbumWeaponList(int weapon_id, int gettime)
    {
        this.weapon_id = weapon_id;
        this.gettime = gettime;
    }

    public AlbumWeaponList() { }
}

[MessagePackObject(true)]
public class AmuletList
{
    public int amulet_id { get; set; }
    public ulong amulet_key_id { get; set; }
    public int is_lock { get; set; }
    public int is_new { get; set; }
    public int gettime { get; set; }
    public int ability_1_level { get; set; }
    public int ability_2_level { get; set; }
    public int ability_3_level { get; set; }
    public int hp_plus_count { get; set; }
    public int attack_plus_count { get; set; }
    public int status_plus_count { get; set; }
    public int level { get; set; }
    public int limit_break_count { get; set; }
    public int exp { get; set; }

    public AmuletList(
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
    )
    {
        this.amulet_id = amulet_id;
        this.amulet_key_id = amulet_key_id;
        this.is_lock = is_lock;
        this.is_new = is_new;
        this.gettime = gettime;
        this.ability_1_level = ability_1_level;
        this.ability_2_level = ability_2_level;
        this.ability_3_level = ability_3_level;
        this.hp_plus_count = hp_plus_count;
        this.attack_plus_count = attack_plus_count;
        this.status_plus_count = status_plus_count;
        this.level = level;
        this.limit_break_count = limit_break_count;
        this.exp = exp;
    }

    public AmuletList() { }
}

[MessagePackObject(true)]
public class AmuletTradeList
{
    public int amulet_trade_id { get; set; }
    public int amulet_id { get; set; }
    public int need_dew_point_1 { get; set; }
    public int need_dew_point_2 { get; set; }
    public int need_dew_point_3 { get; set; }
    public int need_dew_point_4 { get; set; }
    public int need_dew_point_5 { get; set; }
    public int priority { get; set; }
    public int limit { get; set; }
    public int complete_date { get; set; }
    public int pickup_view_start_date { get; set; }
    public int pickup_view_end_date { get; set; }

    public AmuletTradeList(
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
    )
    {
        this.amulet_trade_id = amulet_trade_id;
        this.amulet_id = amulet_id;
        this.need_dew_point_1 = need_dew_point_1;
        this.need_dew_point_2 = need_dew_point_2;
        this.need_dew_point_3 = need_dew_point_3;
        this.need_dew_point_4 = need_dew_point_4;
        this.need_dew_point_5 = need_dew_point_5;
        this.priority = priority;
        this.limit = limit;
        this.complete_date = complete_date;
        this.pickup_view_start_date = pickup_view_start_date;
        this.pickup_view_end_date = pickup_view_end_date;
    }

    public AmuletTradeList() { }
}

[MessagePackObject(true)]
public class ApiTest { }

[MessagePackObject(true)]
public class AreaInfoList
{
    public string scene_path { get; set; }
    public string area_name { get; set; }

    public AreaInfoList(string scene_path, string area_name)
    {
        this.scene_path = scene_path;
        this.area_name = area_name;
    }

    public AreaInfoList() { }
}

[MessagePackObject(true)]
public class AstralItemList
{
    public int astral_item_id { get; set; }
    public int quantity { get; set; }

    public AstralItemList(int astral_item_id, int quantity)
    {
        this.astral_item_id = astral_item_id;
        this.quantity = quantity;
    }

    public AstralItemList() { }
}

[MessagePackObject(true)]
public class AtgenAddCoinList
{
    public ulong build_id { get; set; }
    public int add_coin { get; set; }

    public AtgenAddCoinList(ulong build_id, int add_coin)
    {
        this.build_id = build_id;
        this.add_coin = add_coin;
    }

    public AtgenAddCoinList() { }
}

[MessagePackObject(true)]
public class AtgenAddHarvestList
{
    public int material_id { get; set; }
    public int add_num { get; set; }

    public AtgenAddHarvestList(int material_id, int add_num)
    {
        this.material_id = material_id;
        this.add_num = add_num;
    }

    public AtgenAddHarvestList() { }
}

[MessagePackObject(true)]
public class AtgenAddStaminaList
{
    public ulong build_id { get; set; }
    public int add_stamina { get; set; }

    public AtgenAddStaminaList(ulong build_id, int add_stamina)
    {
        this.build_id = build_id;
        this.add_stamina = add_stamina;
    }

    public AtgenAddStaminaList() { }
}

[MessagePackObject(true)]
public class AtgenAlbumQuestPlayRecordList
{
    public int quest_play_record_id { get; set; }
    public int quest_play_record_value { get; set; }

    public AtgenAlbumQuestPlayRecordList(int quest_play_record_id, int quest_play_record_value)
    {
        this.quest_play_record_id = quest_play_record_id;
        this.quest_play_record_value = quest_play_record_value;
    }

    public AtgenAlbumQuestPlayRecordList() { }
}

[MessagePackObject(true)]
public class AtgenAllBonus
{
    public int hp { get; set; }
    public int attack { get; set; }

    public AtgenAllBonus(int hp, int attack)
    {
        this.hp = hp;
        this.attack = attack;
    }

    public AtgenAllBonus() { }
}

[MessagePackObject(true)]
public class AtgenArchivePartyCharaList
{
    public int unit_no { get; set; }
    public int chara_id { get; set; }

    public AtgenArchivePartyCharaList(int unit_no, int chara_id)
    {
        this.unit_no = unit_no;
        this.chara_id = chara_id;
    }

    public AtgenArchivePartyCharaList() { }
}

[MessagePackObject(true)]
public class AtgenArchivePartyUnitList
{
    public int unit_no { get; set; }
    public int chara_id { get; set; }
    public int equip_dragon_id { get; set; }
    public int equip_weapon_body_id { get; set; }
    public int equip_crest_slot_type_1_crest_id_1 { get; set; }
    public int equip_crest_slot_type_1_crest_id_2 { get; set; }
    public int equip_crest_slot_type_1_crest_id_3 { get; set; }
    public int equip_crest_slot_type_2_crest_id_1 { get; set; }
    public int equip_crest_slot_type_2_crest_id_2 { get; set; }
    public int equip_crest_slot_type_3_crest_id_1 { get; set; }
    public int equip_crest_slot_type_3_crest_id_2 { get; set; }
    public int equip_talisman_id { get; set; }
    public int equip_talisman_ability_id_1 { get; set; }
    public int equip_talisman_ability_id_2 { get; set; }
    public int equip_talisman_ability_id_3 { get; set; }
    public int edit_skill_1_chara_id { get; set; }
    public int edit_skill_2_chara_id { get; set; }
    public int ex_ability_1_chara_id { get; set; }
    public int ex_ability_2_chara_id { get; set; }
    public int ex_ability_3_chara_id { get; set; }
    public int ex_ability_4_chara_id { get; set; }

    public AtgenArchivePartyUnitList(
        int unit_no,
        int chara_id,
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
    )
    {
        this.unit_no = unit_no;
        this.chara_id = chara_id;
        this.equip_dragon_id = equip_dragon_id;
        this.equip_weapon_body_id = equip_weapon_body_id;
        this.equip_crest_slot_type_1_crest_id_1 = equip_crest_slot_type_1_crest_id_1;
        this.equip_crest_slot_type_1_crest_id_2 = equip_crest_slot_type_1_crest_id_2;
        this.equip_crest_slot_type_1_crest_id_3 = equip_crest_slot_type_1_crest_id_3;
        this.equip_crest_slot_type_2_crest_id_1 = equip_crest_slot_type_2_crest_id_1;
        this.equip_crest_slot_type_2_crest_id_2 = equip_crest_slot_type_2_crest_id_2;
        this.equip_crest_slot_type_3_crest_id_1 = equip_crest_slot_type_3_crest_id_1;
        this.equip_crest_slot_type_3_crest_id_2 = equip_crest_slot_type_3_crest_id_2;
        this.equip_talisman_id = equip_talisman_id;
        this.equip_talisman_ability_id_1 = equip_talisman_ability_id_1;
        this.equip_talisman_ability_id_2 = equip_talisman_ability_id_2;
        this.equip_talisman_ability_id_3 = equip_talisman_ability_id_3;
        this.edit_skill_1_chara_id = edit_skill_1_chara_id;
        this.edit_skill_2_chara_id = edit_skill_2_chara_id;
        this.ex_ability_1_chara_id = ex_ability_1_chara_id;
        this.ex_ability_2_chara_id = ex_ability_2_chara_id;
        this.ex_ability_3_chara_id = ex_ability_3_chara_id;
        this.ex_ability_4_chara_id = ex_ability_4_chara_id;
    }

    public AtgenArchivePartyUnitList() { }
}

[MessagePackObject(true)]
public class AtgenBattleRoyalData
{
    public int event_cycle_id { get; set; }
    public int chara_id { get; set; }
    public int dragon_id { get; set; }
    public int weapon_skin_id { get; set; }
    public int special_skill_id { get; set; }
    public string dungeon_key { get; set; }

    public AtgenBattleRoyalData(
        int event_cycle_id,
        int chara_id,
        int dragon_id,
        int weapon_skin_id,
        int special_skill_id,
        string dungeon_key
    )
    {
        this.event_cycle_id = event_cycle_id;
        this.chara_id = chara_id;
        this.dragon_id = dragon_id;
        this.weapon_skin_id = weapon_skin_id;
        this.special_skill_id = special_skill_id;
        this.dungeon_key = dungeon_key;
    }

    public AtgenBattleRoyalData() { }
}

[MessagePackObject(true)]
public class AtgenBattleRoyalHistoryList
{
    public int id { get; set; }
    public int event_id { get; set; }
    public int chara_id { get; set; }
    public int use_weapon_type { get; set; }
    public int ranking { get; set; }
    public int kill_count { get; set; }
    public int assist_count { get; set; }
    public int battle_royal_point { get; set; }
    public int start_time { get; set; }

    public AtgenBattleRoyalHistoryList(
        int id,
        int event_id,
        int chara_id,
        int use_weapon_type,
        int ranking,
        int kill_count,
        int assist_count,
        int battle_royal_point,
        int start_time
    )
    {
        this.id = id;
        this.event_id = event_id;
        this.chara_id = chara_id;
        this.use_weapon_type = use_weapon_type;
        this.ranking = ranking;
        this.kill_count = kill_count;
        this.assist_count = assist_count;
        this.battle_royal_point = battle_royal_point;
        this.start_time = start_time;
    }

    public AtgenBattleRoyalHistoryList() { }
}

[MessagePackObject(true)]
public class AtgenBattleRoyalRecord
{
    public int ranking { get; set; }
    public int kill_count { get; set; }
    public int assist_count { get; set; }

    public AtgenBattleRoyalRecord(int ranking, int kill_count, int assist_count)
    {
        this.ranking = ranking;
        this.kill_count = kill_count;
        this.assist_count = assist_count;
    }

    public AtgenBattleRoyalRecord() { }
}

[MessagePackObject(true)]
public class AtgenBonusFactorList
{
    public int factor_type { get; set; }
    public float factor_value { get; set; }

    public AtgenBonusFactorList(int factor_type, float factor_value)
    {
        this.factor_type = factor_type;
        this.factor_value = factor_value;
    }

    public AtgenBonusFactorList() { }
}

[MessagePackObject(true)]
public class AtgenBoxSummonData
{
    public int event_id { get; set; }
    public int event_point { get; set; }
    public int box_summon_seq { get; set; }
    public int reset_possible { get; set; }
    public int remaining_quantity { get; set; }
    public int max_exec_count { get; set; }
    public IEnumerable<AtgenBoxSummonDetail> box_summon_detail { get; set; }

    public AtgenBoxSummonData(
        int event_id,
        int event_point,
        int box_summon_seq,
        int reset_possible,
        int remaining_quantity,
        int max_exec_count,
        IEnumerable<AtgenBoxSummonDetail> box_summon_detail
    )
    {
        this.event_id = event_id;
        this.event_point = event_point;
        this.box_summon_seq = box_summon_seq;
        this.reset_possible = reset_possible;
        this.remaining_quantity = remaining_quantity;
        this.max_exec_count = max_exec_count;
        this.box_summon_detail = box_summon_detail;
    }

    public AtgenBoxSummonData() { }
}

[MessagePackObject(true)]
public class AtgenBoxSummonDetail
{
    public int id { get; set; }
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }
    public int entity_quantity { get; set; }
    public int limit { get; set; }
    public int pickup_item_state { get; set; }
    public int reset_item_flag { get; set; }
    public int total_count { get; set; }
    public int two_step_id { get; set; }

    public AtgenBoxSummonDetail(
        int id,
        EntityTypes entity_type,
        int entity_id,
        int entity_quantity,
        int limit,
        int pickup_item_state,
        int reset_item_flag,
        int total_count,
        int two_step_id
    )
    {
        this.id = id;
        this.entity_type = entity_type;
        this.entity_id = entity_id;
        this.entity_quantity = entity_quantity;
        this.limit = limit;
        this.pickup_item_state = pickup_item_state;
        this.reset_item_flag = reset_item_flag;
        this.total_count = total_count;
        this.two_step_id = two_step_id;
    }

    public AtgenBoxSummonDetail() { }
}

[MessagePackObject(true)]
public class AtgenBoxSummonResult
{
    public int event_id { get; set; }
    public int box_summon_seq { get; set; }
    public int reset_possible { get; set; }
    public int remaining_quantity { get; set; }
    public int max_exec_count { get; set; }
    public int is_stopped_by_target { get; set; }
    public IEnumerable<AtgenDrawDetails> draw_details { get; set; }
    public IEnumerable<AtgenBoxSummonDetail> box_summon_detail { get; set; }
    public int event_point { get; set; }

    public AtgenBoxSummonResult(
        int event_id,
        int box_summon_seq,
        int reset_possible,
        int remaining_quantity,
        int max_exec_count,
        int is_stopped_by_target,
        IEnumerable<AtgenDrawDetails> draw_details,
        IEnumerable<AtgenBoxSummonDetail> box_summon_detail,
        int event_point
    )
    {
        this.event_id = event_id;
        this.box_summon_seq = box_summon_seq;
        this.reset_possible = reset_possible;
        this.remaining_quantity = remaining_quantity;
        this.max_exec_count = max_exec_count;
        this.is_stopped_by_target = is_stopped_by_target;
        this.draw_details = draw_details;
        this.box_summon_detail = box_summon_detail;
        this.event_point = event_point;
    }

    public AtgenBoxSummonResult() { }
}

[MessagePackObject(true)]
public class AtgenBuildEventRewardEntityList
{
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }
    public int entity_quantity { get; set; }

    public AtgenBuildEventRewardEntityList(
        EntityTypes entity_type,
        int entity_id,
        int entity_quantity
    )
    {
        this.entity_type = entity_type;
        this.entity_id = entity_id;
        this.entity_quantity = entity_quantity;
    }

    public AtgenBuildEventRewardEntityList() { }
}

[MessagePackObject(true)]
public class AtgenBuildupAbilityCrestPieceList
{
    public int buildup_piece_type { get; set; }
    public int step { get; set; }
    public int is_use_dedicated_material { get; set; }

    public AtgenBuildupAbilityCrestPieceList(
        int buildup_piece_type,
        int step,
        int is_use_dedicated_material
    )
    {
        this.buildup_piece_type = buildup_piece_type;
        this.step = step;
        this.is_use_dedicated_material = is_use_dedicated_material;
    }

    public AtgenBuildupAbilityCrestPieceList() { }
}

[MessagePackObject(true)]
public class AtgenBuildupWeaponBodyPieceList
{
    public int buildup_piece_type { get; set; }
    public int buildup_piece_no { get; set; }
    public int step { get; set; }
    public int is_use_dedicated_material { get; set; }

    public AtgenBuildupWeaponBodyPieceList(
        int buildup_piece_type,
        int buildup_piece_no,
        int step,
        int is_use_dedicated_material
    )
    {
        this.buildup_piece_type = buildup_piece_type;
        this.buildup_piece_no = buildup_piece_no;
        this.step = step;
        this.is_use_dedicated_material = is_use_dedicated_material;
    }

    public AtgenBuildupWeaponBodyPieceList() { }
}

[MessagePackObject(true)]
public class AtgenCategoryList
{
    public int category_id { get; set; }
    public string name { get; set; }

    public AtgenCategoryList(int category_id, string name)
    {
        this.category_id = category_id;
        this.name = name;
    }

    public AtgenCategoryList() { }
}

[MessagePackObject(true)]
public class AtgenCharaGrowRecord
{
    public int chara_id { get; set; }
    public int take_exp { get; set; }

    public AtgenCharaGrowRecord(int chara_id, int take_exp)
    {
        this.chara_id = chara_id;
        this.take_exp = take_exp;
    }

    public AtgenCharaGrowRecord() { }
}

[MessagePackObject(true)]
public class AtgenCharaHonorList
{
    public int chara_id { get; set; }
    public IEnumerable<AtgenHonorList> honor_list { get; set; }

    public AtgenCharaHonorList(int chara_id, IEnumerable<AtgenHonorList> honor_list)
    {
        this.chara_id = chara_id;
        this.honor_list = honor_list;
    }

    public AtgenCharaHonorList() { }
}

[MessagePackObject(true)]
public class AtgenCharaUnitSetDetailList
{
    public int unit_set_no { get; set; }
    public string unit_set_name { get; set; }
    public ulong dragon_key_id { get; set; }
    public int weapon_body_id { get; set; }
    public int crest_slot_type_1_crest_id_1 { get; set; }
    public int crest_slot_type_1_crest_id_2 { get; set; }
    public int crest_slot_type_1_crest_id_3 { get; set; }
    public int crest_slot_type_2_crest_id_1 { get; set; }
    public int crest_slot_type_2_crest_id_2 { get; set; }
    public int crest_slot_type_3_crest_id_1 { get; set; }
    public int crest_slot_type_3_crest_id_2 { get; set; }
    public ulong talisman_key_id { get; set; }

    public AtgenCharaUnitSetDetailList(
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
    )
    {
        this.unit_set_no = unit_set_no;
        this.unit_set_name = unit_set_name;
        this.dragon_key_id = dragon_key_id;
        this.weapon_body_id = weapon_body_id;
        this.crest_slot_type_1_crest_id_1 = crest_slot_type_1_crest_id_1;
        this.crest_slot_type_1_crest_id_2 = crest_slot_type_1_crest_id_2;
        this.crest_slot_type_1_crest_id_3 = crest_slot_type_1_crest_id_3;
        this.crest_slot_type_2_crest_id_1 = crest_slot_type_2_crest_id_1;
        this.crest_slot_type_2_crest_id_2 = crest_slot_type_2_crest_id_2;
        this.crest_slot_type_3_crest_id_1 = crest_slot_type_3_crest_id_1;
        this.crest_slot_type_3_crest_id_2 = crest_slot_type_3_crest_id_2;
        this.talisman_key_id = talisman_key_id;
    }

    public AtgenCharaUnitSetDetailList() { }
}

[MessagePackObject(true)]
public class AtgenCommentList
{
    public string comment_text { get; set; }
    public string author_type { get; set; }
    public int comment_created_at { get; set; }

    public AtgenCommentList(string comment_text, string author_type, int comment_created_at)
    {
        this.comment_text = comment_text;
        this.author_type = author_type;
        this.comment_created_at = comment_created_at;
    }

    public AtgenCommentList() { }
}

[MessagePackObject(true)]
public class AtgenCsSummonList
{
    public IEnumerable<SummonList> summon_list { get; set; }
    public IEnumerable<SummonList> campaign_summon_list { get; set; }
    public IEnumerable<SummonList> campaign_ssr_summon_list { get; set; }
    public IEnumerable<SummonList> platinum_summon_list { get; set; }
    public IEnumerable<SummonList> exclude_summon_list { get; set; }

    public AtgenCsSummonList(
        IEnumerable<SummonList> summon_list,
        IEnumerable<SummonList> campaign_summon_list,
        IEnumerable<SummonList> campaign_ssr_summon_list,
        IEnumerable<SummonList> platinum_summon_list,
        IEnumerable<SummonList> exclude_summon_list
    )
    {
        this.summon_list = summon_list;
        this.campaign_summon_list = campaign_summon_list;
        this.campaign_ssr_summon_list = campaign_ssr_summon_list;
        this.platinum_summon_list = platinum_summon_list;
        this.exclude_summon_list = exclude_summon_list;
    }

    public AtgenCsSummonList() { }
}

[MessagePackObject(true)]
public class AtgenDamageRecord
{
    public int total { get; set; }
    public int skill { get; set; }
    public int dot { get; set; }
    public int critical { get; set; }
    public int enchant { get; set; }

    public AtgenDamageRecord(int total, int skill, int dot, int critical, int enchant)
    {
        this.total = total;
        this.skill = skill;
        this.dot = dot;
        this.critical = critical;
        this.enchant = enchant;
    }

    public AtgenDamageRecord() { }
}

[MessagePackObject(true)]
public class AtgenDebugDamageRecordLog
{
    public int chara_id { get; set; }
    public string name { get; set; }
    public string second_name { get; set; }
    public int weapon_type { get; set; }
    public int elemental_type { get; set; }
    public int level { get; set; }
    public int hp { get; set; }
    public int attack { get; set; }
    public int ex_ability_level { get; set; }
    public int ex_ability_2_level { get; set; }
    public int ability_1_level { get; set; }
    public int ability_2_level { get; set; }
    public int ability_3_level { get; set; }
    public int skill_1_level { get; set; }
    public int skill_2_level { get; set; }
    public int burst_attack_level { get; set; }
    public int combo_buildup_count { get; set; }
    public int hp_plus_count { get; set; }
    public int attack_plus_count { get; set; }
    public int total { get; set; }
    public int skill { get; set; }
    public int dot { get; set; }
    public int critical { get; set; }
    public int enchant { get; set; }
    public int dragon_total { get; set; }
    public int dragon_skill { get; set; }
    public int dragon_dot { get; set; }
    public int dragon_critical { get; set; }
    public int dragon_enchant { get; set; }
    public string build_number { get; set; }
    public string resource_version { get; set; }
    public string server_id { get; set; }
    public string device_name { get; set; }

    public AtgenDebugDamageRecordLog(
        int chara_id,
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
    )
    {
        this.chara_id = chara_id;
        this.name = name;
        this.second_name = second_name;
        this.weapon_type = weapon_type;
        this.elemental_type = elemental_type;
        this.level = level;
        this.hp = hp;
        this.attack = attack;
        this.ex_ability_level = ex_ability_level;
        this.ex_ability_2_level = ex_ability_2_level;
        this.ability_1_level = ability_1_level;
        this.ability_2_level = ability_2_level;
        this.ability_3_level = ability_3_level;
        this.skill_1_level = skill_1_level;
        this.skill_2_level = skill_2_level;
        this.burst_attack_level = burst_attack_level;
        this.combo_buildup_count = combo_buildup_count;
        this.hp_plus_count = hp_plus_count;
        this.attack_plus_count = attack_plus_count;
        this.total = total;
        this.skill = skill;
        this.dot = dot;
        this.critical = critical;
        this.enchant = enchant;
        this.dragon_total = dragon_total;
        this.dragon_skill = dragon_skill;
        this.dragon_dot = dragon_dot;
        this.dragon_critical = dragon_critical;
        this.dragon_enchant = dragon_enchant;
        this.build_number = build_number;
        this.resource_version = resource_version;
        this.server_id = server_id;
        this.device_name = device_name;
    }

    public AtgenDebugDamageRecordLog() { }
}

[MessagePackObject(true)]
public class AtgenDebugDebugPartyList
{
    public int id { get; set; }
    public int party_no { get; set; }
    public int chara_id { get; set; }
    public int chara_level { get; set; }
    public int chara_rarity { get; set; }
    public int chara_hp_plus_count { get; set; }
    public int chara_attack_plus_count { get; set; }
    public int release_mana_circle { get; set; }
    public int dragon_id { get; set; }
    public int dragon_level { get; set; }
    public int dragon_limit { get; set; }
    public int dragon_hp_plus_count { get; set; }
    public int dragon_attack_plus_count { get; set; }
    public int dragon_reliability_level { get; set; }
    public int weapon_body_id { get; set; }
    public int weapon_body_buildup_count { get; set; }
    public int weapon_body_limit_break_count { get; set; }
    public int weapon_body_limit_over_count { get; set; }
    public int crest_slot_type_1_ability_crest_id_1 { get; set; }
    public int crest_slot_type_1_ability_crest_buildup_count_1 { get; set; }
    public int crest_slot_type_1_ability_crest_limit_break_count_1 { get; set; }
    public int crest_slot_type_1_ability_crest_hp_plus_count_1 { get; set; }
    public int crest_slot_type_1_ability_crest_attack_plus_count_1 { get; set; }
    public int crest_slot_type_1_ability_crest_id_2 { get; set; }
    public int crest_slot_type_1_ability_crest_buildup_count_2 { get; set; }
    public int crest_slot_type_1_ability_crest_limit_break_count_2 { get; set; }
    public int crest_slot_type_1_ability_crest_hp_plus_count_2 { get; set; }
    public int crest_slot_type_1_ability_crest_attack_plus_count_2 { get; set; }
    public int crest_slot_type_1_ability_crest_id_3 { get; set; }
    public int crest_slot_type_1_ability_crest_buildup_count_3 { get; set; }
    public int crest_slot_type_1_ability_crest_limit_break_count_3 { get; set; }
    public int crest_slot_type_1_ability_crest_hp_plus_count_3 { get; set; }
    public int crest_slot_type_1_ability_crest_attack_plus_count_3 { get; set; }
    public int crest_slot_type_2_ability_crest_id_1 { get; set; }
    public int crest_slot_type_2_ability_crest_buildup_count_1 { get; set; }
    public int crest_slot_type_2_ability_crest_limit_break_count_1 { get; set; }
    public int crest_slot_type_2_ability_crest_hp_plus_count_1 { get; set; }
    public int crest_slot_type_2_ability_crest_attack_plus_count_1 { get; set; }
    public int crest_slot_type_2_ability_crest_id_2 { get; set; }
    public int crest_slot_type_2_ability_crest_buildup_count_2 { get; set; }
    public int crest_slot_type_2_ability_crest_limit_break_count_2 { get; set; }
    public int crest_slot_type_2_ability_crest_hp_plus_count_2 { get; set; }
    public int crest_slot_type_2_ability_crest_attack_plus_count_2 { get; set; }
    public int crest_slot_type_3_ability_crest_id_1 { get; set; }
    public int crest_slot_type_3_ability_crest_buildup_count_1 { get; set; }
    public int crest_slot_type_3_ability_crest_limit_break_count_1 { get; set; }
    public int crest_slot_type_3_ability_crest_hp_plus_count_1 { get; set; }
    public int crest_slot_type_3_ability_crest_attack_plus_count_1 { get; set; }
    public int crest_slot_type_3_ability_crest_id_2 { get; set; }
    public int crest_slot_type_3_ability_crest_buildup_count_2 { get; set; }
    public int crest_slot_type_3_ability_crest_limit_break_count_2 { get; set; }
    public int crest_slot_type_3_ability_crest_hp_plus_count_2 { get; set; }
    public int crest_slot_type_3_ability_crest_attack_plus_count_2 { get; set; }
    public string title { get; set; }

    public AtgenDebugDebugPartyList(
        int id,
        int party_no,
        int chara_id,
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
    )
    {
        this.id = id;
        this.party_no = party_no;
        this.chara_id = chara_id;
        this.chara_level = chara_level;
        this.chara_rarity = chara_rarity;
        this.chara_hp_plus_count = chara_hp_plus_count;
        this.chara_attack_plus_count = chara_attack_plus_count;
        this.release_mana_circle = release_mana_circle;
        this.dragon_id = dragon_id;
        this.dragon_level = dragon_level;
        this.dragon_limit = dragon_limit;
        this.dragon_hp_plus_count = dragon_hp_plus_count;
        this.dragon_attack_plus_count = dragon_attack_plus_count;
        this.dragon_reliability_level = dragon_reliability_level;
        this.weapon_body_id = weapon_body_id;
        this.weapon_body_buildup_count = weapon_body_buildup_count;
        this.weapon_body_limit_break_count = weapon_body_limit_break_count;
        this.weapon_body_limit_over_count = weapon_body_limit_over_count;
        this.crest_slot_type_1_ability_crest_id_1 = crest_slot_type_1_ability_crest_id_1;
        this.crest_slot_type_1_ability_crest_buildup_count_1 =
            crest_slot_type_1_ability_crest_buildup_count_1;
        this.crest_slot_type_1_ability_crest_limit_break_count_1 =
            crest_slot_type_1_ability_crest_limit_break_count_1;
        this.crest_slot_type_1_ability_crest_hp_plus_count_1 =
            crest_slot_type_1_ability_crest_hp_plus_count_1;
        this.crest_slot_type_1_ability_crest_attack_plus_count_1 =
            crest_slot_type_1_ability_crest_attack_plus_count_1;
        this.crest_slot_type_1_ability_crest_id_2 = crest_slot_type_1_ability_crest_id_2;
        this.crest_slot_type_1_ability_crest_buildup_count_2 =
            crest_slot_type_1_ability_crest_buildup_count_2;
        this.crest_slot_type_1_ability_crest_limit_break_count_2 =
            crest_slot_type_1_ability_crest_limit_break_count_2;
        this.crest_slot_type_1_ability_crest_hp_plus_count_2 =
            crest_slot_type_1_ability_crest_hp_plus_count_2;
        this.crest_slot_type_1_ability_crest_attack_plus_count_2 =
            crest_slot_type_1_ability_crest_attack_plus_count_2;
        this.crest_slot_type_1_ability_crest_id_3 = crest_slot_type_1_ability_crest_id_3;
        this.crest_slot_type_1_ability_crest_buildup_count_3 =
            crest_slot_type_1_ability_crest_buildup_count_3;
        this.crest_slot_type_1_ability_crest_limit_break_count_3 =
            crest_slot_type_1_ability_crest_limit_break_count_3;
        this.crest_slot_type_1_ability_crest_hp_plus_count_3 =
            crest_slot_type_1_ability_crest_hp_plus_count_3;
        this.crest_slot_type_1_ability_crest_attack_plus_count_3 =
            crest_slot_type_1_ability_crest_attack_plus_count_3;
        this.crest_slot_type_2_ability_crest_id_1 = crest_slot_type_2_ability_crest_id_1;
        this.crest_slot_type_2_ability_crest_buildup_count_1 =
            crest_slot_type_2_ability_crest_buildup_count_1;
        this.crest_slot_type_2_ability_crest_limit_break_count_1 =
            crest_slot_type_2_ability_crest_limit_break_count_1;
        this.crest_slot_type_2_ability_crest_hp_plus_count_1 =
            crest_slot_type_2_ability_crest_hp_plus_count_1;
        this.crest_slot_type_2_ability_crest_attack_plus_count_1 =
            crest_slot_type_2_ability_crest_attack_plus_count_1;
        this.crest_slot_type_2_ability_crest_id_2 = crest_slot_type_2_ability_crest_id_2;
        this.crest_slot_type_2_ability_crest_buildup_count_2 =
            crest_slot_type_2_ability_crest_buildup_count_2;
        this.crest_slot_type_2_ability_crest_limit_break_count_2 =
            crest_slot_type_2_ability_crest_limit_break_count_2;
        this.crest_slot_type_2_ability_crest_hp_plus_count_2 =
            crest_slot_type_2_ability_crest_hp_plus_count_2;
        this.crest_slot_type_2_ability_crest_attack_plus_count_2 =
            crest_slot_type_2_ability_crest_attack_plus_count_2;
        this.crest_slot_type_3_ability_crest_id_1 = crest_slot_type_3_ability_crest_id_1;
        this.crest_slot_type_3_ability_crest_buildup_count_1 =
            crest_slot_type_3_ability_crest_buildup_count_1;
        this.crest_slot_type_3_ability_crest_limit_break_count_1 =
            crest_slot_type_3_ability_crest_limit_break_count_1;
        this.crest_slot_type_3_ability_crest_hp_plus_count_1 =
            crest_slot_type_3_ability_crest_hp_plus_count_1;
        this.crest_slot_type_3_ability_crest_attack_plus_count_1 =
            crest_slot_type_3_ability_crest_attack_plus_count_1;
        this.crest_slot_type_3_ability_crest_id_2 = crest_slot_type_3_ability_crest_id_2;
        this.crest_slot_type_3_ability_crest_buildup_count_2 =
            crest_slot_type_3_ability_crest_buildup_count_2;
        this.crest_slot_type_3_ability_crest_limit_break_count_2 =
            crest_slot_type_3_ability_crest_limit_break_count_2;
        this.crest_slot_type_3_ability_crest_hp_plus_count_2 =
            crest_slot_type_3_ability_crest_hp_plus_count_2;
        this.crest_slot_type_3_ability_crest_attack_plus_count_2 =
            crest_slot_type_3_ability_crest_attack_plus_count_2;
        this.title = title;
    }

    public AtgenDebugDebugPartyList() { }
}

[MessagePackObject(true)]
public class AtgenDebugUnitDataList
{
    public int chara_id { get; set; }
    public int chara_lv { get; set; }
    public int chara_hp_plus_count { get; set; }
    public int chara_attack_plus_count { get; set; }
    public int chara_rarity { get; set; }
    public int release_mana_circle { get; set; }
    public int weapon_body_id { get; set; }
    public int weapon_body_buildup_count { get; set; }
    public int weapon_body_limit_break_count { get; set; }
    public int weapon_body_limit_over_count { get; set; }
    public int dragon_id { get; set; }
    public int dragon_lv { get; set; }
    public int dragon_hp_plus_count { get; set; }
    public int dragon_attack_plus_count { get; set; }
    public int dragon_limit { get; set; }
    public int dragon_reliability_level { get; set; }
    public int crest_slot_type_1_ability_crest_id_1 { get; set; }
    public int crest_slot_type_1_ability_crest_buildup_count_1 { get; set; }
    public int crest_slot_type_1_ability_crest_limit_break_count_1 { get; set; }
    public int crest_slot_type_1_ability_crest_hp_plus_count_1 { get; set; }
    public int crest_slot_type_1_ability_crest_attack_plus_count_1 { get; set; }
    public int crest_slot_type_1_ability_crest_id_2 { get; set; }
    public int crest_slot_type_1_ability_crest_buildup_count_2 { get; set; }
    public int crest_slot_type_1_ability_crest_limit_break_count_2 { get; set; }
    public int crest_slot_type_1_ability_crest_hp_plus_count_2 { get; set; }
    public int crest_slot_type_1_ability_crest_attack_plus_count_2 { get; set; }
    public int crest_slot_type_1_ability_crest_id_3 { get; set; }
    public int crest_slot_type_1_ability_crest_buildup_count_3 { get; set; }
    public int crest_slot_type_1_ability_crest_limit_break_count_3 { get; set; }
    public int crest_slot_type_1_ability_crest_hp_plus_count_3 { get; set; }
    public int crest_slot_type_1_ability_crest_attack_plus_count_3 { get; set; }
    public int crest_slot_type_2_ability_crest_id_1 { get; set; }
    public int crest_slot_type_2_ability_crest_buildup_count_1 { get; set; }
    public int crest_slot_type_2_ability_crest_limit_break_count_1 { get; set; }
    public int crest_slot_type_2_ability_crest_hp_plus_count_1 { get; set; }
    public int crest_slot_type_2_ability_crest_attack_plus_count_1 { get; set; }
    public int crest_slot_type_2_ability_crest_id_2 { get; set; }
    public int crest_slot_type_2_ability_crest_buildup_count_2 { get; set; }
    public int crest_slot_type_2_ability_crest_limit_break_count_2 { get; set; }
    public int crest_slot_type_2_ability_crest_hp_plus_count_2 { get; set; }
    public int crest_slot_type_2_ability_crest_attack_plus_count_2 { get; set; }
    public int crest_slot_type_3_ability_crest_id_1 { get; set; }
    public int crest_slot_type_3_ability_crest_buildup_count_1 { get; set; }
    public int crest_slot_type_3_ability_crest_limit_break_count_1 { get; set; }
    public int crest_slot_type_3_ability_crest_hp_plus_count_1 { get; set; }
    public int crest_slot_type_3_ability_crest_attack_plus_count_1 { get; set; }
    public int crest_slot_type_3_ability_crest_id_2 { get; set; }
    public int crest_slot_type_3_ability_crest_buildup_count_2 { get; set; }
    public int crest_slot_type_3_ability_crest_limit_break_count_2 { get; set; }
    public int crest_slot_type_3_ability_crest_hp_plus_count_2 { get; set; }
    public int crest_slot_type_3_ability_crest_attack_plus_count_2 { get; set; }

    public AtgenDebugUnitDataList(
        int chara_id,
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
    )
    {
        this.chara_id = chara_id;
        this.chara_lv = chara_lv;
        this.chara_hp_plus_count = chara_hp_plus_count;
        this.chara_attack_plus_count = chara_attack_plus_count;
        this.chara_rarity = chara_rarity;
        this.release_mana_circle = release_mana_circle;
        this.weapon_body_id = weapon_body_id;
        this.weapon_body_buildup_count = weapon_body_buildup_count;
        this.weapon_body_limit_break_count = weapon_body_limit_break_count;
        this.weapon_body_limit_over_count = weapon_body_limit_over_count;
        this.dragon_id = dragon_id;
        this.dragon_lv = dragon_lv;
        this.dragon_hp_plus_count = dragon_hp_plus_count;
        this.dragon_attack_plus_count = dragon_attack_plus_count;
        this.dragon_limit = dragon_limit;
        this.dragon_reliability_level = dragon_reliability_level;
        this.crest_slot_type_1_ability_crest_id_1 = crest_slot_type_1_ability_crest_id_1;
        this.crest_slot_type_1_ability_crest_buildup_count_1 =
            crest_slot_type_1_ability_crest_buildup_count_1;
        this.crest_slot_type_1_ability_crest_limit_break_count_1 =
            crest_slot_type_1_ability_crest_limit_break_count_1;
        this.crest_slot_type_1_ability_crest_hp_plus_count_1 =
            crest_slot_type_1_ability_crest_hp_plus_count_1;
        this.crest_slot_type_1_ability_crest_attack_plus_count_1 =
            crest_slot_type_1_ability_crest_attack_plus_count_1;
        this.crest_slot_type_1_ability_crest_id_2 = crest_slot_type_1_ability_crest_id_2;
        this.crest_slot_type_1_ability_crest_buildup_count_2 =
            crest_slot_type_1_ability_crest_buildup_count_2;
        this.crest_slot_type_1_ability_crest_limit_break_count_2 =
            crest_slot_type_1_ability_crest_limit_break_count_2;
        this.crest_slot_type_1_ability_crest_hp_plus_count_2 =
            crest_slot_type_1_ability_crest_hp_plus_count_2;
        this.crest_slot_type_1_ability_crest_attack_plus_count_2 =
            crest_slot_type_1_ability_crest_attack_plus_count_2;
        this.crest_slot_type_1_ability_crest_id_3 = crest_slot_type_1_ability_crest_id_3;
        this.crest_slot_type_1_ability_crest_buildup_count_3 =
            crest_slot_type_1_ability_crest_buildup_count_3;
        this.crest_slot_type_1_ability_crest_limit_break_count_3 =
            crest_slot_type_1_ability_crest_limit_break_count_3;
        this.crest_slot_type_1_ability_crest_hp_plus_count_3 =
            crest_slot_type_1_ability_crest_hp_plus_count_3;
        this.crest_slot_type_1_ability_crest_attack_plus_count_3 =
            crest_slot_type_1_ability_crest_attack_plus_count_3;
        this.crest_slot_type_2_ability_crest_id_1 = crest_slot_type_2_ability_crest_id_1;
        this.crest_slot_type_2_ability_crest_buildup_count_1 =
            crest_slot_type_2_ability_crest_buildup_count_1;
        this.crest_slot_type_2_ability_crest_limit_break_count_1 =
            crest_slot_type_2_ability_crest_limit_break_count_1;
        this.crest_slot_type_2_ability_crest_hp_plus_count_1 =
            crest_slot_type_2_ability_crest_hp_plus_count_1;
        this.crest_slot_type_2_ability_crest_attack_plus_count_1 =
            crest_slot_type_2_ability_crest_attack_plus_count_1;
        this.crest_slot_type_2_ability_crest_id_2 = crest_slot_type_2_ability_crest_id_2;
        this.crest_slot_type_2_ability_crest_buildup_count_2 =
            crest_slot_type_2_ability_crest_buildup_count_2;
        this.crest_slot_type_2_ability_crest_limit_break_count_2 =
            crest_slot_type_2_ability_crest_limit_break_count_2;
        this.crest_slot_type_2_ability_crest_hp_plus_count_2 =
            crest_slot_type_2_ability_crest_hp_plus_count_2;
        this.crest_slot_type_2_ability_crest_attack_plus_count_2 =
            crest_slot_type_2_ability_crest_attack_plus_count_2;
        this.crest_slot_type_3_ability_crest_id_1 = crest_slot_type_3_ability_crest_id_1;
        this.crest_slot_type_3_ability_crest_buildup_count_1 =
            crest_slot_type_3_ability_crest_buildup_count_1;
        this.crest_slot_type_3_ability_crest_limit_break_count_1 =
            crest_slot_type_3_ability_crest_limit_break_count_1;
        this.crest_slot_type_3_ability_crest_hp_plus_count_1 =
            crest_slot_type_3_ability_crest_hp_plus_count_1;
        this.crest_slot_type_3_ability_crest_attack_plus_count_1 =
            crest_slot_type_3_ability_crest_attack_plus_count_1;
        this.crest_slot_type_3_ability_crest_id_2 = crest_slot_type_3_ability_crest_id_2;
        this.crest_slot_type_3_ability_crest_buildup_count_2 =
            crest_slot_type_3_ability_crest_buildup_count_2;
        this.crest_slot_type_3_ability_crest_limit_break_count_2 =
            crest_slot_type_3_ability_crest_limit_break_count_2;
        this.crest_slot_type_3_ability_crest_hp_plus_count_2 =
            crest_slot_type_3_ability_crest_hp_plus_count_2;
        this.crest_slot_type_3_ability_crest_attack_plus_count_2 =
            crest_slot_type_3_ability_crest_attack_plus_count_2;
    }

    public AtgenDebugUnitDataList() { }
}

[MessagePackObject(true)]
public class AtgenDeleteAmuletList
{
    public ulong amulet_key_id { get; set; }

    public AtgenDeleteAmuletList(ulong amulet_key_id)
    {
        this.amulet_key_id = amulet_key_id;
    }

    public AtgenDeleteAmuletList() { }
}

[MessagePackObject(true)]
public class AtgenDeleteDragonList
{
    public ulong dragon_key_id { get; set; }

    public AtgenDeleteDragonList(ulong dragon_key_id)
    {
        this.dragon_key_id = dragon_key_id;
    }

    public AtgenDeleteDragonList() { }
}

[MessagePackObject(true)]
public class AtgenDeleteTalismanList
{
    public ulong talisman_key_id { get; set; }

    public AtgenDeleteTalismanList(ulong talisman_key_id)
    {
        this.talisman_key_id = talisman_key_id;
    }

    public AtgenDeleteTalismanList() { }
}

[MessagePackObject(true)]
public class AtgenDeleteWeaponList
{
    public ulong weapon_key_id { get; set; }

    public AtgenDeleteWeaponList(ulong weapon_key_id)
    {
        this.weapon_key_id = weapon_key_id;
    }

    public AtgenDeleteWeaponList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeAreaInfo
{
    public int floor_num { get; set; }
    public float quest_time { get; set; }
    public int dmode_score { get; set; }
    public int current_area_theme_id { get; set; }
    public int current_area_id { get; set; }

    public AtgenDmodeAreaInfo(
        int floor_num,
        float quest_time,
        int dmode_score,
        int current_area_theme_id,
        int current_area_id
    )
    {
        this.floor_num = floor_num;
        this.quest_time = quest_time;
        this.dmode_score = dmode_score;
        this.current_area_theme_id = current_area_theme_id;
        this.current_area_id = current_area_id;
    }

    public AtgenDmodeAreaInfo() { }
}

[MessagePackObject(true)]
public class AtgenDmodeDragonUseList
{
    public int dragon_id { get; set; }
    public int use_count { get; set; }

    public AtgenDmodeDragonUseList(int dragon_id, int use_count)
    {
        this.dragon_id = dragon_id;
        this.use_count = use_count;
    }

    public AtgenDmodeDragonUseList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeDropList
{
    public int type { get; set; }
    public int id { get; set; }
    public int quantity { get; set; }

    public AtgenDmodeDropList(int type, int id, int quantity)
    {
        this.type = type;
        this.id = id;
        this.quantity = quantity;
    }

    public AtgenDmodeDropList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeDropObj
{
    public int obj_id { get; set; }
    public int obj_type { get; set; }
    public IEnumerable<AtgenDmodeDropList> dmode_drop_list { get; set; }

    public AtgenDmodeDropObj(
        int obj_id,
        int obj_type,
        IEnumerable<AtgenDmodeDropList> dmode_drop_list
    )
    {
        this.obj_id = obj_id;
        this.obj_type = obj_type;
        this.dmode_drop_list = dmode_drop_list;
    }

    public AtgenDmodeDropObj() { }
}

[MessagePackObject(true)]
public class AtgenDmodeDungeonItemOptionList
{
    public int item_no { get; set; }
    public int abnormal_status_invalid_count { get; set; }

    public AtgenDmodeDungeonItemOptionList(int item_no, int abnormal_status_invalid_count)
    {
        this.item_no = item_no;
        this.abnormal_status_invalid_count = abnormal_status_invalid_count;
    }

    public AtgenDmodeDungeonItemOptionList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeDungeonItemStateList
{
    public int item_no { get; set; }
    public int state { get; set; }

    public AtgenDmodeDungeonItemStateList(int item_no, int state)
    {
        this.item_no = item_no;
        this.state = state;
    }

    public AtgenDmodeDungeonItemStateList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeDungeonOdds
{
    public IEnumerable<AtgenDmodeSelectDragonList> dmode_select_dragon_list { get; set; }
    public IEnumerable<DmodeDungeonItemList> dmode_dungeon_item_list { get; set; }
    public DmodeOddsInfo dmode_odds_info { get; set; }

    public AtgenDmodeDungeonOdds(
        IEnumerable<AtgenDmodeSelectDragonList> dmode_select_dragon_list,
        IEnumerable<DmodeDungeonItemList> dmode_dungeon_item_list,
        DmodeOddsInfo dmode_odds_info
    )
    {
        this.dmode_select_dragon_list = dmode_select_dragon_list;
        this.dmode_dungeon_item_list = dmode_dungeon_item_list;
        this.dmode_odds_info = dmode_odds_info;
    }

    public AtgenDmodeDungeonOdds() { }
}

[MessagePackObject(true)]
public class AtgenDmodeEnemy
{
    public int enemy_idx { get; set; }
    public int is_pop { get; set; }
    public int level { get; set; }
    public int param_id { get; set; }
    public IEnumerable<AtgenDmodeDropList> dmode_drop_list { get; set; }

    public AtgenDmodeEnemy(
        int enemy_idx,
        int is_pop,
        int level,
        int param_id,
        IEnumerable<AtgenDmodeDropList> dmode_drop_list
    )
    {
        this.enemy_idx = enemy_idx;
        this.is_pop = is_pop;
        this.level = level;
        this.param_id = param_id;
        this.dmode_drop_list = dmode_drop_list;
    }

    public AtgenDmodeEnemy() { }
}

[MessagePackObject(true)]
public class AtgenDmodeHoldDragonList
{
    public int dragon_id { get; set; }
    public int count { get; set; }

    public AtgenDmodeHoldDragonList(int dragon_id, int count)
    {
        this.dragon_id = dragon_id;
        this.count = count;
    }

    public AtgenDmodeHoldDragonList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeSelectDragonList
{
    public int select_dragon_no { get; set; }
    public int dragon_id { get; set; }
    public int is_rare { get; set; }
    public int pay_dmode_point_1 { get; set; }
    public int pay_dmode_point_2 { get; set; }

    public AtgenDmodeSelectDragonList(
        int select_dragon_no,
        int dragon_id,
        int is_rare,
        int pay_dmode_point_1,
        int pay_dmode_point_2
    )
    {
        this.select_dragon_no = select_dragon_no;
        this.dragon_id = dragon_id;
        this.is_rare = is_rare;
        this.pay_dmode_point_1 = pay_dmode_point_1;
        this.pay_dmode_point_2 = pay_dmode_point_2;
    }

    public AtgenDmodeSelectDragonList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeTreasureRecord
{
    public IEnumerable<int> drop_obj { get; set; }
    public IEnumerable<int> enemy { get; set; }

    public AtgenDmodeTreasureRecord(IEnumerable<int> drop_obj, IEnumerable<int> enemy)
    {
        this.drop_obj = drop_obj;
        this.enemy = enemy;
    }

    public AtgenDmodeTreasureRecord() { }
}

[MessagePackObject(true)]
public class AtgenDmodeUnitInfo
{
    public int level { get; set; }
    public int exp { get; set; }
    public IEnumerable<int> equip_crest_item_no_sort_list { get; set; }
    public IEnumerable<int> bag_item_no_sort_list { get; set; }
    public IEnumerable<int> skill_bag_item_no_sort_list { get; set; }
    public IEnumerable<AtgenDmodeHoldDragonList> dmode_hold_dragon_list { get; set; }
    public int take_dmode_point_1 { get; set; }
    public int take_dmode_point_2 { get; set; }

    public AtgenDmodeUnitInfo(
        int level,
        int exp,
        IEnumerable<int> equip_crest_item_no_sort_list,
        IEnumerable<int> bag_item_no_sort_list,
        IEnumerable<int> skill_bag_item_no_sort_list,
        IEnumerable<AtgenDmodeHoldDragonList> dmode_hold_dragon_list,
        int take_dmode_point_1,
        int take_dmode_point_2
    )
    {
        this.level = level;
        this.exp = exp;
        this.equip_crest_item_no_sort_list = equip_crest_item_no_sort_list;
        this.bag_item_no_sort_list = bag_item_no_sort_list;
        this.skill_bag_item_no_sort_list = skill_bag_item_no_sort_list;
        this.dmode_hold_dragon_list = dmode_hold_dragon_list;
        this.take_dmode_point_1 = take_dmode_point_1;
        this.take_dmode_point_2 = take_dmode_point_2;
    }

    public AtgenDmodeUnitInfo() { }
}

[MessagePackObject(true)]
public class AtgenDragonBonus
{
    public int elemental_type { get; set; }
    public float dragon_bonus { get; set; }
    public float hp { get; set; }
    public float attack { get; set; }

    public AtgenDragonBonus(int elemental_type, float dragon_bonus, float hp, float attack)
    {
        this.elemental_type = elemental_type;
        this.dragon_bonus = dragon_bonus;
        this.hp = hp;
        this.attack = attack;
    }

    public AtgenDragonBonus() { }
}

[MessagePackObject(true)]
public class AtgenDragonGiftRewardList
{
    public int dragon_gift_id { get; set; }
    public int is_favorite { get; set; }
    public IEnumerable<DragonRewardEntityList> return_gift_list { get; set; }
    public IEnumerable<RewardReliabilityList> reward_reliability_list { get; set; }

    public AtgenDragonGiftRewardList(
        int dragon_gift_id,
        int is_favorite,
        IEnumerable<DragonRewardEntityList> return_gift_list,
        IEnumerable<RewardReliabilityList> reward_reliability_list
    )
    {
        this.dragon_gift_id = dragon_gift_id;
        this.is_favorite = is_favorite;
        this.return_gift_list = return_gift_list;
        this.reward_reliability_list = reward_reliability_list;
    }

    public AtgenDragonGiftRewardList() { }
}

[MessagePackObject(true)]
public class AtgenDragonTimeBonus
{
    public float dragon_time_bonus { get; set; }

    public AtgenDragonTimeBonus(float dragon_time_bonus)
    {
        this.dragon_time_bonus = dragon_time_bonus;
    }

    public AtgenDragonTimeBonus() { }
}

[MessagePackObject(true)]
public class AtgenDrawDetails
{
    public int id { get; set; }
    public int is_new { get; set; }
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }
    public int entity_quantity { get; set; }
    public int view_rarity { get; set; }

    public AtgenDrawDetails(
        int id,
        int is_new,
        EntityTypes entity_type,
        int entity_id,
        int entity_quantity,
        int view_rarity
    )
    {
        this.id = id;
        this.is_new = is_new;
        this.entity_type = entity_type;
        this.entity_id = entity_id;
        this.entity_quantity = entity_quantity;
        this.view_rarity = view_rarity;
    }

    public AtgenDrawDetails() { }
}

[MessagePackObject(true)]
public class AtgenDropAll
{
    public int id { get; set; }
    public EntityTypes type { get; set; }
    public int quantity { get; set; }
    public int place { get; set; }
    public float factor { get; set; }

    public AtgenDropAll(int id, EntityTypes type, int quantity, int place, float factor)
    {
        this.id = id;
        this.type = type;
        this.quantity = quantity;
        this.place = place;
        this.factor = factor;
    }

    public AtgenDropAll() { }
}

[MessagePackObject(true)]
public class AtgenDropList
{
    public int type { get; set; }
    public int id { get; set; }
    public int quantity { get; set; }
    public int place { get; set; }

    public AtgenDropList(int type, int id, int quantity, int place)
    {
        this.type = type;
        this.id = id;
        this.quantity = quantity;
        this.place = place;
    }

    public AtgenDropList() { }
}

[MessagePackObject(true)]
public class AtgenDropObj
{
    public int obj_id { get; set; }
    public int obj_type { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_rare { get; set; }
    public IEnumerable<AtgenDropList> drop_list { get; set; }

    public AtgenDropObj(
        int obj_id,
        int obj_type,
        bool is_rare,
        IEnumerable<AtgenDropList> drop_list
    )
    {
        this.obj_id = obj_id;
        this.obj_type = obj_type;
        this.is_rare = is_rare;
        this.drop_list = drop_list;
    }

    public AtgenDropObj() { }
}

[MessagePackObject(true)]
public class AtgenDuplicateEntityList
{
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }

    public AtgenDuplicateEntityList(EntityTypes entity_type, int entity_id)
    {
        this.entity_type = entity_type;
        this.entity_id = entity_id;
    }

    public AtgenDuplicateEntityList() { }
}

[MessagePackObject(true)]
public class AtgenElementBonus
{
    public int elemental_type { get; set; }
    public float hp { get; set; }
    public float attack { get; set; }

    public AtgenElementBonus(int elemental_type, float hp, float attack)
    {
        this.elemental_type = elemental_type;
        this.hp = hp;
        this.attack = attack;
    }

    public AtgenElementBonus() { }
}

[MessagePackObject(true)]
public class AtgenEnemy
{
    public int piece { get; set; }
    public int enemy_idx { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_pop { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_rare { get; set; }
    public int param_id { get; set; }
    public IEnumerable<EnemyDropList> enemy_drop_list { get; set; }

    public AtgenEnemy(
        int piece,
        int enemy_idx,
        bool is_pop,
        bool is_rare,
        int param_id,
        IEnumerable<EnemyDropList> enemy_drop_list
    )
    {
        this.piece = piece;
        this.enemy_idx = enemy_idx;
        this.is_pop = is_pop;
        this.is_rare = is_rare;
        this.param_id = param_id;
        this.enemy_drop_list = enemy_drop_list;
    }

    public AtgenEnemy() { }
}

[MessagePackObject(true)]
public class AtgenEnemyPiece
{
    public Materials id { get; set; }
    public int quantity { get; set; }

    public AtgenEnemyPiece(Materials id, int quantity)
    {
        this.id = id;
        this.quantity = quantity;
    }

    public AtgenEnemyPiece() { }
}

[MessagePackObject(true)]
public class AtgenEnemySmash
{
    public int count { get; set; }

    public AtgenEnemySmash(int count)
    {
        this.count = count;
    }

    public AtgenEnemySmash() { }
}

[MessagePackObject(true)]
public class AtgenEntryConditions
{
    public IEnumerable<int> unaccepted_element_type_list { get; set; }
    public IEnumerable<int> unaccepted_weapon_type_list { get; set; }
    public int required_party_power { get; set; }
    public int objective_text_id { get; set; }
    public string objective_free_text { get; set; }

    public AtgenEntryConditions(
        IEnumerable<int> unaccepted_element_type_list,
        IEnumerable<int> unaccepted_weapon_type_list,
        int required_party_power,
        int objective_text_id,
        string objective_free_text
    )
    {
        this.unaccepted_element_type_list = unaccepted_element_type_list;
        this.unaccepted_weapon_type_list = unaccepted_weapon_type_list;
        this.required_party_power = required_party_power;
        this.objective_text_id = objective_text_id;
        this.objective_free_text = objective_free_text;
    }

    public AtgenEntryConditions() { }
}

[MessagePackObject(true)]
public class AtgenEventBoost
{
    public int event_effect { get; set; }
    public float effect_value { get; set; }

    public AtgenEventBoost(int event_effect, float effect_value)
    {
        this.event_effect = event_effect;
        this.effect_value = effect_value;
    }

    public AtgenEventBoost() { }
}

[MessagePackObject(true)]
public class AtgenEventDamageData
{
    public long user_damage_value { get; set; }
    public int user_target_time { get; set; }
    public long total_damage_value { get; set; }
    public int total_target_time { get; set; }
    public int total_aggregate_time { get; set; }

    public AtgenEventDamageData(
        long user_damage_value,
        int user_target_time,
        long total_damage_value,
        int total_target_time,
        int total_aggregate_time
    )
    {
        this.user_damage_value = user_damage_value;
        this.user_target_time = user_target_time;
        this.total_damage_value = total_damage_value;
        this.total_target_time = total_target_time;
        this.total_aggregate_time = total_aggregate_time;
    }

    public AtgenEventDamageData() { }
}

[MessagePackObject(true)]
public class AtgenEventDamageHistoryList
{
    public int target_time { get; set; }
    public long total_damage_value { get; set; }

    public AtgenEventDamageHistoryList(int target_time, long total_damage_value)
    {
        this.target_time = target_time;
        this.total_damage_value = total_damage_value;
    }

    public AtgenEventDamageHistoryList() { }
}

[MessagePackObject(true)]
public class AtgenEventDamageRewardList
{
    public int target_time { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> reward_entity_list { get; set; }

    public AtgenEventDamageRewardList(
        int target_time,
        IEnumerable<AtgenBuildEventRewardEntityList> reward_entity_list
    )
    {
        this.target_time = target_time;
        this.reward_entity_list = reward_entity_list;
    }

    public AtgenEventDamageRewardList() { }
}

[MessagePackObject(true)]
public class AtgenEventFortData
{
    public int plant_id { get; set; }
    public int level { get; set; }

    public AtgenEventFortData(int plant_id, int level)
    {
        this.plant_id = plant_id;
        this.level = level;
    }

    public AtgenEventFortData() { }
}

[MessagePackObject(true)]
public class AtgenEventPassiveUpList
{
    public int passive_id { get; set; }
    public int progress { get; set; }

    public AtgenEventPassiveUpList(int passive_id, int progress)
    {
        this.passive_id = passive_id;
        this.progress = progress;
    }

    public AtgenEventPassiveUpList() { }
}

[MessagePackObject(true)]
public class AtgenExchangeSummomPointList
{
    public int summon_point_id { get; set; }
    public int summon_point { get; set; }
    public int cs_summon_point { get; set; }

    public AtgenExchangeSummomPointList(int summon_point_id, int summon_point, int cs_summon_point)
    {
        this.summon_point_id = summon_point_id;
        this.summon_point = summon_point;
        this.cs_summon_point = cs_summon_point;
    }

    public AtgenExchangeSummomPointList() { }
}

[MessagePackObject(true)]
public class AtgenFailQuestDetail
{
    public int quest_id { get; set; }
    public int wall_id { get; set; }
    public int wall_level { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_host { get; set; }

    public AtgenFailQuestDetail(int quest_id, int wall_id, int wall_level, bool is_host)
    {
        this.quest_id = quest_id;
        this.wall_id = wall_id;
        this.wall_level = wall_level;
        this.is_host = is_host;
    }

    public AtgenFailQuestDetail() { }
}

[MessagePackObject(true)]
public class AtgenFirstClearSet
{
    public int id { get; set; }
    public int type { get; set; }
    public int quantity { get; set; }

    public AtgenFirstClearSet(int id, int type, int quantity)
    {
        this.id = id;
        this.type = type;
        this.quantity = quantity;
    }

    public AtgenFirstClearSet() { }
}

[MessagePackObject(true)]
public class AtgenFirstMeeting
{
    public int headcount { get; set; }
    public int id { get; set; }
    public int type { get; set; }
    public int total_quantity { get; set; }

    public AtgenFirstMeeting(int headcount, int id, int type, int total_quantity)
    {
        this.headcount = headcount;
        this.id = id;
        this.type = type;
        this.total_quantity = total_quantity;
    }

    public AtgenFirstMeeting() { }
}

[MessagePackObject(true)]
public class AtgenGrade
{
    public int min_value { get; set; }
    public int max_value { get; set; }
    public int grade_num { get; set; }
    public IEnumerable<AtgenDropList> drop_list { get; set; }

    public AtgenGrade(
        int min_value,
        int max_value,
        int grade_num,
        IEnumerable<AtgenDropList> drop_list
    )
    {
        this.min_value = min_value;
        this.max_value = max_value;
        this.grade_num = grade_num;
        this.drop_list = drop_list;
    }

    public AtgenGrade() { }
}

[MessagePackObject(true)]
public class AtgenGuild
{
    public int guild_id { get; set; }
    public int guild_emblem_id { get; set; }
    public string guild_name { get; set; }
    public int is_penalty_guild_name { get; set; }

    public AtgenGuild(
        int guild_id,
        int guild_emblem_id,
        string guild_name,
        int is_penalty_guild_name
    )
    {
        this.guild_id = guild_id;
        this.guild_emblem_id = guild_emblem_id;
        this.guild_name = guild_name;
        this.is_penalty_guild_name = is_penalty_guild_name;
    }

    public AtgenGuild() { }
}

[MessagePackObject(true)]
public class AtgenGuildInviteParamsList
{
    public int guild_id { get; set; }
    public ulong guild_invite_id { get; set; }

    public AtgenGuildInviteParamsList(int guild_id, ulong guild_invite_id)
    {
        this.guild_id = guild_id;
        this.guild_invite_id = guild_invite_id;
    }

    public AtgenGuildInviteParamsList() { }
}

[MessagePackObject(true)]
public class AtgenHarvestBuildList
{
    public ulong build_id { get; set; }
    public IEnumerable<AtgenAddHarvestList> add_harvest_list { get; set; }

    public AtgenHarvestBuildList(ulong build_id, IEnumerable<AtgenAddHarvestList> add_harvest_list)
    {
        this.build_id = build_id;
        this.add_harvest_list = add_harvest_list;
    }

    public AtgenHarvestBuildList() { }
}

[MessagePackObject(true)]
public class AtgenHelperDetailList
{
    public ulong viewer_id { get; set; }
    public int is_friend { get; set; }
    public int get_mana_point { get; set; }
    public int apply_send_status { get; set; }

    public AtgenHelperDetailList(
        ulong viewer_id,
        int is_friend,
        int get_mana_point,
        int apply_send_status
    )
    {
        this.viewer_id = viewer_id;
        this.is_friend = is_friend;
        this.get_mana_point = get_mana_point;
        this.apply_send_status = apply_send_status;
    }

    public AtgenHelperDetailList() { }
}

[MessagePackObject(true)]
public class AtgenHonorList
{
    public int honor_id { get; set; }

    public AtgenHonorList(int honor_id)
    {
        this.honor_id = honor_id;
    }

    public AtgenHonorList() { }
}

[MessagePackObject(true)]
public class AtgenIngameWalker
{
    public int skill_2_level { get; set; }

    public AtgenIngameWalker(int skill_2_level)
    {
        this.skill_2_level = skill_2_level;
    }

    public AtgenIngameWalker() { }
}

[MessagePackObject(true)]
public class AtgenInquiryFaqList
{
    public int id { get; set; }
    public string question { get; set; }
    public string answer { get; set; }

    public AtgenInquiryFaqList(int id, string question, string answer)
    {
        this.id = id;
        this.question = question;
        this.answer = answer;
    }

    public AtgenInquiryFaqList() { }
}

[MessagePackObject(true)]
public class AtgenItemSummonRateList
{
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }
    public int entity_quantity { get; set; }
    public string entity_rate { get; set; }

    public AtgenItemSummonRateList(
        EntityTypes entity_type,
        int entity_id,
        int entity_quantity,
        string entity_rate
    )
    {
        this.entity_type = entity_type;
        this.entity_id = entity_id;
        this.entity_quantity = entity_quantity;
        this.entity_rate = entity_rate;
    }

    public AtgenItemSummonRateList() { }
}

[MessagePackObject(true)]
public class AtgenLatest
{
    public int episode { get; set; }

    public AtgenLatest(int episode)
    {
        this.episode = episode;
    }

    public AtgenLatest() { }
}

[MessagePackObject(true)]
public class AtgenLoginBonusList
{
    public int reward_code { get; set; }
    public int login_bonus_id { get; set; }
    public int total_login_day { get; set; }
    public int reward_day { get; set; }
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }
    public int entity_quantity { get; set; }
    public int entity_level { get; set; }
    public int entity_limit_break_count { get; set; }

    public AtgenLoginBonusList(
        int reward_code,
        int login_bonus_id,
        int total_login_day,
        int reward_day,
        EntityTypes entity_type,
        int entity_id,
        int entity_quantity,
        int entity_level,
        int entity_limit_break_count
    )
    {
        this.reward_code = reward_code;
        this.login_bonus_id = login_bonus_id;
        this.total_login_day = total_login_day;
        this.reward_day = reward_day;
        this.entity_type = entity_type;
        this.entity_id = entity_id;
        this.entity_quantity = entity_quantity;
        this.entity_level = entity_level;
        this.entity_limit_break_count = entity_limit_break_count;
    }

    public AtgenLoginBonusList() { }
}

[MessagePackObject(true)]
public class AtgenLoginLotteryRewardList
{
    public int login_lottery_id { get; set; }
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }
    public int entity_quantity { get; set; }
    public int is_pickup { get; set; }
    public int is_guaranteed { get; set; }

    public AtgenLoginLotteryRewardList(
        int login_lottery_id,
        EntityTypes entity_type,
        int entity_id,
        int entity_quantity,
        int is_pickup,
        int is_guaranteed
    )
    {
        this.login_lottery_id = login_lottery_id;
        this.entity_type = entity_type;
        this.entity_id = entity_id;
        this.entity_quantity = entity_quantity;
        this.is_pickup = is_pickup;
        this.is_guaranteed = is_guaranteed;
    }

    public AtgenLoginLotteryRewardList() { }
}

[MessagePackObject(true)]
public class AtgenLostUnitList
{
    public int unit_no { get; set; }
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }

    public AtgenLostUnitList(int unit_no, EntityTypes entity_type, int entity_id)
    {
        this.unit_no = unit_no;
        this.entity_type = entity_type;
        this.entity_id = entity_id;
    }

    public AtgenLostUnitList() { }
}

[MessagePackObject(true)]
public class AtgenLotteryEntitySetList
{
    public int lottery_prize_rank { get; set; }
    public string rate { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> entity_list { get; set; }

    public AtgenLotteryEntitySetList(
        int lottery_prize_rank,
        string rate,
        IEnumerable<AtgenBuildEventRewardEntityList> entity_list
    )
    {
        this.lottery_prize_rank = lottery_prize_rank;
        this.rate = rate;
        this.entity_list = entity_list;
    }

    public AtgenLotteryEntitySetList() { }
}

[MessagePackObject(true)]
public class AtgenLotteryPrizeRankList
{
    public int lottery_prize_rank { get; set; }
    public string total_rate { get; set; }

    public AtgenLotteryPrizeRankList(int lottery_prize_rank, string total_rate)
    {
        this.lottery_prize_rank = lottery_prize_rank;
        this.total_rate = total_rate;
    }

    public AtgenLotteryPrizeRankList() { }
}

[MessagePackObject(true)]
public class AtgenLotteryResultList
{
    public int lottery_rank { get; set; }
    public int rank_entiry_quantity { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> entity_list { get; set; }

    public AtgenLotteryResultList(
        int lottery_rank,
        int rank_entiry_quantity,
        IEnumerable<AtgenBuildEventRewardEntityList> entity_list
    )
    {
        this.lottery_rank = lottery_rank;
        this.rank_entiry_quantity = rank_entiry_quantity;
        this.entity_list = entity_list;
    }

    public AtgenLotteryResultList() { }
}

[MessagePackObject(true)]
public class AtgenMainStoryMissionStateList
{
    public int main_story_mission_id { get; set; }
    public int state { get; set; }

    public AtgenMainStoryMissionStateList(int main_story_mission_id, int state)
    {
        this.main_story_mission_id = main_story_mission_id;
        this.state = state;
    }

    public AtgenMainStoryMissionStateList() { }
}

[MessagePackObject(true)]
public class AtgenMissionParamsList
{
    public int daily_mission_id { get; set; }
    public int day_no { get; set; }

    public AtgenMissionParamsList(int daily_mission_id, int day_no)
    {
        this.daily_mission_id = daily_mission_id;
        this.day_no = day_no;
    }

    public AtgenMissionParamsList() { }
}

[MessagePackObject(true)]
public class AtgenMissionsClearSet
{
    public int id { get; set; }
    public int type { get; set; }
    public int quantity { get; set; }
    public int mission_no { get; set; }

    public AtgenMissionsClearSet(int id, int type, int quantity, int mission_no)
    {
        this.id = id;
        this.type = type;
        this.quantity = quantity;
        this.mission_no = mission_no;
    }

    public AtgenMissionsClearSet() { }
}

[MessagePackObject(true)]
public class AtgenMonthlyWallReceiveList
{
    public int quest_group_id { get; set; }
    public int is_receive_reward { get; set; }

    public AtgenMonthlyWallReceiveList(int quest_group_id, int is_receive_reward)
    {
        this.quest_group_id = quest_group_id;
        this.is_receive_reward = is_receive_reward;
    }

    public AtgenMonthlyWallReceiveList() { }
}

[MessagePackObject(true)]
public class AtgenMultiServer
{
    public string host { get; set; }
    public string app_id { get; set; }

    public AtgenMultiServer(string host, string app_id)
    {
        this.host = host;
        this.app_id = app_id;
    }

    public AtgenMultiServer() { }
}

[MessagePackObject(true)]
public class AtgenNAccountInfo
{
    public string email { get; set; }
    public string nickname { get; set; }

    public AtgenNAccountInfo(string email, string nickname)
    {
        this.email = email;
        this.nickname = nickname;
    }

    public AtgenNAccountInfo() { }
}

[MessagePackObject(true)]
public class AtgenNeedTradeEntityList
{
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }
    public int entity_quantity { get; set; }
    public int limit_break_count { get; set; }

    public AtgenNeedTradeEntityList(
        EntityTypes entity_type,
        int entity_id,
        int entity_quantity,
        int limit_break_count
    )
    {
        this.entity_type = entity_type;
        this.entity_id = entity_id;
        this.entity_quantity = entity_quantity;
        this.limit_break_count = limit_break_count;
    }

    public AtgenNeedTradeEntityList() { }
}

[MessagePackObject(true)]
public class AtgenNeedUnitList
{
    public int type { get; set; }
    public ulong key_id { get; set; }

    public AtgenNeedUnitList(int type, ulong key_id)
    {
        this.type = type;
        this.key_id = key_id;
    }

    public AtgenNeedUnitList() { }
}

[MessagePackObject(true)]
public class AtgenNormalMissionNotice
{
    public int is_update { get; set; }
    public int all_mission_count { get; set; }
    public int completed_mission_count { get; set; }
    public int receivable_reward_count { get; set; }
    public int pickup_mission_count { get; set; }
    public int current_mission_id { get; set; }
    public IEnumerable<int> new_complete_mission_id_list { get; set; }

    public AtgenNormalMissionNotice(
        int is_update,
        int all_mission_count,
        int completed_mission_count,
        int receivable_reward_count,
        int pickup_mission_count,
        int current_mission_id,
        IEnumerable<int> new_complete_mission_id_list
    )
    {
        this.is_update = is_update;
        this.all_mission_count = all_mission_count;
        this.completed_mission_count = completed_mission_count;
        this.receivable_reward_count = receivable_reward_count;
        this.pickup_mission_count = pickup_mission_count;
        this.current_mission_id = current_mission_id;
        this.new_complete_mission_id_list = new_complete_mission_id_list;
    }

    public AtgenNormalMissionNotice() { }
}

[MessagePackObject(true)]
public class AtgenNotReceivedMissionIdListWithDayNo
{
    public int day_no { get; set; }
    public IEnumerable<int> not_received_mission_id_list { get; set; }

    public AtgenNotReceivedMissionIdListWithDayNo(
        int day_no,
        IEnumerable<int> not_received_mission_id_list
    )
    {
        this.day_no = day_no;
        this.not_received_mission_id_list = not_received_mission_id_list;
    }

    public AtgenNotReceivedMissionIdListWithDayNo() { }
}

[MessagePackObject(true)]
public class AtgenOpinionList
{
    public string opinion_id { get; set; }
    public string opinion_text { get; set; }
    public int created_at { get; set; }
    public int updated_at { get; set; }

    public AtgenOpinionList(string opinion_id, string opinion_text, int created_at, int updated_at)
    {
        this.opinion_id = opinion_id;
        this.opinion_text = opinion_text;
        this.created_at = created_at;
        this.updated_at = updated_at;
    }

    public AtgenOpinionList() { }
}

[MessagePackObject(true)]
public class AtgenOpinionTypeList
{
    public int opinion_type { get; set; }
    public string name { get; set; }

    public AtgenOpinionTypeList(int opinion_type, string name)
    {
        this.opinion_type = opinion_type;
        this.name = name;
    }

    public AtgenOpinionTypeList() { }
}

[MessagePackObject(true)]
public class AtgenOption
{
    public int strength_param_id { get; set; }
    public int strength_ability_id { get; set; }
    public int strength_skill_id { get; set; }
    public int abnormal_status_invalid_count { get; set; }

    public AtgenOption(
        int strength_param_id,
        int strength_ability_id,
        int strength_skill_id,
        int abnormal_status_invalid_count
    )
    {
        this.strength_param_id = strength_param_id;
        this.strength_ability_id = strength_ability_id;
        this.strength_skill_id = strength_skill_id;
        this.abnormal_status_invalid_count = abnormal_status_invalid_count;
    }

    public AtgenOption() { }
}

[MessagePackObject(true)]
public class AtgenOwnDamageRankingList
{
    public int rank { get; set; }
    public int is_new { get; set; }
    public int chara_id { get; set; }
    public long damage_value { get; set; }

    public AtgenOwnDamageRankingList(int rank, int is_new, int chara_id, long damage_value)
    {
        this.rank = rank;
        this.is_new = is_new;
        this.chara_id = chara_id;
        this.damage_value = damage_value;
    }

    public AtgenOwnDamageRankingList() { }
}

[MessagePackObject(true)]
public class AtgenOwnRankingList
{
    public int rank { get; set; }
    public int is_new { get; set; }
    public int ranking_id { get; set; }
    public ulong viewer_id { get; set; }
    public int quest_id { get; set; }
    public float clear_time { get; set; }
    public int start_time { get; set; }
    public int end_time { get; set; }
    public string party_hash { get; set; }
    public ulong viewer_id_1 { get; set; }
    public ulong viewer_id_2 { get; set; }
    public ulong viewer_id_3 { get; set; }
    public ulong viewer_id_4 { get; set; }
    public int chara_id_1 { get; set; }
    public int chara_id_2 { get; set; }
    public int chara_id_3 { get; set; }
    public int chara_id_4 { get; set; }
    public int chara_rarity_1 { get; set; }
    public int chara_rarity_2 { get; set; }
    public int chara_rarity_3 { get; set; }
    public int chara_rarity_4 { get; set; }
    public int chara_level_1 { get; set; }
    public int chara_level_2 { get; set; }
    public int chara_level_3 { get; set; }
    public int chara_level_4 { get; set; }

    public AtgenOwnRankingList(
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
    )
    {
        this.rank = rank;
        this.is_new = is_new;
        this.ranking_id = ranking_id;
        this.viewer_id = viewer_id;
        this.quest_id = quest_id;
        this.clear_time = clear_time;
        this.start_time = start_time;
        this.end_time = end_time;
        this.party_hash = party_hash;
        this.viewer_id_1 = viewer_id_1;
        this.viewer_id_2 = viewer_id_2;
        this.viewer_id_3 = viewer_id_3;
        this.viewer_id_4 = viewer_id_4;
        this.chara_id_1 = chara_id_1;
        this.chara_id_2 = chara_id_2;
        this.chara_id_3 = chara_id_3;
        this.chara_id_4 = chara_id_4;
        this.chara_rarity_1 = chara_rarity_1;
        this.chara_rarity_2 = chara_rarity_2;
        this.chara_rarity_3 = chara_rarity_3;
        this.chara_rarity_4 = chara_rarity_4;
        this.chara_level_1 = chara_level_1;
        this.chara_level_2 = chara_level_2;
        this.chara_level_3 = chara_level_3;
        this.chara_level_4 = chara_level_4;
    }

    public AtgenOwnRankingList() { }
}

[MessagePackObject(true)]
public class AtgenPaid
{
    public string code { get; set; }
    public int total { get; set; }

    public AtgenPaid(string code, int total)
    {
        this.code = code;
        this.total = total;
    }

    public AtgenPaid() { }
}

[MessagePackObject(true)]
public class AtgenParamBonus
{
    public int weapon_type { get; set; }
    public float hp { get; set; }
    public float attack { get; set; }

    public AtgenParamBonus(int weapon_type, float hp, float attack)
    {
        this.weapon_type = weapon_type;
        this.hp = hp;
        this.attack = attack;
    }

    public AtgenParamBonus() { }
}

[MessagePackObject(true)]
public class AtgenPenaltyData
{
    public int report_id { get; set; }
    public int point { get; set; }
    public int penalty_type { get; set; }
    public int penalty_text_type { get; set; }
    public string penalty_body { get; set; }

    public AtgenPenaltyData(
        int report_id,
        int point,
        int penalty_type,
        int penalty_text_type,
        string penalty_body
    )
    {
        this.report_id = report_id;
        this.point = point;
        this.penalty_type = penalty_type;
        this.penalty_text_type = penalty_text_type;
        this.penalty_body = penalty_body;
    }

    public AtgenPenaltyData() { }
}

[MessagePackObject(true)]
public class AtgenPlayWallDetail
{
    public int wall_id { get; set; }
    public int after_wall_level { get; set; }
    public int before_wall_level { get; set; }

    public AtgenPlayWallDetail(int wall_id, int after_wall_level, int before_wall_level)
    {
        this.wall_id = wall_id;
        this.after_wall_level = after_wall_level;
        this.before_wall_level = before_wall_level;
    }

    public AtgenPlayWallDetail() { }
}

[MessagePackObject(true)]
public class AtgenPlusCountParamsList
{
    public int plus_count_type { get; set; }
    public int plus_count { get; set; }

    public AtgenPlusCountParamsList(int plus_count_type, int plus_count)
    {
        this.plus_count_type = plus_count_type;
        this.plus_count = plus_count;
    }

    public AtgenPlusCountParamsList() { }
}

[MessagePackObject(true)]
public class AtgenProductionRp
{
    public float speed { get; set; }
    public int max { get; set; }

    public AtgenProductionRp(float speed, int max)
    {
        this.speed = speed;
        this.max = max;
    }

    public AtgenProductionRp() { }
}

[MessagePackObject(true)]
public class AtgenProductLockList
{
    public int shop_type { get; set; }
    public int goods_id { get; set; }
    public int is_lock { get; set; }
    public int expire_time { get; set; }

    public AtgenProductLockList(int shop_type, int goods_id, int is_lock, int expire_time)
    {
        this.shop_type = shop_type;
        this.goods_id = goods_id;
        this.is_lock = is_lock;
        this.expire_time = expire_time;
    }

    public AtgenProductLockList() { }
}

[MessagePackObject(true)]
public class AtgenQuestBonus
{
    public int goods_id { get; set; }
    public int effect_start_time { get; set; }
    public int effect_end_time { get; set; }

    public AtgenQuestBonus(int goods_id, int effect_start_time, int effect_end_time)
    {
        this.goods_id = goods_id;
        this.effect_start_time = effect_start_time;
        this.effect_end_time = effect_end_time;
    }

    public AtgenQuestBonus() { }
}

[MessagePackObject(true)]
public class AtgenQuestDropInfo
{
    public IEnumerable<AtgenDuplicateEntityList> drop_info_list { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> host_drop_info_list { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> fever_drop_info_list { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> quest_bonus_info_list { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> campaign_extra_reward_info_list { get; set; }
    public IEnumerable<AtgenQuestRebornBonusInfoList> quest_reborn_bonus_info_list { get; set; }

    public AtgenQuestDropInfo(
        IEnumerable<AtgenDuplicateEntityList> drop_info_list,
        IEnumerable<AtgenDuplicateEntityList> host_drop_info_list,
        IEnumerable<AtgenDuplicateEntityList> fever_drop_info_list,
        IEnumerable<AtgenDuplicateEntityList> quest_bonus_info_list,
        IEnumerable<AtgenDuplicateEntityList> campaign_extra_reward_info_list,
        IEnumerable<AtgenQuestRebornBonusInfoList> quest_reborn_bonus_info_list
    )
    {
        this.drop_info_list = drop_info_list;
        this.host_drop_info_list = host_drop_info_list;
        this.fever_drop_info_list = fever_drop_info_list;
        this.quest_bonus_info_list = quest_bonus_info_list;
        this.campaign_extra_reward_info_list = campaign_extra_reward_info_list;
        this.quest_reborn_bonus_info_list = quest_reborn_bonus_info_list;
    }

    public AtgenQuestDropInfo() { }
}

[MessagePackObject(true)]
public class AtgenQuestRebornBonusInfoList
{
    public int reborn_count { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> bonus_info_list { get; set; }

    public AtgenQuestRebornBonusInfoList(
        int reborn_count,
        IEnumerable<AtgenDuplicateEntityList> bonus_info_list
    )
    {
        this.reborn_count = reborn_count;
        this.bonus_info_list = bonus_info_list;
    }

    public AtgenQuestRebornBonusInfoList() { }
}

[MessagePackObject(true)]
public class AtgenQuestStoryRewardList
{
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }
    public int entity_quantity { get; set; }
    public int entity_level { get; set; }
    public int entity_limit_break_count { get; set; }

    public AtgenQuestStoryRewardList(
        EntityTypes entity_type,
        int entity_id,
        int entity_quantity,
        int entity_level,
        int entity_limit_break_count
    )
    {
        this.entity_type = entity_type;
        this.entity_id = entity_id;
        this.entity_quantity = entity_quantity;
        this.entity_level = entity_level;
        this.entity_limit_break_count = entity_limit_break_count;
    }

    public AtgenQuestStoryRewardList() { }
}

[MessagePackObject(true)]
public class AtgenRarityGroupList
{
    public bool pickup { get; set; }
    public int rarity { get; set; }
    public string total_rate { get; set; }
    public string chara_rate { get; set; }
    public string dragon_rate { get; set; }
    public string amulet_rate { get; set; }

    public AtgenRarityGroupList(
        bool pickup,
        int rarity,
        string total_rate,
        string chara_rate,
        string dragon_rate,
        string amulet_rate
    )
    {
        this.pickup = pickup;
        this.rarity = rarity;
        this.total_rate = total_rate;
        this.chara_rate = chara_rate;
        this.dragon_rate = dragon_rate;
        this.amulet_rate = amulet_rate;
    }

    public AtgenRarityGroupList() { }
}

[MessagePackObject(true)]
public class AtgenRarityList
{
    public int rarity { get; set; }
    public string total_rate { get; set; }

    public AtgenRarityList(int rarity, string total_rate)
    {
        this.rarity = rarity;
        this.total_rate = total_rate;
    }

    public AtgenRarityList() { }
}

[MessagePackObject(true)]
public class AtgenReceiveQuestBonus
{
    public int target_quest_id { get; set; }
    public int receive_bonus_count { get; set; }
    public float bonus_factor { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> quest_bonus_entity_list { get; set; }

    public AtgenReceiveQuestBonus(
        int target_quest_id,
        int receive_bonus_count,
        float bonus_factor,
        IEnumerable<AtgenBuildEventRewardEntityList> quest_bonus_entity_list
    )
    {
        this.target_quest_id = target_quest_id;
        this.receive_bonus_count = receive_bonus_count;
        this.bonus_factor = bonus_factor;
        this.quest_bonus_entity_list = quest_bonus_entity_list;
    }

    public AtgenReceiveQuestBonus() { }
}

[MessagePackObject(true)]
public class AtgenRecoverData
{
    public int recover_stamina_type { get; set; }
    public int recover_stamina_point { get; set; }

    public AtgenRecoverData(int recover_stamina_type, int recover_stamina_point)
    {
        this.recover_stamina_type = recover_stamina_type;
        this.recover_stamina_point = recover_stamina_point;
    }

    public AtgenRecoverData() { }
}

[MessagePackObject(true)]
public class AtgenRedoableSummonResultUnitList
{
    public EntityTypes entity_type { get; set; }
    public int id { get; set; }
    public int rarity { get; set; }

    public AtgenRedoableSummonResultUnitList(EntityTypes entity_type, int id, int rarity)
    {
        this.entity_type = entity_type;
        this.id = id;
        this.rarity = rarity;
    }

    public AtgenRedoableSummonResultUnitList() { }
}

[MessagePackObject(true)]
public class AtgenRequestAbilityCrestSetData
{
    public int crest_slot_type_1_crest_id_1 { get; set; }
    public int crest_slot_type_1_crest_id_2 { get; set; }
    public int crest_slot_type_1_crest_id_3 { get; set; }
    public int crest_slot_type_2_crest_id_1 { get; set; }
    public int crest_slot_type_2_crest_id_2 { get; set; }
    public int crest_slot_type_3_crest_id_1 { get; set; }
    public int crest_slot_type_3_crest_id_2 { get; set; }
    public ulong talisman_key_id { get; set; }

    public AtgenRequestAbilityCrestSetData(
        int crest_slot_type_1_crest_id_1,
        int crest_slot_type_1_crest_id_2,
        int crest_slot_type_1_crest_id_3,
        int crest_slot_type_2_crest_id_1,
        int crest_slot_type_2_crest_id_2,
        int crest_slot_type_3_crest_id_1,
        int crest_slot_type_3_crest_id_2,
        ulong talisman_key_id
    )
    {
        this.crest_slot_type_1_crest_id_1 = crest_slot_type_1_crest_id_1;
        this.crest_slot_type_1_crest_id_2 = crest_slot_type_1_crest_id_2;
        this.crest_slot_type_1_crest_id_3 = crest_slot_type_1_crest_id_3;
        this.crest_slot_type_2_crest_id_1 = crest_slot_type_2_crest_id_1;
        this.crest_slot_type_2_crest_id_2 = crest_slot_type_2_crest_id_2;
        this.crest_slot_type_3_crest_id_1 = crest_slot_type_3_crest_id_1;
        this.crest_slot_type_3_crest_id_2 = crest_slot_type_3_crest_id_2;
        this.talisman_key_id = talisman_key_id;
    }

    public AtgenRequestAbilityCrestSetData() { }
}

[MessagePackObject(true)]
public class AtgenRequestCharaUnitSetData
{
    public ulong dragon_key_id { get; set; }
    public int weapon_body_id { get; set; }
    public int crest_slot_type_1_crest_id_1 { get; set; }
    public int crest_slot_type_1_crest_id_2 { get; set; }
    public int crest_slot_type_1_crest_id_3 { get; set; }
    public int crest_slot_type_2_crest_id_1 { get; set; }
    public int crest_slot_type_2_crest_id_2 { get; set; }
    public int crest_slot_type_3_crest_id_1 { get; set; }
    public int crest_slot_type_3_crest_id_2 { get; set; }
    public ulong talisman_key_id { get; set; }

    public AtgenRequestCharaUnitSetData(
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
    )
    {
        this.dragon_key_id = dragon_key_id;
        this.weapon_body_id = weapon_body_id;
        this.crest_slot_type_1_crest_id_1 = crest_slot_type_1_crest_id_1;
        this.crest_slot_type_1_crest_id_2 = crest_slot_type_1_crest_id_2;
        this.crest_slot_type_1_crest_id_3 = crest_slot_type_1_crest_id_3;
        this.crest_slot_type_2_crest_id_1 = crest_slot_type_2_crest_id_1;
        this.crest_slot_type_2_crest_id_2 = crest_slot_type_2_crest_id_2;
        this.crest_slot_type_3_crest_id_1 = crest_slot_type_3_crest_id_1;
        this.crest_slot_type_3_crest_id_2 = crest_slot_type_3_crest_id_2;
        this.talisman_key_id = talisman_key_id;
    }

    public AtgenRequestCharaUnitSetData() { }
}

[MessagePackObject(true)]
public class AtgenRequestQuestMultipleList
{
    public int quest_id { get; set; }
    public int play_count { get; set; }
    public int bet_count { get; set; }

    public AtgenRequestQuestMultipleList(int quest_id, int play_count, int bet_count)
    {
        this.quest_id = quest_id;
        this.play_count = play_count;
        this.bet_count = bet_count;
    }

    public AtgenRequestQuestMultipleList() { }
}

[MessagePackObject(true)]
public class AtgenResultPrizeList
{
    public int summon_prize_rank { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> entity_list { get; set; }

    public AtgenResultPrizeList(
        int summon_prize_rank,
        IEnumerable<AtgenBuildEventRewardEntityList> entity_list
    )
    {
        this.summon_prize_rank = summon_prize_rank;
        this.entity_list = entity_list;
    }

    public AtgenResultPrizeList() { }
}

[MessagePackObject(true)]
public class AtgenResultUnitList
{
    public EntityTypes entity_type { get; set; }
    public int id { get; set; }
    public int rarity { get; set; }
    public bool is_new { get; set; }
    public int prev_rarity { get; set; }
    public int dew_point { get; set; }

    public AtgenResultUnitList(
        EntityTypes entity_type,
        int id,
        int rarity,
        bool is_new,
        int prev_rarity,
        int dew_point
    )
    {
        this.entity_type = entity_type;
        this.id = id;
        this.rarity = rarity;
        this.is_new = is_new;
        this.prev_rarity = prev_rarity;
        this.dew_point = dew_point;
    }

    public AtgenResultUnitList() { }
}

[MessagePackObject(true)]
public class AtgenRewardTalismanList
{
    public int talisman_id { get; set; }
    public int talisman_ability_id_1 { get; set; }
    public int talisman_ability_id_2 { get; set; }
    public int talisman_ability_id_3 { get; set; }
    public int additional_hp { get; set; }
    public int additional_attack { get; set; }

    public AtgenRewardTalismanList(
        int talisman_id,
        int talisman_ability_id_1,
        int talisman_ability_id_2,
        int talisman_ability_id_3,
        int additional_hp,
        int additional_attack
    )
    {
        this.talisman_id = talisman_id;
        this.talisman_ability_id_1 = talisman_ability_id_1;
        this.talisman_ability_id_2 = talisman_ability_id_2;
        this.talisman_ability_id_3 = talisman_ability_id_3;
        this.additional_hp = additional_hp;
        this.additional_attack = additional_attack;
    }

    public AtgenRewardTalismanList() { }
}

[MessagePackObject(true)]
public class AtgenRoomMemberList
{
    public ulong viewer_id { get; set; }

    public AtgenRoomMemberList(ulong viewer_id)
    {
        this.viewer_id = viewer_id;
    }

    public AtgenRoomMemberList() { }
}

[MessagePackObject(true)]
public class AtgenScoreMissionSuccessList
{
    public int score_mission_complete_type { get; set; }
    public int score_target_value { get; set; }
    public float correction_value { get; set; }

    public AtgenScoreMissionSuccessList(
        int score_mission_complete_type,
        int score_target_value,
        float correction_value
    )
    {
        this.score_mission_complete_type = score_mission_complete_type;
        this.score_target_value = score_target_value;
        this.correction_value = correction_value;
    }

    public AtgenScoreMissionSuccessList() { }
}

[MessagePackObject(true)]
public class AtgenScoringEnemyPointList
{
    public int scoring_enemy_id { get; set; }
    public int smash_count { get; set; }
    public int point { get; set; }

    public AtgenScoringEnemyPointList(int scoring_enemy_id, int smash_count, int point)
    {
        this.scoring_enemy_id = scoring_enemy_id;
        this.smash_count = smash_count;
        this.point = point;
    }

    public AtgenScoringEnemyPointList() { }
}

[MessagePackObject(true)]
public class AtgenShopGiftList
{
    public int dragon_gift_id { get; set; }
    public int price { get; set; }
    public int is_buy { get; set; }

    public AtgenShopGiftList(int dragon_gift_id, int price, int is_buy)
    {
        this.dragon_gift_id = dragon_gift_id;
        this.price = price;
        this.is_buy = is_buy;
    }

    public AtgenShopGiftList() { }
}

[MessagePackObject(true)]
public class AtgenStaminaBonus
{
    public int goods_id { get; set; }
    public int last_bonus_time { get; set; }
    public int effect_start_time { get; set; }
    public int effect_end_time { get; set; }

    public AtgenStaminaBonus(
        int goods_id,
        int last_bonus_time,
        int effect_start_time,
        int effect_end_time
    )
    {
        this.goods_id = goods_id;
        this.last_bonus_time = last_bonus_time;
        this.effect_start_time = effect_start_time;
        this.effect_end_time = effect_end_time;
    }

    public AtgenStaminaBonus() { }
}

[MessagePackObject(true)]
public class AtgenStoneBonus
{
    public int goods_id { get; set; }
    public int bonus_count { get; set; }
    public int last_bonus_time { get; set; }

    public AtgenStoneBonus(int goods_id, int bonus_count, int last_bonus_time)
    {
        this.goods_id = goods_id;
        this.bonus_count = bonus_count;
        this.last_bonus_time = last_bonus_time;
    }

    public AtgenStoneBonus() { }
}

[MessagePackObject(true)]
public class AtgenSummonPointTradeList
{
    public int trade_id { get; set; }
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }

    public AtgenSummonPointTradeList(int trade_id, EntityTypes entity_type, int entity_id)
    {
        this.trade_id = trade_id;
        this.entity_type = entity_type;
        this.entity_id = entity_id;
    }

    public AtgenSummonPointTradeList() { }
}

[MessagePackObject(true)]
public class AtgenSummonPrizeEntitySetList
{
    public int summon_prize_rank { get; set; }
    public string rate { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> entity_list { get; set; }

    public AtgenSummonPrizeEntitySetList(
        int summon_prize_rank,
        string rate,
        IEnumerable<AtgenBuildEventRewardEntityList> entity_list
    )
    {
        this.summon_prize_rank = summon_prize_rank;
        this.rate = rate;
        this.entity_list = entity_list;
    }

    public AtgenSummonPrizeEntitySetList() { }
}

[MessagePackObject(true)]
public class AtgenSummonPrizeRankList
{
    public int summon_prize_rank { get; set; }
    public string total_rate { get; set; }

    public AtgenSummonPrizeRankList(int summon_prize_rank, string total_rate)
    {
        this.summon_prize_rank = summon_prize_rank;
        this.total_rate = total_rate;
    }

    public AtgenSummonPrizeRankList() { }
}

[MessagePackObject(true)]
public class AtgenSupportAmulet
{
    public ulong amulet_key_id { get; set; }
    public int amulet_id { get; set; }
    public int level { get; set; }
    public int attack { get; set; }
    public int ability_1_level { get; set; }
    public int ability_2_level { get; set; }
    public int ability_3_level { get; set; }
    public int hp_plus_count { get; set; }
    public int attack_plus_count { get; set; }
    public int status_plus_count { get; set; }
    public int limit_break_count { get; set; }

    public AtgenSupportAmulet(
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
    )
    {
        this.amulet_key_id = amulet_key_id;
        this.amulet_id = amulet_id;
        this.level = level;
        this.attack = attack;
        this.ability_1_level = ability_1_level;
        this.ability_2_level = ability_2_level;
        this.ability_3_level = ability_3_level;
        this.hp_plus_count = hp_plus_count;
        this.attack_plus_count = attack_plus_count;
        this.status_plus_count = status_plus_count;
        this.limit_break_count = limit_break_count;
    }

    public AtgenSupportAmulet() { }
}

[MessagePackObject(true)]
public class AtgenSupportChara
{
    public Charas chara_id { get; set; }
    public int level { get; set; }
    public int additional_max_level { get; set; }
    public int rarity { get; set; }
    public int hp { get; set; }
    public int attack { get; set; }
    public int hp_plus_count { get; set; }
    public int attack_plus_count { get; set; }
    public int status_plus_count { get; set; }
    public int ability_1_level { get; set; }
    public int ability_2_level { get; set; }
    public int ability_3_level { get; set; }
    public int ex_ability_level { get; set; }
    public int ex_ability_2_level { get; set; }
    public int skill_1_level { get; set; }
    public int skill_2_level { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_unlock_edit_skill { get; set; }

    public AtgenSupportChara(
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
        bool is_unlock_edit_skill
    )
    {
        this.chara_id = chara_id;
        this.level = level;
        this.additional_max_level = additional_max_level;
        this.rarity = rarity;
        this.hp = hp;
        this.attack = attack;
        this.hp_plus_count = hp_plus_count;
        this.attack_plus_count = attack_plus_count;
        this.status_plus_count = status_plus_count;
        this.ability_1_level = ability_1_level;
        this.ability_2_level = ability_2_level;
        this.ability_3_level = ability_3_level;
        this.ex_ability_level = ex_ability_level;
        this.ex_ability_2_level = ex_ability_2_level;
        this.skill_1_level = skill_1_level;
        this.skill_2_level = skill_2_level;
        this.is_unlock_edit_skill = is_unlock_edit_skill;
    }

    public AtgenSupportChara() { }
}

[MessagePackObject(true)]
public class AtgenSupportCrestSlotType1List
{
    public AbilityCrests ability_crest_id { get; set; }
    public int buildup_count { get; set; }
    public int limit_break_count { get; set; }
    public int hp_plus_count { get; set; }
    public int attack_plus_count { get; set; }
    public int equipable_count { get; set; }

    public AtgenSupportCrestSlotType1List(
        AbilityCrests ability_crest_id,
        int buildup_count,
        int limit_break_count,
        int hp_plus_count,
        int attack_plus_count,
        int equipable_count
    )
    {
        this.ability_crest_id = ability_crest_id;
        this.buildup_count = buildup_count;
        this.limit_break_count = limit_break_count;
        this.hp_plus_count = hp_plus_count;
        this.attack_plus_count = attack_plus_count;
        this.equipable_count = equipable_count;
    }

    public AtgenSupportCrestSlotType1List() { }
}

[MessagePackObject(true)]
public class AtgenSupportData
{
    public ulong viewer_id { get; set; }
    public string name { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_friend { get; set; }
    public CharaList chara_data { get; set; }
    public DragonList dragon_data { get; set; }
    public WeaponList weapon_data { get; set; }
    public AmuletList amulet_data { get; set; }
    public AmuletList amulet_2_data { get; set; }
    public GameWeaponSkin weapon_skin_data { get; set; }
    public GameWeaponBody weapon_body_data { get; set; }
    public IEnumerable<GameAbilityCrest> crest_slot_type_1_crest_list { get; set; }
    public IEnumerable<GameAbilityCrest> crest_slot_type_2_crest_list { get; set; }
    public IEnumerable<GameAbilityCrest> crest_slot_type_3_crest_list { get; set; }
    public TalismanList talisman_data { get; set; }
    public IEnumerable<WeaponPassiveAbilityList> game_weapon_passive_ability_list { get; set; }

    public AtgenSupportData(
        ulong viewer_id,
        string name,
        bool is_friend,
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
    )
    {
        this.viewer_id = viewer_id;
        this.name = name;
        this.is_friend = is_friend;
        this.chara_data = chara_data;
        this.dragon_data = dragon_data;
        this.weapon_data = weapon_data;
        this.amulet_data = amulet_data;
        this.amulet_2_data = amulet_2_data;
        this.weapon_skin_data = weapon_skin_data;
        this.weapon_body_data = weapon_body_data;
        this.crest_slot_type_1_crest_list = crest_slot_type_1_crest_list;
        this.crest_slot_type_2_crest_list = crest_slot_type_2_crest_list;
        this.crest_slot_type_3_crest_list = crest_slot_type_3_crest_list;
        this.talisman_data = talisman_data;
        this.game_weapon_passive_ability_list = game_weapon_passive_ability_list;
    }

    public AtgenSupportData() { }
}

[MessagePackObject(true)]
public class AtgenSupportDragon
{
    public ulong dragon_key_id { get; set; }
    public Dragons dragon_id { get; set; }
    public int level { get; set; }
    public int hp { get; set; }
    public int attack { get; set; }
    public int skill_1_level { get; set; }
    public int ability_1_level { get; set; }
    public int ability_2_level { get; set; }
    public int hp_plus_count { get; set; }
    public int attack_plus_count { get; set; }
    public int status_plus_count { get; set; }
    public int limit_break_count { get; set; }

    public AtgenSupportDragon(
        ulong dragon_key_id,
        Dragons dragon_id,
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
    )
    {
        this.dragon_key_id = dragon_key_id;
        this.dragon_id = dragon_id;
        this.level = level;
        this.hp = hp;
        this.attack = attack;
        this.skill_1_level = skill_1_level;
        this.ability_1_level = ability_1_level;
        this.ability_2_level = ability_2_level;
        this.hp_plus_count = hp_plus_count;
        this.attack_plus_count = attack_plus_count;
        this.status_plus_count = status_plus_count;
        this.limit_break_count = limit_break_count;
    }

    public AtgenSupportDragon() { }
}

[MessagePackObject(true)]
public class AtgenSupportReward
{
    public int serve_count { get; set; }
    public int mana_point { get; set; }

    public AtgenSupportReward(int serve_count, int mana_point)
    {
        this.serve_count = serve_count;
        this.mana_point = mana_point;
    }

    public AtgenSupportReward() { }
}

[MessagePackObject(true)]
public class AtgenSupportTalisman
{
    public ulong talisman_key_id { get; set; }
    public Talismans talisman_id { get; set; }
    public int talisman_ability_id_1 { get; set; }
    public int talisman_ability_id_2 { get; set; }
    public int talisman_ability_id_3 { get; set; }
    public int additional_hp { get; set; }
    public int additional_attack { get; set; }

    public AtgenSupportTalisman(
        ulong talisman_key_id,
        Talismans talisman_id,
        int talisman_ability_id_1,
        int talisman_ability_id_2,
        int talisman_ability_id_3,
        int additional_hp,
        int additional_attack
    )
    {
        this.talisman_key_id = talisman_key_id;
        this.talisman_id = talisman_id;
        this.talisman_ability_id_1 = talisman_ability_id_1;
        this.talisman_ability_id_2 = talisman_ability_id_2;
        this.talisman_ability_id_3 = talisman_ability_id_3;
        this.additional_hp = additional_hp;
        this.additional_attack = additional_attack;
    }

    public AtgenSupportTalisman() { }
}

[MessagePackObject(true)]
public class AtgenSupportUserDataDetail
{
    public UserSupportList user_support_data { get; set; }
    public FortBonusList fort_bonus_list { get; set; }
    public IEnumerable<int> mana_circle_piece_id_list { get; set; }
    public int dragon_reliability_level { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_friend { get; set; }
    public int apply_send_status { get; set; }

    public AtgenSupportUserDataDetail(
        UserSupportList user_support_data,
        FortBonusList fort_bonus_list,
        IEnumerable<int> mana_circle_piece_id_list,
        int dragon_reliability_level,
        bool is_friend,
        int apply_send_status
    )
    {
        this.user_support_data = user_support_data;
        this.fort_bonus_list = fort_bonus_list;
        this.mana_circle_piece_id_list = mana_circle_piece_id_list;
        this.dragon_reliability_level = dragon_reliability_level;
        this.is_friend = is_friend;
        this.apply_send_status = apply_send_status;
    }

    public AtgenSupportUserDataDetail() { }
}

[MessagePackObject(true)]
public class AtgenSupportUserDetailList
{
    public ulong viewer_id { get; set; }
    public int gettable_mana_point { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_friend { get; set; }

    public AtgenSupportUserDetailList(ulong viewer_id, int gettable_mana_point, bool is_friend)
    {
        this.viewer_id = viewer_id;
        this.gettable_mana_point = gettable_mana_point;
        this.is_friend = is_friend;
    }

    public AtgenSupportUserDetailList() { }
}

[MessagePackObject(true)]
public class AtgenSupportWeapon
{
    public ulong weapon_key_id { get; set; }
    public int weapon_id { get; set; }
    public int level { get; set; }
    public int attack { get; set; }
    public int skill_level { get; set; }
    public int hp_plus_count { get; set; }
    public int attack_plus_count { get; set; }
    public int status_plus_count { get; set; }
    public int limit_break_count { get; set; }

    public AtgenSupportWeapon(
        ulong weapon_key_id,
        int weapon_id,
        int level,
        int attack,
        int skill_level,
        int hp_plus_count,
        int attack_plus_count,
        int status_plus_count,
        int limit_break_count
    )
    {
        this.weapon_key_id = weapon_key_id;
        this.weapon_id = weapon_id;
        this.level = level;
        this.attack = attack;
        this.skill_level = skill_level;
        this.hp_plus_count = hp_plus_count;
        this.attack_plus_count = attack_plus_count;
        this.status_plus_count = status_plus_count;
        this.limit_break_count = limit_break_count;
    }

    public AtgenSupportWeapon() { }
}

[MessagePackObject(true)]
public class AtgenSupportWeaponBody
{
    public WeaponBodies weapon_body_id { get; set; }
    public int buildup_count { get; set; }
    public int limit_break_count { get; set; }
    public int limit_over_count { get; set; }
    public int additional_effect_count { get; set; }
    public int equipable_count { get; set; }
    public int additional_crest_slot_type_1_count { get; set; }
    public int additional_crest_slot_type_2_count { get; set; }
    public int additional_crest_slot_type_3_count { get; set; }

    public AtgenSupportWeaponBody(
        WeaponBodies weapon_body_id,
        int buildup_count,
        int limit_break_count,
        int limit_over_count,
        int additional_effect_count,
        int equipable_count,
        int additional_crest_slot_type_1_count,
        int additional_crest_slot_type_2_count,
        int additional_crest_slot_type_3_count
    )
    {
        this.weapon_body_id = weapon_body_id;
        this.buildup_count = buildup_count;
        this.limit_break_count = limit_break_count;
        this.limit_over_count = limit_over_count;
        this.additional_effect_count = additional_effect_count;
        this.equipable_count = equipable_count;
        this.additional_crest_slot_type_1_count = additional_crest_slot_type_1_count;
        this.additional_crest_slot_type_2_count = additional_crest_slot_type_2_count;
        this.additional_crest_slot_type_3_count = additional_crest_slot_type_3_count;
    }

    public AtgenSupportWeaponBody() { }
}

[MessagePackObject(true)]
public class AtgenTargetList
{
    public string target_name { get; set; }
    public IEnumerable<ulong> target_id_list { get; set; }

    public AtgenTargetList(string target_name, IEnumerable<ulong> target_id_list)
    {
        this.target_name = target_name;
        this.target_id_list = target_id_list;
    }

    public AtgenTargetList() { }
}

[MessagePackObject(true)]
public class AtgenTransitionResultData
{
    public ulong abolished_viewer_id { get; set; }
    public ulong linked_viewer_id { get; set; }

    public AtgenTransitionResultData(ulong abolished_viewer_id, ulong linked_viewer_id)
    {
        this.abolished_viewer_id = abolished_viewer_id;
        this.linked_viewer_id = linked_viewer_id;
    }

    public AtgenTransitionResultData() { }
}

[MessagePackObject(true)]
public class AtgenTreasureRecord
{
    public int area_idx { get; set; }
    public IEnumerable<int> drop_obj { get; set; }
    public IEnumerable<int> enemy { get; set; }
    public IEnumerable<AtgenEnemySmash> enemy_smash { get; set; }

    public AtgenTreasureRecord(
        int area_idx,
        IEnumerable<int> drop_obj,
        IEnumerable<int> enemy,
        IEnumerable<AtgenEnemySmash> enemy_smash
    )
    {
        this.area_idx = area_idx;
        this.drop_obj = drop_obj;
        this.enemy = enemy;
        this.enemy_smash = enemy_smash;
    }

    public AtgenTreasureRecord() { }
}

[MessagePackObject(true)]
public class AtgenUnit
{
    public IEnumerable<OddsUnitDetail> chara_odds_list { get; set; }
    public IEnumerable<OddsUnitDetail> dragon_odds_list { get; set; }
    public IEnumerable<OddsUnitDetail> amulet_odds_list { get; set; }

    public AtgenUnit(
        IEnumerable<OddsUnitDetail> chara_odds_list,
        IEnumerable<OddsUnitDetail> dragon_odds_list,
        IEnumerable<OddsUnitDetail> amulet_odds_list
    )
    {
        this.chara_odds_list = chara_odds_list;
        this.dragon_odds_list = dragon_odds_list;
        this.amulet_odds_list = amulet_odds_list;
    }

    public AtgenUnit() { }
}

[MessagePackObject(true)]
public class AtgenUnitData
{
    public int chara_id { get; set; }
    public int skill_1_level { get; set; }
    public int skill_2_level { get; set; }
    public int ability_1_level { get; set; }
    public int ability_2_level { get; set; }
    public int ability_3_level { get; set; }
    public int ex_ability_level { get; set; }
    public int ex_ability_2_level { get; set; }
    public int burst_attack_level { get; set; }
    public int combo_buildup_count { get; set; }

    public AtgenUnitData(
        int chara_id,
        int skill_1_level,
        int skill_2_level,
        int ability_1_level,
        int ability_2_level,
        int ability_3_level,
        int ex_ability_level,
        int ex_ability_2_level,
        int burst_attack_level,
        int combo_buildup_count
    )
    {
        this.chara_id = chara_id;
        this.skill_1_level = skill_1_level;
        this.skill_2_level = skill_2_level;
        this.ability_1_level = ability_1_level;
        this.ability_2_level = ability_2_level;
        this.ability_3_level = ability_3_level;
        this.ex_ability_level = ex_ability_level;
        this.ex_ability_2_level = ex_ability_2_level;
        this.burst_attack_level = burst_attack_level;
        this.combo_buildup_count = combo_buildup_count;
    }

    public AtgenUnitData() { }
}

[MessagePackObject(true)]
public class AtgenUnitList
{
    public int id { get; set; }
    public string rate { get; set; }

    public AtgenUnitList(int id, string rate)
    {
        this.id = id;
        this.rate = rate;
    }

    public AtgenUnitList() { }
}

[MessagePackObject(true)]
public class AtgenUseItemList
{
    public int item_id { get; set; }
    public int item_quantity { get; set; }

    public AtgenUseItemList(int item_id, int item_quantity)
    {
        this.item_id = item_id;
        this.item_quantity = item_quantity;
    }

    public AtgenUseItemList() { }
}

[MessagePackObject(true)]
public class AtgenUserBuildEventItemList
{
    public int user_build_event_item { get; set; }
    public int event_item_value { get; set; }

    public AtgenUserBuildEventItemList(int user_build_event_item, int event_item_value)
    {
        this.user_build_event_item = user_build_event_item;
        this.event_item_value = event_item_value;
    }

    public AtgenUserBuildEventItemList() { }
}

[MessagePackObject(true)]
public class AtgenUserClb01EventItemList
{
    public int user_clb_01_event_item { get; set; }
    public int event_item_value { get; set; }

    public AtgenUserClb01EventItemList(int user_clb_01_event_item, int event_item_value)
    {
        this.user_clb_01_event_item = user_clb_01_event_item;
        this.event_item_value = event_item_value;
    }

    public AtgenUserClb01EventItemList() { }
}

[MessagePackObject(true)]
public class AtgenUserCollectEventItemList
{
    public int user_collect_event_item { get; set; }
    public int event_item_value { get; set; }

    public AtgenUserCollectEventItemList(int user_collect_event_item, int event_item_value)
    {
        this.user_collect_event_item = user_collect_event_item;
        this.event_item_value = event_item_value;
    }

    public AtgenUserCollectEventItemList() { }
}

[MessagePackObject(true)]
public class AtgenUserEventTradeList
{
    public int event_trade_id { get; set; }
    public int trade_count { get; set; }

    public AtgenUserEventTradeList(int event_trade_id, int trade_count)
    {
        this.event_trade_id = event_trade_id;
        this.trade_count = trade_count;
    }

    public AtgenUserEventTradeList() { }
}

[MessagePackObject(true)]
public class AtgenUserItemSummon
{
    public int daily_summon_count { get; set; }
    public int last_summon_time { get; set; }

    public AtgenUserItemSummon(int daily_summon_count, int last_summon_time)
    {
        this.daily_summon_count = daily_summon_count;
        this.last_summon_time = last_summon_time;
    }

    public AtgenUserItemSummon() { }
}

[MessagePackObject(true)]
public class AtgenUserMazeEventItemList
{
    public int user_maze_event_item { get; set; }
    public int event_item_value { get; set; }

    public AtgenUserMazeEventItemList(int user_maze_event_item, int event_item_value)
    {
        this.user_maze_event_item = user_maze_event_item;
        this.event_item_value = event_item_value;
    }

    public AtgenUserMazeEventItemList() { }
}

[MessagePackObject(true)]
public class AtgenUserMazeEventItemList__2
{
    public int event_item_id { get; set; }
    public int quantity { get; set; }

    public AtgenUserMazeEventItemList__2(int event_item_id, int quantity)
    {
        this.event_item_id = event_item_id;
        this.quantity = quantity;
    }

    public AtgenUserMazeEventItemList__2() { }
}

[MessagePackObject(true)]
public class AtgenUserWallRewardList
{
    public int quest_group_id { get; set; }
    public int sum_wall_level { get; set; }
    public int last_reward_date { get; set; }
    public int reward_status { get; set; }

    public AtgenUserWallRewardList(
        int quest_group_id,
        int sum_wall_level,
        int last_reward_date,
        int reward_status
    )
    {
        this.quest_group_id = quest_group_id;
        this.sum_wall_level = sum_wall_level;
        this.last_reward_date = last_reward_date;
        this.reward_status = reward_status;
    }

    public AtgenUserWallRewardList() { }
}

[MessagePackObject(true)]
public class AtgenVersionHash
{
    public string region { get; set; }
    public string lang { get; set; }
    public int eula_version { get; set; }
    public int privacy_policy_version { get; set; }

    public AtgenVersionHash(
        string region,
        string lang,
        int eula_version,
        int privacy_policy_version
    )
    {
        this.region = region;
        this.lang = lang;
        this.eula_version = eula_version;
        this.privacy_policy_version = privacy_policy_version;
    }

    public AtgenVersionHash() { }
}

[MessagePackObject(true)]
public class AtgenWalkerData
{
    public int reliability_level { get; set; }
    public int reliability_total_exp { get; set; }
    public int last_contact_time { get; set; }
    public int skill_2_level { get; set; }

    public AtgenWalkerData(
        int reliability_level,
        int reliability_total_exp,
        int last_contact_time,
        int skill_2_level
    )
    {
        this.reliability_level = reliability_level;
        this.reliability_total_exp = reliability_total_exp;
        this.last_contact_time = last_contact_time;
        this.skill_2_level = skill_2_level;
    }

    public AtgenWalkerData() { }
}

[MessagePackObject(true)]
public class AtgenWallDropReward
{
    public IEnumerable<AtgenBuildEventRewardEntityList> reward_entity_list { get; set; }
    public int take_coin { get; set; }
    public int take_mana { get; set; }

    public AtgenWallDropReward(
        IEnumerable<AtgenBuildEventRewardEntityList> reward_entity_list,
        int take_coin,
        int take_mana
    )
    {
        this.reward_entity_list = reward_entity_list;
        this.take_coin = take_coin;
        this.take_mana = take_mana;
    }

    public AtgenWallDropReward() { }
}

[MessagePackObject(true)]
public class AtgenWallUnitInfo
{
    public IEnumerable<PartySettingList> quest_party_setting_list { get; set; }
    public IEnumerable<UserSupportList> helper_list { get; set; }
    public IEnumerable<AtgenHelperDetailList> helper_detail_list { get; set; }

    public AtgenWallUnitInfo(
        IEnumerable<PartySettingList> quest_party_setting_list,
        IEnumerable<UserSupportList> helper_list,
        IEnumerable<AtgenHelperDetailList> helper_detail_list
    )
    {
        this.quest_party_setting_list = quest_party_setting_list;
        this.helper_list = helper_list;
        this.helper_detail_list = helper_detail_list;
    }

    public AtgenWallUnitInfo() { }
}

[MessagePackObject(true)]
public class AtgenWeaponKeyDataList
{
    public ulong key_id { get; set; }
    public int target_set_num { get; set; }
    public int target_weapon_num { get; set; }

    public AtgenWeaponKeyDataList(ulong key_id, int target_set_num, int target_weapon_num)
    {
        this.key_id = key_id;
        this.target_set_num = target_set_num;
        this.target_weapon_num = target_weapon_num;
    }

    public AtgenWeaponKeyDataList() { }
}

[MessagePackObject(true)]
public class AtgenWeaponSetList
{
    public int select_weapon_id { get; set; }
    public IEnumerable<AtgenWeaponKeyDataList> weapon_key_data_list { get; set; }

    public AtgenWeaponSetList(
        int select_weapon_id,
        IEnumerable<AtgenWeaponKeyDataList> weapon_key_data_list
    )
    {
        this.select_weapon_id = select_weapon_id;
        this.weapon_key_data_list = weapon_key_data_list;
    }

    public AtgenWeaponSetList() { }
}

[MessagePackObject(true)]
public class AtgenWebviewUrlList
{
    public string function_name { get; set; }
    public string url { get; set; }

    public AtgenWebviewUrlList(string function_name, string url)
    {
        this.function_name = function_name;
        this.url = url;
    }

    public AtgenWebviewUrlList() { }
}

[MessagePackObject(true)]
public class BattleRoyalCharaSkinList
{
    public int battle_royal_chara_skin_id { get; set; }
    public int gettime { get; set; }

    public BattleRoyalCharaSkinList(int battle_royal_chara_skin_id, int gettime)
    {
        this.battle_royal_chara_skin_id = battle_royal_chara_skin_id;
        this.gettime = gettime;
    }

    public BattleRoyalCharaSkinList() { }
}

[MessagePackObject(true)]
public class BattleRoyalCycleUserRecord
{
    public int event_id { get; set; }
    public int event_cycle_id { get; set; }
    public int cycle_total_battle_royal_point { get; set; }

    public BattleRoyalCycleUserRecord(
        int event_id,
        int event_cycle_id,
        int cycle_total_battle_royal_point
    )
    {
        this.event_id = event_id;
        this.event_cycle_id = event_cycle_id;
        this.cycle_total_battle_royal_point = cycle_total_battle_royal_point;
    }

    public BattleRoyalCycleUserRecord() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventItemList
{
    public int event_id { get; set; }
    public int item_id { get; set; }
    public int quantity { get; set; }

    public BattleRoyalEventItemList(int event_id, int item_id, int quantity)
    {
        this.event_id = event_id;
        this.item_id = item_id;
        this.quantity = quantity;
    }

    public BattleRoyalEventItemList() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventUserRecord
{
    public int event_id { get; set; }
    public int total_battle_royal_point { get; set; }
    public int rank_top_count { get; set; }
    public int top_four_count { get; set; }
    public int total_kill_count { get; set; }
    public int max_kill_count { get; set; }
    public int total_assist_count { get; set; }
    public int max_assist_count { get; set; }
    public int last_use_weapon_type { get; set; }
    public int use_swd_count { get; set; }
    public int use_kat_count { get; set; }
    public int use_dag_count { get; set; }
    public int use_axe_count { get; set; }
    public int use_lan_count { get; set; }
    public int use_bow_count { get; set; }
    public int use_rod_count { get; set; }
    public int use_can_count { get; set; }
    public int use_gun_count { get; set; }

    public BattleRoyalEventUserRecord(
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
    )
    {
        this.event_id = event_id;
        this.total_battle_royal_point = total_battle_royal_point;
        this.rank_top_count = rank_top_count;
        this.top_four_count = top_four_count;
        this.total_kill_count = total_kill_count;
        this.max_kill_count = max_kill_count;
        this.total_assist_count = total_assist_count;
        this.max_assist_count = max_assist_count;
        this.last_use_weapon_type = last_use_weapon_type;
        this.use_swd_count = use_swd_count;
        this.use_kat_count = use_kat_count;
        this.use_dag_count = use_dag_count;
        this.use_axe_count = use_axe_count;
        this.use_lan_count = use_lan_count;
        this.use_bow_count = use_bow_count;
        this.use_rod_count = use_rod_count;
        this.use_can_count = use_can_count;
        this.use_gun_count = use_gun_count;
    }

    public BattleRoyalEventUserRecord() { }
}

[MessagePackObject(true)]
public class BattleRoyalResult
{
    public int battle_royal_cycle_id { get; set; }
    public int chara_id { get; set; }
    public int weapon_skin_id { get; set; }
    public int ranking { get; set; }
    public int kill_count { get; set; }
    public int assist_count { get; set; }
    public int take_exp { get; set; }
    public int player_level_up_fstone { get; set; }
    public int take_accumulate_point { get; set; }
    public int take_battle_royal_point { get; set; }

    public BattleRoyalResult(
        int battle_royal_cycle_id,
        int chara_id,
        int weapon_skin_id,
        int ranking,
        int kill_count,
        int assist_count,
        int take_exp,
        int player_level_up_fstone,
        int take_accumulate_point,
        int take_battle_royal_point
    )
    {
        this.battle_royal_cycle_id = battle_royal_cycle_id;
        this.chara_id = chara_id;
        this.weapon_skin_id = weapon_skin_id;
        this.ranking = ranking;
        this.kill_count = kill_count;
        this.assist_count = assist_count;
        this.take_exp = take_exp;
        this.player_level_up_fstone = player_level_up_fstone;
        this.take_accumulate_point = take_accumulate_point;
        this.take_battle_royal_point = take_battle_royal_point;
    }

    public BattleRoyalResult() { }
}

[MessagePackObject(true)]
public class BeginnerMissionList
{
    public int beginner_mission_id { get; set; }
    public int progress { get; set; }
    public int state { get; set; }
    public int end_date { get; set; }
    public int start_date { get; set; }

    public BeginnerMissionList(
        int beginner_mission_id,
        int progress,
        int state,
        int end_date,
        int start_date
    )
    {
        this.beginner_mission_id = beginner_mission_id;
        this.progress = progress;
        this.state = state;
        this.end_date = end_date;
        this.start_date = start_date;
    }

    public BeginnerMissionList() { }
}

[MessagePackObject(true)]
public class BuildEventRewardList
{
    public int event_id { get; set; }
    public int event_reward_id { get; set; }

    public BuildEventRewardList(int event_id, int event_reward_id)
    {
        this.event_id = event_id;
        this.event_reward_id = event_reward_id;
    }

    public BuildEventRewardList() { }
}

[MessagePackObject(true)]
public class BuildEventUserList
{
    public int build_event_id { get; set; }
    public IEnumerable<AtgenUserBuildEventItemList> user_build_event_item_list { get; set; }

    public BuildEventUserList(
        int build_event_id,
        IEnumerable<AtgenUserBuildEventItemList> user_build_event_item_list
    )
    {
        this.build_event_id = build_event_id;
        this.user_build_event_item_list = user_build_event_item_list;
    }

    public BuildEventUserList() { }
}

[MessagePackObject(true)]
public class BuildList
{
    public ulong build_id { get; set; }
    public FortPlants plant_id { get; set; }
    public int level { get; set; }
    public int fort_plant_detail_id { get; set; }
    public int position_x { get; set; }
    public int position_z { get; set; }
    public FortBuildStatus build_status { get; set; }
    public DateTimeOffset build_start_date { get; set; }
    public DateTimeOffset build_end_date { get; set; }
    public TimeSpan remain_time { get; set; }
    public TimeSpan last_income_time { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool is_new { get; set; }

    public BuildList(
        ulong build_id,
        FortPlants plant_id,
        int level,
        int fort_plant_detail_id,
        int position_x,
        int position_z,
        FortBuildStatus build_status,
        DateTimeOffset build_start_date,
        DateTimeOffset build_end_date,
        TimeSpan remain_time,
        TimeSpan last_income_time,
        bool is_new
    )
    {
        this.build_id = build_id;
        this.plant_id = plant_id;
        this.level = level;
        this.fort_plant_detail_id = fort_plant_detail_id;
        this.position_x = position_x;
        this.position_z = position_z;
        this.build_status = build_status;
        this.build_start_date = build_start_date;
        this.build_end_date = build_end_date;
        this.remain_time = remain_time;
        this.last_income_time = last_income_time;
        this.is_new = is_new;
    }

    public BuildList() { }
}

[MessagePackObject(true)]
public class CastleStoryList
{
    public int castle_story_id { get; set; }
    public int is_read { get; set; }

    public CastleStoryList(int castle_story_id, int is_read)
    {
        this.castle_story_id = castle_story_id;
        this.is_read = is_read;
    }

    public CastleStoryList() { }
}

[MessagePackObject(true)]
public class CharaFriendshipList
{
    public int chara_id { get; set; }
    public int add_point { get; set; }
    public int total_point { get; set; }
    public int is_temporary { get; set; }

    public CharaFriendshipList(int chara_id, int add_point, int total_point, int is_temporary)
    {
        this.chara_id = chara_id;
        this.add_point = add_point;
        this.total_point = total_point;
        this.is_temporary = is_temporary;
    }

    public CharaFriendshipList() { }
}

[MessagePackObject(true)]
public class CharaList
{
    public Charas chara_id { get; set; }
    public int exp { get; set; }
    public int level { get; set; }
    public int additional_max_level { get; set; }
    public int hp { get; set; }
    public int attack { get; set; }
    public int ex_ability_level { get; set; }
    public int ex_ability_2_level { get; set; }
    public int ability_1_level { get; set; }
    public int ability_2_level { get; set; }
    public int ability_3_level { get; set; }
    public int is_new { get; set; }
    public int skill_1_level { get; set; }
    public int skill_2_level { get; set; }
    public int burst_attack_level { get; set; }
    public int rarity { get; set; }
    public int limit_break_count { get; set; }
    public int hp_plus_count { get; set; }
    public int attack_plus_count { get; set; }
    public int status_plus_count { get; set; }
    public int combo_buildup_count { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool is_unlock_edit_skill { get; set; }
    public DateTimeOffset gettime { get; set; }
    public IEnumerable<int> mana_circle_piece_id_list { get; set; }
    public int is_temporary { get; set; }
    public int list_view_flag { get; set; }

    public CharaList(
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
        bool is_unlock_edit_skill,
        DateTimeOffset gettime,
        IEnumerable<int> mana_circle_piece_id_list,
        int is_temporary,
        int list_view_flag
    )
    {
        this.chara_id = chara_id;
        this.exp = exp;
        this.level = level;
        this.additional_max_level = additional_max_level;
        this.hp = hp;
        this.attack = attack;
        this.ex_ability_level = ex_ability_level;
        this.ex_ability_2_level = ex_ability_2_level;
        this.ability_1_level = ability_1_level;
        this.ability_2_level = ability_2_level;
        this.ability_3_level = ability_3_level;
        this.is_new = is_new;
        this.skill_1_level = skill_1_level;
        this.skill_2_level = skill_2_level;
        this.burst_attack_level = burst_attack_level;
        this.rarity = rarity;
        this.limit_break_count = limit_break_count;
        this.hp_plus_count = hp_plus_count;
        this.attack_plus_count = attack_plus_count;
        this.status_plus_count = status_plus_count;
        this.combo_buildup_count = combo_buildup_count;
        this.is_unlock_edit_skill = is_unlock_edit_skill;
        this.gettime = gettime;
        this.mana_circle_piece_id_list = mana_circle_piece_id_list;
        this.is_temporary = is_temporary;
        this.list_view_flag = list_view_flag;
    }

    public CharaList() { }
}

[MessagePackObject(true)]
public class CharaUnitSetList
{
    public int chara_id { get; set; }
    public IEnumerable<AtgenCharaUnitSetDetailList> chara_unit_set_detail_list { get; set; }

    public CharaUnitSetList(
        int chara_id,
        IEnumerable<AtgenCharaUnitSetDetailList> chara_unit_set_detail_list
    )
    {
        this.chara_id = chara_id;
        this.chara_unit_set_detail_list = chara_unit_set_detail_list;
    }

    public CharaUnitSetList() { }
}

[MessagePackObject(true)]
public class Clb01EventUserList
{
    public int event_id { get; set; }
    public IEnumerable<AtgenUserClb01EventItemList> user_clb_01_event_item_list { get; set; }

    public Clb01EventUserList(
        int event_id,
        IEnumerable<AtgenUserClb01EventItemList> user_clb_01_event_item_list
    )
    {
        this.event_id = event_id;
        this.user_clb_01_event_item_list = user_clb_01_event_item_list;
    }

    public Clb01EventUserList() { }
}

[MessagePackObject(true)]
public class CollectEventUserList
{
    public int event_id { get; set; }
    public IEnumerable<AtgenUserCollectEventItemList> user_collect_event_item_list { get; set; }

    public CollectEventUserList(
        int event_id,
        IEnumerable<AtgenUserCollectEventItemList> user_collect_event_item_list
    )
    {
        this.event_id = event_id;
        this.user_collect_event_item_list = user_collect_event_item_list;
    }

    public CollectEventUserList() { }
}

[MessagePackObject(true)]
public class CombatEventUserList
{
    public int event_id { get; set; }
    public int event_point { get; set; }
    public int exchange_item_01 { get; set; }
    public int quest_unlock_item_01 { get; set; }
    public int story_unlock_item_01 { get; set; }
    public int advent_item_01 { get; set; }

    public CombatEventUserList(
        int event_id,
        int event_point,
        int exchange_item_01,
        int quest_unlock_item_01,
        int story_unlock_item_01,
        int advent_item_01
    )
    {
        this.event_id = event_id;
        this.event_point = event_point;
        this.exchange_item_01 = exchange_item_01;
        this.quest_unlock_item_01 = quest_unlock_item_01;
        this.story_unlock_item_01 = story_unlock_item_01;
        this.advent_item_01 = advent_item_01;
    }

    public CombatEventUserList() { }
}

[MessagePackObject(true)]
public class ConvertedEntityList
{
    public int before_entity_type { get; set; }
    public int before_entity_id { get; set; }
    public int before_entity_quantity { get; set; }
    public int after_entity_type { get; set; }
    public int after_entity_id { get; set; }
    public int after_entity_quantity { get; set; }

    public ConvertedEntityList(
        int before_entity_type,
        int before_entity_id,
        int before_entity_quantity,
        int after_entity_type,
        int after_entity_id,
        int after_entity_quantity
    )
    {
        this.before_entity_type = before_entity_type;
        this.before_entity_id = before_entity_id;
        this.before_entity_quantity = before_entity_quantity;
        this.after_entity_type = after_entity_type;
        this.after_entity_id = after_entity_id;
        this.after_entity_quantity = after_entity_quantity;
    }

    public ConvertedEntityList() { }
}

[MessagePackObject(true)]
public class CraftList
{
    public int weapon_id { get; set; }
    public int is_new { get; set; }

    public CraftList(int weapon_id, int is_new)
    {
        this.weapon_id = weapon_id;
        this.is_new = is_new;
    }

    public CraftList() { }
}

[MessagePackObject(true)]
public class CurrentMainStoryMission
{
    public int main_story_mission_group_id { get; set; }
    public IEnumerable<AtgenMainStoryMissionStateList> main_story_mission_state_list { get; set; }

    public CurrentMainStoryMission(
        int main_story_mission_group_id,
        IEnumerable<AtgenMainStoryMissionStateList> main_story_mission_state_list
    )
    {
        this.main_story_mission_group_id = main_story_mission_group_id;
        this.main_story_mission_state_list = main_story_mission_state_list;
    }

    public CurrentMainStoryMission() { }
}

[MessagePackObject(true)]
public class DailyMissionList
{
    public int daily_mission_id { get; set; }
    public int progress { get; set; }
    public int state { get; set; }
    public int day_no { get; set; }
    public int weekly_mission_id { get; set; }
    public int week_no { get; set; }
    public int end_date { get; set; }
    public int start_date { get; set; }
    public int is_lock_receive_reward { get; set; }
    public int is_pickup { get; set; }

    public DailyMissionList(
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
    )
    {
        this.daily_mission_id = daily_mission_id;
        this.progress = progress;
        this.state = state;
        this.day_no = day_no;
        this.weekly_mission_id = weekly_mission_id;
        this.week_no = week_no;
        this.end_date = end_date;
        this.start_date = start_date;
        this.is_lock_receive_reward = is_lock_receive_reward;
        this.is_pickup = is_pickup;
    }

    public DailyMissionList() { }
}

[MessagePackObject(true)]
public class DataHeader
{
    public int result_code { get; set; }

    public DataHeader(int result_code)
    {
        this.result_code = result_code;
    }

    public DataHeader() { }
}

[MessagePackObject(true)]
public class DeleteDataList
{
    public IEnumerable<AtgenDeleteDragonList> delete_dragon_list { get; set; }
    public IEnumerable<AtgenDeleteTalismanList> delete_talisman_list { get; set; }
    public IEnumerable<AtgenDeleteWeaponList> delete_weapon_list { get; set; }
    public IEnumerable<AtgenDeleteAmuletList> delete_amulet_list { get; set; }

    public DeleteDataList(
        IEnumerable<AtgenDeleteDragonList> delete_dragon_list,
        IEnumerable<AtgenDeleteTalismanList> delete_talisman_list,
        IEnumerable<AtgenDeleteWeaponList> delete_weapon_list,
        IEnumerable<AtgenDeleteAmuletList> delete_amulet_list
    )
    {
        this.delete_dragon_list = delete_dragon_list;
        this.delete_talisman_list = delete_talisman_list;
        this.delete_weapon_list = delete_weapon_list;
        this.delete_amulet_list = delete_amulet_list;
    }

    public DeleteDataList() { }
}

[MessagePackObject(true)]
public class DiamondData
{
    public int paid_diamond { get; set; }
    public int free_diamond { get; set; }

    public DiamondData(int paid_diamond, int free_diamond)
    {
        this.paid_diamond = paid_diamond;
        this.free_diamond = free_diamond;
    }

    public DiamondData() { }
}

[MessagePackObject(true)]
public class DmodeCharaList
{
    public int chara_id { get; set; }
    public int max_floor_num { get; set; }
    public int select_servitor_id { get; set; }
    public int select_edit_skill_chara_id_1 { get; set; }
    public int select_edit_skill_chara_id_2 { get; set; }
    public int select_edit_skill_chara_id_3 { get; set; }
    public int max_dmode_score { get; set; }

    public DmodeCharaList(
        int chara_id,
        int max_floor_num,
        int select_servitor_id,
        int select_edit_skill_chara_id_1,
        int select_edit_skill_chara_id_2,
        int select_edit_skill_chara_id_3,
        int max_dmode_score
    )
    {
        this.chara_id = chara_id;
        this.max_floor_num = max_floor_num;
        this.select_servitor_id = select_servitor_id;
        this.select_edit_skill_chara_id_1 = select_edit_skill_chara_id_1;
        this.select_edit_skill_chara_id_2 = select_edit_skill_chara_id_2;
        this.select_edit_skill_chara_id_3 = select_edit_skill_chara_id_3;
        this.max_dmode_score = max_dmode_score;
    }

    public DmodeCharaList() { }
}

[MessagePackObject(true)]
public class DmodeDungeonInfo
{
    public int chara_id { get; set; }
    public int floor_num { get; set; }
    public int quest_time { get; set; }
    public int dungeon_score { get; set; }
    public int is_play_end { get; set; }
    public int state { get; set; }

    public DmodeDungeonInfo(
        int chara_id,
        int floor_num,
        int quest_time,
        int dungeon_score,
        int is_play_end,
        int state
    )
    {
        this.chara_id = chara_id;
        this.floor_num = floor_num;
        this.quest_time = quest_time;
        this.dungeon_score = dungeon_score;
        this.is_play_end = is_play_end;
        this.state = state;
    }

    public DmodeDungeonInfo() { }
}

[MessagePackObject(true)]
public class DmodeDungeonItemList
{
    public int item_no { get; set; }
    public int item_id { get; set; }
    public int item_state { get; set; }
    public AtgenOption option { get; set; }

    public DmodeDungeonItemList(int item_no, int item_id, int item_state, AtgenOption option)
    {
        this.item_no = item_no;
        this.item_id = item_id;
        this.item_state = item_state;
        this.option = option;
    }

    public DmodeDungeonItemList() { }
}

[MessagePackObject(true)]
public class DmodeExpedition
{
    public int chara_id_1 { get; set; }
    public int chara_id_2 { get; set; }
    public int chara_id_3 { get; set; }
    public int chara_id_4 { get; set; }
    public int start_time { get; set; }
    public int target_floor_num { get; set; }
    public int state { get; set; }

    public DmodeExpedition(
        int chara_id_1,
        int chara_id_2,
        int chara_id_3,
        int chara_id_4,
        int start_time,
        int target_floor_num,
        int state
    )
    {
        this.chara_id_1 = chara_id_1;
        this.chara_id_2 = chara_id_2;
        this.chara_id_3 = chara_id_3;
        this.chara_id_4 = chara_id_4;
        this.start_time = start_time;
        this.target_floor_num = target_floor_num;
        this.state = state;
    }

    public DmodeExpedition() { }
}

[MessagePackObject(true)]
public class DmodeFloorData
{
    public string unique_key { get; set; }
    public string floor_key { get; set; }
    public int is_end { get; set; }
    public int is_play_end { get; set; }
    public int is_view_area_start_equipment { get; set; }
    public AtgenDmodeAreaInfo dmode_area_info { get; set; }
    public AtgenDmodeUnitInfo dmode_unit_info { get; set; }
    public AtgenDmodeDungeonOdds dmode_dungeon_odds { get; set; }

    public DmodeFloorData(
        string unique_key,
        string floor_key,
        int is_end,
        int is_play_end,
        int is_view_area_start_equipment,
        AtgenDmodeAreaInfo dmode_area_info,
        AtgenDmodeUnitInfo dmode_unit_info,
        AtgenDmodeDungeonOdds dmode_dungeon_odds
    )
    {
        this.unique_key = unique_key;
        this.floor_key = floor_key;
        this.is_end = is_end;
        this.is_play_end = is_play_end;
        this.is_view_area_start_equipment = is_view_area_start_equipment;
        this.dmode_area_info = dmode_area_info;
        this.dmode_unit_info = dmode_unit_info;
        this.dmode_dungeon_odds = dmode_dungeon_odds;
    }

    public DmodeFloorData() { }
}

[MessagePackObject(true)]
public class DmodeInfo
{
    public int total_max_floor_num { get; set; }
    public int recovery_count { get; set; }
    public int recovery_time { get; set; }
    public int floor_skip_count { get; set; }
    public int floor_skip_time { get; set; }
    public int dmode_point_1 { get; set; }
    public int dmode_point_2 { get; set; }
    public int is_entry { get; set; }

    public DmodeInfo(
        int total_max_floor_num,
        int recovery_count,
        int recovery_time,
        int floor_skip_count,
        int floor_skip_time,
        int dmode_point_1,
        int dmode_point_2,
        int is_entry
    )
    {
        this.total_max_floor_num = total_max_floor_num;
        this.recovery_count = recovery_count;
        this.recovery_time = recovery_time;
        this.floor_skip_count = floor_skip_count;
        this.floor_skip_time = floor_skip_time;
        this.dmode_point_1 = dmode_point_1;
        this.dmode_point_2 = dmode_point_2;
        this.is_entry = is_entry;
    }

    public DmodeInfo() { }
}

[MessagePackObject(true)]
public class DmodeIngameData
{
    public string unique_key { get; set; }
    public int start_floor_num { get; set; }
    public int target_floor_num { get; set; }
    public int recovery_count { get; set; }
    public int recovery_time { get; set; }
    public int servitor_id { get; set; }
    public int dmode_level_group_id { get; set; }
    public AtgenUnitData unit_data { get; set; }
    public IEnumerable<DmodeServitorPassiveList> dmode_servitor_passive_list { get; set; }

    public DmodeIngameData(
        string unique_key,
        int start_floor_num,
        int target_floor_num,
        int recovery_count,
        int recovery_time,
        int servitor_id,
        int dmode_level_group_id,
        AtgenUnitData unit_data,
        IEnumerable<DmodeServitorPassiveList> dmode_servitor_passive_list
    )
    {
        this.unique_key = unique_key;
        this.start_floor_num = start_floor_num;
        this.target_floor_num = target_floor_num;
        this.recovery_count = recovery_count;
        this.recovery_time = recovery_time;
        this.servitor_id = servitor_id;
        this.dmode_level_group_id = dmode_level_group_id;
        this.unit_data = unit_data;
        this.dmode_servitor_passive_list = dmode_servitor_passive_list;
    }

    public DmodeIngameData() { }
}

[MessagePackObject(true)]
public class DmodeIngameResult
{
    public int floor_num { get; set; }
    public int is_record_floor_num { get; set; }
    public IEnumerable<int> chara_id_list { get; set; }
    public float quest_time { get; set; }
    public int is_view_quest_time { get; set; }
    public int dmode_score { get; set; }
    public IEnumerable<AtgenRewardTalismanList> reward_talisman_list { get; set; }
    public int take_dmode_point_1 { get; set; }
    public int take_dmode_point_2 { get; set; }
    public int take_player_exp { get; set; }
    public int player_level_up_fstone { get; set; }
    public int clear_state { get; set; }

    public DmodeIngameResult(
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
    )
    {
        this.floor_num = floor_num;
        this.is_record_floor_num = is_record_floor_num;
        this.chara_id_list = chara_id_list;
        this.quest_time = quest_time;
        this.is_view_quest_time = is_view_quest_time;
        this.dmode_score = dmode_score;
        this.reward_talisman_list = reward_talisman_list;
        this.take_dmode_point_1 = take_dmode_point_1;
        this.take_dmode_point_2 = take_dmode_point_2;
        this.take_player_exp = take_player_exp;
        this.player_level_up_fstone = player_level_up_fstone;
        this.clear_state = clear_state;
    }

    public DmodeIngameResult() { }
}

[MessagePackObject(true)]
public class DmodeOddsInfo
{
    public IEnumerable<AtgenDmodeDropObj> dmode_drop_obj { get; set; }
    public IEnumerable<AtgenDmodeEnemy> dmode_enemy { get; set; }

    public DmodeOddsInfo(
        IEnumerable<AtgenDmodeDropObj> dmode_drop_obj,
        IEnumerable<AtgenDmodeEnemy> dmode_enemy
    )
    {
        this.dmode_drop_obj = dmode_drop_obj;
        this.dmode_enemy = dmode_enemy;
    }

    public DmodeOddsInfo() { }
}

[MessagePackObject(true)]
public class DmodePlayRecord
{
    public string unique_key { get; set; }
    public string floor_key { get; set; }
    public int floor_num { get; set; }
    public int is_floor_incomplete { get; set; }
    public AtgenDmodeTreasureRecord dmode_treasure_record { get; set; }
    public IEnumerable<AtgenDmodeDungeonItemStateList> dmode_dungeon_item_state_list { get; set; }
    public IEnumerable<AtgenDmodeDungeonItemOptionList> dmode_dungeon_item_option_list { get; set; }
    public IEnumerable<AtgenDmodeDragonUseList> dmode_dragon_use_list { get; set; }
    public IEnumerable<int> equip_crest_item_no_sort_list { get; set; }
    public IEnumerable<int> bag_item_no_sort_list { get; set; }
    public IEnumerable<int> skill_bag_item_no_sort_list { get; set; }
    public float quest_time { get; set; }
    public int select_dragon_no { get; set; }

    public DmodePlayRecord(
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
    )
    {
        this.unique_key = unique_key;
        this.floor_key = floor_key;
        this.floor_num = floor_num;
        this.is_floor_incomplete = is_floor_incomplete;
        this.dmode_treasure_record = dmode_treasure_record;
        this.dmode_dungeon_item_state_list = dmode_dungeon_item_state_list;
        this.dmode_dungeon_item_option_list = dmode_dungeon_item_option_list;
        this.dmode_dragon_use_list = dmode_dragon_use_list;
        this.equip_crest_item_no_sort_list = equip_crest_item_no_sort_list;
        this.bag_item_no_sort_list = bag_item_no_sort_list;
        this.skill_bag_item_no_sort_list = skill_bag_item_no_sort_list;
        this.quest_time = quest_time;
        this.select_dragon_no = select_dragon_no;
    }

    public DmodePlayRecord() { }
}

[MessagePackObject(true)]
public class DmodeServitorPassiveList
{
    public int passive_no { get; set; }
    public int passive_level { get; set; }

    public DmodeServitorPassiveList(int passive_no, int passive_level)
    {
        this.passive_no = passive_no;
        this.passive_level = passive_level;
    }

    public DmodeServitorPassiveList() { }
}

[MessagePackObject(true)]
public class DmodeStoryList
{
    public int dmode_story_id { get; set; }
    public int is_read { get; set; }

    public DmodeStoryList(int dmode_story_id, int is_read)
    {
        this.dmode_story_id = dmode_story_id;
        this.is_read = is_read;
    }

    public DmodeStoryList() { }
}

[MessagePackObject(true)]
public class DragonGiftList
{
    public int dragon_gift_id { get; set; }
    public int quantity { get; set; }

    public DragonGiftList(int dragon_gift_id, int quantity)
    {
        this.dragon_gift_id = dragon_gift_id;
        this.quantity = quantity;
    }

    public DragonGiftList() { }
}

[MessagePackObject(true)]
public class DragonList
{
    public Dragons dragon_id { get; set; }
    public ulong dragon_key_id { get; set; }
    public int level { get; set; }
    public int exp { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool is_lock { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool is_new { get; set; }
    public DateTimeOffset get_time { get; set; }
    public int skill_1_level { get; set; }
    public int ability_1_level { get; set; }
    public int ability_2_level { get; set; }
    public int limit_break_count { get; set; }
    public int hp_plus_count { get; set; }
    public int attack_plus_count { get; set; }
    public int status_plus_count { get; set; }

    public DragonList(
        Dragons dragon_id,
        ulong dragon_key_id,
        int level,
        int exp,
        bool is_lock,
        bool is_new,
        DateTimeOffset get_time,
        int skill_1_level,
        int ability_1_level,
        int ability_2_level,
        int limit_break_count,
        int hp_plus_count,
        int attack_plus_count,
        int status_plus_count
    )
    {
        this.dragon_id = dragon_id;
        this.dragon_key_id = dragon_key_id;
        this.level = level;
        this.exp = exp;
        this.is_lock = is_lock;
        this.is_new = is_new;
        this.get_time = get_time;
        this.skill_1_level = skill_1_level;
        this.ability_1_level = ability_1_level;
        this.ability_2_level = ability_2_level;
        this.limit_break_count = limit_break_count;
        this.hp_plus_count = hp_plus_count;
        this.attack_plus_count = attack_plus_count;
        this.status_plus_count = status_plus_count;
    }

    public DragonList() { }
}

[MessagePackObject(true)]
public class DragonReliabilityList
{
    public Dragons dragon_id { get; set; }
    public int reliability_level { get; set; }
    public int reliability_total_exp { get; set; }

    public DateTimeOffset gettime { get; set; }

    public DateTimeOffset last_contact_time { get; set; }

    public DragonReliabilityList(
        Dragons dragon_id,
        int reliability_level,
        int reliability_total_exp,
        DateTimeOffset gettime,
        DateTimeOffset last_contact_time
    )
    {
        this.dragon_id = dragon_id;
        this.reliability_level = reliability_level;
        this.reliability_total_exp = reliability_total_exp;
        this.gettime = gettime;
        this.last_contact_time = last_contact_time;
    }

    public DragonReliabilityList() { }
}

[MessagePackObject(true)]
public class DragonRewardEntityList
{
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }
    public int entity_quantity { get; set; }
    public int is_over { get; set; }

    public DragonRewardEntityList(
        EntityTypes entity_type,
        int entity_id,
        int entity_quantity,
        int is_over
    )
    {
        this.entity_type = entity_type;
        this.entity_id = entity_id;
        this.entity_quantity = entity_quantity;
        this.is_over = is_over;
    }

    public DragonRewardEntityList() { }
}

[MessagePackObject(true)]
public class DrillMissionGroupList
{
    public int drill_mission_group_id { get; set; }

    public DrillMissionGroupList(int drill_mission_group_id)
    {
        this.drill_mission_group_id = drill_mission_group_id;
    }

    public DrillMissionGroupList() { }
}

[MessagePackObject(true)]
public class DrillMissionList
{
    public int drill_mission_id { get; set; }
    public int progress { get; set; }
    public int state { get; set; }
    public int end_date { get; set; }
    public int start_date { get; set; }

    public DrillMissionList(
        int drill_mission_id,
        int progress,
        int state,
        int end_date,
        int start_date
    )
    {
        this.drill_mission_id = drill_mission_id;
        this.progress = progress;
        this.state = state;
        this.end_date = end_date;
        this.start_date = start_date;
    }

    public DrillMissionList() { }
}

[MessagePackObject(true)]
public class EarnEventUserList
{
    public int event_id { get; set; }
    public int event_point { get; set; }
    public int exchange_item_01 { get; set; }
    public int exchange_item_02 { get; set; }
    public int advent_item_quantity_01 { get; set; }

    public EarnEventUserList(
        int event_id,
        int event_point,
        int exchange_item_01,
        int exchange_item_02,
        int advent_item_quantity_01
    )
    {
        this.event_id = event_id;
        this.event_point = event_point;
        this.exchange_item_01 = exchange_item_01;
        this.exchange_item_02 = exchange_item_02;
        this.advent_item_quantity_01 = advent_item_quantity_01;
    }

    public EarnEventUserList() { }
}

[MessagePackObject(true)]
public class EditSkillCharaData
{
    public int chara_id { get; set; }
    public int edit_skill_level { get; set; }

    public EditSkillCharaData(int chara_id, int edit_skill_level)
    {
        this.chara_id = chara_id;
        this.edit_skill_level = edit_skill_level;
    }

    public EditSkillCharaData() { }
}

[MessagePackObject(true)]
public class EmblemList
{
    public int emblem_id { get; set; }
    public int is_new { get; set; }
    public int gettime { get; set; }

    public EmblemList(int emblem_id, int is_new, int gettime)
    {
        this.emblem_id = emblem_id;
        this.is_new = is_new;
        this.gettime = gettime;
    }

    public EmblemList() { }
}

[MessagePackObject(true)]
public class EnemyBookList
{
    public int enemy_book_id { get; set; }
    public int piece_count { get; set; }
    public int kill_count { get; set; }

    public EnemyBookList(int enemy_book_id, int piece_count, int kill_count)
    {
        this.enemy_book_id = enemy_book_id;
        this.piece_count = piece_count;
        this.kill_count = kill_count;
    }

    public EnemyBookList() { }
}

[MessagePackObject(true)]
public class EnemyDamageHistory
{
    public IEnumerable<int> damage { get; set; }
    public IEnumerable<int> combo { get; set; }

    public EnemyDamageHistory(IEnumerable<int> damage, IEnumerable<int> combo)
    {
        this.damage = damage;
        this.combo = combo;
    }

    public EnemyDamageHistory() { }
}

[MessagePackObject(true)]
public class EnemyDropList
{
    public int coin { get; set; }
    public int mana { get; set; }
    public IEnumerable<AtgenDropList> drop_list { get; set; }

    public EnemyDropList(int coin, int mana, IEnumerable<AtgenDropList> drop_list)
    {
        this.coin = coin;
        this.mana = mana;
        this.drop_list = drop_list;
    }

    public EnemyDropList() { }
}

[MessagePackObject(true)]
public class EntityResult
{
    public IEnumerable<AtgenBuildEventRewardEntityList> over_discard_entity_list { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> over_present_entity_list { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> over_present_limit_entity_list { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> new_get_entity_list { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; } =
        new List<ConvertedEntityList>();

    public EntityResult(
        IEnumerable<AtgenBuildEventRewardEntityList> over_discard_entity_list,
        IEnumerable<AtgenBuildEventRewardEntityList> over_present_entity_list,
        IEnumerable<AtgenBuildEventRewardEntityList> over_present_limit_entity_list,
        IEnumerable<AtgenDuplicateEntityList> new_get_entity_list,
        IEnumerable<ConvertedEntityList> converted_entity_list
    )
    {
        this.over_discard_entity_list = over_discard_entity_list;
        this.over_present_entity_list = over_present_entity_list;
        this.over_present_limit_entity_list = over_present_limit_entity_list;
        this.new_get_entity_list = new_get_entity_list;
        this.converted_entity_list = converted_entity_list;
    }

    public EntityResult() { }
}

[MessagePackObject(true)]
public class EquipStampList
{
    public int slot { get; set; }
    public int stamp_id { get; set; }

    public EquipStampList(int slot, int stamp_id)
    {
        this.slot = slot;
        this.stamp_id = stamp_id;
    }

    public EquipStampList() { }
}

[MessagePackObject(true)]
public class EventAbilityCharaList
{
    public int chara_id { get; set; }
    public int ability_id_1 { get; set; }
    public int ability_id_2 { get; set; }

    public EventAbilityCharaList(int chara_id, int ability_id_1, int ability_id_2)
    {
        this.chara_id = chara_id;
        this.ability_id_1 = ability_id_1;
        this.ability_id_2 = ability_id_2;
    }

    public EventAbilityCharaList() { }
}

[MessagePackObject(true)]
public class EventCycleRewardList
{
    public int event_cycle_id { get; set; }
    public int event_cycle_reward_id { get; set; }

    public EventCycleRewardList(int event_cycle_id, int event_cycle_reward_id)
    {
        this.event_cycle_id = event_cycle_id;
        this.event_cycle_reward_id = event_cycle_reward_id;
    }

    public EventCycleRewardList() { }
}

[MessagePackObject(true)]
public class EventDamageRanking
{
    public int event_id { get; set; }
    public IEnumerable<AtgenOwnDamageRankingList> own_damage_ranking_list { get; set; }

    public EventDamageRanking(
        int event_id,
        IEnumerable<AtgenOwnDamageRankingList> own_damage_ranking_list
    )
    {
        this.event_id = event_id;
        this.own_damage_ranking_list = own_damage_ranking_list;
    }

    public EventDamageRanking() { }
}

[MessagePackObject(true)]
public class EventPassiveList
{
    public int event_id { get; set; }
    public IEnumerable<AtgenEventPassiveUpList> event_passive_grow_list { get; set; }

    public EventPassiveList(
        int event_id,
        IEnumerable<AtgenEventPassiveUpList> event_passive_grow_list
    )
    {
        this.event_id = event_id;
        this.event_passive_grow_list = event_passive_grow_list;
    }

    public EventPassiveList() { }
}

[MessagePackObject(true)]
public class EventStoryList
{
    public int event_story_id { get; set; }
    public int state { get; set; }

    public EventStoryList(int event_story_id, int state)
    {
        this.event_story_id = event_story_id;
        this.state = state;
    }

    public EventStoryList() { }
}

[MessagePackObject(true)]
public class EventTradeList
{
    public int event_trade_id { get; set; }
    public int trade_group_id { get; set; }
    public int tab_group_id { get; set; }
    public int priority { get; set; }
    public int is_lock_view { get; set; }
    public int commence_date { get; set; }
    public int complete_date { get; set; }
    public int reset_type { get; set; }
    public int limit { get; set; }
    public int read_story_count { get; set; }
    public int clear_target_quest_id { get; set; }
    public int destination_entity_type { get; set; }
    public int destination_entity_id { get; set; }
    public int destination_entity_quantity { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> need_entity_list { get; set; }

    public EventTradeList(
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
    )
    {
        this.event_trade_id = event_trade_id;
        this.trade_group_id = trade_group_id;
        this.tab_group_id = tab_group_id;
        this.priority = priority;
        this.is_lock_view = is_lock_view;
        this.commence_date = commence_date;
        this.complete_date = complete_date;
        this.reset_type = reset_type;
        this.limit = limit;
        this.read_story_count = read_story_count;
        this.clear_target_quest_id = clear_target_quest_id;
        this.destination_entity_type = destination_entity_type;
        this.destination_entity_id = destination_entity_id;
        this.destination_entity_quantity = destination_entity_quantity;
        this.need_entity_list = need_entity_list;
    }

    public EventTradeList() { }
}

[MessagePackObject(true)]
public class ExchangeTicketList
{
    public int exchange_ticket_id { get; set; }
    public int quantity { get; set; }

    public ExchangeTicketList(int exchange_ticket_id, int quantity)
    {
        this.exchange_ticket_id = exchange_ticket_id;
        this.quantity = quantity;
    }

    public ExchangeTicketList() { }
}

[MessagePackObject(true)]
public class ExHunterEventUserList
{
    public int event_id { get; set; }
    public int box_summon_point { get; set; }
    public int ex_hunter_point_1 { get; set; }
    public int ex_hunter_point_2 { get; set; }
    public int ex_hunter_point_3 { get; set; }
    public int advent_item_quantity_1 { get; set; }
    public int advent_item_quantity_2 { get; set; }
    public int ultimate_key_count { get; set; }
    public int exchange_item_1 { get; set; }
    public int exchange_item_2 { get; set; }

    public ExHunterEventUserList(
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
    )
    {
        this.event_id = event_id;
        this.box_summon_point = box_summon_point;
        this.ex_hunter_point_1 = ex_hunter_point_1;
        this.ex_hunter_point_2 = ex_hunter_point_2;
        this.ex_hunter_point_3 = ex_hunter_point_3;
        this.advent_item_quantity_1 = advent_item_quantity_1;
        this.advent_item_quantity_2 = advent_item_quantity_2;
        this.ultimate_key_count = ultimate_key_count;
        this.exchange_item_1 = exchange_item_1;
        this.exchange_item_2 = exchange_item_2;
    }

    public ExHunterEventUserList() { }
}

[MessagePackObject(true)]
public class ExRushEventUserList
{
    public int event_id { get; set; }
    public int ex_rush_item_1 { get; set; }
    public int ex_rush_item_2 { get; set; }

    public ExRushEventUserList(int event_id, int ex_rush_item_1, int ex_rush_item_2)
    {
        this.event_id = event_id;
        this.ex_rush_item_1 = ex_rush_item_1;
        this.ex_rush_item_2 = ex_rush_item_2;
    }

    public ExRushEventUserList() { }
}

[MessagePackObject(true)]
public class FortBonusList
{
    public IEnumerable<AtgenParamBonus> param_bonus { get; set; }
    public IEnumerable<AtgenParamBonus> param_bonus_by_weapon { get; set; }
    public IEnumerable<AtgenElementBonus> element_bonus { get; set; }
    public AtgenAllBonus all_bonus { get; set; }
    public IEnumerable<AtgenElementBonus> chara_bonus_by_album { get; set; }
    public IEnumerable<AtgenDragonBonus> dragon_bonus { get; set; }
    public AtgenDragonTimeBonus dragon_time_bonus { get; set; }
    public IEnumerable<AtgenElementBonus> dragon_bonus_by_album { get; set; }

    public FortBonusList(
        IEnumerable<AtgenParamBonus> param_bonus,
        IEnumerable<AtgenParamBonus> param_bonus_by_weapon,
        IEnumerable<AtgenElementBonus> element_bonus,
        AtgenAllBonus all_bonus,
        IEnumerable<AtgenElementBonus> chara_bonus_by_album,
        IEnumerable<AtgenDragonBonus> dragon_bonus,
        AtgenDragonTimeBonus dragon_time_bonus,
        IEnumerable<AtgenElementBonus> dragon_bonus_by_album
    )
    {
        this.param_bonus = param_bonus;
        this.param_bonus_by_weapon = param_bonus_by_weapon;
        this.element_bonus = element_bonus;
        this.all_bonus = all_bonus;
        this.chara_bonus_by_album = chara_bonus_by_album;
        this.dragon_bonus = dragon_bonus;
        this.dragon_time_bonus = dragon_time_bonus;
        this.dragon_bonus_by_album = dragon_bonus_by_album;
    }

    public FortBonusList() { }
}

[MessagePackObject(true)]
public class FortDetail
{
    public int max_carpenter_count { get; set; }
    public int carpenter_num { get; set; }
    public int working_carpenter_num { get; set; }

    public FortDetail(int max_carpenter_count, int carpenter_num, int working_carpenter_num)
    {
        this.max_carpenter_count = max_carpenter_count;
        this.carpenter_num = carpenter_num;
        this.working_carpenter_num = working_carpenter_num;
    }

    public FortDetail() { }
}

[MessagePackObject(true)]
public class FortPlantList
{
    public int plant_id { get; set; }
    public int is_new { get; set; }

    public FortPlantList(int plant_id, int is_new)
    {
        this.plant_id = plant_id;
        this.is_new = is_new;
    }

    public FortPlantList() { }
}

[MessagePackObject(true)]
public class FriendNotice
{
    public int friend_new_count { get; set; }
    public int apply_new_count { get; set; }

    public FriendNotice(int friend_new_count, int apply_new_count)
    {
        this.friend_new_count = friend_new_count;
        this.apply_new_count = apply_new_count;
    }

    public FriendNotice() { }
}

[MessagePackObject(true)]
public class FunctionalMaintenanceList
{
    public int functional_maintenance_type { get; set; }

    public FunctionalMaintenanceList(int functional_maintenance_type)
    {
        this.functional_maintenance_type = functional_maintenance_type;
    }

    public FunctionalMaintenanceList() { }
}

[MessagePackObject(true)]
public class GameAbilityCrest
{
    public AbilityCrests ability_crest_id { get; set; }
    public int buildup_count { get; set; }
    public int limit_break_count { get; set; }
    public int equipable_count { get; set; }
    public int ability_1_level { get; set; }
    public int ability_2_level { get; set; }
    public int hp_plus_count { get; set; }
    public int attack_plus_count { get; set; }

    public GameAbilityCrest(
        AbilityCrests ability_crest_id,
        int buildup_count,
        int limit_break_count,
        int equipable_count,
        int ability_1_level,
        int ability_2_level,
        int hp_plus_count,
        int attack_plus_count
    )
    {
        this.ability_crest_id = ability_crest_id;
        this.buildup_count = buildup_count;
        this.limit_break_count = limit_break_count;
        this.equipable_count = equipable_count;
        this.ability_1_level = ability_1_level;
        this.ability_2_level = ability_2_level;
        this.hp_plus_count = hp_plus_count;
        this.attack_plus_count = attack_plus_count;
    }

    public GameAbilityCrest() { }
}

[MessagePackObject(true)]
public class GameWeaponBody
{
    public WeaponBodies weapon_body_id { get; set; }
    public int buildup_count { get; set; }
    public int limit_break_count { get; set; }
    public int limit_over_count { get; set; }
    public int skill_no { get; set; }
    public int skill_level { get; set; }
    public int ability_1_level { get; set; }
    public int ability_2_level { get; set; }
    public int equipable_count { get; set; }
    public int additional_crest_slot_type_1_count { get; set; }
    public int additional_crest_slot_type_2_count { get; set; }
    public int additional_crest_slot_type_3_count { get; set; }
    public int additional_effect_count { get; set; }

    public GameWeaponBody(
        WeaponBodies weapon_body_id,
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
    )
    {
        this.weapon_body_id = weapon_body_id;
        this.buildup_count = buildup_count;
        this.limit_break_count = limit_break_count;
        this.limit_over_count = limit_over_count;
        this.skill_no = skill_no;
        this.skill_level = skill_level;
        this.ability_1_level = ability_1_level;
        this.ability_2_level = ability_2_level;
        this.equipable_count = equipable_count;
        this.additional_crest_slot_type_1_count = additional_crest_slot_type_1_count;
        this.additional_crest_slot_type_2_count = additional_crest_slot_type_2_count;
        this.additional_crest_slot_type_3_count = additional_crest_slot_type_3_count;
        this.additional_effect_count = additional_effect_count;
    }

    public GameWeaponBody() { }
}

[MessagePackObject(true)]
public class GameWeaponSkin
{
    public int weapon_skin_id { get; set; }

    public GameWeaponSkin(int weapon_skin_id)
    {
        this.weapon_skin_id = weapon_skin_id;
    }

    public GameWeaponSkin() { }
}

[MessagePackObject(true)]
public class GatherItemList
{
    public int gather_item_id { get; set; }
    public int quantity { get; set; }
    public int quest_take_weekly_quantity { get; set; }
    public int quest_last_weekly_reset_time { get; set; }

    public GatherItemList(
        int gather_item_id,
        int quantity,
        int quest_take_weekly_quantity,
        int quest_last_weekly_reset_time
    )
    {
        this.gather_item_id = gather_item_id;
        this.quantity = quantity;
        this.quest_take_weekly_quantity = quest_take_weekly_quantity;
        this.quest_last_weekly_reset_time = quest_last_weekly_reset_time;
    }

    public GatherItemList() { }
}

[MessagePackObject(true)]
public class GrowMaterialList
{
    public int type { get; set; }
    public ulong id { get; set; }
    public int quantity { get; set; }

    public GrowMaterialList(int type, ulong id, int quantity)
    {
        this.type = type;
        this.id = id;
        this.quantity = quantity;
    }

    public GrowMaterialList() { }
}

[MessagePackObject(true)]
public class GrowRecord
{
    public int take_player_exp { get; set; }
    public int take_chara_exp { get; set; }
    public int take_mana { get; set; }
    public float bonus_factor { get; set; }
    public float mana_bonus_factor { get; set; }
    public IEnumerable<AtgenCharaGrowRecord> chara_grow_record { get; set; }
    public IEnumerable<CharaFriendshipList> chara_friendship_list { get; set; }

    public GrowRecord(
        int take_player_exp,
        int take_chara_exp,
        int take_mana,
        float bonus_factor,
        float mana_bonus_factor,
        IEnumerable<AtgenCharaGrowRecord> chara_grow_record,
        IEnumerable<CharaFriendshipList> chara_friendship_list
    )
    {
        this.take_player_exp = take_player_exp;
        this.take_chara_exp = take_chara_exp;
        this.take_mana = take_mana;
        this.bonus_factor = bonus_factor;
        this.mana_bonus_factor = mana_bonus_factor;
        this.chara_grow_record = chara_grow_record;
        this.chara_friendship_list = chara_friendship_list;
    }

    public GrowRecord() { }
}

[MessagePackObject(true)]
public class GuildApplyList
{
    public ulong viewer_id { get; set; }
    public string user_name { get; set; }
    public int user_level { get; set; }
    public int max_party_power { get; set; }
    public int profile_entity_type { get; set; }
    public int profile_entity_id { get; set; }
    public int profile_entity_rarity { get; set; }
    public ulong guild_apply_id { get; set; }
    public int last_active_time { get; set; }

    public GuildApplyList(
        ulong viewer_id,
        string user_name,
        int user_level,
        int max_party_power,
        int profile_entity_type,
        int profile_entity_id,
        int profile_entity_rarity,
        ulong guild_apply_id,
        int last_active_time
    )
    {
        this.viewer_id = viewer_id;
        this.user_name = user_name;
        this.user_level = user_level;
        this.max_party_power = max_party_power;
        this.profile_entity_type = profile_entity_type;
        this.profile_entity_id = profile_entity_id;
        this.profile_entity_rarity = profile_entity_rarity;
        this.guild_apply_id = guild_apply_id;
        this.last_active_time = last_active_time;
    }

    public GuildApplyList() { }
}

[MessagePackObject(true)]
public class GuildChatMessageList
{
    public ulong chat_message_id { get; set; }
    public ulong viewer_id { get; set; }
    public string user_name { get; set; }
    public int profile_entity_type { get; set; }
    public int profile_entity_id { get; set; }
    public int profile_entity_rarity { get; set; }
    public int chat_message_type { get; set; }
    public string chat_message_text { get; set; }
    public int chat_message_stamp_id { get; set; }
    public int chat_message_system_message_id { get; set; }
    public int chat_message_param_value_1 { get; set; }
    public int chat_message_param_value_2 { get; set; }
    public int chat_message_param_value_3 { get; set; }
    public int chat_message_param_value_4 { get; set; }
    public int create_time { get; set; }

    public GuildChatMessageList(
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
    )
    {
        this.chat_message_id = chat_message_id;
        this.viewer_id = viewer_id;
        this.user_name = user_name;
        this.profile_entity_type = profile_entity_type;
        this.profile_entity_id = profile_entity_id;
        this.profile_entity_rarity = profile_entity_rarity;
        this.chat_message_type = chat_message_type;
        this.chat_message_text = chat_message_text;
        this.chat_message_stamp_id = chat_message_stamp_id;
        this.chat_message_system_message_id = chat_message_system_message_id;
        this.chat_message_param_value_1 = chat_message_param_value_1;
        this.chat_message_param_value_2 = chat_message_param_value_2;
        this.chat_message_param_value_3 = chat_message_param_value_3;
        this.chat_message_param_value_4 = chat_message_param_value_4;
        this.create_time = create_time;
    }

    public GuildChatMessageList() { }
}

[MessagePackObject(true)]
public class GuildData
{
    public int guild_id { get; set; }
    public string guild_name { get; set; }
    public int guild_emblem_id { get; set; }
    public string guild_introduction { get; set; }
    public int joining_condition_type { get; set; }
    public int activity_policy_type { get; set; }
    public string guild_board { get; set; }
    public int guild_member_count { get; set; }
    public int is_penalty_guild_name { get; set; }
    public int is_penalty_guild_introduction { get; set; }
    public int is_penalty_guild_board { get; set; }

    public GuildData(
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
    )
    {
        this.guild_id = guild_id;
        this.guild_name = guild_name;
        this.guild_emblem_id = guild_emblem_id;
        this.guild_introduction = guild_introduction;
        this.joining_condition_type = joining_condition_type;
        this.activity_policy_type = activity_policy_type;
        this.guild_board = guild_board;
        this.guild_member_count = guild_member_count;
        this.is_penalty_guild_name = is_penalty_guild_name;
        this.is_penalty_guild_introduction = is_penalty_guild_introduction;
        this.is_penalty_guild_board = is_penalty_guild_board;
    }

    public GuildData() { }
}

[MessagePackObject(true)]
public class GuildInviteReceiveList
{
    public ulong guild_invite_id { get; set; }
    public ulong send_viewer_id { get; set; }
    public string send_user_name { get; set; }
    public int send_max_party_power { get; set; }
    public int send_profile_entity_type { get; set; }
    public int send_profile_entity_id { get; set; }
    public int send_profile_entity_rarity { get; set; }
    public int send_last_active_time { get; set; }
    public int guild_invite_message_id { get; set; }
    public GuildData guild_data { get; set; }

    public GuildInviteReceiveList(
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
    )
    {
        this.guild_invite_id = guild_invite_id;
        this.send_viewer_id = send_viewer_id;
        this.send_user_name = send_user_name;
        this.send_max_party_power = send_max_party_power;
        this.send_profile_entity_type = send_profile_entity_type;
        this.send_profile_entity_id = send_profile_entity_id;
        this.send_profile_entity_rarity = send_profile_entity_rarity;
        this.send_last_active_time = send_last_active_time;
        this.guild_invite_message_id = guild_invite_message_id;
        this.guild_data = guild_data;
    }

    public GuildInviteReceiveList() { }
}

[MessagePackObject(true)]
public class GuildInviteSendList
{
    public ulong guild_invite_id { get; set; }
    public ulong send_viewer_id { get; set; }
    public string send_user_name { get; set; }
    public ulong receive_viewer_id { get; set; }
    public string receive_user_name { get; set; }
    public int receive_user_level { get; set; }
    public int receive_max_party_power { get; set; }
    public int receive_profile_entity_type { get; set; }
    public int receive_profile_entity_id { get; set; }
    public int receive_profile_entity_rarity { get; set; }
    public int receive_last_active_time { get; set; }
    public int guild_invite_message_id { get; set; }
    public int limit_time { get; set; }

    public GuildInviteSendList(
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
    )
    {
        this.guild_invite_id = guild_invite_id;
        this.send_viewer_id = send_viewer_id;
        this.send_user_name = send_user_name;
        this.receive_viewer_id = receive_viewer_id;
        this.receive_user_name = receive_user_name;
        this.receive_user_level = receive_user_level;
        this.receive_max_party_power = receive_max_party_power;
        this.receive_profile_entity_type = receive_profile_entity_type;
        this.receive_profile_entity_id = receive_profile_entity_id;
        this.receive_profile_entity_rarity = receive_profile_entity_rarity;
        this.receive_last_active_time = receive_last_active_time;
        this.guild_invite_message_id = guild_invite_message_id;
        this.limit_time = limit_time;
    }

    public GuildInviteSendList() { }
}

[MessagePackObject(true)]
public class GuildMemberList
{
    public ulong viewer_id { get; set; }
    public string user_name { get; set; }
    public int user_level { get; set; }
    public int max_party_power { get; set; }
    public int profile_entity_type { get; set; }
    public int profile_entity_id { get; set; }
    public int profile_entity_rarity { get; set; }
    public int last_active_time { get; set; }
    public int last_guild_active_time { get; set; }
    public int last_attend_time { get; set; }
    public int guild_position_type { get; set; }
    public int temporary_end_time { get; set; }

    public GuildMemberList(
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
    )
    {
        this.viewer_id = viewer_id;
        this.user_name = user_name;
        this.user_level = user_level;
        this.max_party_power = max_party_power;
        this.profile_entity_type = profile_entity_type;
        this.profile_entity_id = profile_entity_id;
        this.profile_entity_rarity = profile_entity_rarity;
        this.last_active_time = last_active_time;
        this.last_guild_active_time = last_guild_active_time;
        this.last_attend_time = last_attend_time;
        this.guild_position_type = guild_position_type;
        this.temporary_end_time = temporary_end_time;
    }

    public GuildMemberList() { }
}

[MessagePackObject(true)]
public class GuildNotice
{
    public int guild_apply_count { get; set; }
    public int is_update_guild_board { get; set; }
    public int is_update_guild_apply_reply { get; set; }
    public int is_update_guild { get; set; }
    public int is_update_guild_invite { get; set; }

    public GuildNotice(
        int guild_apply_count,
        int is_update_guild_board,
        int is_update_guild_apply_reply,
        int is_update_guild,
        int is_update_guild_invite
    )
    {
        this.guild_apply_count = guild_apply_count;
        this.is_update_guild_board = is_update_guild_board;
        this.is_update_guild_apply_reply = is_update_guild_apply_reply;
        this.is_update_guild = is_update_guild;
        this.is_update_guild_invite = is_update_guild_invite;
    }

    public GuildNotice() { }
}

[MessagePackObject(true)]
public class IngameData
{
    public ulong viewer_id { get; set; }
    public string dungeon_key { get; set; }
    public DungeonTypes dungeon_type { get; set; }
    public QuestPlayModeTypes play_type { get; set; }
    public int quest_id { get; set; }
    public int bonus_type { get; set; }
    public int continue_limit { get; set; }
    public int continue_count { get; set; }
    public int reborn_limit { get; set; }
    public DateTimeOffset start_time { get; set; }
    public PartyInfo party_info { get; set; }
    public IEnumerable<AreaInfoList> area_info_list { get; set; }
    public int use_stone { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_host { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_fever_time { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_bot_tutorial { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_receivable_carry_bonus { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_use_event_chara_ability { get; set; }
    public IEnumerable<EventAbilityCharaList> event_ability_chara_list { get; set; }
    public IEnumerable<ulong> first_clear_viewer_id_list { get; set; }
    public int multi_disconnect_type { get; set; }
    public int repeat_state { get; set; }
    public AtgenIngameWalker ingame_walker { get; set; }

    public IngameData(
        ulong viewer_id,
        string dungeon_key,
        DungeonTypes dungeon_type,
        QuestPlayModeTypes play_type,
        int quest_id,
        int bonus_type,
        int continue_limit,
        int continue_count,
        int reborn_limit,
        DateTimeOffset start_time,
        PartyInfo party_info,
        IEnumerable<AreaInfoList> area_info_list,
        int use_stone,
        bool is_host,
        bool is_fever_time,
        bool is_bot_tutorial,
        bool is_receivable_carry_bonus,
        bool is_use_event_chara_ability,
        IEnumerable<EventAbilityCharaList> event_ability_chara_list,
        IEnumerable<ulong> first_clear_viewer_id_list,
        int multi_disconnect_type,
        int repeat_state,
        AtgenIngameWalker ingame_walker
    )
    {
        this.viewer_id = viewer_id;
        this.dungeon_key = dungeon_key;
        this.dungeon_type = dungeon_type;
        this.play_type = play_type;
        this.quest_id = quest_id;
        this.bonus_type = bonus_type;
        this.continue_limit = continue_limit;
        this.continue_count = continue_count;
        this.reborn_limit = reborn_limit;
        this.start_time = start_time;
        this.party_info = party_info;
        this.area_info_list = area_info_list;
        this.use_stone = use_stone;
        this.is_host = is_host;
        this.is_fever_time = is_fever_time;
        this.is_bot_tutorial = is_bot_tutorial;
        this.is_receivable_carry_bonus = is_receivable_carry_bonus;
        this.is_use_event_chara_ability = is_use_event_chara_ability;
        this.event_ability_chara_list = event_ability_chara_list;
        this.first_clear_viewer_id_list = first_clear_viewer_id_list;
        this.multi_disconnect_type = multi_disconnect_type;
        this.repeat_state = repeat_state;
        this.ingame_walker = ingame_walker;
    }

    public IngameData() { }
}

[MessagePackObject(true)]
public class IngameQuestData
{
    public int quest_id { get; set; }
    public int play_count { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_mission_clear_1 { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_mission_clear_2 { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_mission_clear_3 { get; set; }

    public IngameQuestData(
        int quest_id,
        int play_count,
        bool is_mission_clear_1,
        bool is_mission_clear_2,
        bool is_mission_clear_3
    )
    {
        this.quest_id = quest_id;
        this.play_count = play_count;
        this.is_mission_clear_1 = is_mission_clear_1;
        this.is_mission_clear_2 = is_mission_clear_2;
        this.is_mission_clear_3 = is_mission_clear_3;
    }

    public IngameQuestData() { }
}

[MessagePackObject(true)]
public class IngameResultData
{
    public string dungeon_key { get; set; }
    public int play_type { get; set; }
    public int quest_id { get; set; }
    public RewardRecord reward_record { get; set; }
    public GrowRecord grow_record { get; set; }
    public DateTimeOffset start_time { get; set; }
    public DateTimeOffset end_time { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_clear { get; set; }
    public int state { get; set; }
    public int dungeon_skip_type { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_host { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_fever_time { get; set; }
    public int wave_count { get; set; }
    public int current_play_count { get; set; }
    public int reborn_count { get; set; }
    public IEnumerable<PartySettingList> quest_party_setting_list { get; set; }
    public IEnumerable<UserSupportList> helper_list { get; set; }
    public IEnumerable<AtgenScoringEnemyPointList> scoring_enemy_point_list { get; set; }
    public IEnumerable<AtgenHelperDetailList> helper_detail_list { get; set; }
    public IEnumerable<AtgenScoreMissionSuccessList> score_mission_success_list { get; set; }
    public IEnumerable<AtgenBonusFactorList> bonus_factor_list { get; set; }
    public IEnumerable<AtgenEventPassiveUpList> event_passive_up_list { get; set; }
    public float clear_time { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_best_clear_time { get; set; }
    public long total_play_damage { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; }

    public IngameResultData(
        string dungeon_key,
        int play_type,
        int quest_id,
        RewardRecord reward_record,
        GrowRecord grow_record,
        DateTimeOffset start_time,
        DateTimeOffset end_time,
        bool is_clear,
        int state,
        int dungeon_skip_type,
        bool is_host,
        bool is_fever_time,
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
        bool is_best_clear_time,
        long total_play_damage,
        IEnumerable<ConvertedEntityList> converted_entity_list
    )
    {
        this.dungeon_key = dungeon_key;
        this.play_type = play_type;
        this.quest_id = quest_id;
        this.reward_record = reward_record;
        this.grow_record = grow_record;
        this.start_time = start_time;
        this.end_time = end_time;
        this.is_clear = is_clear;
        this.state = state;
        this.dungeon_skip_type = dungeon_skip_type;
        this.is_host = is_host;
        this.is_fever_time = is_fever_time;
        this.wave_count = wave_count;
        this.current_play_count = current_play_count;
        this.reborn_count = reborn_count;
        this.quest_party_setting_list = quest_party_setting_list;
        this.helper_list = helper_list;
        this.scoring_enemy_point_list = scoring_enemy_point_list;
        this.helper_detail_list = helper_detail_list;
        this.score_mission_success_list = score_mission_success_list;
        this.bonus_factor_list = bonus_factor_list;
        this.event_passive_up_list = event_passive_up_list;
        this.clear_time = clear_time;
        this.is_best_clear_time = is_best_clear_time;
        this.total_play_damage = total_play_damage;
        this.converted_entity_list = converted_entity_list;
    }

    public IngameResultData() { }
}

[MessagePackObject(true)]
public class IngameWallData
{
    public int wall_id { get; set; }
    public int wall_level { get; set; }

    public IngameWallData(int wall_id, int wall_level)
    {
        this.wall_id = wall_id;
        this.wall_level = wall_level;
    }

    public IngameWallData() { }
}

[MessagePackObject(true)]
public class ItemList
{
    public int item_id { get; set; }
    public int quantity { get; set; }

    public ItemList(int item_id, int quantity)
    {
        this.item_id = item_id;
        this.quantity = quantity;
    }

    public ItemList() { }
}

[MessagePackObject(true)]
public class LimitBreakGrowList
{
    public int limit_break_count { get; set; }
    public int limit_break_item_type { get; set; }
    public ulong target_id { get; set; }

    public LimitBreakGrowList(int limit_break_count, int limit_break_item_type, ulong target_id)
    {
        this.limit_break_count = limit_break_count;
        this.limit_break_item_type = limit_break_item_type;
        this.target_id = target_id;
    }

    public LimitBreakGrowList() { }
}

[MessagePackObject(true)]
public class LotteryOddsRate
{
    public IEnumerable<AtgenLotteryPrizeRankList> lottery_prize_rank_list { get; set; }
    public IEnumerable<AtgenLotteryEntitySetList> lottery_entity_set_list { get; set; }

    public LotteryOddsRate(
        IEnumerable<AtgenLotteryPrizeRankList> lottery_prize_rank_list,
        IEnumerable<AtgenLotteryEntitySetList> lottery_entity_set_list
    )
    {
        this.lottery_prize_rank_list = lottery_prize_rank_list;
        this.lottery_entity_set_list = lottery_entity_set_list;
    }

    public LotteryOddsRate() { }
}

[MessagePackObject(true)]
public class LotteryOddsRateList
{
    public LotteryOddsRate normal { get; set; }
    public LotteryOddsRate guarantee { get; set; }

    public LotteryOddsRateList(LotteryOddsRate normal, LotteryOddsRate guarantee)
    {
        this.normal = normal;
        this.guarantee = guarantee;
    }

    public LotteryOddsRateList() { }
}

[MessagePackObject(true)]
public class LotteryTicketList
{
    public int lottery_ticket_id { get; set; }
    public int quantity { get; set; }

    public LotteryTicketList(int lottery_ticket_id, int quantity)
    {
        this.lottery_ticket_id = lottery_ticket_id;
        this.quantity = quantity;
    }

    public LotteryTicketList() { }
}

[MessagePackObject(true)]
public class MainStoryMissionList
{
    public int main_story_mission_id { get; set; }
    public int progress { get; set; }
    public int state { get; set; }
    public int end_date { get; set; }
    public int start_date { get; set; }

    public MainStoryMissionList(
        int main_story_mission_id,
        int progress,
        int state,
        int end_date,
        int start_date
    )
    {
        this.main_story_mission_id = main_story_mission_id;
        this.progress = progress;
        this.state = state;
        this.end_date = end_date;
        this.start_date = start_date;
    }

    public MainStoryMissionList() { }
}

[MessagePackObject(true)]
public class MaterialList
{
    public Materials material_id { get; set; }
    public int quantity { get; set; }

    public MaterialList(Materials material_id, int quantity)
    {
        this.material_id = material_id;
        this.quantity = quantity;
    }

    public MaterialList() { }
}

[MessagePackObject(true)]
public class MazeEventUserList
{
    public int event_id { get; set; }
    public IEnumerable<AtgenUserMazeEventItemList> user_maze_event_item_list { get; set; }

    public MazeEventUserList(
        int event_id,
        IEnumerable<AtgenUserMazeEventItemList> user_maze_event_item_list
    )
    {
        this.event_id = event_id;
        this.user_maze_event_item_list = user_maze_event_item_list;
    }

    public MazeEventUserList() { }
}

[MessagePackObject(true)]
public class MemoryEventMissionList
{
    public int memory_event_mission_id { get; set; }
    public int progress { get; set; }
    public int state { get; set; }
    public int end_date { get; set; }
    public int start_date { get; set; }

    public MemoryEventMissionList(
        int memory_event_mission_id,
        int progress,
        int state,
        int end_date,
        int start_date
    )
    {
        this.memory_event_mission_id = memory_event_mission_id;
        this.progress = progress;
        this.state = state;
        this.end_date = end_date;
        this.start_date = start_date;
    }

    public MemoryEventMissionList() { }
}

[MessagePackObject(true)]
public class MissionNotice
{
    public AtgenNormalMissionNotice normal_mission_notice { get; set; }
    public AtgenNormalMissionNotice daily_mission_notice { get; set; }
    public AtgenNormalMissionNotice period_mission_notice { get; set; }
    public AtgenNormalMissionNotice beginner_mission_notice { get; set; }
    public AtgenNormalMissionNotice special_mission_notice { get; set; }
    public AtgenNormalMissionNotice main_story_mission_notice { get; set; }
    public AtgenNormalMissionNotice memory_event_mission_notice { get; set; }
    public AtgenNormalMissionNotice drill_mission_notice { get; set; }
    public AtgenNormalMissionNotice album_mission_notice { get; set; }

    public MissionNotice(
        AtgenNormalMissionNotice normal_mission_notice,
        AtgenNormalMissionNotice daily_mission_notice,
        AtgenNormalMissionNotice period_mission_notice,
        AtgenNormalMissionNotice beginner_mission_notice,
        AtgenNormalMissionNotice special_mission_notice,
        AtgenNormalMissionNotice main_story_mission_notice,
        AtgenNormalMissionNotice memory_event_mission_notice,
        AtgenNormalMissionNotice drill_mission_notice,
        AtgenNormalMissionNotice album_mission_notice
    )
    {
        this.normal_mission_notice = normal_mission_notice;
        this.daily_mission_notice = daily_mission_notice;
        this.period_mission_notice = period_mission_notice;
        this.beginner_mission_notice = beginner_mission_notice;
        this.special_mission_notice = special_mission_notice;
        this.main_story_mission_notice = main_story_mission_notice;
        this.memory_event_mission_notice = memory_event_mission_notice;
        this.drill_mission_notice = drill_mission_notice;
        this.album_mission_notice = album_mission_notice;
    }

    public MissionNotice() { }
}

[MessagePackObject(true)]
public class MuseumDragonList
{
    public int state { get; set; }
    public int dragon_id { get; set; }

    public MuseumDragonList(int state, int dragon_id)
    {
        this.state = state;
        this.dragon_id = dragon_id;
    }

    public MuseumDragonList() { }
}

[MessagePackObject(true)]
public class MuseumList
{
    public int state { get; set; }
    public int chara_id { get; set; }

    public MuseumList(int state, int chara_id)
    {
        this.state = state;
        this.chara_id = chara_id;
    }

    public MuseumList() { }
}

[MessagePackObject(true)]
public class NormalMissionList
{
    public int normal_mission_id { get; set; }
    public int progress { get; set; }
    public int state { get; set; }
    public int end_date { get; set; }
    public int start_date { get; set; }

    public NormalMissionList(
        int normal_mission_id,
        int progress,
        int state,
        int end_date,
        int start_date
    )
    {
        this.normal_mission_id = normal_mission_id;
        this.progress = progress;
        this.state = state;
        this.end_date = end_date;
        this.start_date = start_date;
    }

    public NormalMissionList() { }
}

[MessagePackObject(true)]
public class OddsInfo
{
    public int area_index { get; set; }
    public int reaction_obj_count { get; set; }
    public IEnumerable<AtgenDropObj> drop_obj { get; set; }
    public IEnumerable<AtgenEnemy> enemy { get; set; }
    public IEnumerable<AtgenGrade> grade { get; set; }

    public OddsInfo(
        int area_index,
        int reaction_obj_count,
        IEnumerable<AtgenDropObj> drop_obj,
        IEnumerable<AtgenEnemy> enemy,
        IEnumerable<AtgenGrade> grade
    )
    {
        this.area_index = area_index;
        this.reaction_obj_count = reaction_obj_count;
        this.drop_obj = drop_obj;
        this.enemy = enemy;
        this.grade = grade;
    }

    public OddsInfo() { }
}

[MessagePackObject(true)]
public class OddsRate
{
    public IEnumerable<AtgenRarityList> rarity_list { get; set; }
    public IEnumerable<AtgenRarityGroupList> rarity_group_list { get; set; }
    public AtgenUnit unit { get; set; }

    public OddsRate(
        IEnumerable<AtgenRarityList> rarity_list,
        IEnumerable<AtgenRarityGroupList> rarity_group_list,
        AtgenUnit unit
    )
    {
        this.rarity_list = rarity_list;
        this.rarity_group_list = rarity_group_list;
        this.unit = unit;
    }

    public OddsRate() { }
}

[MessagePackObject(true)]
public class OddsRateList
{
    public int required_count_to_next { get; set; }
    public OddsRate normal { get; set; }
    public OddsRate guarantee { get; set; }

    public OddsRateList(int required_count_to_next, OddsRate normal, OddsRate guarantee)
    {
        this.required_count_to_next = required_count_to_next;
        this.normal = normal;
        this.guarantee = guarantee;
    }

    public OddsRateList() { }
}

[MessagePackObject(true)]
public class OddsUnitDetail
{
    public bool pickup { get; set; }
    public int rarity { get; set; }
    public IEnumerable<AtgenUnitList> unit_list { get; set; }

    public OddsUnitDetail(bool pickup, int rarity, IEnumerable<AtgenUnitList> unit_list)
    {
        this.pickup = pickup;
        this.rarity = rarity;
        this.unit_list = unit_list;
    }

    public OddsUnitDetail() { }
}

[MessagePackObject(true)]
public class OptionData
{
    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_enable_auto_lock_unit { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_auto_lock_dragon_sr { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_auto_lock_dragon_ssr { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_auto_lock_weapon_sr { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_auto_lock_weapon_ssr { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_auto_lock_weapon_sssr { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_auto_lock_amulet_sr { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_auto_lock_amulet_ssr { get; set; }

    public OptionData(
        bool is_enable_auto_lock_unit,
        bool is_auto_lock_dragon_sr,
        bool is_auto_lock_dragon_ssr,
        bool is_auto_lock_weapon_sr,
        bool is_auto_lock_weapon_ssr,
        bool is_auto_lock_weapon_sssr,
        bool is_auto_lock_amulet_sr,
        bool is_auto_lock_amulet_ssr
    )
    {
        this.is_enable_auto_lock_unit = is_enable_auto_lock_unit;
        this.is_auto_lock_dragon_sr = is_auto_lock_dragon_sr;
        this.is_auto_lock_dragon_ssr = is_auto_lock_dragon_ssr;
        this.is_auto_lock_weapon_sr = is_auto_lock_weapon_sr;
        this.is_auto_lock_weapon_ssr = is_auto_lock_weapon_ssr;
        this.is_auto_lock_weapon_sssr = is_auto_lock_weapon_sssr;
        this.is_auto_lock_amulet_sr = is_auto_lock_amulet_sr;
        this.is_auto_lock_amulet_ssr = is_auto_lock_amulet_ssr;
    }

    public OptionData() { }
}

[MessagePackObject(true)]
public class PartyInfo
{
    public IEnumerable<PartyUnitList> party_unit_list { get; set; }
    public FortBonusList fort_bonus_list { get; set; }
    public AtgenEventBoost event_boost { get; set; }
    public AtgenSupportData support_data { get; set; }
    public IEnumerable<AtgenEventPassiveUpList> event_passive_grow_list { get; set; }

    public PartyInfo(
        IEnumerable<PartyUnitList> party_unit_list,
        FortBonusList fort_bonus_list,
        AtgenEventBoost event_boost,
        AtgenSupportData support_data,
        IEnumerable<AtgenEventPassiveUpList> event_passive_grow_list
    )
    {
        this.party_unit_list = party_unit_list;
        this.fort_bonus_list = fort_bonus_list;
        this.event_boost = event_boost;
        this.support_data = support_data;
        this.event_passive_grow_list = event_passive_grow_list;
    }

    public PartyInfo() { }
}

[MessagePackObject(true)]
public class PartyList
{
    public int party_no { get; set; }
    public string party_name { get; set; }
    public IEnumerable<PartySettingList> party_setting_list { get; set; }

    public PartyList(
        int party_no,
        string party_name,
        IEnumerable<PartySettingList> party_setting_list
    )
    {
        this.party_no = party_no;
        this.party_name = party_name;
        this.party_setting_list = party_setting_list;
    }

    public PartyList() { }
}

[MessagePackObject(true)]
public class PartyPowerData
{
    public int max_party_power { get; set; }

    public PartyPowerData(int max_party_power)
    {
        this.max_party_power = max_party_power;
    }

    public PartyPowerData() { }
}

[MessagePackObject(true)]
public class PartySettingList
{
    public int unit_no { get; set; }
    public Charas chara_id { get; set; }
    public ulong equip_weapon_key_id { get; set; }
    public ulong equip_dragon_key_id { get; set; }
    public ulong equip_amulet_key_id { get; set; }
    public ulong equip_amulet_2_key_id { get; set; }
    public int equip_skin_weapon_id { get; set; }
    public WeaponBodies equip_weapon_body_id { get; set; }
    public int equip_weapon_skin_id { get; set; }
    public AbilityCrests equip_crest_slot_type_1_crest_id_1 { get; set; }
    public AbilityCrests equip_crest_slot_type_1_crest_id_2 { get; set; }
    public AbilityCrests equip_crest_slot_type_1_crest_id_3 { get; set; }
    public AbilityCrests equip_crest_slot_type_2_crest_id_1 { get; set; }
    public AbilityCrests equip_crest_slot_type_2_crest_id_2 { get; set; }
    public AbilityCrests equip_crest_slot_type_3_crest_id_1 { get; set; }
    public AbilityCrests equip_crest_slot_type_3_crest_id_2 { get; set; }
    public ulong equip_talisman_key_id { get; set; }
    public int edit_skill_1_chara_id { get; set; }
    public int edit_skill_2_chara_id { get; set; }

    public PartySettingList(
        int unit_no,
        Charas chara_id,
        ulong equip_weapon_key_id,
        ulong equip_dragon_key_id,
        ulong equip_amulet_key_id,
        ulong equip_amulet_2_key_id,
        int equip_skin_weapon_id,
        WeaponBodies equip_weapon_body_id,
        int equip_weapon_skin_id,
        AbilityCrests equip_crest_slot_type_1_crest_id_1,
        AbilityCrests equip_crest_slot_type_1_crest_id_2,
        AbilityCrests equip_crest_slot_type_1_crest_id_3,
        AbilityCrests equip_crest_slot_type_2_crest_id_1,
        AbilityCrests equip_crest_slot_type_2_crest_id_2,
        AbilityCrests equip_crest_slot_type_3_crest_id_1,
        AbilityCrests equip_crest_slot_type_3_crest_id_2,
        ulong equip_talisman_key_id,
        int edit_skill_1_chara_id,
        int edit_skill_2_chara_id
    )
    {
        this.unit_no = unit_no;
        this.chara_id = chara_id;
        this.equip_weapon_key_id = equip_weapon_key_id;
        this.equip_dragon_key_id = equip_dragon_key_id;
        this.equip_amulet_key_id = equip_amulet_key_id;
        this.equip_amulet_2_key_id = equip_amulet_2_key_id;
        this.equip_skin_weapon_id = equip_skin_weapon_id;
        this.equip_weapon_body_id = equip_weapon_body_id;
        this.equip_weapon_skin_id = equip_weapon_skin_id;
        this.equip_crest_slot_type_1_crest_id_1 = equip_crest_slot_type_1_crest_id_1;
        this.equip_crest_slot_type_1_crest_id_2 = equip_crest_slot_type_1_crest_id_2;
        this.equip_crest_slot_type_1_crest_id_3 = equip_crest_slot_type_1_crest_id_3;
        this.equip_crest_slot_type_2_crest_id_1 = equip_crest_slot_type_2_crest_id_1;
        this.equip_crest_slot_type_2_crest_id_2 = equip_crest_slot_type_2_crest_id_2;
        this.equip_crest_slot_type_3_crest_id_1 = equip_crest_slot_type_3_crest_id_1;
        this.equip_crest_slot_type_3_crest_id_2 = equip_crest_slot_type_3_crest_id_2;
        this.equip_talisman_key_id = equip_talisman_key_id;
        this.edit_skill_1_chara_id = edit_skill_1_chara_id;
        this.edit_skill_2_chara_id = edit_skill_2_chara_id;
    }

    public PartySettingList() { }
}

#nullable enable

[MessagePackObject(true)]
public class PartyUnitList
{
    public int position { get; set; }
    public CharaList? chara_data { get; set; }
    public DragonList? dragon_data { get; set; }
    public GameWeaponSkin? weapon_skin_data { get; set; }
    public GameWeaponBody? weapon_body_data { get; set; }
    public IEnumerable<GameAbilityCrest> crest_slot_type_1_crest_list { get; set; } =
        new List<GameAbilityCrest>();
    public IEnumerable<GameAbilityCrest> crest_slot_type_2_crest_list { get; set; } =
        new List<GameAbilityCrest>();
    public IEnumerable<GameAbilityCrest> crest_slot_type_3_crest_list { get; set; } =
        new List<GameAbilityCrest>();
    public TalismanList? talisman_data { get; set; }
    public EditSkillCharaData? edit_skill_1_chara_data { get; set; }
    public EditSkillCharaData? edit_skill_2_chara_data { get; set; }
    public IEnumerable<WeaponPassiveAbilityList> game_weapon_passive_ability_list { get; set; } =
        new List<WeaponPassiveAbilityList>();
    public int dragon_reliability_level { get; set; } = 0;

    public PartyUnitList(
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
    )
    {
        this.position = position;
        this.chara_data = chara_data;
        this.dragon_data = dragon_data;
        this.weapon_skin_data = weapon_skin_data;
        this.weapon_body_data = weapon_body_data;
        this.crest_slot_type_1_crest_list = crest_slot_type_1_crest_list;
        this.crest_slot_type_2_crest_list = crest_slot_type_2_crest_list;
        this.crest_slot_type_3_crest_list = crest_slot_type_3_crest_list;
        this.talisman_data = talisman_data;
        this.edit_skill_1_chara_data = edit_skill_1_chara_data;
        this.edit_skill_2_chara_data = edit_skill_2_chara_data;
        this.game_weapon_passive_ability_list = game_weapon_passive_ability_list;
        this.dragon_reliability_level = dragon_reliability_level;
    }

    public PartyUnitList() { }
}

#nullable disable

[MessagePackObject(true)]
public class PaymentTarget
{
    public int target_hold_quantity { get; set; }
    public int target_cost { get; set; }

    public PaymentTarget(int target_hold_quantity, int target_cost)
    {
        this.target_hold_quantity = target_hold_quantity;
        this.target_cost = target_cost;
    }

    public PaymentTarget() { }
}

[MessagePackObject(true)]
public class PeriodMissionList
{
    public int period_mission_id { get; set; }
    public int progress { get; set; }
    public int state { get; set; }
    public int end_date { get; set; }
    public int start_date { get; set; }

    public PeriodMissionList(
        int period_mission_id,
        int progress,
        int state,
        int end_date,
        int start_date
    )
    {
        this.period_mission_id = period_mission_id;
        this.progress = progress;
        this.state = state;
        this.end_date = end_date;
        this.start_date = start_date;
    }

    public PeriodMissionList() { }
}

[MessagePackObject(true)]
public class PlayRecord
{
    public IEnumerable<AtgenTreasureRecord> treasure_record { get; set; }
    public float time { get; set; }
    public int down_count { get; set; }
    public int trap_count { get; set; }
    public int bad_status { get; set; }
    public int dragon_pillar_count { get; set; }
    public int dragon_transform_count { get; set; }
    public int damage_count { get; set; }
    public int skill_count { get; set; }
    public int guard_broken_count { get; set; }
    public int break_count { get; set; }
    public int give_damage { get; set; }
    public int max_combo_count { get; set; }
    public int is_clear { get; set; }
    public int clear_state { get; set; }
    public int wave { get; set; }
    public int reaction_touch_cnt { get; set; }
    public int grade_point { get; set; }
    public int reborn_count { get; set; }
    public int visit_private_house { get; set; }
    public int protection_damage { get; set; }
    public int remaining_time { get; set; }
    public int lower_drawbridge_count { get; set; }
    public IEnumerable<int> live_unit_no_list { get; set; }
    public long total_play_damage { get; set; }
    public IEnumerable<AtgenDamageRecord> damage_record { get; set; }
    public IEnumerable<AtgenDamageRecord> dragon_damage_record { get; set; }
    public AtgenBattleRoyalRecord battle_royal_record { get; set; }
    public int max_damage { get; set; }
    public int max_critical_damage { get; set; }
    public int dps { get; set; }
    public int play_continue_count { get; set; }

    public PlayRecord(
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
    )
    {
        this.treasure_record = treasure_record;
        this.time = time;
        this.down_count = down_count;
        this.trap_count = trap_count;
        this.bad_status = bad_status;
        this.dragon_pillar_count = dragon_pillar_count;
        this.dragon_transform_count = dragon_transform_count;
        this.damage_count = damage_count;
        this.skill_count = skill_count;
        this.guard_broken_count = guard_broken_count;
        this.break_count = break_count;
        this.give_damage = give_damage;
        this.max_combo_count = max_combo_count;
        this.is_clear = is_clear;
        this.clear_state = clear_state;
        this.wave = wave;
        this.reaction_touch_cnt = reaction_touch_cnt;
        this.grade_point = grade_point;
        this.reborn_count = reborn_count;
        this.visit_private_house = visit_private_house;
        this.protection_damage = protection_damage;
        this.remaining_time = remaining_time;
        this.lower_drawbridge_count = lower_drawbridge_count;
        this.live_unit_no_list = live_unit_no_list;
        this.total_play_damage = total_play_damage;
        this.damage_record = damage_record;
        this.dragon_damage_record = dragon_damage_record;
        this.battle_royal_record = battle_royal_record;
        this.max_damage = max_damage;
        this.max_critical_damage = max_critical_damage;
        this.dps = dps;
        this.play_continue_count = play_continue_count;
    }

    public PlayRecord() { }
}

[MessagePackObject(true)]
public class PresentDetailList
{
    public ulong present_id { get; set; }
    public int master_id { get; set; }
    public int state { get; set; }
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }
    public int entity_quantity { get; set; }
    public int entity_level { get; set; }
    public int entity_limit_break_count { get; set; }
    public int entity_status_plus_count { get; set; }
    public int message_id { get; set; }
    public int message_param_value_1 { get; set; }
    public int message_param_value_2 { get; set; }
    public int message_param_value_3 { get; set; }
    public int message_param_value_4 { get; set; }
    public DateTimeOffset receive_limit_time { get; set; }
    public DateTimeOffset create_time { get; set; }

    public PresentDetailList(
        ulong present_id,
        int master_id,
        int state,
        EntityTypes entity_type,
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
        DateTimeOffset receive_limit_time,
        DateTimeOffset create_time
    )
    {
        this.present_id = present_id;
        this.master_id = master_id;
        this.state = state;
        this.entity_type = entity_type;
        this.entity_id = entity_id;
        this.entity_quantity = entity_quantity;
        this.entity_level = entity_level;
        this.entity_limit_break_count = entity_limit_break_count;
        this.entity_status_plus_count = entity_status_plus_count;
        this.message_id = message_id;
        this.message_param_value_1 = message_param_value_1;
        this.message_param_value_2 = message_param_value_2;
        this.message_param_value_3 = message_param_value_3;
        this.message_param_value_4 = message_param_value_4;
        this.receive_limit_time = receive_limit_time;
        this.create_time = create_time;
    }

    public PresentDetailList() { }
}

[MessagePackObject(true)]
public class PresentHistoryList
{
    public ulong id { get; set; }
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }
    public int entity_quantity { get; set; }
    public int entity_level { get; set; }
    public int entity_limit_break_count { get; set; }
    public int entity_status_plus_count { get; set; }
    public int message_id { get; set; }
    public int message_param_value_1 { get; set; }
    public int message_param_value_2 { get; set; }
    public int message_param_value_3 { get; set; }
    public int message_param_value_4 { get; set; }

    public DateTimeOffset create_time { get; set; }

    public PresentHistoryList(
        ulong id,
        EntityTypes entity_type,
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
        DateTimeOffset create_time
    )
    {
        this.id = id;
        this.entity_type = entity_type;
        this.entity_id = entity_id;
        this.entity_quantity = entity_quantity;
        this.entity_level = entity_level;
        this.entity_limit_break_count = entity_limit_break_count;
        this.entity_status_plus_count = entity_status_plus_count;
        this.message_id = message_id;
        this.message_param_value_1 = message_param_value_1;
        this.message_param_value_2 = message_param_value_2;
        this.message_param_value_3 = message_param_value_3;
        this.message_param_value_4 = message_param_value_4;
        this.create_time = create_time;
    }

    public PresentHistoryList() { }
}

[MessagePackObject(true)]
public class PresentNotice
{
    public int present_limit_count { get; set; }
    public int present_count { get; set; }

    public PresentNotice(int present_limit_count, int present_count)
    {
        this.present_limit_count = present_limit_count;
        this.present_count = present_count;
    }

    public PresentNotice() { }
}

[MessagePackObject(true)]
public class ProductList
{
    public int id { get; set; }
    public string sku { get; set; }
    public int paid_diamond_quantity { get; set; }
    public int free_diamond_quantity { get; set; }
    public int price { get; set; }

    public ProductList(
        int id,
        string sku,
        int paid_diamond_quantity,
        int free_diamond_quantity,
        int price
    )
    {
        this.id = id;
        this.sku = sku;
        this.paid_diamond_quantity = paid_diamond_quantity;
        this.free_diamond_quantity = free_diamond_quantity;
        this.price = price;
    }

    public ProductList() { }
}

[MessagePackObject(true)]
public class QuestCarryList
{
    public int quest_carry_id { get; set; }
    public int receive_count { get; set; }
    public int last_receive_time { get; set; }

    public QuestCarryList(int quest_carry_id, int receive_count, int last_receive_time)
    {
        this.quest_carry_id = quest_carry_id;
        this.receive_count = receive_count;
        this.last_receive_time = last_receive_time;
    }

    public QuestCarryList() { }
}

[MessagePackObject(true)]
public class QuestEntryConditionList
{
    public int quest_entry_condition_id { get; set; }

    public QuestEntryConditionList(int quest_entry_condition_id)
    {
        this.quest_entry_condition_id = quest_entry_condition_id;
    }

    public QuestEntryConditionList() { }
}

[MessagePackObject(true)]
public class QuestEventList
{
    public int quest_event_id { get; set; }
    public int daily_play_count { get; set; }
    public int weekly_play_count { get; set; }
    public int quest_bonus_receive_count { get; set; }
    public int quest_bonus_stack_count { get; set; }
    public int quest_bonus_stack_time { get; set; }
    public int quest_bonus_reserve_count { get; set; }
    public int quest_bonus_reserve_time { get; set; }
    public int last_daily_reset_time { get; set; }
    public int last_weekly_reset_time { get; set; }

    public QuestEventList(
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
    )
    {
        this.quest_event_id = quest_event_id;
        this.daily_play_count = daily_play_count;
        this.weekly_play_count = weekly_play_count;
        this.quest_bonus_receive_count = quest_bonus_receive_count;
        this.quest_bonus_stack_count = quest_bonus_stack_count;
        this.quest_bonus_stack_time = quest_bonus_stack_time;
        this.quest_bonus_reserve_count = quest_bonus_reserve_count;
        this.quest_bonus_reserve_time = quest_bonus_reserve_time;
        this.last_daily_reset_time = last_daily_reset_time;
        this.last_weekly_reset_time = last_weekly_reset_time;
    }

    public QuestEventList() { }
}

[MessagePackObject(true)]
public class QuestEventScheduleList
{
    public int quest_group_id { get; set; }
    public int event_schedule_type { get; set; }
    public int start_date { get; set; }
    public int end_date { get; set; }
    public int interval_type { get; set; }
    public string fever_time_start_1 { get; set; }
    public string fever_time_end_1 { get; set; }
    public string fever_time_start_2 { get; set; }
    public string fever_time_end_2 { get; set; }
    public string fever_time_start_3 { get; set; }
    public string fever_time_end_3 { get; set; }

    public QuestEventScheduleList(
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
    )
    {
        this.quest_group_id = quest_group_id;
        this.event_schedule_type = event_schedule_type;
        this.start_date = start_date;
        this.end_date = end_date;
        this.interval_type = interval_type;
        this.fever_time_start_1 = fever_time_start_1;
        this.fever_time_end_1 = fever_time_end_1;
        this.fever_time_start_2 = fever_time_start_2;
        this.fever_time_end_2 = fever_time_end_2;
        this.fever_time_start_3 = fever_time_start_3;
        this.fever_time_end_3 = fever_time_end_3;
    }

    public QuestEventScheduleList() { }
}

[MessagePackObject(true)]
public class QuestList
{
    public int quest_id { get; set; }
    public int state { get; set; }
    public int is_mission_clear_1 { get; set; }
    public int is_mission_clear_2 { get; set; }
    public int is_mission_clear_3 { get; set; }
    public int daily_play_count { get; set; }
    public int weekly_play_count { get; set; }
    public int play_count { get; set; }
    public int last_daily_reset_time { get; set; }
    public int last_weekly_reset_time { get; set; }
    public int is_appear { get; set; }
    public float best_clear_time { get; set; }

    public QuestList(
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
    )
    {
        this.quest_id = quest_id;
        this.state = state;
        this.is_mission_clear_1 = is_mission_clear_1;
        this.is_mission_clear_2 = is_mission_clear_2;
        this.is_mission_clear_3 = is_mission_clear_3;
        this.daily_play_count = daily_play_count;
        this.weekly_play_count = weekly_play_count;
        this.play_count = play_count;
        this.last_daily_reset_time = last_daily_reset_time;
        this.last_weekly_reset_time = last_weekly_reset_time;
        this.is_appear = is_appear;
        this.best_clear_time = best_clear_time;
    }

    public QuestList() { }
}

[MessagePackObject(true)]
public class QuestScheduleDetailList
{
    public int schedule_detail_id { get; set; }
    public int schedule_group_id { get; set; }
    public int drop_bonus_percent { get; set; }
    public int limit_shop_goods_type { get; set; }
    public int interval_type { get; set; }
    public int start_date { get; set; }
    public int end_date { get; set; }

    public QuestScheduleDetailList(
        int schedule_detail_id,
        int schedule_group_id,
        int drop_bonus_percent,
        int limit_shop_goods_type,
        int interval_type,
        int start_date,
        int end_date
    )
    {
        this.schedule_detail_id = schedule_detail_id;
        this.schedule_group_id = schedule_group_id;
        this.drop_bonus_percent = drop_bonus_percent;
        this.limit_shop_goods_type = limit_shop_goods_type;
        this.interval_type = interval_type;
        this.start_date = start_date;
        this.end_date = end_date;
    }

    public QuestScheduleDetailList() { }
}

[MessagePackObject(true)]
public class QuestStoryList
{
    public int quest_story_id { get; set; }
    public int state { get; set; }

    public QuestStoryList(int quest_story_id, int state)
    {
        this.quest_story_id = quest_story_id;
        this.state = state;
    }

    public QuestStoryList() { }
}

[MessagePackObject(true)]
public class QuestTreasureList
{
    public int quest_treasure_id { get; set; }

    public QuestTreasureList(int quest_treasure_id)
    {
        this.quest_treasure_id = quest_treasure_id;
    }

    public QuestTreasureList() { }
}

[MessagePackObject(true)]
public class QuestWallList
{
    public int wall_id { get; set; }
    public int wall_level { get; set; }
    public int is_start_next_level { get; set; }

    public QuestWallList(int wall_id, int wall_level, int is_start_next_level)
    {
        this.wall_id = wall_id;
        this.wall_level = wall_level;
        this.is_start_next_level = is_start_next_level;
    }

    public QuestWallList() { }
}

[MessagePackObject(true)]
public class RaidEventRewardList
{
    public int raid_event_id { get; set; }
    public int raid_event_reward_id { get; set; }

    public RaidEventRewardList(int raid_event_id, int raid_event_reward_id)
    {
        this.raid_event_id = raid_event_id;
        this.raid_event_reward_id = raid_event_reward_id;
    }

    public RaidEventRewardList() { }
}

[MessagePackObject(true)]
public class RaidEventUserList
{
    public int raid_event_id { get; set; }
    public int box_summon_point { get; set; }
    public int raid_point_1 { get; set; }
    public int raid_point_2 { get; set; }
    public int raid_point_3 { get; set; }
    public int advent_item_quantity_1 { get; set; }
    public int advent_item_quantity_2 { get; set; }
    public int ultimate_key_count { get; set; }
    public int exchange_item_count { get; set; }
    public int exchange_item_count_2 { get; set; }

    public RaidEventUserList(
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
    )
    {
        this.raid_event_id = raid_event_id;
        this.box_summon_point = box_summon_point;
        this.raid_point_1 = raid_point_1;
        this.raid_point_2 = raid_point_2;
        this.raid_point_3 = raid_point_3;
        this.advent_item_quantity_1 = advent_item_quantity_1;
        this.advent_item_quantity_2 = advent_item_quantity_2;
        this.ultimate_key_count = ultimate_key_count;
        this.exchange_item_count = exchange_item_count;
        this.exchange_item_count_2 = exchange_item_count_2;
    }

    public RaidEventUserList() { }
}

[MessagePackObject(true)]
public class RankingTierRewardList
{
    public int ranking_group_id { get; set; }
    public int tier_reward_id { get; set; }

    public RankingTierRewardList(int ranking_group_id, int tier_reward_id)
    {
        this.ranking_group_id = ranking_group_id;
        this.tier_reward_id = tier_reward_id;
    }

    public RankingTierRewardList() { }
}

[MessagePackObject(true)]
public class RedoableSummonOddsRateList
{
    public OddsRate normal { get; set; }
    public OddsRate guarantee { get; set; }

    public RedoableSummonOddsRateList(OddsRate normal, OddsRate guarantee)
    {
        this.normal = normal;
        this.guarantee = guarantee;
    }

    public RedoableSummonOddsRateList() { }
}

[MessagePackObject(true)]
public class RepeatData
{
    public string repeat_key { get; set; }
    public int repeat_count { get; set; }
    public int repeat_state { get; set; }

    public RepeatData(string repeat_key, int repeat_count, int repeat_state)
    {
        this.repeat_key = repeat_key;
        this.repeat_count = repeat_count;
        this.repeat_state = repeat_state;
    }

    public RepeatData() { }
}

[MessagePackObject(true)]
public class RepeatSetting
{
    public int repeat_type { get; set; }
    public int repeat_count { get; set; }
    public IEnumerable<int> use_item_list { get; set; }

    public RepeatSetting(int repeat_type, int repeat_count, IEnumerable<int> use_item_list)
    {
        this.repeat_type = repeat_type;
        this.repeat_count = repeat_count;
        this.use_item_list = use_item_list;
    }

    public RepeatSetting() { }
}

[MessagePackObject(true)]
public class ResponseCommon
{
    public DataHeader data_headers { get; set; }

    public ResponseCommon(DataHeader data_headers)
    {
        this.data_headers = data_headers;
    }

    public ResponseCommon() { }
}

[MessagePackObject(true)]
public class RewardRecord
{
    public IEnumerable<AtgenDropAll> drop_all { get; set; }
    public IEnumerable<AtgenFirstClearSet> first_clear_set { get; set; }
    public IEnumerable<AtgenFirstClearSet> mission_complete { get; set; }
    public IEnumerable<AtgenMissionsClearSet> missions_clear_set { get; set; }
    public IEnumerable<AtgenFirstClearSet> quest_bonus_list { get; set; }
    public IEnumerable<AtgenFirstClearSet> challenge_quest_bonus_list { get; set; }
    public IEnumerable<AtgenFirstClearSet> campaign_extra_reward_list { get; set; }
    public IEnumerable<AtgenEnemyPiece> enemy_piece { get; set; }
    public AtgenFirstMeeting first_meeting { get; set; }
    public IEnumerable<AtgenFirstClearSet> carry_bonus { get; set; }
    public IEnumerable<AtgenFirstClearSet> reborn_bonus { get; set; }
    public IEnumerable<AtgenFirstClearSet> weekly_limit_reward_list { get; set; }
    public int take_coin { get; set; }
    public float shop_quest_bonus_factor { get; set; }
    public int player_level_up_fstone { get; set; }
    public int take_accumulate_point { get; set; }
    public int take_boost_accumulate_point { get; set; }
    public int take_astral_item_quantity { get; set; }

    public RewardRecord(
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
    )
    {
        this.drop_all = drop_all;
        this.first_clear_set = first_clear_set;
        this.mission_complete = mission_complete;
        this.missions_clear_set = missions_clear_set;
        this.quest_bonus_list = quest_bonus_list;
        this.challenge_quest_bonus_list = challenge_quest_bonus_list;
        this.campaign_extra_reward_list = campaign_extra_reward_list;
        this.enemy_piece = enemy_piece;
        this.first_meeting = first_meeting;
        this.carry_bonus = carry_bonus;
        this.reborn_bonus = reborn_bonus;
        this.weekly_limit_reward_list = weekly_limit_reward_list;
        this.take_coin = take_coin;
        this.shop_quest_bonus_factor = shop_quest_bonus_factor;
        this.player_level_up_fstone = player_level_up_fstone;
        this.take_accumulate_point = take_accumulate_point;
        this.take_boost_accumulate_point = take_boost_accumulate_point;
        this.take_astral_item_quantity = take_astral_item_quantity;
    }

    public RewardRecord() { }
}

[MessagePackObject(true)]
public class RewardReliabilityList
{
    public IEnumerable<DragonRewardEntityList> levelup_entity_list { get; set; }
    public int level { get; set; }
    public int is_release_story { get; set; }

    public RewardReliabilityList(
        IEnumerable<DragonRewardEntityList> levelup_entity_list,
        int level,
        int is_release_story
    )
    {
        this.levelup_entity_list = levelup_entity_list;
        this.level = level;
        this.is_release_story = is_release_story;
    }

    public RewardReliabilityList() { }
}

[MessagePackObject(true)]
public class RoomList
{
    public int room_id { get; set; }
    public string room_name { get; set; }
    public string region { get; set; }
    public string cluster_name { get; set; }
    public string language { get; set; }
    public int status { get; set; }
    public int entry_type { get; set; }
    public int entry_guild_id { get; set; }
    public ulong host_viewer_id { get; set; }
    public string host_name { get; set; }
    public int host_level { get; set; }
    public int leader_chara_id { get; set; }
    public int leader_chara_level { get; set; }
    public int leader_chara_rarity { get; set; }
    public int quest_id { get; set; }
    public int quest_type { get; set; }
    public IEnumerable<AtgenRoomMemberList> room_member_list { get; set; }
    public int start_entry_time { get; set; }
    public int member_num { get; set; }
    public AtgenEntryConditions entry_conditions { get; set; }
    public int compatible_id { get; set; }

    public RoomList(
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
    )
    {
        this.room_id = room_id;
        this.room_name = room_name;
        this.region = region;
        this.cluster_name = cluster_name;
        this.language = language;
        this.status = status;
        this.entry_type = entry_type;
        this.entry_guild_id = entry_guild_id;
        this.host_viewer_id = host_viewer_id;
        this.host_name = host_name;
        this.host_level = host_level;
        this.leader_chara_id = leader_chara_id;
        this.leader_chara_level = leader_chara_level;
        this.leader_chara_rarity = leader_chara_rarity;
        this.quest_id = quest_id;
        this.quest_type = quest_type;
        this.room_member_list = room_member_list;
        this.start_entry_time = start_entry_time;
        this.member_num = member_num;
        this.entry_conditions = entry_conditions;
        this.compatible_id = compatible_id;
    }

    public RoomList() { }
}

[MessagePackObject(true)]
public class SearchClearPartyCharaList
{
    public int quest_id { get; set; }
    public IEnumerable<AtgenArchivePartyCharaList> archive_party_chara_list { get; set; }

    public SearchClearPartyCharaList(
        int quest_id,
        IEnumerable<AtgenArchivePartyCharaList> archive_party_chara_list
    )
    {
        this.quest_id = quest_id;
        this.archive_party_chara_list = archive_party_chara_list;
    }

    public SearchClearPartyCharaList() { }
}

[MessagePackObject(true)]
public class SearchClearPartyList
{
    public IEnumerable<AtgenArchivePartyUnitList> archive_party_unit_list { get; set; }

    public SearchClearPartyList(IEnumerable<AtgenArchivePartyUnitList> archive_party_unit_list)
    {
        this.archive_party_unit_list = archive_party_unit_list;
    }

    public SearchClearPartyList() { }
}

[MessagePackObject(true)]
public class SettingSupport
{
    public Charas chara_id { get; set; }
    public ulong equip_dragon_key_id { get; set; }
    public ulong equip_weapon_key_id { get; set; }
    public ulong equip_amulet_key_id { get; set; }
    public ulong equip_amulet_2_key_id { get; set; }
    public int equip_weapon_body_id { get; set; }
    public int equip_crest_slot_type_1_crest_id_1 { get; set; }
    public int equip_crest_slot_type_1_crest_id_2 { get; set; }
    public int equip_crest_slot_type_1_crest_id_3 { get; set; }
    public int equip_crest_slot_type_2_crest_id_1 { get; set; }
    public int equip_crest_slot_type_2_crest_id_2 { get; set; }
    public int equip_crest_slot_type_3_crest_id_1 { get; set; }
    public int equip_crest_slot_type_3_crest_id_2 { get; set; }
    public ulong equip_talisman_key_id { get; set; }

    public SettingSupport(
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
    )
    {
        this.chara_id = chara_id;
        this.equip_dragon_key_id = equip_dragon_key_id;
        this.equip_weapon_key_id = equip_weapon_key_id;
        this.equip_amulet_key_id = equip_amulet_key_id;
        this.equip_amulet_2_key_id = equip_amulet_2_key_id;
        this.equip_weapon_body_id = equip_weapon_body_id;
        this.equip_crest_slot_type_1_crest_id_1 = equip_crest_slot_type_1_crest_id_1;
        this.equip_crest_slot_type_1_crest_id_2 = equip_crest_slot_type_1_crest_id_2;
        this.equip_crest_slot_type_1_crest_id_3 = equip_crest_slot_type_1_crest_id_3;
        this.equip_crest_slot_type_2_crest_id_1 = equip_crest_slot_type_2_crest_id_1;
        this.equip_crest_slot_type_2_crest_id_2 = equip_crest_slot_type_2_crest_id_2;
        this.equip_crest_slot_type_3_crest_id_1 = equip_crest_slot_type_3_crest_id_1;
        this.equip_crest_slot_type_3_crest_id_2 = equip_crest_slot_type_3_crest_id_2;
        this.equip_talisman_key_id = equip_talisman_key_id;
    }

    public SettingSupport() { }
}

[MessagePackObject(true)]
public class ShopNotice
{
    public int is_shop_notification { get; set; }

    public ShopNotice(int is_shop_notification)
    {
        this.is_shop_notification = is_shop_notification;
    }

    public ShopNotice() { }
}

[MessagePackObject(true)]
public class ShopPurchaseList
{
    public int goods_id { get; set; }
    public int last_buy_time { get; set; }
    public int effect_start_time { get; set; }
    public int effect_end_time { get; set; }
    public int buy_count { get; set; }

    public ShopPurchaseList(
        int goods_id,
        int last_buy_time,
        int effect_start_time,
        int effect_end_time,
        int buy_count
    )
    {
        this.goods_id = goods_id;
        this.last_buy_time = last_buy_time;
        this.effect_start_time = effect_start_time;
        this.effect_end_time = effect_end_time;
        this.buy_count = buy_count;
    }

    public ShopPurchaseList() { }
}

[MessagePackObject(true)]
public class SimpleEventUserList
{
    public int event_id { get; set; }
    public int simple_event_item_1 { get; set; }
    public int simple_event_item_2 { get; set; }
    public int simple_event_item_3 { get; set; }

    public SimpleEventUserList(
        int event_id,
        int simple_event_item_1,
        int simple_event_item_2,
        int simple_event_item_3
    )
    {
        this.event_id = event_id;
        this.simple_event_item_1 = simple_event_item_1;
        this.simple_event_item_2 = simple_event_item_2;
        this.simple_event_item_3 = simple_event_item_3;
    }

    public SimpleEventUserList() { }
}

[MessagePackObject(true)]
public class SkinWeaponData
{
    public int weapon_id { get; set; }

    public SkinWeaponData(int weapon_id)
    {
        this.weapon_id = weapon_id;
    }

    public SkinWeaponData() { }
}

[MessagePackObject(true)]
public class SpecialMissionList
{
    public int special_mission_id { get; set; }
    public int progress { get; set; }
    public int state { get; set; }
    public int end_date { get; set; }
    public int start_date { get; set; }

    public SpecialMissionList(
        int special_mission_id,
        int progress,
        int state,
        int end_date,
        int start_date
    )
    {
        this.special_mission_id = special_mission_id;
        this.progress = progress;
        this.state = state;
        this.end_date = end_date;
        this.start_date = start_date;
    }

    public SpecialMissionList() { }
}

[MessagePackObject(true)]
public class StampList
{
    public int stamp_id { get; set; }
    public int is_new { get; set; }

    public StampList(int stamp_id, int is_new)
    {
        this.stamp_id = stamp_id;
        this.is_new = is_new;
    }

    public StampList() { }
}

[MessagePackObject(true)]
public class SummonHistoryList
{
    public int key_id { get; set; }
    public int summon_id { get; set; }
    public SummonExecTypes summon_exec_type { get; set; }
    public DateTimeOffset exec_date { get; set; }
    public int payment_type { get; set; }
    public EntityTypes entity_type { get; set; }
    public int entity_id { get; set; }
    public int entity_quantity { get; set; }
    public int entity_level { get; set; }
    public int entity_rarity { get; set; }
    public int entity_limit_break_count { get; set; }
    public int entity_hp_plus_count { get; set; }
    public int entity_attack_plus_count { get; set; }
    public int summon_prize_rank { get; set; }
    public int summon_point_id { get; set; }
    public int summon_point { get; set; }
    public int get_dew_point_quantity { get; set; }

    public SummonHistoryList(
        int key_id,
        int summon_id,
        SummonExecTypes summon_exec_type,
        DateTimeOffset exec_date,
        int payment_type,
        EntityTypes entity_type,
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
    )
    {
        this.key_id = key_id;
        this.summon_id = summon_id;
        this.summon_exec_type = summon_exec_type;
        this.exec_date = exec_date;
        this.payment_type = payment_type;
        this.entity_type = entity_type;
        this.entity_id = entity_id;
        this.entity_quantity = entity_quantity;
        this.entity_level = entity_level;
        this.entity_rarity = entity_rarity;
        this.entity_limit_break_count = entity_limit_break_count;
        this.entity_hp_plus_count = entity_hp_plus_count;
        this.entity_attack_plus_count = entity_attack_plus_count;
        this.summon_prize_rank = summon_prize_rank;
        this.summon_point_id = summon_point_id;
        this.summon_point = summon_point;
        this.get_dew_point_quantity = get_dew_point_quantity;
    }

    public SummonHistoryList() { }
}

[MessagePackObject(true)]
public class SummonList
{
    public int summon_id { get; set; }
    public int summon_type { get; set; }
    public int summon_group_id { get; set; }
    public int single_crystal { get; set; }
    public int single_diamond { get; set; }
    public int multi_crystal { get; set; }
    public int multi_diamond { get; set; }
    public int limited_crystal { get; set; }
    public int limited_diamond { get; set; }
    public int summon_point_id { get; set; }
    public int add_summon_point { get; set; }
    public int add_summon_point_stone { get; set; }
    public int exchange_summon_point { get; set; }
    public int status { get; set; }
    public int commence_date { get; set; }
    public int complete_date { get; set; }
    public int daily_count { get; set; }
    public int daily_limit { get; set; }
    public int total_count { get; set; }
    public int total_limit { get; set; }
    public int campaign_type { get; set; }
    public int free_count_rest { get; set; }
    public int is_beginner_campaign { get; set; }
    public int beginner_campaign_count_rest { get; set; }
    public int consecution_campaign_count_rest { get; set; }

    /// UNKNOWN: params: priority, summon_type, status, daily(unsure), campaign_type, [x]_rest
    /// <summary>
    /// Banner Data<br/>
    /// This is composed from static banner data and DB saved player-banner data
    /// </summary>
    /// <param name="summon_id">Banner Id</param>
    /// <param name="priority">Unknown</param>
    /// <param name="summon_type">Unknown, maybe for special banners like platinum only banners</param>
    /// <param name="single_crystal">1x summon Wyrmite cost (Negative numbers won't allow summons, 0 for default)</param>
    /// <param name="single_diamond">Client uses <see cref="single_crystal"/> for displaying both wyrmite and diamantium cost<br/>Most likely 1x summon Diamantium cost (Negative numbers won't allow summons, 0 for default)</param>
    /// <param name="multi_crystal">10x summon Wyrmite cost (Negative numbers won't allow summons, 0 for default)</param>
    /// <param name="multi_diamond">Client uses <see cref="multi_crystal"/> for displaying both wyrmite and diamantium cost<br/>Most likely 10x summon Diamantium cost (Negative numbers won't allow summons, 0 for default)</param>
    /// <param name="limited_crystal">Unknown: Presumably Wyrmite cost of the limited 1x summon button but it never existed</param>
    /// <param name="limited_diamond">Diamantium cost of the limited 1x summon button</param>
    /// <param name="add_summon_point">Summon points for a 1x Wyrmite summon</param>
    /// <param name="add_summon_point_stone">Summon points for a 1x Diamantium summon</param>
    /// <param name="exchange_summon_point">Summon point cost for sparking, the client doesn't seem to care though</param>
    /// <param name="status">Unknown function, maybe just active = 1, inactive = 0 but no change in normal banner</param>
    /// <param name="commence_date">Banner start date</param>
    /// <param name="complete_date">Banner end date</param>
    /// <param name="daily_count">Currently used summons for the daily discounted diamantium summon</param>
    /// <param name="daily_limit">Total limit for the daily discounted diamantium summon</param>
    /// <param name="total_limit">Total amount of summons limit(seems ignored for normal banners)</param>
    /// <param name="total_count">Current total amount of summons(seems ignored for normal banners)</param>
    /// <param name="campaign_type">Unknown, maybe used for </param>
    /// <param name="free_count_rest">Most likely free summons for certain banner/campaign types</param>
    /// <param name="is_beginner_campaign">If this banner is part of the beginner campaign</param>
    /// <param name="beginner_campaign_count_rest">Begginer banner has a free tenfold available(only if <see cref="is_beginner_campaign"/> is set)</param>
    /// <param name="consecution_campaign_count_rest">Unknown</param>
    public SummonList(
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
    )
    {
        this.summon_id = summon_id;
        this.summon_type = summon_type;
        this.summon_group_id = summon_group_id;
        this.single_crystal = single_crystal;
        this.single_diamond = single_diamond;
        this.multi_crystal = multi_crystal;
        this.multi_diamond = multi_diamond;
        this.limited_crystal = limited_crystal;
        this.limited_diamond = limited_diamond;
        this.summon_point_id = summon_point_id;
        this.add_summon_point = add_summon_point;
        this.add_summon_point_stone = add_summon_point_stone;
        this.exchange_summon_point = exchange_summon_point;
        this.status = status;
        this.commence_date = commence_date;
        this.complete_date = complete_date;
        this.daily_count = daily_count;
        this.daily_limit = daily_limit;
        this.total_count = total_count;
        this.total_limit = total_limit;
        this.campaign_type = campaign_type;
        this.free_count_rest = free_count_rest;
        this.is_beginner_campaign = is_beginner_campaign;
        this.beginner_campaign_count_rest = beginner_campaign_count_rest;
        this.consecution_campaign_count_rest = consecution_campaign_count_rest;
    }

    public SummonList() { }
}

[MessagePackObject(true)]
public class SummonPointList
{
    public int summon_point_id { get; set; }
    public int summon_point { get; set; }
    public int cs_summon_point { get; set; }
    public int cs_point_term_min_date { get; set; }
    public int cs_point_term_max_date { get; set; }

    public SummonPointList(
        int summon_point_id,
        int summon_point,
        int cs_summon_point,
        int cs_point_term_min_date,
        int cs_point_term_max_date
    )
    {
        this.summon_point_id = summon_point_id;
        this.summon_point = summon_point;
        this.cs_summon_point = cs_summon_point;
        this.cs_point_term_min_date = cs_point_term_min_date;
        this.cs_point_term_max_date = cs_point_term_max_date;
    }

    public SummonPointList() { }
}

[MessagePackObject(true)]
public class SummonPrizeOddsRate
{
    public IEnumerable<AtgenSummonPrizeRankList> summon_prize_rank_list { get; set; }
    public IEnumerable<AtgenSummonPrizeEntitySetList> summon_prize_entity_set_list { get; set; }

    public SummonPrizeOddsRate(
        IEnumerable<AtgenSummonPrizeRankList> summon_prize_rank_list,
        IEnumerable<AtgenSummonPrizeEntitySetList> summon_prize_entity_set_list
    )
    {
        this.summon_prize_rank_list = summon_prize_rank_list;
        this.summon_prize_entity_set_list = summon_prize_entity_set_list;
    }

    public SummonPrizeOddsRate() { }
}

[MessagePackObject(true)]
public class SummonPrizeOddsRateList
{
    public SummonPrizeOddsRate normal { get; set; }
    public SummonPrizeOddsRate guarantee { get; set; }

    public SummonPrizeOddsRateList(SummonPrizeOddsRate normal, SummonPrizeOddsRate guarantee)
    {
        this.normal = normal;
        this.guarantee = guarantee;
    }

    public SummonPrizeOddsRateList() { }
}

[MessagePackObject(true)]
public class SummonTicketList
{
    public ulong key_id { get; set; }
    public int summon_ticket_id { get; set; }
    public int quantity { get; set; }
    public int use_limit_time { get; set; }

    public SummonTicketList(ulong key_id, int summon_ticket_id, int quantity, int use_limit_time)
    {
        this.key_id = key_id;
        this.summon_ticket_id = summon_ticket_id;
        this.quantity = quantity;
        this.use_limit_time = use_limit_time;
    }

    public SummonTicketList() { }
}

[MessagePackObject(true)]
public class TalismanList
{
    public ulong talisman_key_id { get; set; }
    public int talisman_id { get; set; }
    public int talisman_ability_id_1 { get; set; }
    public int talisman_ability_id_2 { get; set; }
    public int talisman_ability_id_3 { get; set; }
    public int additional_hp { get; set; }
    public int additional_attack { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool is_new { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool is_lock { get; set; }
    public DateTimeOffset gettime { get; set; }

    public TalismanList(
        ulong talisman_key_id,
        int talisman_id,
        int talisman_ability_id_1,
        int talisman_ability_id_2,
        int talisman_ability_id_3,
        int additional_hp,
        int additional_attack,
        bool is_new,
        bool is_lock,
        DateTimeOffset gettime
    )
    {
        this.talisman_key_id = talisman_key_id;
        this.talisman_id = talisman_id;
        this.talisman_ability_id_1 = talisman_ability_id_1;
        this.talisman_ability_id_2 = talisman_ability_id_2;
        this.talisman_ability_id_3 = talisman_ability_id_3;
        this.additional_hp = additional_hp;
        this.additional_attack = additional_attack;
        this.is_new = is_new;
        this.is_lock = is_lock;
        this.gettime = gettime;
    }

    public TalismanList() { }
}

[MessagePackObject(true)]
public class TimeAttackRankingData
{
    public int ranking_id { get; set; }
    public IEnumerable<AtgenOwnRankingList> own_ranking_list { get; set; }

    public TimeAttackRankingData(int ranking_id, IEnumerable<AtgenOwnRankingList> own_ranking_list)
    {
        this.ranking_id = ranking_id;
        this.own_ranking_list = own_ranking_list;
    }

    public TimeAttackRankingData() { }
}

[MessagePackObject(true)]
public class TreasureTradeList
{
    public int priority { get; set; }
    public int treasure_trade_id { get; set; }
    public int tab_group_id { get; set; }
    public int is_lock_view { get; set; }
    public int commence_date { get; set; }
    public int complete_date { get; set; }
    public int reset_type { get; set; }
    public int limit { get; set; }
    public IEnumerable<AtgenNeedTradeEntityList> need_trade_entity_list { get; set; }
    public int destination_entity_type { get; set; }
    public int destination_entity_id { get; set; }
    public int destination_entity_quantity { get; set; }
    public int destination_limit_break_count { get; set; }

    public TreasureTradeList(
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
    )
    {
        this.priority = priority;
        this.treasure_trade_id = treasure_trade_id;
        this.tab_group_id = tab_group_id;
        this.is_lock_view = is_lock_view;
        this.commence_date = commence_date;
        this.complete_date = complete_date;
        this.reset_type = reset_type;
        this.limit = limit;
        this.need_trade_entity_list = need_trade_entity_list;
        this.destination_entity_type = destination_entity_type;
        this.destination_entity_id = destination_entity_id;
        this.destination_entity_quantity = destination_entity_quantity;
        this.destination_limit_break_count = destination_limit_break_count;
    }

    public TreasureTradeList() { }
}

[MessagePackObject(true)]
public class UnitStoryList
{
    public int unit_story_id { get; set; }
    public int is_read { get; set; }

    public UnitStoryList(int unit_story_id, int is_read)
    {
        this.unit_story_id = unit_story_id;
        this.is_read = is_read;
    }

    public UnitStoryList() { }
}

[MessagePackObject(true)]
public class UnusedType { }

[MessagePackObject(true)]
public class UpdateDataList
{
    public UserData user_data { get; set; }
    public DiamondData diamond_data { get; set; }
    public PartyPowerData party_power_data { get; set; }
    public IEnumerable<CharaList> chara_list { get; set; }
    public IEnumerable<DragonList> dragon_list { get; set; }
    public IEnumerable<WeaponList> weapon_list { get; set; }
    public IEnumerable<AmuletList> amulet_list { get; set; }
    public IEnumerable<WeaponSkinList> weapon_skin_list { get; set; }
    public IEnumerable<WeaponBodyList> weapon_body_list { get; set; }
    public IEnumerable<WeaponPassiveAbilityList> weapon_passive_ability_list { get; set; }
    public IEnumerable<AbilityCrestList> ability_crest_list { get; set; }
    public IEnumerable<AbilityCrestSetList> ability_crest_set_list { get; set; }
    public IEnumerable<TalismanList> talisman_list { get; set; }

    private IEnumerable<PartyList> _party_list;
    public IEnumerable<PartyList> party_list
    {
        get => this._party_list;
        set
        {
            if (value is null)
                return;

            this._party_list = value.Select(
                x =>
                    new PartyList()
                    {
                        party_name = x.party_name,
                        party_no = x.party_no,
                        party_setting_list = x.party_setting_list.OrderBy(y => y.unit_no)
                    }
            );
        }
    }
    public IEnumerable<MuseumList> museum_list { get; set; }
    public IEnumerable<AlbumDragonData> album_dragon_list { get; set; }
    public IEnumerable<AlbumWeaponList> album_weapon_list { get; set; }
    public IEnumerable<EnemyBookList> enemy_book_list { get; set; }
    public IEnumerable<ItemList> item_list { get; set; }
    public IEnumerable<AstralItemList> astral_item_list { get; set; }
    public IEnumerable<MaterialList> material_list { get; set; }
    public IEnumerable<QuestList> quest_list { get; set; }
    public IEnumerable<QuestEventList> quest_event_list { get; set; }
    public IEnumerable<DragonGiftList> dragon_gift_list { get; set; }
    public IEnumerable<DragonReliabilityList> dragon_reliability_list { get; set; }
    public IEnumerable<UnitStoryList> unit_story_list { get; set; }
    public IEnumerable<CastleStoryList> castle_story_list { get; set; }
    public IEnumerable<QuestStoryList> quest_story_list { get; set; }
    public IEnumerable<QuestTreasureList> quest_treasure_list { get; set; }
    public IEnumerable<QuestWallList> quest_wall_list { get; set; }
    public IEnumerable<QuestCarryList> quest_carry_list { get; set; }
    public IEnumerable<QuestEntryConditionList> quest_entry_condition_list { get; set; }
    public IEnumerable<SummonTicketList> summon_ticket_list { get; set; }
    public IEnumerable<SummonPointList> summon_point_list { get; set; }
    public IEnumerable<LotteryTicketList> lottery_ticket_list { get; set; }
    public IEnumerable<ExchangeTicketList> exchange_ticket_list { get; set; }
    public IEnumerable<GatherItemList> gather_item_list { get; set; }
    public IEnumerable<BuildList> build_list { get; set; }
    public IEnumerable<FortPlantList> fort_plant_list { get; set; }
    public FortBonusList fort_bonus_list { get; set; }
    public IEnumerable<CraftList> craft_list { get; set; }
    public CurrentMainStoryMission current_main_story_mission { get; set; }
    public IEnumerable<CharaUnitSetList> chara_unit_set_list { get; set; }
    public UserGuildData user_guild_data { get; set; }
    public GuildData guild_data { get; set; }
    public IEnumerable<BattleRoyalCharaSkinList> battle_royal_chara_skin_list { get; set; }
    public DmodeInfo dmode_info { get; set; }
    public IEnumerable<DmodeStoryList> dmode_story_list { get; set; }
    public PresentNotice present_notice { get; set; }
    public FriendNotice friend_notice { get; set; }
    public MissionNotice mission_notice { get; set; }
    public GuildNotice guild_notice { get; set; }
    public ShopNotice shop_notice { get; set; }
    public AlbumPassiveNotice album_passive_notice { get; set; }
    public IEnumerable<RaidEventUserList> raid_event_user_list { get; set; }
    public IEnumerable<MazeEventUserList> maze_event_user_list { get; set; }
    public IEnumerable<BuildEventUserList> build_event_user_list { get; set; }
    public IEnumerable<CollectEventUserList> collect_event_user_list { get; set; }
    public IEnumerable<Clb01EventUserList> clb_01_event_user_list { get; set; }
    public IEnumerable<ExRushEventUserList> ex_rush_event_user_list { get; set; }
    public IEnumerable<SimpleEventUserList> simple_event_user_list { get; set; }
    public IEnumerable<ExHunterEventUserList> ex_hunter_event_user_list { get; set; }
    public IEnumerable<CombatEventUserList> combat_event_user_list { get; set; }
    public IEnumerable<BattleRoyalEventItemList> battle_royal_event_item_list { get; set; }
    public IEnumerable<BattleRoyalEventUserRecord> battle_royal_event_user_record { get; set; }
    public IEnumerable<BattleRoyalCycleUserRecord> battle_royal_cycle_user_record { get; set; }
    public IEnumerable<EarnEventUserList> earn_event_user_list { get; set; }
    public IEnumerable<EventPassiveList> event_passive_list { get; set; }
    public IEnumerable<FunctionalMaintenanceList> functional_maintenance_list { get; set; }

    public UpdateDataList(
        UserData user_data,
        DiamondData diamond_data,
        PartyPowerData party_power_data,
        IEnumerable<CharaList> chara_list,
        IEnumerable<DragonList> dragon_list,
        IEnumerable<WeaponList> weapon_list,
        IEnumerable<AmuletList> amulet_list,
        IEnumerable<WeaponSkinList> weapon_skin_list,
        IEnumerable<WeaponBodyList> weapon_body_list,
        IEnumerable<WeaponPassiveAbilityList> weapon_passive_ability_list,
        IEnumerable<AbilityCrestList> ability_crest_list,
        IEnumerable<AbilityCrestSetList> ability_crest_set_list,
        IEnumerable<TalismanList> talisman_list,
        IEnumerable<PartyList> party_list,
        IEnumerable<MuseumList> museum_list,
        IEnumerable<AlbumDragonData> album_dragon_list,
        IEnumerable<AlbumWeaponList> album_weapon_list,
        IEnumerable<EnemyBookList> enemy_book_list,
        IEnumerable<ItemList> item_list,
        IEnumerable<AstralItemList> astral_item_list,
        IEnumerable<MaterialList> material_list,
        IEnumerable<QuestList> quest_list,
        IEnumerable<QuestEventList> quest_event_list,
        IEnumerable<DragonGiftList> dragon_gift_list,
        IEnumerable<DragonReliabilityList> dragon_reliability_list,
        IEnumerable<UnitStoryList> unit_story_list,
        IEnumerable<CastleStoryList> castle_story_list,
        IEnumerable<QuestStoryList> quest_story_list,
        IEnumerable<QuestTreasureList> quest_treasure_list,
        IEnumerable<QuestWallList> quest_wall_list,
        IEnumerable<QuestCarryList> quest_carry_list,
        IEnumerable<QuestEntryConditionList> quest_entry_condition_list,
        IEnumerable<SummonTicketList> summon_ticket_list,
        IEnumerable<SummonPointList> summon_point_list,
        IEnumerable<LotteryTicketList> lottery_ticket_list,
        IEnumerable<ExchangeTicketList> exchange_ticket_list,
        IEnumerable<GatherItemList> gather_item_list,
        IEnumerable<BuildList> build_list,
        IEnumerable<FortPlantList> fort_plant_list,
        FortBonusList fort_bonus_list,
        IEnumerable<CraftList> craft_list,
        CurrentMainStoryMission current_main_story_mission,
        IEnumerable<CharaUnitSetList> chara_unit_set_list,
        UserGuildData user_guild_data,
        GuildData guild_data,
        IEnumerable<BattleRoyalCharaSkinList> battle_royal_chara_skin_list,
        DmodeInfo dmode_info,
        IEnumerable<DmodeStoryList> dmode_story_list,
        PresentNotice present_notice,
        FriendNotice friend_notice,
        MissionNotice mission_notice,
        GuildNotice guild_notice,
        ShopNotice shop_notice,
        AlbumPassiveNotice album_passive_notice,
        IEnumerable<RaidEventUserList> raid_event_user_list,
        IEnumerable<MazeEventUserList> maze_event_user_list,
        IEnumerable<BuildEventUserList> build_event_user_list,
        IEnumerable<CollectEventUserList> collect_event_user_list,
        IEnumerable<Clb01EventUserList> clb_01_event_user_list,
        IEnumerable<ExRushEventUserList> ex_rush_event_user_list,
        IEnumerable<SimpleEventUserList> simple_event_user_list,
        IEnumerable<ExHunterEventUserList> ex_hunter_event_user_list,
        IEnumerable<CombatEventUserList> combat_event_user_list,
        IEnumerable<BattleRoyalEventItemList> battle_royal_event_item_list,
        IEnumerable<BattleRoyalEventUserRecord> battle_royal_event_user_record,
        IEnumerable<BattleRoyalCycleUserRecord> battle_royal_cycle_user_record,
        IEnumerable<EarnEventUserList> earn_event_user_list,
        IEnumerable<EventPassiveList> event_passive_list,
        IEnumerable<FunctionalMaintenanceList> functional_maintenance_list
    )
    {
        this.user_data = user_data;
        this.diamond_data = diamond_data;
        this.party_power_data = party_power_data;
        this.chara_list = chara_list;
        this.dragon_list = dragon_list;
        this.weapon_list = weapon_list;
        this.amulet_list = amulet_list;
        this.weapon_skin_list = weapon_skin_list;
        this.weapon_body_list = weapon_body_list;
        this.weapon_passive_ability_list = weapon_passive_ability_list;
        this.ability_crest_list = ability_crest_list;
        this.ability_crest_set_list = ability_crest_set_list;
        this.talisman_list = talisman_list;
        this.party_list = party_list;
        this.museum_list = museum_list;
        this.album_dragon_list = album_dragon_list;
        this.album_weapon_list = album_weapon_list;
        this.enemy_book_list = enemy_book_list;
        this.item_list = item_list;
        this.astral_item_list = astral_item_list;
        this.material_list = material_list;
        this.quest_list = quest_list;
        this.quest_event_list = quest_event_list;
        this.dragon_gift_list = dragon_gift_list;
        this.dragon_reliability_list = dragon_reliability_list;
        this.unit_story_list = unit_story_list;
        this.castle_story_list = castle_story_list;
        this.quest_story_list = quest_story_list;
        this.quest_treasure_list = quest_treasure_list;
        this.quest_wall_list = quest_wall_list;
        this.quest_carry_list = quest_carry_list;
        this.quest_entry_condition_list = quest_entry_condition_list;
        this.summon_ticket_list = summon_ticket_list;
        this.summon_point_list = summon_point_list;
        this.lottery_ticket_list = lottery_ticket_list;
        this.exchange_ticket_list = exchange_ticket_list;
        this.gather_item_list = gather_item_list;
        this.build_list = build_list;
        this.fort_plant_list = fort_plant_list;
        this.fort_bonus_list = fort_bonus_list;
        this.craft_list = craft_list;
        this.current_main_story_mission = current_main_story_mission;
        this.chara_unit_set_list = chara_unit_set_list;
        this.user_guild_data = user_guild_data;
        this.guild_data = guild_data;
        this.battle_royal_chara_skin_list = battle_royal_chara_skin_list;
        this.dmode_info = dmode_info;
        this.dmode_story_list = dmode_story_list;
        this.present_notice = present_notice;
        this.friend_notice = friend_notice;
        this.mission_notice = mission_notice;
        this.guild_notice = guild_notice;
        this.shop_notice = shop_notice;
        this.album_passive_notice = album_passive_notice;
        this.raid_event_user_list = raid_event_user_list;
        this.maze_event_user_list = maze_event_user_list;
        this.build_event_user_list = build_event_user_list;
        this.collect_event_user_list = collect_event_user_list;
        this.clb_01_event_user_list = clb_01_event_user_list;
        this.ex_rush_event_user_list = ex_rush_event_user_list;
        this.simple_event_user_list = simple_event_user_list;
        this.ex_hunter_event_user_list = ex_hunter_event_user_list;
        this.combat_event_user_list = combat_event_user_list;
        this.battle_royal_event_item_list = battle_royal_event_item_list;
        this.battle_royal_event_user_record = battle_royal_event_user_record;
        this.battle_royal_cycle_user_record = battle_royal_cycle_user_record;
        this.earn_event_user_list = earn_event_user_list;
        this.event_passive_list = event_passive_list;
        this.functional_maintenance_list = functional_maintenance_list;
    }

    public UpdateDataList() { }
}

[MessagePackObject(true)]
public class UserAbilityCrestTradeList
{
    public int ability_crest_trade_id { get; set; }
    public int trade_count { get; set; }

    public UserAbilityCrestTradeList(int ability_crest_trade_id, int trade_count)
    {
        this.ability_crest_trade_id = ability_crest_trade_id;
        this.trade_count = trade_count;
    }

    public UserAbilityCrestTradeList() { }
}

[MessagePackObject(true)]
public class UserAmuletTradeList
{
    public int amulet_trade_id { get; set; }
    public int trade_count { get; set; }

    public UserAmuletTradeList(int amulet_trade_id, int trade_count)
    {
        this.amulet_trade_id = amulet_trade_id;
        this.trade_count = trade_count;
    }

    public UserAmuletTradeList() { }
}

[MessagePackObject(true)]
public class UserData
{
    public ulong viewer_id { get; set; }
    public string name { get; set; }
    public int level { get; set; }
    public int exp { get; set; }
    public long coin { get; set; }
    public int crystal { get; set; }
    public int dew_point { get; set; }
    public int stamina_single { get; set; }
    public DateTimeOffset last_stamina_single_update_time { get; set; }
    public int stamina_single_surplus_second { get; set; }
    public int stamina_multi { get; set; }
    public DateTimeOffset last_stamina_multi_update_time { get; set; }
    public int stamina_multi_surplus_second { get; set; }
    public int max_dragon_quantity { get; set; }
    public int max_weapon_quantity { get; set; }
    public int max_amulet_quantity { get; set; }
    public int quest_skip_point { get; set; }
    public int build_time_point { get; set; }
    public int age_group { get; set; }
    public int main_party_no { get; set; }
    public int emblem_id { get; set; }
    public int active_memory_event_id { get; set; }
    public int mana_point { get; set; }
    public DateTimeOffset last_login_time { get; set; }
    public int tutorial_status { get; set; }
    public IEnumerable<int> tutorial_flag_list { get; set; }
    public int prologue_end_time { get; set; }
    public DateTimeOffset fort_open_time { get; set; }
    public DateTimeOffset create_time { get; set; }
    public int is_optin { get; set; }

    public UserData(
        ulong viewer_id,
        string name,
        int level,
        int exp,
        long coin,
        int crystal,
        int dew_point,
        int stamina_single,
        DateTimeOffset last_stamina_single_update_time,
        int stamina_single_surplus_second,
        int stamina_multi,
        DateTimeOffset last_stamina_multi_update_time,
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
        DateTimeOffset last_login_time,
        int tutorial_status,
        IEnumerable<int> tutorial_flag_list,
        int prologue_end_time,
        DateTimeOffset fort_open_time,
        DateTimeOffset create_time,
        int is_optin
    )
    {
        this.viewer_id = viewer_id;
        this.name = name;
        this.level = level;
        this.exp = exp;
        this.coin = coin;
        this.crystal = crystal;
        this.dew_point = dew_point;
        this.stamina_single = stamina_single;
        this.last_stamina_single_update_time = last_stamina_single_update_time;
        this.stamina_single_surplus_second = stamina_single_surplus_second;
        this.stamina_multi = stamina_multi;
        this.last_stamina_multi_update_time = last_stamina_multi_update_time;
        this.stamina_multi_surplus_second = stamina_multi_surplus_second;
        this.max_dragon_quantity = max_dragon_quantity;
        this.max_weapon_quantity = max_weapon_quantity;
        this.max_amulet_quantity = max_amulet_quantity;
        this.quest_skip_point = quest_skip_point;
        this.build_time_point = build_time_point;
        this.age_group = age_group;
        this.main_party_no = main_party_no;
        this.emblem_id = emblem_id;
        this.active_memory_event_id = active_memory_event_id;
        this.mana_point = mana_point;
        this.last_login_time = last_login_time;
        this.tutorial_status = tutorial_status;
        this.tutorial_flag_list = tutorial_flag_list;
        this.prologue_end_time = prologue_end_time;
        this.fort_open_time = fort_open_time;
        this.create_time = create_time;
        this.is_optin = is_optin;
    }

    public UserData() { }
}

[MessagePackObject(true)]
public class UserEventItemData
{
    public IEnumerable<AtgenUserMazeEventItemList__2> user_maze_event_item_list { get; set; }

    public UserEventItemData(IEnumerable<AtgenUserMazeEventItemList__2> user_maze_event_item_list)
    {
        this.user_maze_event_item_list = user_maze_event_item_list;
    }

    public UserEventItemData() { }
}

[MessagePackObject(true)]
public class UserEventLocationRewardList
{
    public int event_id { get; set; }
    public int location_reward_id { get; set; }

    public UserEventLocationRewardList(int event_id, int location_reward_id)
    {
        this.event_id = event_id;
        this.location_reward_id = location_reward_id;
    }

    public UserEventLocationRewardList() { }
}

[MessagePackObject(true)]
public class UserGuildData
{
    public int guild_id { get; set; }
    public ulong guild_apply_id { get; set; }
    public int penalty_end_time { get; set; }
    public int guild_push_notification_type_running { get; set; }
    public int guild_push_notification_type_suspending { get; set; }
    public int profile_entity_type { get; set; }
    public int profile_entity_id { get; set; }
    public int profile_entity_rarity { get; set; }
    public int is_enable_invite_receive { get; set; }
    public int is_enable_invite_send { get; set; }
    public int last_attend_time { get; set; }

    public UserGuildData(
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
    )
    {
        this.guild_id = guild_id;
        this.guild_apply_id = guild_apply_id;
        this.penalty_end_time = penalty_end_time;
        this.guild_push_notification_type_running = guild_push_notification_type_running;
        this.guild_push_notification_type_suspending = guild_push_notification_type_suspending;
        this.profile_entity_type = profile_entity_type;
        this.profile_entity_id = profile_entity_id;
        this.profile_entity_rarity = profile_entity_rarity;
        this.is_enable_invite_receive = is_enable_invite_receive;
        this.is_enable_invite_send = is_enable_invite_send;
        this.last_attend_time = last_attend_time;
    }

    public UserGuildData() { }
}

[MessagePackObject(true)]
public class UserRedoableSummonData
{
    public int is_fixed_result { get; set; }
    public IEnumerable<AtgenRedoableSummonResultUnitList> redoable_summon_result_unit_list { get; set; }

    public UserRedoableSummonData(
        int is_fixed_result,
        IEnumerable<AtgenRedoableSummonResultUnitList> redoable_summon_result_unit_list
    )
    {
        this.is_fixed_result = is_fixed_result;
        this.redoable_summon_result_unit_list = redoable_summon_result_unit_list;
    }

    public UserRedoableSummonData() { }
}

[MessagePackObject(true)]
public class UserSummonList
{
    public int summon_id { get; set; }
    public int summon_count { get; set; }
    public int campaign_type { get; set; }
    public int free_count_rest { get; set; }
    public int is_beginner_campaign { get; set; }
    public int beginner_campaign_count_rest { get; set; }
    public int consecution_campaign_count_rest { get; set; }

    public UserSummonList(
        int summon_id,
        int summon_count,
        int campaign_type,
        int free_count_rest,
        int is_beginner_campaign,
        int beginner_campaign_count_rest,
        int consecution_campaign_count_rest
    )
    {
        this.summon_id = summon_id;
        this.summon_count = summon_count;
        this.campaign_type = campaign_type;
        this.free_count_rest = free_count_rest;
        this.is_beginner_campaign = is_beginner_campaign;
        this.beginner_campaign_count_rest = beginner_campaign_count_rest;
        this.consecution_campaign_count_rest = consecution_campaign_count_rest;
    }

    public UserSummonList() { }
}

[MessagePackObject(true)]
public class UserSupportList
{
    public ulong viewer_id { get; set; }
    public string name { get; set; }
    public int level { get; set; }

    public DateTimeOffset last_login_date { get; set; }
    public int emblem_id { get; set; }
    public int max_party_power { get; set; }
    public AtgenGuild guild { get; set; }
    public AtgenSupportChara support_chara { get; set; }
    public AtgenSupportWeapon support_weapon { get; set; }
    public AtgenSupportDragon support_dragon { get; set; }
    public AtgenSupportWeaponBody support_weapon_body { get; set; }
    public IEnumerable<AtgenSupportCrestSlotType1List> support_crest_slot_type_1_list { get; set; }
    public IEnumerable<AtgenSupportCrestSlotType1List> support_crest_slot_type_2_list { get; set; }
    public IEnumerable<AtgenSupportCrestSlotType1List> support_crest_slot_type_3_list { get; set; }
    public AtgenSupportTalisman support_talisman { get; set; }
    public AtgenSupportAmulet support_amulet { get; set; }
    public AtgenSupportAmulet support_amulet_2 { get; set; }

    public UserSupportList(
        ulong viewer_id,
        string name,
        int level,
        DateTimeOffset last_login_date,
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
    )
    {
        this.viewer_id = viewer_id;
        this.name = name;
        this.level = level;
        this.last_login_date = last_login_date;
        this.emblem_id = emblem_id;
        this.max_party_power = max_party_power;
        this.guild = guild;
        this.support_chara = support_chara;
        this.support_weapon = support_weapon;
        this.support_dragon = support_dragon;
        this.support_weapon_body = support_weapon_body;
        this.support_crest_slot_type_1_list = support_crest_slot_type_1_list;
        this.support_crest_slot_type_2_list = support_crest_slot_type_2_list;
        this.support_crest_slot_type_3_list = support_crest_slot_type_3_list;
        this.support_talisman = support_talisman;
        this.support_amulet = support_amulet;
        this.support_amulet_2 = support_amulet_2;
    }

    public UserSupportList() { }
}

[MessagePackObject(true)]
public class UserTreasureTradeList
{
    public int treasure_trade_id { get; set; }
    public int trade_count { get; set; }
    public int last_reset_time { get; set; }

    public UserTreasureTradeList(int treasure_trade_id, int trade_count, int last_reset_time)
    {
        this.treasure_trade_id = treasure_trade_id;
        this.trade_count = trade_count;
        this.last_reset_time = last_reset_time;
    }

    public UserTreasureTradeList() { }
}

[MessagePackObject(true)]
public class WalletBalance
{
    public int total { get; set; }
    public int free { get; set; }
    public IEnumerable<AtgenPaid> paid { get; set; }

    public WalletBalance(int total, int free, IEnumerable<AtgenPaid> paid)
    {
        this.total = total;
        this.free = free;
        this.paid = paid;
    }

    public WalletBalance() { }
}

[MessagePackObject(true)]
public class WeaponBodyList
{
    public int weapon_body_id { get; set; }
    public int buildup_count { get; set; }
    public int limit_break_count { get; set; }
    public int limit_over_count { get; set; }
    public int equipable_count { get; set; }
    public int additional_crest_slot_type_1_count { get; set; }
    public int additional_crest_slot_type_2_count { get; set; }
    public int additional_crest_slot_type_3_count { get; set; }
    public int additional_effect_count { get; set; }
    public IEnumerable<int> unlock_weapon_passive_ability_no_list { get; set; }
    public int fort_passive_chara_weapon_buildup_count { get; set; }
    public int is_new { get; set; }
    public DateTimeOffset gettime { get; set; }
    public int skill_no { get; set; }
    public int skill_level { get; set; }
    public int ability_1_level { get; set; }
    public int ability_2_levell { get; set; }

    public WeaponBodyList(
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
        DateTimeOffset gettime,
        int skill_no,
        int skill_level,
        int ability_1_level,
        int ability_2_levell
    )
    {
        this.weapon_body_id = weapon_body_id;
        this.buildup_count = buildup_count;
        this.limit_break_count = limit_break_count;
        this.limit_over_count = limit_over_count;
        this.equipable_count = equipable_count;
        this.additional_crest_slot_type_1_count = additional_crest_slot_type_1_count;
        this.additional_crest_slot_type_2_count = additional_crest_slot_type_2_count;
        this.additional_crest_slot_type_3_count = additional_crest_slot_type_3_count;
        this.additional_effect_count = additional_effect_count;
        this.unlock_weapon_passive_ability_no_list = unlock_weapon_passive_ability_no_list;
        this.fort_passive_chara_weapon_buildup_count = fort_passive_chara_weapon_buildup_count;
        this.is_new = is_new;
        this.gettime = gettime;
        this.skill_no = skill_no;
        this.skill_level = skill_level;
        this.ability_1_level = ability_1_level;
        this.ability_2_levell = ability_2_levell;
    }

    public WeaponBodyList() { }
}

[MessagePackObject(true)]
public class WeaponList
{
    public int weapon_id { get; set; }
    public ulong weapon_key_id { get; set; }
    public int is_lock { get; set; }
    public int is_new { get; set; }
    public int gettime { get; set; }
    public int skill_level { get; set; }
    public int hp_plus_count { get; set; }
    public int attack_plus_count { get; set; }
    public int status_plus_count { get; set; }
    public int level { get; set; }
    public int limit_break_count { get; set; }
    public int exp { get; set; }

    public WeaponList(
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
    )
    {
        this.weapon_id = weapon_id;
        this.weapon_key_id = weapon_key_id;
        this.is_lock = is_lock;
        this.is_new = is_new;
        this.gettime = gettime;
        this.skill_level = skill_level;
        this.hp_plus_count = hp_plus_count;
        this.attack_plus_count = attack_plus_count;
        this.status_plus_count = status_plus_count;
        this.level = level;
        this.limit_break_count = limit_break_count;
        this.exp = exp;
    }

    public WeaponList() { }
}

[MessagePackObject(true)]
public class WeaponPassiveAbilityList
{
    public int weapon_passive_ability_id { get; set; }

    public WeaponPassiveAbilityList(int weapon_passive_ability_id)
    {
        this.weapon_passive_ability_id = weapon_passive_ability_id;
    }

    public WeaponPassiveAbilityList() { }
}

[MessagePackObject(true)]
public class WeaponSkinList
{
    public int weapon_skin_id { get; set; }
    public int is_new { get; set; }
    public int gettime { get; set; }

    public WeaponSkinList(int weapon_skin_id, int is_new, int gettime)
    {
        this.weapon_skin_id = weapon_skin_id;
        this.is_new = is_new;
        this.gettime = gettime;
    }

    public WeaponSkinList() { }
}
