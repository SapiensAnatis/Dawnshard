#nullable disable

using System.Text.Json.Serialization;
using DragaliaAPI.MessagePack;
using MessagePack;

namespace DragaliaAPI.Models.Generated;

[MessagePackObject(true)]
public class AbilityCrestBuildupPieceData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AbilityCrestBuildupPieceData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AbilityCrestBuildupPieceData() { }
}

[MessagePackObject(true)]
public class AbilityCrestBuildupPlusCountData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AbilityCrestBuildupPlusCountData(
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AbilityCrestBuildupPlusCountData() { }
}

[MessagePackObject(true)]
public class AbilityCrestGetAbilityCrestSetListData
{
    public IEnumerable<AbilityCrestSetList> ability_crest_set_list { get; set; }

    public AbilityCrestGetAbilityCrestSetListData(
        IEnumerable<AbilityCrestSetList> ability_crest_set_list
    )
    {
        this.ability_crest_set_list = ability_crest_set_list;
    }

    public AbilityCrestGetAbilityCrestSetListData() { }
}

[MessagePackObject(true)]
public class AbilityCrestResetPlusCountData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AbilityCrestResetPlusCountData(
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AbilityCrestResetPlusCountData() { }
}

[MessagePackObject(true)]
public class AbilityCrestSetAbilityCrestSetData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AbilityCrestSetAbilityCrestSetData(
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AbilityCrestSetAbilityCrestSetData() { }
}

[MessagePackObject(true)]
public class AbilityCrestSetFavoriteData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AbilityCrestSetFavoriteData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AbilityCrestSetFavoriteData() { }
}

[MessagePackObject(true)]
public class AbilityCrestTradeGetListData
{
    public IEnumerable<UserAbilityCrestTradeList> user_ability_crest_trade_list { get; set; }
    public IEnumerable<AbilityCrestTradeList> ability_crest_trade_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AbilityCrestTradeGetListData(
        IEnumerable<UserAbilityCrestTradeList> user_ability_crest_trade_list,
        IEnumerable<AbilityCrestTradeList> ability_crest_trade_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.user_ability_crest_trade_list = user_ability_crest_trade_list;
        this.ability_crest_trade_list = ability_crest_trade_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AbilityCrestTradeGetListData() { }
}

[MessagePackObject(true)]
public class AbilityCrestTradeTradeData
{
    public IEnumerable<UserAbilityCrestTradeList> user_ability_crest_trade_list { get; set; }
    public IEnumerable<AbilityCrestTradeList> ability_crest_trade_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AbilityCrestTradeTradeData(
        IEnumerable<UserAbilityCrestTradeList> user_ability_crest_trade_list,
        IEnumerable<AbilityCrestTradeList> ability_crest_trade_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.user_ability_crest_trade_list = user_ability_crest_trade_list;
        this.ability_crest_trade_list = ability_crest_trade_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AbilityCrestTradeTradeData() { }
}

[MessagePackObject(true)]
public class AbilityCrestUpdateAbilityCrestSetNameData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AbilityCrestUpdateAbilityCrestSetNameData(
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AbilityCrestUpdateAbilityCrestSetNameData() { }
}

[MessagePackObject(true)]
public class AlbumIndexData
{
    public IEnumerable<AtgenAlbumQuestPlayRecordList> album_quest_play_record_list { get; set; }
    public IEnumerable<AlbumDragonData> album_dragon_list { get; set; }
    public AlbumPassiveNotice album_passive_update_result { get; set; }
    public IEnumerable<AtgenCharaHonorList> chara_honor_list { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public AlbumIndexData(
        IEnumerable<AtgenAlbumQuestPlayRecordList> album_quest_play_record_list,
        IEnumerable<AlbumDragonData> album_dragon_list,
        AlbumPassiveNotice album_passive_update_result,
        IEnumerable<AtgenCharaHonorList> chara_honor_list,
        UpdateDataList update_data_list
    )
    {
        this.album_quest_play_record_list = album_quest_play_record_list;
        this.album_dragon_list = album_dragon_list;
        this.album_passive_update_result = album_passive_update_result;
        this.chara_honor_list = chara_honor_list;
        this.update_data_list = update_data_list;
    }

    public AlbumIndexData() { }
}

[MessagePackObject(true)]
public class AmuletBuildupData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public DeleteDataList delete_data_list { get; set; }

    public AmuletBuildupData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        DeleteDataList delete_data_list
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.delete_data_list = delete_data_list;
    }

    public AmuletBuildupData() { }
}

[MessagePackObject(true)]
public class AmuletLimitBreakData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public DeleteDataList delete_data_list { get; set; }

    public AmuletLimitBreakData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        DeleteDataList delete_data_list
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.delete_data_list = delete_data_list;
    }

    public AmuletLimitBreakData() { }
}

[MessagePackObject(true)]
public class AmuletResetPlusCountData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AmuletResetPlusCountData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AmuletResetPlusCountData() { }
}

[MessagePackObject(true)]
public class AmuletSellData
{
    public DeleteDataList delete_data_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AmuletSellData(
        DeleteDataList delete_data_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.delete_data_list = delete_data_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AmuletSellData() { }
}

[MessagePackObject(true)]
public class AmuletSetLockData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AmuletSetLockData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AmuletSetLockData() { }
}

[MessagePackObject(true)]
public class AmuletTradeGetListData
{
    public IEnumerable<UserAmuletTradeList> user_amulet_trade_list { get; set; }
    public IEnumerable<AmuletTradeList> amulet_trade_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AmuletTradeGetListData(
        IEnumerable<UserAmuletTradeList> user_amulet_trade_list,
        IEnumerable<AmuletTradeList> amulet_trade_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.user_amulet_trade_list = user_amulet_trade_list;
        this.amulet_trade_list = amulet_trade_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AmuletTradeGetListData() { }
}

[MessagePackObject(true)]
public class AmuletTradeTradeData
{
    public IEnumerable<UserAmuletTradeList> user_amulet_trade_list { get; set; }
    public IEnumerable<AmuletTradeList> amulet_trade_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public AmuletTradeTradeData(
        IEnumerable<UserAmuletTradeList> user_amulet_trade_list,
        IEnumerable<AmuletTradeList> amulet_trade_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.user_amulet_trade_list = user_amulet_trade_list;
        this.amulet_trade_list = amulet_trade_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public AmuletTradeTradeData() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventEntryData
{
    public BattleRoyalEventUserRecord battle_royal_event_user_record { get; set; }
    public BattleRoyalCycleUserRecord battle_royal_cycle_user_record { get; set; }
    public IEnumerable<BattleRoyalEventItemList> battle_royal_event_item_list { get; set; }
    public IEnumerable<EventCycleRewardList> event_cycle_reward_list { get; set; }
    public IEnumerable<BattleRoyalCharaSkinList> battle_royal_chara_skin_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public BattleRoyalEventEntryData(
        BattleRoyalEventUserRecord battle_royal_event_user_record,
        BattleRoyalCycleUserRecord battle_royal_cycle_user_record,
        IEnumerable<BattleRoyalEventItemList> battle_royal_event_item_list,
        IEnumerable<EventCycleRewardList> event_cycle_reward_list,
        IEnumerable<BattleRoyalCharaSkinList> battle_royal_chara_skin_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.battle_royal_event_user_record = battle_royal_event_user_record;
        this.battle_royal_cycle_user_record = battle_royal_cycle_user_record;
        this.battle_royal_event_item_list = battle_royal_event_item_list;
        this.event_cycle_reward_list = event_cycle_reward_list;
        this.battle_royal_chara_skin_list = battle_royal_chara_skin_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public BattleRoyalEventEntryData() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventGetEventDataData
{
    public BattleRoyalEventUserRecord battle_royal_event_user_record { get; set; }
    public BattleRoyalCycleUserRecord battle_royal_cycle_user_record { get; set; }
    public IEnumerable<BattleRoyalEventItemList> battle_royal_event_item_list { get; set; }
    public IEnumerable<EventCycleRewardList> event_cycle_reward_list { get; set; }
    public IEnumerable<BattleRoyalCharaSkinList> battle_royal_chara_skin_list { get; set; }
    public IEnumerable<EventTradeList> event_trade_list { get; set; }

    public BattleRoyalEventGetEventDataData(
        BattleRoyalEventUserRecord battle_royal_event_user_record,
        BattleRoyalCycleUserRecord battle_royal_cycle_user_record,
        IEnumerable<BattleRoyalEventItemList> battle_royal_event_item_list,
        IEnumerable<EventCycleRewardList> event_cycle_reward_list,
        IEnumerable<BattleRoyalCharaSkinList> battle_royal_chara_skin_list,
        IEnumerable<EventTradeList> event_trade_list
    )
    {
        this.battle_royal_event_user_record = battle_royal_event_user_record;
        this.battle_royal_cycle_user_record = battle_royal_cycle_user_record;
        this.battle_royal_event_item_list = battle_royal_event_item_list;
        this.event_cycle_reward_list = event_cycle_reward_list;
        this.battle_royal_chara_skin_list = battle_royal_chara_skin_list;
        this.event_trade_list = event_trade_list;
    }

    public BattleRoyalEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventReceiveEventCyclePointRewardData
{
    public IEnumerable<EventCycleRewardList> event_cycle_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> event_cycle_reward_entity_list { get; set; }

    public BattleRoyalEventReceiveEventCyclePointRewardData(
        IEnumerable<EventCycleRewardList> event_cycle_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenBuildEventRewardEntityList> event_cycle_reward_entity_list
    )
    {
        this.event_cycle_reward_list = event_cycle_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.event_cycle_reward_entity_list = event_cycle_reward_entity_list;
    }

    public BattleRoyalEventReceiveEventCyclePointRewardData() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventReleaseCharaSkinData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public BattleRoyalEventReleaseCharaSkinData(
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public BattleRoyalEventReleaseCharaSkinData() { }
}

[MessagePackObject(true)]
public class BattleRoyalFailData
{
    public int result { get; set; }

    public BattleRoyalFailData(int result)
    {
        this.result = result;
    }

    public BattleRoyalFailData() { }
}

[MessagePackObject(true)]
public class BattleRoyalGetBattleRoyalHistoryData
{
    public IEnumerable<AtgenBattleRoyalHistoryList> battle_royal_history_list { get; set; }

    public BattleRoyalGetBattleRoyalHistoryData(
        IEnumerable<AtgenBattleRoyalHistoryList> battle_royal_history_list
    )
    {
        this.battle_royal_history_list = battle_royal_history_list;
    }

    public BattleRoyalGetBattleRoyalHistoryData() { }
}

[MessagePackObject(true)]
public class BattleRoyalRecordRoyalRecordMultiData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public BattleRoyalResult battle_royal_result { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> event_cycle_reward_entity_list { get; set; }

    public BattleRoyalRecordRoyalRecordMultiData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        BattleRoyalResult battle_royal_result,
        IEnumerable<AtgenBuildEventRewardEntityList> event_cycle_reward_entity_list
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.battle_royal_result = battle_royal_result;
        this.event_cycle_reward_entity_list = event_cycle_reward_entity_list;
    }

    public BattleRoyalRecordRoyalRecordMultiData() { }
}

[MessagePackObject(true)]
public class BattleRoyalStartMultiData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public AtgenBattleRoyalData battle_royal_data { get; set; }

    public BattleRoyalStartMultiData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        AtgenBattleRoyalData battle_royal_data
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.battle_royal_data = battle_royal_data;
    }

    public BattleRoyalStartMultiData() { }
}

[MessagePackObject(true)]
public class BattleRoyalStartRoyalMultiData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public AtgenBattleRoyalData battle_royal_data { get; set; }

    public BattleRoyalStartRoyalMultiData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        AtgenBattleRoyalData battle_royal_data
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.battle_royal_data = battle_royal_data;
    }

    public BattleRoyalStartRoyalMultiData() { }
}

[MessagePackObject(true)]
public class BuildEventEntryData
{
    public BuildEventUserList build_event_user_data { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public int is_receivable_event_daily_bonus { get; set; }

    public BuildEventEntryData(
        BuildEventUserList build_event_user_data,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        int is_receivable_event_daily_bonus
    )
    {
        this.build_event_user_data = build_event_user_data;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.is_receivable_event_daily_bonus = is_receivable_event_daily_bonus;
    }

    public BuildEventEntryData() { }
}

[MessagePackObject(true)]
public class BuildEventGetEventDataData
{
    public BuildEventUserList build_event_user_data { get; set; }
    public int is_receivable_event_daily_bonus { get; set; }
    public IEnumerable<BuildEventRewardList> build_event_reward_list { get; set; }
    public IEnumerable<EventTradeList> event_trade_list { get; set; }
    public AtgenEventFortData event_fort_data { get; set; }

    public BuildEventGetEventDataData(
        BuildEventUserList build_event_user_data,
        int is_receivable_event_daily_bonus,
        IEnumerable<BuildEventRewardList> build_event_reward_list,
        IEnumerable<EventTradeList> event_trade_list,
        AtgenEventFortData event_fort_data
    )
    {
        this.build_event_user_data = build_event_user_data;
        this.is_receivable_event_daily_bonus = is_receivable_event_daily_bonus;
        this.build_event_reward_list = build_event_reward_list;
        this.event_trade_list = event_trade_list;
        this.event_fort_data = event_fort_data;
    }

    public BuildEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class BuildEventReceiveBuildPointRewardData
{
    public IEnumerable<BuildEventRewardList> build_event_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> build_event_reward_entity_list { get; set; }

    public BuildEventReceiveBuildPointRewardData(
        IEnumerable<BuildEventRewardList> build_event_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenBuildEventRewardEntityList> build_event_reward_entity_list
    )
    {
        this.build_event_reward_list = build_event_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.build_event_reward_entity_list = build_event_reward_entity_list;
    }

    public BuildEventReceiveBuildPointRewardData() { }
}

[MessagePackObject(true)]
public class BuildEventReceiveDailyBonusData
{
    public IEnumerable<AtgenBuildEventRewardEntityList> event_daily_bonus_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public BuildEventReceiveDailyBonusData(
        IEnumerable<AtgenBuildEventRewardEntityList> event_daily_bonus_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.event_daily_bonus_list = event_daily_bonus_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public BuildEventReceiveDailyBonusData() { }
}

[MessagePackObject(true)]
public class CartoonLatestData
{
    public AtgenLatest latest { get; set; }

    public CartoonLatestData(AtgenLatest latest)
    {
        this.latest = latest;
    }

    public CartoonLatestData() { }
}

[MessagePackObject(true)]
public class CastleStoryReadData
{
    public IEnumerable<AtgenBuildEventRewardEntityList> castle_story_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list { get; set; }

    public CastleStoryReadData(
        IEnumerable<AtgenBuildEventRewardEntityList> castle_story_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list
    )
    {
        this.castle_story_reward_list = castle_story_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.duplicate_entity_list = duplicate_entity_list;
    }

    public CastleStoryReadData() { }
}

[MessagePackObject(true)]
public class CharaAwakeData
{
    public UpdateDataList update_data_list { get; set; }

    public CharaAwakeData(UpdateDataList update_data_list)
    {
        this.update_data_list = update_data_list;
    }

    public CharaAwakeData() { }
}

[MessagePackObject(true)]
public class CharaBuildupManaData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public CharaBuildupManaData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public CharaBuildupManaData() { }
}

[MessagePackObject(true)]
public class CharaBuildupPlatinumData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public CharaBuildupPlatinumData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public CharaBuildupPlatinumData() { }
}

[MessagePackObject(true)]
public class CharaBuildupData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public CharaBuildupData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public CharaBuildupData() { }
}

[MessagePackObject(true)]
public class CharaGetCharaUnitSetData
{
    public IEnumerable<CharaUnitSetList> chara_unit_set_list { get; set; }

    public CharaGetCharaUnitSetData(IEnumerable<CharaUnitSetList> chara_unit_set_list)
    {
        this.chara_unit_set_list = chara_unit_set_list;
    }

    public CharaGetCharaUnitSetData() { }
}

[MessagePackObject(true)]
public class CharaGetListData
{
    public IEnumerable<CharaList> chara_list { get; set; }

    public CharaGetListData(IEnumerable<CharaList> chara_list)
    {
        this.chara_list = chara_list;
    }

    public CharaGetListData() { }
}

[MessagePackObject(true)]
public class CharaLimitBreakAndBuildupManaData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public CharaLimitBreakAndBuildupManaData(
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public CharaLimitBreakAndBuildupManaData() { }
}

[MessagePackObject(true)]
public class CharaLimitBreakData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public CharaLimitBreakData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public CharaLimitBreakData() { }
}

[MessagePackObject(true)]
public class CharaResetPlusCountData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public CharaResetPlusCountData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public CharaResetPlusCountData() { }
}

[MessagePackObject(true)]
public class CharaSetCharaUnitSetData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public CharaSetCharaUnitSetData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public CharaSetCharaUnitSetData() { }
}

[MessagePackObject(true)]
public class CharaUnlockEditSkillData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public CharaUnlockEditSkillData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public CharaUnlockEditSkillData() { }
}

[MessagePackObject(true)]
public class Clb01EventEntryData
{
    public Clb01EventUserList clb_01_event_user_data { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public Clb01EventEntryData(
        Clb01EventUserList clb_01_event_user_data,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.clb_01_event_user_data = clb_01_event_user_data;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public Clb01EventEntryData() { }
}

[MessagePackObject(true)]
public class Clb01EventGetEventDataData
{
    public Clb01EventUserList clb_01_event_user_data { get; set; }
    public IEnumerable<BuildEventRewardList> clb_01_event_reward_list { get; set; }
    public IEnumerable<CharaFriendshipList> chara_friendship_list { get; set; }

    public Clb01EventGetEventDataData(
        Clb01EventUserList clb_01_event_user_data,
        IEnumerable<BuildEventRewardList> clb_01_event_reward_list,
        IEnumerable<CharaFriendshipList> chara_friendship_list
    )
    {
        this.clb_01_event_user_data = clb_01_event_user_data;
        this.clb_01_event_reward_list = clb_01_event_reward_list;
        this.chara_friendship_list = chara_friendship_list;
    }

    public Clb01EventGetEventDataData() { }
}

[MessagePackObject(true)]
public class Clb01EventReceiveClb01PointRewardData
{
    public IEnumerable<BuildEventRewardList> clb_01_event_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> clb_01_event_reward_entity_list { get; set; }

    public Clb01EventReceiveClb01PointRewardData(
        IEnumerable<BuildEventRewardList> clb_01_event_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenBuildEventRewardEntityList> clb_01_event_reward_entity_list
    )
    {
        this.clb_01_event_reward_list = clb_01_event_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.clb_01_event_reward_entity_list = clb_01_event_reward_entity_list;
    }

    public Clb01EventReceiveClb01PointRewardData() { }
}

[MessagePackObject(true)]
public class CollectEventEntryData
{
    public CollectEventUserList collect_event_user_data { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public CollectEventEntryData(
        CollectEventUserList collect_event_user_data,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.collect_event_user_data = collect_event_user_data;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public CollectEventEntryData() { }
}

[MessagePackObject(true)]
public class CollectEventGetEventDataData
{
    public CollectEventUserList collect_event_user_data { get; set; }
    public IEnumerable<EventStoryList> event_story_list { get; set; }

    public CollectEventGetEventDataData(
        CollectEventUserList collect_event_user_data,
        IEnumerable<EventStoryList> event_story_list
    )
    {
        this.collect_event_user_data = collect_event_user_data;
        this.event_story_list = event_story_list;
    }

    public CollectEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class CombatEventEntryData
{
    public CombatEventUserList combat_event_user_data { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public CombatEventEntryData(
        CombatEventUserList combat_event_user_data,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.combat_event_user_data = combat_event_user_data;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public CombatEventEntryData() { }
}

[MessagePackObject(true)]
public class CombatEventGetEventDataData
{
    public CombatEventUserList combat_event_user_data { get; set; }
    public IEnumerable<EventTradeList> event_trade_list { get; set; }
    public IEnumerable<BuildEventRewardList> event_reward_list { get; set; }
    public IEnumerable<UserEventLocationRewardList> user_event_location_reward_list { get; set; }

    public CombatEventGetEventDataData(
        CombatEventUserList combat_event_user_data,
        IEnumerable<EventTradeList> event_trade_list,
        IEnumerable<BuildEventRewardList> event_reward_list,
        IEnumerable<UserEventLocationRewardList> user_event_location_reward_list
    )
    {
        this.combat_event_user_data = combat_event_user_data;
        this.event_trade_list = event_trade_list;
        this.event_reward_list = event_reward_list;
        this.user_event_location_reward_list = user_event_location_reward_list;
    }

    public CombatEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class CombatEventReceiveEventLocationRewardData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<UserEventLocationRewardList> user_event_location_reward_list { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> event_location_reward_entity_list { get; set; }

    public CombatEventReceiveEventLocationRewardData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<UserEventLocationRewardList> user_event_location_reward_list,
        IEnumerable<AtgenBuildEventRewardEntityList> event_location_reward_entity_list
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.user_event_location_reward_list = user_event_location_reward_list;
        this.event_location_reward_entity_list = event_location_reward_entity_list;
    }

    public CombatEventReceiveEventLocationRewardData() { }
}

[MessagePackObject(true)]
public class CombatEventReceiveEventPointRewardData
{
    public IEnumerable<BuildEventRewardList> event_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> event_reward_entity_list { get; set; }

    public CombatEventReceiveEventPointRewardData(
        IEnumerable<BuildEventRewardList> event_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenBuildEventRewardEntityList> event_reward_entity_list
    )
    {
        this.event_reward_list = event_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.event_reward_entity_list = event_reward_entity_list;
    }

    public CombatEventReceiveEventPointRewardData() { }
}

[MessagePackObject(true)]
public class CraftAssembleData
{
    public UpdateDataList update_data_list { get; set; }
    public DeleteDataList delete_data_list { get; set; }
    public SettingSupport setting_support { get; set; }

    public CraftAssembleData(
        UpdateDataList update_data_list,
        DeleteDataList delete_data_list,
        SettingSupport setting_support
    )
    {
        this.update_data_list = update_data_list;
        this.delete_data_list = delete_data_list;
        this.setting_support = setting_support;
    }

    public CraftAssembleData() { }
}

[MessagePackObject(true)]
public class CraftCreateData
{
    public UpdateDataList update_data_list { get; set; }
    public DeleteDataList delete_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public CraftCreateData(
        UpdateDataList update_data_list,
        DeleteDataList delete_data_list,
        EntityResult entity_result
    )
    {
        this.update_data_list = update_data_list;
        this.delete_data_list = delete_data_list;
        this.entity_result = entity_result;
    }

    public CraftCreateData() { }
}

[MessagePackObject(true)]
public class CraftDisassembleData
{
    public UpdateDataList update_data_list { get; set; }
    public DeleteDataList delete_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public CraftDisassembleData(
        UpdateDataList update_data_list,
        DeleteDataList delete_data_list,
        EntityResult entity_result
    )
    {
        this.update_data_list = update_data_list;
        this.delete_data_list = delete_data_list;
        this.entity_result = entity_result;
    }

    public CraftDisassembleData() { }
}

[MessagePackObject(true)]
public class CraftResetNewData
{
    public UpdateDataList update_data_list { get; set; }

    public CraftResetNewData(UpdateDataList update_data_list)
    {
        this.update_data_list = update_data_list;
    }

    public CraftResetNewData() { }
}

[MessagePackObject(true)]
public class DeployGetDeployVersionData
{
    public string deploy_hash { get; set; }

    public DeployGetDeployVersionData(string deploy_hash)
    {
        this.deploy_hash = deploy_hash;
    }

    public DeployGetDeployVersionData() { }
}

[MessagePackObject(true)]
public class DmodeBuildupServitorPassiveData
{
    public IEnumerable<DmodeServitorPassiveList> dmode_servitor_passive_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DmodeBuildupServitorPassiveData(
        IEnumerable<DmodeServitorPassiveList> dmode_servitor_passive_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.dmode_servitor_passive_list = dmode_servitor_passive_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DmodeBuildupServitorPassiveData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonFinishData
{
    public int dmode_dungeon_state { get; set; }
    public DmodeIngameResult dmode_ingame_result { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DmodeDungeonFinishData(
        int dmode_dungeon_state,
        DmodeIngameResult dmode_ingame_result,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.dmode_dungeon_state = dmode_dungeon_state;
        this.dmode_ingame_result = dmode_ingame_result;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DmodeDungeonFinishData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonFloorData
{
    public int dmode_dungeon_state { get; set; }
    public DmodeFloorData dmode_floor_data { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DmodeDungeonFloorData(
        int dmode_dungeon_state,
        DmodeFloorData dmode_floor_data,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.dmode_dungeon_state = dmode_dungeon_state;
        this.dmode_floor_data = dmode_floor_data;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DmodeDungeonFloorData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonFloorSkipData
{
    public int dmode_dungeon_state { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DmodeDungeonFloorSkipData(
        int dmode_dungeon_state,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.dmode_dungeon_state = dmode_dungeon_state;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DmodeDungeonFloorSkipData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonRestartData
{
    public DmodeIngameData dmode_ingame_data { get; set; }
    public int dmode_dungeon_state { get; set; }

    public DmodeDungeonRestartData(DmodeIngameData dmode_ingame_data, int dmode_dungeon_state)
    {
        this.dmode_ingame_data = dmode_ingame_data;
        this.dmode_dungeon_state = dmode_dungeon_state;
    }

    public DmodeDungeonRestartData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonStartData
{
    public DmodeIngameData dmode_ingame_data { get; set; }
    public int dmode_dungeon_state { get; set; }

    public DmodeDungeonStartData(DmodeIngameData dmode_ingame_data, int dmode_dungeon_state)
    {
        this.dmode_ingame_data = dmode_ingame_data;
        this.dmode_dungeon_state = dmode_dungeon_state;
    }

    public DmodeDungeonStartData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonSystemHaltData
{
    public int dmode_dungeon_state { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DmodeDungeonSystemHaltData(
        int dmode_dungeon_state,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.dmode_dungeon_state = dmode_dungeon_state;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DmodeDungeonSystemHaltData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonUserHaltData
{
    public int dmode_dungeon_state { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DmodeDungeonUserHaltData(
        int dmode_dungeon_state,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.dmode_dungeon_state = dmode_dungeon_state;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DmodeDungeonUserHaltData() { }
}

[MessagePackObject(true)]
public class DmodeEntryData
{
    public DmodeInfo dmode_info { get; set; }
    public IEnumerable<DmodeCharaList> dmode_chara_list { get; set; }
    public IEnumerable<DmodeServitorPassiveList> dmode_servitor_passive_list { get; set; }
    public DmodeDungeonInfo dmode_dungeon_info { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DmodeEntryData(
        DmodeInfo dmode_info,
        IEnumerable<DmodeCharaList> dmode_chara_list,
        IEnumerable<DmodeServitorPassiveList> dmode_servitor_passive_list,
        DmodeDungeonInfo dmode_dungeon_info,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.dmode_info = dmode_info;
        this.dmode_chara_list = dmode_chara_list;
        this.dmode_servitor_passive_list = dmode_servitor_passive_list;
        this.dmode_dungeon_info = dmode_dungeon_info;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DmodeEntryData() { }
}

[MessagePackObject(true)]
public class DmodeExpeditionFinishData
{
    public DmodeIngameResult dmode_ingame_result { get; set; }
    public DmodeExpedition dmode_expedition { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DmodeExpeditionFinishData(
        DmodeIngameResult dmode_ingame_result,
        DmodeExpedition dmode_expedition,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.dmode_ingame_result = dmode_ingame_result;
        this.dmode_expedition = dmode_expedition;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DmodeExpeditionFinishData() { }
}

[MessagePackObject(true)]
public class DmodeExpeditionForceFinishData
{
    public DmodeIngameResult dmode_ingame_result { get; set; }
    public DmodeExpedition dmode_expedition { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DmodeExpeditionForceFinishData(
        DmodeIngameResult dmode_ingame_result,
        DmodeExpedition dmode_expedition,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.dmode_ingame_result = dmode_ingame_result;
        this.dmode_expedition = dmode_expedition;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DmodeExpeditionForceFinishData() { }
}

[MessagePackObject(true)]
public class DmodeExpeditionStartData
{
    public DmodeExpedition dmode_expedition { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DmodeExpeditionStartData(
        DmodeExpedition dmode_expedition,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.dmode_expedition = dmode_expedition;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DmodeExpeditionStartData() { }
}

[MessagePackObject(true)]
public class DmodeGetDataData
{
    public DmodeInfo dmode_info { get; set; }
    public IEnumerable<DmodeCharaList> dmode_chara_list { get; set; }
    public IEnumerable<DmodeServitorPassiveList> dmode_servitor_passive_list { get; set; }
    public DmodeDungeonInfo dmode_dungeon_info { get; set; }
    public IEnumerable<DmodeStoryList> dmode_story_list { get; set; }
    public DmodeExpedition dmode_expedition { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public int current_server_time { get; set; }

    public DmodeGetDataData(
        DmodeInfo dmode_info,
        IEnumerable<DmodeCharaList> dmode_chara_list,
        IEnumerable<DmodeServitorPassiveList> dmode_servitor_passive_list,
        DmodeDungeonInfo dmode_dungeon_info,
        IEnumerable<DmodeStoryList> dmode_story_list,
        DmodeExpedition dmode_expedition,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        int current_server_time
    )
    {
        this.dmode_info = dmode_info;
        this.dmode_chara_list = dmode_chara_list;
        this.dmode_servitor_passive_list = dmode_servitor_passive_list;
        this.dmode_dungeon_info = dmode_dungeon_info;
        this.dmode_story_list = dmode_story_list;
        this.dmode_expedition = dmode_expedition;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.current_server_time = current_server_time;
    }

    public DmodeGetDataData() { }
}

[MessagePackObject(true)]
public class DmodeReadStoryData
{
    public IEnumerable<AtgenBuildEventRewardEntityList> dmode_story_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list { get; set; }

    public DmodeReadStoryData(
        IEnumerable<AtgenBuildEventRewardEntityList> dmode_story_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list
    )
    {
        this.dmode_story_reward_list = dmode_story_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.duplicate_entity_list = duplicate_entity_list;
    }

    public DmodeReadStoryData() { }
}

[MessagePackObject(true)]
public class DragonBuildupData
{
    public UpdateDataList update_data_list { get; set; }
    public DeleteDataList delete_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DragonBuildupData(
        UpdateDataList update_data_list,
        DeleteDataList delete_data_list,
        EntityResult entity_result
    )
    {
        this.update_data_list = update_data_list;
        this.delete_data_list = delete_data_list;
        this.entity_result = entity_result;
    }

    public DragonBuildupData() { }
}

[MessagePackObject(true)]
public class DragonBuyGiftToSendMultipleData
{
    public IEnumerable<AtgenShopGiftList> shop_gift_list { get; set; }
    public IEnumerable<AtgenDragonGiftRewardList> dragon_gift_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public int dragon_contact_free_gift_count { get; set; }

    public DragonBuyGiftToSendMultipleData(
        IEnumerable<AtgenShopGiftList> shop_gift_list,
        IEnumerable<AtgenDragonGiftRewardList> dragon_gift_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        int dragon_contact_free_gift_count
    )
    {
        this.shop_gift_list = shop_gift_list;
        this.dragon_gift_reward_list = dragon_gift_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.dragon_contact_free_gift_count = dragon_contact_free_gift_count;
    }

    public DragonBuyGiftToSendMultipleData() { }
}

[MessagePackObject(true)]
public class DragonBuyGiftToSendData
{
    public IEnumerable<AtgenShopGiftList> shop_gift_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public int is_favorite { get; set; }
    public IEnumerable<DragonRewardEntityList> return_gift_list { get; set; }
    public IEnumerable<RewardReliabilityList> reward_reliability_list { get; set; }
    public int dragon_contact_free_gift_count { get; set; }

    public DragonBuyGiftToSendData(
        IEnumerable<AtgenShopGiftList> shop_gift_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        int is_favorite,
        IEnumerable<DragonRewardEntityList> return_gift_list,
        IEnumerable<RewardReliabilityList> reward_reliability_list,
        int dragon_contact_free_gift_count
    )
    {
        this.shop_gift_list = shop_gift_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.is_favorite = is_favorite;
        this.return_gift_list = return_gift_list;
        this.reward_reliability_list = reward_reliability_list;
        this.dragon_contact_free_gift_count = dragon_contact_free_gift_count;
    }

    public DragonBuyGiftToSendData() { }
}

[MessagePackObject(true)]
public class DragonGetContactDataData
{
    public IEnumerable<AtgenShopGiftList> shop_gift_list { get; set; }

    public DragonGetContactDataData(IEnumerable<AtgenShopGiftList> shop_gift_list)
    {
        this.shop_gift_list = shop_gift_list;
    }

    public DragonGetContactDataData() { }
}

[MessagePackObject(true)]
public class DragonLimitBreakData
{
    public UpdateDataList update_data_list { get; set; }
    public DeleteDataList delete_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DragonLimitBreakData(
        UpdateDataList update_data_list,
        DeleteDataList delete_data_list,
        EntityResult entity_result
    )
    {
        this.update_data_list = update_data_list;
        this.delete_data_list = delete_data_list;
        this.entity_result = entity_result;
    }

    public DragonLimitBreakData() { }
}

[MessagePackObject(true)]
public class DragonResetPlusCountData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DragonResetPlusCountData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DragonResetPlusCountData() { }
}

[MessagePackObject(true)]
public class DragonSellData
{
    public DeleteDataList delete_data_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DragonSellData(
        DeleteDataList delete_data_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.delete_data_list = delete_data_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DragonSellData() { }
}

[MessagePackObject(true)]
public class DragonSendGiftMultipleData
{
    public UpdateDataList update_data_list { get; set; }
    public int is_favorite { get; set; }
    public IEnumerable<DragonRewardEntityList> return_gift_list { get; set; }
    public IEnumerable<RewardReliabilityList> reward_reliability_list { get; set; }

    public DragonSendGiftMultipleData(
        UpdateDataList update_data_list,
        int is_favorite,
        IEnumerable<DragonRewardEntityList> return_gift_list,
        IEnumerable<RewardReliabilityList> reward_reliability_list
    )
    {
        this.update_data_list = update_data_list;
        this.is_favorite = is_favorite;
        this.return_gift_list = return_gift_list;
        this.reward_reliability_list = reward_reliability_list;
    }

    public DragonSendGiftMultipleData() { }
}

[MessagePackObject(true)]
public class DragonSendGiftData
{
    public UpdateDataList update_data_list { get; set; }
    public int is_favorite { get; set; }
    public IEnumerable<DragonRewardEntityList> return_gift_list { get; set; }
    public IEnumerable<RewardReliabilityList> reward_reliability_list { get; set; }

    public DragonSendGiftData(
        UpdateDataList update_data_list,
        int is_favorite,
        IEnumerable<DragonRewardEntityList> return_gift_list,
        IEnumerable<RewardReliabilityList> reward_reliability_list
    )
    {
        this.update_data_list = update_data_list;
        this.is_favorite = is_favorite;
        this.return_gift_list = return_gift_list;
        this.reward_reliability_list = reward_reliability_list;
    }

    public DragonSendGiftData() { }
}

[MessagePackObject(true)]
public class DragonSetLockData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public DragonSetLockData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public DragonSetLockData() { }
}

[MessagePackObject(true)]
public class DreamAdventureClearData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public int result { get; set; }

    public DreamAdventureClearData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        int result
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.result = result;
    }

    public DreamAdventureClearData() { }
}

[MessagePackObject(true)]
public class DreamAdventurePlayData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public int result { get; set; }

    public DreamAdventurePlayData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        int result
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.result = result;
    }

    public DreamAdventurePlayData() { }
}

[MessagePackObject(true)]
public class DungeonFailData
{
    public int result { get; set; }
    public IEnumerable<UserSupportList> fail_helper_list { get; set; }
    public IEnumerable<AtgenHelperDetailList> fail_helper_detail_list { get; set; }
    public AtgenFailQuestDetail fail_quest_detail { get; set; }

    public DungeonFailData(
        int result,
        IEnumerable<UserSupportList> fail_helper_list,
        IEnumerable<AtgenHelperDetailList> fail_helper_detail_list,
        AtgenFailQuestDetail fail_quest_detail
    )
    {
        this.result = result;
        this.fail_helper_list = fail_helper_list;
        this.fail_helper_detail_list = fail_helper_detail_list;
        this.fail_quest_detail = fail_quest_detail;
    }

    public DungeonFailData() { }
}

[MessagePackObject(true)]
public class DungeonGetAreaOddsData
{
    public OddsInfo odds_info { get; set; }

    public DungeonGetAreaOddsData(OddsInfo odds_info)
    {
        this.odds_info = odds_info;
    }

    public DungeonGetAreaOddsData() { }
}

[MessagePackObject(true)]
public class DungeonReceiveQuestBonusData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public AtgenReceiveQuestBonus receive_quest_bonus { get; set; }

    public DungeonReceiveQuestBonusData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        AtgenReceiveQuestBonus receive_quest_bonus
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.receive_quest_bonus = receive_quest_bonus;
    }

    public DungeonReceiveQuestBonusData() { }
}

[MessagePackObject(true)]
public class DungeonRecordRecordMultiData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IngameResultData ingame_result_data { get; set; }
    public TimeAttackRankingData time_attack_ranking_data { get; set; }
    public EventDamageRanking event_damage_ranking { get; set; }

    public DungeonRecordRecordMultiData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IngameResultData ingame_result_data,
        TimeAttackRankingData time_attack_ranking_data,
        EventDamageRanking event_damage_ranking
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.ingame_result_data = ingame_result_data;
        this.time_attack_ranking_data = time_attack_ranking_data;
        this.event_damage_ranking = event_damage_ranking;
    }

    public DungeonRecordRecordMultiData() { }
}

[MessagePackObject(true)]
public class DungeonRecordRecordData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IngameResultData ingame_result_data { get; set; }
    public TimeAttackRankingData time_attack_ranking_data { get; set; }
    public RepeatData repeat_data { get; set; }
    public EventDamageRanking event_damage_ranking { get; set; }

    public DungeonRecordRecordData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IngameResultData ingame_result_data,
        TimeAttackRankingData time_attack_ranking_data,
        RepeatData repeat_data,
        EventDamageRanking event_damage_ranking
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.ingame_result_data = ingame_result_data;
        this.time_attack_ranking_data = time_attack_ranking_data;
        this.repeat_data = repeat_data;
        this.event_damage_ranking = event_damage_ranking;
    }

    public DungeonRecordRecordData() { }
}

[MessagePackObject(true)]
public class DungeonRetryData
{
    public int continue_count { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public DungeonRetryData(int continue_count, UpdateDataList update_data_list)
    {
        this.continue_count = continue_count;
        this.update_data_list = update_data_list;
    }

    public DungeonRetryData() { }
}

[MessagePackObject(true)]
public class DungeonSkipStartAssignUnitData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IngameResultData ingame_result_data { get; set; }
    public TimeAttackRankingData time_attack_ranking_data { get; set; }

    public DungeonSkipStartAssignUnitData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IngameResultData ingame_result_data,
        TimeAttackRankingData time_attack_ranking_data
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.ingame_result_data = ingame_result_data;
        this.time_attack_ranking_data = time_attack_ranking_data;
    }

    public DungeonSkipStartAssignUnitData() { }
}

[MessagePackObject(true)]
public class DungeonSkipStartMultipleQuestAssignUnitData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IngameResultData ingame_result_data { get; set; }
    public TimeAttackRankingData time_attack_ranking_data { get; set; }

    public DungeonSkipStartMultipleQuestAssignUnitData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IngameResultData ingame_result_data,
        TimeAttackRankingData time_attack_ranking_data
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.ingame_result_data = ingame_result_data;
        this.time_attack_ranking_data = time_attack_ranking_data;
    }

    public DungeonSkipStartMultipleQuestAssignUnitData() { }
}

[MessagePackObject(true)]
public class DungeonSkipStartMultipleQuestData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IngameResultData ingame_result_data { get; set; }
    public TimeAttackRankingData time_attack_ranking_data { get; set; }

    public DungeonSkipStartMultipleQuestData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IngameResultData ingame_result_data,
        TimeAttackRankingData time_attack_ranking_data
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.ingame_result_data = ingame_result_data;
        this.time_attack_ranking_data = time_attack_ranking_data;
    }

    public DungeonSkipStartMultipleQuestData() { }
}

[MessagePackObject(true)]
public class DungeonSkipStartData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IngameResultData ingame_result_data { get; set; }
    public TimeAttackRankingData time_attack_ranking_data { get; set; }

    public DungeonSkipStartData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IngameResultData ingame_result_data,
        TimeAttackRankingData time_attack_ranking_data
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.ingame_result_data = ingame_result_data;
        this.time_attack_ranking_data = time_attack_ranking_data;
    }

    public DungeonSkipStartData() { }
}

[MessagePackObject(true)]
public class DungeonStartStartAssignUnitData
{
    public IngameData ingame_data { get; set; }
    public IngameQuestData ingame_quest_data { get; set; }
    public OddsInfo odds_info { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public DungeonStartStartAssignUnitData(
        IngameData ingame_data,
        IngameQuestData ingame_quest_data,
        OddsInfo odds_info,
        UpdateDataList update_data_list
    )
    {
        this.ingame_data = ingame_data;
        this.ingame_quest_data = ingame_quest_data;
        this.odds_info = odds_info;
        this.update_data_list = update_data_list;
    }

    public DungeonStartStartAssignUnitData() { }
}

[MessagePackObject(true)]
public class DungeonStartStartMultiAssignUnitData
{
    public IngameData ingame_data { get; set; }
    public IngameQuestData ingame_quest_data { get; set; }
    public OddsInfo odds_info { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public DungeonStartStartMultiAssignUnitData(
        IngameData ingame_data,
        IngameQuestData ingame_quest_data,
        OddsInfo odds_info,
        UpdateDataList update_data_list
    )
    {
        this.ingame_data = ingame_data;
        this.ingame_quest_data = ingame_quest_data;
        this.odds_info = odds_info;
        this.update_data_list = update_data_list;
    }

    public DungeonStartStartMultiAssignUnitData() { }
}

[MessagePackObject(true)]
public class DungeonStartStartMultiData
{
    public IngameData ingame_data { get; set; }
    public IngameQuestData ingame_quest_data { get; set; }
    public OddsInfo odds_info { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public DungeonStartStartMultiData(
        IngameData ingame_data,
        IngameQuestData ingame_quest_data,
        OddsInfo odds_info,
        UpdateDataList update_data_list
    )
    {
        this.ingame_data = ingame_data;
        this.ingame_quest_data = ingame_quest_data;
        this.odds_info = odds_info;
        this.update_data_list = update_data_list;
    }

    public DungeonStartStartMultiData() { }
}

[MessagePackObject(true)]
public class DungeonStartStartData
{
    public IngameData ingame_data { get; set; }
    public IngameQuestData ingame_quest_data { get; set; }
    public OddsInfo odds_info { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public DungeonStartStartData(
        IngameData ingame_data,
        IngameQuestData ingame_quest_data,
        OddsInfo odds_info,
        UpdateDataList update_data_list
    )
    {
        this.ingame_data = ingame_data;
        this.ingame_quest_data = ingame_quest_data;
        this.odds_info = odds_info;
        this.update_data_list = update_data_list;
    }

    public DungeonStartStartData() { }
}

[MessagePackObject(true)]
public class EarnEventEntryData
{
    public EarnEventUserList earn_event_user_data { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public EarnEventEntryData(
        EarnEventUserList earn_event_user_data,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.earn_event_user_data = earn_event_user_data;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public EarnEventEntryData() { }
}

[MessagePackObject(true)]
public class EarnEventGetEventDataData
{
    public EarnEventUserList earn_event_user_data { get; set; }
    public IEnumerable<EventTradeList> event_trade_list { get; set; }
    public IEnumerable<BuildEventRewardList> event_reward_list { get; set; }

    public EarnEventGetEventDataData(
        EarnEventUserList earn_event_user_data,
        IEnumerable<EventTradeList> event_trade_list,
        IEnumerable<BuildEventRewardList> event_reward_list
    )
    {
        this.earn_event_user_data = earn_event_user_data;
        this.event_trade_list = event_trade_list;
        this.event_reward_list = event_reward_list;
    }

    public EarnEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class EarnEventReceiveEventPointRewardData
{
    public IEnumerable<BuildEventRewardList> event_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> event_reward_entity_list { get; set; }

    public EarnEventReceiveEventPointRewardData(
        IEnumerable<BuildEventRewardList> event_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenBuildEventRewardEntityList> event_reward_entity_list
    )
    {
        this.event_reward_list = event_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.event_reward_entity_list = event_reward_entity_list;
    }

    public EarnEventReceiveEventPointRewardData() { }
}

[MessagePackObject(true)]
public class EmblemGetListData
{
    public IEnumerable<EmblemList> emblem_list { get; set; }

    public EmblemGetListData(IEnumerable<EmblemList> emblem_list)
    {
        this.emblem_list = emblem_list;
    }

    public EmblemGetListData() { }
}

[MessagePackObject(true)]
public class EmblemSetData
{
    public int result { get; set; }

    public EmblemSetData(int result)
    {
        this.result = result;
    }

    public EmblemSetData() { }
}

[MessagePackObject(true)]
public class EulaAgreeAgreeData
{
    public AtgenVersionHash version_hash { get; set; }
    public int is_optin { get; set; }

    public EulaAgreeAgreeData(AtgenVersionHash version_hash, int is_optin)
    {
        this.version_hash = version_hash;
        this.is_optin = is_optin;
    }

    public EulaAgreeAgreeData() { }
}

[MessagePackObject(true)]
public class EulaGetVersionListData
{
    public IEnumerable<AtgenVersionHash> version_hash_list { get; set; }

    public EulaGetVersionListData(IEnumerable<AtgenVersionHash> version_hash_list)
    {
        this.version_hash_list = version_hash_list;
    }

    public EulaGetVersionListData() { }
}

[MessagePackObject(true)]
public class EulaGetVersionData
{
    public AtgenVersionHash version_hash { get; set; }
    public bool is_required_agree { get; set; }
    public int agreement_status { get; set; }

    public EulaGetVersionData(
        AtgenVersionHash version_hash,
        bool is_required_agree,
        int agreement_status
    )
    {
        this.version_hash = version_hash;
        this.is_required_agree = is_required_agree;
        this.agreement_status = agreement_status;
    }

    public EulaGetVersionData() { }
}

[MessagePackObject(true)]
public class EventDamageGetTotalDamageHistoryData
{
    public IEnumerable<AtgenEventDamageHistoryList> event_damage_history_list { get; set; }

    public EventDamageGetTotalDamageHistoryData(
        IEnumerable<AtgenEventDamageHistoryList> event_damage_history_list
    )
    {
        this.event_damage_history_list = event_damage_history_list;
    }

    public EventDamageGetTotalDamageHistoryData() { }
}

[MessagePackObject(true)]
public class EventDamageReceiveDamageRewardData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenEventDamageRewardList> event_damage_reward_list { get; set; }

    public EventDamageReceiveDamageRewardData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenEventDamageRewardList> event_damage_reward_list
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.event_damage_reward_list = event_damage_reward_list;
    }

    public EventDamageReceiveDamageRewardData() { }
}

[MessagePackObject(true)]
public class EventStoryReadData
{
    public IEnumerable<AtgenBuildEventRewardEntityList> event_story_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public EventStoryReadData(
        IEnumerable<AtgenBuildEventRewardEntityList> event_story_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.event_story_reward_list = event_story_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public EventStoryReadData() { }
}

[MessagePackObject(true)]
public class EventSummonExecData
{
    public AtgenBoxSummonResult box_summon_result { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public EventSummonExecData(
        AtgenBoxSummonResult box_summon_result,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.box_summon_result = box_summon_result;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public EventSummonExecData() { }
}

[MessagePackObject(true)]
public class EventSummonGetDataData
{
    public AtgenBoxSummonData box_summon_data { get; set; }

    public EventSummonGetDataData(AtgenBoxSummonData box_summon_data)
    {
        this.box_summon_data = box_summon_data;
    }

    public EventSummonGetDataData() { }
}

[MessagePackObject(true)]
public class EventSummonResetData
{
    public AtgenBoxSummonData box_summon_data { get; set; }

    public EventSummonResetData(AtgenBoxSummonData box_summon_data)
    {
        this.box_summon_data = box_summon_data;
    }

    public EventSummonResetData() { }
}

[MessagePackObject(true)]
public class EventTradeGetListData
{
    public IEnumerable<AtgenUserEventTradeList> user_event_trade_list { get; set; }
    public IEnumerable<EventTradeList> event_trade_list { get; set; }
    public IEnumerable<MaterialList> material_list { get; set; }
    public UserEventItemData user_event_item_data { get; set; }

    public EventTradeGetListData(
        IEnumerable<AtgenUserEventTradeList> user_event_trade_list,
        IEnumerable<EventTradeList> event_trade_list,
        IEnumerable<MaterialList> material_list,
        UserEventItemData user_event_item_data
    )
    {
        this.user_event_trade_list = user_event_trade_list;
        this.event_trade_list = event_trade_list;
        this.material_list = material_list;
        this.user_event_item_data = user_event_item_data;
    }

    public EventTradeGetListData() { }
}

[MessagePackObject(true)]
public class EventTradeTradeData
{
    public IEnumerable<AtgenUserEventTradeList> user_event_trade_list { get; set; }
    public IEnumerable<EventTradeList> event_trade_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<MaterialList> material_list { get; set; }
    public UserEventItemData user_event_item_data { get; set; }

    public EventTradeTradeData(
        IEnumerable<AtgenUserEventTradeList> user_event_trade_list,
        IEnumerable<EventTradeList> event_trade_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<MaterialList> material_list,
        UserEventItemData user_event_item_data
    )
    {
        this.user_event_trade_list = user_event_trade_list;
        this.event_trade_list = event_trade_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.material_list = material_list;
        this.user_event_item_data = user_event_item_data;
    }

    public EventTradeTradeData() { }
}

[MessagePackObject(true)]
public class ExchangeGetUnitListData
{
    public IEnumerable<AtgenDuplicateEntityList> select_unit_list { get; set; }

    public ExchangeGetUnitListData(IEnumerable<AtgenDuplicateEntityList> select_unit_list)
    {
        this.select_unit_list = select_unit_list;
    }

    public ExchangeGetUnitListData() { }
}

[MessagePackObject(true)]
public class ExchangeSelectUnitData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public ExchangeSelectUnitData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public ExchangeSelectUnitData() { }
}

[MessagePackObject(true)]
public class ExHunterEventEntryData
{
    public ExHunterEventUserList ex_hunter_event_user_data { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public ExHunterEventEntryData(
        ExHunterEventUserList ex_hunter_event_user_data,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.ex_hunter_event_user_data = ex_hunter_event_user_data;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public ExHunterEventEntryData() { }
}

[MessagePackObject(true)]
public class ExHunterEventGetEventDataData
{
    public ExHunterEventUserList ex_hunter_event_user_data { get; set; }
    public IEnumerable<BuildEventRewardList> ex_hunter_event_reward_list { get; set; }
    public IEnumerable<EventTradeList> event_trade_list { get; set; }
    public IEnumerable<EventPassiveList> event_passive_list { get; set; }

    public ExHunterEventGetEventDataData(
        ExHunterEventUserList ex_hunter_event_user_data,
        IEnumerable<BuildEventRewardList> ex_hunter_event_reward_list,
        IEnumerable<EventTradeList> event_trade_list,
        IEnumerable<EventPassiveList> event_passive_list
    )
    {
        this.ex_hunter_event_user_data = ex_hunter_event_user_data;
        this.ex_hunter_event_reward_list = ex_hunter_event_reward_list;
        this.event_trade_list = event_trade_list;
        this.event_passive_list = event_passive_list;
    }

    public ExHunterEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class ExHunterEventReceiveExHunterPointRewardData
{
    public IEnumerable<BuildEventRewardList> ex_hunter_event_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public ExHunterEventReceiveExHunterPointRewardData(
        IEnumerable<BuildEventRewardList> ex_hunter_event_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.ex_hunter_event_reward_list = ex_hunter_event_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public ExHunterEventReceiveExHunterPointRewardData() { }
}

[MessagePackObject(true)]
public class ExRushEventEntryData
{
    public ExRushEventUserList ex_rush_event_user_data { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public ExRushEventEntryData(
        ExRushEventUserList ex_rush_event_user_data,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.ex_rush_event_user_data = ex_rush_event_user_data;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public ExRushEventEntryData() { }
}

[MessagePackObject(true)]
public class ExRushEventGetEventDataData
{
    public ExRushEventUserList ex_rush_event_user_data { get; set; }
    public IEnumerable<CharaFriendshipList> chara_friendship_list { get; set; }
    public IEnumerable<EventTradeList> event_trade_list { get; set; }

    public ExRushEventGetEventDataData(
        ExRushEventUserList ex_rush_event_user_data,
        IEnumerable<CharaFriendshipList> chara_friendship_list,
        IEnumerable<EventTradeList> event_trade_list
    )
    {
        this.ex_rush_event_user_data = ex_rush_event_user_data;
        this.chara_friendship_list = chara_friendship_list;
        this.event_trade_list = event_trade_list;
    }

    public ExRushEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class FortAddCarpenterData
{
    public int result { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public FortDetail fort_detail { get; set; }

    public FortAddCarpenterData(int result, UpdateDataList update_data_list, FortDetail fort_detail)
    {
        this.result = result;
        this.update_data_list = update_data_list;
        this.fort_detail = fort_detail;
    }

    public FortAddCarpenterData() { }
}

[MessagePackObject(true)]
public class FortBuildAtOnceData
{
    public int result { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public ulong build_id { get; set; }
    public FortDetail fort_detail { get; set; }
    public FortBonusList fort_bonus_list { get; set; }
    public AtgenProductionRp production_rp { get; set; }
    public AtgenProductionRp production_df { get; set; }
    public AtgenProductionRp production_st { get; set; }

    public FortBuildAtOnceData(
        int result,
        UpdateDataList update_data_list,
        ulong build_id,
        FortDetail fort_detail,
        FortBonusList fort_bonus_list,
        AtgenProductionRp production_rp,
        AtgenProductionRp production_df,
        AtgenProductionRp production_st
    )
    {
        this.result = result;
        this.update_data_list = update_data_list;
        this.build_id = build_id;
        this.fort_detail = fort_detail;
        this.fort_bonus_list = fort_bonus_list;
        this.production_rp = production_rp;
        this.production_df = production_df;
        this.production_st = production_st;
    }

    public FortBuildAtOnceData() { }
}

[MessagePackObject(true)]
public class FortBuildCancelData
{
    public int result { get; set; }
    public ulong build_id { get; set; }
    public FortDetail fort_detail { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public FortBuildCancelData(
        int result,
        ulong build_id,
        FortDetail fort_detail,
        UpdateDataList update_data_list
    )
    {
        this.result = result;
        this.build_id = build_id;
        this.fort_detail = fort_detail;
        this.update_data_list = update_data_list;
    }

    public FortBuildCancelData() { }
}

[MessagePackObject(true)]
public class FortBuildEndData
{
    public int result { get; set; }
    public ulong build_id { get; set; }
    public FortBonusList fort_bonus_list { get; set; }
    public FortDetail fort_detail { get; set; }
    public AtgenProductionRp production_rp { get; set; }
    public AtgenProductionRp production_df { get; set; }
    public AtgenProductionRp production_st { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public FortBuildEndData(
        int result,
        ulong build_id,
        FortBonusList fort_bonus_list,
        FortDetail fort_detail,
        AtgenProductionRp production_rp,
        AtgenProductionRp production_df,
        AtgenProductionRp production_st,
        UpdateDataList update_data_list
    )
    {
        this.result = result;
        this.build_id = build_id;
        this.fort_bonus_list = fort_bonus_list;
        this.fort_detail = fort_detail;
        this.production_rp = production_rp;
        this.production_df = production_df;
        this.production_st = production_st;
        this.update_data_list = update_data_list;
    }

    public FortBuildEndData() { }
}

[MessagePackObject(true)]
public class FortBuildStartData
{
    public int result { get; set; }
    public ulong build_id { get; set; }
    public int build_start_date { get; set; }
    public int build_end_date { get; set; }
    public int remain_time { get; set; }
    public FortDetail fort_detail { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public FortBuildStartData(
        int result,
        ulong build_id,
        int build_start_date,
        int build_end_date,
        int remain_time,
        FortDetail fort_detail,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.result = result;
        this.build_id = build_id;
        this.build_start_date = build_start_date;
        this.build_end_date = build_end_date;
        this.remain_time = remain_time;
        this.fort_detail = fort_detail;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public FortBuildStartData() { }
}

[MessagePackObject(true)]
public class FortGetDataData
{
    public FortDetail fort_detail { get; set; }
    public IEnumerable<BuildList> build_list { get; set; }
    public FortBonusList fort_bonus_list { get; set; }
    public AtgenProductionRp production_rp { get; set; }
    public AtgenProductionRp production_df { get; set; }
    public AtgenProductionRp production_st { get; set; }
    public int dragon_contact_free_gift_count { get; set; }
    public int current_server_time { get; set; }

    public FortGetDataData(
        FortDetail fort_detail,
        IEnumerable<BuildList> build_list,
        FortBonusList fort_bonus_list,
        AtgenProductionRp production_rp,
        AtgenProductionRp production_df,
        AtgenProductionRp production_st,
        int dragon_contact_free_gift_count,
        int current_server_time
    )
    {
        this.fort_detail = fort_detail;
        this.build_list = build_list;
        this.fort_bonus_list = fort_bonus_list;
        this.production_rp = production_rp;
        this.production_df = production_df;
        this.production_st = production_st;
        this.dragon_contact_free_gift_count = dragon_contact_free_gift_count;
        this.current_server_time = current_server_time;
    }

    public FortGetDataData() { }
}

[MessagePackObject(true)]
public class FortGetMultiIncomeData
{
    public int result { get; set; }
    public IEnumerable<AtgenHarvestBuildList> harvest_build_list { get; set; }
    public IEnumerable<AtgenAddCoinList> add_coin_list { get; set; }
    public IEnumerable<AtgenAddStaminaList> add_stamina_list { get; set; }
    public int is_over_coin { get; set; }
    public int is_over_material { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public FortGetMultiIncomeData(
        int result,
        IEnumerable<AtgenHarvestBuildList> harvest_build_list,
        IEnumerable<AtgenAddCoinList> add_coin_list,
        IEnumerable<AtgenAddStaminaList> add_stamina_list,
        int is_over_coin,
        int is_over_material,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.result = result;
        this.harvest_build_list = harvest_build_list;
        this.add_coin_list = add_coin_list;
        this.add_stamina_list = add_stamina_list;
        this.is_over_coin = is_over_coin;
        this.is_over_material = is_over_material;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public FortGetMultiIncomeData() { }
}

[MessagePackObject(true)]
public class FortLevelupAtOnceData
{
    public int result { get; set; }
    public ulong build_id { get; set; }
    public FortDetail fort_detail { get; set; }
    public FortBonusList fort_bonus_list { get; set; }
    public int current_fort_level { get; set; }
    public int current_fort_craft_level { get; set; }
    public AtgenProductionRp production_rp { get; set; }
    public AtgenProductionRp production_df { get; set; }
    public AtgenProductionRp production_st { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public FortLevelupAtOnceData(
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
    )
    {
        this.result = result;
        this.build_id = build_id;
        this.fort_detail = fort_detail;
        this.fort_bonus_list = fort_bonus_list;
        this.current_fort_level = current_fort_level;
        this.current_fort_craft_level = current_fort_craft_level;
        this.production_rp = production_rp;
        this.production_df = production_df;
        this.production_st = production_st;
        this.update_data_list = update_data_list;
    }

    public FortLevelupAtOnceData() { }
}

[MessagePackObject(true)]
public class FortLevelupCancelData
{
    public int result { get; set; }
    public ulong build_id { get; set; }
    public FortDetail fort_detail { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public FortLevelupCancelData(
        int result,
        ulong build_id,
        FortDetail fort_detail,
        UpdateDataList update_data_list
    )
    {
        this.result = result;
        this.build_id = build_id;
        this.fort_detail = fort_detail;
        this.update_data_list = update_data_list;
    }

    public FortLevelupCancelData() { }
}

[MessagePackObject(true)]
public class FortLevelupEndData
{
    public int result { get; set; }
    public ulong build_id { get; set; }
    public FortDetail fort_detail { get; set; }
    public FortBonusList fort_bonus_list { get; set; }
    public int current_fort_level { get; set; }
    public int current_fort_craft_level { get; set; }
    public AtgenProductionRp production_rp { get; set; }
    public AtgenProductionRp production_df { get; set; }
    public AtgenProductionRp production_st { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public FortLevelupEndData(
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
    )
    {
        this.result = result;
        this.build_id = build_id;
        this.fort_detail = fort_detail;
        this.fort_bonus_list = fort_bonus_list;
        this.current_fort_level = current_fort_level;
        this.current_fort_craft_level = current_fort_craft_level;
        this.production_rp = production_rp;
        this.production_df = production_df;
        this.production_st = production_st;
        this.update_data_list = update_data_list;
    }

    public FortLevelupEndData() { }
}

[MessagePackObject(true)]
public class FortLevelupStartData
{
    public int result { get; set; }
    public int levelup_start_date { get; set; }
    public int levelup_end_date { get; set; }
    public int remain_time { get; set; }
    public ulong build_id { get; set; }
    public FortDetail fort_detail { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public FortLevelupStartData(
        int result,
        int levelup_start_date,
        int levelup_end_date,
        int remain_time,
        ulong build_id,
        FortDetail fort_detail,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.result = result;
        this.levelup_start_date = levelup_start_date;
        this.levelup_end_date = levelup_end_date;
        this.remain_time = remain_time;
        this.build_id = build_id;
        this.fort_detail = fort_detail;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public FortLevelupStartData() { }
}

[MessagePackObject(true)]
public class FortMoveData
{
    public int result { get; set; }
    public ulong build_id { get; set; }
    public FortBonusList fort_bonus_list { get; set; }
    public AtgenProductionRp production_rp { get; set; }
    public AtgenProductionRp production_df { get; set; }
    public AtgenProductionRp production_st { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public FortMoveData(
        int result,
        ulong build_id,
        FortBonusList fort_bonus_list,
        AtgenProductionRp production_rp,
        AtgenProductionRp production_df,
        AtgenProductionRp production_st,
        UpdateDataList update_data_list
    )
    {
        this.result = result;
        this.build_id = build_id;
        this.fort_bonus_list = fort_bonus_list;
        this.production_rp = production_rp;
        this.production_df = production_df;
        this.production_st = production_st;
        this.update_data_list = update_data_list;
    }

    public FortMoveData() { }
}

[MessagePackObject(true)]
public class FortSetNewFortPlantData
{
    public int result { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public FortSetNewFortPlantData(int result, UpdateDataList update_data_list)
    {
        this.result = result;
        this.update_data_list = update_data_list;
    }

    public FortSetNewFortPlantData() { }
}

[MessagePackObject(true)]
public class FriendAllReplyDenyData
{
    public int result { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public FriendAllReplyDenyData(
        int result,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.result = result;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public FriendAllReplyDenyData() { }
}

[MessagePackObject(true)]
public class FriendApplyListData
{
    public int result { get; set; }
    public IEnumerable<UserSupportList> friend_apply { get; set; }
    public IEnumerable<ulong> new_apply_viewer_id_list { get; set; }

    public FriendApplyListData(
        int result,
        IEnumerable<UserSupportList> friend_apply,
        IEnumerable<ulong> new_apply_viewer_id_list
    )
    {
        this.result = result;
        this.friend_apply = friend_apply;
        this.new_apply_viewer_id_list = new_apply_viewer_id_list;
    }

    public FriendApplyListData() { }
}

[MessagePackObject(true)]
public class FriendAutoSearchData
{
    public int result { get; set; }
    public IEnumerable<UserSupportList> search_list { get; set; }

    public FriendAutoSearchData(int result, IEnumerable<UserSupportList> search_list)
    {
        this.result = result;
        this.search_list = search_list;
    }

    public FriendAutoSearchData() { }
}

[MessagePackObject(true)]
public class FriendDeleteData
{
    public int result { get; set; }

    public FriendDeleteData(int result)
    {
        this.result = result;
    }

    public FriendDeleteData() { }
}

[MessagePackObject(true)]
public class FriendFriendIndexData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public int friend_count { get; set; }

    public FriendFriendIndexData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        int friend_count
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.friend_count = friend_count;
    }

    public FriendFriendIndexData() { }
}

[MessagePackObject(true)]
public class FriendFriendListData
{
    public int result { get; set; }
    public IEnumerable<UserSupportList> friend_list { get; set; }
    public IEnumerable<ulong> new_friend_viewer_id_list { get; set; }

    public FriendFriendListData(
        int result,
        IEnumerable<UserSupportList> friend_list,
        IEnumerable<ulong> new_friend_viewer_id_list
    )
    {
        this.result = result;
        this.friend_list = friend_list;
        this.new_friend_viewer_id_list = new_friend_viewer_id_list;
    }

    public FriendFriendListData() { }
}

[MessagePackObject(true)]
public class FriendGetSupportCharaDetailData
{
    public AtgenSupportUserDataDetail support_user_data_detail { get; set; }

    public FriendGetSupportCharaDetailData(AtgenSupportUserDataDetail support_user_data_detail)
    {
        this.support_user_data_detail = support_user_data_detail;
    }

    public FriendGetSupportCharaDetailData() { }
}

[MessagePackObject(true)]
public class FriendGetSupportCharaData
{
    public int result { get; set; }
    public SettingSupport setting_support { get; set; }

    public FriendGetSupportCharaData(int result, SettingSupport setting_support)
    {
        this.result = result;
        this.setting_support = setting_support;
    }

    public FriendGetSupportCharaData() { }
}

[MessagePackObject(true)]
public class FriendIdSearchData
{
    public int result { get; set; }
    public UserSupportList search_user { get; set; }

    public FriendIdSearchData(int result, UserSupportList search_user)
    {
        this.result = result;
        this.search_user = search_user;
    }

    public FriendIdSearchData() { }
}

[MessagePackObject(true)]
public class FriendReplyData
{
    public int result { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public FriendReplyData(int result, UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.result = result;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public FriendReplyData() { }
}

[MessagePackObject(true)]
public class FriendRequestCancelData
{
    public int result { get; set; }

    public FriendRequestCancelData(int result)
    {
        this.result = result;
    }

    public FriendRequestCancelData() { }
}

[MessagePackObject(true)]
public class FriendRequestListData
{
    public int result { get; set; }
    public IEnumerable<UserSupportList> request_list { get; set; }

    public FriendRequestListData(int result, IEnumerable<UserSupportList> request_list)
    {
        this.result = result;
        this.request_list = request_list;
    }

    public FriendRequestListData() { }
}

[MessagePackObject(true)]
public class FriendRequestData
{
    public int result { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public FriendRequestData(int result, UpdateDataList update_data_list)
    {
        this.result = result;
        this.update_data_list = update_data_list;
    }

    public FriendRequestData() { }
}

[MessagePackObject(true)]
public class FriendSetSupportCharaData
{
    public int result { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public SettingSupport setting_support { get; set; }

    public FriendSetSupportCharaData(
        int result,
        UpdateDataList update_data_list,
        SettingSupport setting_support
    )
    {
        this.result = result;
        this.update_data_list = update_data_list;
        this.setting_support = setting_support;
    }

    public FriendSetSupportCharaData() { }
}

[MessagePackObject(true)]
public class GuildChatGetNewMessageListData
{
    public IEnumerable<GuildChatMessageList> guild_chat_message_list { get; set; }
    public int polling_interval { get; set; }

    public GuildChatGetNewMessageListData(
        IEnumerable<GuildChatMessageList> guild_chat_message_list,
        int polling_interval
    )
    {
        this.guild_chat_message_list = guild_chat_message_list;
        this.polling_interval = polling_interval;
    }

    public GuildChatGetNewMessageListData() { }
}

[MessagePackObject(true)]
public class GuildChatGetOldMessageListData
{
    public IEnumerable<GuildChatMessageList> guild_chat_message_list { get; set; }

    public GuildChatGetOldMessageListData(IEnumerable<GuildChatMessageList> guild_chat_message_list)
    {
        this.guild_chat_message_list = guild_chat_message_list;
    }

    public GuildChatGetOldMessageListData() { }
}

[MessagePackObject(true)]
public class GuildChatPostMessageStampData
{
    public IEnumerable<GuildChatMessageList> guild_chat_message_list { get; set; }
    public int polling_interval { get; set; }

    public GuildChatPostMessageStampData(
        IEnumerable<GuildChatMessageList> guild_chat_message_list,
        int polling_interval
    )
    {
        this.guild_chat_message_list = guild_chat_message_list;
        this.polling_interval = polling_interval;
    }

    public GuildChatPostMessageStampData() { }
}

[MessagePackObject(true)]
public class GuildChatPostMessageTextData
{
    public IEnumerable<GuildChatMessageList> guild_chat_message_list { get; set; }
    public int polling_interval { get; set; }

    public GuildChatPostMessageTextData(
        IEnumerable<GuildChatMessageList> guild_chat_message_list,
        int polling_interval
    )
    {
        this.guild_chat_message_list = guild_chat_message_list;
        this.polling_interval = polling_interval;
    }

    public GuildChatPostMessageTextData() { }
}

[MessagePackObject(true)]
public class GuildChatPostReportData
{
    public int result { get; set; }

    public GuildChatPostReportData(int result)
    {
        this.result = result;
    }

    public GuildChatPostReportData() { }
}

[MessagePackObject(true)]
public class GuildDisbandData
{
    public UpdateDataList update_data_list { get; set; }

    public GuildDisbandData(UpdateDataList update_data_list)
    {
        this.update_data_list = update_data_list;
    }

    public GuildDisbandData() { }
}

[MessagePackObject(true)]
public class GuildDropMemberData
{
    public IEnumerable<GuildMemberList> guild_member_list { get; set; }

    public GuildDropMemberData(IEnumerable<GuildMemberList> guild_member_list)
    {
        this.guild_member_list = guild_member_list;
    }

    public GuildDropMemberData() { }
}

[MessagePackObject(true)]
public class GuildEstablishData
{
    public UpdateDataList update_data_list { get; set; }
    public IEnumerable<GuildMemberList> guild_member_list { get; set; }

    public GuildEstablishData(
        UpdateDataList update_data_list,
        IEnumerable<GuildMemberList> guild_member_list
    )
    {
        this.update_data_list = update_data_list;
        this.guild_member_list = guild_member_list;
    }

    public GuildEstablishData() { }
}

[MessagePackObject(true)]
public class GuildGetGuildApplyDataData
{
    public IEnumerable<GuildApplyList> guild_apply_list { get; set; }

    public GuildGetGuildApplyDataData(IEnumerable<GuildApplyList> guild_apply_list)
    {
        this.guild_apply_list = guild_apply_list;
    }

    public GuildGetGuildApplyDataData() { }
}

[MessagePackObject(true)]
public class GuildGetGuildMemberDataData
{
    public IEnumerable<GuildMemberList> guild_member_list { get; set; }

    public GuildGetGuildMemberDataData(IEnumerable<GuildMemberList> guild_member_list)
    {
        this.guild_member_list = guild_member_list;
    }

    public GuildGetGuildMemberDataData() { }
}

[MessagePackObject(true)]
public class GuildIndexData
{
    public UpdateDataList update_data_list { get; set; }
    public IEnumerable<GuildMemberList> guild_member_list { get; set; }
    public IEnumerable<GuildChatMessageList> guild_chat_message_list { get; set; }
    public IEnumerable<GuildApplyList> guild_apply_list { get; set; }
    public int is_update_guild_position_type { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> attend_bonus_list { get; set; }
    public int polling_interval { get; set; }
    public int current_server_time { get; set; }
    public IEnumerable<GuildInviteSendList> guild_invite_send_list { get; set; }
    public int guild_invite_receive_count { get; set; }

    public GuildIndexData(
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
    )
    {
        this.update_data_list = update_data_list;
        this.guild_member_list = guild_member_list;
        this.guild_chat_message_list = guild_chat_message_list;
        this.guild_apply_list = guild_apply_list;
        this.is_update_guild_position_type = is_update_guild_position_type;
        this.attend_bonus_list = attend_bonus_list;
        this.polling_interval = polling_interval;
        this.current_server_time = current_server_time;
        this.guild_invite_send_list = guild_invite_send_list;
        this.guild_invite_receive_count = guild_invite_receive_count;
    }

    public GuildIndexData() { }
}

[MessagePackObject(true)]
public class GuildInviteGetGuildInviteReceiveDataData
{
    public IEnumerable<GuildInviteReceiveList> guild_invite_receive_list { get; set; }

    public GuildInviteGetGuildInviteReceiveDataData(
        IEnumerable<GuildInviteReceiveList> guild_invite_receive_list
    )
    {
        this.guild_invite_receive_list = guild_invite_receive_list;
    }

    public GuildInviteGetGuildInviteReceiveDataData() { }
}

[MessagePackObject(true)]
public class GuildInviteGetGuildInviteSendDataData
{
    public IEnumerable<GuildInviteSendList> guild_invite_send_list { get; set; }

    public GuildInviteGetGuildInviteSendDataData(
        IEnumerable<GuildInviteSendList> guild_invite_send_list
    )
    {
        this.guild_invite_send_list = guild_invite_send_list;
    }

    public GuildInviteGetGuildInviteSendDataData() { }
}

[MessagePackObject(true)]
public class GuildInviteInviteCancelData
{
    public IEnumerable<GuildInviteSendList> guild_invite_send_list { get; set; }

    public GuildInviteInviteCancelData(IEnumerable<GuildInviteSendList> guild_invite_send_list)
    {
        this.guild_invite_send_list = guild_invite_send_list;
    }

    public GuildInviteInviteCancelData() { }
}

[MessagePackObject(true)]
public class GuildInviteInviteReplyAllDenyData
{
    public UpdateDataList update_data_list { get; set; }
    public IEnumerable<GuildInviteReceiveList> guild_invite_receive_list { get; set; }

    public GuildInviteInviteReplyAllDenyData(
        UpdateDataList update_data_list,
        IEnumerable<GuildInviteReceiveList> guild_invite_receive_list
    )
    {
        this.update_data_list = update_data_list;
        this.guild_invite_receive_list = guild_invite_receive_list;
    }

    public GuildInviteInviteReplyAllDenyData() { }
}

[MessagePackObject(true)]
public class GuildInviteInviteReplyData
{
    public UpdateDataList update_data_list { get; set; }
    public IEnumerable<GuildInviteReceiveList> guild_invite_receive_list { get; set; }

    public GuildInviteInviteReplyData(
        UpdateDataList update_data_list,
        IEnumerable<GuildInviteReceiveList> guild_invite_receive_list
    )
    {
        this.update_data_list = update_data_list;
        this.guild_invite_receive_list = guild_invite_receive_list;
    }

    public GuildInviteInviteReplyData() { }
}

[MessagePackObject(true)]
public class GuildInviteInviteSendData
{
    public IEnumerable<GuildInviteSendList> guild_invite_send_list { get; set; }

    public GuildInviteInviteSendData(IEnumerable<GuildInviteSendList> guild_invite_send_list)
    {
        this.guild_invite_send_list = guild_invite_send_list;
    }

    public GuildInviteInviteSendData() { }
}

[MessagePackObject(true)]
public class GuildJoinReplyAllDenyData
{
    public IEnumerable<GuildApplyList> guild_apply_list { get; set; }

    public GuildJoinReplyAllDenyData(IEnumerable<GuildApplyList> guild_apply_list)
    {
        this.guild_apply_list = guild_apply_list;
    }

    public GuildJoinReplyAllDenyData() { }
}

[MessagePackObject(true)]
public class GuildJoinReplyData
{
    public IEnumerable<GuildMemberList> guild_member_list { get; set; }
    public IEnumerable<GuildApplyList> guild_apply_list { get; set; }

    public GuildJoinReplyData(
        IEnumerable<GuildMemberList> guild_member_list,
        IEnumerable<GuildApplyList> guild_apply_list
    )
    {
        this.guild_member_list = guild_member_list;
        this.guild_apply_list = guild_apply_list;
    }

    public GuildJoinReplyData() { }
}

[MessagePackObject(true)]
public class GuildJoinRequestCancelData
{
    public UpdateDataList update_data_list { get; set; }

    public GuildJoinRequestCancelData(UpdateDataList update_data_list)
    {
        this.update_data_list = update_data_list;
    }

    public GuildJoinRequestCancelData() { }
}

[MessagePackObject(true)]
public class GuildJoinRequestData
{
    public UpdateDataList update_data_list { get; set; }

    public GuildJoinRequestData(UpdateDataList update_data_list)
    {
        this.update_data_list = update_data_list;
    }

    public GuildJoinRequestData() { }
}

[MessagePackObject(true)]
public class GuildJoinData
{
    public UpdateDataList update_data_list { get; set; }
    public IEnumerable<GuildMemberList> guild_member_list { get; set; }

    public GuildJoinData(
        UpdateDataList update_data_list,
        IEnumerable<GuildMemberList> guild_member_list
    )
    {
        this.update_data_list = update_data_list;
        this.guild_member_list = guild_member_list;
    }

    public GuildJoinData() { }
}

[MessagePackObject(true)]
public class GuildPostReportData
{
    public int result { get; set; }

    public GuildPostReportData(int result)
    {
        this.result = result;
    }

    public GuildPostReportData() { }
}

[MessagePackObject(true)]
public class GuildResignData
{
    public UpdateDataList update_data_list { get; set; }

    public GuildResignData(UpdateDataList update_data_list)
    {
        this.update_data_list = update_data_list;
    }

    public GuildResignData() { }
}

[MessagePackObject(true)]
public class GuildSearchAutoSearchData
{
    public IEnumerable<GuildData> auto_search_guild_list { get; set; }

    public GuildSearchAutoSearchData(IEnumerable<GuildData> auto_search_guild_list)
    {
        this.auto_search_guild_list = auto_search_guild_list;
    }

    public GuildSearchAutoSearchData() { }
}

[MessagePackObject(true)]
public class GuildSearchGetGuildDetailData
{
    public IEnumerable<GuildData> search_guild_list { get; set; }

    public GuildSearchGetGuildDetailData(IEnumerable<GuildData> search_guild_list)
    {
        this.search_guild_list = search_guild_list;
    }

    public GuildSearchGetGuildDetailData() { }
}

[MessagePackObject(true)]
public class GuildSearchIdSearchData
{
    public IEnumerable<GuildData> search_guild_list { get; set; }

    public GuildSearchIdSearchData(IEnumerable<GuildData> search_guild_list)
    {
        this.search_guild_list = search_guild_list;
    }

    public GuildSearchIdSearchData() { }
}

[MessagePackObject(true)]
public class GuildSearchNameSearchData
{
    public IEnumerable<GuildData> search_guild_list { get; set; }

    public GuildSearchNameSearchData(IEnumerable<GuildData> search_guild_list)
    {
        this.search_guild_list = search_guild_list;
    }

    public GuildSearchNameSearchData() { }
}

[MessagePackObject(true)]
public class GuildUpdateGuildPositionTypeData
{
    public IEnumerable<GuildMemberList> guild_member_list { get; set; }

    public GuildUpdateGuildPositionTypeData(IEnumerable<GuildMemberList> guild_member_list)
    {
        this.guild_member_list = guild_member_list;
    }

    public GuildUpdateGuildPositionTypeData() { }
}

[MessagePackObject(true)]
public class GuildUpdateGuildSettingData
{
    public UpdateDataList update_data_list { get; set; }

    public GuildUpdateGuildSettingData(UpdateDataList update_data_list)
    {
        this.update_data_list = update_data_list;
    }

    public GuildUpdateGuildSettingData() { }
}

[MessagePackObject(true)]
public class GuildUpdateUserSettingData
{
    public UpdateDataList update_data_list { get; set; }

    public GuildUpdateUserSettingData(UpdateDataList update_data_list)
    {
        this.update_data_list = update_data_list;
    }

    public GuildUpdateUserSettingData() { }
}

[MessagePackObject(true)]
public class InquiryAggregationData
{
    public int result { get; set; }

    public InquiryAggregationData(int result)
    {
        this.result = result;
    }

    public InquiryAggregationData() { }
}

[MessagePackObject(true)]
public class InquiryDetailData
{
    public string opinion_id { get; set; }
    public int opinion_type { get; set; }
    public string opinion_text { get; set; }
    public IEnumerable<AtgenCommentList> comment_list { get; set; }
    public int occurred_at { get; set; }
    public int created_at { get; set; }

    public InquiryDetailData(
        string opinion_id,
        int opinion_type,
        string opinion_text,
        IEnumerable<AtgenCommentList> comment_list,
        int occurred_at,
        int created_at
    )
    {
        this.opinion_id = opinion_id;
        this.opinion_type = opinion_type;
        this.opinion_text = opinion_text;
        this.comment_list = comment_list;
        this.occurred_at = occurred_at;
        this.created_at = created_at;
    }

    public InquiryDetailData() { }
}

[MessagePackObject(true)]
public class InquiryReplyData { }

[MessagePackObject(true)]
public class InquirySendData { }

[MessagePackObject(true)]
public class InquiryTopData
{
    public IEnumerable<AtgenOpinionList> opinion_list { get; set; }
    public IEnumerable<AtgenOpinionTypeList> opinion_type_list { get; set; }
    public IEnumerable<AtgenInquiryFaqList> inquiry_faq_list { get; set; }

    public InquiryTopData(
        IEnumerable<AtgenOpinionList> opinion_list,
        IEnumerable<AtgenOpinionTypeList> opinion_type_list,
        IEnumerable<AtgenInquiryFaqList> inquiry_faq_list
    )
    {
        this.opinion_list = opinion_list;
        this.opinion_type_list = opinion_type_list;
        this.inquiry_faq_list = inquiry_faq_list;
    }

    public InquiryTopData() { }
}

[MessagePackObject(true)]
public class ItemGetListData
{
    public IEnumerable<ItemList> item_list { get; set; }

    public ItemGetListData(IEnumerable<ItemList> item_list)
    {
        this.item_list = item_list;
    }

    public ItemGetListData() { }
}

[MessagePackObject(true)]
public class ItemUseRecoveryStaminaData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public AtgenRecoverData recover_data { get; set; }

    public ItemUseRecoveryStaminaData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        AtgenRecoverData recover_data
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.recover_data = recover_data;
    }

    public ItemUseRecoveryStaminaData() { }
}

[MessagePackObject(true)]
public class LoadIndexData
{
    public UserData user_data { get; set; }
    public PartyPowerData party_power_data { get; set; }
    public IEnumerable<PartyList> party_list { get; set; }
    public IEnumerable<CharaList> chara_list { get; set; }
    public IEnumerable<DragonList> dragon_list { get; set; }
    public IEnumerable<QuestList> quest_list { get; set; }
    public IEnumerable<QuestEventList> quest_event_list { get; set; }
    public IEnumerable<MaterialList> material_list { get; set; }
    public IEnumerable<AstralItemList> astral_item_list { get; set; }
    public IEnumerable<WeaponList> weapon_list { get; set; }
    public IEnumerable<AlbumWeaponList> album_weapon_list { get; set; }
    public IEnumerable<AmuletList> amulet_list { get; set; }
    public IEnumerable<WeaponSkinList> weapon_skin_list { get; set; }
    public IEnumerable<WeaponBodyList> weapon_body_list { get; set; }
    public IEnumerable<WeaponPassiveAbilityList> weapon_passive_ability_list { get; set; }
    public IEnumerable<AbilityCrestList> ability_crest_list { get; set; }
    public IEnumerable<TalismanList> talisman_list { get; set; }
    public IEnumerable<DragonReliabilityList> dragon_reliability_list { get; set; }
    public IEnumerable<DragonGiftList> dragon_gift_list { get; set; }
    public IEnumerable<AlbumDragonData> album_dragon_list { get; set; }
    public IEnumerable<EquipStampList> equip_stamp_list { get; set; }
    public IEnumerable<UnitStoryList> unit_story_list { get; set; }
    public IEnumerable<CastleStoryList> castle_story_list { get; set; }
    public IEnumerable<QuestStoryList> quest_story_list { get; set; }
    public IEnumerable<QuestTreasureList> quest_treasure_list { get; set; }
    public IEnumerable<QuestWallList> quest_wall_list { get; set; }
    public IEnumerable<QuestCarryList> quest_carry_list { get; set; }
    public IEnumerable<QuestEntryConditionList> quest_entry_condition_list { get; set; }
    public FortBonusList fort_bonus_list { get; set; }
    public IEnumerable<BuildList> build_list { get; set; }
    public IEnumerable<CraftList> craft_list { get; set; }
    public IEnumerable<UserSummonList> user_summon_list { get; set; }
    public IEnumerable<SummonTicketList> summon_ticket_list { get; set; }
    public IEnumerable<SummonPointList> summon_point_list { get; set; }
    public IEnumerable<LotteryTicketList> lottery_ticket_list { get; set; }
    public IEnumerable<ExchangeTicketList> exchange_ticket_list { get; set; }
    public IEnumerable<GatherItemList> gather_item_list { get; set; }
    public IEnumerable<FortPlantList> fort_plant_list { get; set; }
    public UserGuildData user_guild_data { get; set; }
    public GuildData guild_data { get; set; }
    public PresentNotice present_notice { get; set; }
    public FriendNotice friend_notice { get; set; }
    public MissionNotice mission_notice { get; set; }
    public GuildNotice guild_notice { get; set; }
    public ShopNotice shop_notice { get; set; }
    public AlbumPassiveNotice album_passive_notice { get; set; }
    public IEnumerable<FunctionalMaintenanceList> functional_maintenance_list { get; set; }
    public IEnumerable<TreasureTradeList> treasure_trade_all_list { get; set; }
    public IEnumerable<UserTreasureTradeList> user_treasure_trade_list { get; set; }
    public IEnumerable<ShopPurchaseList> special_shop_purchase { get; set; }
    public int stamina_single_recover_second { get; set; }
    public int stamina_multi_user_max { get; set; }
    public int stamina_multi_system_max { get; set; }
    public int quest_skip_point_system_max { get; set; }
    public int quest_skip_point_use_limit_max { get; set; }
    public int spec_upgrade_time { get; set; }

    [MessagePackFormatter(typeof(DateTimeOffsetIntFormatter))]
    public DateTimeOffset server_time { get; set; }
    public int quest_bonus_stack_base_time { get; set; }
    public IEnumerable<AtgenQuestBonus> quest_bonus { get; set; }
    public AtgenMultiServer multi_server { get; set; }
    public AtgenWalkerData walker_data { get; set; }

    [JsonConstructor]
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
        this.weapon_list = weapon_list;
        this.album_weapon_list = album_weapon_list;
        this.amulet_list = amulet_list;
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

    public LoadIndexData() { }
}

[MessagePackObject(true)]
public class LoginIndexData
{
    public IEnumerable<AtgenLoginBonusList> login_bonus_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public AtgenSupportReward support_reward { get; set; }
    public int dragon_contact_free_gift_count { get; set; }
    public IEnumerable<AtgenMonthlyWallReceiveList> monthly_wall_receive_list { get; set; }
    public IEnumerable<AtgenLoginLotteryRewardList> login_lottery_reward_list { get; set; }
    public AtgenPenaltyData penalty_data { get; set; }
    public IEnumerable<AtgenExchangeSummomPointList> exchange_summom_point_list { get; set; }
    public int before_exchange_summon_item_quantity { get; set; }
    public int server_time { get; set; }

    public LoginIndexData(
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
    )
    {
        this.login_bonus_list = login_bonus_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.support_reward = support_reward;
        this.dragon_contact_free_gift_count = dragon_contact_free_gift_count;
        this.monthly_wall_receive_list = monthly_wall_receive_list;
        this.login_lottery_reward_list = login_lottery_reward_list;
        this.penalty_data = penalty_data;
        this.exchange_summom_point_list = exchange_summom_point_list;
        this.before_exchange_summon_item_quantity = before_exchange_summon_item_quantity;
        this.server_time = server_time;
    }

    public LoginIndexData() { }
}

[MessagePackObject(true)]
public class LoginPenaltyConfirmData
{
    public int result { get; set; }
    public AtgenPenaltyData penalty_data { get; set; }

    public LoginPenaltyConfirmData(int result, AtgenPenaltyData penalty_data)
    {
        this.result = result;
        this.penalty_data = penalty_data;
    }

    public LoginPenaltyConfirmData() { }
}

[MessagePackObject(true)]
public class LoginVerifyJwsData { }

[MessagePackObject(true)]
public class LotteryGetOddsDataData
{
    public LotteryOddsRateList lottery_odds_rate_list { get; set; }

    public LotteryGetOddsDataData(LotteryOddsRateList lottery_odds_rate_list)
    {
        this.lottery_odds_rate_list = lottery_odds_rate_list;
    }

    public LotteryGetOddsDataData() { }
}

[MessagePackObject(true)]
public class LotteryLotteryExecData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenLotteryResultList> lottery_result_list { get; set; }

    public LotteryLotteryExecData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenLotteryResultList> lottery_result_list
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.lottery_result_list = lottery_result_list;
    }

    public LotteryLotteryExecData() { }
}

[MessagePackObject(true)]
public class LotteryResultData
{
    public IEnumerable<AtgenLotteryResultList> lottery_result_list { get; set; }

    public LotteryResultData(IEnumerable<AtgenLotteryResultList> lottery_result_list)
    {
        this.lottery_result_list = lottery_result_list;
    }

    public LotteryResultData() { }
}

[MessagePackObject(true)]
public class MaintenanceGetTextData
{
    public string maintenance_text { get; set; }

    public MaintenanceGetTextData(string maintenance_text)
    {
        this.maintenance_text = maintenance_text;
    }

    public MaintenanceGetTextData() { }
}

[MessagePackObject(true)]
public class MatchingCheckPenaltyUserData
{
    public int result { get; set; }

    public MatchingCheckPenaltyUserData(int result)
    {
        this.result = result;
    }

    public MatchingCheckPenaltyUserData() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomListByGuildData
{
    public IEnumerable<RoomList> room_list { get; set; }

    public MatchingGetRoomListByGuildData(IEnumerable<RoomList> room_list)
    {
        this.room_list = room_list;
    }

    public MatchingGetRoomListByGuildData() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomListByLocationData
{
    public IEnumerable<RoomList> room_list { get; set; }

    public MatchingGetRoomListByLocationData(IEnumerable<RoomList> room_list)
    {
        this.room_list = room_list;
    }

    public MatchingGetRoomListByLocationData() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomListByQuestIdData
{
    public IEnumerable<RoomList> room_list { get; set; }

    public MatchingGetRoomListByQuestIdData(IEnumerable<RoomList> room_list)
    {
        this.room_list = room_list;
    }

    public MatchingGetRoomListByQuestIdData() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomListData
{
    public IEnumerable<RoomList> room_list { get; set; }
    public IEnumerable<RoomList> friend_room_list { get; set; }
    public IEnumerable<RoomList> event_room_list { get; set; }
    public IEnumerable<RoomList> event_friend_room_list { get; set; }

    public MatchingGetRoomListData(
        IEnumerable<RoomList> room_list,
        IEnumerable<RoomList> friend_room_list,
        IEnumerable<RoomList> event_room_list,
        IEnumerable<RoomList> event_friend_room_list
    )
    {
        this.room_list = room_list;
        this.friend_room_list = friend_room_list;
        this.event_room_list = event_room_list;
        this.event_friend_room_list = event_friend_room_list;
    }

    public MatchingGetRoomListData() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomNameData
{
    public string room_name { get; set; }
    public int quest_id { get; set; }
    public string cluster_name { get; set; }
    public RoomList room_data { get; set; }
    public int is_friend { get; set; }

    public MatchingGetRoomNameData(
        string room_name,
        int quest_id,
        string cluster_name,
        RoomList room_data,
        int is_friend
    )
    {
        this.room_name = room_name;
        this.quest_id = quest_id;
        this.cluster_name = cluster_name;
        this.room_data = room_data;
        this.is_friend = is_friend;
    }

    public MatchingGetRoomNameData() { }
}

[MessagePackObject(true)]
public class MazeEventEntryData
{
    public MazeEventUserList maze_event_user_data { get; set; }

    public MazeEventEntryData(MazeEventUserList maze_event_user_data)
    {
        this.maze_event_user_data = maze_event_user_data;
    }

    public MazeEventEntryData() { }
}

[MessagePackObject(true)]
public class MazeEventGetEventDataData
{
    public MazeEventUserList maze_event_user_data { get; set; }
    public IEnumerable<BuildEventRewardList> maze_event_reward_list { get; set; }
    public IEnumerable<EventTradeList> event_trade_list { get; set; }

    public MazeEventGetEventDataData(
        MazeEventUserList maze_event_user_data,
        IEnumerable<BuildEventRewardList> maze_event_reward_list,
        IEnumerable<EventTradeList> event_trade_list
    )
    {
        this.maze_event_user_data = maze_event_user_data;
        this.maze_event_reward_list = maze_event_reward_list;
        this.event_trade_list = event_trade_list;
    }

    public MazeEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class MazeEventReceiveMazePointRewardData
{
    public IEnumerable<BuildEventRewardList> maze_event_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> maze_event_reward_entity_list { get; set; }

    public MazeEventReceiveMazePointRewardData(
        IEnumerable<BuildEventRewardList> maze_event_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenBuildEventRewardEntityList> maze_event_reward_entity_list
    )
    {
        this.maze_event_reward_list = maze_event_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.maze_event_reward_entity_list = maze_event_reward_entity_list;
    }

    public MazeEventReceiveMazePointRewardData() { }
}

[MessagePackObject(true)]
public class MemoryEventActivateData
{
    public int result { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public MemoryEventActivateData(
        int result,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.result = result;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public MemoryEventActivateData() { }
}

[MessagePackObject(true)]
public class MissionGetDrillMissionListData
{
    public IEnumerable<DrillMissionList> drill_mission_list { get; set; }
    public IEnumerable<DrillMissionGroupList> drill_mission_group_list { get; set; }
    public MissionNotice mission_notice { get; set; }

    public MissionGetDrillMissionListData(
        IEnumerable<DrillMissionList> drill_mission_list,
        IEnumerable<DrillMissionGroupList> drill_mission_group_list,
        MissionNotice mission_notice
    )
    {
        this.drill_mission_list = drill_mission_list;
        this.drill_mission_group_list = drill_mission_group_list;
        this.mission_notice = mission_notice;
    }

    public MissionGetDrillMissionListData() { }
}

[MessagePackObject(true)]
public class MissionGetMissionListData
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<int> special_mission_purchased_group_id_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public CurrentMainStoryMission current_main_story_mission { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public MissionNotice mission_notice { get; set; }

    public MissionGetMissionListData(
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
    )
    {
        this.normal_mission_list = normal_mission_list;
        this.daily_mission_list = daily_mission_list;
        this.period_mission_list = period_mission_list;
        this.beginner_mission_list = beginner_mission_list;
        this.special_mission_list = special_mission_list;
        this.special_mission_purchased_group_id_list = special_mission_purchased_group_id_list;
        this.main_story_mission_list = main_story_mission_list;
        this.current_main_story_mission = current_main_story_mission;
        this.memory_event_mission_list = memory_event_mission_list;
        this.album_mission_list = album_mission_list;
        this.mission_notice = mission_notice;
    }

    public MissionGetMissionListData() { }
}

[MessagePackObject(true)]
public class MissionReceiveAlbumRewardData
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<int> not_received_mission_id_list { get; set; }
    public IEnumerable<int> need_entry_event_id_list { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; }

    public MissionReceiveAlbumRewardData(
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
    )
    {
        this.normal_mission_list = normal_mission_list;
        this.daily_mission_list = daily_mission_list;
        this.period_mission_list = period_mission_list;
        this.beginner_mission_list = beginner_mission_list;
        this.special_mission_list = special_mission_list;
        this.main_story_mission_list = main_story_mission_list;
        this.memory_event_mission_list = memory_event_mission_list;
        this.album_mission_list = album_mission_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.not_received_mission_id_list = not_received_mission_id_list;
        this.need_entry_event_id_list = need_entry_event_id_list;
        this.converted_entity_list = converted_entity_list;
    }

    public MissionReceiveAlbumRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveBeginnerRewardData
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<int> not_received_mission_id_list { get; set; }
    public IEnumerable<int> need_entry_event_id_list { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; }

    public MissionReceiveBeginnerRewardData(
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
    )
    {
        this.normal_mission_list = normal_mission_list;
        this.daily_mission_list = daily_mission_list;
        this.period_mission_list = period_mission_list;
        this.beginner_mission_list = beginner_mission_list;
        this.special_mission_list = special_mission_list;
        this.main_story_mission_list = main_story_mission_list;
        this.memory_event_mission_list = memory_event_mission_list;
        this.album_mission_list = album_mission_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.not_received_mission_id_list = not_received_mission_id_list;
        this.need_entry_event_id_list = need_entry_event_id_list;
        this.converted_entity_list = converted_entity_list;
    }

    public MissionReceiveBeginnerRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveDailyRewardData
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenNotReceivedMissionIdListWithDayNo> not_received_mission_id_list_with_day_no { get; set; }
    public IEnumerable<int> need_entry_event_id_list { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; }

    public MissionReceiveDailyRewardData(
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
    )
    {
        this.normal_mission_list = normal_mission_list;
        this.daily_mission_list = daily_mission_list;
        this.period_mission_list = period_mission_list;
        this.beginner_mission_list = beginner_mission_list;
        this.special_mission_list = special_mission_list;
        this.main_story_mission_list = main_story_mission_list;
        this.memory_event_mission_list = memory_event_mission_list;
        this.album_mission_list = album_mission_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.not_received_mission_id_list_with_day_no = not_received_mission_id_list_with_day_no;
        this.need_entry_event_id_list = need_entry_event_id_list;
        this.converted_entity_list = converted_entity_list;
    }

    public MissionReceiveDailyRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveDrillRewardData
{
    public IEnumerable<DrillMissionList> drill_mission_list { get; set; }
    public IEnumerable<DrillMissionGroupList> drill_mission_group_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<int> not_received_mission_id_list { get; set; }
    public IEnumerable<int> need_entry_event_id_list { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> drill_mission_group_complete_reward_list { get; set; }

    public MissionReceiveDrillRewardData(
        IEnumerable<DrillMissionList> drill_mission_list,
        IEnumerable<DrillMissionGroupList> drill_mission_group_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<int> not_received_mission_id_list,
        IEnumerable<int> need_entry_event_id_list,
        IEnumerable<ConvertedEntityList> converted_entity_list,
        IEnumerable<AtgenBuildEventRewardEntityList> drill_mission_group_complete_reward_list
    )
    {
        this.drill_mission_list = drill_mission_list;
        this.drill_mission_group_list = drill_mission_group_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.not_received_mission_id_list = not_received_mission_id_list;
        this.need_entry_event_id_list = need_entry_event_id_list;
        this.converted_entity_list = converted_entity_list;
        this.drill_mission_group_complete_reward_list = drill_mission_group_complete_reward_list;
    }

    public MissionReceiveDrillRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveMainStoryRewardData
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<int> not_received_mission_id_list { get; set; }
    public IEnumerable<int> need_entry_event_id_list { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; }

    public MissionReceiveMainStoryRewardData(
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
    )
    {
        this.normal_mission_list = normal_mission_list;
        this.daily_mission_list = daily_mission_list;
        this.period_mission_list = period_mission_list;
        this.beginner_mission_list = beginner_mission_list;
        this.special_mission_list = special_mission_list;
        this.main_story_mission_list = main_story_mission_list;
        this.memory_event_mission_list = memory_event_mission_list;
        this.album_mission_list = album_mission_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.not_received_mission_id_list = not_received_mission_id_list;
        this.need_entry_event_id_list = need_entry_event_id_list;
        this.converted_entity_list = converted_entity_list;
    }

    public MissionReceiveMainStoryRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveMemoryEventRewardData
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<int> not_received_mission_id_list { get; set; }
    public IEnumerable<int> need_entry_event_id_list { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; }

    public MissionReceiveMemoryEventRewardData(
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
    )
    {
        this.normal_mission_list = normal_mission_list;
        this.daily_mission_list = daily_mission_list;
        this.period_mission_list = period_mission_list;
        this.beginner_mission_list = beginner_mission_list;
        this.special_mission_list = special_mission_list;
        this.main_story_mission_list = main_story_mission_list;
        this.memory_event_mission_list = memory_event_mission_list;
        this.album_mission_list = album_mission_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.not_received_mission_id_list = not_received_mission_id_list;
        this.need_entry_event_id_list = need_entry_event_id_list;
        this.converted_entity_list = converted_entity_list;
    }

    public MissionReceiveMemoryEventRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveNormalRewardData
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<int> not_received_mission_id_list { get; set; }
    public IEnumerable<int> need_entry_event_id_list { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; }

    public MissionReceiveNormalRewardData(
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
    )
    {
        this.normal_mission_list = normal_mission_list;
        this.daily_mission_list = daily_mission_list;
        this.period_mission_list = period_mission_list;
        this.beginner_mission_list = beginner_mission_list;
        this.special_mission_list = special_mission_list;
        this.main_story_mission_list = main_story_mission_list;
        this.memory_event_mission_list = memory_event_mission_list;
        this.album_mission_list = album_mission_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.not_received_mission_id_list = not_received_mission_id_list;
        this.need_entry_event_id_list = need_entry_event_id_list;
        this.converted_entity_list = converted_entity_list;
    }

    public MissionReceiveNormalRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceivePeriodRewardData
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<int> not_received_mission_id_list { get; set; }
    public IEnumerable<int> need_entry_event_id_list { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; }

    public MissionReceivePeriodRewardData(
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
    )
    {
        this.normal_mission_list = normal_mission_list;
        this.daily_mission_list = daily_mission_list;
        this.period_mission_list = period_mission_list;
        this.beginner_mission_list = beginner_mission_list;
        this.special_mission_list = special_mission_list;
        this.main_story_mission_list = main_story_mission_list;
        this.memory_event_mission_list = memory_event_mission_list;
        this.album_mission_list = album_mission_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.not_received_mission_id_list = not_received_mission_id_list;
        this.need_entry_event_id_list = need_entry_event_id_list;
        this.converted_entity_list = converted_entity_list;
    }

    public MissionReceivePeriodRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveSpecialRewardData
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<int> not_received_mission_id_list { get; set; }
    public IEnumerable<int> need_entry_event_id_list { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; }

    public MissionReceiveSpecialRewardData(
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
    )
    {
        this.normal_mission_list = normal_mission_list;
        this.daily_mission_list = daily_mission_list;
        this.period_mission_list = period_mission_list;
        this.beginner_mission_list = beginner_mission_list;
        this.special_mission_list = special_mission_list;
        this.main_story_mission_list = main_story_mission_list;
        this.memory_event_mission_list = memory_event_mission_list;
        this.album_mission_list = album_mission_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.not_received_mission_id_list = not_received_mission_id_list;
        this.need_entry_event_id_list = need_entry_event_id_list;
        this.converted_entity_list = converted_entity_list;
    }

    public MissionReceiveSpecialRewardData() { }
}

[MessagePackObject(true)]
public class MissionUnlockDrillMissionGroupData
{
    public IEnumerable<DrillMissionList> drill_mission_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public MissionUnlockDrillMissionGroupData(
        IEnumerable<DrillMissionList> drill_mission_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.drill_mission_list = drill_mission_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public MissionUnlockDrillMissionGroupData() { }
}

[MessagePackObject(true)]
public class MissionUnlockMainStoryGroupData
{
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> main_story_mission_unlock_bonus_list { get; set; }

    public MissionUnlockMainStoryGroupData(
        IEnumerable<MainStoryMissionList> main_story_mission_list,
        UpdateDataList update_data_list,
        IEnumerable<AtgenBuildEventRewardEntityList> main_story_mission_unlock_bonus_list
    )
    {
        this.main_story_mission_list = main_story_mission_list;
        this.update_data_list = update_data_list;
        this.main_story_mission_unlock_bonus_list = main_story_mission_unlock_bonus_list;
    }

    public MissionUnlockMainStoryGroupData() { }
}

[MessagePackObject(true)]
public class MypageInfoData
{
    public int present_cnt { get; set; }
    public int friend_apply { get; set; }
    public bool friend { get; set; }
    public int achievement_cnt { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public int is_receive_event_damage_reward { get; set; }
    public int is_view_start_dash { get; set; }
    public int is_view_dream_select { get; set; }
    public int is_shop_notification { get; set; }
    public RepeatData repeat_data { get; set; }
    public IEnumerable<UserSummonList> user_summon_list { get; set; }
    public IEnumerable<QuestEventScheduleList> quest_event_schedule_list { get; set; }
    public IEnumerable<QuestScheduleDetailList> quest_schedule_detail_list { get; set; }

    public MypageInfoData(
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
    )
    {
        this.present_cnt = present_cnt;
        this.friend_apply = friend_apply;
        this.friend = friend;
        this.achievement_cnt = achievement_cnt;
        this.update_data_list = update_data_list;
        this.is_receive_event_damage_reward = is_receive_event_damage_reward;
        this.is_view_start_dash = is_view_start_dash;
        this.is_view_dream_select = is_view_dream_select;
        this.is_shop_notification = is_shop_notification;
        this.repeat_data = repeat_data;
        this.user_summon_list = user_summon_list;
        this.quest_event_schedule_list = quest_event_schedule_list;
        this.quest_schedule_detail_list = quest_schedule_detail_list;
    }

    public MypageInfoData() { }
}

[MessagePackObject(true)]
public class OptionGetOptionData
{
    public OptionData option_data { get; set; }

    public OptionGetOptionData(OptionData option_data)
    {
        this.option_data = option_data;
    }

    public OptionGetOptionData() { }
}

[MessagePackObject(true)]
public class OptionSetOptionData
{
    public OptionData option_data { get; set; }

    public OptionSetOptionData(OptionData option_data)
    {
        this.option_data = option_data;
    }

    public OptionSetOptionData() { }
}

[MessagePackObject(true)]
public class PartyIndexData
{
    public IEnumerable<BuildList> build_list { get; set; }

    public PartyIndexData(IEnumerable<BuildList> build_list)
    {
        this.build_list = build_list;
    }

    public PartyIndexData() { }
}

[MessagePackObject(true)]
public class PartySetMainPartyNoData
{
    public int main_party_no { get; set; }

    public PartySetMainPartyNoData(int main_party_no)
    {
        this.main_party_no = main_party_no;
    }

    public PartySetMainPartyNoData() { }
}

[MessagePackObject(true)]
public class PartySetPartySettingData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public PartySetPartySettingData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public PartySetPartySettingData() { }
}

[MessagePackObject(true)]
public class PartyUpdatePartyNameData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public PartyUpdatePartyNameData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public PartyUpdatePartyNameData() { }
}

[MessagePackObject(true)]
public class PlatformAchievementGetPlatformAchievementListData
{
    public IEnumerable<AchievementList> achievement_list { get; set; }

    public PlatformAchievementGetPlatformAchievementListData(
        IEnumerable<AchievementList> achievement_list
    )
    {
        this.achievement_list = achievement_list;
    }

    public PlatformAchievementGetPlatformAchievementListData() { }
}

[MessagePackObject(true)]
public class PresentGetHistoryListData
{
    public IEnumerable<PresentHistoryList> present_history_list { get; set; }

    public PresentGetHistoryListData(IEnumerable<PresentHistoryList> present_history_list)
    {
        this.present_history_list = present_history_list;
    }

    public PresentGetHistoryListData() { }
}

[MessagePackObject(true)]
public class PresentGetPresentListData
{
    public IEnumerable<PresentDetailList> present_list { get; set; }
    public IEnumerable<PresentDetailList> present_limit_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public PresentGetPresentListData(
        IEnumerable<PresentDetailList> present_list,
        IEnumerable<PresentDetailList> present_limit_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.present_list = present_list;
        this.present_limit_list = present_limit_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public PresentGetPresentListData() { }
}

[MessagePackObject(true)]
public class PresentReceiveData
{
    public IEnumerable<ulong> receive_present_id_list { get; set; }
    public IEnumerable<ulong> not_receive_present_id_list { get; set; }
    public IEnumerable<ulong> delete_present_id_list { get; set; }
    public IEnumerable<ulong> limit_over_present_id_list { get; set; }
    public IEnumerable<PresentDetailList> present_list { get; set; }
    public IEnumerable<PresentDetailList> present_limit_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; }

    public PresentReceiveData(
        IEnumerable<ulong> receive_present_id_list,
        IEnumerable<ulong> not_receive_present_id_list,
        IEnumerable<ulong> delete_present_id_list,
        IEnumerable<ulong> limit_over_present_id_list,
        IEnumerable<PresentDetailList> present_list,
        IEnumerable<PresentDetailList> present_limit_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<ConvertedEntityList> converted_entity_list
    )
    {
        this.receive_present_id_list = receive_present_id_list;
        this.not_receive_present_id_list = not_receive_present_id_list;
        this.delete_present_id_list = delete_present_id_list;
        this.limit_over_present_id_list = limit_over_present_id_list;
        this.present_list = present_list;
        this.present_limit_list = present_limit_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.converted_entity_list = converted_entity_list;
    }

    public PresentReceiveData() { }
}

[MessagePackObject(true)]
public class PushNotificationUpdateSettingData
{
    public int result { get; set; }

    public PushNotificationUpdateSettingData(int result)
    {
        this.result = result;
    }

    public PushNotificationUpdateSettingData() { }
}

[MessagePackObject(true)]
public class QuestDropListData
{
    public AtgenQuestDropInfo quest_drop_info { get; set; }

    public QuestDropListData(AtgenQuestDropInfo quest_drop_info)
    {
        this.quest_drop_info = quest_drop_info;
    }

    public QuestDropListData() { }
}

[MessagePackObject(true)]
public class QuestGetQuestClearPartyMultiData
{
    public IEnumerable<PartySettingList> quest_multi_clear_party_setting_list { get; set; }
    public IEnumerable<AtgenLostUnitList> lost_unit_list { get; set; }

    public QuestGetQuestClearPartyMultiData(
        IEnumerable<PartySettingList> quest_multi_clear_party_setting_list,
        IEnumerable<AtgenLostUnitList> lost_unit_list
    )
    {
        this.quest_multi_clear_party_setting_list = quest_multi_clear_party_setting_list;
        this.lost_unit_list = lost_unit_list;
    }

    public QuestGetQuestClearPartyMultiData() { }
}

[MessagePackObject(true)]
public class QuestGetQuestClearPartyData
{
    public IEnumerable<PartySettingList> quest_clear_party_setting_list { get; set; }
    public IEnumerable<AtgenLostUnitList> lost_unit_list { get; set; }

    public QuestGetQuestClearPartyData(
        IEnumerable<PartySettingList> quest_clear_party_setting_list,
        IEnumerable<AtgenLostUnitList> lost_unit_list
    )
    {
        this.quest_clear_party_setting_list = quest_clear_party_setting_list;
        this.lost_unit_list = lost_unit_list;
    }

    public QuestGetQuestClearPartyData() { }
}

[MessagePackObject(true)]
public class QuestGetSupportUserListData
{
    public IEnumerable<UserSupportList> support_user_list { get; set; }
    public IEnumerable<AtgenSupportUserDetailList> support_user_detail_list { get; set; }

    public QuestGetSupportUserListData(
        IEnumerable<UserSupportList> support_user_list,
        IEnumerable<AtgenSupportUserDetailList> support_user_detail_list
    )
    {
        this.support_user_list = support_user_list;
        this.support_user_detail_list = support_user_detail_list;
    }

    public QuestGetSupportUserListData() { }
}

[MessagePackObject(true)]
public class QuestOpenTreasureData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> quest_treasure_reward_list { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list { get; set; }
    public int add_max_dragon_quantity { get; set; }
    public int add_max_weapon_quantity { get; set; }
    public int add_max_amulet_quantity { get; set; }

    public QuestOpenTreasureData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenBuildEventRewardEntityList> quest_treasure_reward_list,
        IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list,
        int add_max_dragon_quantity,
        int add_max_weapon_quantity,
        int add_max_amulet_quantity
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.quest_treasure_reward_list = quest_treasure_reward_list;
        this.duplicate_entity_list = duplicate_entity_list;
        this.add_max_dragon_quantity = add_max_dragon_quantity;
        this.add_max_weapon_quantity = add_max_weapon_quantity;
        this.add_max_amulet_quantity = add_max_amulet_quantity;
    }

    public QuestOpenTreasureData() { }
}

[MessagePackObject(true)]
public class QuestReadStoryData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenQuestStoryRewardList> quest_story_reward_list { get; set; }
    public IEnumerable<ConvertedEntityList> converted_entity_list { get; set; }

    public QuestReadStoryData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenQuestStoryRewardList> quest_story_reward_list,
        IEnumerable<ConvertedEntityList> converted_entity_list
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.quest_story_reward_list = quest_story_reward_list;
        this.converted_entity_list = converted_entity_list;
    }

    public QuestReadStoryData() { }
}

[MessagePackObject(true)]
public class QuestSearchQuestClearPartyCharaMultiData
{
    public IEnumerable<SearchClearPartyCharaList> search_quest_clear_party_chara_list { get; set; }

    public QuestSearchQuestClearPartyCharaMultiData(
        IEnumerable<SearchClearPartyCharaList> search_quest_clear_party_chara_list
    )
    {
        this.search_quest_clear_party_chara_list = search_quest_clear_party_chara_list;
    }

    public QuestSearchQuestClearPartyCharaMultiData() { }
}

[MessagePackObject(true)]
public class QuestSearchQuestClearPartyCharaData
{
    public IEnumerable<SearchClearPartyCharaList> search_quest_clear_party_chara_list { get; set; }

    public QuestSearchQuestClearPartyCharaData(
        IEnumerable<SearchClearPartyCharaList> search_quest_clear_party_chara_list
    )
    {
        this.search_quest_clear_party_chara_list = search_quest_clear_party_chara_list;
    }

    public QuestSearchQuestClearPartyCharaData() { }
}

[MessagePackObject(true)]
public class QuestSearchQuestClearPartyMultiData
{
    public IEnumerable<SearchClearPartyList> search_quest_clear_party_list { get; set; }

    public QuestSearchQuestClearPartyMultiData(
        IEnumerable<SearchClearPartyList> search_quest_clear_party_list
    )
    {
        this.search_quest_clear_party_list = search_quest_clear_party_list;
    }

    public QuestSearchQuestClearPartyMultiData() { }
}

[MessagePackObject(true)]
public class QuestSearchQuestClearPartyData
{
    public IEnumerable<SearchClearPartyList> search_quest_clear_party_list { get; set; }

    public QuestSearchQuestClearPartyData(
        IEnumerable<SearchClearPartyList> search_quest_clear_party_list
    )
    {
        this.search_quest_clear_party_list = search_quest_clear_party_list;
    }

    public QuestSearchQuestClearPartyData() { }
}

[MessagePackObject(true)]
public class QuestSetQuestClearPartyMultiData
{
    public int result { get; set; }

    public QuestSetQuestClearPartyMultiData(int result)
    {
        this.result = result;
    }

    public QuestSetQuestClearPartyMultiData() { }
}

[MessagePackObject(true)]
public class QuestSetQuestClearPartyData
{
    public int result { get; set; }

    public QuestSetQuestClearPartyData(int result)
    {
        this.result = result;
    }

    public QuestSetQuestClearPartyData() { }
}

[MessagePackObject(true)]
public class RaidEventEntryData
{
    public RaidEventUserList raid_event_user_data { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public RaidEventEntryData(
        RaidEventUserList raid_event_user_data,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.raid_event_user_data = raid_event_user_data;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public RaidEventEntryData() { }
}

[MessagePackObject(true)]
public class RaidEventGetEventDataData
{
    public RaidEventUserList raid_event_user_data { get; set; }
    public IEnumerable<RaidEventRewardList> raid_event_reward_list { get; set; }
    public IEnumerable<CharaFriendshipList> chara_friendship_list { get; set; }
    public IEnumerable<EventTradeList> event_trade_list { get; set; }
    public IEnumerable<EventPassiveList> event_passive_list { get; set; }
    public IEnumerable<EventAbilityCharaList> event_ability_chara_list { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool is_receive_event_damage_reward { get; set; }
    public AtgenEventDamageData event_damage_data { get; set; }

    public RaidEventGetEventDataData(
        RaidEventUserList raid_event_user_data,
        IEnumerable<RaidEventRewardList> raid_event_reward_list,
        IEnumerable<CharaFriendshipList> chara_friendship_list,
        IEnumerable<EventTradeList> event_trade_list,
        IEnumerable<EventPassiveList> event_passive_list,
        IEnumerable<EventAbilityCharaList> event_ability_chara_list,
        bool is_receive_event_damage_reward,
        AtgenEventDamageData event_damage_data
    )
    {
        this.raid_event_user_data = raid_event_user_data;
        this.raid_event_reward_list = raid_event_reward_list;
        this.chara_friendship_list = chara_friendship_list;
        this.event_trade_list = event_trade_list;
        this.event_passive_list = event_passive_list;
        this.event_ability_chara_list = event_ability_chara_list;
        this.is_receive_event_damage_reward = is_receive_event_damage_reward;
        this.event_damage_data = event_damage_data;
    }

    public RaidEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class RaidEventReceiveRaidPointRewardData
{
    public IEnumerable<RaidEventRewardList> raid_event_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public RaidEventReceiveRaidPointRewardData(
        IEnumerable<RaidEventRewardList> raid_event_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.raid_event_reward_list = raid_event_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public RaidEventReceiveRaidPointRewardData() { }
}

[MessagePackObject(true)]
public class RedoableSummonFixExecData
{
    public UserRedoableSummonData user_redoable_summon_data { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public RedoableSummonFixExecData(
        UserRedoableSummonData user_redoable_summon_data,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.user_redoable_summon_data = user_redoable_summon_data;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public RedoableSummonFixExecData() { }
}

[MessagePackObject(true)]
public class RedoableSummonGetDataData
{
    public UserRedoableSummonData user_redoable_summon_data { get; set; }
    public RedoableSummonOddsRateList redoable_summon_odds_rate_list { get; set; }

    public RedoableSummonGetDataData(
        UserRedoableSummonData user_redoable_summon_data,
        RedoableSummonOddsRateList redoable_summon_odds_rate_list
    )
    {
        this.user_redoable_summon_data = user_redoable_summon_data;
        this.redoable_summon_odds_rate_list = redoable_summon_odds_rate_list;
    }

    public RedoableSummonGetDataData() { }
}

[MessagePackObject(true)]
public class RedoableSummonPreExecData
{
    public UserRedoableSummonData user_redoable_summon_data { get; set; }

    public RedoableSummonPreExecData(UserRedoableSummonData user_redoable_summon_data)
    {
        this.user_redoable_summon_data = user_redoable_summon_data;
    }

    public RedoableSummonPreExecData() { }
}

[MessagePackObject(true)]
public class RepeatEndData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IngameResultData ingame_result_data { get; set; }
    public RepeatData repeat_data { get; set; }

    public RepeatEndData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IngameResultData ingame_result_data,
        RepeatData repeat_data
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.ingame_result_data = ingame_result_data;
        this.repeat_data = repeat_data;
    }

    public RepeatEndData() { }
}

[MessagePackObject(true)]
public class ShopChargeCancelData
{
    public int is_quest_bonus { get; set; }
    public int is_stone_bonus { get; set; }
    public int is_stamina_bonus { get; set; }
    public IEnumerable<ShopPurchaseList> material_shop_purchase { get; set; }
    public IEnumerable<ShopPurchaseList> normal_shop_purchase { get; set; }
    public IEnumerable<ShopPurchaseList> special_shop_purchase { get; set; }
    public IEnumerable<AtgenStoneBonus> stone_bonus { get; set; }
    public IEnumerable<AtgenStaminaBonus> stamina_bonus { get; set; }
    public IEnumerable<AtgenQuestBonus> quest_bonus { get; set; }
    public IEnumerable<AtgenProductLockList> product_lock_list { get; set; }
    public IEnumerable<ProductList> product_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public AtgenUserItemSummon user_item_summon { get; set; }
    public int infancy_paid_diamond_limit { get; set; }

    public ShopChargeCancelData(
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
    )
    {
        this.is_quest_bonus = is_quest_bonus;
        this.is_stone_bonus = is_stone_bonus;
        this.is_stamina_bonus = is_stamina_bonus;
        this.material_shop_purchase = material_shop_purchase;
        this.normal_shop_purchase = normal_shop_purchase;
        this.special_shop_purchase = special_shop_purchase;
        this.stone_bonus = stone_bonus;
        this.stamina_bonus = stamina_bonus;
        this.quest_bonus = quest_bonus;
        this.product_lock_list = product_lock_list;
        this.product_list = product_list;
        this.update_data_list = update_data_list;
        this.user_item_summon = user_item_summon;
        this.infancy_paid_diamond_limit = infancy_paid_diamond_limit;
    }

    public ShopChargeCancelData() { }
}

[MessagePackObject(true)]
public class ShopGetBonusData
{
    public int is_quest_bonus { get; set; }
    public int is_stone_bonus { get; set; }
    public int is_stamina_bonus { get; set; }
    public IEnumerable<AtgenStoneBonus> stone_bonus { get; set; }
    public IEnumerable<AtgenStaminaBonus> stamina_bonus { get; set; }
    public IEnumerable<AtgenQuestBonus> quest_bonus { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> stone_bonus_entity_list { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public ShopGetBonusData(
        int is_quest_bonus,
        int is_stone_bonus,
        int is_stamina_bonus,
        IEnumerable<AtgenStoneBonus> stone_bonus,
        IEnumerable<AtgenStaminaBonus> stamina_bonus,
        IEnumerable<AtgenQuestBonus> quest_bonus,
        IEnumerable<AtgenBuildEventRewardEntityList> stone_bonus_entity_list,
        UpdateDataList update_data_list
    )
    {
        this.is_quest_bonus = is_quest_bonus;
        this.is_stone_bonus = is_stone_bonus;
        this.is_stamina_bonus = is_stamina_bonus;
        this.stone_bonus = stone_bonus;
        this.stamina_bonus = stamina_bonus;
        this.quest_bonus = quest_bonus;
        this.stone_bonus_entity_list = stone_bonus_entity_list;
        this.update_data_list = update_data_list;
    }

    public ShopGetBonusData() { }
}

[MessagePackObject(true)]
public class ShopGetDreamSelectUnitListData
{
    public IEnumerable<AtgenDuplicateEntityList> dream_select_unit_list { get; set; }

    public ShopGetDreamSelectUnitListData(
        IEnumerable<AtgenDuplicateEntityList> dream_select_unit_list
    )
    {
        this.dream_select_unit_list = dream_select_unit_list;
    }

    public ShopGetDreamSelectUnitListData() { }
}

[MessagePackObject(true)]
public class ShopGetListData
{
    public int is_quest_bonus { get; set; }
    public int is_stone_bonus { get; set; }
    public int is_stamina_bonus { get; set; }
    public IEnumerable<ShopPurchaseList> material_shop_purchase { get; set; }
    public IEnumerable<ShopPurchaseList> normal_shop_purchase { get; set; }
    public IEnumerable<ShopPurchaseList> special_shop_purchase { get; set; }
    public IEnumerable<AtgenStoneBonus> stone_bonus { get; set; }
    public IEnumerable<AtgenStaminaBonus> stamina_bonus { get; set; }
    public IEnumerable<AtgenQuestBonus> quest_bonus { get; set; }
    public IEnumerable<AtgenProductLockList> product_lock_list { get; set; }
    public IEnumerable<ProductList> product_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public AtgenUserItemSummon user_item_summon { get; set; }
    public int infancy_paid_diamond_limit { get; set; }

    public ShopGetListData(
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
    )
    {
        this.is_quest_bonus = is_quest_bonus;
        this.is_stone_bonus = is_stone_bonus;
        this.is_stamina_bonus = is_stamina_bonus;
        this.material_shop_purchase = material_shop_purchase;
        this.normal_shop_purchase = normal_shop_purchase;
        this.special_shop_purchase = special_shop_purchase;
        this.stone_bonus = stone_bonus;
        this.stamina_bonus = stamina_bonus;
        this.quest_bonus = quest_bonus;
        this.product_lock_list = product_lock_list;
        this.product_list = product_list;
        this.update_data_list = update_data_list;
        this.user_item_summon = user_item_summon;
        this.infancy_paid_diamond_limit = infancy_paid_diamond_limit;
    }

    public ShopGetListData() { }
}

[MessagePackObject(true)]
public class ShopGetProductListData
{
    public IEnumerable<ProductList> product_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public int infancy_paid_diamond_limit { get; set; }

    public ShopGetProductListData(
        IEnumerable<ProductList> product_list,
        UpdateDataList update_data_list,
        int infancy_paid_diamond_limit
    )
    {
        this.product_list = product_list;
        this.update_data_list = update_data_list;
        this.infancy_paid_diamond_limit = infancy_paid_diamond_limit;
    }

    public ShopGetProductListData() { }
}

[MessagePackObject(true)]
public class ShopItemSummonExecData
{
    public AtgenUserItemSummon user_item_summon { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> item_summon_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public ShopItemSummonExecData(
        AtgenUserItemSummon user_item_summon,
        IEnumerable<AtgenBuildEventRewardEntityList> item_summon_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.user_item_summon = user_item_summon;
        this.item_summon_reward_list = item_summon_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public ShopItemSummonExecData() { }
}

[MessagePackObject(true)]
public class ShopItemSummonOddData
{
    public IEnumerable<AtgenItemSummonRateList> item_summon_rate_list { get; set; }

    public ShopItemSummonOddData(IEnumerable<AtgenItemSummonRateList> item_summon_rate_list)
    {
        this.item_summon_rate_list = item_summon_rate_list;
    }

    public ShopItemSummonOddData() { }
}

[MessagePackObject(true)]
public class ShopMaterialShopPurchaseData
{
    public int is_quest_bonus { get; set; }
    public int is_stone_bonus { get; set; }
    public int is_stamina_bonus { get; set; }
    public IEnumerable<ShopPurchaseList> material_shop_purchase { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public ShopMaterialShopPurchaseData(
        int is_quest_bonus,
        int is_stone_bonus,
        int is_stamina_bonus,
        IEnumerable<ShopPurchaseList> material_shop_purchase,
        UpdateDataList update_data_list
    )
    {
        this.is_quest_bonus = is_quest_bonus;
        this.is_stone_bonus = is_stone_bonus;
        this.is_stamina_bonus = is_stamina_bonus;
        this.material_shop_purchase = material_shop_purchase;
        this.update_data_list = update_data_list;
    }

    public ShopMaterialShopPurchaseData() { }
}

[MessagePackObject(true)]
public class ShopNormalShopPurchaseData
{
    public int is_quest_bonus { get; set; }
    public int is_stone_bonus { get; set; }
    public int is_stamina_bonus { get; set; }
    public IEnumerable<ShopPurchaseList> normal_shop_purchase { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public ShopNormalShopPurchaseData(
        int is_quest_bonus,
        int is_stone_bonus,
        int is_stamina_bonus,
        IEnumerable<ShopPurchaseList> normal_shop_purchase,
        UpdateDataList update_data_list
    )
    {
        this.is_quest_bonus = is_quest_bonus;
        this.is_stone_bonus = is_stone_bonus;
        this.is_stamina_bonus = is_stamina_bonus;
        this.normal_shop_purchase = normal_shop_purchase;
        this.update_data_list = update_data_list;
    }

    public ShopNormalShopPurchaseData() { }
}

[MessagePackObject(true)]
public class ShopPreChargeCheckData
{
    public int is_quest_bonus { get; set; }
    public int is_stone_bonus { get; set; }
    public int is_stamina_bonus { get; set; }
    public IEnumerable<ShopPurchaseList> material_shop_purchase { get; set; }
    public IEnumerable<ShopPurchaseList> normal_shop_purchase { get; set; }
    public IEnumerable<ShopPurchaseList> special_shop_purchase { get; set; }
    public IEnumerable<AtgenStoneBonus> stone_bonus { get; set; }
    public IEnumerable<AtgenStaminaBonus> stamina_bonus { get; set; }
    public IEnumerable<AtgenQuestBonus> quest_bonus { get; set; }
    public IEnumerable<AtgenProductLockList> product_lock_list { get; set; }
    public IEnumerable<ProductList> product_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public AtgenUserItemSummon user_item_summon { get; set; }
    public int infancy_paid_diamond_limit { get; set; }
    public int is_purchase { get; set; }

    public ShopPreChargeCheckData(
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
    )
    {
        this.is_quest_bonus = is_quest_bonus;
        this.is_stone_bonus = is_stone_bonus;
        this.is_stamina_bonus = is_stamina_bonus;
        this.material_shop_purchase = material_shop_purchase;
        this.normal_shop_purchase = normal_shop_purchase;
        this.special_shop_purchase = special_shop_purchase;
        this.stone_bonus = stone_bonus;
        this.stamina_bonus = stamina_bonus;
        this.quest_bonus = quest_bonus;
        this.product_lock_list = product_lock_list;
        this.product_list = product_list;
        this.update_data_list = update_data_list;
        this.user_item_summon = user_item_summon;
        this.infancy_paid_diamond_limit = infancy_paid_diamond_limit;
        this.is_purchase = is_purchase;
    }

    public ShopPreChargeCheckData() { }
}

[MessagePackObject(true)]
public class ShopSpecialShopPurchaseData
{
    public int is_quest_bonus { get; set; }
    public int is_stone_bonus { get; set; }
    public int is_stamina_bonus { get; set; }
    public IEnumerable<ShopPurchaseList> special_shop_purchase { get; set; }
    public IEnumerable<AtgenStoneBonus> stone_bonus { get; set; }
    public IEnumerable<AtgenStaminaBonus> stamina_bonus { get; set; }
    public IEnumerable<AtgenQuestBonus> quest_bonus { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public ShopSpecialShopPurchaseData(
        int is_quest_bonus,
        int is_stone_bonus,
        int is_stamina_bonus,
        IEnumerable<ShopPurchaseList> special_shop_purchase,
        IEnumerable<AtgenStoneBonus> stone_bonus,
        IEnumerable<AtgenStaminaBonus> stamina_bonus,
        IEnumerable<AtgenQuestBonus> quest_bonus,
        UpdateDataList update_data_list
    )
    {
        this.is_quest_bonus = is_quest_bonus;
        this.is_stone_bonus = is_stone_bonus;
        this.is_stamina_bonus = is_stamina_bonus;
        this.special_shop_purchase = special_shop_purchase;
        this.stone_bonus = stone_bonus;
        this.stamina_bonus = stamina_bonus;
        this.quest_bonus = quest_bonus;
        this.update_data_list = update_data_list;
    }

    public ShopSpecialShopPurchaseData() { }
}

[MessagePackObject(true)]
public class SimpleEventEntryData
{
    public SimpleEventUserList simple_event_user_data { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public SimpleEventEntryData(
        SimpleEventUserList simple_event_user_data,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.simple_event_user_data = simple_event_user_data;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public SimpleEventEntryData() { }
}

[MessagePackObject(true)]
public class SimpleEventGetEventDataData
{
    public SimpleEventUserList simple_event_user_data { get; set; }
    public IEnumerable<EventTradeList> event_trade_list { get; set; }

    public SimpleEventGetEventDataData(
        SimpleEventUserList simple_event_user_data,
        IEnumerable<EventTradeList> event_trade_list
    )
    {
        this.simple_event_user_data = simple_event_user_data;
        this.event_trade_list = event_trade_list;
    }

    public SimpleEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class StampGetStampData
{
    public IEnumerable<StampList> stamp_list { get; set; }

    public StampGetStampData(IEnumerable<StampList> stamp_list)
    {
        this.stamp_list = stamp_list;
    }

    public StampGetStampData() { }
}

[MessagePackObject(true)]
public class StampSetEquipStampData
{
    public int result { get; set; }
    public IEnumerable<EquipStampList> equip_stamp_list { get; set; }

    public StampSetEquipStampData(int result, IEnumerable<EquipStampList> equip_stamp_list)
    {
        this.result = result;
        this.equip_stamp_list = equip_stamp_list;
    }

    public StampSetEquipStampData() { }
}

[MessagePackObject(true)]
public class StoryReadData
{
    public IEnumerable<AtgenBuildEventRewardEntityList> unit_story_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list { get; set; }

    public StoryReadData(
        IEnumerable<AtgenBuildEventRewardEntityList> unit_story_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenDuplicateEntityList> duplicate_entity_list
    )
    {
        this.unit_story_reward_list = unit_story_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.duplicate_entity_list = duplicate_entity_list;
    }

    public StoryReadData() { }
}

[MessagePackObject(true)]
public class StorySkipSkipData
{
    public int result_state { get; set; }

    public StorySkipSkipData(int result_state)
    {
        this.result_state = result_state;
    }

    public StorySkipSkipData() { }
}

[MessagePackObject(true)]
public class SuggestionGetCategoryListData
{
    public IEnumerable<AtgenCategoryList> category_list { get; set; }

    public SuggestionGetCategoryListData(IEnumerable<AtgenCategoryList> category_list)
    {
        this.category_list = category_list;
    }

    public SuggestionGetCategoryListData() { }
}

[MessagePackObject(true)]
public class SuggestionSetData { }

[MessagePackObject(true)]
public class SummonExcludeGetListData
{
    public IEnumerable<AtgenDuplicateEntityList> summon_exclude_unit_list { get; set; }

    public SummonExcludeGetListData(IEnumerable<AtgenDuplicateEntityList> summon_exclude_unit_list)
    {
        this.summon_exclude_unit_list = summon_exclude_unit_list;
    }

    public SummonExcludeGetListData() { }
}

[MessagePackObject(true)]
public class SummonExcludeGetOddsDataData
{
    public OddsRateList odds_rate_list { get; set; }
    public SummonPrizeOddsRateList summon_prize_odds_rate_list { get; set; }

    public SummonExcludeGetOddsDataData(
        OddsRateList odds_rate_list,
        SummonPrizeOddsRateList summon_prize_odds_rate_list
    )
    {
        this.odds_rate_list = odds_rate_list;
        this.summon_prize_odds_rate_list = summon_prize_odds_rate_list;
    }

    public SummonExcludeGetOddsDataData() { }
}

[MessagePackObject(true)]
public class SummonExcludeRequestData
{
    public IEnumerable<AtgenResultUnitList> result_unit_list { get; set; }
    public IEnumerable<AtgenResultPrizeList> result_prize_list { get; set; }
    public IEnumerable<int> presage_effect_list { get; set; }
    public int reversal_effect_index { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<SummonTicketList> summon_ticket_list { get; set; }
    public int result_summon_point { get; set; }
    public IEnumerable<UserSummonList> user_summon_list { get; set; }

    public SummonExcludeRequestData(
        IEnumerable<AtgenResultUnitList> result_unit_list,
        IEnumerable<AtgenResultPrizeList> result_prize_list,
        IEnumerable<int> presage_effect_list,
        int reversal_effect_index,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<SummonTicketList> summon_ticket_list,
        int result_summon_point,
        IEnumerable<UserSummonList> user_summon_list
    )
    {
        this.result_unit_list = result_unit_list;
        this.result_prize_list = result_prize_list;
        this.presage_effect_list = presage_effect_list;
        this.reversal_effect_index = reversal_effect_index;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.summon_ticket_list = summon_ticket_list;
        this.result_summon_point = result_summon_point;
        this.user_summon_list = user_summon_list;
    }

    public SummonExcludeRequestData() { }
}

[MessagePackObject(true)]
public class SummonGetOddsDataData
{
    public OddsRateList odds_rate_list { get; set; }
    public SummonPrizeOddsRateList summon_prize_odds_rate_list { get; set; }

    public SummonGetOddsDataData(
        OddsRateList odds_rate_list,
        SummonPrizeOddsRateList summon_prize_odds_rate_list
    )
    {
        this.odds_rate_list = odds_rate_list;
        this.summon_prize_odds_rate_list = summon_prize_odds_rate_list;
    }

    public SummonGetOddsDataData() { }
}

[MessagePackObject(true)]
public class SummonGetSummonHistoryData
{
    public IEnumerable<SummonHistoryList> summon_history_list { get; set; }

    public SummonGetSummonHistoryData(IEnumerable<SummonHistoryList> summon_history_list)
    {
        this.summon_history_list = summon_history_list;
    }

    public SummonGetSummonHistoryData() { }
}

[MessagePackObject(true)]
public class SummonGetSummonListData
{
    public IEnumerable<SummonList> summon_list { get; set; }
    public IEnumerable<SummonList> chara_ssr_summon_list { get; set; }
    public IEnumerable<SummonList> dragon_ssr_summon_list { get; set; }
    public IEnumerable<SummonList> chara_ssr_update_summon_list { get; set; }
    public IEnumerable<SummonList> dragon_ssr_update_summon_list { get; set; }
    public IEnumerable<SummonList> campaign_summon_list { get; set; }
    public IEnumerable<SummonList> campaign_ssr_summon_list { get; set; }
    public IEnumerable<SummonList> platinum_summon_list { get; set; }
    public IEnumerable<SummonList> exclude_summon_list { get; set; }
    public AtgenCsSummonList cs_summon_list { get; set; }
    public IEnumerable<SummonTicketList> summon_ticket_list { get; set; }
    public IEnumerable<SummonPointList> summon_point_list { get; set; }

    public SummonGetSummonListData(
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
    )
    {
        this.summon_list = summon_list;
        this.chara_ssr_summon_list = chara_ssr_summon_list;
        this.dragon_ssr_summon_list = dragon_ssr_summon_list;
        this.chara_ssr_update_summon_list = chara_ssr_update_summon_list;
        this.dragon_ssr_update_summon_list = dragon_ssr_update_summon_list;
        this.campaign_summon_list = campaign_summon_list;
        this.campaign_ssr_summon_list = campaign_ssr_summon_list;
        this.platinum_summon_list = platinum_summon_list;
        this.exclude_summon_list = exclude_summon_list;
        this.cs_summon_list = cs_summon_list;
        this.summon_ticket_list = summon_ticket_list;
        this.summon_point_list = summon_point_list;
    }

    public SummonGetSummonListData() { }
}

[MessagePackObject(true)]
public class SummonGetSummonPointTradeData
{
    public IEnumerable<AtgenSummonPointTradeList> summon_point_trade_list { get; set; }
    public IEnumerable<SummonPointList> summon_point_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public SummonGetSummonPointTradeData(
        IEnumerable<AtgenSummonPointTradeList> summon_point_trade_list,
        IEnumerable<SummonPointList> summon_point_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.summon_point_trade_list = summon_point_trade_list;
        this.summon_point_list = summon_point_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public SummonGetSummonPointTradeData() { }
}

[MessagePackObject(true)]
public class SummonRequestData
{
    public IEnumerable<AtgenResultUnitList> result_unit_list { get; set; }
    public IEnumerable<AtgenResultPrizeList> result_prize_list { get; set; }
    public IEnumerable<int> presage_effect_list { get; set; }
    public int reversal_effect_index { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<SummonTicketList> summon_ticket_list { get; set; }
    public int result_summon_point { get; set; }
    public IEnumerable<UserSummonList> user_summon_list { get; set; }

    public SummonRequestData(
        IEnumerable<AtgenResultUnitList> result_unit_list,
        IEnumerable<AtgenResultPrizeList> result_prize_list,
        IEnumerable<int> presage_effect_list,
        int reversal_effect_index,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<SummonTicketList> summon_ticket_list,
        int result_summon_point,
        IEnumerable<UserSummonList> user_summon_list
    )
    {
        this.result_unit_list = result_unit_list;
        this.result_prize_list = result_prize_list;
        this.presage_effect_list = presage_effect_list;
        this.reversal_effect_index = reversal_effect_index;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.summon_ticket_list = summon_ticket_list;
        this.result_summon_point = result_summon_point;
        this.user_summon_list = user_summon_list;
    }

    public SummonRequestData() { }
}

[MessagePackObject(true)]
public class SummonSummonPointTradeData
{
    public IEnumerable<AtgenBuildEventRewardEntityList> exchange_entity_list { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public SummonSummonPointTradeData(
        IEnumerable<AtgenBuildEventRewardEntityList> exchange_entity_list,
        UpdateDataList update_data_list
    )
    {
        this.exchange_entity_list = exchange_entity_list;
        this.update_data_list = update_data_list;
    }

    public SummonSummonPointTradeData() { }
}

[MessagePackObject(true)]
public class TalismanSellData
{
    public DeleteDataList delete_data_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public TalismanSellData(
        DeleteDataList delete_data_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.delete_data_list = delete_data_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public TalismanSellData() { }
}

[MessagePackObject(true)]
public class TalismanSetLockData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public TalismanSetLockData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public TalismanSetLockData() { }
}

[MessagePackObject(true)]
public class TimeAttackRankingGetDataData
{
    public IEnumerable<RankingTierRewardList> ranking_tier_reward_list { get; set; }

    public TimeAttackRankingGetDataData(IEnumerable<RankingTierRewardList> ranking_tier_reward_list)
    {
        this.ranking_tier_reward_list = ranking_tier_reward_list;
    }

    public TimeAttackRankingGetDataData() { }
}

[MessagePackObject(true)]
public class TimeAttackRankingReceiveTierRewardData
{
    public IEnumerable<RankingTierRewardList> ranking_tier_reward_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> ranking_tier_reward_entity_list { get; set; }

    public TimeAttackRankingReceiveTierRewardData(
        IEnumerable<RankingTierRewardList> ranking_tier_reward_list,
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenBuildEventRewardEntityList> ranking_tier_reward_entity_list
    )
    {
        this.ranking_tier_reward_list = ranking_tier_reward_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.ranking_tier_reward_entity_list = ranking_tier_reward_entity_list;
    }

    public TimeAttackRankingReceiveTierRewardData() { }
}

[MessagePackObject(true)]
public class ToolAuthData
{
    public ulong viewer_id { get; set; }
    public string session_id { get; set; }
    public string nonce { get; set; }

    public ToolAuthData(ulong viewer_id, string session_id, string nonce)
    {
        this.viewer_id = viewer_id;
        this.session_id = session_id;
        this.nonce = nonce;
    }

    public ToolAuthData() { }
}

[MessagePackObject(true)]
public class ToolGetMaintenanceTimeData
{
    public int maintenance_start_time { get; set; }
    public int maintenance_end_time { get; set; }

    public ToolGetMaintenanceTimeData(int maintenance_start_time, int maintenance_end_time)
    {
        this.maintenance_start_time = maintenance_start_time;
        this.maintenance_end_time = maintenance_end_time;
    }

    public ToolGetMaintenanceTimeData() { }
}

[MessagePackObject(true)]
public class ToolGetServiceStatusData
{
    public int service_status { get; set; }

    public ToolGetServiceStatusData(int service_status)
    {
        this.service_status = service_status;
    }

    public ToolGetServiceStatusData() { }
}

[MessagePackObject(true)]
public class ToolReauthData
{
    public ulong viewer_id { get; set; }
    public string session_id { get; set; }
    public string nonce { get; set; }

    public ToolReauthData(ulong viewer_id, string session_id, string nonce)
    {
        this.viewer_id = viewer_id;
        this.session_id = session_id;
        this.nonce = nonce;
    }

    public ToolReauthData() { }
}

[MessagePackObject(true)]
public class ToolSignupData
{
    public ulong viewer_id { get; set; }
    public DateTimeOffset servertime { get; set; }

    public ToolSignupData(ulong viewer_id, DateTimeOffset servertime)
    {
        this.viewer_id = viewer_id;
        this.servertime = servertime;
    }

    public ToolSignupData() { }
}

[MessagePackObject(true)]
public class TrackRecordUpdateProgressData
{
    public UpdateDataList update_data_list { get; set; }

    public TrackRecordUpdateProgressData(UpdateDataList update_data_list)
    {
        this.update_data_list = update_data_list;
    }

    public TrackRecordUpdateProgressData() { }
}

[MessagePackObject(true)]
public class TransitionTransitionByNAccountData
{
    public AtgenTransitionResultData transition_result_data { get; set; }

    public TransitionTransitionByNAccountData(AtgenTransitionResultData transition_result_data)
    {
        this.transition_result_data = transition_result_data;
    }

    public TransitionTransitionByNAccountData() { }
}

[MessagePackObject(true)]
public class TreasureTradeGetListAllData
{
    public IEnumerable<UserTreasureTradeList> user_treasure_trade_list { get; set; }
    public IEnumerable<TreasureTradeList> treasure_trade_list { get; set; }
    public IEnumerable<TreasureTradeList> treasure_trade_all_list { get; set; }
    public DmodeInfo dmode_info { get; set; }

    public TreasureTradeGetListAllData(
        IEnumerable<UserTreasureTradeList> user_treasure_trade_list,
        IEnumerable<TreasureTradeList> treasure_trade_list,
        IEnumerable<TreasureTradeList> treasure_trade_all_list,
        DmodeInfo dmode_info
    )
    {
        this.user_treasure_trade_list = user_treasure_trade_list;
        this.treasure_trade_list = treasure_trade_list;
        this.treasure_trade_all_list = treasure_trade_all_list;
        this.dmode_info = dmode_info;
    }

    public TreasureTradeGetListAllData() { }
}

[MessagePackObject(true)]
public class TreasureTradeGetListData
{
    public IEnumerable<UserTreasureTradeList> user_treasure_trade_list { get; set; }
    public IEnumerable<TreasureTradeList> treasure_trade_list { get; set; }
    public IEnumerable<TreasureTradeList> treasure_trade_all_list { get; set; }
    public DmodeInfo dmode_info { get; set; }

    public TreasureTradeGetListData(
        IEnumerable<UserTreasureTradeList> user_treasure_trade_list,
        IEnumerable<TreasureTradeList> treasure_trade_list,
        IEnumerable<TreasureTradeList> treasure_trade_all_list,
        DmodeInfo dmode_info
    )
    {
        this.user_treasure_trade_list = user_treasure_trade_list;
        this.treasure_trade_list = treasure_trade_list;
        this.treasure_trade_all_list = treasure_trade_all_list;
        this.dmode_info = dmode_info;
    }

    public TreasureTradeGetListData() { }
}

[MessagePackObject(true)]
public class TreasureTradeTradeData
{
    public IEnumerable<UserTreasureTradeList> user_treasure_trade_list { get; set; }
    public IEnumerable<TreasureTradeList> treasure_trade_list { get; set; }
    public IEnumerable<TreasureTradeList> treasure_trade_all_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public DeleteDataList delete_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public TreasureTradeTradeData(
        IEnumerable<UserTreasureTradeList> user_treasure_trade_list,
        IEnumerable<TreasureTradeList> treasure_trade_list,
        IEnumerable<TreasureTradeList> treasure_trade_all_list,
        UpdateDataList update_data_list,
        DeleteDataList delete_data_list,
        EntityResult entity_result
    )
    {
        this.user_treasure_trade_list = user_treasure_trade_list;
        this.treasure_trade_list = treasure_trade_list;
        this.treasure_trade_all_list = treasure_trade_all_list;
        this.update_data_list = update_data_list;
        this.delete_data_list = delete_data_list;
        this.entity_result = entity_result;
    }

    public TreasureTradeTradeData() { }
}

[MessagePackObject(true)]
public class TutorialUpdateFlagsData
{
    public IEnumerable<int> tutorial_flag_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public TutorialUpdateFlagsData(
        IEnumerable<int> tutorial_flag_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.tutorial_flag_list = tutorial_flag_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public TutorialUpdateFlagsData() { }
}

[MessagePackObject(true)]
public class TutorialUpdateStepData
{
    public int step { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public TutorialUpdateStepData(
        int step,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.step = step;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public TutorialUpdateStepData() { }
}

[MessagePackObject(true)]
public class UpdateNamechangeData
{
    public string checked_name { get; set; }

    public UpdateNamechangeData(string checked_name)
    {
        this.checked_name = checked_name;
    }

    public UpdateNamechangeData() { }
}

[MessagePackObject(true)]
public class UpdateResetNewData
{
    public int result { get; set; }

    public UpdateResetNewData(int result)
    {
        this.result = result;
    }

    public UpdateResetNewData() { }
}

[MessagePackObject(true)]
public class UserGetNAccountInfoData
{
    public UpdateDataList update_data_list { get; set; }
    public AtgenNAccountInfo n_account_info { get; set; }

    public UserGetNAccountInfoData(
        UpdateDataList update_data_list,
        AtgenNAccountInfo n_account_info
    )
    {
        this.update_data_list = update_data_list;
        this.n_account_info = n_account_info;
    }

    public UserGetNAccountInfoData() { }
}

[MessagePackObject(true)]
public class UserGetWalletBalanceData
{
    public WalletBalance wallet_balance { get; set; }

    public UserGetWalletBalanceData(WalletBalance wallet_balance)
    {
        this.wallet_balance = wallet_balance;
    }

    public UserGetWalletBalanceData() { }
}

[MessagePackObject(true)]
public class UserLinkedNAccountData
{
    public UpdateDataList update_data_list { get; set; }

    public UserLinkedNAccountData(UpdateDataList update_data_list)
    {
        this.update_data_list = update_data_list;
    }

    public UserLinkedNAccountData() { }
}

[MessagePackObject(true)]
public class UserOptInSettingData
{
    public int is_optin { get; set; }

    public UserOptInSettingData(int is_optin)
    {
        this.is_optin = is_optin;
    }

    public UserOptInSettingData() { }
}

[MessagePackObject(true)]
public class UserRecoverStaminaByStoneData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public AtgenRecoverData recover_data { get; set; }

    public UserRecoverStaminaByStoneData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        AtgenRecoverData recover_data
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.recover_data = recover_data;
    }

    public UserRecoverStaminaByStoneData() { }
}

[MessagePackObject(true)]
public class UserWithdrawalData
{
    public int result { get; set; }

    public UserWithdrawalData(int result)
    {
        this.result = result;
    }

    public UserWithdrawalData() { }
}

[MessagePackObject(true)]
public class VersionGetResourceVersionData
{
    public string resource_version { get; set; }

    public VersionGetResourceVersionData(string resource_version)
    {
        this.resource_version = resource_version;
    }

    public VersionGetResourceVersionData() { }
}

[MessagePackObject(true)]
public class WalkerSendGiftMultipleData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public int is_favorite { get; set; }
    public IEnumerable<DragonRewardEntityList> return_gift_list { get; set; }
    public IEnumerable<RewardReliabilityList> reward_reliability_list { get; set; }
    public AtgenWalkerData walker_data { get; set; }

    public WalkerSendGiftMultipleData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        int is_favorite,
        IEnumerable<DragonRewardEntityList> return_gift_list,
        IEnumerable<RewardReliabilityList> reward_reliability_list,
        AtgenWalkerData walker_data
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.is_favorite = is_favorite;
        this.return_gift_list = return_gift_list;
        this.reward_reliability_list = reward_reliability_list;
        this.walker_data = walker_data;
    }

    public WalkerSendGiftMultipleData() { }
}

[MessagePackObject(true)]
public class WallFailData
{
    public int result { get; set; }
    public IEnumerable<UserSupportList> fail_helper_list { get; set; }
    public IEnumerable<AtgenHelperDetailList> fail_helper_detail_list { get; set; }
    public AtgenFailQuestDetail fail_quest_detail { get; set; }

    public WallFailData(
        int result,
        IEnumerable<UserSupportList> fail_helper_list,
        IEnumerable<AtgenHelperDetailList> fail_helper_detail_list,
        AtgenFailQuestDetail fail_quest_detail
    )
    {
        this.result = result;
        this.fail_helper_list = fail_helper_list;
        this.fail_helper_detail_list = fail_helper_detail_list;
        this.fail_quest_detail = fail_quest_detail;
    }

    public WallFailData() { }
}

[MessagePackObject(true)]
public class WallGetMonthlyRewardData
{
    public IEnumerable<AtgenUserWallRewardList> user_wall_reward_list { get; set; }

    public WallGetMonthlyRewardData(IEnumerable<AtgenUserWallRewardList> user_wall_reward_list)
    {
        this.user_wall_reward_list = user_wall_reward_list;
    }

    public WallGetMonthlyRewardData() { }
}

[MessagePackObject(true)]
public class WallGetWallClearPartyData
{
    public IEnumerable<PartySettingList> wall_clear_party_setting_list { get; set; }
    public IEnumerable<AtgenLostUnitList> lost_unit_list { get; set; }

    public WallGetWallClearPartyData(
        IEnumerable<PartySettingList> wall_clear_party_setting_list,
        IEnumerable<AtgenLostUnitList> lost_unit_list
    )
    {
        this.wall_clear_party_setting_list = wall_clear_party_setting_list;
        this.lost_unit_list = lost_unit_list;
    }

    public WallGetWallClearPartyData() { }
}

[MessagePackObject(true)]
public class WallReceiveMonthlyRewardData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> wall_monthly_reward_list { get; set; }
    public IEnumerable<AtgenUserWallRewardList> user_wall_reward_list { get; set; }
    public IEnumerable<AtgenMonthlyWallReceiveList> monthly_wall_receive_list { get; set; }

    public WallReceiveMonthlyRewardData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        IEnumerable<AtgenBuildEventRewardEntityList> wall_monthly_reward_list,
        IEnumerable<AtgenUserWallRewardList> user_wall_reward_list,
        IEnumerable<AtgenMonthlyWallReceiveList> monthly_wall_receive_list
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.wall_monthly_reward_list = wall_monthly_reward_list;
        this.user_wall_reward_list = user_wall_reward_list;
        this.monthly_wall_receive_list = monthly_wall_receive_list;
    }

    public WallReceiveMonthlyRewardData() { }
}

[MessagePackObject(true)]
public class WallRecordRecordData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public AtgenPlayWallDetail play_wall_detail { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> wall_clear_reward_list { get; set; }
    public AtgenWallDropReward wall_drop_reward { get; set; }
    public AtgenWallUnitInfo wall_unit_info { get; set; }

    public WallRecordRecordData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        AtgenPlayWallDetail play_wall_detail,
        IEnumerable<AtgenBuildEventRewardEntityList> wall_clear_reward_list,
        AtgenWallDropReward wall_drop_reward,
        AtgenWallUnitInfo wall_unit_info
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.play_wall_detail = play_wall_detail;
        this.wall_clear_reward_list = wall_clear_reward_list;
        this.wall_drop_reward = wall_drop_reward;
        this.wall_unit_info = wall_unit_info;
    }

    public WallRecordRecordData() { }
}

[MessagePackObject(true)]
public class WallSetWallClearPartyData
{
    public int result { get; set; }

    public WallSetWallClearPartyData(int result)
    {
        this.result = result;
    }

    public WallSetWallClearPartyData() { }
}

[MessagePackObject(true)]
public class WallStartStartAssignUnitData
{
    public IngameData ingame_data { get; set; }
    public IngameWallData ingame_wall_data { get; set; }
    public OddsInfo odds_info { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public WallStartStartAssignUnitData(
        IngameData ingame_data,
        IngameWallData ingame_wall_data,
        OddsInfo odds_info,
        UpdateDataList update_data_list
    )
    {
        this.ingame_data = ingame_data;
        this.ingame_wall_data = ingame_wall_data;
        this.odds_info = odds_info;
        this.update_data_list = update_data_list;
    }

    public WallStartStartAssignUnitData() { }
}

[MessagePackObject(true)]
public class WallStartStartData
{
    public IngameData ingame_data { get; set; }
    public IngameWallData ingame_wall_data { get; set; }
    public OddsInfo odds_info { get; set; }
    public UpdateDataList update_data_list { get; set; }

    public WallStartStartData(
        IngameData ingame_data,
        IngameWallData ingame_wall_data,
        OddsInfo odds_info,
        UpdateDataList update_data_list
    )
    {
        this.ingame_data = ingame_data;
        this.ingame_wall_data = ingame_wall_data;
        this.odds_info = odds_info;
        this.update_data_list = update_data_list;
    }

    public WallStartStartData() { }
}

[MessagePackObject(true)]
public class WeaponBodyBuildupPieceData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public WeaponBodyBuildupPieceData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public WeaponBodyBuildupPieceData() { }
}

[MessagePackObject(true)]
public class WeaponBodyCraftData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public WeaponBodyCraftData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public WeaponBodyCraftData() { }
}

[MessagePackObject(true)]
public class WeaponBuildupData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public DeleteDataList delete_data_list { get; set; }

    public WeaponBuildupData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        DeleteDataList delete_data_list
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.delete_data_list = delete_data_list;
    }

    public WeaponBuildupData() { }
}

[MessagePackObject(true)]
public class WeaponLimitBreakData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }
    public DeleteDataList delete_data_list { get; set; }

    public WeaponLimitBreakData(
        UpdateDataList update_data_list,
        EntityResult entity_result,
        DeleteDataList delete_data_list
    )
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
        this.delete_data_list = delete_data_list;
    }

    public WeaponLimitBreakData() { }
}

[MessagePackObject(true)]
public class WeaponResetPlusCountData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public WeaponResetPlusCountData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public WeaponResetPlusCountData() { }
}

[MessagePackObject(true)]
public class WeaponSellData
{
    public DeleteDataList delete_data_list { get; set; }
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public WeaponSellData(
        DeleteDataList delete_data_list,
        UpdateDataList update_data_list,
        EntityResult entity_result
    )
    {
        this.delete_data_list = delete_data_list;
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public WeaponSellData() { }
}

[MessagePackObject(true)]
public class WeaponSetLockData
{
    public UpdateDataList update_data_list { get; set; }
    public EntityResult entity_result { get; set; }

    public WeaponSetLockData(UpdateDataList update_data_list, EntityResult entity_result)
    {
        this.update_data_list = update_data_list;
        this.entity_result = entity_result;
    }

    public WeaponSetLockData() { }
}

[MessagePackObject(true)]
public class WebviewVersionUrlListData
{
    public IEnumerable<AtgenWebviewUrlList> webview_url_list { get; set; }

    public WebviewVersionUrlListData(IEnumerable<AtgenWebviewUrlList> webview_url_list)
    {
        this.webview_url_list = webview_url_list;
    }

    public WebviewVersionUrlListData() { }
}
