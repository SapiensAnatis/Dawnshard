using DragaliaAPI.Features.Missions;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Models.Generated;

[MessagePackObject(true)]
public class AbilityCrestBuildupPieceData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AbilityCrestBuildupPieceData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AbilityCrestBuildupPieceData() { }
}

[MessagePackObject(true)]
public class AbilityCrestBuildupPlusCountData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AbilityCrestBuildupPlusCountData(
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AbilityCrestBuildupPlusCountData() { }
}

[MessagePackObject(true)]
public class AbilityCrestGetAbilityCrestSetListData
{
    public IEnumerable<AbilityCrestSetList> AbilityCrestSetList { get; set; }

    public AbilityCrestGetAbilityCrestSetListData(
        IEnumerable<AbilityCrestSetList> abilityCrestSetList
    )
    {
        this.AbilityCrestSetList = abilityCrestSetList;
    }

    public AbilityCrestGetAbilityCrestSetListData() { }
}

[MessagePackObject(true)]
public class AbilityCrestResetPlusCountData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AbilityCrestResetPlusCountData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AbilityCrestResetPlusCountData() { }
}

[MessagePackObject(true)]
public class AbilityCrestSetAbilityCrestSetData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AbilityCrestSetAbilityCrestSetData(
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AbilityCrestSetAbilityCrestSetData() { }
}

[MessagePackObject(true)]
public class AbilityCrestSetFavoriteData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AbilityCrestSetFavoriteData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AbilityCrestSetFavoriteData() { }
}

[MessagePackObject(true)]
public class AbilityCrestTradeGetListData
{
    public IEnumerable<UserAbilityCrestTradeList> UserAbilityCrestTradeList { get; set; }
    public IEnumerable<AbilityCrestTradeList> AbilityCrestTradeList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AbilityCrestTradeGetListData(
        IEnumerable<UserAbilityCrestTradeList> userAbilityCrestTradeList,
        IEnumerable<AbilityCrestTradeList> abilityCrestTradeList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.UserAbilityCrestTradeList = userAbilityCrestTradeList;
        this.AbilityCrestTradeList = abilityCrestTradeList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AbilityCrestTradeGetListData() { }
}

[MessagePackObject(true)]
public class AbilityCrestTradeTradeData
{
    public IEnumerable<UserAbilityCrestTradeList> UserAbilityCrestTradeList { get; set; }
    public IEnumerable<AbilityCrestTradeList> AbilityCrestTradeList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AbilityCrestTradeTradeData(
        IEnumerable<UserAbilityCrestTradeList> userAbilityCrestTradeList,
        IEnumerable<AbilityCrestTradeList> abilityCrestTradeList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.UserAbilityCrestTradeList = userAbilityCrestTradeList;
        this.AbilityCrestTradeList = abilityCrestTradeList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AbilityCrestTradeTradeData() { }
}

[MessagePackObject(true)]
public class AbilityCrestUpdateAbilityCrestSetNameData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AbilityCrestUpdateAbilityCrestSetNameData(
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AbilityCrestUpdateAbilityCrestSetNameData() { }
}

[MessagePackObject(true)]
public class AlbumIndexData
{
    public IEnumerable<AtgenAlbumQuestPlayRecordList> AlbumQuestPlayRecordList { get; set; }
    public IEnumerable<AlbumDragonData> AlbumDragonList { get; set; }
    public AlbumPassiveNotice AlbumPassiveUpdateResult { get; set; }
    public IEnumerable<AtgenCharaHonorList> CharaHonorList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public AlbumIndexData(
        IEnumerable<AtgenAlbumQuestPlayRecordList> albumQuestPlayRecordList,
        IEnumerable<AlbumDragonData> albumDragonList,
        AlbumPassiveNotice albumPassiveUpdateResult,
        IEnumerable<AtgenCharaHonorList> charaHonorList,
        UpdateDataList updateDataList
    )
    {
        this.AlbumQuestPlayRecordList = albumQuestPlayRecordList;
        this.AlbumDragonList = albumDragonList;
        this.AlbumPassiveUpdateResult = albumPassiveUpdateResult;
        this.CharaHonorList = charaHonorList;
        this.UpdateDataList = updateDataList;
    }

    public AlbumIndexData() { }
}

[MessagePackObject(true)]
public class AmuletBuildupData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public DeleteDataList DeleteDataList { get; set; }

    public AmuletBuildupData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        DeleteDataList deleteDataList
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.DeleteDataList = deleteDataList;
    }

    public AmuletBuildupData() { }
}

[MessagePackObject(true)]
public class AmuletLimitBreakData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public DeleteDataList DeleteDataList { get; set; }

    public AmuletLimitBreakData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        DeleteDataList deleteDataList
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.DeleteDataList = deleteDataList;
    }

    public AmuletLimitBreakData() { }
}

[MessagePackObject(true)]
public class AmuletResetPlusCountData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AmuletResetPlusCountData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AmuletResetPlusCountData() { }
}

[MessagePackObject(true)]
public class AmuletSellData
{
    public DeleteDataList DeleteDataList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AmuletSellData(
        DeleteDataList deleteDataList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.DeleteDataList = deleteDataList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AmuletSellData() { }
}

[MessagePackObject(true)]
public class AmuletSetLockData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AmuletSetLockData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AmuletSetLockData() { }
}

[MessagePackObject(true)]
public class AmuletTradeGetListData
{
    public IEnumerable<UserAmuletTradeList> UserAmuletTradeList { get; set; }
    public IEnumerable<AmuletTradeList> AmuletTradeList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AmuletTradeGetListData(
        IEnumerable<UserAmuletTradeList> userAmuletTradeList,
        IEnumerable<AmuletTradeList> amuletTradeList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.UserAmuletTradeList = userAmuletTradeList;
        this.AmuletTradeList = amuletTradeList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AmuletTradeGetListData() { }
}

[MessagePackObject(true)]
public class AmuletTradeTradeData
{
    public IEnumerable<UserAmuletTradeList> UserAmuletTradeList { get; set; }
    public IEnumerable<AmuletTradeList> AmuletTradeList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public AmuletTradeTradeData(
        IEnumerable<UserAmuletTradeList> userAmuletTradeList,
        IEnumerable<AmuletTradeList> amuletTradeList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.UserAmuletTradeList = userAmuletTradeList;
        this.AmuletTradeList = amuletTradeList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public AmuletTradeTradeData() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventEntryData
{
    public BattleRoyalEventUserRecord BattleRoyalEventUserRecord { get; set; }
    public BattleRoyalCycleUserRecord BattleRoyalCycleUserRecord { get; set; }
    public IEnumerable<BattleRoyalEventItemList> BattleRoyalEventItemList { get; set; }
    public IEnumerable<EventCycleRewardList> EventCycleRewardList { get; set; }
    public IEnumerable<BattleRoyalCharaSkinList> BattleRoyalCharaSkinList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public BattleRoyalEventEntryData(
        BattleRoyalEventUserRecord battleRoyalEventUserRecord,
        BattleRoyalCycleUserRecord battleRoyalCycleUserRecord,
        IEnumerable<BattleRoyalEventItemList> battleRoyalEventItemList,
        IEnumerable<EventCycleRewardList> eventCycleRewardList,
        IEnumerable<BattleRoyalCharaSkinList> battleRoyalCharaSkinList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.BattleRoyalEventUserRecord = battleRoyalEventUserRecord;
        this.BattleRoyalCycleUserRecord = battleRoyalCycleUserRecord;
        this.BattleRoyalEventItemList = battleRoyalEventItemList;
        this.EventCycleRewardList = eventCycleRewardList;
        this.BattleRoyalCharaSkinList = battleRoyalCharaSkinList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public BattleRoyalEventEntryData() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventGetEventDataData
{
    public BattleRoyalEventUserRecord BattleRoyalEventUserRecord { get; set; }
    public BattleRoyalCycleUserRecord BattleRoyalCycleUserRecord { get; set; }
    public IEnumerable<BattleRoyalEventItemList> BattleRoyalEventItemList { get; set; }
    public IEnumerable<EventCycleRewardList> EventCycleRewardList { get; set; }
    public IEnumerable<BattleRoyalCharaSkinList> BattleRoyalCharaSkinList { get; set; }
    public IEnumerable<EventTradeList> EventTradeList { get; set; }

    public BattleRoyalEventGetEventDataData(
        BattleRoyalEventUserRecord battleRoyalEventUserRecord,
        BattleRoyalCycleUserRecord battleRoyalCycleUserRecord,
        IEnumerable<BattleRoyalEventItemList> battleRoyalEventItemList,
        IEnumerable<EventCycleRewardList> eventCycleRewardList,
        IEnumerable<BattleRoyalCharaSkinList> battleRoyalCharaSkinList,
        IEnumerable<EventTradeList> eventTradeList
    )
    {
        this.BattleRoyalEventUserRecord = battleRoyalEventUserRecord;
        this.BattleRoyalCycleUserRecord = battleRoyalCycleUserRecord;
        this.BattleRoyalEventItemList = battleRoyalEventItemList;
        this.EventCycleRewardList = eventCycleRewardList;
        this.BattleRoyalCharaSkinList = battleRoyalCharaSkinList;
        this.EventTradeList = eventTradeList;
    }

    public BattleRoyalEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventReceiveEventCyclePointRewardData
{
    public IEnumerable<EventCycleRewardList> EventCycleRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> EventCycleRewardEntityList { get; set; }

    public BattleRoyalEventReceiveEventCyclePointRewardData(
        IEnumerable<EventCycleRewardList> eventCycleRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenBuildEventRewardEntityList> eventCycleRewardEntityList
    )
    {
        this.EventCycleRewardList = eventCycleRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.EventCycleRewardEntityList = eventCycleRewardEntityList;
    }

    public BattleRoyalEventReceiveEventCyclePointRewardData() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventReleaseCharaSkinData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public BattleRoyalEventReleaseCharaSkinData(
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public BattleRoyalEventReleaseCharaSkinData() { }
}

[MessagePackObject(true)]
public class BattleRoyalFailData
{
    public int Result { get; set; }

    public BattleRoyalFailData(int result)
    {
        this.Result = result;
    }

    public BattleRoyalFailData() { }
}

[MessagePackObject(true)]
public class BattleRoyalGetBattleRoyalHistoryData
{
    public IEnumerable<AtgenBattleRoyalHistoryList> BattleRoyalHistoryList { get; set; }

    public BattleRoyalGetBattleRoyalHistoryData(
        IEnumerable<AtgenBattleRoyalHistoryList> battleRoyalHistoryList
    )
    {
        this.BattleRoyalHistoryList = battleRoyalHistoryList;
    }

    public BattleRoyalGetBattleRoyalHistoryData() { }
}

[MessagePackObject(true)]
public class BattleRoyalRecordRoyalRecordMultiData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public BattleRoyalResult BattleRoyalResult { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> EventCycleRewardEntityList { get; set; }

    public BattleRoyalRecordRoyalRecordMultiData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        BattleRoyalResult battleRoyalResult,
        IEnumerable<AtgenBuildEventRewardEntityList> eventCycleRewardEntityList
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.BattleRoyalResult = battleRoyalResult;
        this.EventCycleRewardEntityList = eventCycleRewardEntityList;
    }

    public BattleRoyalRecordRoyalRecordMultiData() { }
}

[MessagePackObject(true)]
public class BattleRoyalStartMultiData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public AtgenBattleRoyalData BattleRoyalData { get; set; }

    public BattleRoyalStartMultiData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        AtgenBattleRoyalData battleRoyalData
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.BattleRoyalData = battleRoyalData;
    }

    public BattleRoyalStartMultiData() { }
}

[MessagePackObject(true)]
public class BattleRoyalStartRoyalMultiData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public AtgenBattleRoyalData BattleRoyalData { get; set; }

    public BattleRoyalStartRoyalMultiData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        AtgenBattleRoyalData battleRoyalData
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.BattleRoyalData = battleRoyalData;
    }

    public BattleRoyalStartRoyalMultiData() { }
}

[MessagePackObject(true)]
public class BuildEventEntryData
{
    public BuildEventUserList BuildEventUserData { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsReceivableEventDailyBonus { get; set; }

    public BuildEventEntryData(
        BuildEventUserList buildEventUserData,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        bool isReceivableEventDailyBonus
    )
    {
        this.BuildEventUserData = buildEventUserData;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.IsReceivableEventDailyBonus = isReceivableEventDailyBonus;
    }

    public BuildEventEntryData() { }
}

[MessagePackObject(true)]
public class BuildEventGetEventDataData
{
    public BuildEventUserList BuildEventUserData { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsReceivableEventDailyBonus { get; set; }
    public IEnumerable<BuildEventRewardList> BuildEventRewardList { get; set; }
    public IEnumerable<EventTradeList> EventTradeList { get; set; }
    public AtgenEventFortData EventFortData { get; set; }

    public BuildEventGetEventDataData(
        BuildEventUserList buildEventUserData,
        bool isReceivableEventDailyBonus,
        IEnumerable<BuildEventRewardList> buildEventRewardList,
        IEnumerable<EventTradeList> eventTradeList,
        AtgenEventFortData eventFortData
    )
    {
        this.BuildEventUserData = buildEventUserData;
        this.IsReceivableEventDailyBonus = isReceivableEventDailyBonus;
        this.BuildEventRewardList = buildEventRewardList;
        this.EventTradeList = eventTradeList;
        this.EventFortData = eventFortData;
    }

    public BuildEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class BuildEventReceiveBuildPointRewardData
{
    public IEnumerable<BuildEventRewardList> BuildEventRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> BuildEventRewardEntityList { get; set; }

    public BuildEventReceiveBuildPointRewardData(
        IEnumerable<BuildEventRewardList> buildEventRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenBuildEventRewardEntityList> buildEventRewardEntityList
    )
    {
        this.BuildEventRewardList = buildEventRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.BuildEventRewardEntityList = buildEventRewardEntityList;
    }

    public BuildEventReceiveBuildPointRewardData() { }
}

[MessagePackObject(true)]
public class BuildEventReceiveDailyBonusData
{
    public IEnumerable<AtgenBuildEventRewardEntityList> EventDailyBonusList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public BuildEventReceiveDailyBonusData(
        IEnumerable<AtgenBuildEventRewardEntityList> eventDailyBonusList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.EventDailyBonusList = eventDailyBonusList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public BuildEventReceiveDailyBonusData() { }
}

[MessagePackObject(true)]
public class CartoonLatestData
{
    public AtgenLatest Latest { get; set; }

    public CartoonLatestData(AtgenLatest latest)
    {
        this.Latest = latest;
    }

    public CartoonLatestData() { }
}

[MessagePackObject(true)]
public class CastleStoryReadData
{
    public IEnumerable<AtgenBuildEventRewardEntityList> CastleStoryRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> DuplicateEntityList { get; set; }

    public CastleStoryReadData(
        IEnumerable<AtgenBuildEventRewardEntityList> castleStoryRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenDuplicateEntityList> duplicateEntityList
    )
    {
        this.CastleStoryRewardList = castleStoryRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.DuplicateEntityList = duplicateEntityList;
    }

    public CastleStoryReadData() { }
}

[MessagePackObject(true)]
public class CharaAwakeData
{
    public UpdateDataList UpdateDataList { get; set; }

    public CharaAwakeData(UpdateDataList updateDataList)
    {
        this.UpdateDataList = updateDataList;
    }

    public CharaAwakeData() { }
}

[MessagePackObject(true)]
public class CharaBuildupManaData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public CharaBuildupManaData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public CharaBuildupManaData() { }
}

[MessagePackObject(true)]
public class CharaBuildupPlatinumData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public CharaBuildupPlatinumData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public CharaBuildupPlatinumData() { }
}

[MessagePackObject(true)]
public class CharaBuildupData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public CharaBuildupData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public CharaBuildupData() { }
}

[MessagePackObject(true)]
public class CharaGetCharaUnitSetData
{
    public IEnumerable<CharaUnitSetList> CharaUnitSetList { get; set; }

    public CharaGetCharaUnitSetData(IEnumerable<CharaUnitSetList> charaUnitSetList)
    {
        this.CharaUnitSetList = charaUnitSetList;
    }

    public CharaGetCharaUnitSetData() { }
}

[MessagePackObject(true)]
public class CharaGetListData
{
    public IEnumerable<CharaList> CharaList { get; set; }

    public CharaGetListData(IEnumerable<CharaList> charaList)
    {
        this.CharaList = charaList;
    }

    public CharaGetListData() { }
}

[MessagePackObject(true)]
public class CharaLimitBreakAndBuildupManaData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public CharaLimitBreakAndBuildupManaData(
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public CharaLimitBreakAndBuildupManaData() { }
}

[MessagePackObject(true)]
public class CharaLimitBreakData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public CharaLimitBreakData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public CharaLimitBreakData() { }
}

[MessagePackObject(true)]
public class CharaResetPlusCountData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public CharaResetPlusCountData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public CharaResetPlusCountData() { }
}

[MessagePackObject(true)]
public class CharaSetCharaUnitSetData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public CharaSetCharaUnitSetData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public CharaSetCharaUnitSetData() { }
}

[MessagePackObject(true)]
public class CharaUnlockEditSkillData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public CharaUnlockEditSkillData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public CharaUnlockEditSkillData() { }
}

[MessagePackObject(true)]
public class Clb01EventEntryData
{
    public Clb01EventUserList Clb01EventUserData { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public Clb01EventEntryData(
        Clb01EventUserList clb01EventUserData,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.Clb01EventUserData = clb01EventUserData;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public Clb01EventEntryData() { }
}

[MessagePackObject(true)]
public class Clb01EventGetEventDataData
{
    public Clb01EventUserList Clb01EventUserData { get; set; }
    public IEnumerable<BuildEventRewardList> Clb01EventRewardList { get; set; }
    public IEnumerable<CharaFriendshipList> CharaFriendshipList { get; set; }

    public Clb01EventGetEventDataData(
        Clb01EventUserList clb01EventUserData,
        IEnumerable<BuildEventRewardList> clb01EventRewardList,
        IEnumerable<CharaFriendshipList> charaFriendshipList
    )
    {
        this.Clb01EventUserData = clb01EventUserData;
        this.Clb01EventRewardList = clb01EventRewardList;
        this.CharaFriendshipList = charaFriendshipList;
    }

    public Clb01EventGetEventDataData() { }
}

[MessagePackObject(true)]
public class Clb01EventReceiveClb01PointRewardData
{
    public IEnumerable<BuildEventRewardList> Clb01EventRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> Clb01EventRewardEntityList { get; set; }

    public Clb01EventReceiveClb01PointRewardData(
        IEnumerable<BuildEventRewardList> clb01EventRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenBuildEventRewardEntityList> clb01EventRewardEntityList
    )
    {
        this.Clb01EventRewardList = clb01EventRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.Clb01EventRewardEntityList = clb01EventRewardEntityList;
    }

    public Clb01EventReceiveClb01PointRewardData() { }
}

[MessagePackObject(true)]
public class CollectEventEntryData
{
    public CollectEventUserList CollectEventUserData { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public CollectEventEntryData(
        CollectEventUserList collectEventUserData,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.CollectEventUserData = collectEventUserData;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public CollectEventEntryData() { }
}

[MessagePackObject(true)]
public class CollectEventGetEventDataData
{
    public CollectEventUserList CollectEventUserData { get; set; }
    public IEnumerable<EventStoryList> EventStoryList { get; set; }

    public CollectEventGetEventDataData(
        CollectEventUserList collectEventUserData,
        IEnumerable<EventStoryList> eventStoryList
    )
    {
        this.CollectEventUserData = collectEventUserData;
        this.EventStoryList = eventStoryList;
    }

    public CollectEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class CombatEventEntryData
{
    public CombatEventUserList CombatEventUserData { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public CombatEventEntryData(
        CombatEventUserList combatEventUserData,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.CombatEventUserData = combatEventUserData;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public CombatEventEntryData() { }
}

[MessagePackObject(true)]
public class CombatEventGetEventDataData
{
    public CombatEventUserList CombatEventUserData { get; set; }
    public IEnumerable<EventTradeList> EventTradeList { get; set; }
    public IEnumerable<BuildEventRewardList> EventRewardList { get; set; }
    public IEnumerable<UserEventLocationRewardList> UserEventLocationRewardList { get; set; }

    public CombatEventGetEventDataData(
        CombatEventUserList combatEventUserData,
        IEnumerable<EventTradeList> eventTradeList,
        IEnumerable<BuildEventRewardList> eventRewardList,
        IEnumerable<UserEventLocationRewardList> userEventLocationRewardList
    )
    {
        this.CombatEventUserData = combatEventUserData;
        this.EventTradeList = eventTradeList;
        this.EventRewardList = eventRewardList;
        this.UserEventLocationRewardList = userEventLocationRewardList;
    }

    public CombatEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class CombatEventReceiveEventLocationRewardData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<UserEventLocationRewardList> UserEventLocationRewardList { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> EventLocationRewardEntityList { get; set; }

    public CombatEventReceiveEventLocationRewardData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<UserEventLocationRewardList> userEventLocationRewardList,
        IEnumerable<AtgenBuildEventRewardEntityList> eventLocationRewardEntityList
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.UserEventLocationRewardList = userEventLocationRewardList;
        this.EventLocationRewardEntityList = eventLocationRewardEntityList;
    }

    public CombatEventReceiveEventLocationRewardData() { }
}

[MessagePackObject(true)]
public class CombatEventReceiveEventPointRewardData
{
    public IEnumerable<BuildEventRewardList> EventRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> EventRewardEntityList { get; set; }

    public CombatEventReceiveEventPointRewardData(
        IEnumerable<BuildEventRewardList> eventRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenBuildEventRewardEntityList> eventRewardEntityList
    )
    {
        this.EventRewardList = eventRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.EventRewardEntityList = eventRewardEntityList;
    }

    public CombatEventReceiveEventPointRewardData() { }
}

[MessagePackObject(true)]
public class CraftAssembleData
{
    public UpdateDataList UpdateDataList { get; set; }
    public DeleteDataList DeleteDataList { get; set; }
    public SettingSupport SettingSupport { get; set; }

    public CraftAssembleData(
        UpdateDataList updateDataList,
        DeleteDataList deleteDataList,
        SettingSupport settingSupport
    )
    {
        this.UpdateDataList = updateDataList;
        this.DeleteDataList = deleteDataList;
        this.SettingSupport = settingSupport;
    }

    public CraftAssembleData() { }
}

[MessagePackObject(true)]
public class CraftCreateData
{
    public UpdateDataList UpdateDataList { get; set; }
    public DeleteDataList DeleteDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public CraftCreateData(
        UpdateDataList updateDataList,
        DeleteDataList deleteDataList,
        EntityResult entityResult
    )
    {
        this.UpdateDataList = updateDataList;
        this.DeleteDataList = deleteDataList;
        this.EntityResult = entityResult;
    }

    public CraftCreateData() { }
}

[MessagePackObject(true)]
public class CraftDisassembleData
{
    public UpdateDataList UpdateDataList { get; set; }
    public DeleteDataList DeleteDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public CraftDisassembleData(
        UpdateDataList updateDataList,
        DeleteDataList deleteDataList,
        EntityResult entityResult
    )
    {
        this.UpdateDataList = updateDataList;
        this.DeleteDataList = deleteDataList;
        this.EntityResult = entityResult;
    }

    public CraftDisassembleData() { }
}

[MessagePackObject(true)]
public class CraftResetNewData
{
    public UpdateDataList UpdateDataList { get; set; }

    public CraftResetNewData(UpdateDataList updateDataList)
    {
        this.UpdateDataList = updateDataList;
    }

    public CraftResetNewData() { }
}

[MessagePackObject(true)]
public class DeployGetDeployVersionData
{
    public string DeployHash { get; set; }

    public DeployGetDeployVersionData(string deployHash)
    {
        this.DeployHash = deployHash;
    }

    public DeployGetDeployVersionData() { }
}

[MessagePackObject(true)]
public class DmodeBuildupServitorPassiveData
{
    public IEnumerable<DmodeServitorPassiveList> DmodeServitorPassiveList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public DmodeBuildupServitorPassiveData(
        IEnumerable<DmodeServitorPassiveList> dmodeServitorPassiveList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.DmodeServitorPassiveList = dmodeServitorPassiveList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public DmodeBuildupServitorPassiveData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonFinishData
{
    public DungeonState DmodeDungeonState { get; set; }
    public DmodeIngameResult DmodeIngameResult { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public DmodeDungeonFinishData(
        DungeonState dmodeDungeonState,
        DmodeIngameResult dmodeIngameResult,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.DmodeDungeonState = dmodeDungeonState;
        this.DmodeIngameResult = dmodeIngameResult;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public DmodeDungeonFinishData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonFloorData
{
    public DungeonState DmodeDungeonState { get; set; }
    public DmodeFloorData DmodeFloorData { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public DmodeDungeonFloorData(
        DungeonState dmodeDungeonState,
        DmodeFloorData dmodeFloorData,
        UpdateDataList updateDataList
    )
    {
        this.DmodeDungeonState = dmodeDungeonState;
        this.DmodeFloorData = dmodeFloorData;
        this.UpdateDataList = updateDataList;
    }

    public DmodeDungeonFloorData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonFloorSkipData
{
    public DungeonState DmodeDungeonState { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public DmodeDungeonFloorSkipData(DungeonState dmodeDungeonState, UpdateDataList updateDataList)
    {
        this.DmodeDungeonState = dmodeDungeonState;
        this.UpdateDataList = updateDataList;
    }

    public DmodeDungeonFloorSkipData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonRestartData
{
    public DmodeIngameData DmodeIngameData { get; set; }
    public DungeonState DmodeDungeonState { get; set; }

    public DmodeDungeonRestartData(DmodeIngameData dmodeIngameData, DungeonState dmodeDungeonState)
    {
        this.DmodeIngameData = dmodeIngameData;
        this.DmodeDungeonState = dmodeDungeonState;
    }

    public DmodeDungeonRestartData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonStartData
{
    public DmodeIngameData DmodeIngameData { get; set; }
    public DungeonState DmodeDungeonState { get; set; }

    public DmodeDungeonStartData(DmodeIngameData dmodeIngameData, DungeonState dmodeDungeonState)
    {
        this.DmodeIngameData = dmodeIngameData;
        this.DmodeDungeonState = dmodeDungeonState;
    }

    public DmodeDungeonStartData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonSystemHaltData
{
    public DungeonState DmodeDungeonState { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public DmodeDungeonSystemHaltData(DungeonState dmodeDungeonState, UpdateDataList updateDataList)
    {
        this.DmodeDungeonState = dmodeDungeonState;
        this.UpdateDataList = updateDataList;
    }

    public DmodeDungeonSystemHaltData() { }
}

[MessagePackObject(true)]
public class DmodeDungeonUserHaltData
{
    public DungeonState DmodeDungeonState { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public DmodeDungeonUserHaltData(DungeonState dmodeDungeonState, UpdateDataList updateDataList)
    {
        this.DmodeDungeonState = dmodeDungeonState;
        this.UpdateDataList = updateDataList;
    }

    public DmodeDungeonUserHaltData() { }
}

[MessagePackObject(true)]
public class DmodeEntryData
{
    public DmodeInfo DmodeInfo { get; set; }
    public IEnumerable<DmodeCharaList> DmodeCharaList { get; set; }
    public IEnumerable<DmodeServitorPassiveList> DmodeServitorPassiveList { get; set; }
    public DmodeDungeonInfo DmodeDungeonInfo { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public DmodeEntryData(
        DmodeInfo dmodeInfo,
        IEnumerable<DmodeCharaList> dmodeCharaList,
        IEnumerable<DmodeServitorPassiveList> dmodeServitorPassiveList,
        DmodeDungeonInfo dmodeDungeonInfo,
        UpdateDataList updateDataList
    )
    {
        this.DmodeInfo = dmodeInfo;
        this.DmodeCharaList = dmodeCharaList;
        this.DmodeServitorPassiveList = dmodeServitorPassiveList;
        this.DmodeDungeonInfo = dmodeDungeonInfo;
        this.UpdateDataList = updateDataList;
    }

    public DmodeEntryData() { }
}

[MessagePackObject(true)]
public class DmodeExpeditionFinishData
{
    public DmodeIngameResult DmodeIngameResult { get; set; }
    public DmodeExpedition DmodeExpedition { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public DmodeExpeditionFinishData(
        DmodeIngameResult dmodeIngameResult,
        DmodeExpedition dmodeExpedition,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.DmodeIngameResult = dmodeIngameResult;
        this.DmodeExpedition = dmodeExpedition;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public DmodeExpeditionFinishData() { }
}

[MessagePackObject(true)]
public class DmodeExpeditionForceFinishData
{
    public DmodeIngameResult DmodeIngameResult { get; set; }
    public DmodeExpedition DmodeExpedition { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public DmodeExpeditionForceFinishData(
        DmodeIngameResult dmodeIngameResult,
        DmodeExpedition dmodeExpedition,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.DmodeIngameResult = dmodeIngameResult;
        this.DmodeExpedition = dmodeExpedition;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public DmodeExpeditionForceFinishData() { }
}

[MessagePackObject(true)]
public class DmodeExpeditionStartData
{
    public DmodeExpedition DmodeExpedition { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public DmodeExpeditionStartData(DmodeExpedition dmodeExpedition, UpdateDataList updateDataList)
    {
        this.DmodeExpedition = dmodeExpedition;
        this.UpdateDataList = updateDataList;
    }

    public DmodeExpeditionStartData() { }
}

[MessagePackObject(true)]
public class DmodeGetDataData
{
    public DmodeInfo DmodeInfo { get; set; }
    public IEnumerable<DmodeCharaList> DmodeCharaList { get; set; }
    public IEnumerable<DmodeServitorPassiveList> DmodeServitorPassiveList { get; set; }
    public DmodeDungeonInfo DmodeDungeonInfo { get; set; }
    public IEnumerable<DmodeStoryList> DmodeStoryList { get; set; }
    public DmodeExpedition DmodeExpedition { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    [MessagePackFormatter(typeof(DateTimeOffsetIntFormatter))]
    public DateTimeOffset CurrentServerTime { get; set; }

    public DmodeGetDataData(
        DmodeInfo dmodeInfo,
        IEnumerable<DmodeCharaList> dmodeCharaList,
        IEnumerable<DmodeServitorPassiveList> dmodeServitorPassiveList,
        DmodeDungeonInfo dmodeDungeonInfo,
        IEnumerable<DmodeStoryList> dmodeStoryList,
        DmodeExpedition dmodeExpedition,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        DateTimeOffset currentServerTime
    )
    {
        this.DmodeInfo = dmodeInfo;
        this.DmodeCharaList = dmodeCharaList;
        this.DmodeServitorPassiveList = dmodeServitorPassiveList;
        this.DmodeDungeonInfo = dmodeDungeonInfo;
        this.DmodeStoryList = dmodeStoryList;
        this.DmodeExpedition = dmodeExpedition;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.CurrentServerTime = currentServerTime;
    }

    public DmodeGetDataData() { }
}

[MessagePackObject(true)]
public class DmodeReadStoryData
{
    public IEnumerable<AtgenBuildEventRewardEntityList> DmodeStoryRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> DuplicateEntityList { get; set; }

    public DmodeReadStoryData(
        IEnumerable<AtgenBuildEventRewardEntityList> dmodeStoryRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenDuplicateEntityList> duplicateEntityList
    )
    {
        this.DmodeStoryRewardList = dmodeStoryRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.DuplicateEntityList = duplicateEntityList;
    }

    public DmodeReadStoryData() { }
}

[MessagePackObject(true)]
public class DragonBuildupData
{
    public UpdateDataList UpdateDataList { get; set; }
    public DeleteDataList DeleteDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public DragonBuildupData(
        UpdateDataList updateDataList,
        DeleteDataList deleteDataList,
        EntityResult entityResult
    )
    {
        this.UpdateDataList = updateDataList;
        this.DeleteDataList = deleteDataList;
        this.EntityResult = entityResult;
    }

    public DragonBuildupData() { }
}

[MessagePackObject(true)]
public class DragonBuyGiftToSendMultipleData
{
    public IEnumerable<AtgenShopGiftList> ShopGiftList { get; set; }
    public IEnumerable<AtgenDragonGiftRewardList> DragonGiftRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public int DragonContactFreeGiftCount { get; set; }

    public DragonBuyGiftToSendMultipleData(
        IEnumerable<AtgenShopGiftList> shopGiftList,
        IEnumerable<AtgenDragonGiftRewardList> dragonGiftRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        int dragonContactFreeGiftCount
    )
    {
        this.ShopGiftList = shopGiftList;
        this.DragonGiftRewardList = dragonGiftRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.DragonContactFreeGiftCount = dragonContactFreeGiftCount;
    }

    public DragonBuyGiftToSendMultipleData() { }
}

[MessagePackObject(true)]
public class DragonBuyGiftToSendData
{
    public IEnumerable<AtgenShopGiftList> ShopGiftList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsFavorite { get; set; }
    public IEnumerable<DragonRewardEntityList> ReturnGiftList { get; set; }
    public IEnumerable<RewardReliabilityList> RewardReliabilityList { get; set; }
    public int DragonContactFreeGiftCount { get; set; }

    public DragonBuyGiftToSendData(
        IEnumerable<AtgenShopGiftList> shopGiftList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        bool isFavorite,
        IEnumerable<DragonRewardEntityList> returnGiftList,
        IEnumerable<RewardReliabilityList> rewardReliabilityList,
        int dragonContactFreeGiftCount
    )
    {
        this.ShopGiftList = shopGiftList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.IsFavorite = isFavorite;
        this.ReturnGiftList = returnGiftList;
        this.RewardReliabilityList = rewardReliabilityList;
        this.DragonContactFreeGiftCount = dragonContactFreeGiftCount;
    }

    public DragonBuyGiftToSendData() { }
}

[MessagePackObject(true)]
public class DragonGetContactDataData
{
    public IEnumerable<AtgenShopGiftList> ShopGiftList { get; set; }

    public DragonGetContactDataData(IEnumerable<AtgenShopGiftList> shopGiftList)
    {
        this.ShopGiftList = shopGiftList;
    }

    public DragonGetContactDataData() { }
}

[MessagePackObject(true)]
public class DragonLimitBreakData
{
    public UpdateDataList UpdateDataList { get; set; }
    public DeleteDataList DeleteDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public DragonLimitBreakData(
        UpdateDataList updateDataList,
        DeleteDataList deleteDataList,
        EntityResult entityResult
    )
    {
        this.UpdateDataList = updateDataList;
        this.DeleteDataList = deleteDataList;
        this.EntityResult = entityResult;
    }

    public DragonLimitBreakData() { }
}

[MessagePackObject(true)]
public class DragonResetPlusCountData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public DragonResetPlusCountData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public DragonResetPlusCountData() { }
}

[MessagePackObject(true)]
public class DragonSellData
{
    public DeleteDataList DeleteDataList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public DragonSellData(
        DeleteDataList deleteDataList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.DeleteDataList = deleteDataList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public DragonSellData() { }
}

[MessagePackObject(true)]
public class DragonSendGiftMultipleData
{
    public UpdateDataList UpdateDataList { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsFavorite { get; set; }
    public IEnumerable<DragonRewardEntityList> ReturnGiftList { get; set; }
    public IEnumerable<RewardReliabilityList> RewardReliabilityList { get; set; }

    public DragonSendGiftMultipleData(
        UpdateDataList updateDataList,
        bool isFavorite,
        IEnumerable<DragonRewardEntityList> returnGiftList,
        IEnumerable<RewardReliabilityList> rewardReliabilityList
    )
    {
        this.UpdateDataList = updateDataList;
        this.IsFavorite = isFavorite;
        this.ReturnGiftList = returnGiftList;
        this.RewardReliabilityList = rewardReliabilityList;
    }

    public DragonSendGiftMultipleData() { }
}

[MessagePackObject(true)]
public class DragonSendGiftData
{
    public UpdateDataList UpdateDataList { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsFavorite { get; set; }
    public IEnumerable<DragonRewardEntityList> ReturnGiftList { get; set; }
    public IEnumerable<RewardReliabilityList> RewardReliabilityList { get; set; }

    public DragonSendGiftData(
        UpdateDataList updateDataList,
        bool isFavorite,
        IEnumerable<DragonRewardEntityList> returnGiftList,
        IEnumerable<RewardReliabilityList> rewardReliabilityList
    )
    {
        this.UpdateDataList = updateDataList;
        this.IsFavorite = isFavorite;
        this.ReturnGiftList = returnGiftList;
        this.RewardReliabilityList = rewardReliabilityList;
    }

    public DragonSendGiftData() { }
}

[MessagePackObject(true)]
public class DragonSetLockData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public DragonSetLockData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public DragonSetLockData() { }
}

[MessagePackObject(true)]
public class DreamAdventureClearData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public int Result { get; set; }

    public DreamAdventureClearData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        int result
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.Result = result;
    }

    public DreamAdventureClearData() { }
}

[MessagePackObject(true)]
public class DreamAdventurePlayData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public int Result { get; set; }

    public DreamAdventurePlayData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        int result
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.Result = result;
    }

    public DreamAdventurePlayData() { }
}

[MessagePackObject(true)]
public class DungeonFailData
{
    public int Result { get; set; }
    public IEnumerable<UserSupportList> FailHelperList { get; set; }
    public IEnumerable<AtgenHelperDetailList> FailHelperDetailList { get; set; }
    public AtgenFailQuestDetail FailQuestDetail { get; set; }

    public DungeonFailData(
        int result,
        IEnumerable<UserSupportList> failHelperList,
        IEnumerable<AtgenHelperDetailList> failHelperDetailList,
        AtgenFailQuestDetail failQuestDetail
    )
    {
        this.Result = result;
        this.FailHelperList = failHelperList;
        this.FailHelperDetailList = failHelperDetailList;
        this.FailQuestDetail = failQuestDetail;
    }

    public DungeonFailData() { }
}

[MessagePackObject(true)]
public class DungeonGetAreaOddsData
{
    public OddsInfo OddsInfo { get; set; }

    public DungeonGetAreaOddsData(OddsInfo oddsInfo)
    {
        this.OddsInfo = oddsInfo;
    }

    public DungeonGetAreaOddsData() { }
}

[MessagePackObject(true)]
public class DungeonReceiveQuestBonusData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public AtgenReceiveQuestBonus ReceiveQuestBonus { get; set; }

    public DungeonReceiveQuestBonusData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        AtgenReceiveQuestBonus receiveQuestBonus
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.ReceiveQuestBonus = receiveQuestBonus;
    }

    public DungeonReceiveQuestBonusData() { }
}

[MessagePackObject(true)]
public class DungeonRecordRecordMultiData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IngameResultData IngameResultData { get; set; }
    public TimeAttackRankingData TimeAttackRankingData { get; set; }
    public EventDamageRanking EventDamageRanking { get; set; }

    public DungeonRecordRecordMultiData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IngameResultData ingameResultData,
        TimeAttackRankingData timeAttackRankingData,
        EventDamageRanking eventDamageRanking
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.IngameResultData = ingameResultData;
        this.TimeAttackRankingData = timeAttackRankingData;
        this.EventDamageRanking = eventDamageRanking;
    }

    public DungeonRecordRecordMultiData() { }
}

[MessagePackObject(true)]
public class DungeonRecordRecordData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IngameResultData IngameResultData { get; set; }
    public TimeAttackRankingData TimeAttackRankingData { get; set; }
    public RepeatData RepeatData { get; set; }
    public EventDamageRanking EventDamageRanking { get; set; }

    public DungeonRecordRecordData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IngameResultData ingameResultData,
        TimeAttackRankingData timeAttackRankingData,
        RepeatData repeatData,
        EventDamageRanking eventDamageRanking
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.IngameResultData = ingameResultData;
        this.TimeAttackRankingData = timeAttackRankingData;
        this.RepeatData = repeatData;
        this.EventDamageRanking = eventDamageRanking;
    }

    public DungeonRecordRecordData() { }
}

[MessagePackObject(true)]
public class DungeonRetryData
{
    public int ContinueCount { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public DungeonRetryData(int continueCount, UpdateDataList updateDataList)
    {
        this.ContinueCount = continueCount;
        this.UpdateDataList = updateDataList;
    }

    public DungeonRetryData() { }
}

[MessagePackObject(true)]
public class DungeonSkipStartAssignUnitData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IngameResultData IngameResultData { get; set; }
    public TimeAttackRankingData TimeAttackRankingData { get; set; }

    public DungeonSkipStartAssignUnitData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IngameResultData ingameResultData,
        TimeAttackRankingData timeAttackRankingData
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.IngameResultData = ingameResultData;
        this.TimeAttackRankingData = timeAttackRankingData;
    }

    public DungeonSkipStartAssignUnitData() { }
}

[MessagePackObject(true)]
public class DungeonSkipStartMultipleQuestAssignUnitData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IngameResultData IngameResultData { get; set; }
    public TimeAttackRankingData TimeAttackRankingData { get; set; }

    public DungeonSkipStartMultipleQuestAssignUnitData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IngameResultData ingameResultData,
        TimeAttackRankingData timeAttackRankingData
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.IngameResultData = ingameResultData;
        this.TimeAttackRankingData = timeAttackRankingData;
    }

    public DungeonSkipStartMultipleQuestAssignUnitData() { }
}

[MessagePackObject(true)]
public class DungeonSkipStartMultipleQuestData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IngameResultData IngameResultData { get; set; }
    public TimeAttackRankingData TimeAttackRankingData { get; set; }

    public DungeonSkipStartMultipleQuestData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IngameResultData ingameResultData,
        TimeAttackRankingData timeAttackRankingData
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.IngameResultData = ingameResultData;
        this.TimeAttackRankingData = timeAttackRankingData;
    }

    public DungeonSkipStartMultipleQuestData() { }
}

[MessagePackObject(true)]
public class DungeonSkipStartData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IngameResultData IngameResultData { get; set; }
    public TimeAttackRankingData TimeAttackRankingData { get; set; }

    public DungeonSkipStartData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IngameResultData ingameResultData,
        TimeAttackRankingData timeAttackRankingData
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.IngameResultData = ingameResultData;
        this.TimeAttackRankingData = timeAttackRankingData;
    }

    public DungeonSkipStartData() { }
}

[MessagePackObject(true)]
public class DungeonStartStartAssignUnitData
{
    public IngameData IngameData { get; set; }
    public IngameQuestData IngameQuestData { get; set; }
    public OddsInfo OddsInfo { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public DungeonStartStartAssignUnitData(
        IngameData ingameData,
        IngameQuestData ingameQuestData,
        OddsInfo oddsInfo,
        UpdateDataList updateDataList
    )
    {
        this.IngameData = ingameData;
        this.IngameQuestData = ingameQuestData;
        this.OddsInfo = oddsInfo;
        this.UpdateDataList = updateDataList;
    }

    public DungeonStartStartAssignUnitData() { }
}

[MessagePackObject(true)]
public class DungeonStartStartMultiAssignUnitData
{
    public IngameData IngameData { get; set; }
    public IngameQuestData IngameQuestData { get; set; }
    public OddsInfo OddsInfo { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public DungeonStartStartMultiAssignUnitData(
        IngameData ingameData,
        IngameQuestData ingameQuestData,
        OddsInfo oddsInfo,
        UpdateDataList updateDataList
    )
    {
        this.IngameData = ingameData;
        this.IngameQuestData = ingameQuestData;
        this.OddsInfo = oddsInfo;
        this.UpdateDataList = updateDataList;
    }

    public DungeonStartStartMultiAssignUnitData() { }
}

[MessagePackObject(true)]
public class DungeonStartStartMultiData
{
    public IngameData IngameData { get; set; }
    public IngameQuestData IngameQuestData { get; set; }
    public OddsInfo OddsInfo { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public DungeonStartStartMultiData(
        IngameData ingameData,
        IngameQuestData ingameQuestData,
        OddsInfo oddsInfo,
        UpdateDataList updateDataList
    )
    {
        this.IngameData = ingameData;
        this.IngameQuestData = ingameQuestData;
        this.OddsInfo = oddsInfo;
        this.UpdateDataList = updateDataList;
    }

    public DungeonStartStartMultiData() { }
}

[MessagePackObject(true)]
public class DungeonStartStartData
{
    public IngameData IngameData { get; set; }
    public IngameQuestData IngameQuestData { get; set; }
    public OddsInfo OddsInfo { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public DungeonStartStartData(
        IngameData ingameData,
        IngameQuestData ingameQuestData,
        OddsInfo oddsInfo,
        UpdateDataList updateDataList
    )
    {
        this.IngameData = ingameData;
        this.IngameQuestData = ingameQuestData;
        this.OddsInfo = oddsInfo;
        this.UpdateDataList = updateDataList;
    }

    public DungeonStartStartData() { }
}

[MessagePackObject(true)]
public class EarnEventEntryData
{
    public EarnEventUserList EarnEventUserData { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public EarnEventEntryData(
        EarnEventUserList earnEventUserData,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.EarnEventUserData = earnEventUserData;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public EarnEventEntryData() { }
}

[MessagePackObject(true)]
public class EarnEventGetEventDataData
{
    public EarnEventUserList EarnEventUserData { get; set; }
    public IEnumerable<EventTradeList> EventTradeList { get; set; }
    public IEnumerable<BuildEventRewardList> EventRewardList { get; set; }

    public EarnEventGetEventDataData(
        EarnEventUserList earnEventUserData,
        IEnumerable<EventTradeList> eventTradeList,
        IEnumerable<BuildEventRewardList> eventRewardList
    )
    {
        this.EarnEventUserData = earnEventUserData;
        this.EventTradeList = eventTradeList;
        this.EventRewardList = eventRewardList;
    }

    public EarnEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class EarnEventReceiveEventPointRewardData
{
    public IEnumerable<BuildEventRewardList> EventRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> EventRewardEntityList { get; set; }

    public EarnEventReceiveEventPointRewardData(
        IEnumerable<BuildEventRewardList> eventRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenBuildEventRewardEntityList> eventRewardEntityList
    )
    {
        this.EventRewardList = eventRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.EventRewardEntityList = eventRewardEntityList;
    }

    public EarnEventReceiveEventPointRewardData() { }
}

[MessagePackObject(true)]
public class EmblemGetListData
{
    public IEnumerable<EmblemList> EmblemList { get; set; }

    public EmblemGetListData(IEnumerable<EmblemList> emblemList)
    {
        this.EmblemList = emblemList;
    }

    public EmblemGetListData() { }
}

[MessagePackObject(true)]
public class EmblemSetData
{
    public int Result { get; set; }

    public EmblemSetData(int result)
    {
        this.Result = result;
    }

    public EmblemSetData() { }
}

[MessagePackObject(true)]
public class EulaAgreeAgreeData
{
    public AtgenVersionHash VersionHash { get; set; }
    public int IsOptin { get; set; }

    public EulaAgreeAgreeData(AtgenVersionHash versionHash, int isOptin)
    {
        this.VersionHash = versionHash;
        this.IsOptin = isOptin;
    }

    public EulaAgreeAgreeData() { }
}

[MessagePackObject(true)]
public class EulaGetVersionListData
{
    public IEnumerable<AtgenVersionHash> VersionHashList { get; set; }

    public EulaGetVersionListData(IEnumerable<AtgenVersionHash> versionHashList)
    {
        this.VersionHashList = versionHashList;
    }

    public EulaGetVersionListData() { }
}

[MessagePackObject(true)]
public class EulaGetVersionData
{
    public AtgenVersionHash VersionHash { get; set; }
    public bool IsRequiredAgree { get; set; }
    public int AgreementStatus { get; set; }

    public EulaGetVersionData(
        AtgenVersionHash versionHash,
        bool isRequiredAgree,
        int agreementStatus
    )
    {
        this.VersionHash = versionHash;
        this.IsRequiredAgree = isRequiredAgree;
        this.AgreementStatus = agreementStatus;
    }

    public EulaGetVersionData() { }
}

[MessagePackObject(true)]
public class EventDamageGetTotalDamageHistoryData
{
    public IEnumerable<AtgenEventDamageHistoryList> EventDamageHistoryList { get; set; }

    public EventDamageGetTotalDamageHistoryData(
        IEnumerable<AtgenEventDamageHistoryList> eventDamageHistoryList
    )
    {
        this.EventDamageHistoryList = eventDamageHistoryList;
    }

    public EventDamageGetTotalDamageHistoryData() { }
}

[MessagePackObject(true)]
public class EventDamageReceiveDamageRewardData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenEventDamageRewardList> EventDamageRewardList { get; set; }

    public EventDamageReceiveDamageRewardData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenEventDamageRewardList> eventDamageRewardList
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.EventDamageRewardList = eventDamageRewardList;
    }

    public EventDamageReceiveDamageRewardData() { }
}

[MessagePackObject(true)]
public class EventStoryReadData
{
    public IEnumerable<AtgenBuildEventRewardEntityList> EventStoryRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public EventStoryReadData(
        IEnumerable<AtgenBuildEventRewardEntityList> eventStoryRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.EventStoryRewardList = eventStoryRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public EventStoryReadData() { }
}

[MessagePackObject(true)]
public class EventSummonExecData
{
    public AtgenBoxSummonResult BoxSummonResult { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public EventSummonExecData(
        AtgenBoxSummonResult boxSummonResult,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.BoxSummonResult = boxSummonResult;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public EventSummonExecData() { }
}

[MessagePackObject(true)]
public class EventSummonGetDataData
{
    public AtgenBoxSummonData BoxSummonData { get; set; }

    public EventSummonGetDataData(AtgenBoxSummonData boxSummonData)
    {
        this.BoxSummonData = boxSummonData;
    }

    public EventSummonGetDataData() { }
}

[MessagePackObject(true)]
public class EventSummonResetData
{
    public AtgenBoxSummonData BoxSummonData { get; set; }

    public EventSummonResetData(AtgenBoxSummonData boxSummonData)
    {
        this.BoxSummonData = boxSummonData;
    }

    public EventSummonResetData() { }
}

[MessagePackObject(true)]
public class EventTradeGetListData
{
    public IEnumerable<AtgenUserEventTradeList> UserEventTradeList { get; set; }
    public IEnumerable<EventTradeList> EventTradeList { get; set; }
    public IEnumerable<MaterialList> MaterialList { get; set; }
    public UserEventItemData UserEventItemData { get; set; }

    public EventTradeGetListData(
        IEnumerable<AtgenUserEventTradeList> userEventTradeList,
        IEnumerable<EventTradeList> eventTradeList,
        IEnumerable<MaterialList> materialList,
        UserEventItemData userEventItemData
    )
    {
        this.UserEventTradeList = userEventTradeList;
        this.EventTradeList = eventTradeList;
        this.MaterialList = materialList;
        this.UserEventItemData = userEventItemData;
    }

    public EventTradeGetListData() { }
}

[MessagePackObject(true)]
public class EventTradeTradeData
{
    public IEnumerable<AtgenUserEventTradeList> UserEventTradeList { get; set; }
    public IEnumerable<EventTradeList> EventTradeList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<MaterialList> MaterialList { get; set; }
    public UserEventItemData UserEventItemData { get; set; }

    public EventTradeTradeData(
        IEnumerable<AtgenUserEventTradeList> userEventTradeList,
        IEnumerable<EventTradeList> eventTradeList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<MaterialList> materialList,
        UserEventItemData userEventItemData
    )
    {
        this.UserEventTradeList = userEventTradeList;
        this.EventTradeList = eventTradeList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.MaterialList = materialList;
        this.UserEventItemData = userEventItemData;
    }

    public EventTradeTradeData() { }
}

[MessagePackObject(true)]
public class ExchangeGetUnitListData
{
    public IEnumerable<AtgenDuplicateEntityList> SelectUnitList { get; set; }

    public ExchangeGetUnitListData(IEnumerable<AtgenDuplicateEntityList> selectUnitList)
    {
        this.SelectUnitList = selectUnitList;
    }

    public ExchangeGetUnitListData() { }
}

[MessagePackObject(true)]
public class ExchangeSelectUnitData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public ExchangeSelectUnitData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public ExchangeSelectUnitData() { }
}

[MessagePackObject(true)]
public class ExHunterEventEntryData
{
    public ExHunterEventUserList ExHunterEventUserData { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public ExHunterEventEntryData(
        ExHunterEventUserList exHunterEventUserData,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.ExHunterEventUserData = exHunterEventUserData;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public ExHunterEventEntryData() { }
}

[MessagePackObject(true)]
public class ExHunterEventGetEventDataData
{
    public ExHunterEventUserList ExHunterEventUserData { get; set; }
    public IEnumerable<BuildEventRewardList> ExHunterEventRewardList { get; set; }
    public IEnumerable<EventTradeList> EventTradeList { get; set; }
    public IEnumerable<EventPassiveList> EventPassiveList { get; set; }

    public ExHunterEventGetEventDataData(
        ExHunterEventUserList exHunterEventUserData,
        IEnumerable<BuildEventRewardList> exHunterEventRewardList,
        IEnumerable<EventTradeList> eventTradeList,
        IEnumerable<EventPassiveList> eventPassiveList
    )
    {
        this.ExHunterEventUserData = exHunterEventUserData;
        this.ExHunterEventRewardList = exHunterEventRewardList;
        this.EventTradeList = eventTradeList;
        this.EventPassiveList = eventPassiveList;
    }

    public ExHunterEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class ExHunterEventReceiveExHunterPointRewardData
{
    public IEnumerable<BuildEventRewardList> ExHunterEventRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public ExHunterEventReceiveExHunterPointRewardData(
        IEnumerable<BuildEventRewardList> exHunterEventRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.ExHunterEventRewardList = exHunterEventRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public ExHunterEventReceiveExHunterPointRewardData() { }
}

[MessagePackObject(true)]
public class ExRushEventEntryData
{
    public ExRushEventUserList ExRushEventUserData { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public ExRushEventEntryData(
        ExRushEventUserList exRushEventUserData,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.ExRushEventUserData = exRushEventUserData;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public ExRushEventEntryData() { }
}

[MessagePackObject(true)]
public class ExRushEventGetEventDataData
{
    public ExRushEventUserList ExRushEventUserData { get; set; }
    public IEnumerable<CharaFriendshipList> CharaFriendshipList { get; set; }
    public IEnumerable<EventTradeList> EventTradeList { get; set; }

    public ExRushEventGetEventDataData(
        ExRushEventUserList exRushEventUserData,
        IEnumerable<CharaFriendshipList> charaFriendshipList,
        IEnumerable<EventTradeList> eventTradeList
    )
    {
        this.ExRushEventUserData = exRushEventUserData;
        this.CharaFriendshipList = charaFriendshipList;
        this.EventTradeList = eventTradeList;
    }

    public ExRushEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class FortAddCarpenterData
{
    public int Result { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public FortDetail FortDetail { get; set; }

    public FortAddCarpenterData(int result, UpdateDataList updateDataList, FortDetail fortDetail)
    {
        this.Result = result;
        this.UpdateDataList = updateDataList;
        this.FortDetail = fortDetail;
    }

    public FortAddCarpenterData() { }
}

[MessagePackObject(true)]
public class FortBuildAtOnceData
{
    public int Result { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public long BuildId { get; set; }
    public FortDetail FortDetail { get; set; }
    public FortBonusList FortBonusList { get; set; }
    public AtgenProductionRp ProductionRp { get; set; }
    public AtgenProductionRp ProductionDf { get; set; }
    public AtgenProductionRp ProductionSt { get; set; }

    public FortBuildAtOnceData(
        int result,
        UpdateDataList updateDataList,
        long buildId,
        FortDetail fortDetail,
        FortBonusList fortBonusList,
        AtgenProductionRp productionRp,
        AtgenProductionRp productionDf,
        AtgenProductionRp productionSt
    )
    {
        this.Result = result;
        this.UpdateDataList = updateDataList;
        this.BuildId = buildId;
        this.FortDetail = fortDetail;
        this.FortBonusList = fortBonusList;
        this.ProductionRp = productionRp;
        this.ProductionDf = productionDf;
        this.ProductionSt = productionSt;
    }

    public FortBuildAtOnceData() { }
}

[MessagePackObject(true)]
public class FortBuildCancelData
{
    public int Result { get; set; }
    public long BuildId { get; set; }
    public FortDetail FortDetail { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public FortBuildCancelData(
        int result,
        long buildId,
        FortDetail fortDetail,
        UpdateDataList updateDataList
    )
    {
        this.Result = result;
        this.BuildId = buildId;
        this.FortDetail = fortDetail;
        this.UpdateDataList = updateDataList;
    }

    public FortBuildCancelData() { }
}

[MessagePackObject(true)]
public class FortBuildEndData
{
    public int Result { get; set; }
    public long BuildId { get; set; }
    public FortBonusList FortBonusList { get; set; }
    public FortDetail FortDetail { get; set; }
    public AtgenProductionRp ProductionRp { get; set; }
    public AtgenProductionRp ProductionDf { get; set; }
    public AtgenProductionRp ProductionSt { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public FortBuildEndData(
        int result,
        long buildId,
        FortBonusList fortBonusList,
        FortDetail fortDetail,
        AtgenProductionRp productionRp,
        AtgenProductionRp productionDf,
        AtgenProductionRp productionSt,
        UpdateDataList updateDataList
    )
    {
        this.Result = result;
        this.BuildId = buildId;
        this.FortBonusList = fortBonusList;
        this.FortDetail = fortDetail;
        this.ProductionRp = productionRp;
        this.ProductionDf = productionDf;
        this.ProductionSt = productionSt;
        this.UpdateDataList = updateDataList;
    }

    public FortBuildEndData() { }
}

[MessagePackObject(true)]
public class FortBuildStartData
{
    public int Result { get; set; }
    public ulong BuildId { get; set; }
    public DateTimeOffset BuildStartDate { get; set; }
    public DateTimeOffset BuildEndDate { get; set; }
    public TimeSpan RemainTime { get; set; }
    public FortDetail FortDetail { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public FortBuildStartData(
        int result,
        ulong buildId,
        DateTimeOffset buildStartDate,
        DateTimeOffset buildEndDate,
        TimeSpan remainTime,
        FortDetail fortDetail,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.Result = result;
        this.BuildId = buildId;
        this.BuildStartDate = buildStartDate;
        this.BuildEndDate = buildEndDate;
        this.RemainTime = remainTime;
        this.FortDetail = fortDetail;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public FortBuildStartData() { }
}

[MessagePackObject(true)]
public class FortGetDataData
{
    public FortDetail FortDetail { get; set; }
    public IEnumerable<BuildList> BuildList { get; set; }
    public FortBonusList FortBonusList { get; set; }
    public AtgenProductionRp ProductionRp { get; set; }
    public AtgenProductionRp ProductionDf { get; set; }
    public AtgenProductionRp ProductionSt { get; set; }
    public int DragonContactFreeGiftCount { get; set; }

    [MessagePackFormatter(typeof(DateTimeOffsetIntFormatter))]
    public DateTimeOffset CurrentServerTime { get; set; }

    public FortGetDataData(
        FortDetail fortDetail,
        IEnumerable<BuildList> buildList,
        FortBonusList fortBonusList,
        AtgenProductionRp productionRp,
        AtgenProductionRp productionDf,
        AtgenProductionRp productionSt,
        int dragonContactFreeGiftCount,
        DateTimeOffset currentServerTime
    )
    {
        this.FortDetail = fortDetail;
        this.BuildList = buildList;
        this.FortBonusList = fortBonusList;
        this.ProductionRp = productionRp;
        this.ProductionDf = productionDf;
        this.ProductionSt = productionSt;
        this.DragonContactFreeGiftCount = dragonContactFreeGiftCount;
        this.CurrentServerTime = currentServerTime;
    }

    public FortGetDataData() { }
}

[MessagePackObject(true)]
public class FortGetMultiIncomeData
{
    public int Result { get; set; }
    public IEnumerable<AtgenHarvestBuildList> HarvestBuildList { get; set; }
    public IEnumerable<AtgenAddCoinList> AddCoinList { get; set; }
    public IEnumerable<AtgenAddStaminaList> AddStaminaList { get; set; }
    public int IsOverCoin { get; set; }
    public int IsOverMaterial { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public FortGetMultiIncomeData(
        int result,
        IEnumerable<AtgenHarvestBuildList> harvestBuildList,
        IEnumerable<AtgenAddCoinList> addCoinList,
        IEnumerable<AtgenAddStaminaList> addStaminaList,
        int isOverCoin,
        int isOverMaterial,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.Result = result;
        this.HarvestBuildList = harvestBuildList;
        this.AddCoinList = addCoinList;
        this.AddStaminaList = addStaminaList;
        this.IsOverCoin = isOverCoin;
        this.IsOverMaterial = isOverMaterial;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public FortGetMultiIncomeData() { }
}

[MessagePackObject(true)]
public class FortLevelupAtOnceData
{
    public int Result { get; set; }
    public long BuildId { get; set; }
    public FortDetail FortDetail { get; set; }
    public FortBonusList FortBonusList { get; set; }
    public int CurrentFortLevel { get; set; }
    public int CurrentFortCraftLevel { get; set; }
    public AtgenProductionRp ProductionRp { get; set; }
    public AtgenProductionRp ProductionDf { get; set; }
    public AtgenProductionRp ProductionSt { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public FortLevelupAtOnceData(
        int result,
        long buildId,
        FortDetail fortDetail,
        FortBonusList fortBonusList,
        int currentFortLevel,
        int currentFortCraftLevel,
        AtgenProductionRp productionRp,
        AtgenProductionRp productionDf,
        AtgenProductionRp productionSt,
        UpdateDataList updateDataList
    )
    {
        this.Result = result;
        this.BuildId = buildId;
        this.FortDetail = fortDetail;
        this.FortBonusList = fortBonusList;
        this.CurrentFortLevel = currentFortLevel;
        this.CurrentFortCraftLevel = currentFortCraftLevel;
        this.ProductionRp = productionRp;
        this.ProductionDf = productionDf;
        this.ProductionSt = productionSt;
        this.UpdateDataList = updateDataList;
    }

    public FortLevelupAtOnceData() { }
}

[MessagePackObject(true)]
public class FortLevelupCancelData
{
    public int Result { get; set; }
    public long BuildId { get; set; }
    public FortDetail FortDetail { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public FortLevelupCancelData(
        int result,
        long buildId,
        FortDetail fortDetail,
        UpdateDataList updateDataList
    )
    {
        this.Result = result;
        this.BuildId = buildId;
        this.FortDetail = fortDetail;
        this.UpdateDataList = updateDataList;
    }

    public FortLevelupCancelData() { }
}

[MessagePackObject(true)]
public class FortLevelupEndData
{
    public int Result { get; set; }
    public long BuildId { get; set; }
    public FortDetail FortDetail { get; set; }
    public FortBonusList FortBonusList { get; set; }
    public int CurrentFortLevel { get; set; }
    public int CurrentFortCraftLevel { get; set; }
    public AtgenProductionRp ProductionRp { get; set; }
    public AtgenProductionRp ProductionDf { get; set; }
    public AtgenProductionRp ProductionSt { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public FortLevelupEndData(
        int result,
        long buildId,
        FortDetail fortDetail,
        FortBonusList fortBonusList,
        int currentFortLevel,
        int currentFortCraftLevel,
        AtgenProductionRp productionRp,
        AtgenProductionRp productionDf,
        AtgenProductionRp productionSt,
        UpdateDataList updateDataList
    )
    {
        this.Result = result;
        this.BuildId = buildId;
        this.FortDetail = fortDetail;
        this.FortBonusList = fortBonusList;
        this.CurrentFortLevel = currentFortLevel;
        this.CurrentFortCraftLevel = currentFortCraftLevel;
        this.ProductionRp = productionRp;
        this.ProductionDf = productionDf;
        this.ProductionSt = productionSt;
        this.UpdateDataList = updateDataList;
    }

    public FortLevelupEndData() { }
}

[MessagePackObject(true)]
public class FortLevelupStartData
{
    public int Result { get; set; }
    public DateTimeOffset LevelupStartDate { get; set; }
    public DateTimeOffset LevelupEndDate { get; set; }
    public TimeSpan RemainTime { get; set; }
    public long BuildId { get; set; }
    public FortDetail FortDetail { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public FortLevelupStartData(
        int result,
        DateTimeOffset levelupStartDate,
        DateTimeOffset levelupEndDate,
        TimeSpan remainTime,
        long buildId,
        FortDetail fortDetail,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.Result = result;
        this.LevelupStartDate = levelupStartDate;
        this.LevelupEndDate = levelupEndDate;
        this.RemainTime = remainTime;
        this.BuildId = buildId;
        this.FortDetail = fortDetail;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public FortLevelupStartData() { }
}

[MessagePackObject(true)]
public class FortMoveData
{
    public int Result { get; set; }
    public long BuildId { get; set; }
    public FortBonusList FortBonusList { get; set; }
    public AtgenProductionRp ProductionRp { get; set; }
    public AtgenProductionRp ProductionDf { get; set; }
    public AtgenProductionRp ProductionSt { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public FortMoveData(
        int result,
        long buildId,
        FortBonusList fortBonusList,
        AtgenProductionRp productionRp,
        AtgenProductionRp productionDf,
        AtgenProductionRp productionSt,
        UpdateDataList updateDataList
    )
    {
        this.Result = result;
        this.BuildId = buildId;
        this.FortBonusList = fortBonusList;
        this.ProductionRp = productionRp;
        this.ProductionDf = productionDf;
        this.ProductionSt = productionSt;
        this.UpdateDataList = updateDataList;
    }

    public FortMoveData() { }
}

[MessagePackObject(true)]
public class FortSetNewFortPlantData
{
    public int Result { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public FortSetNewFortPlantData(int result, UpdateDataList updateDataList)
    {
        this.Result = result;
        this.UpdateDataList = updateDataList;
    }

    public FortSetNewFortPlantData() { }
}

[MessagePackObject(true)]
public class FriendAllReplyDenyData
{
    public int Result { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public FriendAllReplyDenyData(
        int result,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.Result = result;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public FriendAllReplyDenyData() { }
}

[MessagePackObject(true)]
public class FriendApplyListData
{
    public int Result { get; set; }
    public IEnumerable<UserSupportList> FriendApply { get; set; }
    public IEnumerable<ulong> NewApplyViewerIdList { get; set; }

    public FriendApplyListData(
        int result,
        IEnumerable<UserSupportList> friendApply,
        IEnumerable<ulong> newApplyViewerIdList
    )
    {
        this.Result = result;
        this.FriendApply = friendApply;
        this.NewApplyViewerIdList = newApplyViewerIdList;
    }

    public FriendApplyListData() { }
}

[MessagePackObject(true)]
public class FriendAutoSearchData
{
    public int Result { get; set; }
    public IEnumerable<UserSupportList> SearchList { get; set; }

    public FriendAutoSearchData(int result, IEnumerable<UserSupportList> searchList)
    {
        this.Result = result;
        this.SearchList = searchList;
    }

    public FriendAutoSearchData() { }
}

[MessagePackObject(true)]
public class FriendDeleteData
{
    public int Result { get; set; }

    public FriendDeleteData(int result)
    {
        this.Result = result;
    }

    public FriendDeleteData() { }
}

[MessagePackObject(true)]
public class FriendFriendIndexData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public int FriendCount { get; set; }

    public FriendFriendIndexData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        int friendCount
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.FriendCount = friendCount;
    }

    public FriendFriendIndexData() { }
}

[MessagePackObject(true)]
public class FriendFriendListData
{
    public int Result { get; set; }
    public IEnumerable<UserSupportList> FriendList { get; set; }
    public IEnumerable<ulong> NewFriendViewerIdList { get; set; }

    public FriendFriendListData(
        int result,
        IEnumerable<UserSupportList> friendList,
        IEnumerable<ulong> newFriendViewerIdList
    )
    {
        this.Result = result;
        this.FriendList = friendList;
        this.NewFriendViewerIdList = newFriendViewerIdList;
    }

    public FriendFriendListData() { }
}

[MessagePackObject(true)]
public class FriendGetSupportCharaDetailData
{
    public AtgenSupportUserDataDetail SupportUserDataDetail { get; set; }

    public FriendGetSupportCharaDetailData(AtgenSupportUserDataDetail supportUserDataDetail)
    {
        this.SupportUserDataDetail = supportUserDataDetail;
    }

    public FriendGetSupportCharaDetailData() { }
}

[MessagePackObject(true)]
public class FriendGetSupportCharaData
{
    public int Result { get; set; }
    public SettingSupport SettingSupport { get; set; }

    public FriendGetSupportCharaData(int result, SettingSupport settingSupport)
    {
        this.Result = result;
        this.SettingSupport = settingSupport;
    }

    public FriendGetSupportCharaData() { }
}

[MessagePackObject(true)]
public class FriendIdSearchData
{
    public int Result { get; set; }
    public UserSupportList SearchUser { get; set; }

    public FriendIdSearchData(int result, UserSupportList searchUser)
    {
        this.Result = result;
        this.SearchUser = searchUser;
    }

    public FriendIdSearchData() { }
}

[MessagePackObject(true)]
public class FriendReplyData
{
    public int Result { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public FriendReplyData(int result, UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.Result = result;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public FriendReplyData() { }
}

[MessagePackObject(true)]
public class FriendRequestCancelData
{
    public int Result { get; set; }

    public FriendRequestCancelData(int result)
    {
        this.Result = result;
    }

    public FriendRequestCancelData() { }
}

[MessagePackObject(true)]
public class FriendRequestListData
{
    public int Result { get; set; }
    public IEnumerable<UserSupportList> RequestList { get; set; }

    public FriendRequestListData(int result, IEnumerable<UserSupportList> requestList)
    {
        this.Result = result;
        this.RequestList = requestList;
    }

    public FriendRequestListData() { }
}

[MessagePackObject(true)]
public class FriendRequestData
{
    public int Result { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public FriendRequestData(int result, UpdateDataList updateDataList)
    {
        this.Result = result;
        this.UpdateDataList = updateDataList;
    }

    public FriendRequestData() { }
}

[MessagePackObject(true)]
public class FriendSetSupportCharaData
{
    public int Result { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public SettingSupport SettingSupport { get; set; }

    public FriendSetSupportCharaData(
        int result,
        UpdateDataList updateDataList,
        SettingSupport settingSupport
    )
    {
        this.Result = result;
        this.UpdateDataList = updateDataList;
        this.SettingSupport = settingSupport;
    }

    public FriendSetSupportCharaData() { }
}

[MessagePackObject(true)]
public class GuildChatGetNewMessageListData
{
    public IEnumerable<GuildChatMessageList> GuildChatMessageList { get; set; }
    public int PollingInterval { get; set; }

    public GuildChatGetNewMessageListData(
        IEnumerable<GuildChatMessageList> guildChatMessageList,
        int pollingInterval
    )
    {
        this.GuildChatMessageList = guildChatMessageList;
        this.PollingInterval = pollingInterval;
    }

    public GuildChatGetNewMessageListData() { }
}

[MessagePackObject(true)]
public class GuildChatGetOldMessageListData
{
    public IEnumerable<GuildChatMessageList> GuildChatMessageList { get; set; }

    public GuildChatGetOldMessageListData(IEnumerable<GuildChatMessageList> guildChatMessageList)
    {
        this.GuildChatMessageList = guildChatMessageList;
    }

    public GuildChatGetOldMessageListData() { }
}

[MessagePackObject(true)]
public class GuildChatPostMessageStampData
{
    public IEnumerable<GuildChatMessageList> GuildChatMessageList { get; set; }
    public int PollingInterval { get; set; }

    public GuildChatPostMessageStampData(
        IEnumerable<GuildChatMessageList> guildChatMessageList,
        int pollingInterval
    )
    {
        this.GuildChatMessageList = guildChatMessageList;
        this.PollingInterval = pollingInterval;
    }

    public GuildChatPostMessageStampData() { }
}

[MessagePackObject(true)]
public class GuildChatPostMessageTextData
{
    public IEnumerable<GuildChatMessageList> GuildChatMessageList { get; set; }
    public int PollingInterval { get; set; }

    public GuildChatPostMessageTextData(
        IEnumerable<GuildChatMessageList> guildChatMessageList,
        int pollingInterval
    )
    {
        this.GuildChatMessageList = guildChatMessageList;
        this.PollingInterval = pollingInterval;
    }

    public GuildChatPostMessageTextData() { }
}

[MessagePackObject(true)]
public class GuildChatPostReportData
{
    public int Result { get; set; }

    public GuildChatPostReportData(int result)
    {
        this.Result = result;
    }

    public GuildChatPostReportData() { }
}

[MessagePackObject(true)]
public class GuildDisbandData
{
    public UpdateDataList UpdateDataList { get; set; }

    public GuildDisbandData(UpdateDataList updateDataList)
    {
        this.UpdateDataList = updateDataList;
    }

    public GuildDisbandData() { }
}

[MessagePackObject(true)]
public class GuildDropMemberData
{
    public IEnumerable<GuildMemberList> GuildMemberList { get; set; }

    public GuildDropMemberData(IEnumerable<GuildMemberList> guildMemberList)
    {
        this.GuildMemberList = guildMemberList;
    }

    public GuildDropMemberData() { }
}

[MessagePackObject(true)]
public class GuildEstablishData
{
    public UpdateDataList UpdateDataList { get; set; }
    public IEnumerable<GuildMemberList> GuildMemberList { get; set; }

    public GuildEstablishData(
        UpdateDataList updateDataList,
        IEnumerable<GuildMemberList> guildMemberList
    )
    {
        this.UpdateDataList = updateDataList;
        this.GuildMemberList = guildMemberList;
    }

    public GuildEstablishData() { }
}

[MessagePackObject(true)]
public class GuildGetGuildApplyDataData
{
    public IEnumerable<GuildApplyList> GuildApplyList { get; set; }

    public GuildGetGuildApplyDataData(IEnumerable<GuildApplyList> guildApplyList)
    {
        this.GuildApplyList = guildApplyList;
    }

    public GuildGetGuildApplyDataData() { }
}

[MessagePackObject(true)]
public class GuildGetGuildMemberDataData
{
    public IEnumerable<GuildMemberList> GuildMemberList { get; set; }

    public GuildGetGuildMemberDataData(IEnumerable<GuildMemberList> guildMemberList)
    {
        this.GuildMemberList = guildMemberList;
    }

    public GuildGetGuildMemberDataData() { }
}

[MessagePackObject(true)]
public class GuildIndexData
{
    public UpdateDataList UpdateDataList { get; set; }
    public IEnumerable<GuildMemberList> GuildMemberList { get; set; }
    public IEnumerable<GuildChatMessageList> GuildChatMessageList { get; set; }
    public IEnumerable<GuildApplyList> GuildApplyList { get; set; }
    public int IsUpdateGuildPositionType { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> AttendBonusList { get; set; }
    public int PollingInterval { get; set; }
    public int CurrentServerTime { get; set; }
    public IEnumerable<GuildInviteSendList> GuildInviteSendList { get; set; }
    public int GuildInviteReceiveCount { get; set; }

    public GuildIndexData(
        UpdateDataList updateDataList,
        IEnumerable<GuildMemberList> guildMemberList,
        IEnumerable<GuildChatMessageList> guildChatMessageList,
        IEnumerable<GuildApplyList> guildApplyList,
        int isUpdateGuildPositionType,
        IEnumerable<AtgenBuildEventRewardEntityList> attendBonusList,
        int pollingInterval,
        int currentServerTime,
        IEnumerable<GuildInviteSendList> guildInviteSendList,
        int guildInviteReceiveCount
    )
    {
        this.UpdateDataList = updateDataList;
        this.GuildMemberList = guildMemberList;
        this.GuildChatMessageList = guildChatMessageList;
        this.GuildApplyList = guildApplyList;
        this.IsUpdateGuildPositionType = isUpdateGuildPositionType;
        this.AttendBonusList = attendBonusList;
        this.PollingInterval = pollingInterval;
        this.CurrentServerTime = currentServerTime;
        this.GuildInviteSendList = guildInviteSendList;
        this.GuildInviteReceiveCount = guildInviteReceiveCount;
    }

    public GuildIndexData() { }
}

[MessagePackObject(true)]
public class GuildInviteGetGuildInviteReceiveDataData
{
    public IEnumerable<GuildInviteReceiveList> GuildInviteReceiveList { get; set; }

    public GuildInviteGetGuildInviteReceiveDataData(
        IEnumerable<GuildInviteReceiveList> guildInviteReceiveList
    )
    {
        this.GuildInviteReceiveList = guildInviteReceiveList;
    }

    public GuildInviteGetGuildInviteReceiveDataData() { }
}

[MessagePackObject(true)]
public class GuildInviteGetGuildInviteSendDataData
{
    public IEnumerable<GuildInviteSendList> GuildInviteSendList { get; set; }

    public GuildInviteGetGuildInviteSendDataData(
        IEnumerable<GuildInviteSendList> guildInviteSendList
    )
    {
        this.GuildInviteSendList = guildInviteSendList;
    }

    public GuildInviteGetGuildInviteSendDataData() { }
}

[MessagePackObject(true)]
public class GuildInviteInviteCancelData
{
    public IEnumerable<GuildInviteSendList> GuildInviteSendList { get; set; }

    public GuildInviteInviteCancelData(IEnumerable<GuildInviteSendList> guildInviteSendList)
    {
        this.GuildInviteSendList = guildInviteSendList;
    }

    public GuildInviteInviteCancelData() { }
}

[MessagePackObject(true)]
public class GuildInviteInviteReplyAllDenyData
{
    public UpdateDataList UpdateDataList { get; set; }
    public IEnumerable<GuildInviteReceiveList> GuildInviteReceiveList { get; set; }

    public GuildInviteInviteReplyAllDenyData(
        UpdateDataList updateDataList,
        IEnumerable<GuildInviteReceiveList> guildInviteReceiveList
    )
    {
        this.UpdateDataList = updateDataList;
        this.GuildInviteReceiveList = guildInviteReceiveList;
    }

    public GuildInviteInviteReplyAllDenyData() { }
}

[MessagePackObject(true)]
public class GuildInviteInviteReplyData
{
    public UpdateDataList UpdateDataList { get; set; }
    public IEnumerable<GuildInviteReceiveList> GuildInviteReceiveList { get; set; }

    public GuildInviteInviteReplyData(
        UpdateDataList updateDataList,
        IEnumerable<GuildInviteReceiveList> guildInviteReceiveList
    )
    {
        this.UpdateDataList = updateDataList;
        this.GuildInviteReceiveList = guildInviteReceiveList;
    }

    public GuildInviteInviteReplyData() { }
}

[MessagePackObject(true)]
public class GuildInviteInviteSendData
{
    public IEnumerable<GuildInviteSendList> GuildInviteSendList { get; set; }

    public GuildInviteInviteSendData(IEnumerable<GuildInviteSendList> guildInviteSendList)
    {
        this.GuildInviteSendList = guildInviteSendList;
    }

    public GuildInviteInviteSendData() { }
}

[MessagePackObject(true)]
public class GuildJoinReplyAllDenyData
{
    public IEnumerable<GuildApplyList> GuildApplyList { get; set; }

    public GuildJoinReplyAllDenyData(IEnumerable<GuildApplyList> guildApplyList)
    {
        this.GuildApplyList = guildApplyList;
    }

    public GuildJoinReplyAllDenyData() { }
}

[MessagePackObject(true)]
public class GuildJoinReplyData
{
    public IEnumerable<GuildMemberList> GuildMemberList { get; set; }
    public IEnumerable<GuildApplyList> GuildApplyList { get; set; }

    public GuildJoinReplyData(
        IEnumerable<GuildMemberList> guildMemberList,
        IEnumerable<GuildApplyList> guildApplyList
    )
    {
        this.GuildMemberList = guildMemberList;
        this.GuildApplyList = guildApplyList;
    }

    public GuildJoinReplyData() { }
}

[MessagePackObject(true)]
public class GuildJoinRequestCancelData
{
    public UpdateDataList UpdateDataList { get; set; }

    public GuildJoinRequestCancelData(UpdateDataList updateDataList)
    {
        this.UpdateDataList = updateDataList;
    }

    public GuildJoinRequestCancelData() { }
}

[MessagePackObject(true)]
public class GuildJoinRequestData
{
    public UpdateDataList UpdateDataList { get; set; }

    public GuildJoinRequestData(UpdateDataList updateDataList)
    {
        this.UpdateDataList = updateDataList;
    }

    public GuildJoinRequestData() { }
}

[MessagePackObject(true)]
public class GuildJoinData
{
    public UpdateDataList UpdateDataList { get; set; }
    public IEnumerable<GuildMemberList> GuildMemberList { get; set; }

    public GuildJoinData(
        UpdateDataList updateDataList,
        IEnumerable<GuildMemberList> guildMemberList
    )
    {
        this.UpdateDataList = updateDataList;
        this.GuildMemberList = guildMemberList;
    }

    public GuildJoinData() { }
}

[MessagePackObject(true)]
public class GuildPostReportData
{
    public int Result { get; set; }

    public GuildPostReportData(int result)
    {
        this.Result = result;
    }

    public GuildPostReportData() { }
}

[MessagePackObject(true)]
public class GuildResignData
{
    public UpdateDataList UpdateDataList { get; set; }

    public GuildResignData(UpdateDataList updateDataList)
    {
        this.UpdateDataList = updateDataList;
    }

    public GuildResignData() { }
}

[MessagePackObject(true)]
public class GuildSearchAutoSearchData
{
    public IEnumerable<GuildData> AutoSearchGuildList { get; set; }

    public GuildSearchAutoSearchData(IEnumerable<GuildData> autoSearchGuildList)
    {
        this.AutoSearchGuildList = autoSearchGuildList;
    }

    public GuildSearchAutoSearchData() { }
}

[MessagePackObject(true)]
public class GuildSearchGetGuildDetailData
{
    public IEnumerable<GuildData> SearchGuildList { get; set; }

    public GuildSearchGetGuildDetailData(IEnumerable<GuildData> searchGuildList)
    {
        this.SearchGuildList = searchGuildList;
    }

    public GuildSearchGetGuildDetailData() { }
}

[MessagePackObject(true)]
public class GuildSearchIdSearchData
{
    public IEnumerable<GuildData> SearchGuildList { get; set; }

    public GuildSearchIdSearchData(IEnumerable<GuildData> searchGuildList)
    {
        this.SearchGuildList = searchGuildList;
    }

    public GuildSearchIdSearchData() { }
}

[MessagePackObject(true)]
public class GuildSearchNameSearchData
{
    public IEnumerable<GuildData> SearchGuildList { get; set; }

    public GuildSearchNameSearchData(IEnumerable<GuildData> searchGuildList)
    {
        this.SearchGuildList = searchGuildList;
    }

    public GuildSearchNameSearchData() { }
}

[MessagePackObject(true)]
public class GuildUpdateGuildPositionTypeData
{
    public IEnumerable<GuildMemberList> GuildMemberList { get; set; }

    public GuildUpdateGuildPositionTypeData(IEnumerable<GuildMemberList> guildMemberList)
    {
        this.GuildMemberList = guildMemberList;
    }

    public GuildUpdateGuildPositionTypeData() { }
}

[MessagePackObject(true)]
public class GuildUpdateGuildSettingData
{
    public UpdateDataList UpdateDataList { get; set; }

    public GuildUpdateGuildSettingData(UpdateDataList updateDataList)
    {
        this.UpdateDataList = updateDataList;
    }

    public GuildUpdateGuildSettingData() { }
}

[MessagePackObject(true)]
public class GuildUpdateUserSettingData
{
    public UpdateDataList UpdateDataList { get; set; }

    public GuildUpdateUserSettingData(UpdateDataList updateDataList)
    {
        this.UpdateDataList = updateDataList;
    }

    public GuildUpdateUserSettingData() { }
}

[MessagePackObject(true)]
public class InquiryAggregationData
{
    public int Result { get; set; }

    public InquiryAggregationData(int result)
    {
        this.Result = result;
    }

    public InquiryAggregationData() { }
}

[MessagePackObject(true)]
public class InquiryDetailData
{
    public string OpinionId { get; set; }
    public int OpinionType { get; set; }
    public string OpinionText { get; set; }
    public IEnumerable<AtgenCommentList> CommentList { get; set; }
    public int OccurredAt { get; set; }
    public int CreatedAt { get; set; }

    public InquiryDetailData(
        string opinionId,
        int opinionType,
        string opinionText,
        IEnumerable<AtgenCommentList> commentList,
        int occurredAt,
        int createdAt
    )
    {
        this.OpinionId = opinionId;
        this.OpinionType = opinionType;
        this.OpinionText = opinionText;
        this.CommentList = commentList;
        this.OccurredAt = occurredAt;
        this.CreatedAt = createdAt;
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
    public IEnumerable<AtgenOpinionList> OpinionList { get; set; }
    public IEnumerable<AtgenOpinionTypeList> OpinionTypeList { get; set; }
    public IEnumerable<AtgenInquiryFaqList> InquiryFaqList { get; set; }

    public InquiryTopData(
        IEnumerable<AtgenOpinionList> opinionList,
        IEnumerable<AtgenOpinionTypeList> opinionTypeList,
        IEnumerable<AtgenInquiryFaqList> inquiryFaqList
    )
    {
        this.OpinionList = opinionList;
        this.OpinionTypeList = opinionTypeList;
        this.InquiryFaqList = inquiryFaqList;
    }

    public InquiryTopData() { }
}

[MessagePackObject(true)]
public class ItemGetListData
{
    public IEnumerable<ItemList> ItemList { get; set; }

    public ItemGetListData(IEnumerable<ItemList> itemList)
    {
        this.ItemList = itemList;
    }

    public ItemGetListData() { }
}

[MessagePackObject(true)]
public class ItemUseRecoveryStaminaData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public AtgenRecoverData RecoverData { get; set; }

    public ItemUseRecoveryStaminaData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        AtgenRecoverData recoverData
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.RecoverData = recoverData;
    }

    public ItemUseRecoveryStaminaData() { }
}

[MessagePackObject(true)]
public class LoginIndexData
{
    public IEnumerable<AtgenLoginBonusList> LoginBonusList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public AtgenSupportReward SupportReward { get; set; }
    public int DragonContactFreeGiftCount { get; set; }
    public IEnumerable<AtgenMonthlyWallReceiveList> MonthlyWallReceiveList { get; set; }
    public IEnumerable<AtgenLoginLotteryRewardList> LoginLotteryRewardList { get; set; }
    public AtgenPenaltyData PenaltyData { get; set; }
    public IEnumerable<AtgenExchangeSummomPointList> ExchangeSummomPointList { get; set; }
    public int BeforeExchangeSummonItemQuantity { get; set; }
    public DateTimeOffset ServerTime { get; set; }

    public LoginIndexData(
        IEnumerable<AtgenLoginBonusList> loginBonusList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        AtgenSupportReward supportReward,
        int dragonContactFreeGiftCount,
        IEnumerable<AtgenMonthlyWallReceiveList> monthlyWallReceiveList,
        IEnumerable<AtgenLoginLotteryRewardList> loginLotteryRewardList,
        AtgenPenaltyData penaltyData,
        IEnumerable<AtgenExchangeSummomPointList> exchangeSummomPointList,
        int beforeExchangeSummonItemQuantity,
        DateTimeOffset serverTime
    )
    {
        this.LoginBonusList = loginBonusList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.SupportReward = supportReward;
        this.DragonContactFreeGiftCount = dragonContactFreeGiftCount;
        this.MonthlyWallReceiveList = monthlyWallReceiveList;
        this.LoginLotteryRewardList = loginLotteryRewardList;
        this.PenaltyData = penaltyData;
        this.ExchangeSummomPointList = exchangeSummomPointList;
        this.BeforeExchangeSummonItemQuantity = beforeExchangeSummonItemQuantity;
        this.ServerTime = serverTime;
    }

    public LoginIndexData() { }
}

[MessagePackObject(true)]
public class LoginPenaltyConfirmData
{
    public int Result { get; set; }
    public AtgenPenaltyData PenaltyData { get; set; }

    public LoginPenaltyConfirmData(int result, AtgenPenaltyData penaltyData)
    {
        this.Result = result;
        this.PenaltyData = penaltyData;
    }

    public LoginPenaltyConfirmData() { }
}

[MessagePackObject(true)]
public class LoginVerifyJwsData { }

[MessagePackObject(true)]
public class LotteryGetOddsDataData
{
    public LotteryOddsRateList LotteryOddsRateList { get; set; }

    public LotteryGetOddsDataData(LotteryOddsRateList lotteryOddsRateList)
    {
        this.LotteryOddsRateList = lotteryOddsRateList;
    }

    public LotteryGetOddsDataData() { }
}

[MessagePackObject(true)]
public class LotteryLotteryExecData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenLotteryResultList> LotteryResultList { get; set; }

    public LotteryLotteryExecData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenLotteryResultList> lotteryResultList
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.LotteryResultList = lotteryResultList;
    }

    public LotteryLotteryExecData() { }
}

[MessagePackObject(true)]
public class LotteryResultData
{
    public IEnumerable<AtgenLotteryResultList> LotteryResultList { get; set; }

    public LotteryResultData(IEnumerable<AtgenLotteryResultList> lotteryResultList)
    {
        this.LotteryResultList = lotteryResultList;
    }

    public LotteryResultData() { }
}

[MessagePackObject(true)]
public class MaintenanceGetTextData
{
    public string MaintenanceText { get; set; }

    public MaintenanceGetTextData(string maintenanceText)
    {
        this.MaintenanceText = maintenanceText;
    }

    public MaintenanceGetTextData() { }
}

[MessagePackObject(true)]
public class MatchingCheckPenaltyUserData
{
    public int Result { get; set; }

    public MatchingCheckPenaltyUserData(int result)
    {
        this.Result = result;
    }

    public MatchingCheckPenaltyUserData() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomListByGuildData
{
    public IEnumerable<RoomList> RoomList { get; set; }

    public MatchingGetRoomListByGuildData(IEnumerable<RoomList> roomList)
    {
        this.RoomList = roomList;
    }

    public MatchingGetRoomListByGuildData() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomListByLocationData
{
    public IEnumerable<RoomList> RoomList { get; set; }

    public MatchingGetRoomListByLocationData(IEnumerable<RoomList> roomList)
    {
        this.RoomList = roomList;
    }

    public MatchingGetRoomListByLocationData() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomListByQuestIdData
{
    public IEnumerable<RoomList> RoomList { get; set; }

    public MatchingGetRoomListByQuestIdData(IEnumerable<RoomList> roomList)
    {
        this.RoomList = roomList;
    }

    public MatchingGetRoomListByQuestIdData() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomListData
{
    public IEnumerable<RoomList> RoomList { get; set; }
    public IEnumerable<RoomList> FriendRoomList { get; set; }
    public IEnumerable<RoomList> EventRoomList { get; set; }
    public IEnumerable<RoomList> EventFriendRoomList { get; set; }

    public MatchingGetRoomListData(
        IEnumerable<RoomList> roomList,
        IEnumerable<RoomList> friendRoomList,
        IEnumerable<RoomList> eventRoomList,
        IEnumerable<RoomList> eventFriendRoomList
    )
    {
        this.RoomList = roomList;
        this.FriendRoomList = friendRoomList;
        this.EventRoomList = eventRoomList;
        this.EventFriendRoomList = eventFriendRoomList;
    }

    public MatchingGetRoomListData() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomNameData
{
    public string RoomName { get; set; }
    public int QuestId { get; set; }
    public string ClusterName { get; set; }
    public RoomList RoomData { get; set; }
    public int IsFriend { get; set; }

    public MatchingGetRoomNameData(
        string roomName,
        int questId,
        string clusterName,
        RoomList roomData,
        int isFriend
    )
    {
        this.RoomName = roomName;
        this.QuestId = questId;
        this.ClusterName = clusterName;
        this.RoomData = roomData;
        this.IsFriend = isFriend;
    }

    public MatchingGetRoomNameData() { }
}

[MessagePackObject(true)]
public class MazeEventEntryData
{
    public MazeEventUserList MazeEventUserData { get; set; }

    public MazeEventEntryData(MazeEventUserList mazeEventUserData)
    {
        this.MazeEventUserData = mazeEventUserData;
    }

    public MazeEventEntryData() { }
}

[MessagePackObject(true)]
public class MazeEventGetEventDataData
{
    public MazeEventUserList MazeEventUserData { get; set; }
    public IEnumerable<BuildEventRewardList> MazeEventRewardList { get; set; }
    public IEnumerable<EventTradeList> EventTradeList { get; set; }

    public MazeEventGetEventDataData(
        MazeEventUserList mazeEventUserData,
        IEnumerable<BuildEventRewardList> mazeEventRewardList,
        IEnumerable<EventTradeList> eventTradeList
    )
    {
        this.MazeEventUserData = mazeEventUserData;
        this.MazeEventRewardList = mazeEventRewardList;
        this.EventTradeList = eventTradeList;
    }

    public MazeEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class MazeEventReceiveMazePointRewardData
{
    public IEnumerable<BuildEventRewardList> MazeEventRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> MazeEventRewardEntityList { get; set; }

    public MazeEventReceiveMazePointRewardData(
        IEnumerable<BuildEventRewardList> mazeEventRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenBuildEventRewardEntityList> mazeEventRewardEntityList
    )
    {
        this.MazeEventRewardList = mazeEventRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.MazeEventRewardEntityList = mazeEventRewardEntityList;
    }

    public MazeEventReceiveMazePointRewardData() { }
}

[MessagePackObject(true)]
public class MemoryEventActivateData
{
    public int Result { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public MemoryEventActivateData(
        int result,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.Result = result;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public MemoryEventActivateData() { }
}

[MessagePackObject(true)]
public class MissionGetDrillMissionListData
{
    public IEnumerable<DrillMissionList> DrillMissionList { get; set; }
    public IEnumerable<DrillMissionGroupList> DrillMissionGroupList { get; set; }
    public MissionNotice MissionNotice { get; set; }

    public MissionGetDrillMissionListData(
        IEnumerable<DrillMissionList> drillMissionList,
        IEnumerable<DrillMissionGroupList> drillMissionGroupList,
        MissionNotice missionNotice
    )
    {
        this.DrillMissionList = drillMissionList;
        this.DrillMissionGroupList = drillMissionGroupList;
        this.MissionNotice = missionNotice;
    }

    public MissionGetDrillMissionListData() { }
}

[MessagePackObject(true)]
public class MissionGetMissionListData : INormalMissionEndpointResponse
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<int> SpecialMissionPurchasedGroupIdList { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public CurrentMainStoryMission CurrentMainStoryMission { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public MissionNotice MissionNotice { get; set; }

    public MissionGetMissionListData(
        IEnumerable<NormalMissionList> normalMissionList,
        IEnumerable<DailyMissionList> dailyMissionList,
        IEnumerable<PeriodMissionList> periodMissionList,
        IEnumerable<BeginnerMissionList> beginnerMissionList,
        IEnumerable<SpecialMissionList> specialMissionList,
        IEnumerable<int> specialMissionPurchasedGroupIdList,
        IEnumerable<MainStoryMissionList> mainStoryMissionList,
        CurrentMainStoryMission currentMainStoryMission,
        IEnumerable<MemoryEventMissionList> memoryEventMissionList,
        IEnumerable<AlbumMissionList> albumMissionList,
        MissionNotice missionNotice
    )
    {
        this.normal_mission_list = normalMissionList;
        this.daily_mission_list = dailyMissionList;
        this.period_mission_list = periodMissionList;
        this.beginner_mission_list = beginnerMissionList;
        this.special_mission_list = specialMissionList;
        this.SpecialMissionPurchasedGroupIdList = specialMissionPurchasedGroupIdList;
        this.main_story_mission_list = mainStoryMissionList;
        this.CurrentMainStoryMission = currentMainStoryMission;
        this.memory_event_mission_list = memoryEventMissionList;
        this.album_mission_list = albumMissionList;
        this.MissionNotice = missionNotice;
    }

    public MissionGetMissionListData() { }
}

[MessagePackObject(true)]
public class MissionReceiveAlbumRewardData : INormalMissionEndpointResponse
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<int> NotReceivedMissionIdList { get; set; }
    public IEnumerable<int> NeedEntryEventIdList { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; }

    public MissionReceiveAlbumRewardData(
        IEnumerable<NormalMissionList> normalMissionList,
        IEnumerable<DailyMissionList> dailyMissionList,
        IEnumerable<PeriodMissionList> periodMissionList,
        IEnumerable<BeginnerMissionList> beginnerMissionList,
        IEnumerable<SpecialMissionList> specialMissionList,
        IEnumerable<MainStoryMissionList> mainStoryMissionList,
        IEnumerable<MemoryEventMissionList> memoryEventMissionList,
        IEnumerable<AlbumMissionList> albumMissionList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<int> notReceivedMissionIdList,
        IEnumerable<int> needEntryEventIdList,
        IEnumerable<ConvertedEntityList> convertedEntityList
    )
    {
        this.normal_mission_list = normalMissionList;
        this.daily_mission_list = dailyMissionList;
        this.period_mission_list = periodMissionList;
        this.beginner_mission_list = beginnerMissionList;
        this.special_mission_list = specialMissionList;
        this.main_story_mission_list = mainStoryMissionList;
        this.memory_event_mission_list = memoryEventMissionList;
        this.album_mission_list = albumMissionList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.NotReceivedMissionIdList = notReceivedMissionIdList;
        this.NeedEntryEventIdList = needEntryEventIdList;
        this.ConvertedEntityList = convertedEntityList;
    }

    public MissionReceiveAlbumRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveBeginnerRewardData : INormalMissionEndpointResponse
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<int> NotReceivedMissionIdList { get; set; }
    public IEnumerable<int> NeedEntryEventIdList { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; }

    public MissionReceiveBeginnerRewardData(
        IEnumerable<NormalMissionList> normalMissionList,
        IEnumerable<DailyMissionList> dailyMissionList,
        IEnumerable<PeriodMissionList> periodMissionList,
        IEnumerable<BeginnerMissionList> beginnerMissionList,
        IEnumerable<SpecialMissionList> specialMissionList,
        IEnumerable<MainStoryMissionList> mainStoryMissionList,
        IEnumerable<MemoryEventMissionList> memoryEventMissionList,
        IEnumerable<AlbumMissionList> albumMissionList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<int> notReceivedMissionIdList,
        IEnumerable<int> needEntryEventIdList,
        IEnumerable<ConvertedEntityList> convertedEntityList
    )
    {
        this.normal_mission_list = normalMissionList;
        this.daily_mission_list = dailyMissionList;
        this.period_mission_list = periodMissionList;
        this.beginner_mission_list = beginnerMissionList;
        this.special_mission_list = specialMissionList;
        this.main_story_mission_list = mainStoryMissionList;
        this.memory_event_mission_list = memoryEventMissionList;
        this.album_mission_list = albumMissionList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.NotReceivedMissionIdList = notReceivedMissionIdList;
        this.NeedEntryEventIdList = needEntryEventIdList;
        this.ConvertedEntityList = convertedEntityList;
    }

    public MissionReceiveBeginnerRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveDailyRewardData : INormalMissionEndpointResponse
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenNotReceivedMissionIdListWithDayNo> NotReceivedMissionIdListWithDayNo { get; set; }
    public IEnumerable<int> NeedEntryEventIdList { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; }

    public MissionReceiveDailyRewardData(
        IEnumerable<NormalMissionList> normalMissionList,
        IEnumerable<DailyMissionList> dailyMissionList,
        IEnumerable<PeriodMissionList> periodMissionList,
        IEnumerable<BeginnerMissionList> beginnerMissionList,
        IEnumerable<SpecialMissionList> specialMissionList,
        IEnumerable<MainStoryMissionList> mainStoryMissionList,
        IEnumerable<MemoryEventMissionList> memoryEventMissionList,
        IEnumerable<AlbumMissionList> albumMissionList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenNotReceivedMissionIdListWithDayNo> notReceivedMissionIdListWithDayNo,
        IEnumerable<int> needEntryEventIdList,
        IEnumerable<ConvertedEntityList> convertedEntityList
    )
    {
        this.normal_mission_list = normalMissionList;
        this.daily_mission_list = dailyMissionList;
        this.period_mission_list = periodMissionList;
        this.beginner_mission_list = beginnerMissionList;
        this.special_mission_list = specialMissionList;
        this.main_story_mission_list = mainStoryMissionList;
        this.memory_event_mission_list = memoryEventMissionList;
        this.album_mission_list = albumMissionList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.NotReceivedMissionIdListWithDayNo = notReceivedMissionIdListWithDayNo;
        this.NeedEntryEventIdList = needEntryEventIdList;
        this.ConvertedEntityList = convertedEntityList;
    }

    public MissionReceiveDailyRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveDrillRewardData
{
    public IEnumerable<DrillMissionList> DrillMissionList { get; set; }
    public IEnumerable<DrillMissionGroupList> DrillMissionGroupList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<int> NotReceivedMissionIdList { get; set; }
    public IEnumerable<int> NeedEntryEventIdList { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> DrillMissionGroupCompleteRewardList { get; set; }

    public MissionReceiveDrillRewardData(
        IEnumerable<DrillMissionList> drillMissionList,
        IEnumerable<DrillMissionGroupList> drillMissionGroupList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<int> notReceivedMissionIdList,
        IEnumerable<int> needEntryEventIdList,
        IEnumerable<ConvertedEntityList> convertedEntityList,
        IEnumerable<AtgenBuildEventRewardEntityList> drillMissionGroupCompleteRewardList
    )
    {
        this.DrillMissionList = drillMissionList;
        this.DrillMissionGroupList = drillMissionGroupList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.NotReceivedMissionIdList = notReceivedMissionIdList;
        this.NeedEntryEventIdList = needEntryEventIdList;
        this.ConvertedEntityList = convertedEntityList;
        this.DrillMissionGroupCompleteRewardList = drillMissionGroupCompleteRewardList;
    }

    public MissionReceiveDrillRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveMainStoryRewardData : INormalMissionEndpointResponse
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<int> NotReceivedMissionIdList { get; set; }
    public IEnumerable<int> NeedEntryEventIdList { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; }

    public MissionReceiveMainStoryRewardData(
        IEnumerable<NormalMissionList> normalMissionList,
        IEnumerable<DailyMissionList> dailyMissionList,
        IEnumerable<PeriodMissionList> periodMissionList,
        IEnumerable<BeginnerMissionList> beginnerMissionList,
        IEnumerable<SpecialMissionList> specialMissionList,
        IEnumerable<MainStoryMissionList> mainStoryMissionList,
        IEnumerable<MemoryEventMissionList> memoryEventMissionList,
        IEnumerable<AlbumMissionList> albumMissionList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<int> notReceivedMissionIdList,
        IEnumerable<int> needEntryEventIdList,
        IEnumerable<ConvertedEntityList> convertedEntityList
    )
    {
        this.normal_mission_list = normalMissionList;
        this.daily_mission_list = dailyMissionList;
        this.period_mission_list = periodMissionList;
        this.beginner_mission_list = beginnerMissionList;
        this.special_mission_list = specialMissionList;
        this.main_story_mission_list = mainStoryMissionList;
        this.memory_event_mission_list = memoryEventMissionList;
        this.album_mission_list = albumMissionList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.NotReceivedMissionIdList = notReceivedMissionIdList;
        this.NeedEntryEventIdList = needEntryEventIdList;
        this.ConvertedEntityList = convertedEntityList;
    }

    public MissionReceiveMainStoryRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveMemoryEventRewardData : INormalMissionEndpointResponse
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<int> NotReceivedMissionIdList { get; set; }
    public IEnumerable<int> NeedEntryEventIdList { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; }

    public MissionReceiveMemoryEventRewardData(
        IEnumerable<NormalMissionList> normalMissionList,
        IEnumerable<DailyMissionList> dailyMissionList,
        IEnumerable<PeriodMissionList> periodMissionList,
        IEnumerable<BeginnerMissionList> beginnerMissionList,
        IEnumerable<SpecialMissionList> specialMissionList,
        IEnumerable<MainStoryMissionList> mainStoryMissionList,
        IEnumerable<MemoryEventMissionList> memoryEventMissionList,
        IEnumerable<AlbumMissionList> albumMissionList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<int> notReceivedMissionIdList,
        IEnumerable<int> needEntryEventIdList,
        IEnumerable<ConvertedEntityList> convertedEntityList
    )
    {
        this.normal_mission_list = normalMissionList;
        this.daily_mission_list = dailyMissionList;
        this.period_mission_list = periodMissionList;
        this.beginner_mission_list = beginnerMissionList;
        this.special_mission_list = specialMissionList;
        this.main_story_mission_list = mainStoryMissionList;
        this.memory_event_mission_list = memoryEventMissionList;
        this.album_mission_list = albumMissionList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.NotReceivedMissionIdList = notReceivedMissionIdList;
        this.NeedEntryEventIdList = needEntryEventIdList;
        this.ConvertedEntityList = convertedEntityList;
    }

    public MissionReceiveMemoryEventRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveNormalRewardData : INormalMissionEndpointResponse
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<int> NotReceivedMissionIdList { get; set; }
    public IEnumerable<int> NeedEntryEventIdList { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; }

    public MissionReceiveNormalRewardData(
        IEnumerable<NormalMissionList> normalMissionList,
        IEnumerable<DailyMissionList> dailyMissionList,
        IEnumerable<PeriodMissionList> periodMissionList,
        IEnumerable<BeginnerMissionList> beginnerMissionList,
        IEnumerable<SpecialMissionList> specialMissionList,
        IEnumerable<MainStoryMissionList> mainStoryMissionList,
        IEnumerable<MemoryEventMissionList> memoryEventMissionList,
        IEnumerable<AlbumMissionList> albumMissionList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<int> notReceivedMissionIdList,
        IEnumerable<int> needEntryEventIdList,
        IEnumerable<ConvertedEntityList> convertedEntityList
    )
    {
        this.normal_mission_list = normalMissionList;
        this.daily_mission_list = dailyMissionList;
        this.period_mission_list = periodMissionList;
        this.beginner_mission_list = beginnerMissionList;
        this.special_mission_list = specialMissionList;
        this.main_story_mission_list = mainStoryMissionList;
        this.memory_event_mission_list = memoryEventMissionList;
        this.album_mission_list = albumMissionList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.NotReceivedMissionIdList = notReceivedMissionIdList;
        this.NeedEntryEventIdList = needEntryEventIdList;
        this.ConvertedEntityList = convertedEntityList;
    }

    public MissionReceiveNormalRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceivePeriodRewardData : INormalMissionEndpointResponse
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<int> NotReceivedMissionIdList { get; set; }
    public IEnumerable<int> NeedEntryEventIdList { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; }

    public MissionReceivePeriodRewardData(
        IEnumerable<NormalMissionList> normalMissionList,
        IEnumerable<DailyMissionList> dailyMissionList,
        IEnumerable<PeriodMissionList> periodMissionList,
        IEnumerable<BeginnerMissionList> beginnerMissionList,
        IEnumerable<SpecialMissionList> specialMissionList,
        IEnumerable<MainStoryMissionList> mainStoryMissionList,
        IEnumerable<MemoryEventMissionList> memoryEventMissionList,
        IEnumerable<AlbumMissionList> albumMissionList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<int> notReceivedMissionIdList,
        IEnumerable<int> needEntryEventIdList,
        IEnumerable<ConvertedEntityList> convertedEntityList
    )
    {
        this.normal_mission_list = normalMissionList;
        this.daily_mission_list = dailyMissionList;
        this.period_mission_list = periodMissionList;
        this.beginner_mission_list = beginnerMissionList;
        this.special_mission_list = specialMissionList;
        this.main_story_mission_list = mainStoryMissionList;
        this.memory_event_mission_list = memoryEventMissionList;
        this.album_mission_list = albumMissionList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.NotReceivedMissionIdList = notReceivedMissionIdList;
        this.NeedEntryEventIdList = needEntryEventIdList;
        this.ConvertedEntityList = convertedEntityList;
    }

    public MissionReceivePeriodRewardData() { }
}

[MessagePackObject(true)]
public class MissionReceiveSpecialRewardData : INormalMissionEndpointResponse
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<int> NotReceivedMissionIdList { get; set; }
    public IEnumerable<int> NeedEntryEventIdList { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; }

    public MissionReceiveSpecialRewardData(
        IEnumerable<NormalMissionList> normalMissionList,
        IEnumerable<DailyMissionList> dailyMissionList,
        IEnumerable<PeriodMissionList> periodMissionList,
        IEnumerable<BeginnerMissionList> beginnerMissionList,
        IEnumerable<SpecialMissionList> specialMissionList,
        IEnumerable<MainStoryMissionList> mainStoryMissionList,
        IEnumerable<MemoryEventMissionList> memoryEventMissionList,
        IEnumerable<AlbumMissionList> albumMissionList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<int> notReceivedMissionIdList,
        IEnumerable<int> needEntryEventIdList,
        IEnumerable<ConvertedEntityList> convertedEntityList
    )
    {
        this.normal_mission_list = normalMissionList;
        this.daily_mission_list = dailyMissionList;
        this.period_mission_list = periodMissionList;
        this.beginner_mission_list = beginnerMissionList;
        this.special_mission_list = specialMissionList;
        this.main_story_mission_list = mainStoryMissionList;
        this.memory_event_mission_list = memoryEventMissionList;
        this.album_mission_list = albumMissionList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.NotReceivedMissionIdList = notReceivedMissionIdList;
        this.NeedEntryEventIdList = needEntryEventIdList;
        this.ConvertedEntityList = convertedEntityList;
    }

    public MissionReceiveSpecialRewardData() { }
}

[MessagePackObject(true)]
public class MissionUnlockDrillMissionGroupData
{
    public IEnumerable<DrillMissionList> DrillMissionList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public MissionUnlockDrillMissionGroupData(
        IEnumerable<DrillMissionList> drillMissionList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.DrillMissionList = drillMissionList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public MissionUnlockDrillMissionGroupData() { }
}

[MessagePackObject(true)]
public class MissionUnlockMainStoryGroupData
{
    public IEnumerable<MainStoryMissionList> MainStoryMissionList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> MainStoryMissionUnlockBonusList { get; set; }

    public MissionUnlockMainStoryGroupData(
        IEnumerable<MainStoryMissionList> mainStoryMissionList,
        UpdateDataList updateDataList,
        IEnumerable<AtgenBuildEventRewardEntityList> mainStoryMissionUnlockBonusList
    )
    {
        this.MainStoryMissionList = mainStoryMissionList;
        this.UpdateDataList = updateDataList;
        this.MainStoryMissionUnlockBonusList = mainStoryMissionUnlockBonusList;
    }

    public MissionUnlockMainStoryGroupData() { }
}

[MessagePackObject(true)]
public class MypageInfoData
{
    public int PresentCnt { get; set; }
    public int FriendApply { get; set; }
    public bool Friend { get; set; }
    public int AchievementCnt { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public int IsReceiveEventDamageReward { get; set; }
    public int IsViewStartDash { get; set; }
    public int IsViewDreamSelect { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsShopNotification { get; set; }
    public RepeatData RepeatData { get; set; }
    public IEnumerable<UserSummonList> UserSummonList { get; set; }
    public IEnumerable<QuestEventScheduleList> QuestEventScheduleList { get; set; }
    public IEnumerable<QuestScheduleDetailList> QuestScheduleDetailList { get; set; }

    public MypageInfoData(
        int presentCnt,
        int friendApply,
        bool friend,
        int achievementCnt,
        UpdateDataList updateDataList,
        int isReceiveEventDamageReward,
        int isViewStartDash,
        int isViewDreamSelect,
        bool isShopNotification,
        RepeatData repeatData,
        IEnumerable<UserSummonList> userSummonList,
        IEnumerable<QuestEventScheduleList> questEventScheduleList,
        IEnumerable<QuestScheduleDetailList> questScheduleDetailList
    )
    {
        this.PresentCnt = presentCnt;
        this.FriendApply = friendApply;
        this.Friend = friend;
        this.AchievementCnt = achievementCnt;
        this.UpdateDataList = updateDataList;
        this.IsReceiveEventDamageReward = isReceiveEventDamageReward;
        this.IsViewStartDash = isViewStartDash;
        this.IsViewDreamSelect = isViewDreamSelect;
        this.IsShopNotification = isShopNotification;
        this.RepeatData = repeatData;
        this.UserSummonList = userSummonList;
        this.QuestEventScheduleList = questEventScheduleList;
        this.QuestScheduleDetailList = questScheduleDetailList;
    }

    public MypageInfoData() { }
}

[MessagePackObject(true)]
public class OptionGetOptionData
{
    public OptionData OptionData { get; set; }

    public OptionGetOptionData(OptionData optionData)
    {
        this.OptionData = optionData;
    }

    public OptionGetOptionData() { }
}

[MessagePackObject(true)]
public class OptionSetOptionData
{
    public OptionData OptionData { get; set; }

    public OptionSetOptionData(OptionData optionData)
    {
        this.OptionData = optionData;
    }

    public OptionSetOptionData() { }
}

[MessagePackObject(true)]
public class PartyIndexData
{
    public IEnumerable<BuildList> BuildList { get; set; }

    public PartyIndexData(IEnumerable<BuildList> buildList)
    {
        this.BuildList = buildList;
    }

    public PartyIndexData() { }
}

[MessagePackObject(true)]
public class PartySetMainPartyNoData
{
    public int MainPartyNo { get; set; }

    public PartySetMainPartyNoData(int mainPartyNo)
    {
        this.MainPartyNo = mainPartyNo;
    }

    public PartySetMainPartyNoData() { }
}

[MessagePackObject(true)]
public class PartySetPartySettingData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public PartySetPartySettingData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public PartySetPartySettingData() { }
}

[MessagePackObject(true)]
public class PartyUpdatePartyNameData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public PartyUpdatePartyNameData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public PartyUpdatePartyNameData() { }
}

[MessagePackObject(true)]
public class PlatformAchievementGetPlatformAchievementListData
{
    public IEnumerable<AchievementList> AchievementList { get; set; }

    public PlatformAchievementGetPlatformAchievementListData(
        IEnumerable<AchievementList> achievementList
    )
    {
        this.AchievementList = achievementList;
    }

    public PlatformAchievementGetPlatformAchievementListData() { }
}

[MessagePackObject(true)]
public class PresentGetHistoryListData
{
    public IEnumerable<PresentHistoryList> PresentHistoryList { get; set; }

    public PresentGetHistoryListData(IEnumerable<PresentHistoryList> presentHistoryList)
    {
        this.PresentHistoryList = presentHistoryList;
    }

    public PresentGetHistoryListData() { }
}

[MessagePackObject(true)]
public class PresentGetPresentListData
{
    public IEnumerable<PresentDetailList> PresentList { get; set; }
    public IEnumerable<PresentDetailList> PresentLimitList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public PresentGetPresentListData(
        IEnumerable<PresentDetailList> presentList,
        IEnumerable<PresentDetailList> presentLimitList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.PresentList = presentList;
        this.PresentLimitList = presentLimitList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public PresentGetPresentListData() { }
}

[MessagePackObject(true)]
public class PresentReceiveData
{
    public IEnumerable<ulong> ReceivePresentIdList { get; set; }
    public IEnumerable<ulong> NotReceivePresentIdList { get; set; }
    public IEnumerable<ulong> DeletePresentIdList { get; set; }
    public IEnumerable<ulong> LimitOverPresentIdList { get; set; }
    public IEnumerable<PresentDetailList> PresentList { get; set; }
    public IEnumerable<PresentDetailList> PresentLimitList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; }

    public PresentReceiveData(
        IEnumerable<ulong> receivePresentIdList,
        IEnumerable<ulong> notReceivePresentIdList,
        IEnumerable<ulong> deletePresentIdList,
        IEnumerable<ulong> limitOverPresentIdList,
        IEnumerable<PresentDetailList> presentList,
        IEnumerable<PresentDetailList> presentLimitList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.ReceivePresentIdList = receivePresentIdList;
        this.NotReceivePresentIdList = notReceivePresentIdList;
        this.DeletePresentIdList = deletePresentIdList;
        this.LimitOverPresentIdList = limitOverPresentIdList;
        this.PresentList = presentList;
        this.PresentLimitList = presentLimitList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public PresentReceiveData() { }
}

[MessagePackObject(true)]
public class PushNotificationUpdateSettingData
{
    public int Result { get; set; }

    public PushNotificationUpdateSettingData(int result)
    {
        this.Result = result;
    }

    public PushNotificationUpdateSettingData() { }
}

[MessagePackObject(true)]
public class QuestDropListData
{
    public AtgenQuestDropInfo QuestDropInfo { get; set; }

    public QuestDropListData(AtgenQuestDropInfo questDropInfo)
    {
        this.QuestDropInfo = questDropInfo;
    }

    public QuestDropListData() { }
}

[MessagePackObject(true)]
public class QuestGetQuestClearPartyMultiData
{
    public IEnumerable<PartySettingList> QuestMultiClearPartySettingList { get; set; }
    public IEnumerable<AtgenLostUnitList> LostUnitList { get; set; }

    public QuestGetQuestClearPartyMultiData(
        IEnumerable<PartySettingList> questMultiClearPartySettingList,
        IEnumerable<AtgenLostUnitList> lostUnitList
    )
    {
        this.QuestMultiClearPartySettingList = questMultiClearPartySettingList;
        this.LostUnitList = lostUnitList;
    }

    public QuestGetQuestClearPartyMultiData() { }
}

[MessagePackObject(true)]
public class QuestGetQuestClearPartyData
{
    public IEnumerable<PartySettingList> QuestClearPartySettingList { get; set; }
    public IEnumerable<AtgenLostUnitList> LostUnitList { get; set; }

    public QuestGetQuestClearPartyData(
        IEnumerable<PartySettingList> questClearPartySettingList,
        IEnumerable<AtgenLostUnitList> lostUnitList
    )
    {
        this.QuestClearPartySettingList = questClearPartySettingList;
        this.LostUnitList = lostUnitList;
    }

    public QuestGetQuestClearPartyData() { }
}

[MessagePackObject(true)]
public class QuestGetSupportUserListData
{
    public IEnumerable<UserSupportList> SupportUserList { get; set; }
    public IEnumerable<AtgenSupportUserDetailList> SupportUserDetailList { get; set; }

    public QuestGetSupportUserListData(
        IEnumerable<UserSupportList> supportUserList,
        IEnumerable<AtgenSupportUserDetailList> supportUserDetailList
    )
    {
        this.SupportUserList = supportUserList;
        this.SupportUserDetailList = supportUserDetailList;
    }

    public QuestGetSupportUserListData() { }
}

[MessagePackObject(true)]
public class QuestOpenTreasureData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> QuestTreasureRewardList { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> DuplicateEntityList { get; set; }
    public int AddMaxDragonQuantity { get; set; }
    public int AddMaxWeaponQuantity { get; set; }
    public int AddMaxAmuletQuantity { get; set; }

    public QuestOpenTreasureData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenBuildEventRewardEntityList> questTreasureRewardList,
        IEnumerable<AtgenDuplicateEntityList> duplicateEntityList,
        int addMaxDragonQuantity,
        int addMaxWeaponQuantity,
        int addMaxAmuletQuantity
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.QuestTreasureRewardList = questTreasureRewardList;
        this.DuplicateEntityList = duplicateEntityList;
        this.AddMaxDragonQuantity = addMaxDragonQuantity;
        this.AddMaxWeaponQuantity = addMaxWeaponQuantity;
        this.AddMaxAmuletQuantity = addMaxAmuletQuantity;
    }

    public QuestOpenTreasureData() { }
}

[MessagePackObject(true)]
public class QuestReadStoryData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenQuestStoryRewardList> QuestStoryRewardList { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; }

    public QuestReadStoryData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenQuestStoryRewardList> questStoryRewardList,
        IEnumerable<ConvertedEntityList> convertedEntityList
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.QuestStoryRewardList = questStoryRewardList;
        this.ConvertedEntityList = convertedEntityList;
    }

    public QuestReadStoryData() { }
}

[MessagePackObject(true)]
public class QuestSearchQuestClearPartyCharaMultiData
{
    public IEnumerable<SearchClearPartyCharaList> SearchQuestClearPartyCharaList { get; set; }

    public QuestSearchQuestClearPartyCharaMultiData(
        IEnumerable<SearchClearPartyCharaList> searchQuestClearPartyCharaList
    )
    {
        this.SearchQuestClearPartyCharaList = searchQuestClearPartyCharaList;
    }

    public QuestSearchQuestClearPartyCharaMultiData() { }
}

[MessagePackObject(true)]
public class QuestSearchQuestClearPartyCharaData
{
    public IEnumerable<SearchClearPartyCharaList> SearchQuestClearPartyCharaList { get; set; }

    public QuestSearchQuestClearPartyCharaData(
        IEnumerable<SearchClearPartyCharaList> searchQuestClearPartyCharaList
    )
    {
        this.SearchQuestClearPartyCharaList = searchQuestClearPartyCharaList;
    }

    public QuestSearchQuestClearPartyCharaData() { }
}

[MessagePackObject(true)]
public class QuestSearchQuestClearPartyMultiData
{
    public IEnumerable<SearchClearPartyList> SearchQuestClearPartyList { get; set; }

    public QuestSearchQuestClearPartyMultiData(
        IEnumerable<SearchClearPartyList> searchQuestClearPartyList
    )
    {
        this.SearchQuestClearPartyList = searchQuestClearPartyList;
    }

    public QuestSearchQuestClearPartyMultiData() { }
}

[MessagePackObject(true)]
public class QuestSearchQuestClearPartyData
{
    public IEnumerable<SearchClearPartyList> SearchQuestClearPartyList { get; set; }

    public QuestSearchQuestClearPartyData(
        IEnumerable<SearchClearPartyList> searchQuestClearPartyList
    )
    {
        this.SearchQuestClearPartyList = searchQuestClearPartyList;
    }

    public QuestSearchQuestClearPartyData() { }
}

[MessagePackObject(true)]
public class QuestSetQuestClearPartyMultiData
{
    public int Result { get; set; }

    public QuestSetQuestClearPartyMultiData(int result)
    {
        this.Result = result;
    }

    public QuestSetQuestClearPartyMultiData() { }
}

[MessagePackObject(true)]
public class QuestSetQuestClearPartyData
{
    public int Result { get; set; }

    public QuestSetQuestClearPartyData(int result)
    {
        this.Result = result;
    }

    public QuestSetQuestClearPartyData() { }
}

[MessagePackObject(true)]
public class RaidEventEntryData
{
    public RaidEventUserList RaidEventUserData { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public RaidEventEntryData(
        RaidEventUserList raidEventUserData,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.RaidEventUserData = raidEventUserData;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public RaidEventEntryData() { }
}

[MessagePackObject(true)]
public class RaidEventGetEventDataData
{
    public RaidEventUserList RaidEventUserData { get; set; }
    public IEnumerable<RaidEventRewardList> RaidEventRewardList { get; set; }
    public IEnumerable<CharaFriendshipList> CharaFriendshipList { get; set; }
    public IEnumerable<EventTradeList> EventTradeList { get; set; }
    public IEnumerable<EventPassiveList> EventPassiveList { get; set; }
    public IEnumerable<EventAbilityCharaList> EventAbilityCharaList { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsReceiveEventDamageReward { get; set; }
    public AtgenEventDamageData EventDamageData { get; set; }

    public RaidEventGetEventDataData(
        RaidEventUserList raidEventUserData,
        IEnumerable<RaidEventRewardList> raidEventRewardList,
        IEnumerable<CharaFriendshipList> charaFriendshipList,
        IEnumerable<EventTradeList> eventTradeList,
        IEnumerable<EventPassiveList> eventPassiveList,
        IEnumerable<EventAbilityCharaList> eventAbilityCharaList,
        bool isReceiveEventDamageReward,
        AtgenEventDamageData eventDamageData
    )
    {
        this.RaidEventUserData = raidEventUserData;
        this.RaidEventRewardList = raidEventRewardList;
        this.CharaFriendshipList = charaFriendshipList;
        this.EventTradeList = eventTradeList;
        this.EventPassiveList = eventPassiveList;
        this.EventAbilityCharaList = eventAbilityCharaList;
        this.IsReceiveEventDamageReward = isReceiveEventDamageReward;
        this.EventDamageData = eventDamageData;
    }

    public RaidEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class RaidEventReceiveRaidPointRewardData
{
    public IEnumerable<RaidEventRewardList> RaidEventRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public RaidEventReceiveRaidPointRewardData(
        IEnumerable<RaidEventRewardList> raidEventRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.RaidEventRewardList = raidEventRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public RaidEventReceiveRaidPointRewardData() { }
}

[MessagePackObject(true)]
public class RedoableSummonFixExecData
{
    public UserRedoableSummonData UserRedoableSummonData { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public RedoableSummonFixExecData(
        UserRedoableSummonData userRedoableSummonData,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.UserRedoableSummonData = userRedoableSummonData;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public RedoableSummonFixExecData() { }
}

[MessagePackObject(true)]
public class RedoableSummonGetDataData
{
    public UserRedoableSummonData UserRedoableSummonData { get; set; }
    public RedoableSummonOddsRateList RedoableSummonOddsRateList { get; set; }

    public RedoableSummonGetDataData(
        UserRedoableSummonData userRedoableSummonData,
        RedoableSummonOddsRateList redoableSummonOddsRateList
    )
    {
        this.UserRedoableSummonData = userRedoableSummonData;
        this.RedoableSummonOddsRateList = redoableSummonOddsRateList;
    }

    public RedoableSummonGetDataData() { }
}

[MessagePackObject(true)]
public class RedoableSummonPreExecData
{
    public UserRedoableSummonData UserRedoableSummonData { get; set; }

    public RedoableSummonPreExecData(UserRedoableSummonData userRedoableSummonData)
    {
        this.UserRedoableSummonData = userRedoableSummonData;
    }

    public RedoableSummonPreExecData() { }
}

[MessagePackObject(true)]
public class RepeatEndData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IngameResultData IngameResultData { get; set; }
    public RepeatData RepeatData { get; set; }

    public RepeatEndData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IngameResultData ingameResultData,
        RepeatData repeatData
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.IngameResultData = ingameResultData;
        this.RepeatData = repeatData;
    }

    public RepeatEndData() { }
}

[MessagePackObject(true)]
public class ShopChargeCancelData
{
    public int IsQuestBonus { get; set; }
    public int IsStoneBonus { get; set; }
    public int IsStaminaBonus { get; set; }
    public IEnumerable<ShopPurchaseList> MaterialShopPurchase { get; set; }
    public IEnumerable<ShopPurchaseList> NormalShopPurchase { get; set; }
    public IEnumerable<ShopPurchaseList> SpecialShopPurchase { get; set; }
    public IEnumerable<AtgenStoneBonus> StoneBonus { get; set; }
    public IEnumerable<AtgenStaminaBonus> StaminaBonus { get; set; }
    public IEnumerable<AtgenQuestBonus> QuestBonus { get; set; }
    public IEnumerable<AtgenProductLockList> ProductLockList { get; set; }
    public IEnumerable<ProductList> ProductList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public AtgenUserItemSummon UserItemSummon { get; set; }
    public int InfancyPaidDiamondLimit { get; set; }

    public ShopChargeCancelData(
        int isQuestBonus,
        int isStoneBonus,
        int isStaminaBonus,
        IEnumerable<ShopPurchaseList> materialShopPurchase,
        IEnumerable<ShopPurchaseList> normalShopPurchase,
        IEnumerable<ShopPurchaseList> specialShopPurchase,
        IEnumerable<AtgenStoneBonus> stoneBonus,
        IEnumerable<AtgenStaminaBonus> staminaBonus,
        IEnumerable<AtgenQuestBonus> questBonus,
        IEnumerable<AtgenProductLockList> productLockList,
        IEnumerable<ProductList> productList,
        UpdateDataList updateDataList,
        AtgenUserItemSummon userItemSummon,
        int infancyPaidDiamondLimit
    )
    {
        this.IsQuestBonus = isQuestBonus;
        this.IsStoneBonus = isStoneBonus;
        this.IsStaminaBonus = isStaminaBonus;
        this.MaterialShopPurchase = materialShopPurchase;
        this.NormalShopPurchase = normalShopPurchase;
        this.SpecialShopPurchase = specialShopPurchase;
        this.StoneBonus = stoneBonus;
        this.StaminaBonus = staminaBonus;
        this.QuestBonus = questBonus;
        this.ProductLockList = productLockList;
        this.ProductList = productList;
        this.UpdateDataList = updateDataList;
        this.UserItemSummon = userItemSummon;
        this.InfancyPaidDiamondLimit = infancyPaidDiamondLimit;
    }

    public ShopChargeCancelData() { }
}

[MessagePackObject(true)]
public class ShopGetBonusData
{
    public int IsQuestBonus { get; set; }
    public int IsStoneBonus { get; set; }
    public int IsStaminaBonus { get; set; }
    public IEnumerable<AtgenStoneBonus> StoneBonus { get; set; }
    public IEnumerable<AtgenStaminaBonus> StaminaBonus { get; set; }
    public IEnumerable<AtgenQuestBonus> QuestBonus { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> StoneBonusEntityList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public ShopGetBonusData(
        int isQuestBonus,
        int isStoneBonus,
        int isStaminaBonus,
        IEnumerable<AtgenStoneBonus> stoneBonus,
        IEnumerable<AtgenStaminaBonus> staminaBonus,
        IEnumerable<AtgenQuestBonus> questBonus,
        IEnumerable<AtgenBuildEventRewardEntityList> stoneBonusEntityList,
        UpdateDataList updateDataList
    )
    {
        this.IsQuestBonus = isQuestBonus;
        this.IsStoneBonus = isStoneBonus;
        this.IsStaminaBonus = isStaminaBonus;
        this.StoneBonus = stoneBonus;
        this.StaminaBonus = staminaBonus;
        this.QuestBonus = questBonus;
        this.StoneBonusEntityList = stoneBonusEntityList;
        this.UpdateDataList = updateDataList;
    }

    public ShopGetBonusData() { }
}

[MessagePackObject(true)]
public class ShopGetDreamSelectUnitListData
{
    public IEnumerable<AtgenDuplicateEntityList> DreamSelectUnitList { get; set; }

    public ShopGetDreamSelectUnitListData(IEnumerable<AtgenDuplicateEntityList> dreamSelectUnitList)
    {
        this.DreamSelectUnitList = dreamSelectUnitList;
    }

    public ShopGetDreamSelectUnitListData() { }
}

[MessagePackObject(true)]
public class ShopGetListData
{
    public int IsQuestBonus { get; set; }
    public int IsStoneBonus { get; set; }
    public int IsStaminaBonus { get; set; }
    public IEnumerable<ShopPurchaseList> MaterialShopPurchase { get; set; }
    public IEnumerable<ShopPurchaseList> NormalShopPurchase { get; set; }
    public IEnumerable<ShopPurchaseList> SpecialShopPurchase { get; set; }
    public IEnumerable<AtgenStoneBonus> StoneBonus { get; set; }
    public IEnumerable<AtgenStaminaBonus> StaminaBonus { get; set; }
    public IEnumerable<AtgenQuestBonus> QuestBonus { get; set; }
    public IEnumerable<AtgenProductLockList> ProductLockList { get; set; }
    public IEnumerable<ProductList> ProductList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public AtgenUserItemSummon UserItemSummon { get; set; }
    public int InfancyPaidDiamondLimit { get; set; }

    public ShopGetListData(
        int isQuestBonus,
        int isStoneBonus,
        int isStaminaBonus,
        IEnumerable<ShopPurchaseList> materialShopPurchase,
        IEnumerable<ShopPurchaseList> normalShopPurchase,
        IEnumerable<ShopPurchaseList> specialShopPurchase,
        IEnumerable<AtgenStoneBonus> stoneBonus,
        IEnumerable<AtgenStaminaBonus> staminaBonus,
        IEnumerable<AtgenQuestBonus> questBonus,
        IEnumerable<AtgenProductLockList> productLockList,
        IEnumerable<ProductList> productList,
        UpdateDataList updateDataList,
        AtgenUserItemSummon userItemSummon,
        int infancyPaidDiamondLimit
    )
    {
        this.IsQuestBonus = isQuestBonus;
        this.IsStoneBonus = isStoneBonus;
        this.IsStaminaBonus = isStaminaBonus;
        this.MaterialShopPurchase = materialShopPurchase;
        this.NormalShopPurchase = normalShopPurchase;
        this.SpecialShopPurchase = specialShopPurchase;
        this.StoneBonus = stoneBonus;
        this.StaminaBonus = staminaBonus;
        this.QuestBonus = questBonus;
        this.ProductLockList = productLockList;
        this.ProductList = productList;
        this.UpdateDataList = updateDataList;
        this.UserItemSummon = userItemSummon;
        this.InfancyPaidDiamondLimit = infancyPaidDiamondLimit;
    }

    public ShopGetListData() { }
}

[MessagePackObject(true)]
public class ShopGetProductListData
{
    public IEnumerable<ProductList> ProductList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public int InfancyPaidDiamondLimit { get; set; }

    public ShopGetProductListData(
        IEnumerable<ProductList> productList,
        UpdateDataList updateDataList,
        int infancyPaidDiamondLimit
    )
    {
        this.ProductList = productList;
        this.UpdateDataList = updateDataList;
        this.InfancyPaidDiamondLimit = infancyPaidDiamondLimit;
    }

    public ShopGetProductListData() { }
}

[MessagePackObject(true)]
public class ShopItemSummonExecData
{
    public AtgenUserItemSummon UserItemSummon { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> ItemSummonRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public ShopItemSummonExecData(
        AtgenUserItemSummon userItemSummon,
        IEnumerable<AtgenBuildEventRewardEntityList> itemSummonRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.UserItemSummon = userItemSummon;
        this.ItemSummonRewardList = itemSummonRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public ShopItemSummonExecData() { }
}

[MessagePackObject(true)]
public class ShopItemSummonOddData
{
    public IEnumerable<AtgenItemSummonRateList> ItemSummonRateList { get; set; }

    public ShopItemSummonOddData(IEnumerable<AtgenItemSummonRateList> itemSummonRateList)
    {
        this.ItemSummonRateList = itemSummonRateList;
    }

    public ShopItemSummonOddData() { }
}

[MessagePackObject(true)]
public class ShopMaterialShopPurchaseData
{
    public int IsQuestBonus { get; set; }
    public int IsStoneBonus { get; set; }
    public int IsStaminaBonus { get; set; }
    public IEnumerable<ShopPurchaseList> MaterialShopPurchase { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public ShopMaterialShopPurchaseData(
        int isQuestBonus,
        int isStoneBonus,
        int isStaminaBonus,
        IEnumerable<ShopPurchaseList> materialShopPurchase,
        UpdateDataList updateDataList
    )
    {
        this.IsQuestBonus = isQuestBonus;
        this.IsStoneBonus = isStoneBonus;
        this.IsStaminaBonus = isStaminaBonus;
        this.MaterialShopPurchase = materialShopPurchase;
        this.UpdateDataList = updateDataList;
    }

    public ShopMaterialShopPurchaseData() { }
}

[MessagePackObject(true)]
public class ShopNormalShopPurchaseData
{
    public int IsQuestBonus { get; set; }
    public int IsStoneBonus { get; set; }
    public int IsStaminaBonus { get; set; }
    public IEnumerable<ShopPurchaseList> NormalShopPurchase { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public ShopNormalShopPurchaseData(
        int isQuestBonus,
        int isStoneBonus,
        int isStaminaBonus,
        IEnumerable<ShopPurchaseList> normalShopPurchase,
        UpdateDataList updateDataList
    )
    {
        this.IsQuestBonus = isQuestBonus;
        this.IsStoneBonus = isStoneBonus;
        this.IsStaminaBonus = isStaminaBonus;
        this.NormalShopPurchase = normalShopPurchase;
        this.UpdateDataList = updateDataList;
    }

    public ShopNormalShopPurchaseData() { }
}

[MessagePackObject(true)]
public class ShopPreChargeCheckData
{
    public int IsQuestBonus { get; set; }
    public int IsStoneBonus { get; set; }
    public int IsStaminaBonus { get; set; }
    public IEnumerable<ShopPurchaseList> MaterialShopPurchase { get; set; }
    public IEnumerable<ShopPurchaseList> NormalShopPurchase { get; set; }
    public IEnumerable<ShopPurchaseList> SpecialShopPurchase { get; set; }
    public IEnumerable<AtgenStoneBonus> StoneBonus { get; set; }
    public IEnumerable<AtgenStaminaBonus> StaminaBonus { get; set; }
    public IEnumerable<AtgenQuestBonus> QuestBonus { get; set; }
    public IEnumerable<AtgenProductLockList> ProductLockList { get; set; }
    public IEnumerable<ProductList> ProductList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public AtgenUserItemSummon UserItemSummon { get; set; }
    public int InfancyPaidDiamondLimit { get; set; }
    public int IsPurchase { get; set; }

    public ShopPreChargeCheckData(
        int isQuestBonus,
        int isStoneBonus,
        int isStaminaBonus,
        IEnumerable<ShopPurchaseList> materialShopPurchase,
        IEnumerable<ShopPurchaseList> normalShopPurchase,
        IEnumerable<ShopPurchaseList> specialShopPurchase,
        IEnumerable<AtgenStoneBonus> stoneBonus,
        IEnumerable<AtgenStaminaBonus> staminaBonus,
        IEnumerable<AtgenQuestBonus> questBonus,
        IEnumerable<AtgenProductLockList> productLockList,
        IEnumerable<ProductList> productList,
        UpdateDataList updateDataList,
        AtgenUserItemSummon userItemSummon,
        int infancyPaidDiamondLimit,
        int isPurchase
    )
    {
        this.IsQuestBonus = isQuestBonus;
        this.IsStoneBonus = isStoneBonus;
        this.IsStaminaBonus = isStaminaBonus;
        this.MaterialShopPurchase = materialShopPurchase;
        this.NormalShopPurchase = normalShopPurchase;
        this.SpecialShopPurchase = specialShopPurchase;
        this.StoneBonus = stoneBonus;
        this.StaminaBonus = staminaBonus;
        this.QuestBonus = questBonus;
        this.ProductLockList = productLockList;
        this.ProductList = productList;
        this.UpdateDataList = updateDataList;
        this.UserItemSummon = userItemSummon;
        this.InfancyPaidDiamondLimit = infancyPaidDiamondLimit;
        this.IsPurchase = isPurchase;
    }

    public ShopPreChargeCheckData() { }
}

[MessagePackObject(true)]
public class ShopSpecialShopPurchaseData
{
    public int IsQuestBonus { get; set; }
    public int IsStoneBonus { get; set; }
    public int IsStaminaBonus { get; set; }
    public IEnumerable<ShopPurchaseList> SpecialShopPurchase { get; set; }
    public IEnumerable<AtgenStoneBonus> StoneBonus { get; set; }
    public IEnumerable<AtgenStaminaBonus> StaminaBonus { get; set; }
    public IEnumerable<AtgenQuestBonus> QuestBonus { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public ShopSpecialShopPurchaseData(
        int isQuestBonus,
        int isStoneBonus,
        int isStaminaBonus,
        IEnumerable<ShopPurchaseList> specialShopPurchase,
        IEnumerable<AtgenStoneBonus> stoneBonus,
        IEnumerable<AtgenStaminaBonus> staminaBonus,
        IEnumerable<AtgenQuestBonus> questBonus,
        UpdateDataList updateDataList
    )
    {
        this.IsQuestBonus = isQuestBonus;
        this.IsStoneBonus = isStoneBonus;
        this.IsStaminaBonus = isStaminaBonus;
        this.SpecialShopPurchase = specialShopPurchase;
        this.StoneBonus = stoneBonus;
        this.StaminaBonus = staminaBonus;
        this.QuestBonus = questBonus;
        this.UpdateDataList = updateDataList;
    }

    public ShopSpecialShopPurchaseData() { }
}

[MessagePackObject(true)]
public class SimpleEventEntryData
{
    public SimpleEventUserList SimpleEventUserData { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public SimpleEventEntryData(
        SimpleEventUserList simpleEventUserData,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.SimpleEventUserData = simpleEventUserData;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public SimpleEventEntryData() { }
}

[MessagePackObject(true)]
public class SimpleEventGetEventDataData
{
    public SimpleEventUserList SimpleEventUserData { get; set; }
    public IEnumerable<EventTradeList> EventTradeList { get; set; }

    public SimpleEventGetEventDataData(
        SimpleEventUserList simpleEventUserData,
        IEnumerable<EventTradeList> eventTradeList
    )
    {
        this.SimpleEventUserData = simpleEventUserData;
        this.EventTradeList = eventTradeList;
    }

    public SimpleEventGetEventDataData() { }
}

[MessagePackObject(true)]
public class StampGetStampData
{
    public IEnumerable<StampList> StampList { get; set; }

    public StampGetStampData(IEnumerable<StampList> stampList)
    {
        this.StampList = stampList;
    }

    public StampGetStampData() { }
}

[MessagePackObject(true)]
public class StampSetEquipStampData
{
    public int Result { get; set; }
    public IEnumerable<EquipStampList> EquipStampList { get; set; }

    public StampSetEquipStampData(int result, IEnumerable<EquipStampList> equipStampList)
    {
        this.Result = result;
        this.EquipStampList = equipStampList;
    }

    public StampSetEquipStampData() { }
}

[MessagePackObject(true)]
public class StoryReadData
{
    public IEnumerable<AtgenBuildEventRewardEntityList> UnitStoryRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> DuplicateEntityList { get; set; }

    public StoryReadData(
        IEnumerable<AtgenBuildEventRewardEntityList> unitStoryRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenDuplicateEntityList> duplicateEntityList
    )
    {
        this.UnitStoryRewardList = unitStoryRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.DuplicateEntityList = duplicateEntityList;
    }

    public StoryReadData() { }
}

[MessagePackObject(true)]
public class StorySkipSkipData
{
    public int ResultState { get; set; }

    public StorySkipSkipData(int resultState)
    {
        this.ResultState = resultState;
    }

    public StorySkipSkipData() { }
}

[MessagePackObject(true)]
public class SuggestionGetCategoryListData
{
    public IEnumerable<AtgenCategoryList> CategoryList { get; set; }

    public SuggestionGetCategoryListData(IEnumerable<AtgenCategoryList> categoryList)
    {
        this.CategoryList = categoryList;
    }

    public SuggestionGetCategoryListData() { }
}

[MessagePackObject(true)]
public class SuggestionSetData { }

[MessagePackObject(true)]
public class SummonExcludeGetListData
{
    public IEnumerable<AtgenDuplicateEntityList> SummonExcludeUnitList { get; set; }

    public SummonExcludeGetListData(IEnumerable<AtgenDuplicateEntityList> summonExcludeUnitList)
    {
        this.SummonExcludeUnitList = summonExcludeUnitList;
    }

    public SummonExcludeGetListData() { }
}

[MessagePackObject(true)]
public class SummonExcludeGetOddsDataData
{
    public OddsRateList OddsRateList { get; set; }
    public SummonPrizeOddsRateList SummonPrizeOddsRateList { get; set; }

    public SummonExcludeGetOddsDataData(
        OddsRateList oddsRateList,
        SummonPrizeOddsRateList summonPrizeOddsRateList
    )
    {
        this.OddsRateList = oddsRateList;
        this.SummonPrizeOddsRateList = summonPrizeOddsRateList;
    }

    public SummonExcludeGetOddsDataData() { }
}

[MessagePackObject(true)]
public class SummonExcludeRequestData
{
    public IEnumerable<AtgenResultUnitList> ResultUnitList { get; set; }
    public IEnumerable<AtgenResultPrizeList> ResultPrizeList { get; set; }
    public IEnumerable<int> PresageEffectList { get; set; }
    public int ReversalEffectIndex { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<SummonTicketList> SummonTicketList { get; set; }
    public int ResultSummonPoint { get; set; }
    public IEnumerable<UserSummonList> UserSummonList { get; set; }

    public SummonExcludeRequestData(
        IEnumerable<AtgenResultUnitList> resultUnitList,
        IEnumerable<AtgenResultPrizeList> resultPrizeList,
        IEnumerable<int> presageEffectList,
        int reversalEffectIndex,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<SummonTicketList> summonTicketList,
        int resultSummonPoint,
        IEnumerable<UserSummonList> userSummonList
    )
    {
        this.ResultUnitList = resultUnitList;
        this.ResultPrizeList = resultPrizeList;
        this.PresageEffectList = presageEffectList;
        this.ReversalEffectIndex = reversalEffectIndex;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.SummonTicketList = summonTicketList;
        this.ResultSummonPoint = resultSummonPoint;
        this.UserSummonList = userSummonList;
    }

    public SummonExcludeRequestData() { }
}

[MessagePackObject(true)]
public class SummonGetOddsDataData
{
    public OddsRateList OddsRateList { get; set; }
    public SummonPrizeOddsRateList SummonPrizeOddsRateList { get; set; }

    public SummonGetOddsDataData(
        OddsRateList oddsRateList,
        SummonPrizeOddsRateList summonPrizeOddsRateList
    )
    {
        this.OddsRateList = oddsRateList;
        this.SummonPrizeOddsRateList = summonPrizeOddsRateList;
    }

    public SummonGetOddsDataData() { }
}

[MessagePackObject(true)]
public class SummonGetSummonHistoryData
{
    public IEnumerable<SummonHistoryList> SummonHistoryList { get; set; }

    public SummonGetSummonHistoryData(IEnumerable<SummonHistoryList> summonHistoryList)
    {
        this.SummonHistoryList = summonHistoryList;
    }

    public SummonGetSummonHistoryData() { }
}

[MessagePackObject(true)]
public class SummonGetSummonListData
{
    public IEnumerable<SummonList> SummonList { get; set; }
    public IEnumerable<SummonList> CharaSsrSummonList { get; set; }
    public IEnumerable<SummonList> DragonSsrSummonList { get; set; }
    public IEnumerable<SummonList> CharaSsrUpdateSummonList { get; set; }
    public IEnumerable<SummonList> DragonSsrUpdateSummonList { get; set; }
    public IEnumerable<SummonList> CampaignSummonList { get; set; }
    public IEnumerable<SummonList> CampaignSsrSummonList { get; set; }
    public IEnumerable<SummonList> PlatinumSummonList { get; set; }
    public IEnumerable<SummonList> ExcludeSummonList { get; set; }
    public AtgenCsSummonList CsSummonList { get; set; }
    public IEnumerable<SummonTicketList> SummonTicketList { get; set; }
    public IEnumerable<SummonPointList> SummonPointList { get; set; }

    public SummonGetSummonListData(
        IEnumerable<SummonList> summonList,
        IEnumerable<SummonList> charaSsrSummonList,
        IEnumerable<SummonList> dragonSsrSummonList,
        IEnumerable<SummonList> charaSsrUpdateSummonList,
        IEnumerable<SummonList> dragonSsrUpdateSummonList,
        IEnumerable<SummonList> campaignSummonList,
        IEnumerable<SummonList> campaignSsrSummonList,
        IEnumerable<SummonList> platinumSummonList,
        IEnumerable<SummonList> excludeSummonList,
        AtgenCsSummonList csSummonList,
        IEnumerable<SummonTicketList> summonTicketList,
        IEnumerable<SummonPointList> summonPointList
    )
    {
        this.SummonList = summonList;
        this.CharaSsrSummonList = charaSsrSummonList;
        this.DragonSsrSummonList = dragonSsrSummonList;
        this.CharaSsrUpdateSummonList = charaSsrUpdateSummonList;
        this.DragonSsrUpdateSummonList = dragonSsrUpdateSummonList;
        this.CampaignSummonList = campaignSummonList;
        this.CampaignSsrSummonList = campaignSsrSummonList;
        this.PlatinumSummonList = platinumSummonList;
        this.ExcludeSummonList = excludeSummonList;
        this.CsSummonList = csSummonList;
        this.SummonTicketList = summonTicketList;
        this.SummonPointList = summonPointList;
    }

    public SummonGetSummonListData() { }
}

[MessagePackObject(true)]
public class SummonGetSummonPointTradeData
{
    public IEnumerable<AtgenSummonPointTradeList> SummonPointTradeList { get; set; }
    public IEnumerable<SummonPointList> SummonPointList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public SummonGetSummonPointTradeData(
        IEnumerable<AtgenSummonPointTradeList> summonPointTradeList,
        IEnumerable<SummonPointList> summonPointList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.SummonPointTradeList = summonPointTradeList;
        this.SummonPointList = summonPointList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public SummonGetSummonPointTradeData() { }
}

/// <summary>
/// Data for a SummonRequest response
/// </summary>
/// <param name="reversal_effect_index">Index in <see cref="ResultPrizeList"/> to do a fakeout on (rarity 4 orb in sky turns to rarity 5 upon landing)</param>
/// <param name="presage_effect_list">Probably the summon effect (doves, rainbow, fafnirs, etc)</param>
/// <param name="result_unit_list">List of rolled units</param>
/// <param name="result_prize_list">List of rolled prizes from prize summons</param>
/// <param name="summon_ticket_list">Probably list of used summon tickets<br/>List because of summons with multiple singles?</param>
/// <param name="result_summon_point">Summon points for sparking</param>
/// <param name="user_summon_list">Updated summon banner data</param>
/// <param name="update_data_list">Updated user data</param>
/// <param name="entity_result">List of converted and new entities</param>
[MessagePackObject(true)]
public class SummonRequestData
{
    public IEnumerable<AtgenResultUnitList> ResultUnitList { get; set; }
    public IEnumerable<AtgenResultPrizeList> ResultPrizeList { get; set; }
    public IEnumerable<int> PresageEffectList { get; set; }
    public int ReversalEffectIndex { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<SummonTicketList> SummonTicketList { get; set; }
    public int ResultSummonPoint { get; set; }
    public IEnumerable<UserSummonList> UserSummonList { get; set; }

    public SummonRequestData(
        IEnumerable<AtgenResultUnitList> resultUnitList,
        IEnumerable<AtgenResultPrizeList> resultPrizeList,
        IEnumerable<int> presageEffectList,
        int reversalEffectIndex,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<SummonTicketList> summonTicketList,
        int resultSummonPoint,
        IEnumerable<UserSummonList> userSummonList
    )
    {
        this.ResultUnitList = resultUnitList;
        this.ResultPrizeList = resultPrizeList;
        this.PresageEffectList = presageEffectList;
        this.ReversalEffectIndex = reversalEffectIndex;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.SummonTicketList = summonTicketList;
        this.ResultSummonPoint = resultSummonPoint;
        this.UserSummonList = userSummonList;
    }

    public SummonRequestData() { }
}

[MessagePackObject(true)]
public class SummonSummonPointTradeData
{
    public IEnumerable<AtgenBuildEventRewardEntityList> ExchangeEntityList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public SummonSummonPointTradeData(
        IEnumerable<AtgenBuildEventRewardEntityList> exchangeEntityList,
        UpdateDataList updateDataList
    )
    {
        this.ExchangeEntityList = exchangeEntityList;
        this.UpdateDataList = updateDataList;
    }

    public SummonSummonPointTradeData() { }
}

[MessagePackObject(true)]
public class TalismanSellData
{
    public DeleteDataList DeleteDataList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public TalismanSellData(
        DeleteDataList deleteDataList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.DeleteDataList = deleteDataList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public TalismanSellData() { }
}

[MessagePackObject(true)]
public class TalismanSetLockData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public TalismanSetLockData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public TalismanSetLockData() { }
}

[MessagePackObject(true)]
public class TimeAttackRankingGetDataData
{
    public IEnumerable<RankingTierRewardList> RankingTierRewardList { get; set; }

    public TimeAttackRankingGetDataData(IEnumerable<RankingTierRewardList> rankingTierRewardList)
    {
        this.RankingTierRewardList = rankingTierRewardList;
    }

    public TimeAttackRankingGetDataData() { }
}

[MessagePackObject(true)]
public class TimeAttackRankingReceiveTierRewardData
{
    public IEnumerable<RankingTierRewardList> RankingTierRewardList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> RankingTierRewardEntityList { get; set; }

    public TimeAttackRankingReceiveTierRewardData(
        IEnumerable<RankingTierRewardList> rankingTierRewardList,
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenBuildEventRewardEntityList> rankingTierRewardEntityList
    )
    {
        this.RankingTierRewardList = rankingTierRewardList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.RankingTierRewardEntityList = rankingTierRewardEntityList;
    }

    public TimeAttackRankingReceiveTierRewardData() { }
}

[MessagePackObject(true)]
public class ToolAuthData
{
    public ulong ViewerId { get; set; }
    public string SessionId { get; set; }
    public string Nonce { get; set; }

    public ToolAuthData(ulong viewerId, string sessionId, string nonce)
    {
        this.ViewerId = viewerId;
        this.SessionId = sessionId;
        this.Nonce = nonce;
    }

    public ToolAuthData() { }
}

[MessagePackObject(true)]
public class ToolGetMaintenanceTimeData
{
    public int MaintenanceStartTime { get; set; }
    public int MaintenanceEndTime { get; set; }

    public ToolGetMaintenanceTimeData(int maintenanceStartTime, int maintenanceEndTime)
    {
        this.MaintenanceStartTime = maintenanceStartTime;
        this.MaintenanceEndTime = maintenanceEndTime;
    }

    public ToolGetMaintenanceTimeData() { }
}

[MessagePackObject(true)]
public class ToolGetServiceStatusData
{
    public int ServiceStatus { get; set; }

    public ToolGetServiceStatusData(int serviceStatus)
    {
        this.ServiceStatus = serviceStatus;
    }

    public ToolGetServiceStatusData() { }
}

[MessagePackObject(true)]
public class ToolReauthData
{
    public ulong ViewerId { get; set; }
    public string SessionId { get; set; }
    public string Nonce { get; set; }

    public ToolReauthData(ulong viewerId, string sessionId, string nonce)
    {
        this.ViewerId = viewerId;
        this.SessionId = sessionId;
        this.Nonce = nonce;
    }

    public ToolReauthData() { }
}

[MessagePackObject(true)]
public class ToolSignupData
{
    public ulong ViewerId { get; set; }
    public DateTimeOffset Servertime { get; set; }

    public ToolSignupData(ulong viewerId, DateTimeOffset servertime)
    {
        this.ViewerId = viewerId;
        this.Servertime = servertime;
    }

    public ToolSignupData() { }
}

[MessagePackObject(true)]
public class TrackRecordUpdateProgressData
{
    public UpdateDataList UpdateDataList { get; set; }

    public TrackRecordUpdateProgressData(UpdateDataList updateDataList)
    {
        this.UpdateDataList = updateDataList;
    }

    public TrackRecordUpdateProgressData() { }
}

[MessagePackObject(true)]
public class TransitionTransitionByNAccountData
{
    public AtgenTransitionResultData TransitionResultData { get; set; }

    public TransitionTransitionByNAccountData(AtgenTransitionResultData transitionResultData)
    {
        this.TransitionResultData = transitionResultData;
    }

    public TransitionTransitionByNAccountData() { }
}

[MessagePackObject(true)]
public class TreasureTradeGetListAllData
{
    public IEnumerable<UserTreasureTradeList> UserTreasureTradeList { get; set; }
    public IEnumerable<TreasureTradeList> TreasureTradeList { get; set; }
    public IEnumerable<TreasureTradeList> TreasureTradeAllList { get; set; }
    public DmodeInfo DmodeInfo { get; set; }

    public TreasureTradeGetListAllData(
        IEnumerable<UserTreasureTradeList> userTreasureTradeList,
        IEnumerable<TreasureTradeList> treasureTradeList,
        IEnumerable<TreasureTradeList> treasureTradeAllList,
        DmodeInfo dmodeInfo
    )
    {
        this.UserTreasureTradeList = userTreasureTradeList;
        this.TreasureTradeList = treasureTradeList;
        this.TreasureTradeAllList = treasureTradeAllList;
        this.DmodeInfo = dmodeInfo;
    }

    public TreasureTradeGetListAllData() { }
}

[MessagePackObject(true)]
public class TreasureTradeGetListData
{
    public IEnumerable<UserTreasureTradeList> UserTreasureTradeList { get; set; }
    public IEnumerable<TreasureTradeList> TreasureTradeList { get; set; }
    public IEnumerable<TreasureTradeList> TreasureTradeAllList { get; set; }
    public DmodeInfo DmodeInfo { get; set; }

    public TreasureTradeGetListData(
        IEnumerable<UserTreasureTradeList> userTreasureTradeList,
        IEnumerable<TreasureTradeList> treasureTradeList,
        IEnumerable<TreasureTradeList> treasureTradeAllList,
        DmodeInfo dmodeInfo
    )
    {
        this.UserTreasureTradeList = userTreasureTradeList;
        this.TreasureTradeList = treasureTradeList;
        this.TreasureTradeAllList = treasureTradeAllList;
        this.DmodeInfo = dmodeInfo;
    }

    public TreasureTradeGetListData() { }
}

[MessagePackObject(true)]
public class TreasureTradeTradeData
{
    public IEnumerable<UserTreasureTradeList> UserTreasureTradeList { get; set; }
    public IEnumerable<TreasureTradeList> TreasureTradeList { get; set; }
    public IEnumerable<TreasureTradeList> TreasureTradeAllList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public DeleteDataList DeleteDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public TreasureTradeTradeData(
        IEnumerable<UserTreasureTradeList> userTreasureTradeList,
        IEnumerable<TreasureTradeList> treasureTradeList,
        IEnumerable<TreasureTradeList> treasureTradeAllList,
        UpdateDataList updateDataList,
        DeleteDataList deleteDataList,
        EntityResult entityResult
    )
    {
        this.UserTreasureTradeList = userTreasureTradeList;
        this.TreasureTradeList = treasureTradeList;
        this.TreasureTradeAllList = treasureTradeAllList;
        this.UpdateDataList = updateDataList;
        this.DeleteDataList = deleteDataList;
        this.EntityResult = entityResult;
    }

    public TreasureTradeTradeData() { }
}

[MessagePackObject(true)]
public class TutorialUpdateFlagsData
{
    public IEnumerable<int> TutorialFlagList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public TutorialUpdateFlagsData(
        IEnumerable<int> tutorialFlagList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.TutorialFlagList = tutorialFlagList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public TutorialUpdateFlagsData() { }
}

[MessagePackObject(true)]
public class TutorialUpdateStepData
{
    public int Step { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public TutorialUpdateStepData(
        int step,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.Step = step;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public TutorialUpdateStepData() { }
}

[MessagePackObject(true)]
public class UpdateNamechangeData
{
    public string CheckedName { get; set; }

    public UpdateNamechangeData(string checkedName)
    {
        this.CheckedName = checkedName;
    }

    public UpdateNamechangeData() { }
}

[MessagePackObject(true)]
public class UpdateResetNewData
{
    public int Result { get; set; }

    public UpdateResetNewData(int result)
    {
        this.Result = result;
    }

    public UpdateResetNewData() { }
}

[MessagePackObject(true)]
public class UserGetNAccountInfoData
{
    public UpdateDataList UpdateDataList { get; set; }
    public AtgenNAccountInfo NAccountInfo { get; set; }

    public UserGetNAccountInfoData(UpdateDataList updateDataList, AtgenNAccountInfo nAccountInfo)
    {
        this.UpdateDataList = updateDataList;
        this.NAccountInfo = nAccountInfo;
    }

    public UserGetNAccountInfoData() { }
}

[MessagePackObject(true)]
public class UserGetWalletBalanceData
{
    public WalletBalance WalletBalance { get; set; }

    public UserGetWalletBalanceData(WalletBalance walletBalance)
    {
        this.WalletBalance = walletBalance;
    }

    public UserGetWalletBalanceData() { }
}

[MessagePackObject(true)]
public class UserLinkedNAccountData
{
    public UpdateDataList UpdateDataList { get; set; }

    public UserLinkedNAccountData(UpdateDataList updateDataList)
    {
        this.UpdateDataList = updateDataList;
    }

    public UserLinkedNAccountData() { }
}

[MessagePackObject(true)]
public class UserOptInSettingData
{
    public int IsOptin { get; set; }

    public UserOptInSettingData(int isOptin)
    {
        this.IsOptin = isOptin;
    }

    public UserOptInSettingData() { }
}

[MessagePackObject(true)]
public class UserRecoverStaminaByStoneData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public AtgenRecoverData RecoverData { get; set; }

    public UserRecoverStaminaByStoneData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        AtgenRecoverData recoverData
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.RecoverData = recoverData;
    }

    public UserRecoverStaminaByStoneData() { }
}

[MessagePackObject(true)]
public class UserWithdrawalData
{
    public int Result { get; set; }

    public UserWithdrawalData(int result)
    {
        this.Result = result;
    }

    public UserWithdrawalData() { }
}

[MessagePackObject(true)]
public class VersionGetResourceVersionData
{
    public string ResourceVersion { get; set; }

    public VersionGetResourceVersionData(string resourceVersion)
    {
        this.ResourceVersion = resourceVersion;
    }

    public VersionGetResourceVersionData() { }
}

[MessagePackObject(true)]
public class WalkerSendGiftMultipleData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public int IsFavorite { get; set; }
    public IEnumerable<DragonRewardEntityList> ReturnGiftList { get; set; }
    public IEnumerable<RewardReliabilityList> RewardReliabilityList { get; set; }
    public AtgenWalkerData WalkerData { get; set; }

    public WalkerSendGiftMultipleData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        int isFavorite,
        IEnumerable<DragonRewardEntityList> returnGiftList,
        IEnumerable<RewardReliabilityList> rewardReliabilityList,
        AtgenWalkerData walkerData
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.IsFavorite = isFavorite;
        this.ReturnGiftList = returnGiftList;
        this.RewardReliabilityList = rewardReliabilityList;
        this.WalkerData = walkerData;
    }

    public WalkerSendGiftMultipleData() { }
}

[MessagePackObject(true)]
public class WallFailData
{
    public int Result { get; set; }
    public IEnumerable<UserSupportList> FailHelperList { get; set; }
    public IEnumerable<AtgenHelperDetailList> FailHelperDetailList { get; set; }
    public AtgenFailQuestDetail FailQuestDetail { get; set; }

    public WallFailData(
        int result,
        IEnumerable<UserSupportList> failHelperList,
        IEnumerable<AtgenHelperDetailList> failHelperDetailList,
        AtgenFailQuestDetail failQuestDetail
    )
    {
        this.Result = result;
        this.FailHelperList = failHelperList;
        this.FailHelperDetailList = failHelperDetailList;
        this.FailQuestDetail = failQuestDetail;
    }

    public WallFailData() { }
}

[MessagePackObject(true)]
public class WallGetMonthlyRewardData
{
    public IEnumerable<AtgenUserWallRewardList> UserWallRewardList { get; set; }

    public WallGetMonthlyRewardData(IEnumerable<AtgenUserWallRewardList> userWallRewardList)
    {
        this.UserWallRewardList = userWallRewardList;
    }

    public WallGetMonthlyRewardData() { }
}

[MessagePackObject(true)]
public class WallGetWallClearPartyData
{
    public IEnumerable<PartySettingList> WallClearPartySettingList { get; set; }
    public IEnumerable<AtgenLostUnitList> LostUnitList { get; set; }

    public WallGetWallClearPartyData(
        IEnumerable<PartySettingList> wallClearPartySettingList,
        IEnumerable<AtgenLostUnitList> lostUnitList
    )
    {
        this.WallClearPartySettingList = wallClearPartySettingList;
        this.LostUnitList = lostUnitList;
    }

    public WallGetWallClearPartyData() { }
}

[MessagePackObject(true)]
public class WallReceiveMonthlyRewardData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> WallMonthlyRewardList { get; set; }
    public IEnumerable<AtgenUserWallRewardList> UserWallRewardList { get; set; }
    public IEnumerable<AtgenMonthlyWallReceiveList> MonthlyWallReceiveList { get; set; }

    public WallReceiveMonthlyRewardData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        IEnumerable<AtgenBuildEventRewardEntityList> wallMonthlyRewardList,
        IEnumerable<AtgenUserWallRewardList> userWallRewardList,
        IEnumerable<AtgenMonthlyWallReceiveList> monthlyWallReceiveList
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.WallMonthlyRewardList = wallMonthlyRewardList;
        this.UserWallRewardList = userWallRewardList;
        this.MonthlyWallReceiveList = monthlyWallReceiveList;
    }

    public WallReceiveMonthlyRewardData() { }
}

[MessagePackObject(true)]
public class WallRecordRecordData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public AtgenPlayWallDetail PlayWallDetail { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> WallClearRewardList { get; set; }
    public AtgenWallDropReward WallDropReward { get; set; }
    public AtgenWallUnitInfo WallUnitInfo { get; set; }

    public WallRecordRecordData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        AtgenPlayWallDetail playWallDetail,
        IEnumerable<AtgenBuildEventRewardEntityList> wallClearRewardList,
        AtgenWallDropReward wallDropReward,
        AtgenWallUnitInfo wallUnitInfo
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.PlayWallDetail = playWallDetail;
        this.WallClearRewardList = wallClearRewardList;
        this.WallDropReward = wallDropReward;
        this.WallUnitInfo = wallUnitInfo;
    }

    public WallRecordRecordData() { }
}

[MessagePackObject(true)]
public class WallSetWallClearPartyData
{
    public int Result { get; set; }

    public WallSetWallClearPartyData(int result)
    {
        this.Result = result;
    }

    public WallSetWallClearPartyData() { }
}

[MessagePackObject(true)]
public class WallStartStartAssignUnitData
{
    public IngameData IngameData { get; set; }
    public IngameWallData IngameWallData { get; set; }
    public OddsInfo OddsInfo { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public WallStartStartAssignUnitData(
        IngameData ingameData,
        IngameWallData ingameWallData,
        OddsInfo oddsInfo,
        UpdateDataList updateDataList
    )
    {
        this.IngameData = ingameData;
        this.IngameWallData = ingameWallData;
        this.OddsInfo = oddsInfo;
        this.UpdateDataList = updateDataList;
    }

    public WallStartStartAssignUnitData() { }
}

[MessagePackObject(true)]
public class WallStartStartData
{
    public IngameData IngameData { get; set; }
    public IngameWallData IngameWallData { get; set; }
    public OddsInfo OddsInfo { get; set; }
    public UpdateDataList UpdateDataList { get; set; }

    public WallStartStartData(
        IngameData ingameData,
        IngameWallData ingameWallData,
        OddsInfo oddsInfo,
        UpdateDataList updateDataList
    )
    {
        this.IngameData = ingameData;
        this.IngameWallData = ingameWallData;
        this.OddsInfo = oddsInfo;
        this.UpdateDataList = updateDataList;
    }

    public WallStartStartData() { }
}

[MessagePackObject(true)]
public class WeaponBodyBuildupPieceData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public WeaponBodyBuildupPieceData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public WeaponBodyBuildupPieceData() { }
}

[MessagePackObject(true)]
public class WeaponBodyCraftData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public WeaponBodyCraftData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public WeaponBodyCraftData() { }
}

[MessagePackObject(true)]
public class WeaponBuildupData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public DeleteDataList DeleteDataList { get; set; }

    public WeaponBuildupData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        DeleteDataList deleteDataList
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.DeleteDataList = deleteDataList;
    }

    public WeaponBuildupData() { }
}

[MessagePackObject(true)]
public class WeaponLimitBreakData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }
    public DeleteDataList DeleteDataList { get; set; }

    public WeaponLimitBreakData(
        UpdateDataList updateDataList,
        EntityResult entityResult,
        DeleteDataList deleteDataList
    )
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
        this.DeleteDataList = deleteDataList;
    }

    public WeaponLimitBreakData() { }
}

[MessagePackObject(true)]
public class WeaponResetPlusCountData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public WeaponResetPlusCountData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public WeaponResetPlusCountData() { }
}

[MessagePackObject(true)]
public class WeaponSellData
{
    public DeleteDataList DeleteDataList { get; set; }
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public WeaponSellData(
        DeleteDataList deleteDataList,
        UpdateDataList updateDataList,
        EntityResult entityResult
    )
    {
        this.DeleteDataList = deleteDataList;
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public WeaponSellData() { }
}

[MessagePackObject(true)]
public class WeaponSetLockData
{
    public UpdateDataList UpdateDataList { get; set; }
    public EntityResult EntityResult { get; set; }

    public WeaponSetLockData(UpdateDataList updateDataList, EntityResult entityResult)
    {
        this.UpdateDataList = updateDataList;
        this.EntityResult = entityResult;
    }

    public WeaponSetLockData() { }
}

[MessagePackObject(true)]
public class WebviewVersionUrlListData
{
    public IEnumerable<AtgenWebviewUrlList> WebviewUrlList { get; set; }

    public WebviewVersionUrlListData(IEnumerable<AtgenWebviewUrlList> webviewUrlList)
    {
        this.WebviewUrlList = webviewUrlList;
    }

    public WebviewVersionUrlListData() { }
}
