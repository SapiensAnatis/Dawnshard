using System.ComponentModel.DataAnnotations;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Features.Version;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Models.Generated;

[MessagePackObject(true)]
public class AbilityCrestBuildupPieceRequest
{
    public AbilityCrests AbilityCrestId { get; set; }
    public IEnumerable<AtgenBuildupAbilityCrestPieceList> BuildupAbilityCrestPieceList { get; set; }

    public AbilityCrestBuildupPieceRequest(
        AbilityCrests abilityCrestId,
        IEnumerable<AtgenBuildupAbilityCrestPieceList> buildupAbilityCrestPieceList
    )
    {
        this.AbilityCrestId = abilityCrestId;
        this.BuildupAbilityCrestPieceList = buildupAbilityCrestPieceList;
    }

    public AbilityCrestBuildupPieceRequest() { }
}

[MessagePackObject(true)]
public class AbilityCrestBuildupPlusCountRequest
{
    public AbilityCrests AbilityCrestId { get; set; }
    public IEnumerable<AtgenPlusCountParamsList> PlusCountParamsList { get; set; }

    public AbilityCrestBuildupPlusCountRequest(
        AbilityCrests abilityCrestId,
        IEnumerable<AtgenPlusCountParamsList> plusCountParamsList
    )
    {
        this.AbilityCrestId = abilityCrestId;
        this.PlusCountParamsList = plusCountParamsList;
    }

    public AbilityCrestBuildupPlusCountRequest() { }
}

[MessagePackObject(true)]
public class AbilityCrestGetAbilityCrestSetListRequest { }

[MessagePackObject(true)]
public class AbilityCrestResetPlusCountRequest
{
    public AbilityCrests AbilityCrestId { get; set; }
    public IEnumerable<PlusCountType> PlusCountTypeList { get; set; }

    public AbilityCrestResetPlusCountRequest(
        AbilityCrests abilityCrestId,
        IEnumerable<PlusCountType> plusCountTypeList
    )
    {
        this.AbilityCrestId = abilityCrestId;
        this.PlusCountTypeList = plusCountTypeList;
    }

    public AbilityCrestResetPlusCountRequest() { }
}

[MessagePackObject(true)]
public class AbilityCrestSetAbilityCrestSetRequest
{
    public int AbilityCrestSetNo { get; set; }
    public string AbilityCrestSetName { get; set; }
    public AtgenRequestAbilityCrestSetData RequestAbilityCrestSetData { get; set; }

    public AbilityCrestSetAbilityCrestSetRequest(
        int abilityCrestSetNo,
        string abilityCrestSetName,
        AtgenRequestAbilityCrestSetData requestAbilityCrestSetData
    )
    {
        this.AbilityCrestSetNo = abilityCrestSetNo;
        this.AbilityCrestSetName = abilityCrestSetName;
        this.RequestAbilityCrestSetData = requestAbilityCrestSetData;
    }

    public AbilityCrestSetAbilityCrestSetRequest() { }
}

[MessagePackObject(true)]
public class AbilityCrestSetFavoriteRequest
{
    public AbilityCrests AbilityCrestId { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsFavorite { get; set; }

    public AbilityCrestSetFavoriteRequest(AbilityCrests abilityCrestId, bool isFavorite)
    {
        this.AbilityCrestId = abilityCrestId;
        this.IsFavorite = isFavorite;
    }

    public AbilityCrestSetFavoriteRequest() { }
}

[MessagePackObject(true)]
public class AbilityCrestTradeGetListRequest { }

[MessagePackObject(true)]
public class AbilityCrestTradeTradeRequest
{
    public int AbilityCrestTradeId { get; set; }
    public int TradeCount { get; set; }

    public AbilityCrestTradeTradeRequest(int abilityCrestTradeId, int tradeCount)
    {
        this.AbilityCrestTradeId = abilityCrestTradeId;
        this.TradeCount = tradeCount;
    }

    public AbilityCrestTradeTradeRequest() { }
}

[MessagePackObject(true)]
public class AbilityCrestUpdateAbilityCrestSetNameRequest
{
    public int AbilityCrestSetNo { get; set; }
    public string AbilityCrestSetName { get; set; }

    public AbilityCrestUpdateAbilityCrestSetNameRequest(
        int abilityCrestSetNo,
        string abilityCrestSetName
    )
    {
        this.AbilityCrestSetNo = abilityCrestSetNo;
        this.AbilityCrestSetName = abilityCrestSetName;
    }

    public AbilityCrestUpdateAbilityCrestSetNameRequest() { }
}

[MessagePackObject(true)]
public class AlbumIndexRequest { }

[MessagePackObject(true)]
public class AmuletBuildupRequest
{
    public ulong BaseAmuletKeyId { get; set; }
    public IEnumerable<GrowMaterialList> GrowMaterialList { get; set; }

    public AmuletBuildupRequest(
        ulong baseAmuletKeyId,
        IEnumerable<GrowMaterialList> growMaterialList
    )
    {
        this.BaseAmuletKeyId = baseAmuletKeyId;
        this.GrowMaterialList = growMaterialList;
    }

    public AmuletBuildupRequest() { }
}

[MessagePackObject(true)]
public class AmuletLimitBreakRequest
{
    public ulong BaseAmuletKeyId { get; set; }
    public IEnumerable<GrowMaterialList> GrowMaterialList { get; set; }

    public AmuletLimitBreakRequest(
        ulong baseAmuletKeyId,
        IEnumerable<GrowMaterialList> growMaterialList
    )
    {
        this.BaseAmuletKeyId = baseAmuletKeyId;
        this.GrowMaterialList = growMaterialList;
    }

    public AmuletLimitBreakRequest() { }
}

[MessagePackObject(true)]
public class AmuletResetPlusCountRequest
{
    public ulong AmuletKeyId { get; set; }
    public int PlusCountType { get; set; }

    public AmuletResetPlusCountRequest(ulong amuletKeyId, int plusCountType)
    {
        this.AmuletKeyId = amuletKeyId;
        this.PlusCountType = plusCountType;
    }

    public AmuletResetPlusCountRequest() { }
}

[MessagePackObject(true)]
public class AmuletSellRequest
{
    public IEnumerable<ulong> AmuletKeyIdList { get; set; }

    public AmuletSellRequest(IEnumerable<ulong> amuletKeyIdList)
    {
        this.AmuletKeyIdList = amuletKeyIdList;
    }

    public AmuletSellRequest() { }
}

[MessagePackObject(true)]
public class AmuletSetLockRequest
{
    public ulong AmuletKeyId { get; set; }
    public int IsLock { get; set; }

    public AmuletSetLockRequest(ulong amuletKeyId, int isLock)
    {
        this.AmuletKeyId = amuletKeyId;
        this.IsLock = isLock;
    }

    public AmuletSetLockRequest() { }
}

[MessagePackObject(true)]
public class AmuletTradeGetListRequest { }

[MessagePackObject(true)]
public class AmuletTradeTradeRequest
{
    public int AmuletTradeId { get; set; }
    public int TradeCount { get; set; }

    public AmuletTradeTradeRequest(int amuletTradeId, int tradeCount)
    {
        this.AmuletTradeId = amuletTradeId;
        this.TradeCount = tradeCount;
    }

    public AmuletTradeTradeRequest() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventEntryRequest
{
    public int EventId { get; set; }

    public BattleRoyalEventEntryRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public BattleRoyalEventEntryRequest() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventGetEventDataRequest
{
    public int EventId { get; set; }

    public BattleRoyalEventGetEventDataRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public BattleRoyalEventGetEventDataRequest() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventReceiveEventCyclePointRewardRequest
{
    public int EventId { get; set; }
    public int EventCycleId { get; set; }

    public BattleRoyalEventReceiveEventCyclePointRewardRequest(int eventId, int eventCycleId)
    {
        this.EventId = eventId;
        this.EventCycleId = eventCycleId;
    }

    public BattleRoyalEventReceiveEventCyclePointRewardRequest() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventReleaseCharaSkinRequest
{
    public int BattleRoyalCharaSkinId { get; set; }
    public int IsPickup { get; set; }

    public BattleRoyalEventReleaseCharaSkinRequest(int battleRoyalCharaSkinId, int isPickup)
    {
        this.BattleRoyalCharaSkinId = battleRoyalCharaSkinId;
        this.IsPickup = isPickup;
    }

    public BattleRoyalEventReleaseCharaSkinRequest() { }
}

[MessagePackObject(true)]
public class BattleRoyalFailRequest
{
    public string DungeonKey { get; set; }
    public int FailState { get; set; }
    public int NoPlayFlg { get; set; }

    public BattleRoyalFailRequest(string dungeonKey, int failState, int noPlayFlg)
    {
        this.DungeonKey = dungeonKey;
        this.FailState = failState;
        this.NoPlayFlg = noPlayFlg;
    }

    public BattleRoyalFailRequest() { }
}

[MessagePackObject(true)]
public class BattleRoyalGetBattleRoyalHistoryRequest
{
    public int EventId { get; set; }

    public BattleRoyalGetBattleRoyalHistoryRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public BattleRoyalGetBattleRoyalHistoryRequest() { }
}

[MessagePackObject(true)]
public class BattleRoyalRecordRoyalRecordMultiRequest
{
    public PlayRecord PlayRecord { get; set; }
    public string DungeonKey { get; set; }
    public string MultiplayKey { get; set; }

    public BattleRoyalRecordRoyalRecordMultiRequest(
        PlayRecord playRecord,
        string dungeonKey,
        string multiplayKey
    )
    {
        this.PlayRecord = playRecord;
        this.DungeonKey = dungeonKey;
        this.MultiplayKey = multiplayKey;
    }

    public BattleRoyalRecordRoyalRecordMultiRequest() { }
}

[MessagePackObject(true)]
public class BattleRoyalStartMultiRequest
{
    public Charas CharaId { get; set; }
    public int IsTutorial { get; set; }
    public string MultiplayKey { get; set; }

    public BattleRoyalStartMultiRequest(Charas charaId, int isTutorial, string multiplayKey)
    {
        this.CharaId = charaId;
        this.IsTutorial = isTutorial;
        this.MultiplayKey = multiplayKey;
    }

    public BattleRoyalStartMultiRequest() { }
}

[MessagePackObject(true)]
public class BattleRoyalStartRoyalMultiRequest { }

[MessagePackObject(true)]
public class BuildEventEntryRequest
{
    public int EventId { get; set; }

    public BuildEventEntryRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public BuildEventEntryRequest() { }
}

[MessagePackObject(true)]
public class BuildEventGetEventDataRequest
{
    public int EventId { get; set; }

    public BuildEventGetEventDataRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public BuildEventGetEventDataRequest() { }
}

[MessagePackObject(true)]
public class BuildEventReceiveBuildPointRewardRequest
{
    public int EventId { get; set; }

    public BuildEventReceiveBuildPointRewardRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public BuildEventReceiveBuildPointRewardRequest() { }
}

[MessagePackObject(true)]
public class BuildEventReceiveDailyBonusRequest
{
    public int EventId { get; set; }

    public BuildEventReceiveDailyBonusRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public BuildEventReceiveDailyBonusRequest() { }
}

[MessagePackObject(true)]
public class CartoonLatestRequest { }

[MessagePackObject(true)]
public class CastleStoryReadRequest
{
    public int CastleStoryId { get; set; }

    public CastleStoryReadRequest(int castleStoryId)
    {
        this.CastleStoryId = castleStoryId;
    }

    public CastleStoryReadRequest() { }
}

[MessagePackObject(true)]
public class CharaAwakeRequest
{
    public Charas CharaId { get; set; }
    public int NextRarity { get; set; }

    public CharaAwakeRequest(Charas charaId, int nextRarity)
    {
        this.CharaId = charaId;
        this.NextRarity = nextRarity;
    }

    public CharaAwakeRequest() { }
}

[MessagePackObject(true)]
public class CharaBuildupManaRequest
{
    public Charas CharaId { get; set; }
    public IEnumerable<int> ManaCirclePieceIdList { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsUseGrowMaterial { get; set; }

    public CharaBuildupManaRequest(
        Charas charaId,
        IEnumerable<int> manaCirclePieceIdList,
        bool isUseGrowMaterial
    )
    {
        this.CharaId = charaId;
        this.ManaCirclePieceIdList = manaCirclePieceIdList;
        this.IsUseGrowMaterial = isUseGrowMaterial;
    }

    public CharaBuildupManaRequest() { }
}

[MessagePackObject(true)]
public class CharaBuildupPlatinumRequest
{
    public Charas CharaId { get; set; }

    public CharaBuildupPlatinumRequest(Charas charaId)
    {
        this.CharaId = charaId;
    }

    public CharaBuildupPlatinumRequest() { }
}

[MessagePackObject(true)]
public class CharaBuildupRequest
{
    public Charas CharaId { get; set; }
    public IEnumerable<AtgenEnemyPiece> MaterialList { get; set; }

    public CharaBuildupRequest(Charas charaId, IEnumerable<AtgenEnemyPiece> materialList)
    {
        this.CharaId = charaId;
        this.MaterialList = materialList;
    }

    public CharaBuildupRequest() { }
}

[MessagePackObject(true)]
public class CharaGetCharaUnitSetRequest
{
    public IEnumerable<Charas> CharaIdList { get; set; }

    public CharaGetCharaUnitSetRequest(IEnumerable<Charas> charaIdList)
    {
        this.CharaIdList = charaIdList;
    }

    public CharaGetCharaUnitSetRequest() { }
}

[MessagePackObject(true)]
public class CharaGetListRequest { }

[MessagePackObject(true)]
public class CharaLimitBreakAndBuildupManaRequest
{
    public Charas CharaId { get; set; }
    public int NextLimitBreakCount { get; set; }
    public IEnumerable<int> ManaCirclePieceIdList { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsUseGrowMaterial { get; set; }

    public CharaLimitBreakAndBuildupManaRequest(
        Charas charaId,
        int nextLimitBreakCount,
        IEnumerable<int> manaCirclePieceIdList,
        bool isUseGrowMaterial
    )
    {
        this.CharaId = charaId;
        this.NextLimitBreakCount = nextLimitBreakCount;
        this.ManaCirclePieceIdList = manaCirclePieceIdList;
        this.IsUseGrowMaterial = isUseGrowMaterial;
    }

    public CharaLimitBreakAndBuildupManaRequest() { }
}

[MessagePackObject(true)]
public class CharaLimitBreakRequest
{
    public Charas CharaId { get; set; }
    public int NextLimitBreakCount { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsUseGrowMaterial { get; set; }

    public CharaLimitBreakRequest(Charas charaId, int nextLimitBreakCount, bool isUseGrowMaterial)
    {
        this.CharaId = charaId;
        this.NextLimitBreakCount = nextLimitBreakCount;
        this.IsUseGrowMaterial = isUseGrowMaterial;
    }

    public CharaLimitBreakRequest() { }
}

[MessagePackObject(true)]
public class CharaResetPlusCountRequest
{
    public Charas CharaId { get; set; }
    public PlusCountType PlusCountType { get; set; }

    public CharaResetPlusCountRequest(Charas charaId, PlusCountType plusCountType)
    {
        this.CharaId = charaId;
        this.PlusCountType = plusCountType;
    }

    public CharaResetPlusCountRequest() { }
}

[MessagePackObject(true)]
public class CharaSetCharaUnitSetRequest
{
    public int UnitSetNo { get; set; }
    public string UnitSetName { get; set; }
    public Charas CharaId { get; set; }
    public AtgenRequestCharaUnitSetData RequestCharaUnitSetData { get; set; }

    public CharaSetCharaUnitSetRequest(
        int unitSetNo,
        string unitSetName,
        Charas charaId,
        AtgenRequestCharaUnitSetData requestCharaUnitSetData
    )
    {
        this.UnitSetNo = unitSetNo;
        this.UnitSetName = unitSetName;
        this.CharaId = charaId;
        this.RequestCharaUnitSetData = requestCharaUnitSetData;
    }

    public CharaSetCharaUnitSetRequest() { }
}

[MessagePackObject(true)]
public class CharaUnlockEditSkillRequest
{
    public Charas CharaId { get; set; }

    public CharaUnlockEditSkillRequest(Charas charaId)
    {
        this.CharaId = charaId;
    }

    public CharaUnlockEditSkillRequest() { }
}

[MessagePackObject(true)]
public class Clb01EventEntryRequest
{
    public int EventId { get; set; }

    public Clb01EventEntryRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public Clb01EventEntryRequest() { }
}

[MessagePackObject(true)]
public class Clb01EventGetEventDataRequest
{
    public int EventId { get; set; }

    public Clb01EventGetEventDataRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public Clb01EventGetEventDataRequest() { }
}

[MessagePackObject(true)]
public class Clb01EventReceiveClb01PointRewardRequest
{
    public int EventId { get; set; }

    public Clb01EventReceiveClb01PointRewardRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public Clb01EventReceiveClb01PointRewardRequest() { }
}

[MessagePackObject(true)]
public class CollectEventEntryRequest
{
    public int EventId { get; set; }

    public CollectEventEntryRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public CollectEventEntryRequest() { }
}

[MessagePackObject(true)]
public class CollectEventGetEventDataRequest
{
    public int EventId { get; set; }

    public CollectEventGetEventDataRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public CollectEventGetEventDataRequest() { }
}

[MessagePackObject(true)]
public class CombatEventEntryRequest
{
    public int EventId { get; set; }

    public CombatEventEntryRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public CombatEventEntryRequest() { }
}

[MessagePackObject(true)]
public class CombatEventGetEventDataRequest
{
    public int EventId { get; set; }

    public CombatEventGetEventDataRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public CombatEventGetEventDataRequest() { }
}

[MessagePackObject(true)]
public class CombatEventReceiveEventLocationRewardRequest
{
    public int EventId { get; set; }
    public int EventLocationRewardId { get; set; }

    public CombatEventReceiveEventLocationRewardRequest(int eventId, int eventLocationRewardId)
    {
        this.EventId = eventId;
        this.EventLocationRewardId = eventLocationRewardId;
    }

    public CombatEventReceiveEventLocationRewardRequest() { }
}

[MessagePackObject(true)]
public class CombatEventReceiveEventPointRewardRequest
{
    public int EventId { get; set; }

    public CombatEventReceiveEventPointRewardRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public CombatEventReceiveEventPointRewardRequest() { }
}

[MessagePackObject(true)]
public class CraftAssembleRequest
{
    public ulong WeaponKeyId { get; set; }
    public int TargetWeaponId { get; set; }
    public IEnumerable<GrowMaterialList> AssembleItemList { get; set; }
    public IEnumerable<AtgenWeaponSetList> WeaponSetList { get; set; }

    public CraftAssembleRequest(
        ulong weaponKeyId,
        int targetWeaponId,
        IEnumerable<GrowMaterialList> assembleItemList,
        IEnumerable<AtgenWeaponSetList> weaponSetList
    )
    {
        this.WeaponKeyId = weaponKeyId;
        this.TargetWeaponId = targetWeaponId;
        this.AssembleItemList = assembleItemList;
        this.WeaponSetList = weaponSetList;
    }

    public CraftAssembleRequest() { }
}

[MessagePackObject(true)]
public class CraftCreateRequest
{
    public int TargetWeaponId { get; set; }
    public int TargetWeaponQuantity { get; set; }
    public int ForceLimitBreak { get; set; }
    public IEnumerable<AtgenWeaponSetList> WeaponSetList { get; set; }

    public CraftCreateRequest(
        int targetWeaponId,
        int targetWeaponQuantity,
        int forceLimitBreak,
        IEnumerable<AtgenWeaponSetList> weaponSetList
    )
    {
        this.TargetWeaponId = targetWeaponId;
        this.TargetWeaponQuantity = targetWeaponQuantity;
        this.ForceLimitBreak = forceLimitBreak;
        this.WeaponSetList = weaponSetList;
    }

    public CraftCreateRequest() { }
}

[MessagePackObject(true)]
public class CraftDisassembleRequest
{
    public ulong WeaponKeyId { get; set; }
    public PaymentTypes PaymentType { get; set; }

    public CraftDisassembleRequest(ulong weaponKeyId, PaymentTypes paymentType)
    {
        this.WeaponKeyId = weaponKeyId;
        this.PaymentType = paymentType;
    }

    public CraftDisassembleRequest() { }
}

[MessagePackObject(true)]
public class CraftResetNewRequest
{
    public IEnumerable<int> WeaponIdList { get; set; }

    public CraftResetNewRequest(IEnumerable<int> weaponIdList)
    {
        this.WeaponIdList = weaponIdList;
    }

    public CraftResetNewRequest() { }
}

[MessagePackObject(true)]
public class DeployGetDeployVersionRequest { }

[MessagePackObject(true)]
public class DmodeBuildupServitorPassiveRequest
{
    public IEnumerable<DmodeServitorPassiveList> RequestBuildupPassiveList { get; set; }

    public DmodeBuildupServitorPassiveRequest(
        IEnumerable<DmodeServitorPassiveList> requestBuildupPassiveList
    )
    {
        this.RequestBuildupPassiveList = requestBuildupPassiveList;
    }

    public DmodeBuildupServitorPassiveRequest() { }
}

[MessagePackObject(true)]
public class DmodeDungeonFinishRequest
{
    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsGameOver { get; set; }

    public DmodeDungeonFinishRequest(bool isGameOver)
    {
        this.IsGameOver = isGameOver;
    }

    public DmodeDungeonFinishRequest() { }
}

#nullable enable
[MessagePackObject(true)]
public class DmodeDungeonFloorRequest
{
    public DmodePlayRecord? DmodePlayRecord { get; set; }

    public DmodeDungeonFloorRequest(DmodePlayRecord? dmodePlayRecord)
    {
        this.DmodePlayRecord = dmodePlayRecord;
    }

    public DmodeDungeonFloorRequest() { }
}

#nullable disable

[MessagePackObject(true)]
public class DmodeDungeonFloorSkipRequest { }

[MessagePackObject(true)]
public class DmodeDungeonRestartRequest { }

[MessagePackObject(true)]
public class DmodeDungeonStartRequest
{
    public Charas CharaId { get; set; }
    public int StartFloorNum { get; set; }
    public int ServitorId { get; set; }
    public IEnumerable<Charas> BringEditSkillCharaIdList { get; set; }

    public DmodeDungeonStartRequest(
        Charas charaId,
        int startFloorNum,
        int servitorId,
        IEnumerable<Charas> bringEditSkillCharaIdList
    )
    {
        this.CharaId = charaId;
        this.StartFloorNum = startFloorNum;
        this.ServitorId = servitorId;
        this.BringEditSkillCharaIdList = bringEditSkillCharaIdList;
    }

    public DmodeDungeonStartRequest() { }
}

[MessagePackObject(true)]
public class DmodeDungeonSystemHaltRequest { }

[MessagePackObject(true)]
public class DmodeDungeonUserHaltRequest { }

[MessagePackObject(true)]
public class DmodeEntryRequest { }

[MessagePackObject(true)]
public class DmodeExpeditionFinishRequest { }

[MessagePackObject(true)]
public class DmodeExpeditionForceFinishRequest { }

[MessagePackObject(true)]
public class DmodeExpeditionStartRequest
{
    public int TargetFloorNum { get; set; }
    public IEnumerable<Charas> CharaIdList { get; set; }

    public DmodeExpeditionStartRequest(int targetFloorNum, IEnumerable<Charas> charaIdList)
    {
        this.TargetFloorNum = targetFloorNum;
        this.CharaIdList = charaIdList;
    }

    public DmodeExpeditionStartRequest() { }
}

[MessagePackObject(true)]
public class DmodeGetDataRequest { }

[MessagePackObject(true)]
public class DmodeReadStoryRequest
{
    public int DmodeStoryId { get; set; }

    public DmodeReadStoryRequest(int dmodeStoryId)
    {
        this.DmodeStoryId = dmodeStoryId;
    }

    public DmodeReadStoryRequest() { }
}

[MessagePackObject(true)]
public class DragonBuildupRequest
{
    public ulong BaseDragonKeyId { get; set; }
    public IEnumerable<GrowMaterialList> GrowMaterialList { get; set; }

    public DragonBuildupRequest(
        ulong baseDragonKeyId,
        IEnumerable<GrowMaterialList> growMaterialList
    )
    {
        this.BaseDragonKeyId = baseDragonKeyId;
        this.GrowMaterialList = growMaterialList;
    }

    public DragonBuildupRequest() { }
}

[MessagePackObject(true)]
public class DragonBuyGiftToSendMultipleRequest
{
    public Dragons DragonId { get; set; }
    public IEnumerable<DragonGifts> DragonGiftIdList { get; set; }

    public DragonBuyGiftToSendMultipleRequest(
        Dragons dragonId,
        IEnumerable<DragonGifts> dragonGiftIdList
    )
    {
        this.DragonId = dragonId;
        this.DragonGiftIdList = dragonGiftIdList;
    }

    public DragonBuyGiftToSendMultipleRequest() { }
}

[MessagePackObject(true)]
public class DragonBuyGiftToSendRequest
{
    public Dragons DragonId { get; set; }
    public DragonGifts DragonGiftId { get; set; }

    public DragonBuyGiftToSendRequest(Dragons dragonId, DragonGifts dragonGiftId)
    {
        this.DragonId = dragonId;
        this.DragonGiftId = dragonGiftId;
    }

    public DragonBuyGiftToSendRequest() { }
}

[MessagePackObject(true)]
public class DragonGetContactDataRequest { }

[MessagePackObject(true)]
public class DragonLimitBreakRequest
{
    public ulong BaseDragonKeyId { get; set; }
    public IEnumerable<LimitBreakGrowList> LimitBreakGrowList { get; set; }

    public DragonLimitBreakRequest(
        ulong baseDragonKeyId,
        IEnumerable<LimitBreakGrowList> limitBreakGrowList
    )
    {
        this.BaseDragonKeyId = baseDragonKeyId;
        this.LimitBreakGrowList = limitBreakGrowList;
    }

    public DragonLimitBreakRequest() { }
}

[MessagePackObject(true)]
public class DragonResetPlusCountRequest
{
    public ulong DragonKeyId { get; set; }
    public PlusCountType PlusCountType { get; set; }

    public DragonResetPlusCountRequest(ulong dragonKeyId, PlusCountType plusCountType)
    {
        this.DragonKeyId = dragonKeyId;
        this.PlusCountType = plusCountType;
    }

    public DragonResetPlusCountRequest() { }
}

[MessagePackObject(true)]
public class DragonSellRequest
{
    public IEnumerable<ulong> DragonKeyIdList { get; set; }

    public DragonSellRequest(IEnumerable<ulong> dragonKeyIdList)
    {
        this.DragonKeyIdList = dragonKeyIdList;
    }

    public DragonSellRequest() { }
}

[MessagePackObject(true)]
public class DragonSendGiftMultipleRequest
{
    public Dragons DragonId { get; set; }
    public DragonGifts DragonGiftId { get; set; }
    public int Quantity { get; set; }

    public DragonSendGiftMultipleRequest(Dragons dragonId, DragonGifts dragonGiftId, int quantity)
    {
        this.DragonId = dragonId;
        this.DragonGiftId = dragonGiftId;
        this.Quantity = quantity;
    }

    public DragonSendGiftMultipleRequest() { }
}

[MessagePackObject(true)]
public class DragonSendGiftRequest
{
    public Dragons DragonId { get; set; }
    public DragonGifts DragonGiftId { get; set; }

    public DragonSendGiftRequest(Dragons dragonId, DragonGifts dragonGiftId)
    {
        this.DragonId = dragonId;
        this.DragonGiftId = dragonGiftId;
    }

    public DragonSendGiftRequest() { }
}

[MessagePackObject(true)]
public class DragonSetLockRequest
{
    public ulong DragonKeyId { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsLock { get; set; }

    public DragonSetLockRequest(ulong dragonKeyId, bool isLock)
    {
        this.DragonKeyId = dragonKeyId;
        this.IsLock = isLock;
    }

    public DragonSetLockRequest() { }
}

[MessagePackObject(true)]
public class DreamAdventureClearRequest
{
    public int Difficulty { get; set; }

    public DreamAdventureClearRequest(int difficulty)
    {
        this.Difficulty = difficulty;
    }

    public DreamAdventureClearRequest() { }
}

[MessagePackObject(true)]
public class DreamAdventurePlayRequest
{
    public int Difficulty { get; set; }

    public DreamAdventurePlayRequest(int difficulty)
    {
        this.Difficulty = difficulty;
    }

    public DreamAdventurePlayRequest() { }
}

[MessagePackObject(true)]
public class DungeonFailRequest
{
    public string DungeonKey { get; set; }
    public int FailState { get; set; }
    public int NoPlayFlg { get; set; }

    public DungeonFailRequest(string dungeonKey, int failState, int noPlayFlg)
    {
        this.DungeonKey = dungeonKey;
        this.FailState = failState;
        this.NoPlayFlg = noPlayFlg;
    }

    public DungeonFailRequest() { }
}

[MessagePackObject(true)]
public class DungeonGetAreaOddsRequest
{
    public string DungeonKey { get; set; }
    public int AreaIdx { get; set; }

    public DungeonGetAreaOddsRequest(string dungeonKey, int areaIdx)
    {
        this.DungeonKey = dungeonKey;
        this.AreaIdx = areaIdx;
    }

    public DungeonGetAreaOddsRequest() { }
}

[MessagePackObject(true)]
public class DungeonReceiveQuestBonusRequest
{
    public int QuestEventId { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsReceive { get; set; }
    public int ReceiveBonusCount { get; set; }

    public DungeonReceiveQuestBonusRequest(int questEventId, bool isReceive, int receiveBonusCount)
    {
        this.QuestEventId = questEventId;
        this.IsReceive = isReceive;
        this.ReceiveBonusCount = receiveBonusCount;
    }

    public DungeonReceiveQuestBonusRequest() { }
}

[MessagePackObject(true)]
public class DungeonRecordRecordMultiRequest
{
    public PlayRecord PlayRecord { get; set; }
    public string DungeonKey { get; set; }
    public IEnumerable<ulong> ConnectingViewerIdList { get; set; }
    public int NoPlayFlg { get; set; }

    public DungeonRecordRecordMultiRequest(
        PlayRecord playRecord,
        string dungeonKey,
        IEnumerable<ulong> connectingViewerIdList,
        int noPlayFlg
    )
    {
        this.PlayRecord = playRecord;
        this.DungeonKey = dungeonKey;
        this.ConnectingViewerIdList = connectingViewerIdList;
        this.NoPlayFlg = noPlayFlg;
    }

    public DungeonRecordRecordMultiRequest() { }
}

#nullable restore
[MessagePackObject(true)]
public class DungeonRecordRecordRequest
{
    public required PlayRecord PlayRecord { get; set; }
    public required string DungeonKey { get; set; }
    public int RepeatState { get; set; }
    public string? RepeatKey { get; set; }

    public DungeonRecordRecordRequest(
        PlayRecord playRecord,
        string dungeonKey,
        int repeatState,
        string repeatKey
    )
    {
        this.PlayRecord = playRecord;
        this.DungeonKey = dungeonKey;
        this.RepeatState = repeatState;
        this.RepeatKey = repeatKey;
    }

    public DungeonRecordRecordRequest() { }
}

#nullable  disable

[MessagePackObject(true)]
public class DungeonRetryRequest
{
    public string DungeonKey { get; set; }
    public PaymentTypes PaymentType { get; set; }

    public DungeonRetryRequest(string dungeonKey, PaymentTypes paymentType)
    {
        this.DungeonKey = dungeonKey;
        this.PaymentType = paymentType;
    }

    public DungeonRetryRequest() { }
}

[MessagePackObject(true)]
public class DungeonSkipStartAssignUnitRequest
{
    public int QuestId { get; set; }
    public IEnumerable<PartySettingList> RequestPartySettingList { get; set; }
    public ulong SupportViewerId { get; set; }
    public int PlayCount { get; set; }
    public int BetCount { get; set; }

    public DungeonSkipStartAssignUnitRequest(
        int questId,
        IEnumerable<PartySettingList> requestPartySettingList,
        ulong supportViewerId,
        int playCount,
        int betCount
    )
    {
        this.QuestId = questId;
        this.RequestPartySettingList = requestPartySettingList;
        this.SupportViewerId = supportViewerId;
        this.PlayCount = playCount;
        this.BetCount = betCount;
    }

    public DungeonSkipStartAssignUnitRequest() { }
}

[MessagePackObject(true)]
public class DungeonSkipStartMultipleQuestAssignUnitRequest
{
    public IEnumerable<PartySettingList> RequestPartySettingList { get; set; }
    public IEnumerable<AtgenRequestQuestMultipleList> RequestQuestMultipleList { get; set; }
    public ulong SupportViewerId { get; set; }

    public DungeonSkipStartMultipleQuestAssignUnitRequest(
        IEnumerable<PartySettingList> requestPartySettingList,
        IEnumerable<AtgenRequestQuestMultipleList> requestQuestMultipleList,
        ulong supportViewerId
    )
    {
        this.RequestPartySettingList = requestPartySettingList;
        this.RequestQuestMultipleList = requestQuestMultipleList;
        this.SupportViewerId = supportViewerId;
    }

    public DungeonSkipStartMultipleQuestAssignUnitRequest() { }
}

[MessagePackObject(true)]
public class DungeonSkipStartMultipleQuestRequest
{
    public int PartyNo { get; set; }
    public IEnumerable<AtgenRequestQuestMultipleList> RequestQuestMultipleList { get; set; }
    public ulong SupportViewerId { get; set; }

    public DungeonSkipStartMultipleQuestRequest(
        int partyNo,
        IEnumerable<AtgenRequestQuestMultipleList> requestQuestMultipleList,
        ulong supportViewerId
    )
    {
        this.PartyNo = partyNo;
        this.RequestQuestMultipleList = requestQuestMultipleList;
        this.SupportViewerId = supportViewerId;
    }

    public DungeonSkipStartMultipleQuestRequest() { }
}

[MessagePackObject(true)]
public class DungeonSkipStartRequest
{
    public int QuestId { get; set; }
    public int PartyNo { get; set; }
    public ulong SupportViewerId { get; set; }
    public int PlayCount { get; set; }
    public int BetCount { get; set; }

    public DungeonSkipStartRequest(
        int questId,
        int partyNo,
        ulong supportViewerId,
        int playCount,
        int betCount
    )
    {
        this.QuestId = questId;
        this.PartyNo = partyNo;
        this.SupportViewerId = supportViewerId;
        this.PlayCount = playCount;
        this.BetCount = betCount;
    }

    public DungeonSkipStartRequest() { }
}

[MessagePackObject(true)]
public class DungeonStartStartAssignUnitRequest
{
    public int QuestId { get; set; }
    public IList<PartySettingList> RequestPartySettingList { get; set; }
    public int BetCount { get; set; }
    public int RepeatState { get; set; }
    public ulong SupportViewerId { get; set; }
    public RepeatSetting RepeatSetting { get; set; }

    public DungeonStartStartAssignUnitRequest(
        int questId,
        IList<PartySettingList> requestPartySettingList,
        int betCount,
        int repeatState,
        ulong supportViewerId,
        RepeatSetting repeatSetting
    )
    {
        this.QuestId = questId;
        this.RequestPartySettingList = requestPartySettingList;
        this.BetCount = betCount;
        this.RepeatState = repeatState;
        this.SupportViewerId = supportViewerId;
        this.RepeatSetting = repeatSetting;
    }

    public DungeonStartStartAssignUnitRequest() { }
}

[MessagePackObject(true)]
public class DungeonStartStartMultiAssignUnitRequest
{
    public int QuestId { get; set; }
    public IList<PartySettingList> RequestPartySettingList { get; set; }

    public DungeonStartStartMultiAssignUnitRequest(
        int questId,
        IList<PartySettingList> requestPartySettingList
    )
    {
        this.QuestId = questId;
        this.RequestPartySettingList = requestPartySettingList;
    }

    public DungeonStartStartMultiAssignUnitRequest() { }
}

[MessagePackObject(true)]
public class DungeonStartStartMultiRequest
{
    public int QuestId { get; set; }
    public int PartyNo { get; set; }
    public IList<int> PartyNoList { get; set; }

    public DungeonStartStartMultiRequest(int questId, int partyNo, IList<int> partyNoList)
    {
        this.QuestId = questId;
        this.PartyNo = partyNo;
        this.PartyNoList = partyNoList;
    }

    public DungeonStartStartMultiRequest() { }
}

#nullable restore

[MessagePackObject(true)]
public class DungeonStartStartRequest
{
    public int QuestId { get; set; }
    public int PartyNo { get; set; }
    public required List<int> PartyNoList { get; set; }
    public int BetCount { get; set; }
    public int RepeatState { get; set; }
    public ulong SupportViewerId { get; set; }
    public RepeatSetting? RepeatSetting { get; set; }

    public DungeonStartStartRequest(
        int questId,
        int partyNo,
        List<int> partyNoList,
        int betCount,
        int repeatState,
        ulong supportViewerId,
        RepeatSetting repeatSetting
    )
    {
        this.QuestId = questId;
        this.PartyNo = partyNo;
        this.PartyNoList = partyNoList;
        this.BetCount = betCount;
        this.RepeatState = repeatState;
        this.SupportViewerId = supportViewerId;
        this.RepeatSetting = repeatSetting;
    }

    public DungeonStartStartRequest() { }
}

#nullable disable

[MessagePackObject(true)]
public class EarnEventEntryRequest
{
    public int EventId { get; set; }

    public EarnEventEntryRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public EarnEventEntryRequest() { }
}

[MessagePackObject(true)]
public class EarnEventGetEventDataRequest
{
    public int EventId { get; set; }

    public EarnEventGetEventDataRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public EarnEventGetEventDataRequest() { }
}

[MessagePackObject(true)]
public class EarnEventReceiveEventPointRewardRequest
{
    public int EventId { get; set; }

    public EarnEventReceiveEventPointRewardRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public EarnEventReceiveEventPointRewardRequest() { }
}

[MessagePackObject(true)]
public class EmblemGetListRequest { }

[MessagePackObject(true)]
public class EmblemSetRequest
{
    public Emblems EmblemId { get; set; }

    public EmblemSetRequest(Emblems emblemId)
    {
        this.EmblemId = emblemId;
    }

    public EmblemSetRequest() { }
}

[MessagePackObject(true)]
public class EulaAgreeAgreeRequest
{
    public string IdToken { get; set; }
    public string Region { get; set; }
    public string Lang { get; set; }
    public int EulaVersion { get; set; }
    public int PrivacyPolicyVersion { get; set; }
    public string Uuid { get; set; }

    public EulaAgreeAgreeRequest(
        string idToken,
        string region,
        string lang,
        int eulaVersion,
        int privacyPolicyVersion,
        string uuid
    )
    {
        this.IdToken = idToken;
        this.Region = region;
        this.Lang = lang;
        this.EulaVersion = eulaVersion;
        this.PrivacyPolicyVersion = privacyPolicyVersion;
        this.Uuid = uuid;
    }

    public EulaAgreeAgreeRequest() { }
}

[MessagePackObject(true)]
public class EulaGetVersionListRequest { }

[MessagePackObject(true)]
public class EulaGetVersionRequest
{
    public string IdToken { get; set; }
    public string Region { get; set; }
    public string Lang { get; set; }

    public EulaGetVersionRequest(string idToken, string region, string lang)
    {
        this.IdToken = idToken;
        this.Region = region;
        this.Lang = lang;
    }

    public EulaGetVersionRequest() { }
}

[MessagePackObject(true)]
public class EventDamageGetTotalDamageHistoryRequest
{
    public int EventId { get; set; }

    public EventDamageGetTotalDamageHistoryRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public EventDamageGetTotalDamageHistoryRequest() { }
}

[MessagePackObject(true)]
public class EventDamageReceiveDamageRewardRequest
{
    public int EventId { get; set; }

    public EventDamageReceiveDamageRewardRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public EventDamageReceiveDamageRewardRequest() { }
}

[MessagePackObject(true)]
public class EventStoryReadRequest
{
    public int EventStoryId { get; set; }

    public EventStoryReadRequest(int eventStoryId)
    {
        this.EventStoryId = eventStoryId;
    }

    public EventStoryReadRequest() { }
}

[MessagePackObject(true)]
public class EventSummonExecRequest
{
    public int EventId { get; set; }
    public int Count { get; set; }
    public int IsEnableStopByTarget { get; set; }

    public EventSummonExecRequest(int eventId, int count, int isEnableStopByTarget)
    {
        this.EventId = eventId;
        this.Count = count;
        this.IsEnableStopByTarget = isEnableStopByTarget;
    }

    public EventSummonExecRequest() { }
}

[MessagePackObject(true)]
public class EventSummonGetDataRequest
{
    public int EventId { get; set; }

    public EventSummonGetDataRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public EventSummonGetDataRequest() { }
}

[MessagePackObject(true)]
public class EventSummonResetRequest
{
    public int EventId { get; set; }

    public EventSummonResetRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public EventSummonResetRequest() { }
}

[MessagePackObject(true)]
public class EventTradeGetListRequest
{
    public int TradeGroupId { get; set; }

    public EventTradeGetListRequest(int tradeGroupId)
    {
        this.TradeGroupId = tradeGroupId;
    }

    public EventTradeGetListRequest() { }
}

[MessagePackObject(true)]
public class EventTradeTradeRequest
{
    public int TradeGroupId { get; set; }
    public int TradeId { get; set; }
    public int TradeCount { get; set; }

    public EventTradeTradeRequest(int tradeGroupId, int tradeId, int tradeCount)
    {
        this.TradeGroupId = tradeGroupId;
        this.TradeId = tradeId;
        this.TradeCount = tradeCount;
    }

    public EventTradeTradeRequest() { }
}

[MessagePackObject(true)]
public class ExchangeGetUnitListRequest
{
    public int ExchangeTicketId { get; set; }

    public ExchangeGetUnitListRequest(int exchangeTicketId)
    {
        this.ExchangeTicketId = exchangeTicketId;
    }

    public ExchangeGetUnitListRequest() { }
}

[MessagePackObject(true)]
public class ExchangeSelectUnitRequest
{
    public int ExchangeTicketId { get; set; }
    public AtgenDuplicateEntityList SelectedUnit { get; set; }

    public ExchangeSelectUnitRequest(int exchangeTicketId, AtgenDuplicateEntityList selectedUnit)
    {
        this.ExchangeTicketId = exchangeTicketId;
        this.SelectedUnit = selectedUnit;
    }

    public ExchangeSelectUnitRequest() { }
}

[MessagePackObject(true)]
public class ExHunterEventEntryRequest
{
    public int EventId { get; set; }

    public ExHunterEventEntryRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public ExHunterEventEntryRequest() { }
}

[MessagePackObject(true)]
public class ExHunterEventGetEventDataRequest
{
    public int EventId { get; set; }

    public ExHunterEventGetEventDataRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public ExHunterEventGetEventDataRequest() { }
}

[MessagePackObject(true)]
public class ExHunterEventReceiveExHunterPointRewardRequest
{
    public int EventId { get; set; }
    public IEnumerable<int> ExHunterEventRewardIdList { get; set; }

    public ExHunterEventReceiveExHunterPointRewardRequest(
        int eventId,
        IEnumerable<int> exHunterEventRewardIdList
    )
    {
        this.EventId = eventId;
        this.ExHunterEventRewardIdList = exHunterEventRewardIdList;
    }

    public ExHunterEventReceiveExHunterPointRewardRequest() { }
}

[MessagePackObject(true)]
public class ExRushEventEntryRequest
{
    public int EventId { get; set; }

    public ExRushEventEntryRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public ExRushEventEntryRequest() { }
}

[MessagePackObject(true)]
public class ExRushEventGetEventDataRequest
{
    public int EventId { get; set; }

    public ExRushEventGetEventDataRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public ExRushEventGetEventDataRequest() { }
}

[MessagePackObject(true)]
public class FortAddCarpenterRequest
{
    public PaymentTypes PaymentType { get; set; }

    public FortAddCarpenterRequest(PaymentTypes paymentType)
    {
        this.PaymentType = paymentType;
    }

    public FortAddCarpenterRequest() { }
}

[MessagePackObject(true)]
public class FortBuildAtOnceRequest
{
    public long BuildId { get; set; }
    public PaymentTypes PaymentType { get; set; }

    public FortBuildAtOnceRequest(long buildId, PaymentTypes paymentType)
    {
        this.BuildId = buildId;
        this.PaymentType = paymentType;
    }

    public FortBuildAtOnceRequest() { }
}

[MessagePackObject(true)]
public class FortBuildCancelRequest
{
    public long BuildId { get; set; }

    public FortBuildCancelRequest(long buildId)
    {
        this.BuildId = buildId;
    }

    public FortBuildCancelRequest() { }
}

[MessagePackObject(true)]
public class FortBuildEndRequest
{
    public long BuildId { get; set; }

    public FortBuildEndRequest(long buildId)
    {
        this.BuildId = buildId;
    }

    public FortBuildEndRequest() { }
}

[MessagePackObject(true)]
public class FortBuildStartRequest
{
    public FortPlants FortPlantId { get; set; }
    public int PositionX { get; set; }
    public int PositionZ { get; set; }

    public FortBuildStartRequest(FortPlants fortPlantId, int positionX, int positionZ)
    {
        this.FortPlantId = fortPlantId;
        this.PositionX = positionX;
        this.PositionZ = positionZ;
    }

    public FortBuildStartRequest() { }
}

[MessagePackObject(true)]
public class FortGetDataRequest { }

[MessagePackObject(true)]
public class FortGetMultiIncomeRequest
{
    public IEnumerable<long> BuildIdList { get; set; }

    public FortGetMultiIncomeRequest(IEnumerable<long> buildIdList)
    {
        this.BuildIdList = buildIdList;
    }

    public FortGetMultiIncomeRequest() { }
}

[MessagePackObject(true)]
public class FortLevelupAtOnceRequest
{
    public long BuildId { get; set; }
    public PaymentTypes PaymentType { get; set; }

    public FortLevelupAtOnceRequest(long buildId, PaymentTypes paymentType)
    {
        this.BuildId = buildId;
        this.PaymentType = paymentType;
    }

    public FortLevelupAtOnceRequest() { }
}

[MessagePackObject(true)]
public class FortLevelupCancelRequest
{
    public long BuildId { get; set; }

    public FortLevelupCancelRequest(long buildId)
    {
        this.BuildId = buildId;
    }

    public FortLevelupCancelRequest() { }
}

[MessagePackObject(true)]
public class FortLevelupEndRequest
{
    public long BuildId { get; set; }

    public FortLevelupEndRequest(long buildId)
    {
        this.BuildId = buildId;
    }

    public FortLevelupEndRequest() { }
}

[MessagePackObject(true)]
public class FortLevelupStartRequest
{
    public long BuildId { get; set; }

    public FortLevelupStartRequest(long buildId)
    {
        this.BuildId = buildId;
    }

    public FortLevelupStartRequest() { }
}

[MessagePackObject(true)]
public class FortMoveRequest
{
    public long BuildId { get; set; }
    public int AfterPositionX { get; set; }
    public int AfterPositionZ { get; set; }

    public FortMoveRequest(long buildId, int afterPositionX, int afterPositionZ)
    {
        this.BuildId = buildId;
        this.AfterPositionX = afterPositionX;
        this.AfterPositionZ = afterPositionZ;
    }

    public FortMoveRequest() { }
}

[MessagePackObject(true)]
public class FortSetNewFortPlantRequest
{
    public IEnumerable<FortPlants> FortPlantIdList { get; set; }

    public FortSetNewFortPlantRequest(IEnumerable<FortPlants> fortPlantIdList)
    {
        this.FortPlantIdList = fortPlantIdList;
    }

    public FortSetNewFortPlantRequest() { }
}

[MessagePackObject(true)]
public class FriendAllReplyDenyRequest { }

[MessagePackObject(true)]
public class FriendApplyListRequest { }

[MessagePackObject(true)]
public class FriendAutoSearchRequest { }

[MessagePackObject(true)]
public class FriendDeleteRequest
{
    public ulong FriendId { get; set; }

    public FriendDeleteRequest(ulong friendId)
    {
        this.FriendId = friendId;
    }

    public FriendDeleteRequest() { }
}

[MessagePackObject(true)]
public class FriendFriendIndexRequest { }

[MessagePackObject(true)]
public class FriendFriendListRequest { }

[MessagePackObject(true)]
public class FriendGetSupportCharaDetailRequest
{
    public ulong SupportViewerId { get; set; }

    public FriendGetSupportCharaDetailRequest(ulong supportViewerId)
    {
        this.SupportViewerId = supportViewerId;
    }

    public FriendGetSupportCharaDetailRequest() { }
}

[MessagePackObject(true)]
public class FriendGetSupportCharaRequest { }

[MessagePackObject(true)]
public class FriendIdSearchRequest
{
    public ulong SearchId { get; set; }

    public FriendIdSearchRequest(ulong searchId)
    {
        this.SearchId = searchId;
    }

    public FriendIdSearchRequest() { }
}

[MessagePackObject(true)]
public class FriendReplyRequest
{
    public ulong FriendId { get; set; }
    public int Reply { get; set; }

    public FriendReplyRequest(ulong friendId, int reply)
    {
        this.FriendId = friendId;
        this.Reply = reply;
    }

    public FriendReplyRequest() { }
}

[MessagePackObject(true)]
public class FriendRequestCancelRequest
{
    public ulong FriendId { get; set; }

    public FriendRequestCancelRequest(ulong friendId)
    {
        this.FriendId = friendId;
    }

    public FriendRequestCancelRequest() { }
}

[MessagePackObject(true)]
public class FriendRequestListRequest { }

[MessagePackObject(true)]
public class FriendRequestRequest
{
    public ulong FriendId { get; set; }

    public FriendRequestRequest(ulong friendId)
    {
        this.FriendId = friendId;
    }

    public FriendRequestRequest() { }
}

[MessagePackObject(true)]
public class FriendSetSupportCharaRequest
{
    public Charas CharaId { get; set; }
    public ulong DragonKeyId { get; set; }
    public ulong WeaponKeyId { get; set; }
    public ulong AmuletKeyId { get; set; }
    public ulong Amulet2KeyId { get; set; }
    public int WeaponBodyId { get; set; }
    public int CrestSlotType1CrestId1 { get; set; }
    public int CrestSlotType1CrestId2 { get; set; }
    public int CrestSlotType1CrestId3 { get; set; }
    public int CrestSlotType2CrestId1 { get; set; }
    public int CrestSlotType2CrestId2 { get; set; }
    public int CrestSlotType3CrestId1 { get; set; }
    public int CrestSlotType3CrestId2 { get; set; }
    public ulong TalismanKeyId { get; set; }

    public FriendSetSupportCharaRequest(
        Charas charaId,
        ulong dragonKeyId,
        ulong weaponKeyId,
        ulong amuletKeyId,
        ulong amulet2KeyId,
        int weaponBodyId,
        int crestSlotType1CrestId1,
        int crestSlotType1CrestId2,
        int crestSlotType1CrestId3,
        int crestSlotType2CrestId1,
        int crestSlotType2CrestId2,
        int crestSlotType3CrestId1,
        int crestSlotType3CrestId2,
        ulong talismanKeyId
    )
    {
        this.CharaId = charaId;
        this.DragonKeyId = dragonKeyId;
        this.WeaponKeyId = weaponKeyId;
        this.AmuletKeyId = amuletKeyId;
        this.Amulet2KeyId = amulet2KeyId;
        this.WeaponBodyId = weaponBodyId;
        this.CrestSlotType1CrestId1 = crestSlotType1CrestId1;
        this.CrestSlotType1CrestId2 = crestSlotType1CrestId2;
        this.CrestSlotType1CrestId3 = crestSlotType1CrestId3;
        this.CrestSlotType2CrestId1 = crestSlotType2CrestId1;
        this.CrestSlotType2CrestId2 = crestSlotType2CrestId2;
        this.CrestSlotType3CrestId1 = crestSlotType3CrestId1;
        this.CrestSlotType3CrestId2 = crestSlotType3CrestId2;
        this.TalismanKeyId = talismanKeyId;
    }

    public FriendSetSupportCharaRequest() { }
}

[MessagePackObject(true)]
public class GuildChatGetNewMessageListRequest
{
    public int GuildId { get; set; }
    public ulong ChatMessageId { get; set; }

    public GuildChatGetNewMessageListRequest(int guildId, ulong chatMessageId)
    {
        this.GuildId = guildId;
        this.ChatMessageId = chatMessageId;
    }

    public GuildChatGetNewMessageListRequest() { }
}

[MessagePackObject(true)]
public class GuildChatGetOldMessageListRequest
{
    public int GuildId { get; set; }
    public ulong ChatMessageId { get; set; }

    public GuildChatGetOldMessageListRequest(int guildId, ulong chatMessageId)
    {
        this.GuildId = guildId;
        this.ChatMessageId = chatMessageId;
    }

    public GuildChatGetOldMessageListRequest() { }
}

[MessagePackObject(true)]
public class GuildChatPostMessageStampRequest
{
    public int GuildId { get; set; }
    public ulong ChatMessageId { get; set; }
    public int ChatMessageStampId { get; set; }

    public GuildChatPostMessageStampRequest(
        int guildId,
        ulong chatMessageId,
        int chatMessageStampId
    )
    {
        this.GuildId = guildId;
        this.ChatMessageId = chatMessageId;
        this.ChatMessageStampId = chatMessageStampId;
    }

    public GuildChatPostMessageStampRequest() { }
}

[MessagePackObject(true)]
public class GuildChatPostMessageTextRequest
{
    public int GuildId { get; set; }
    public ulong ChatMessageId { get; set; }
    public string ChatMessageText { get; set; }

    public GuildChatPostMessageTextRequest(int guildId, ulong chatMessageId, string chatMessageText)
    {
        this.GuildId = guildId;
        this.ChatMessageId = chatMessageId;
        this.ChatMessageText = chatMessageText;
    }

    public GuildChatPostMessageTextRequest() { }
}

[MessagePackObject(true)]
public class GuildChatPostReportRequest
{
    public int GuildId { get; set; }
    public ulong ChatMessageId { get; set; }
    public int ReportCategoryId { get; set; }
    public string Message { get; set; }

    public GuildChatPostReportRequest(
        int guildId,
        ulong chatMessageId,
        int reportCategoryId,
        string message
    )
    {
        this.GuildId = guildId;
        this.ChatMessageId = chatMessageId;
        this.ReportCategoryId = reportCategoryId;
        this.Message = message;
    }

    public GuildChatPostReportRequest() { }
}

[MessagePackObject(true)]
public class GuildDisbandRequest
{
    public int GuildId { get; set; }

    public GuildDisbandRequest(int guildId)
    {
        this.GuildId = guildId;
    }

    public GuildDisbandRequest() { }
}

[MessagePackObject(true)]
public class GuildDropMemberRequest
{
    public int GuildId { get; set; }
    public ulong TargetViewerId { get; set; }

    public GuildDropMemberRequest(int guildId, ulong targetViewerId)
    {
        this.GuildId = guildId;
        this.TargetViewerId = targetViewerId;
    }

    public GuildDropMemberRequest() { }
}

[MessagePackObject(true)]
public class GuildEstablishRequest
{
    public string GuildName { get; set; }
    public int GuildEmblemId { get; set; }
    public int JoiningConditionType { get; set; }
    public int ActivityPolicyType { get; set; }
    public string GuildIntroduction { get; set; }
    public string GuildBoard { get; set; }

    public GuildEstablishRequest(
        string guildName,
        int guildEmblemId,
        int joiningConditionType,
        int activityPolicyType,
        string guildIntroduction,
        string guildBoard
    )
    {
        this.GuildName = guildName;
        this.GuildEmblemId = guildEmblemId;
        this.JoiningConditionType = joiningConditionType;
        this.ActivityPolicyType = activityPolicyType;
        this.GuildIntroduction = guildIntroduction;
        this.GuildBoard = guildBoard;
    }

    public GuildEstablishRequest() { }
}

[MessagePackObject(true)]
public class GuildGetGuildApplyDataRequest
{
    public int GuildId { get; set; }

    public GuildGetGuildApplyDataRequest(int guildId)
    {
        this.GuildId = guildId;
    }

    public GuildGetGuildApplyDataRequest() { }
}

[MessagePackObject(true)]
public class GuildGetGuildMemberDataRequest
{
    public int GuildId { get; set; }

    public GuildGetGuildMemberDataRequest(int guildId)
    {
        this.GuildId = guildId;
    }

    public GuildGetGuildMemberDataRequest() { }
}

[MessagePackObject(true)]
public class GuildIndexRequest { }

[MessagePackObject(true)]
public class GuildInviteGetGuildInviteReceiveDataRequest { }

[MessagePackObject(true)]
public class GuildInviteGetGuildInviteSendDataRequest
{
    public int GuildId { get; set; }

    public GuildInviteGetGuildInviteSendDataRequest(int guildId)
    {
        this.GuildId = guildId;
    }

    public GuildInviteGetGuildInviteSendDataRequest() { }
}

[MessagePackObject(true)]
public class GuildInviteInviteCancelRequest
{
    public int GuildId { get; set; }
    public ulong GuildInviteId { get; set; }

    public GuildInviteInviteCancelRequest(int guildId, ulong guildInviteId)
    {
        this.GuildId = guildId;
        this.GuildInviteId = guildInviteId;
    }

    public GuildInviteInviteCancelRequest() { }
}

[MessagePackObject(true)]
public class GuildInviteInviteReplyAllDenyRequest
{
    public IEnumerable<AtgenGuildInviteParamsList> GuildInviteParamsList { get; set; }

    public GuildInviteInviteReplyAllDenyRequest(
        IEnumerable<AtgenGuildInviteParamsList> guildInviteParamsList
    )
    {
        this.GuildInviteParamsList = guildInviteParamsList;
    }

    public GuildInviteInviteReplyAllDenyRequest() { }
}

[MessagePackObject(true)]
public class GuildInviteInviteReplyRequest
{
    public int GuildId { get; set; }
    public ulong GuildInviteId { get; set; }
    public int ReplyStatus { get; set; }

    public GuildInviteInviteReplyRequest(int guildId, ulong guildInviteId, int replyStatus)
    {
        this.GuildId = guildId;
        this.GuildInviteId = guildInviteId;
        this.ReplyStatus = replyStatus;
    }

    public GuildInviteInviteReplyRequest() { }
}

[MessagePackObject(true)]
public class GuildInviteInviteSendRequest
{
    public ulong TargetViewerId { get; set; }
    public int GuildId { get; set; }
    public int GuildInviteMessageId { get; set; }

    public GuildInviteInviteSendRequest(ulong targetViewerId, int guildId, int guildInviteMessageId)
    {
        this.TargetViewerId = targetViewerId;
        this.GuildId = guildId;
        this.GuildInviteMessageId = guildInviteMessageId;
    }

    public GuildInviteInviteSendRequest() { }
}

[MessagePackObject(true)]
public class GuildJoinReplyAllDenyRequest
{
    public int GuildId { get; set; }
    public IEnumerable<ulong> GuildApplyIdList { get; set; }

    public GuildJoinReplyAllDenyRequest(int guildId, IEnumerable<ulong> guildApplyIdList)
    {
        this.GuildId = guildId;
        this.GuildApplyIdList = guildApplyIdList;
    }

    public GuildJoinReplyAllDenyRequest() { }
}

[MessagePackObject(true)]
public class GuildJoinReplyRequest
{
    public int GuildId { get; set; }
    public ulong GuildApplyId { get; set; }
    public int ReplyStatus { get; set; }

    public GuildJoinReplyRequest(int guildId, ulong guildApplyId, int replyStatus)
    {
        this.GuildId = guildId;
        this.GuildApplyId = guildApplyId;
        this.ReplyStatus = replyStatus;
    }

    public GuildJoinReplyRequest() { }
}

[MessagePackObject(true)]
public class GuildJoinRequest
{
    public int GuildId { get; set; }

    public GuildJoinRequest(int guildId)
    {
        this.GuildId = guildId;
    }

    public GuildJoinRequest() { }
}

[MessagePackObject(true)]
public class GuildJoinRequestCancelRequest
{
    public int GuildId { get; set; }

    public GuildJoinRequestCancelRequest(int guildId)
    {
        this.GuildId = guildId;
    }

    public GuildJoinRequestCancelRequest() { }
}

[MessagePackObject(true)]
public class GuildJoinRequestRequest
{
    public int GuildId { get; set; }

    public GuildJoinRequestRequest(int guildId)
    {
        this.GuildId = guildId;
    }

    public GuildJoinRequestRequest() { }
}

[MessagePackObject(true)]
public class GuildPostReportRequest
{
    public int GuildId { get; set; }
    public int ReportType { get; set; }
    public string ReportData { get; set; }
    public int ReportCategoryId { get; set; }
    public string Message { get; set; }

    public GuildPostReportRequest(
        int guildId,
        int reportType,
        string reportData,
        int reportCategoryId,
        string message
    )
    {
        this.GuildId = guildId;
        this.ReportType = reportType;
        this.ReportData = reportData;
        this.ReportCategoryId = reportCategoryId;
        this.Message = message;
    }

    public GuildPostReportRequest() { }
}

[MessagePackObject(true)]
public class GuildResignRequest
{
    public int GuildId { get; set; }
    public int IsTemporaryMember { get; set; }

    public GuildResignRequest(int guildId, int isTemporaryMember)
    {
        this.GuildId = guildId;
        this.IsTemporaryMember = isTemporaryMember;
    }

    public GuildResignRequest() { }
}

[MessagePackObject(true)]
public class GuildSearchAutoSearchRequest
{
    public IEnumerable<int> JoiningConditionTypeList { get; set; }
    public IEnumerable<int> ActivityPolicyTypeList { get; set; }
    public IEnumerable<int> MemberCountTypeList { get; set; }

    public GuildSearchAutoSearchRequest(
        IEnumerable<int> joiningConditionTypeList,
        IEnumerable<int> activityPolicyTypeList,
        IEnumerable<int> memberCountTypeList
    )
    {
        this.JoiningConditionTypeList = joiningConditionTypeList;
        this.ActivityPolicyTypeList = activityPolicyTypeList;
        this.MemberCountTypeList = memberCountTypeList;
    }

    public GuildSearchAutoSearchRequest() { }
}

[MessagePackObject(true)]
public class GuildSearchGetGuildDetailRequest
{
    public int GuildId { get; set; }

    public GuildSearchGetGuildDetailRequest(int guildId)
    {
        this.GuildId = guildId;
    }

    public GuildSearchGetGuildDetailRequest() { }
}

[MessagePackObject(true)]
public class GuildSearchIdSearchRequest
{
    public int GuildId { get; set; }

    public GuildSearchIdSearchRequest(int guildId)
    {
        this.GuildId = guildId;
    }

    public GuildSearchIdSearchRequest() { }
}

[MessagePackObject(true)]
public class GuildSearchNameSearchRequest
{
    public string GuildName { get; set; }

    public GuildSearchNameSearchRequest(string guildName)
    {
        this.GuildName = guildName;
    }

    public GuildSearchNameSearchRequest() { }
}

[MessagePackObject(true)]
public class GuildUpdateGuildPositionTypeRequest
{
    public int GuildId { get; set; }
    public ulong TargetViewerId { get; set; }
    public int GuildPositionType { get; set; }

    public GuildUpdateGuildPositionTypeRequest(
        int guildId,
        ulong targetViewerId,
        int guildPositionType
    )
    {
        this.GuildId = guildId;
        this.TargetViewerId = targetViewerId;
        this.GuildPositionType = guildPositionType;
    }

    public GuildUpdateGuildPositionTypeRequest() { }
}

[MessagePackObject(true)]
public class GuildUpdateGuildSettingRequest
{
    public int GuildId { get; set; }
    public string GuildName { get; set; }
    public int GuildEmblemId { get; set; }
    public string GuildIntroduction { get; set; }
    public string GuildBoard { get; set; }
    public int JoiningConditionType { get; set; }
    public int ActivityPolicyType { get; set; }

    public GuildUpdateGuildSettingRequest(
        int guildId,
        string guildName,
        int guildEmblemId,
        string guildIntroduction,
        string guildBoard,
        int joiningConditionType,
        int activityPolicyType
    )
    {
        this.GuildId = guildId;
        this.GuildName = guildName;
        this.GuildEmblemId = guildEmblemId;
        this.GuildIntroduction = guildIntroduction;
        this.GuildBoard = guildBoard;
        this.JoiningConditionType = joiningConditionType;
        this.ActivityPolicyType = activityPolicyType;
    }

    public GuildUpdateGuildSettingRequest() { }
}

[MessagePackObject(true)]
public class GuildUpdateUserSettingRequest
{
    public int ProfileEntityType { get; set; }
    public int ProfileEntityId { get; set; }
    public int ProfileEntityRarity { get; set; }
    public int GuildPushNotificationTypeRunning { get; set; }
    public int GuildPushNotificationTypeSuspending { get; set; }
    public int IsEnableInviteReceive { get; set; }

    public GuildUpdateUserSettingRequest(
        int profileEntityType,
        int profileEntityId,
        int profileEntityRarity,
        int guildPushNotificationTypeRunning,
        int guildPushNotificationTypeSuspending,
        int isEnableInviteReceive
    )
    {
        this.ProfileEntityType = profileEntityType;
        this.ProfileEntityId = profileEntityId;
        this.ProfileEntityRarity = profileEntityRarity;
        this.GuildPushNotificationTypeRunning = guildPushNotificationTypeRunning;
        this.GuildPushNotificationTypeSuspending = guildPushNotificationTypeSuspending;
        this.IsEnableInviteReceive = isEnableInviteReceive;
    }

    public GuildUpdateUserSettingRequest() { }
}

[MessagePackObject(true)]
public class InquiryAggregationRequest
{
    public string LanguageCode { get; set; }
    public int InquiryFaqId { get; set; }

    public InquiryAggregationRequest(string languageCode, int inquiryFaqId)
    {
        this.LanguageCode = languageCode;
        this.InquiryFaqId = inquiryFaqId;
    }

    public InquiryAggregationRequest() { }
}

[MessagePackObject(true)]
public class InquiryDetailRequest
{
    public string OpinionId { get; set; }
    public string LanguageCode { get; set; }

    public InquiryDetailRequest(string opinionId, string languageCode)
    {
        this.OpinionId = opinionId;
        this.LanguageCode = languageCode;
    }

    public InquiryDetailRequest() { }
}

[MessagePackObject(true)]
public class InquiryReplyRequest
{
    public string OpinionId { get; set; }
    public string CommentText { get; set; }

    public InquiryReplyRequest(string opinionId, string commentText)
    {
        this.OpinionId = opinionId;
        this.CommentText = commentText;
    }

    public InquiryReplyRequest() { }
}

[MessagePackObject(true)]
public class InquirySendRequest
{
    public string OpinionText { get; set; }
    public int OpinionType { get; set; }
    public string LanguageCode { get; set; }
    public string RegionCode { get; set; }
    public int OccurredAt { get; set; }

    public InquirySendRequest(
        string opinionText,
        int opinionType,
        string languageCode,
        string regionCode,
        int occurredAt
    )
    {
        this.OpinionText = opinionText;
        this.OpinionType = opinionType;
        this.LanguageCode = languageCode;
        this.RegionCode = regionCode;
        this.OccurredAt = occurredAt;
    }

    public InquirySendRequest() { }
}

[MessagePackObject(true)]
public class InquiryTopRequest
{
    public string LanguageCode { get; set; }

    public InquiryTopRequest(string languageCode)
    {
        this.LanguageCode = languageCode;
    }

    public InquiryTopRequest() { }
}

[MessagePackObject(true)]
public class ItemGetListRequest { }

[MessagePackObject(true)]
public class ItemUseRecoveryStaminaRequest
{
    public IEnumerable<AtgenUseItemList> UseItemList { get; set; }

    public ItemUseRecoveryStaminaRequest(IEnumerable<AtgenUseItemList> useItemList)
    {
        this.UseItemList = useItemList;
    }

    public ItemUseRecoveryStaminaRequest() { }
}

[MessagePackObject(true)]
public class LoadIndexRequest { }

[MessagePackObject(true)]
public class LoginIndexRequest
{
    public string JwsResult { get; set; }

    public LoginIndexRequest(string jwsResult)
    {
        this.JwsResult = jwsResult;
    }

    public LoginIndexRequest() { }
}

[MessagePackObject(true)]
public class LoginPenaltyConfirmRequest
{
    public int PenaltyType { get; set; }
    public int ReportId { get; set; }

    public LoginPenaltyConfirmRequest(int penaltyType, int reportId)
    {
        this.PenaltyType = penaltyType;
        this.ReportId = reportId;
    }

    public LoginPenaltyConfirmRequest() { }
}

[MessagePackObject(true)]
public class LoginVerifyJwsRequest
{
    public string JwsResult { get; set; }

    public LoginVerifyJwsRequest(string jwsResult)
    {
        this.JwsResult = jwsResult;
    }

    public LoginVerifyJwsRequest() { }
}

[MessagePackObject(true)]
public class LotteryGetOddsDataRequest
{
    public int LotteryId { get; set; }

    public LotteryGetOddsDataRequest(int lotteryId)
    {
        this.LotteryId = lotteryId;
    }

    public LotteryGetOddsDataRequest() { }
}

[MessagePackObject(true)]
public class LotteryLotteryExecRequest
{
    public int LotteryId { get; set; }

    public LotteryLotteryExecRequest(int lotteryId)
    {
        this.LotteryId = lotteryId;
    }

    public LotteryLotteryExecRequest() { }
}

[MessagePackObject(true)]
public class LotteryResultRequest
{
    public int LotteryId { get; set; }

    public LotteryResultRequest(int lotteryId)
    {
        this.LotteryId = lotteryId;
    }

    public LotteryResultRequest() { }
}

[MessagePackObject(true)]
public class MaintenanceGetTextRequest
{
    public int Type { get; set; }
    public string Lang { get; set; }

    public MaintenanceGetTextRequest(int type, string lang)
    {
        this.Type = type;
        this.Lang = lang;
    }

    public MaintenanceGetTextRequest() { }
}

[MessagePackObject(true)]
public class MatchingCheckPenaltyUserRequest
{
    public ulong ViewwerId { get; set; }

    public MatchingCheckPenaltyUserRequest(ulong viewwerId)
    {
        this.ViewwerId = viewwerId;
    }

    public MatchingCheckPenaltyUserRequest() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomListByGuildRequest
{
    public int CompatibleId { get; set; }

    public MatchingGetRoomListByGuildRequest(int compatibleId)
    {
        this.CompatibleId = compatibleId;
    }

    public MatchingGetRoomListByGuildRequest() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomListByLocationRequest
{
    public string Region { get; set; }
    public int QuestType { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public int CompatibleId { get; set; }

    public MatchingGetRoomListByLocationRequest(
        string region,
        int questType,
        float latitude,
        float longitude,
        int compatibleId
    )
    {
        this.Region = region;
        this.QuestType = questType;
        this.Latitude = latitude;
        this.Longitude = longitude;
        this.CompatibleId = compatibleId;
    }

    public MatchingGetRoomListByLocationRequest() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomListByQuestIdRequest
{
    public string Region { get; set; }
    public int QuestId { get; set; }
    public int CompatibleId { get; set; }

    public MatchingGetRoomListByQuestIdRequest(string region, int questId, int compatibleId)
    {
        this.Region = region;
        this.QuestId = questId;
        this.CompatibleId = compatibleId;
    }

    public MatchingGetRoomListByQuestIdRequest() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomListRequest
{
    public string Region { get; set; }
    public int TabType { get; set; }
    public int CompatibleId { get; set; }

    public MatchingGetRoomListRequest(string region, int tabType, int compatibleId)
    {
        this.Region = region;
        this.TabType = tabType;
        this.CompatibleId = compatibleId;
    }

    public MatchingGetRoomListRequest() { }
}

[MessagePackObject(true)]
public class MatchingGetRoomNameRequest
{
    public int RoomId { get; set; }

    public MatchingGetRoomNameRequest(int roomId)
    {
        this.RoomId = roomId;
    }

    public MatchingGetRoomNameRequest() { }
}

[MessagePackObject(true)]
public class MazeEventEntryRequest
{
    public int EventId { get; set; }

    public MazeEventEntryRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public MazeEventEntryRequest() { }
}

[MessagePackObject(true)]
public class MazeEventGetEventDataRequest
{
    public int EventId { get; set; }

    public MazeEventGetEventDataRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public MazeEventGetEventDataRequest() { }
}

[MessagePackObject(true)]
public class MazeEventReceiveMazePointRewardRequest
{
    public int EventId { get; set; }

    public MazeEventReceiveMazePointRewardRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public MazeEventReceiveMazePointRewardRequest() { }
}

[MessagePackObject(true)]
public class MemoryEventActivateRequest
{
    public int EventId { get; set; }

    public MemoryEventActivateRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public MemoryEventActivateRequest() { }
}

[MessagePackObject(true)]
public class MissionGetDrillMissionListRequest { }

[MessagePackObject(true)]
public class MissionGetMissionListRequest { }

[MessagePackObject(true)]
public class MissionReceiveAlbumRewardRequest
{
    public IEnumerable<int> AlbumMissionIdList { get; set; }

    public MissionReceiveAlbumRewardRequest(IEnumerable<int> albumMissionIdList)
    {
        this.AlbumMissionIdList = albumMissionIdList;
    }

    public MissionReceiveAlbumRewardRequest() { }
}

[MessagePackObject(true)]
public class MissionReceiveBeginnerRewardRequest
{
    public IEnumerable<int> BeginnerMissionIdList { get; set; }

    public MissionReceiveBeginnerRewardRequest(IEnumerable<int> beginnerMissionIdList)
    {
        this.BeginnerMissionIdList = beginnerMissionIdList;
    }

    public MissionReceiveBeginnerRewardRequest() { }
}

[MessagePackObject(true)]
public class MissionReceiveDailyRewardRequest
{
    public IEnumerable<AtgenMissionParamsList> MissionParamsList { get; set; }

    public MissionReceiveDailyRewardRequest(IEnumerable<AtgenMissionParamsList> missionParamsList)
    {
        this.MissionParamsList = missionParamsList;
    }

    public MissionReceiveDailyRewardRequest() { }
}

[MessagePackObject(true)]
public class MissionReceiveDrillRewardRequest
{
    public IEnumerable<int> DrillMissionIdList { get; set; }
    public IEnumerable<int> DrillMissionGroupIdList { get; set; }

    public MissionReceiveDrillRewardRequest(
        IEnumerable<int> drillMissionIdList,
        IEnumerable<int> drillMissionGroupIdList
    )
    {
        this.DrillMissionIdList = drillMissionIdList;
        this.DrillMissionGroupIdList = drillMissionGroupIdList;
    }

    public MissionReceiveDrillRewardRequest() { }
}

[MessagePackObject(true)]
public class MissionReceiveMainStoryRewardRequest
{
    public IEnumerable<int> MainStoryMissionIdList { get; set; }

    public MissionReceiveMainStoryRewardRequest(IEnumerable<int> mainStoryMissionIdList)
    {
        this.MainStoryMissionIdList = mainStoryMissionIdList;
    }

    public MissionReceiveMainStoryRewardRequest() { }
}

[MessagePackObject(true)]
public class MissionReceiveMemoryEventRewardRequest
{
    public IEnumerable<int> MemoryEventMissionIdList { get; set; }

    public MissionReceiveMemoryEventRewardRequest(IEnumerable<int> memoryEventMissionIdList)
    {
        this.MemoryEventMissionIdList = memoryEventMissionIdList;
    }

    public MissionReceiveMemoryEventRewardRequest() { }
}

[MessagePackObject(true)]
public class MissionReceiveNormalRewardRequest
{
    public IEnumerable<int> NormalMissionIdList { get; set; }

    public MissionReceiveNormalRewardRequest(IEnumerable<int> normalMissionIdList)
    {
        this.NormalMissionIdList = normalMissionIdList;
    }

    public MissionReceiveNormalRewardRequest() { }
}

[MessagePackObject(true)]
public class MissionReceivePeriodRewardRequest
{
    public IEnumerable<int> PeriodMissionIdList { get; set; }

    public MissionReceivePeriodRewardRequest(IEnumerable<int> periodMissionIdList)
    {
        this.PeriodMissionIdList = periodMissionIdList;
    }

    public MissionReceivePeriodRewardRequest() { }
}

[MessagePackObject(true)]
public class MissionReceiveSpecialRewardRequest
{
    public IEnumerable<int> SpecialMissionIdList { get; set; }

    public MissionReceiveSpecialRewardRequest(IEnumerable<int> specialMissionIdList)
    {
        this.SpecialMissionIdList = specialMissionIdList;
    }

    public MissionReceiveSpecialRewardRequest() { }
}

[MessagePackObject(true)]
public class MissionUnlockDrillMissionGroupRequest
{
    public int DrillMissionGroupId { get; set; }

    public MissionUnlockDrillMissionGroupRequest(int drillMissionGroupId)
    {
        this.DrillMissionGroupId = drillMissionGroupId;
    }

    public MissionUnlockDrillMissionGroupRequest() { }
}

[MessagePackObject(true)]
public class MissionUnlockMainStoryGroupRequest
{
    public int MainStoryMissionGroupId { get; set; }

    public MissionUnlockMainStoryGroupRequest(int mainStoryMissionGroupId)
    {
        this.MainStoryMissionGroupId = mainStoryMissionGroupId;
    }

    public MissionUnlockMainStoryGroupRequest() { }
}

[MessagePackObject(true)]
public class MypageInfoRequest { }

[MessagePackObject(true)]
public class OptionGetOptionRequest { }

[MessagePackObject(true)]
public class OptionSetOptionRequest
{
    public OptionData OptionSetting { get; set; }

    public OptionSetOptionRequest(OptionData optionSetting)
    {
        this.OptionSetting = optionSetting;
    }

    public OptionSetOptionRequest() { }
}

[MessagePackObject(true)]
public class PartyIndexRequest { }

[MessagePackObject(true)]
public class PartySetMainPartyNoRequest
{
    public int MainPartyNo { get; set; }

    public PartySetMainPartyNoRequest(int mainPartyNo)
    {
        this.MainPartyNo = mainPartyNo;
    }

    public PartySetMainPartyNoRequest() { }
}

[MessagePackObject(true)]
public class PartySetPartySettingRequest
{
    public int PartyNo { get; set; }
    public IEnumerable<PartySettingList> RequestPartySettingList { get; set; }

    [MaxLength(20)]
    public string PartyName { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsEntrust { get; set; }
    public UnitElement EntrustElement { get; set; }

    public PartySetPartySettingRequest(
        int partyNo,
        IEnumerable<PartySettingList> requestPartySettingList,
        string partyName,
        bool isEntrust,
        UnitElement entrustElement
    )
    {
        this.PartyNo = partyNo;
        this.RequestPartySettingList = requestPartySettingList;
        this.PartyName = partyName;
        this.IsEntrust = isEntrust;
        this.EntrustElement = entrustElement;
    }

    public PartySetPartySettingRequest() { }
}

[MessagePackObject(true)]
public class PartyUpdatePartyNameRequest
{
    public int PartyNo { get; set; }

    [MaxLength(20)]
    public string PartyName { get; set; }

    public PartyUpdatePartyNameRequest(int partyNo, string partyName)
    {
        this.PartyNo = partyNo;
        this.PartyName = partyName;
    }

    public PartyUpdatePartyNameRequest() { }
}

[MessagePackObject(true)]
public class PlatformAchievementGetPlatformAchievementListRequest { }

/// <summary>
/// Request object for fetching presents
/// Notte's Note:
/// present_history_id is 0 for the first request, but the client will send another request when reaching the end of the previously received list.
/// On subsequent fetch requests present_history_id will contain the id of the first present of the previously fetched list.
/// </summary>
[MessagePackObject(true)]
public class PresentGetHistoryListRequest
{
    public ulong PresentHistoryId { get; set; }

    public PresentGetHistoryListRequest(ulong presentHistoryId)
    {
        this.PresentHistoryId = presentHistoryId;
    }

    public PresentGetHistoryListRequest() { }
}

/// <summary>
/// Request object for fetching presents
/// Notte's Note:
/// present_id is 0 for the first request, but the client will send another request when reaching the end of the previously received list.
/// On subsequent fetch requests present_id will contain the id of the first present of the previously fetched list.
/// </summary>
[MessagePackObject(true)]
public class PresentGetPresentListRequest
{
    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsLimit { get; set; }
    public ulong PresentId { get; set; }

    public PresentGetPresentListRequest(bool isLimit, ulong presentId)
    {
        this.IsLimit = isLimit;
        this.PresentId = presentId;
    }

    public PresentGetPresentListRequest() { }
}

[MessagePackObject(true)]
public class PresentReceiveRequest
{
    public IEnumerable<ulong> PresentIdList { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsLimit { get; set; }

    public PresentReceiveRequest(IEnumerable<ulong> presentIdList, bool isLimit)
    {
        this.PresentIdList = presentIdList;
        this.IsLimit = isLimit;
    }

    public PresentReceiveRequest() { }
}

[MessagePackObject(true)]
public class PushNotificationUpdateSettingRequest
{
    public string Region { get; set; }
    public string Lang { get; set; }
    public string Uuid { get; set; }

    public PushNotificationUpdateSettingRequest(string region, string lang, string uuid)
    {
        this.Region = region;
        this.Lang = lang;
        this.Uuid = uuid;
    }

    public PushNotificationUpdateSettingRequest() { }
}

[MessagePackObject(true)]
public class QuestDropListRequest
{
    public int QuestId { get; set; }

    public QuestDropListRequest(int questId)
    {
        this.QuestId = questId;
    }

    public QuestDropListRequest() { }
}

[MessagePackObject(true)]
public class QuestGetQuestClearPartyMultiRequest
{
    public int QuestId { get; set; }

    public QuestGetQuestClearPartyMultiRequest(int questId)
    {
        this.QuestId = questId;
    }

    public QuestGetQuestClearPartyMultiRequest() { }
}

[MessagePackObject(true)]
public class QuestGetQuestClearPartyRequest
{
    public int QuestId { get; set; }

    public QuestGetQuestClearPartyRequest(int questId)
    {
        this.QuestId = questId;
    }

    public QuestGetQuestClearPartyRequest() { }
}

[MessagePackObject(true)]
public class QuestGetSupportUserListRequest { }

[MessagePackObject(true)]
public class QuestOpenTreasureRequest
{
    public int QuestTreasureId { get; set; }

    public QuestOpenTreasureRequest(int questTreasureId)
    {
        this.QuestTreasureId = questTreasureId;
    }

    public QuestOpenTreasureRequest() { }
}

[MessagePackObject(true)]
public class QuestReadStoryRequest
{
    public int QuestStoryId { get; set; }

    public QuestReadStoryRequest(int questStoryId)
    {
        this.QuestStoryId = questStoryId;
    }

    public QuestReadStoryRequest() { }
}

[MessagePackObject(true)]
public class QuestSearchQuestClearPartyCharaMultiRequest
{
    public IEnumerable<int> QuestIdList { get; set; }

    public QuestSearchQuestClearPartyCharaMultiRequest(IEnumerable<int> questIdList)
    {
        this.QuestIdList = questIdList;
    }

    public QuestSearchQuestClearPartyCharaMultiRequest() { }
}

[MessagePackObject(true)]
public class QuestSearchQuestClearPartyCharaRequest
{
    public IEnumerable<int> QuestIdList { get; set; }

    public QuestSearchQuestClearPartyCharaRequest(IEnumerable<int> questIdList)
    {
        this.QuestIdList = questIdList;
    }

    public QuestSearchQuestClearPartyCharaRequest() { }
}

[MessagePackObject(true)]
public class QuestSearchQuestClearPartyMultiRequest
{
    public int QuestId { get; set; }
    public int PartySwitchNo { get; set; }
    public IEnumerable<int> CharaIdList { get; set; }
    public IEnumerable<int> DragonIdList { get; set; }
    public IEnumerable<int> WeaponBodyIdList { get; set; }
    public IEnumerable<int> AbilityCrestIdList { get; set; }

    public QuestSearchQuestClearPartyMultiRequest(
        int questId,
        int partySwitchNo,
        IEnumerable<int> charaIdList,
        IEnumerable<int> dragonIdList,
        IEnumerable<int> weaponBodyIdList,
        IEnumerable<int> abilityCrestIdList
    )
    {
        this.QuestId = questId;
        this.PartySwitchNo = partySwitchNo;
        this.CharaIdList = charaIdList;
        this.DragonIdList = dragonIdList;
        this.WeaponBodyIdList = weaponBodyIdList;
        this.AbilityCrestIdList = abilityCrestIdList;
    }

    public QuestSearchQuestClearPartyMultiRequest() { }
}

[MessagePackObject(true)]
public class QuestSearchQuestClearPartyRequest
{
    public int QuestId { get; set; }
    public int PartySwitchNo { get; set; }
    public IEnumerable<int> CharaIdList { get; set; }
    public IEnumerable<int> DragonIdList { get; set; }
    public IEnumerable<int> WeaponBodyIdList { get; set; }
    public IEnumerable<int> AbilityCrestIdList { get; set; }

    public QuestSearchQuestClearPartyRequest(
        int questId,
        int partySwitchNo,
        IEnumerable<int> charaIdList,
        IEnumerable<int> dragonIdList,
        IEnumerable<int> weaponBodyIdList,
        IEnumerable<int> abilityCrestIdList
    )
    {
        this.QuestId = questId;
        this.PartySwitchNo = partySwitchNo;
        this.CharaIdList = charaIdList;
        this.DragonIdList = dragonIdList;
        this.WeaponBodyIdList = weaponBodyIdList;
        this.AbilityCrestIdList = abilityCrestIdList;
    }

    public QuestSearchQuestClearPartyRequest() { }
}

[MessagePackObject(true)]
public class QuestSetQuestClearPartyMultiRequest
{
    public int QuestId { get; set; }
    public IEnumerable<PartySettingList> RequestPartySettingList { get; set; }

    public QuestSetQuestClearPartyMultiRequest(
        int questId,
        IEnumerable<PartySettingList> requestPartySettingList
    )
    {
        this.QuestId = questId;
        this.RequestPartySettingList = requestPartySettingList;
    }

    public QuestSetQuestClearPartyMultiRequest() { }
}

[MessagePackObject(true)]
public class QuestSetQuestClearPartyRequest
{
    public int QuestId { get; set; }
    public IEnumerable<PartySettingList> RequestPartySettingList { get; set; }

    public QuestSetQuestClearPartyRequest(
        int questId,
        IEnumerable<PartySettingList> requestPartySettingList
    )
    {
        this.QuestId = questId;
        this.RequestPartySettingList = requestPartySettingList;
    }

    public QuestSetQuestClearPartyRequest() { }
}

[MessagePackObject(true)]
public class RaidEventEntryRequest
{
    public int RaidEventId { get; set; }

    public RaidEventEntryRequest(int raidEventId)
    {
        this.RaidEventId = raidEventId;
    }

    public RaidEventEntryRequest() { }
}

[MessagePackObject(true)]
public class RaidEventGetEventDataRequest
{
    public int RaidEventId { get; set; }

    public RaidEventGetEventDataRequest(int raidEventId)
    {
        this.RaidEventId = raidEventId;
    }

    public RaidEventGetEventDataRequest() { }
}

[MessagePackObject(true)]
public class RaidEventReceiveRaidPointRewardRequest
{
    public int RaidEventId { get; set; }
    public IEnumerable<int> RaidEventRewardIdList { get; set; }

    public RaidEventReceiveRaidPointRewardRequest(
        int raidEventId,
        IEnumerable<int> raidEventRewardIdList
    )
    {
        this.RaidEventId = raidEventId;
        this.RaidEventRewardIdList = raidEventRewardIdList;
    }

    public RaidEventReceiveRaidPointRewardRequest() { }
}

[MessagePackObject(true)]
public class RedoableSummonFixExecRequest { }

[MessagePackObject(true)]
public class RedoableSummonGetDataRequest { }

[MessagePackObject(true)]
public class RedoableSummonPreExecRequest
{
    public int SummonId { get; set; }

    public RedoableSummonPreExecRequest(int summonId)
    {
        this.SummonId = summonId;
    }

    public RedoableSummonPreExecRequest() { }
}

[MessagePackObject(true)]
public class RepeatEndRequest { }

[MessagePackObject(true)]
public class ShopChargeCancelRequest
{
    public int ShopType { get; set; }
    public int GoodsId { get; set; }

    public ShopChargeCancelRequest(int shopType, int goodsId)
    {
        this.ShopType = shopType;
        this.GoodsId = goodsId;
    }

    public ShopChargeCancelRequest() { }
}

[MessagePackObject(true)]
public class ShopGetBonusRequest
{
    public int BonusType { get; set; }

    public ShopGetBonusRequest(int bonusType)
    {
        this.BonusType = bonusType;
    }

    public ShopGetBonusRequest() { }
}

[MessagePackObject(true)]
public class ShopGetDreamSelectUnitListRequest
{
    public int GoodsId { get; set; }

    public ShopGetDreamSelectUnitListRequest(int goodsId)
    {
        this.GoodsId = goodsId;
    }

    public ShopGetDreamSelectUnitListRequest() { }
}

[MessagePackObject(true)]
public class ShopGetListRequest { }

[MessagePackObject(true)]
public class ShopGetProductListRequest { }

[MessagePackObject(true)]
public class ShopItemSummonExecRequest
{
    public PaymentTypes PaymentType { get; set; }
    public PaymentTarget PaymentTarget { get; set; }

    public ShopItemSummonExecRequest(PaymentTypes paymentType, PaymentTarget paymentTarget)
    {
        this.PaymentType = paymentType;
        this.PaymentTarget = paymentTarget;
    }

    public ShopItemSummonExecRequest() { }
}

[MessagePackObject(true)]
public class ShopItemSummonOddRequest { }

[MessagePackObject(true)]
public class ShopMaterialShopPurchaseRequest
{
    public int GoodsId { get; set; }
    public MaterialShopType ShopType { get; set; }
    public PaymentTypes PaymentType { get; set; }
    public int Quantity { get; set; }

    public ShopMaterialShopPurchaseRequest(
        int goodsId,
        MaterialShopType shopType,
        PaymentTypes paymentType,
        int quantity
    )
    {
        this.GoodsId = goodsId;
        this.ShopType = shopType;
        this.PaymentType = paymentType;
        this.Quantity = quantity;
    }

    public ShopMaterialShopPurchaseRequest() { }
}

[MessagePackObject(true)]
public class ShopNormalShopPurchaseRequest
{
    public int GoodsId { get; set; }
    public PaymentTypes PaymentType { get; set; }
    public int Quantity { get; set; }

    public ShopNormalShopPurchaseRequest(int goodsId, PaymentTypes paymentType, int quantity)
    {
        this.GoodsId = goodsId;
        this.PaymentType = paymentType;
        this.Quantity = quantity;
    }

    public ShopNormalShopPurchaseRequest() { }
}

[MessagePackObject(true)]
public class ShopPreChargeCheckRequest
{
    public int ShopType { get; set; }
    public int GoodsId { get; set; }
    public int Quantity { get; set; }

    public ShopPreChargeCheckRequest(int shopType, int goodsId, int quantity)
    {
        this.ShopType = shopType;
        this.GoodsId = goodsId;
        this.Quantity = quantity;
    }

    public ShopPreChargeCheckRequest() { }
}

[MessagePackObject(true)]
public class ShopSpecialShopPurchaseRequest
{
    public int GoodsId { get; set; }
    public PaymentTypes PaymentType { get; set; }
    public int Quantity { get; set; }
    public AtgenDuplicateEntityList SelectedUnit { get; set; }

    public ShopSpecialShopPurchaseRequest(
        int goodsId,
        PaymentTypes paymentType,
        int quantity,
        AtgenDuplicateEntityList selectedUnit
    )
    {
        this.GoodsId = goodsId;
        this.PaymentType = paymentType;
        this.Quantity = quantity;
        this.SelectedUnit = selectedUnit;
    }

    public ShopSpecialShopPurchaseRequest() { }
}

[MessagePackObject(true)]
public class SimpleEventEntryRequest
{
    public int EventId { get; set; }

    public SimpleEventEntryRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public SimpleEventEntryRequest() { }
}

[MessagePackObject(true)]
public class SimpleEventGetEventDataRequest
{
    public int EventId { get; set; }

    public SimpleEventGetEventDataRequest(int eventId)
    {
        this.EventId = eventId;
    }

    public SimpleEventGetEventDataRequest() { }
}

[MessagePackObject(true)]
public class StampGetStampRequest { }

[MessagePackObject(true)]
public class StampSetEquipStampRequest
{
    public int DeckNo { get; set; }
    public IEnumerable<EquipStampList> StampList { get; set; }

    public StampSetEquipStampRequest(int deckNo, IEnumerable<EquipStampList> stampList)
    {
        this.DeckNo = deckNo;
        this.StampList = stampList;
    }

    public StampSetEquipStampRequest() { }
}

[MessagePackObject(true)]
public class StoryReadRequest
{
    public int UnitStoryId { get; set; }

    public StoryReadRequest(int unitStoryId)
    {
        this.UnitStoryId = unitStoryId;
    }

    public StoryReadRequest() { }
}

[MessagePackObject(true)]
public class StorySkipSkipRequest { }

[MessagePackObject(true)]
public class SuggestionGetCategoryListRequest
{
    public string LanguageCode { get; set; }

    public SuggestionGetCategoryListRequest(string languageCode)
    {
        this.LanguageCode = languageCode;
    }

    public SuggestionGetCategoryListRequest() { }
}

[MessagePackObject(true)]
public class SuggestionSetRequest
{
    public string Message { get; set; }
    public int CategoryId { get; set; }

    public SuggestionSetRequest(string message, int categoryId)
    {
        this.Message = message;
        this.CategoryId = categoryId;
    }

    public SuggestionSetRequest() { }
}

[MessagePackObject(true)]
public class SummonExcludeGetListRequest
{
    public int SummonId { get; set; }

    public SummonExcludeGetListRequest(int summonId)
    {
        this.SummonId = summonId;
    }

    public SummonExcludeGetListRequest() { }
}

[MessagePackObject(true)]
public class SummonExcludeGetOddsDataRequest
{
    public int SummonId { get; set; }
    public int ExcludeEntityType { get; set; }
    public IEnumerable<int> ExcludeEntityIdList { get; set; }

    public SummonExcludeGetOddsDataRequest(
        int summonId,
        int excludeEntityType,
        IEnumerable<int> excludeEntityIdList
    )
    {
        this.SummonId = summonId;
        this.ExcludeEntityType = excludeEntityType;
        this.ExcludeEntityIdList = excludeEntityIdList;
    }

    public SummonExcludeGetOddsDataRequest() { }
}

[MessagePackObject(true)]
public class SummonExcludeRequestRequest
{
    public int SummonId { get; set; }
    public PaymentTypes PaymentType { get; set; }
    public int ExcludeEntityType { get; set; }
    public IEnumerable<int> ExcludeEntityIdList { get; set; }

    public SummonExcludeRequestRequest(
        int summonId,
        PaymentTypes paymentType,
        int excludeEntityType,
        IEnumerable<int> excludeEntityIdList
    )
    {
        this.SummonId = summonId;
        this.PaymentType = paymentType;
        this.ExcludeEntityType = excludeEntityType;
        this.ExcludeEntityIdList = excludeEntityIdList;
    }

    public SummonExcludeRequestRequest() { }
}

[MessagePackObject(true)]
public class SummonGetOddsDataRequest
{
    public int SummonId { get; set; }

    public SummonGetOddsDataRequest(int summonId)
    {
        this.SummonId = summonId;
    }

    public SummonGetOddsDataRequest() { }
}

[MessagePackObject(true)]
public class SummonGetSummonHistoryRequest { }

[MessagePackObject(true)]
public class SummonGetSummonListRequest { }

[MessagePackObject(true)]
public class SummonGetSummonPointTradeRequest
{
    public int SummonId { get; set; }

    public SummonGetSummonPointTradeRequest(int summonId)
    {
        this.SummonId = summonId;
    }

    public SummonGetSummonPointTradeRequest() { }
}

//TODO: HasUnknown
/// <summary>
/// A summoning request
/// </summary>
/// <param name="summon_id">Id of the summon banner</param>
/// <param name="exec_type">Distinguishing single(1) from tenfold(2) summons, maybe more</param>
/// <param name="exec_count">Seemingly only passed for multiple single summons, 0 for tenfold</param>
/// <param name="payment_type">Type of currency used</param>
/// <param name="payment_target"><b>See: <see cref="Generated.PaymentTarget"/></b></param>
[MessagePackObject(true)]
public class SummonRequestRequest
{
    public int SummonId { get; set; }
    public SummonExecTypes ExecType { get; set; }
    public int ExecCount { get; set; }
    public PaymentTypes PaymentType { get; set; }
    public PaymentTarget PaymentTarget { get; set; }

    public SummonRequestRequest(
        int summonId,
        SummonExecTypes execType,
        int execCount,
        PaymentTypes paymentType,
        PaymentTarget paymentTarget
    )
    {
        this.SummonId = summonId;
        this.ExecType = execType;
        this.ExecCount = execCount;
        this.PaymentType = paymentType;
        this.PaymentTarget = paymentTarget;
    }

    public SummonRequestRequest() { }
}

[MessagePackObject(true)]
public class SummonSummonPointTradeRequest
{
    public int SummonId { get; set; }
    public int TradeId { get; set; }

    public SummonSummonPointTradeRequest(int summonId, int tradeId)
    {
        this.SummonId = summonId;
        this.TradeId = tradeId;
    }

    public SummonSummonPointTradeRequest() { }
}

[MessagePackObject(true)]
public class TalismanSellRequest
{
    public IEnumerable<long> TalismanKeyIdList { get; set; }

    public TalismanSellRequest(IEnumerable<long> talismanKeyIdList)
    {
        this.TalismanKeyIdList = talismanKeyIdList;
    }

    public TalismanSellRequest() { }
}

[MessagePackObject(true)]
public class TalismanSetLockRequest
{
    public long TalismanKeyId { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsLock { get; set; }

    public TalismanSetLockRequest(long talismanKeyId, bool isLock)
    {
        this.TalismanKeyId = talismanKeyId;
        this.IsLock = isLock;
    }

    public TalismanSetLockRequest() { }
}

[MessagePackObject(true)]
public class TimeAttackRankingGetDataRequest { }

[MessagePackObject(true)]
public class TimeAttackRankingReceiveTierRewardRequest
{
    public int QuestId { get; set; }

    public TimeAttackRankingReceiveTierRewardRequest(int questId)
    {
        this.QuestId = questId;
    }

    public TimeAttackRankingReceiveTierRewardRequest() { }
}

[MessagePackObject(true)]
public class ToolAuthRequest
{
    public string Uuid { get; set; }
    public string IdToken { get; set; }

    public ToolAuthRequest(string uuid, string idToken)
    {
        this.Uuid = uuid;
        this.IdToken = idToken;
    }

    public ToolAuthRequest() { }
}

[MessagePackObject(true)]
public class ToolGetMaintenanceTimeRequest { }

[MessagePackObject(true)]
public class ToolGetServiceStatusRequest { }

[MessagePackObject(true)]
public class ToolReauthRequest
{
    public string Uuid { get; set; }
    public string IdToken { get; set; }

    public ToolReauthRequest(string uuid, string idToken)
    {
        this.Uuid = uuid;
        this.IdToken = idToken;
    }

    public ToolReauthRequest() { }
}

[MessagePackObject(true)]
public class ToolSignupRequest
{
    public string Uuid { get; set; }
    public string IdToken { get; set; }
    public string AppVersion { get; set; }
    public string Platform { get; set; }
    public string Hashcode { get; set; }
    public int ResetCount { get; set; }
    public string EulaRegion { get; set; }
    public string EulaLang { get; set; }
    public int EulaVersion { get; set; }
    public int PrivacyPolicyVersion { get; set; }

    public ToolSignupRequest(
        string uuid,
        string idToken,
        string appVersion,
        string platform,
        string hashcode,
        int resetCount,
        string eulaRegion,
        string eulaLang,
        int eulaVersion,
        int privacyPolicyVersion
    )
    {
        this.Uuid = uuid;
        this.IdToken = idToken;
        this.AppVersion = appVersion;
        this.Platform = platform;
        this.Hashcode = hashcode;
        this.ResetCount = resetCount;
        this.EulaRegion = eulaRegion;
        this.EulaLang = eulaLang;
        this.EulaVersion = eulaVersion;
        this.PrivacyPolicyVersion = privacyPolicyVersion;
    }

    public ToolSignupRequest() { }
}

[MessagePackObject(true)]
public class TrackRecordUpdateProgressRequest { }

[MessagePackObject(true)]
public class TransitionTransitionByNAccountRequest
{
    public string Uuid { get; set; }
    public string IdToken { get; set; }

    public TransitionTransitionByNAccountRequest(string uuid, string idToken)
    {
        this.Uuid = uuid;
        this.IdToken = idToken;
    }

    public TransitionTransitionByNAccountRequest() { }
}

[MessagePackObject(true)]
public class TreasureTradeGetListAllRequest { }

[MessagePackObject(true)]
public class TreasureTradeGetListRequest
{
    public int TradeGroupId { get; set; }

    public TreasureTradeGetListRequest(int tradeGroupId)
    {
        this.TradeGroupId = tradeGroupId;
    }

    public TreasureTradeGetListRequest() { }
}

[MessagePackObject(true)]
public class TreasureTradeTradeRequest
{
    public int TradeGroupId { get; set; }
    public int TreasureTradeId { get; set; }
    public IEnumerable<AtgenNeedUnitList> NeedUnitList { get; set; }
    public int TradeCount { get; set; }

    public TreasureTradeTradeRequest(
        int tradeGroupId,
        int treasureTradeId,
        IEnumerable<AtgenNeedUnitList> needUnitList,
        int tradeCount
    )
    {
        this.TradeGroupId = tradeGroupId;
        this.TreasureTradeId = treasureTradeId;
        this.NeedUnitList = needUnitList;
        this.TradeCount = tradeCount;
    }

    public TreasureTradeTradeRequest() { }
}

[MessagePackObject(true)]
public class TutorialUpdateFlagsRequest
{
    public int FlagId { get; set; }

    public TutorialUpdateFlagsRequest(int flagId)
    {
        this.FlagId = flagId;
    }

    public TutorialUpdateFlagsRequest() { }
}

[MessagePackObject(true)]
public class TutorialUpdateStepRequest
{
    public int Step { get; set; }
    public int IsSkip { get; set; }
    public int EntityId { get; set; }
    public EntityTypes EntityType { get; set; }

    public TutorialUpdateStepRequest(int step, int isSkip, int entityId, EntityTypes entityType)
    {
        this.Step = step;
        this.IsSkip = isSkip;
        this.EntityId = entityId;
        this.EntityType = entityType;
    }

    public TutorialUpdateStepRequest() { }
}

[MessagePackObject(true)]
public class UpdateNamechangeRequest
{
    [MaxLength(10)]
    public string Name { get; set; }

    public UpdateNamechangeRequest(string name)
    {
        this.Name = name;
    }

    public UpdateNamechangeRequest() { }
}

[MessagePackObject(true)]
public class UpdateResetNewRequest
{
    public IEnumerable<AtgenTargetList> TargetList { get; set; }

    public UpdateResetNewRequest(IEnumerable<AtgenTargetList> targetList)
    {
        this.TargetList = targetList;
    }

    public UpdateResetNewRequest() { }
}

[MessagePackObject(true)]
public class UserGetNAccountInfoRequest { }

[MessagePackObject(true)]
public class UserGetWalletBalanceRequest { }

[MessagePackObject(true)]
public class UserLinkedNAccountRequest { }

[MessagePackObject(true)]
public class UserOptInSettingRequest
{
    public int IsOptin { get; set; }

    public UserOptInSettingRequest(int isOptin)
    {
        this.IsOptin = isOptin;
    }

    public UserOptInSettingRequest() { }
}

[MessagePackObject(true)]
public class UserRecoverStaminaByStoneRequest
{
    public int RecoveryType { get; set; }
    public int RecoveryQuantity { get; set; }
    public PaymentTypes PaymentType { get; set; }

    public UserRecoverStaminaByStoneRequest(
        int recoveryType,
        int recoveryQuantity,
        PaymentTypes paymentType
    )
    {
        this.RecoveryType = recoveryType;
        this.RecoveryQuantity = recoveryQuantity;
        this.PaymentType = paymentType;
    }

    public UserRecoverStaminaByStoneRequest() { }
}

[MessagePackObject(true)]
public class UserWithdrawalRequest { }

[MessagePackObject(true)]
public class VersionGetResourceVersionRequest
{
    public Platform Platform { get; set; }
    public string AppVersion { get; set; }

    public VersionGetResourceVersionRequest(Platform platform, string appVersion)
    {
        this.Platform = platform;
        this.AppVersion = appVersion;
    }

    public VersionGetResourceVersionRequest() { }
}

[MessagePackObject(true)]
public class WalkerSendGiftMultipleRequest
{
    public int DragonGiftId { get; set; }
    public int Quantity { get; set; }

    public WalkerSendGiftMultipleRequest(int dragonGiftId, int quantity)
    {
        this.DragonGiftId = dragonGiftId;
        this.Quantity = quantity;
    }

    public WalkerSendGiftMultipleRequest() { }
}

[MessagePackObject(true)]
public class WallFailRequest
{
    public string DungeonKey { get; set; }
    public int FailState { get; set; }

    public WallFailRequest(string dungeonKey, int failState)
    {
        this.DungeonKey = dungeonKey;
        this.FailState = failState;
    }

    public WallFailRequest() { }
}

[MessagePackObject(true)]
public class WallGetMonthlyRewardRequest
{
    public int QuestGroupId { get; set; }

    public WallGetMonthlyRewardRequest(int questGroupId)
    {
        this.QuestGroupId = questGroupId;
    }

    public WallGetMonthlyRewardRequest() { }
}

[MessagePackObject(true)]
public class WallGetWallClearPartyRequest
{
    public int WallId { get; set; }

    public WallGetWallClearPartyRequest(int wallId)
    {
        this.WallId = wallId;
    }

    public WallGetWallClearPartyRequest() { }
}

[MessagePackObject(true)]
public class WallReceiveMonthlyRewardRequest
{
    public int QuestGroupId { get; set; }

    public WallReceiveMonthlyRewardRequest(int questGroupId)
    {
        this.QuestGroupId = questGroupId;
    }

    public WallReceiveMonthlyRewardRequest() { }
}

[MessagePackObject(true)]
public class WallRecordRecordRequest
{
    public int WallId { get; set; }
    public string DungeonKey { get; set; }

    public WallRecordRecordRequest(int wallId, string dungeonKey)
    {
        this.WallId = wallId;
        this.DungeonKey = dungeonKey;
    }

    public WallRecordRecordRequest() { }
}

[MessagePackObject(true)]
public class WallSetWallClearPartyRequest
{
    public int WallId { get; set; }
    public IEnumerable<PartySettingList> RequestPartySettingList { get; set; }

    public WallSetWallClearPartyRequest(
        int wallId,
        IEnumerable<PartySettingList> requestPartySettingList
    )
    {
        this.WallId = wallId;
        this.RequestPartySettingList = requestPartySettingList;
    }

    public WallSetWallClearPartyRequest() { }
}

[MessagePackObject(true)]
public class WallStartStartAssignUnitRequest
{
    public int WallId { get; set; }
    public int WallLevel { get; set; }
    public IList<PartySettingList> RequestPartySettingList { get; set; }
    public ulong SupportViewerId { get; set; }

    public WallStartStartAssignUnitRequest(
        int wallId,
        int wallLevel,
        IList<PartySettingList> requestPartySettingList,
        ulong supportViewerId
    )
    {
        this.WallId = wallId;
        this.WallLevel = wallLevel;
        this.RequestPartySettingList = requestPartySettingList;
        this.SupportViewerId = supportViewerId;
    }

    public WallStartStartAssignUnitRequest() { }
}

[MessagePackObject(true)]
public class WallStartStartRequest
{
    public int WallId { get; set; }
    public int WallLevel { get; set; }
    public int PartyNo { get; set; }
    public ulong SupportViewerId { get; set; }

    public WallStartStartRequest(int wallId, int wallLevel, int partyNo, ulong supportViewerId)
    {
        this.WallId = wallId;
        this.WallLevel = wallLevel;
        this.PartyNo = partyNo;
        this.SupportViewerId = supportViewerId;
    }

    public WallStartStartRequest() { }
}

[MessagePackObject(true)]
public class WeaponBodyBuildupPieceRequest
{
    public WeaponBodies WeaponBodyId { get; set; }
    public IEnumerable<AtgenBuildupWeaponBodyPieceList> BuildupWeaponBodyPieceList { get; set; }

    public WeaponBodyBuildupPieceRequest(
        WeaponBodies weaponBodyId,
        IEnumerable<AtgenBuildupWeaponBodyPieceList> buildupWeaponBodyPieceList
    )
    {
        this.WeaponBodyId = weaponBodyId;
        this.BuildupWeaponBodyPieceList = buildupWeaponBodyPieceList;
    }

    public WeaponBodyBuildupPieceRequest() { }
}

[MessagePackObject(true)]
public class WeaponBodyCraftRequest
{
    public WeaponBodies WeaponBodyId { get; set; }

    public WeaponBodyCraftRequest(WeaponBodies weaponBodyId)
    {
        this.WeaponBodyId = weaponBodyId;
    }

    public WeaponBodyCraftRequest() { }
}

[MessagePackObject(true)]
public class WeaponBuildupRequest
{
    public ulong BaseWeaponKeyId { get; set; }
    public IEnumerable<GrowMaterialList> GrowMaterialList { get; set; }

    public WeaponBuildupRequest(
        ulong baseWeaponKeyId,
        IEnumerable<GrowMaterialList> growMaterialList
    )
    {
        this.BaseWeaponKeyId = baseWeaponKeyId;
        this.GrowMaterialList = growMaterialList;
    }

    public WeaponBuildupRequest() { }
}

[MessagePackObject(true)]
public class WeaponLimitBreakRequest
{
    public ulong BaseWeaponKeyId { get; set; }
    public IEnumerable<GrowMaterialList> GrowMaterialList { get; set; }

    public WeaponLimitBreakRequest(
        ulong baseWeaponKeyId,
        IEnumerable<GrowMaterialList> growMaterialList
    )
    {
        this.BaseWeaponKeyId = baseWeaponKeyId;
        this.GrowMaterialList = growMaterialList;
    }

    public WeaponLimitBreakRequest() { }
}

[MessagePackObject(true)]
public class WeaponResetPlusCountRequest
{
    public ulong WeaponKeyId { get; set; }
    public int PlusCountType { get; set; }

    public WeaponResetPlusCountRequest(ulong weaponKeyId, int plusCountType)
    {
        this.WeaponKeyId = weaponKeyId;
        this.PlusCountType = plusCountType;
    }

    public WeaponResetPlusCountRequest() { }
}

[MessagePackObject(true)]
public class WeaponSellRequest
{
    public IEnumerable<ulong> WeaponKeyIdList { get; set; }

    public WeaponSellRequest(IEnumerable<ulong> weaponKeyIdList)
    {
        this.WeaponKeyIdList = weaponKeyIdList;
    }

    public WeaponSellRequest() { }
}

[MessagePackObject(true)]
public class WeaponSetLockRequest
{
    public ulong WeaponKeyId { get; set; }
    public int IsLock { get; set; }

    public WeaponSetLockRequest(ulong weaponKeyId, int isLock)
    {
        this.WeaponKeyId = weaponKeyId;
        this.IsLock = isLock;
    }

    public WeaponSetLockRequest() { }
}

[MessagePackObject(true)]
public class WebviewVersionUrlListRequest
{
    public string Region { get; set; }

    public WebviewVersionUrlListRequest(string region)
    {
        this.Region = region;
    }

    public WebviewVersionUrlListRequest() { }
}
