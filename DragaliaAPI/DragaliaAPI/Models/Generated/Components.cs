using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Present;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.Dungeon;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.Json;
using MessagePack;
using KeyAttribute = MessagePack.KeyAttribute;

namespace DragaliaAPI.Models.Generated;

[MessagePackObject(true)]
public class AbilityCrestList
{
    public AbilityCrests AbilityCrestId { get; set; }
    public int BuildupCount { get; set; }
    public int LimitBreakCount { get; set; }
    public int EquipableCount { get; set; }
    public int HpPlusCount { get; set; }
    public int AttackPlusCount { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool IsFavorite { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool IsNew { get; set; }
    public DateTimeOffset Gettime { get; set; }
    public int Ability1Level { get; set; }
    public int Ability2Level { get; set; }

    public AbilityCrestList(
        AbilityCrests abilityCrestId,
        int buildupCount,
        int limitBreakCount,
        int equipableCount,
        int hpPlusCount,
        int attackPlusCount,
        bool isFavorite,
        bool isNew,
        DateTimeOffset gettime,
        int ability1Level,
        int ability2Level
    )
    {
        this.AbilityCrestId = abilityCrestId;
        this.BuildupCount = buildupCount;
        this.LimitBreakCount = limitBreakCount;
        this.EquipableCount = equipableCount;
        this.HpPlusCount = hpPlusCount;
        this.AttackPlusCount = attackPlusCount;
        this.IsFavorite = isFavorite;
        this.IsNew = isNew;
        this.Gettime = gettime;
        this.Ability1Level = ability1Level;
        this.Ability2Level = ability2Level;
    }

    public AbilityCrestList() { }
}

[MessagePackObject(true)]
public class AbilityCrestSetList
{
    public int AbilityCrestSetNo { get; set; }
    public string AbilityCrestSetName { get; set; }
    public AbilityCrests CrestSlotType1CrestId1 { get; set; }
    public AbilityCrests CrestSlotType1CrestId2 { get; set; }
    public AbilityCrests CrestSlotType1CrestId3 { get; set; }
    public AbilityCrests CrestSlotType2CrestId1 { get; set; }
    public AbilityCrests CrestSlotType2CrestId2 { get; set; }
    public AbilityCrests CrestSlotType3CrestId1 { get; set; }
    public AbilityCrests CrestSlotType3CrestId2 { get; set; }
    public ulong TalismanKeyId { get; set; }

    public AbilityCrestSetList(
        int abilityCrestSetNo,
        string abilityCrestSetName,
        AbilityCrests crestSlotType1CrestId1,
        AbilityCrests crestSlotType1CrestId2,
        AbilityCrests crestSlotType1CrestId3,
        AbilityCrests crestSlotType2CrestId1,
        AbilityCrests crestSlotType2CrestId2,
        AbilityCrests crestSlotType3CrestId1,
        AbilityCrests crestSlotType3CrestId2,
        ulong talismanKeyId
    )
    {
        this.AbilityCrestSetNo = abilityCrestSetNo;
        this.AbilityCrestSetName = abilityCrestSetName;
        this.CrestSlotType1CrestId1 = crestSlotType1CrestId1;
        this.CrestSlotType1CrestId2 = crestSlotType1CrestId2;
        this.CrestSlotType1CrestId3 = crestSlotType1CrestId3;
        this.CrestSlotType2CrestId1 = crestSlotType2CrestId1;
        this.CrestSlotType2CrestId2 = crestSlotType2CrestId2;
        this.CrestSlotType3CrestId1 = crestSlotType3CrestId1;
        this.CrestSlotType3CrestId2 = crestSlotType3CrestId2;
        this.TalismanKeyId = talismanKeyId;
    }

    public AbilityCrestSetList() { }
}

[MessagePackObject(true)]
public class AbilityCrestTradeList
{
    public int AbilityCrestTradeId { get; set; }
    public AbilityCrests AbilityCrestId { get; set; }
    public int NeedDewPoint { get; set; }
    public int Priority { get; set; }
    public DateTimeOffset CompleteDate { get; set; }
    public DateTimeOffset PickupViewStartDate { get; set; }
    public DateTimeOffset PickupViewEndDate { get; set; }

    public AbilityCrestTradeList(
        int abilityCrestTradeId,
        AbilityCrests abilityCrestId,
        int needDewPoint,
        int priority,
        DateTimeOffset completeDate,
        DateTimeOffset pickupViewStartDate,
        DateTimeOffset pickupViewEndDate
    )
    {
        this.AbilityCrestTradeId = abilityCrestTradeId;
        this.AbilityCrestId = abilityCrestId;
        this.NeedDewPoint = needDewPoint;
        this.Priority = priority;
        this.CompleteDate = completeDate;
        this.PickupViewStartDate = pickupViewStartDate;
        this.PickupViewEndDate = pickupViewEndDate;
    }

    public AbilityCrestTradeList() { }
}

[MessagePackObject(true)]
public class AchievementList
{
    public int AchievementId { get; set; }
    public int Progress { get; set; }
    public int State { get; set; }

    public AchievementList(int achievementId, int progress, int state)
    {
        this.AchievementId = achievementId;
        this.Progress = progress;
        this.State = state;
    }

    public AchievementList() { }
}

[MessagePackObject(true)]
public class AlbumDragonData
{
    public int DragonId { get; set; }
    public int MaxLevel { get; set; }
    public int MaxLimitBreakCount { get; set; }

    public AlbumDragonData(int dragonId, int maxLevel, int maxLimitBreakCount)
    {
        this.DragonId = dragonId;
        this.MaxLevel = maxLevel;
        this.MaxLimitBreakCount = maxLimitBreakCount;
    }

    public AlbumDragonData() { }
}

[MessagePackObject(true)]
public class AlbumMissionList
{
    public int AlbumMissionId { get; set; }
    public int Progress { get; set; }
    public int State { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset StartDate { get; set; }

    public AlbumMissionList(
        int albumMissionId,
        int progress,
        int state,
        DateTimeOffset endDate,
        DateTimeOffset startDate
    )
    {
        this.AlbumMissionId = albumMissionId;
        this.Progress = progress;
        this.State = state;
        this.EndDate = endDate;
        this.StartDate = startDate;
    }

    public AlbumMissionList() { }
}

[MessagePackObject(true)]
public class AlbumPassiveNotice
{
    public int IsUpdateChara { get; set; }
    public int IsUpdateDragon { get; set; }

    public AlbumPassiveNotice(int isUpdateChara, int isUpdateDragon)
    {
        this.IsUpdateChara = isUpdateChara;
        this.IsUpdateDragon = isUpdateDragon;
    }

    public AlbumPassiveNotice() { }
}

[MessagePackObject(true)]
public class AlbumWeaponList
{
    public int WeaponId { get; set; }
    public int Gettime { get; set; }

    public AlbumWeaponList(int weaponId, int gettime)
    {
        this.WeaponId = weaponId;
        this.Gettime = gettime;
    }

    public AlbumWeaponList() { }
}

[MessagePackObject(true)]
public class AmuletList
{
    public int AmuletId { get; set; }
    public ulong AmuletKeyId { get; set; }
    public int IsLock { get; set; }
    public int IsNew { get; set; }
    public int Gettime { get; set; }
    public int Ability1Level { get; set; }
    public int Ability2Level { get; set; }
    public int Ability3Level { get; set; }
    public int HpPlusCount { get; set; }
    public int AttackPlusCount { get; set; }
    public int StatusPlusCount { get; set; }
    public int Level { get; set; }
    public int LimitBreakCount { get; set; }
    public int Exp { get; set; }

    public AmuletList(
        int amuletId,
        ulong amuletKeyId,
        int isLock,
        int isNew,
        int gettime,
        int ability1Level,
        int ability2Level,
        int ability3Level,
        int hpPlusCount,
        int attackPlusCount,
        int statusPlusCount,
        int level,
        int limitBreakCount,
        int exp
    )
    {
        this.AmuletId = amuletId;
        this.AmuletKeyId = amuletKeyId;
        this.IsLock = isLock;
        this.IsNew = isNew;
        this.Gettime = gettime;
        this.Ability1Level = ability1Level;
        this.Ability2Level = ability2Level;
        this.Ability3Level = ability3Level;
        this.HpPlusCount = hpPlusCount;
        this.AttackPlusCount = attackPlusCount;
        this.StatusPlusCount = statusPlusCount;
        this.Level = level;
        this.LimitBreakCount = limitBreakCount;
        this.Exp = exp;
    }

    public AmuletList() { }
}

[MessagePackObject(true)]
public class AmuletTradeList
{
    public int AmuletTradeId { get; set; }
    public int AmuletId { get; set; }
    public int NeedDewPoint1 { get; set; }
    public int NeedDewPoint2 { get; set; }
    public int NeedDewPoint3 { get; set; }
    public int NeedDewPoint4 { get; set; }
    public int NeedDewPoint5 { get; set; }
    public int Priority { get; set; }
    public int Limit { get; set; }
    public int CompleteDate { get; set; }
    public int PickupViewStartDate { get; set; }
    public int PickupViewEndDate { get; set; }

    public AmuletTradeList(
        int amuletTradeId,
        int amuletId,
        int needDewPoint1,
        int needDewPoint2,
        int needDewPoint3,
        int needDewPoint4,
        int needDewPoint5,
        int priority,
        int limit,
        int completeDate,
        int pickupViewStartDate,
        int pickupViewEndDate
    )
    {
        this.AmuletTradeId = amuletTradeId;
        this.AmuletId = amuletId;
        this.NeedDewPoint1 = needDewPoint1;
        this.NeedDewPoint2 = needDewPoint2;
        this.NeedDewPoint3 = needDewPoint3;
        this.NeedDewPoint4 = needDewPoint4;
        this.NeedDewPoint5 = needDewPoint5;
        this.Priority = priority;
        this.Limit = limit;
        this.CompleteDate = completeDate;
        this.PickupViewStartDate = pickupViewStartDate;
        this.PickupViewEndDate = pickupViewEndDate;
    }

    public AmuletTradeList() { }
}

[MessagePackObject(true)]
public class ApiTest { }

[MessagePackObject(true)]
public class AreaInfoList
{
    public string ScenePath { get; set; }
    public string AreaName { get; set; }

    public AreaInfoList(string scenePath, string areaName)
    {
        this.ScenePath = scenePath;
        this.AreaName = areaName;
    }

    public AreaInfoList() { }
}

[MessagePackObject(true)]
public class AstralItemList
{
    public int AstralItemId { get; set; }
    public int Quantity { get; set; }

    public AstralItemList(int astralItemId, int quantity)
    {
        this.AstralItemId = astralItemId;
        this.Quantity = quantity;
    }

    public AstralItemList() { }
}

[MessagePackObject(true)]
public class AtgenAddCoinList
{
    public long BuildId { get; set; }
    public int AddCoin { get; set; }

    public AtgenAddCoinList(long buildId, int addCoin)
    {
        this.BuildId = buildId;
        this.AddCoin = addCoin;
    }

    public AtgenAddCoinList() { }
}

[MessagePackObject(true)]
public class AtgenAddHarvestList
{
    public Materials MaterialId { get; set; }
    public int AddNum { get; set; }

    public AtgenAddHarvestList(Materials materialId, int addNum)
    {
        this.MaterialId = materialId;
        this.AddNum = addNum;
    }

    public AtgenAddHarvestList() { }
}

[MessagePackObject(true)]
public class AtgenAddStaminaList
{
    public long BuildId { get; set; }
    public int AddStamina { get; set; }

    public AtgenAddStaminaList(long buildId, int addStamina)
    {
        this.BuildId = buildId;
        this.AddStamina = addStamina;
    }

    public AtgenAddStaminaList() { }
}

[MessagePackObject(true)]
public class AtgenAlbumQuestPlayRecordList
{
    public int QuestPlayRecordId { get; set; }
    public int QuestPlayRecordValue { get; set; }

    public AtgenAlbumQuestPlayRecordList(int questPlayRecordId, int questPlayRecordValue)
    {
        this.QuestPlayRecordId = questPlayRecordId;
        this.QuestPlayRecordValue = questPlayRecordValue;
    }

    public AtgenAlbumQuestPlayRecordList() { }
}

[MessagePackObject(true)]
public class AtgenAllBonus
{
    public int Hp { get; set; }
    public int Attack { get; set; }

    public AtgenAllBonus(int hp, int attack)
    {
        this.Hp = hp;
        this.Attack = attack;
    }

    public AtgenAllBonus() { }
}

[MessagePackObject(true)]
public class AtgenArchivePartyCharaList
{
    public int UnitNo { get; set; }
    public int CharaId { get; set; }

    public AtgenArchivePartyCharaList(int unitNo, int charaId)
    {
        this.UnitNo = unitNo;
        this.CharaId = charaId;
    }

    public AtgenArchivePartyCharaList() { }
}

[MessagePackObject(true)]
public class AtgenArchivePartyUnitList
{
    public int UnitNo { get; set; }
    public int CharaId { get; set; }
    public int EquipDragonId { get; set; }
    public int EquipWeaponBodyId { get; set; }
    public int EquipCrestSlotType1CrestId1 { get; set; }
    public int EquipCrestSlotType1CrestId2 { get; set; }
    public int EquipCrestSlotType1CrestId3 { get; set; }
    public int EquipCrestSlotType2CrestId1 { get; set; }
    public int EquipCrestSlotType2CrestId2 { get; set; }
    public int EquipCrestSlotType3CrestId1 { get; set; }
    public int EquipCrestSlotType3CrestId2 { get; set; }
    public int EquipTalismanId { get; set; }
    public int EquipTalismanAbilityId1 { get; set; }
    public int EquipTalismanAbilityId2 { get; set; }
    public int EquipTalismanAbilityId3 { get; set; }
    public int EditSkill1CharaId { get; set; }
    public int EditSkill2CharaId { get; set; }
    public int ExAbility1CharaId { get; set; }
    public int ExAbility2CharaId { get; set; }
    public int ExAbility3CharaId { get; set; }
    public int ExAbility4CharaId { get; set; }

    public AtgenArchivePartyUnitList(
        int unitNo,
        int charaId,
        int equipDragonId,
        int equipWeaponBodyId,
        int equipCrestSlotType1CrestId1,
        int equipCrestSlotType1CrestId2,
        int equipCrestSlotType1CrestId3,
        int equipCrestSlotType2CrestId1,
        int equipCrestSlotType2CrestId2,
        int equipCrestSlotType3CrestId1,
        int equipCrestSlotType3CrestId2,
        int equipTalismanId,
        int equipTalismanAbilityId1,
        int equipTalismanAbilityId2,
        int equipTalismanAbilityId3,
        int editSkill1CharaId,
        int editSkill2CharaId,
        int exAbility1CharaId,
        int exAbility2CharaId,
        int exAbility3CharaId,
        int exAbility4CharaId
    )
    {
        this.UnitNo = unitNo;
        this.CharaId = charaId;
        this.EquipDragonId = equipDragonId;
        this.EquipWeaponBodyId = equipWeaponBodyId;
        this.EquipCrestSlotType1CrestId1 = equipCrestSlotType1CrestId1;
        this.EquipCrestSlotType1CrestId2 = equipCrestSlotType1CrestId2;
        this.EquipCrestSlotType1CrestId3 = equipCrestSlotType1CrestId3;
        this.EquipCrestSlotType2CrestId1 = equipCrestSlotType2CrestId1;
        this.EquipCrestSlotType2CrestId2 = equipCrestSlotType2CrestId2;
        this.EquipCrestSlotType3CrestId1 = equipCrestSlotType3CrestId1;
        this.EquipCrestSlotType3CrestId2 = equipCrestSlotType3CrestId2;
        this.EquipTalismanId = equipTalismanId;
        this.EquipTalismanAbilityId1 = equipTalismanAbilityId1;
        this.EquipTalismanAbilityId2 = equipTalismanAbilityId2;
        this.EquipTalismanAbilityId3 = equipTalismanAbilityId3;
        this.EditSkill1CharaId = editSkill1CharaId;
        this.EditSkill2CharaId = editSkill2CharaId;
        this.ExAbility1CharaId = exAbility1CharaId;
        this.ExAbility2CharaId = exAbility2CharaId;
        this.ExAbility3CharaId = exAbility3CharaId;
        this.ExAbility4CharaId = exAbility4CharaId;
    }

    public AtgenArchivePartyUnitList() { }
}

[MessagePackObject(true)]
public class AtgenBattleRoyalData
{
    public int EventCycleId { get; set; }
    public int CharaId { get; set; }
    public int DragonId { get; set; }
    public int WeaponSkinId { get; set; }
    public int SpecialSkillId { get; set; }
    public string DungeonKey { get; set; }

    public AtgenBattleRoyalData(
        int eventCycleId,
        int charaId,
        int dragonId,
        int weaponSkinId,
        int specialSkillId,
        string dungeonKey
    )
    {
        this.EventCycleId = eventCycleId;
        this.CharaId = charaId;
        this.DragonId = dragonId;
        this.WeaponSkinId = weaponSkinId;
        this.SpecialSkillId = specialSkillId;
        this.DungeonKey = dungeonKey;
    }

    public AtgenBattleRoyalData() { }
}

[MessagePackObject(true)]
public class AtgenBattleRoyalHistoryList
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int CharaId { get; set; }
    public int UseWeaponType { get; set; }
    public int Ranking { get; set; }
    public int KillCount { get; set; }
    public int AssistCount { get; set; }
    public int BattleRoyalPoint { get; set; }
    public int StartTime { get; set; }

    public AtgenBattleRoyalHistoryList(
        int id,
        int eventId,
        int charaId,
        int useWeaponType,
        int ranking,
        int killCount,
        int assistCount,
        int battleRoyalPoint,
        int startTime
    )
    {
        this.Id = id;
        this.EventId = eventId;
        this.CharaId = charaId;
        this.UseWeaponType = useWeaponType;
        this.Ranking = ranking;
        this.KillCount = killCount;
        this.AssistCount = assistCount;
        this.BattleRoyalPoint = battleRoyalPoint;
        this.StartTime = startTime;
    }

    public AtgenBattleRoyalHistoryList() { }
}

[MessagePackObject(true)]
public class AtgenBattleRoyalRecord
{
    public int Ranking { get; set; }
    public int KillCount { get; set; }
    public int AssistCount { get; set; }

    public AtgenBattleRoyalRecord(int ranking, int killCount, int assistCount)
    {
        this.Ranking = ranking;
        this.KillCount = killCount;
        this.AssistCount = assistCount;
    }

    public AtgenBattleRoyalRecord() { }
}

[MessagePackObject(true)]
public class AtgenBonusFactorList
{
    public int FactorType { get; set; }
    public float FactorValue { get; set; }

    public AtgenBonusFactorList(int factorType, float factorValue)
    {
        this.FactorType = factorType;
        this.FactorValue = factorValue;
    }

    public AtgenBonusFactorList() { }
}

[MessagePackObject(true)]
public class AtgenBoxSummonData
{
    public int EventId { get; set; }
    public int EventPoint { get; set; }
    public int BoxSummonSeq { get; set; }
    public int ResetPossible { get; set; }
    public int RemainingQuantity { get; set; }
    public int MaxExecCount { get; set; }
    public IEnumerable<AtgenBoxSummonDetail> BoxSummonDetail { get; set; }

    public AtgenBoxSummonData(
        int eventId,
        int eventPoint,
        int boxSummonSeq,
        int resetPossible,
        int remainingQuantity,
        int maxExecCount,
        IEnumerable<AtgenBoxSummonDetail> boxSummonDetail
    )
    {
        this.EventId = eventId;
        this.EventPoint = eventPoint;
        this.BoxSummonSeq = boxSummonSeq;
        this.ResetPossible = resetPossible;
        this.RemainingQuantity = remainingQuantity;
        this.MaxExecCount = maxExecCount;
        this.BoxSummonDetail = boxSummonDetail;
    }

    public AtgenBoxSummonData() { }
}

[MessagePackObject(true)]
public class AtgenBoxSummonDetail
{
    public int Id { get; set; }
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }
    public int EntityQuantity { get; set; }
    public int Limit { get; set; }
    public int PickupItemState { get; set; }
    public int ResetItemFlag { get; set; }
    public int TotalCount { get; set; }
    public int TwoStepId { get; set; }

    public AtgenBoxSummonDetail(
        int id,
        EntityTypes entityType,
        int entityId,
        int entityQuantity,
        int limit,
        int pickupItemState,
        int resetItemFlag,
        int totalCount,
        int twoStepId
    )
    {
        this.Id = id;
        this.EntityType = entityType;
        this.EntityId = entityId;
        this.EntityQuantity = entityQuantity;
        this.Limit = limit;
        this.PickupItemState = pickupItemState;
        this.ResetItemFlag = resetItemFlag;
        this.TotalCount = totalCount;
        this.TwoStepId = twoStepId;
    }

    public AtgenBoxSummonDetail() { }
}

[MessagePackObject(true)]
public class AtgenBoxSummonResult
{
    public int EventId { get; set; }
    public int BoxSummonSeq { get; set; }
    public int ResetPossible { get; set; }
    public int RemainingQuantity { get; set; }
    public int MaxExecCount { get; set; }
    public int IsStoppedByTarget { get; set; }
    public IEnumerable<AtgenDrawDetails> DrawDetails { get; set; }
    public IEnumerable<AtgenBoxSummonDetail> BoxSummonDetail { get; set; }
    public int EventPoint { get; set; }

    public AtgenBoxSummonResult(
        int eventId,
        int boxSummonSeq,
        int resetPossible,
        int remainingQuantity,
        int maxExecCount,
        int isStoppedByTarget,
        IEnumerable<AtgenDrawDetails> drawDetails,
        IEnumerable<AtgenBoxSummonDetail> boxSummonDetail,
        int eventPoint
    )
    {
        this.EventId = eventId;
        this.BoxSummonSeq = boxSummonSeq;
        this.ResetPossible = resetPossible;
        this.RemainingQuantity = remainingQuantity;
        this.MaxExecCount = maxExecCount;
        this.IsStoppedByTarget = isStoppedByTarget;
        this.DrawDetails = drawDetails;
        this.BoxSummonDetail = boxSummonDetail;
        this.EventPoint = eventPoint;
    }

    public AtgenBoxSummonResult() { }
}

[MessagePackObject(true)]
public class AtgenBuildEventRewardEntityList
{
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }
    public int EntityQuantity { get; set; }

    public AtgenBuildEventRewardEntityList(EntityTypes entityType, int entityId, int entityQuantity)
    {
        this.EntityType = entityType;
        this.EntityId = entityId;
        this.EntityQuantity = entityQuantity;
    }

    public AtgenBuildEventRewardEntityList() { }
}

[MessagePackObject(true)]
public class AtgenBuildupAbilityCrestPieceList
{
    public BuildupPieceTypes BuildupPieceType { get; set; }
    public int Step { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool IsUseDedicatedMaterial { get; set; }

    public AtgenBuildupAbilityCrestPieceList(
        BuildupPieceTypes buildupPieceType,
        int step,
        bool isUseDedicatedMaterial
    )
    {
        this.BuildupPieceType = buildupPieceType;
        this.Step = step;
        this.IsUseDedicatedMaterial = isUseDedicatedMaterial;
    }

    public AtgenBuildupAbilityCrestPieceList() { }
}

[MessagePackObject(true)]
public class AtgenBuildupWeaponBodyPieceList
{
    public BuildupPieceTypes BuildupPieceType { get; set; }
    public int BuildupPieceNo { get; set; }
    public int Step { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool IsUseDedicatedMaterial { get; set; }

    public AtgenBuildupWeaponBodyPieceList(
        BuildupPieceTypes buildupPieceType,
        int buildupPieceNo,
        int step,
        bool isUseDedicatedMaterial
    )
    {
        this.BuildupPieceType = buildupPieceType;
        this.BuildupPieceNo = buildupPieceNo;
        this.Step = step;
        this.IsUseDedicatedMaterial = isUseDedicatedMaterial;
    }

    public AtgenBuildupWeaponBodyPieceList() { }
}

[MessagePackObject(true)]
public class AtgenCategoryList
{
    public int CategoryId { get; set; }
    public string Name { get; set; }

    public AtgenCategoryList(int categoryId, string name)
    {
        this.CategoryId = categoryId;
        this.Name = name;
    }

    public AtgenCategoryList() { }
}

[MessagePackObject(true)]
public class AtgenCharaGrowRecord
{
    public Charas CharaId { get; set; }
    public int TakeExp { get; set; }

    public AtgenCharaGrowRecord(Charas charaId, int takeExp)
    {
        this.CharaId = charaId;
        this.TakeExp = takeExp;
    }

    public AtgenCharaGrowRecord() { }
}

[MessagePackObject(true)]
public class AtgenCharaHonorList
{
    public int CharaId { get; set; }
    public IEnumerable<AtgenHonorList> HonorList { get; set; }

    public AtgenCharaHonorList(int charaId, IEnumerable<AtgenHonorList> honorList)
    {
        this.CharaId = charaId;
        this.HonorList = honorList;
    }

    public AtgenCharaHonorList() { }
}

[MessagePackObject(true)]
public class AtgenCharaUnitSetDetailList
{
    public int UnitSetNo { get; set; }
    public string UnitSetName { get; set; }
    public long DragonKeyId { get; set; }
    public WeaponBodies WeaponBodyId { get; set; }
    public AbilityCrests CrestSlotType1CrestId1 { get; set; }
    public AbilityCrests CrestSlotType1CrestId2 { get; set; }
    public AbilityCrests CrestSlotType1CrestId3 { get; set; }
    public AbilityCrests CrestSlotType2CrestId1 { get; set; }
    public AbilityCrests CrestSlotType2CrestId2 { get; set; }
    public AbilityCrests CrestSlotType3CrestId1 { get; set; }
    public AbilityCrests CrestSlotType3CrestId2 { get; set; }
    public long TalismanKeyId { get; set; }

    public AtgenCharaUnitSetDetailList(
        int unitSetNo,
        string unitSetName,
        long dragonKeyId,
        WeaponBodies weaponBodyId,
        AbilityCrests crestSlotType1CrestId1,
        AbilityCrests crestSlotType1CrestId2,
        AbilityCrests crestSlotType1CrestId3,
        AbilityCrests crestSlotType2CrestId1,
        AbilityCrests crestSlotType2CrestId2,
        AbilityCrests crestSlotType3CrestId1,
        AbilityCrests crestSlotType3CrestId2,
        long talismanKeyId
    )
    {
        this.UnitSetNo = unitSetNo;
        this.UnitSetName = unitSetName;
        this.DragonKeyId = dragonKeyId;
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

    public AtgenCharaUnitSetDetailList() { }
}

[MessagePackObject(true)]
public class AtgenCommentList
{
    public string CommentText { get; set; }
    public string AuthorType { get; set; }
    public int CommentCreatedAt { get; set; }

    public AtgenCommentList(string commentText, string authorType, int commentCreatedAt)
    {
        this.CommentText = commentText;
        this.AuthorType = authorType;
        this.CommentCreatedAt = commentCreatedAt;
    }

    public AtgenCommentList() { }
}

[MessagePackObject(true)]
public class AtgenCsSummonList
{
    public IEnumerable<SummonList> SummonList { get; set; }
    public IEnumerable<SummonList> CampaignSummonList { get; set; }
    public IEnumerable<SummonList> CampaignSsrSummonList { get; set; }
    public IEnumerable<SummonList> PlatinumSummonList { get; set; }
    public IEnumerable<SummonList> ExcludeSummonList { get; set; }

    public AtgenCsSummonList(
        IEnumerable<SummonList> summonList,
        IEnumerable<SummonList> campaignSummonList,
        IEnumerable<SummonList> campaignSsrSummonList,
        IEnumerable<SummonList> platinumSummonList,
        IEnumerable<SummonList> excludeSummonList
    )
    {
        this.SummonList = summonList;
        this.CampaignSummonList = campaignSummonList;
        this.CampaignSsrSummonList = campaignSsrSummonList;
        this.PlatinumSummonList = platinumSummonList;
        this.ExcludeSummonList = excludeSummonList;
    }

    public AtgenCsSummonList() { }
}

[MessagePackObject(true)]
public class AtgenDamageRecord
{
    public int Total { get; set; }
    public int Skill { get; set; }
    public int Dot { get; set; }
    public int Critical { get; set; }
    public int Enchant { get; set; }

    public AtgenDamageRecord(int total, int skill, int dot, int critical, int enchant)
    {
        this.Total = total;
        this.Skill = skill;
        this.Dot = dot;
        this.Critical = critical;
        this.Enchant = enchant;
    }

    public AtgenDamageRecord() { }
}

[MessagePackObject(true)]
public class AtgenDebugDamageRecordLog
{
    public int CharaId { get; set; }
    public string Name { get; set; }
    public string SecondName { get; set; }
    public int WeaponType { get; set; }
    public int ElementalType { get; set; }
    public int Level { get; set; }
    public int Hp { get; set; }
    public int Attack { get; set; }
    public int ExAbilityLevel { get; set; }
    public int ExAbility2Level { get; set; }
    public int Ability1Level { get; set; }
    public int Ability2Level { get; set; }
    public int Ability3Level { get; set; }
    public int Skill1Level { get; set; }
    public int Skill2Level { get; set; }
    public int BurstAttackLevel { get; set; }
    public int ComboBuildupCount { get; set; }
    public int HpPlusCount { get; set; }
    public int AttackPlusCount { get; set; }
    public int Total { get; set; }
    public int Skill { get; set; }
    public int Dot { get; set; }
    public int Critical { get; set; }
    public int Enchant { get; set; }
    public int DragonTotal { get; set; }
    public int DragonSkill { get; set; }
    public int DragonDot { get; set; }
    public int DragonCritical { get; set; }
    public int DragonEnchant { get; set; }
    public string BuildNumber { get; set; }
    public string ResourceVersion { get; set; }
    public string ServerId { get; set; }
    public string DeviceName { get; set; }

    public AtgenDebugDamageRecordLog(
        int charaId,
        string name,
        string secondName,
        int weaponType,
        int elementalType,
        int level,
        int hp,
        int attack,
        int exAbilityLevel,
        int exAbility2Level,
        int ability1Level,
        int ability2Level,
        int ability3Level,
        int skill1Level,
        int skill2Level,
        int burstAttackLevel,
        int comboBuildupCount,
        int hpPlusCount,
        int attackPlusCount,
        int total,
        int skill,
        int dot,
        int critical,
        int enchant,
        int dragonTotal,
        int dragonSkill,
        int dragonDot,
        int dragonCritical,
        int dragonEnchant,
        string buildNumber,
        string resourceVersion,
        string serverId,
        string deviceName
    )
    {
        this.CharaId = charaId;
        this.Name = name;
        this.SecondName = secondName;
        this.WeaponType = weaponType;
        this.ElementalType = elementalType;
        this.Level = level;
        this.Hp = hp;
        this.Attack = attack;
        this.ExAbilityLevel = exAbilityLevel;
        this.ExAbility2Level = exAbility2Level;
        this.Ability1Level = ability1Level;
        this.Ability2Level = ability2Level;
        this.Ability3Level = ability3Level;
        this.Skill1Level = skill1Level;
        this.Skill2Level = skill2Level;
        this.BurstAttackLevel = burstAttackLevel;
        this.ComboBuildupCount = comboBuildupCount;
        this.HpPlusCount = hpPlusCount;
        this.AttackPlusCount = attackPlusCount;
        this.Total = total;
        this.Skill = skill;
        this.Dot = dot;
        this.Critical = critical;
        this.Enchant = enchant;
        this.DragonTotal = dragonTotal;
        this.DragonSkill = dragonSkill;
        this.DragonDot = dragonDot;
        this.DragonCritical = dragonCritical;
        this.DragonEnchant = dragonEnchant;
        this.BuildNumber = buildNumber;
        this.ResourceVersion = resourceVersion;
        this.ServerId = serverId;
        this.DeviceName = deviceName;
    }

    public AtgenDebugDamageRecordLog() { }
}

[MessagePackObject(true)]
public class AtgenDebugDebugPartyList
{
    public int Id { get; set; }
    public int PartyNo { get; set; }
    public int CharaId { get; set; }
    public int CharaLevel { get; set; }
    public int CharaRarity { get; set; }
    public int CharaHpPlusCount { get; set; }
    public int CharaAttackPlusCount { get; set; }
    public int ReleaseManaCircle { get; set; }
    public int DragonId { get; set; }
    public int DragonLevel { get; set; }
    public int DragonLimit { get; set; }
    public int DragonHpPlusCount { get; set; }
    public int DragonAttackPlusCount { get; set; }
    public int DragonReliabilityLevel { get; set; }
    public int WeaponBodyId { get; set; }
    public int WeaponBodyBuildupCount { get; set; }
    public int WeaponBodyLimitBreakCount { get; set; }
    public int WeaponBodyLimitOverCount { get; set; }
    public int CrestSlotType1AbilityCrestId1 { get; set; }
    public int CrestSlotType1AbilityCrestBuildupCount1 { get; set; }
    public int CrestSlotType1AbilityCrestLimitBreakCount1 { get; set; }
    public int CrestSlotType1AbilityCrestHpPlusCount1 { get; set; }
    public int CrestSlotType1AbilityCrestAttackPlusCount1 { get; set; }
    public int CrestSlotType1AbilityCrestId2 { get; set; }
    public int CrestSlotType1AbilityCrestBuildupCount2 { get; set; }
    public int CrestSlotType1AbilityCrestLimitBreakCount2 { get; set; }
    public int CrestSlotType1AbilityCrestHpPlusCount2 { get; set; }
    public int CrestSlotType1AbilityCrestAttackPlusCount2 { get; set; }
    public int CrestSlotType1AbilityCrestId3 { get; set; }
    public int CrestSlotType1AbilityCrestBuildupCount3 { get; set; }
    public int CrestSlotType1AbilityCrestLimitBreakCount3 { get; set; }
    public int CrestSlotType1AbilityCrestHpPlusCount3 { get; set; }
    public int CrestSlotType1AbilityCrestAttackPlusCount3 { get; set; }
    public int CrestSlotType2AbilityCrestId1 { get; set; }
    public int CrestSlotType2AbilityCrestBuildupCount1 { get; set; }
    public int CrestSlotType2AbilityCrestLimitBreakCount1 { get; set; }
    public int CrestSlotType2AbilityCrestHpPlusCount1 { get; set; }
    public int CrestSlotType2AbilityCrestAttackPlusCount1 { get; set; }
    public int CrestSlotType2AbilityCrestId2 { get; set; }
    public int CrestSlotType2AbilityCrestBuildupCount2 { get; set; }
    public int CrestSlotType2AbilityCrestLimitBreakCount2 { get; set; }
    public int CrestSlotType2AbilityCrestHpPlusCount2 { get; set; }
    public int CrestSlotType2AbilityCrestAttackPlusCount2 { get; set; }
    public int CrestSlotType3AbilityCrestId1 { get; set; }
    public int CrestSlotType3AbilityCrestBuildupCount1 { get; set; }
    public int CrestSlotType3AbilityCrestLimitBreakCount1 { get; set; }
    public int CrestSlotType3AbilityCrestHpPlusCount1 { get; set; }
    public int CrestSlotType3AbilityCrestAttackPlusCount1 { get; set; }
    public int CrestSlotType3AbilityCrestId2 { get; set; }
    public int CrestSlotType3AbilityCrestBuildupCount2 { get; set; }
    public int CrestSlotType3AbilityCrestLimitBreakCount2 { get; set; }
    public int CrestSlotType3AbilityCrestHpPlusCount2 { get; set; }
    public int CrestSlotType3AbilityCrestAttackPlusCount2 { get; set; }
    public string Title { get; set; }

    public AtgenDebugDebugPartyList(
        int id,
        int partyNo,
        int charaId,
        int charaLevel,
        int charaRarity,
        int charaHpPlusCount,
        int charaAttackPlusCount,
        int releaseManaCircle,
        int dragonId,
        int dragonLevel,
        int dragonLimit,
        int dragonHpPlusCount,
        int dragonAttackPlusCount,
        int dragonReliabilityLevel,
        int weaponBodyId,
        int weaponBodyBuildupCount,
        int weaponBodyLimitBreakCount,
        int weaponBodyLimitOverCount,
        int crestSlotType1AbilityCrestId1,
        int crestSlotType1AbilityCrestBuildupCount1,
        int crestSlotType1AbilityCrestLimitBreakCount1,
        int crestSlotType1AbilityCrestHpPlusCount1,
        int crestSlotType1AbilityCrestAttackPlusCount1,
        int crestSlotType1AbilityCrestId2,
        int crestSlotType1AbilityCrestBuildupCount2,
        int crestSlotType1AbilityCrestLimitBreakCount2,
        int crestSlotType1AbilityCrestHpPlusCount2,
        int crestSlotType1AbilityCrestAttackPlusCount2,
        int crestSlotType1AbilityCrestId3,
        int crestSlotType1AbilityCrestBuildupCount3,
        int crestSlotType1AbilityCrestLimitBreakCount3,
        int crestSlotType1AbilityCrestHpPlusCount3,
        int crestSlotType1AbilityCrestAttackPlusCount3,
        int crestSlotType2AbilityCrestId1,
        int crestSlotType2AbilityCrestBuildupCount1,
        int crestSlotType2AbilityCrestLimitBreakCount1,
        int crestSlotType2AbilityCrestHpPlusCount1,
        int crestSlotType2AbilityCrestAttackPlusCount1,
        int crestSlotType2AbilityCrestId2,
        int crestSlotType2AbilityCrestBuildupCount2,
        int crestSlotType2AbilityCrestLimitBreakCount2,
        int crestSlotType2AbilityCrestHpPlusCount2,
        int crestSlotType2AbilityCrestAttackPlusCount2,
        int crestSlotType3AbilityCrestId1,
        int crestSlotType3AbilityCrestBuildupCount1,
        int crestSlotType3AbilityCrestLimitBreakCount1,
        int crestSlotType3AbilityCrestHpPlusCount1,
        int crestSlotType3AbilityCrestAttackPlusCount1,
        int crestSlotType3AbilityCrestId2,
        int crestSlotType3AbilityCrestBuildupCount2,
        int crestSlotType3AbilityCrestLimitBreakCount2,
        int crestSlotType3AbilityCrestHpPlusCount2,
        int crestSlotType3AbilityCrestAttackPlusCount2,
        string title
    )
    {
        this.Id = id;
        this.PartyNo = partyNo;
        this.CharaId = charaId;
        this.CharaLevel = charaLevel;
        this.CharaRarity = charaRarity;
        this.CharaHpPlusCount = charaHpPlusCount;
        this.CharaAttackPlusCount = charaAttackPlusCount;
        this.ReleaseManaCircle = releaseManaCircle;
        this.DragonId = dragonId;
        this.DragonLevel = dragonLevel;
        this.DragonLimit = dragonLimit;
        this.DragonHpPlusCount = dragonHpPlusCount;
        this.DragonAttackPlusCount = dragonAttackPlusCount;
        this.DragonReliabilityLevel = dragonReliabilityLevel;
        this.WeaponBodyId = weaponBodyId;
        this.WeaponBodyBuildupCount = weaponBodyBuildupCount;
        this.WeaponBodyLimitBreakCount = weaponBodyLimitBreakCount;
        this.WeaponBodyLimitOverCount = weaponBodyLimitOverCount;
        this.CrestSlotType1AbilityCrestId1 = crestSlotType1AbilityCrestId1;
        this.CrestSlotType1AbilityCrestBuildupCount1 = crestSlotType1AbilityCrestBuildupCount1;
        this.CrestSlotType1AbilityCrestLimitBreakCount1 =
            crestSlotType1AbilityCrestLimitBreakCount1;
        this.CrestSlotType1AbilityCrestHpPlusCount1 = crestSlotType1AbilityCrestHpPlusCount1;
        this.CrestSlotType1AbilityCrestAttackPlusCount1 =
            crestSlotType1AbilityCrestAttackPlusCount1;
        this.CrestSlotType1AbilityCrestId2 = crestSlotType1AbilityCrestId2;
        this.CrestSlotType1AbilityCrestBuildupCount2 = crestSlotType1AbilityCrestBuildupCount2;
        this.CrestSlotType1AbilityCrestLimitBreakCount2 =
            crestSlotType1AbilityCrestLimitBreakCount2;
        this.CrestSlotType1AbilityCrestHpPlusCount2 = crestSlotType1AbilityCrestHpPlusCount2;
        this.CrestSlotType1AbilityCrestAttackPlusCount2 =
            crestSlotType1AbilityCrestAttackPlusCount2;
        this.CrestSlotType1AbilityCrestId3 = crestSlotType1AbilityCrestId3;
        this.CrestSlotType1AbilityCrestBuildupCount3 = crestSlotType1AbilityCrestBuildupCount3;
        this.CrestSlotType1AbilityCrestLimitBreakCount3 =
            crestSlotType1AbilityCrestLimitBreakCount3;
        this.CrestSlotType1AbilityCrestHpPlusCount3 = crestSlotType1AbilityCrestHpPlusCount3;
        this.CrestSlotType1AbilityCrestAttackPlusCount3 =
            crestSlotType1AbilityCrestAttackPlusCount3;
        this.CrestSlotType2AbilityCrestId1 = crestSlotType2AbilityCrestId1;
        this.CrestSlotType2AbilityCrestBuildupCount1 = crestSlotType2AbilityCrestBuildupCount1;
        this.CrestSlotType2AbilityCrestLimitBreakCount1 =
            crestSlotType2AbilityCrestLimitBreakCount1;
        this.CrestSlotType2AbilityCrestHpPlusCount1 = crestSlotType2AbilityCrestHpPlusCount1;
        this.CrestSlotType2AbilityCrestAttackPlusCount1 =
            crestSlotType2AbilityCrestAttackPlusCount1;
        this.CrestSlotType2AbilityCrestId2 = crestSlotType2AbilityCrestId2;
        this.CrestSlotType2AbilityCrestBuildupCount2 = crestSlotType2AbilityCrestBuildupCount2;
        this.CrestSlotType2AbilityCrestLimitBreakCount2 =
            crestSlotType2AbilityCrestLimitBreakCount2;
        this.CrestSlotType2AbilityCrestHpPlusCount2 = crestSlotType2AbilityCrestHpPlusCount2;
        this.CrestSlotType2AbilityCrestAttackPlusCount2 =
            crestSlotType2AbilityCrestAttackPlusCount2;
        this.CrestSlotType3AbilityCrestId1 = crestSlotType3AbilityCrestId1;
        this.CrestSlotType3AbilityCrestBuildupCount1 = crestSlotType3AbilityCrestBuildupCount1;
        this.CrestSlotType3AbilityCrestLimitBreakCount1 =
            crestSlotType3AbilityCrestLimitBreakCount1;
        this.CrestSlotType3AbilityCrestHpPlusCount1 = crestSlotType3AbilityCrestHpPlusCount1;
        this.CrestSlotType3AbilityCrestAttackPlusCount1 =
            crestSlotType3AbilityCrestAttackPlusCount1;
        this.CrestSlotType3AbilityCrestId2 = crestSlotType3AbilityCrestId2;
        this.CrestSlotType3AbilityCrestBuildupCount2 = crestSlotType3AbilityCrestBuildupCount2;
        this.CrestSlotType3AbilityCrestLimitBreakCount2 =
            crestSlotType3AbilityCrestLimitBreakCount2;
        this.CrestSlotType3AbilityCrestHpPlusCount2 = crestSlotType3AbilityCrestHpPlusCount2;
        this.CrestSlotType3AbilityCrestAttackPlusCount2 =
            crestSlotType3AbilityCrestAttackPlusCount2;
        this.Title = title;
    }

    public AtgenDebugDebugPartyList() { }
}

[MessagePackObject(true)]
public class AtgenDebugUnitDataList
{
    public int CharaId { get; set; }
    public int CharaLv { get; set; }
    public int CharaHpPlusCount { get; set; }
    public int CharaAttackPlusCount { get; set; }
    public int CharaRarity { get; set; }
    public int ReleaseManaCircle { get; set; }
    public int WeaponBodyId { get; set; }
    public int WeaponBodyBuildupCount { get; set; }
    public int WeaponBodyLimitBreakCount { get; set; }
    public int WeaponBodyLimitOverCount { get; set; }
    public int DragonId { get; set; }
    public int DragonLv { get; set; }
    public int DragonHpPlusCount { get; set; }
    public int DragonAttackPlusCount { get; set; }
    public int DragonLimit { get; set; }
    public int DragonReliabilityLevel { get; set; }
    public int CrestSlotType1AbilityCrestId1 { get; set; }
    public int CrestSlotType1AbilityCrestBuildupCount1 { get; set; }
    public int CrestSlotType1AbilityCrestLimitBreakCount1 { get; set; }
    public int CrestSlotType1AbilityCrestHpPlusCount1 { get; set; }
    public int CrestSlotType1AbilityCrestAttackPlusCount1 { get; set; }
    public int CrestSlotType1AbilityCrestId2 { get; set; }
    public int CrestSlotType1AbilityCrestBuildupCount2 { get; set; }
    public int CrestSlotType1AbilityCrestLimitBreakCount2 { get; set; }
    public int CrestSlotType1AbilityCrestHpPlusCount2 { get; set; }
    public int CrestSlotType1AbilityCrestAttackPlusCount2 { get; set; }
    public int CrestSlotType1AbilityCrestId3 { get; set; }
    public int CrestSlotType1AbilityCrestBuildupCount3 { get; set; }
    public int CrestSlotType1AbilityCrestLimitBreakCount3 { get; set; }
    public int CrestSlotType1AbilityCrestHpPlusCount3 { get; set; }
    public int CrestSlotType1AbilityCrestAttackPlusCount3 { get; set; }
    public int CrestSlotType2AbilityCrestId1 { get; set; }
    public int CrestSlotType2AbilityCrestBuildupCount1 { get; set; }
    public int CrestSlotType2AbilityCrestLimitBreakCount1 { get; set; }
    public int CrestSlotType2AbilityCrestHpPlusCount1 { get; set; }
    public int CrestSlotType2AbilityCrestAttackPlusCount1 { get; set; }
    public int CrestSlotType2AbilityCrestId2 { get; set; }
    public int CrestSlotType2AbilityCrestBuildupCount2 { get; set; }
    public int CrestSlotType2AbilityCrestLimitBreakCount2 { get; set; }
    public int CrestSlotType2AbilityCrestHpPlusCount2 { get; set; }
    public int CrestSlotType2AbilityCrestAttackPlusCount2 { get; set; }
    public int CrestSlotType3AbilityCrestId1 { get; set; }
    public int CrestSlotType3AbilityCrestBuildupCount1 { get; set; }
    public int CrestSlotType3AbilityCrestLimitBreakCount1 { get; set; }
    public int CrestSlotType3AbilityCrestHpPlusCount1 { get; set; }
    public int CrestSlotType3AbilityCrestAttackPlusCount1 { get; set; }
    public int CrestSlotType3AbilityCrestId2 { get; set; }
    public int CrestSlotType3AbilityCrestBuildupCount2 { get; set; }
    public int CrestSlotType3AbilityCrestLimitBreakCount2 { get; set; }
    public int CrestSlotType3AbilityCrestHpPlusCount2 { get; set; }
    public int CrestSlotType3AbilityCrestAttackPlusCount2 { get; set; }

    public AtgenDebugUnitDataList(
        int charaId,
        int charaLv,
        int charaHpPlusCount,
        int charaAttackPlusCount,
        int charaRarity,
        int releaseManaCircle,
        int weaponBodyId,
        int weaponBodyBuildupCount,
        int weaponBodyLimitBreakCount,
        int weaponBodyLimitOverCount,
        int dragonId,
        int dragonLv,
        int dragonHpPlusCount,
        int dragonAttackPlusCount,
        int dragonLimit,
        int dragonReliabilityLevel,
        int crestSlotType1AbilityCrestId1,
        int crestSlotType1AbilityCrestBuildupCount1,
        int crestSlotType1AbilityCrestLimitBreakCount1,
        int crestSlotType1AbilityCrestHpPlusCount1,
        int crestSlotType1AbilityCrestAttackPlusCount1,
        int crestSlotType1AbilityCrestId2,
        int crestSlotType1AbilityCrestBuildupCount2,
        int crestSlotType1AbilityCrestLimitBreakCount2,
        int crestSlotType1AbilityCrestHpPlusCount2,
        int crestSlotType1AbilityCrestAttackPlusCount2,
        int crestSlotType1AbilityCrestId3,
        int crestSlotType1AbilityCrestBuildupCount3,
        int crestSlotType1AbilityCrestLimitBreakCount3,
        int crestSlotType1AbilityCrestHpPlusCount3,
        int crestSlotType1AbilityCrestAttackPlusCount3,
        int crestSlotType2AbilityCrestId1,
        int crestSlotType2AbilityCrestBuildupCount1,
        int crestSlotType2AbilityCrestLimitBreakCount1,
        int crestSlotType2AbilityCrestHpPlusCount1,
        int crestSlotType2AbilityCrestAttackPlusCount1,
        int crestSlotType2AbilityCrestId2,
        int crestSlotType2AbilityCrestBuildupCount2,
        int crestSlotType2AbilityCrestLimitBreakCount2,
        int crestSlotType2AbilityCrestHpPlusCount2,
        int crestSlotType2AbilityCrestAttackPlusCount2,
        int crestSlotType3AbilityCrestId1,
        int crestSlotType3AbilityCrestBuildupCount1,
        int crestSlotType3AbilityCrestLimitBreakCount1,
        int crestSlotType3AbilityCrestHpPlusCount1,
        int crestSlotType3AbilityCrestAttackPlusCount1,
        int crestSlotType3AbilityCrestId2,
        int crestSlotType3AbilityCrestBuildupCount2,
        int crestSlotType3AbilityCrestLimitBreakCount2,
        int crestSlotType3AbilityCrestHpPlusCount2,
        int crestSlotType3AbilityCrestAttackPlusCount2
    )
    {
        this.CharaId = charaId;
        this.CharaLv = charaLv;
        this.CharaHpPlusCount = charaHpPlusCount;
        this.CharaAttackPlusCount = charaAttackPlusCount;
        this.CharaRarity = charaRarity;
        this.ReleaseManaCircle = releaseManaCircle;
        this.WeaponBodyId = weaponBodyId;
        this.WeaponBodyBuildupCount = weaponBodyBuildupCount;
        this.WeaponBodyLimitBreakCount = weaponBodyLimitBreakCount;
        this.WeaponBodyLimitOverCount = weaponBodyLimitOverCount;
        this.DragonId = dragonId;
        this.DragonLv = dragonLv;
        this.DragonHpPlusCount = dragonHpPlusCount;
        this.DragonAttackPlusCount = dragonAttackPlusCount;
        this.DragonLimit = dragonLimit;
        this.DragonReliabilityLevel = dragonReliabilityLevel;
        this.CrestSlotType1AbilityCrestId1 = crestSlotType1AbilityCrestId1;
        this.CrestSlotType1AbilityCrestBuildupCount1 = crestSlotType1AbilityCrestBuildupCount1;
        this.CrestSlotType1AbilityCrestLimitBreakCount1 =
            crestSlotType1AbilityCrestLimitBreakCount1;
        this.CrestSlotType1AbilityCrestHpPlusCount1 = crestSlotType1AbilityCrestHpPlusCount1;
        this.CrestSlotType1AbilityCrestAttackPlusCount1 =
            crestSlotType1AbilityCrestAttackPlusCount1;
        this.CrestSlotType1AbilityCrestId2 = crestSlotType1AbilityCrestId2;
        this.CrestSlotType1AbilityCrestBuildupCount2 = crestSlotType1AbilityCrestBuildupCount2;
        this.CrestSlotType1AbilityCrestLimitBreakCount2 =
            crestSlotType1AbilityCrestLimitBreakCount2;
        this.CrestSlotType1AbilityCrestHpPlusCount2 = crestSlotType1AbilityCrestHpPlusCount2;
        this.CrestSlotType1AbilityCrestAttackPlusCount2 =
            crestSlotType1AbilityCrestAttackPlusCount2;
        this.CrestSlotType1AbilityCrestId3 = crestSlotType1AbilityCrestId3;
        this.CrestSlotType1AbilityCrestBuildupCount3 = crestSlotType1AbilityCrestBuildupCount3;
        this.CrestSlotType1AbilityCrestLimitBreakCount3 =
            crestSlotType1AbilityCrestLimitBreakCount3;
        this.CrestSlotType1AbilityCrestHpPlusCount3 = crestSlotType1AbilityCrestHpPlusCount3;
        this.CrestSlotType1AbilityCrestAttackPlusCount3 =
            crestSlotType1AbilityCrestAttackPlusCount3;
        this.CrestSlotType2AbilityCrestId1 = crestSlotType2AbilityCrestId1;
        this.CrestSlotType2AbilityCrestBuildupCount1 = crestSlotType2AbilityCrestBuildupCount1;
        this.CrestSlotType2AbilityCrestLimitBreakCount1 =
            crestSlotType2AbilityCrestLimitBreakCount1;
        this.CrestSlotType2AbilityCrestHpPlusCount1 = crestSlotType2AbilityCrestHpPlusCount1;
        this.CrestSlotType2AbilityCrestAttackPlusCount1 =
            crestSlotType2AbilityCrestAttackPlusCount1;
        this.CrestSlotType2AbilityCrestId2 = crestSlotType2AbilityCrestId2;
        this.CrestSlotType2AbilityCrestBuildupCount2 = crestSlotType2AbilityCrestBuildupCount2;
        this.CrestSlotType2AbilityCrestLimitBreakCount2 =
            crestSlotType2AbilityCrestLimitBreakCount2;
        this.CrestSlotType2AbilityCrestHpPlusCount2 = crestSlotType2AbilityCrestHpPlusCount2;
        this.CrestSlotType2AbilityCrestAttackPlusCount2 =
            crestSlotType2AbilityCrestAttackPlusCount2;
        this.CrestSlotType3AbilityCrestId1 = crestSlotType3AbilityCrestId1;
        this.CrestSlotType3AbilityCrestBuildupCount1 = crestSlotType3AbilityCrestBuildupCount1;
        this.CrestSlotType3AbilityCrestLimitBreakCount1 =
            crestSlotType3AbilityCrestLimitBreakCount1;
        this.CrestSlotType3AbilityCrestHpPlusCount1 = crestSlotType3AbilityCrestHpPlusCount1;
        this.CrestSlotType3AbilityCrestAttackPlusCount1 =
            crestSlotType3AbilityCrestAttackPlusCount1;
        this.CrestSlotType3AbilityCrestId2 = crestSlotType3AbilityCrestId2;
        this.CrestSlotType3AbilityCrestBuildupCount2 = crestSlotType3AbilityCrestBuildupCount2;
        this.CrestSlotType3AbilityCrestLimitBreakCount2 =
            crestSlotType3AbilityCrestLimitBreakCount2;
        this.CrestSlotType3AbilityCrestHpPlusCount2 = crestSlotType3AbilityCrestHpPlusCount2;
        this.CrestSlotType3AbilityCrestAttackPlusCount2 =
            crestSlotType3AbilityCrestAttackPlusCount2;
    }

    public AtgenDebugUnitDataList() { }
}

[MessagePackObject(true)]
public class AtgenDeleteAmuletList
{
    public ulong AmuletKeyId { get; set; }

    public AtgenDeleteAmuletList(ulong amuletKeyId)
    {
        this.AmuletKeyId = amuletKeyId;
    }

    public AtgenDeleteAmuletList() { }
}

[MessagePackObject(true)]
public class AtgenDeleteDragonList
{
    public ulong DragonKeyId { get; set; }

    public AtgenDeleteDragonList(ulong dragonKeyId)
    {
        this.DragonKeyId = dragonKeyId;
    }

    public AtgenDeleteDragonList() { }
}

[MessagePackObject(true)]
public class AtgenDeleteTalismanList
{
    public long TalismanKeyId { get; set; }

    public AtgenDeleteTalismanList(long talismanKeyId)
    {
        this.TalismanKeyId = talismanKeyId;
    }

    public AtgenDeleteTalismanList() { }
}

[MessagePackObject(true)]
public class AtgenDeleteWeaponList
{
    public ulong WeaponKeyId { get; set; }

    public AtgenDeleteWeaponList(ulong weaponKeyId)
    {
        this.WeaponKeyId = weaponKeyId;
    }

    public AtgenDeleteWeaponList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeAreaInfo
{
    public int FloorNum { get; set; }
    public float QuestTime { get; set; }
    public int DmodeScore { get; set; }
    public int CurrentAreaThemeId { get; set; }
    public int CurrentAreaId { get; set; }

    public AtgenDmodeAreaInfo(
        int floorNum,
        float questTime,
        int dmodeScore,
        int currentAreaThemeId,
        int currentAreaId
    )
    {
        this.FloorNum = floorNum;
        this.QuestTime = questTime;
        this.DmodeScore = dmodeScore;
        this.CurrentAreaThemeId = currentAreaThemeId;
        this.CurrentAreaId = currentAreaId;
    }

    public AtgenDmodeAreaInfo() { }
}

[MessagePackObject(true)]
public class AtgenDmodeDragonUseList
{
    public Dragons DragonId { get; set; }
    public int UseCount { get; set; }

    public AtgenDmodeDragonUseList(Dragons dragonId, int useCount)
    {
        this.DragonId = dragonId;
        this.UseCount = useCount;
    }

    public AtgenDmodeDragonUseList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeDropList
{
    public EntityTypes Type { get; set; }
    public int Id { get; set; }
    public int Quantity { get; set; }

    public AtgenDmodeDropList(EntityTypes type, int id, int quantity)
    {
        this.Type = type;
        this.Id = id;
        this.Quantity = quantity;
    }

    public AtgenDmodeDropList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeDropObj
{
    public int ObjId { get; set; }
    public int ObjType { get; set; }
    public IEnumerable<AtgenDmodeDropList> DmodeDropList { get; set; }

    public AtgenDmodeDropObj(int objId, int objType, IEnumerable<AtgenDmodeDropList> dmodeDropList)
    {
        this.ObjId = objId;
        this.ObjType = objType;
        this.DmodeDropList = dmodeDropList;
    }

    public AtgenDmodeDropObj() { }
}

[MessagePackObject(true)]
public class AtgenDmodeDungeonItemOptionList
{
    public int ItemNo { get; set; }
    public int AbnormalStatusInvalidCount { get; set; }

    public AtgenDmodeDungeonItemOptionList(int itemNo, int abnormalStatusInvalidCount)
    {
        this.ItemNo = itemNo;
        this.AbnormalStatusInvalidCount = abnormalStatusInvalidCount;
    }

    public AtgenDmodeDungeonItemOptionList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeDungeonItemStateList
{
    public int ItemNo { get; set; }
    public DmodeDungeonItemState State { get; set; }

    public AtgenDmodeDungeonItemStateList(int itemNo, DmodeDungeonItemState state)
    {
        this.ItemNo = itemNo;
        this.State = state;
    }

    public AtgenDmodeDungeonItemStateList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeDungeonOdds
{
    public IEnumerable<AtgenDmodeSelectDragonList> DmodeSelectDragonList { get; set; }
    public ICollection<DmodeDungeonItemList> DmodeDungeonItemList { get; set; }
    public DmodeOddsInfo DmodeOddsInfo { get; set; }

    public AtgenDmodeDungeonOdds(
        IEnumerable<AtgenDmodeSelectDragonList> dmodeSelectDragonList,
        ICollection<DmodeDungeonItemList> dmodeDungeonItemList,
        DmodeOddsInfo dmodeOddsInfo
    )
    {
        this.DmodeSelectDragonList = dmodeSelectDragonList;
        this.DmodeDungeonItemList = dmodeDungeonItemList;
        this.DmodeOddsInfo = dmodeOddsInfo;
    }

    public AtgenDmodeDungeonOdds() { }
}

[MessagePackObject(true)]
public class AtgenDmodeEnemy
{
    public int EnemyIdx { get; set; }
    public int IsPop { get; set; }
    public int Level { get; set; }
    public int ParamId { get; set; }
    public IEnumerable<AtgenDmodeDropList> DmodeDropList { get; set; }

    public AtgenDmodeEnemy(
        int enemyIdx,
        int isPop,
        int level,
        int paramId,
        IEnumerable<AtgenDmodeDropList> dmodeDropList
    )
    {
        this.EnemyIdx = enemyIdx;
        this.IsPop = isPop;
        this.Level = level;
        this.ParamId = paramId;
        this.DmodeDropList = dmodeDropList;
    }

    public AtgenDmodeEnemy() { }
}

[MessagePackObject(true)]
public class AtgenDmodeHoldDragonList
{
    public Dragons DragonId { get; set; }
    public int Count { get; set; }

    public AtgenDmodeHoldDragonList(Dragons dragonId, int count)
    {
        this.DragonId = dragonId;
        this.Count = count;
    }

    public AtgenDmodeHoldDragonList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeSelectDragonList
{
    public int SelectDragonNo { get; set; }
    public Dragons DragonId { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsRare { get; set; }
    public int PayDmodePoint1 { get; set; }
    public int PayDmodePoint2 { get; set; }

    public AtgenDmodeSelectDragonList(
        int selectDragonNo,
        Dragons dragonId,
        bool isRare,
        int payDmodePoint1,
        int payDmodePoint2
    )
    {
        this.SelectDragonNo = selectDragonNo;
        this.DragonId = dragonId;
        this.IsRare = isRare;
        this.PayDmodePoint1 = payDmodePoint1;
        this.PayDmodePoint2 = payDmodePoint2;
    }

    public AtgenDmodeSelectDragonList() { }
}

[MessagePackObject(true)]
public class AtgenDmodeTreasureRecord
{
    public IEnumerable<int> DropObj { get; set; }
    public IEnumerable<int> Enemy { get; set; }

    public AtgenDmodeTreasureRecord(IEnumerable<int> dropObj, IEnumerable<int> enemy)
    {
        this.DropObj = dropObj;
        this.Enemy = enemy;
    }

    public AtgenDmodeTreasureRecord() { }
}

[MessagePackObject(true)]
public class AtgenDmodeUnitInfo
{
    public int Level { get; set; }
    public int Exp { get; set; }
    public int[] EquipCrestItemNoSortList { get; set; }
    public IEnumerable<int> BagItemNoSortList { get; set; }
    public IEnumerable<int> SkillBagItemNoSortList { get; set; }
    public IEnumerable<AtgenDmodeHoldDragonList> DmodeHoldDragonList { get; set; }
    public int TakeDmodePoint1 { get; set; }
    public int TakeDmodePoint2 { get; set; }

    public AtgenDmodeUnitInfo(
        int level,
        int exp,
        int[] equipCrestItemNoSortList,
        IEnumerable<int> bagItemNoSortList,
        IEnumerable<int> skillBagItemNoSortList,
        IEnumerable<AtgenDmodeHoldDragonList> dmodeHoldDragonList,
        int takeDmodePoint1,
        int takeDmodePoint2
    )
    {
        this.Level = level;
        this.Exp = exp;
        this.EquipCrestItemNoSortList = equipCrestItemNoSortList;
        this.BagItemNoSortList = bagItemNoSortList;
        this.SkillBagItemNoSortList = skillBagItemNoSortList;
        this.DmodeHoldDragonList = dmodeHoldDragonList;
        this.TakeDmodePoint1 = takeDmodePoint1;
        this.TakeDmodePoint2 = takeDmodePoint2;
    }

    public AtgenDmodeUnitInfo() { }
}

[MessagePackObject(true)]
public class AtgenDragonBonus
{
    public UnitElement ElementalType { get; set; }
    public float DragonBonus { get; set; }
    public float Hp { get; set; }
    public float Attack { get; set; }

    public AtgenDragonBonus(UnitElement elementalType, float dragonBonus, float hp, float attack)
    {
        this.ElementalType = elementalType;
        this.DragonBonus = dragonBonus;
        this.Hp = hp;
        this.Attack = attack;
    }

    public AtgenDragonBonus() { }
}

[MessagePackObject(true)]
public class AtgenDragonGiftRewardList
{
    public DragonGifts DragonGiftId { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsFavorite { get; set; }
    public IEnumerable<DragonRewardEntityList> ReturnGiftList { get; set; }
    public IEnumerable<RewardReliabilityList> RewardReliabilityList { get; set; }

    public AtgenDragonGiftRewardList(
        DragonGifts dragonGiftId,
        bool isFavorite,
        IEnumerable<DragonRewardEntityList> returnGiftList,
        IEnumerable<RewardReliabilityList> rewardReliabilityList
    )
    {
        this.DragonGiftId = dragonGiftId;
        this.IsFavorite = isFavorite;
        this.ReturnGiftList = returnGiftList;
        this.RewardReliabilityList = rewardReliabilityList;
    }

    public AtgenDragonGiftRewardList() { }
}

[MessagePackObject(true)]
public class AtgenDragonTimeBonus
{
    public float DragonTimeBonus { get; set; }

    public AtgenDragonTimeBonus(float dragonTimeBonus)
    {
        this.DragonTimeBonus = dragonTimeBonus;
    }

    public AtgenDragonTimeBonus() { }
}

[MessagePackObject(true)]
public class AtgenDrawDetails
{
    public int Id { get; set; }
    public int IsNew { get; set; }
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }
    public int EntityQuantity { get; set; }
    public int ViewRarity { get; set; }

    public AtgenDrawDetails(
        int id,
        int isNew,
        EntityTypes entityType,
        int entityId,
        int entityQuantity,
        int viewRarity
    )
    {
        this.Id = id;
        this.IsNew = isNew;
        this.EntityType = entityType;
        this.EntityId = entityId;
        this.EntityQuantity = entityQuantity;
        this.ViewRarity = viewRarity;
    }

    public AtgenDrawDetails() { }
}

[MessagePackObject(true)]
public class AtgenDropAll
{
    public int Id { get; set; }
    public EntityTypes Type { get; set; }
    public int Quantity { get; set; }
    public int Place { get; set; }
    public float Factor { get; set; }

    public AtgenDropAll(int id, EntityTypes type, int quantity, int place, float factor)
    {
        this.Id = id;
        this.Type = type;
        this.Quantity = quantity;
        this.Place = place;
        this.Factor = factor;
    }

    public AtgenDropAll() { }
}

[MessagePackObject(true)]
public class AtgenDropList
{
    public EntityTypes Type { get; set; }
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int Place { get; set; }

    public AtgenDropList(EntityTypes type, int id, int quantity, int place)
    {
        this.Type = type;
        this.Id = id;
        this.Quantity = quantity;
        this.Place = place;
    }

    public AtgenDropList() { }
}

[MessagePackObject(true)]
public class AtgenDropObj
{
    public int ObjId { get; set; }
    public int ObjType { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsRare { get; set; }
    public IEnumerable<AtgenDropList> DropList { get; set; }

    public AtgenDropObj(int objId, int objType, bool isRare, IEnumerable<AtgenDropList> dropList)
    {
        this.ObjId = objId;
        this.ObjType = objType;
        this.IsRare = isRare;
        this.DropList = dropList;
    }

    public AtgenDropObj() { }
}

[MessagePackObject(true)]
public class AtgenDuplicateEntityList
{
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }

    public AtgenDuplicateEntityList(EntityTypes entityType, int entityId)
    {
        this.EntityType = entityType;
        this.EntityId = entityId;
    }

    public AtgenDuplicateEntityList() { }
}

[MessagePackObject(true)]
public class AtgenElementBonus
{
    public UnitElement ElementalType { get; set; }
    public float Hp { get; set; }
    public float Attack { get; set; }

    public AtgenElementBonus(UnitElement elementalType, float hp, float attack)
    {
        this.ElementalType = elementalType;
        this.Hp = hp;
        this.Attack = attack;
    }

    public AtgenElementBonus() { }
}

[MessagePackObject(true)]
public class AtgenEnemy
{
    public int Piece { get; set; }
    public int EnemyIdx { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsPop { get; set; } = true;

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsRare { get; set; }
    public int ParamId { get; set; }
    public List<EnemyDropList> EnemyDropList { get; set; }

    public AtgenEnemy(
        int piece,
        int enemyIdx,
        bool isPop,
        bool isRare,
        int paramId,
        List<EnemyDropList> enemyDropList
    )
    {
        this.Piece = piece;
        this.EnemyIdx = enemyIdx;
        this.IsPop = isPop;
        this.IsRare = isRare;
        this.ParamId = paramId;
        this.EnemyDropList = enemyDropList;
    }

    public AtgenEnemy() { }
}

[MessagePackObject(true)]
public class AtgenEnemyPiece
{
    public Materials Id { get; set; }
    public int Quantity { get; set; }

    public AtgenEnemyPiece(Materials id, int quantity)
    {
        this.Id = id;
        this.Quantity = quantity;
    }

    public AtgenEnemyPiece() { }
}

[MessagePackObject(true)]
public class AtgenEnemySmash
{
    public int Count { get; set; }

    public AtgenEnemySmash(int count)
    {
        this.Count = count;
    }

    public AtgenEnemySmash() { }
}

[MessagePackObject(true)]
public class AtgenEntryConditions
{
    public IEnumerable<int> UnacceptedElementTypeList { get; set; }
    public IEnumerable<int> UnacceptedWeaponTypeList { get; set; }
    public int RequiredPartyPower { get; set; }
    public int ObjectiveTextId { get; set; }
    public string ObjectiveFreeText { get; set; }

    public AtgenEntryConditions(
        IEnumerable<int> unacceptedElementTypeList,
        IEnumerable<int> unacceptedWeaponTypeList,
        int requiredPartyPower,
        int objectiveTextId,
        string objectiveFreeText
    )
    {
        this.UnacceptedElementTypeList = unacceptedElementTypeList;
        this.UnacceptedWeaponTypeList = unacceptedWeaponTypeList;
        this.RequiredPartyPower = requiredPartyPower;
        this.ObjectiveTextId = objectiveTextId;
        this.ObjectiveFreeText = objectiveFreeText;
    }

    public AtgenEntryConditions() { }
}

[MessagePackObject(true)]
public class AtgenEventBoost
{
    public EventEffectTypes EventEffect { get; set; }
    public float EffectValue { get; set; }

    public AtgenEventBoost(EventEffectTypes eventEffect, float effectValue)
    {
        this.EventEffect = eventEffect;
        this.EffectValue = effectValue;
    }

    public AtgenEventBoost() { }
}

[MessagePackObject(true)]
public class AtgenEventDamageData
{
    public long UserDamageValue { get; set; }
    public int UserTargetTime { get; set; }
    public long TotalDamageValue { get; set; }
    public int TotalTargetTime { get; set; }
    public int TotalAggregateTime { get; set; }

    public AtgenEventDamageData(
        long userDamageValue,
        int userTargetTime,
        long totalDamageValue,
        int totalTargetTime,
        int totalAggregateTime
    )
    {
        this.UserDamageValue = userDamageValue;
        this.UserTargetTime = userTargetTime;
        this.TotalDamageValue = totalDamageValue;
        this.TotalTargetTime = totalTargetTime;
        this.TotalAggregateTime = totalAggregateTime;
    }

    public AtgenEventDamageData() { }
}

[MessagePackObject(true)]
public class AtgenEventDamageHistoryList
{
    public int TargetTime { get; set; }
    public long TotalDamageValue { get; set; }

    public AtgenEventDamageHistoryList(int targetTime, long totalDamageValue)
    {
        this.TargetTime = targetTime;
        this.TotalDamageValue = totalDamageValue;
    }

    public AtgenEventDamageHistoryList() { }
}

[MessagePackObject(true)]
public class AtgenEventDamageRewardList
{
    public int TargetTime { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> RewardEntityList { get; set; }

    public AtgenEventDamageRewardList(
        int targetTime,
        IEnumerable<AtgenBuildEventRewardEntityList> rewardEntityList
    )
    {
        this.TargetTime = targetTime;
        this.RewardEntityList = rewardEntityList;
    }

    public AtgenEventDamageRewardList() { }
}

[MessagePackObject(true)]
public class AtgenEventFortData
{
    public int PlantId { get; set; }
    public int Level { get; set; }

    public AtgenEventFortData(int plantId, int level)
    {
        this.PlantId = plantId;
        this.Level = level;
    }

    public AtgenEventFortData() { }
}

[MessagePackObject(true)]
public class AtgenEventPassiveUpList
{
    public int PassiveId { get; set; }
    public int Progress { get; set; }

    public AtgenEventPassiveUpList(int passiveId, int progress)
    {
        this.PassiveId = passiveId;
        this.Progress = progress;
    }

    public AtgenEventPassiveUpList() { }
}

[MessagePackObject(true)]
public class AtgenExchangeSummomPointList
{
    public int SummonPointId { get; set; }
    public int SummonPoint { get; set; }
    public int CsSummonPoint { get; set; }

    public AtgenExchangeSummomPointList(int summonPointId, int summonPoint, int csSummonPoint)
    {
        this.SummonPointId = summonPointId;
        this.SummonPoint = summonPoint;
        this.CsSummonPoint = csSummonPoint;
    }

    public AtgenExchangeSummomPointList() { }
}

[MessagePackObject(true)]
public class AtgenFailQuestDetail
{
    public int QuestId { get; set; }
    public int WallId { get; set; }
    public int WallLevel { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsHost { get; set; }

    public AtgenFailQuestDetail(int questId, int wallId, int wallLevel, bool isHost)
    {
        this.QuestId = questId;
        this.WallId = wallId;
        this.WallLevel = wallLevel;
        this.IsHost = isHost;
    }

    public AtgenFailQuestDetail() { }
}

[MessagePackObject(true)]
public class AtgenFirstClearSet
{
    public int Id { get; set; }
    public EntityTypes Type { get; set; }
    public int Quantity { get; set; }

    public AtgenFirstClearSet(int id, EntityTypes type, int quantity)
    {
        this.Id = id;
        this.Type = type;
        this.Quantity = quantity;
    }

    public AtgenFirstClearSet() { }
}

[MessagePackObject(true)]
public class AtgenFirstMeeting
{
    public int Headcount { get; set; }
    public int Id { get; set; }
    public int Type { get; set; }
    public int TotalQuantity { get; set; }

    public AtgenFirstMeeting(int headcount, int id, int type, int totalQuantity)
    {
        this.Headcount = headcount;
        this.Id = id;
        this.Type = type;
        this.TotalQuantity = totalQuantity;
    }

    public AtgenFirstMeeting() { }
}

[MessagePackObject(true)]
public class AtgenGrade
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public int GradeNum { get; set; }
    public IEnumerable<AtgenDropList> DropList { get; set; }

    public AtgenGrade(int minValue, int maxValue, int gradeNum, IEnumerable<AtgenDropList> dropList)
    {
        this.MinValue = minValue;
        this.MaxValue = maxValue;
        this.GradeNum = gradeNum;
        this.DropList = dropList;
    }

    public AtgenGrade() { }
}

[MessagePackObject(true)]
public class AtgenGuild
{
    public int GuildId { get; set; }
    public int GuildEmblemId { get; set; }
    public string GuildName { get; set; }
    public int IsPenaltyGuildName { get; set; }

    public AtgenGuild(int guildId, int guildEmblemId, string guildName, int isPenaltyGuildName)
    {
        this.GuildId = guildId;
        this.GuildEmblemId = guildEmblemId;
        this.GuildName = guildName;
        this.IsPenaltyGuildName = isPenaltyGuildName;
    }

    public AtgenGuild() { }
}

[MessagePackObject(true)]
public class AtgenGuildInviteParamsList
{
    public int GuildId { get; set; }
    public ulong GuildInviteId { get; set; }

    public AtgenGuildInviteParamsList(int guildId, ulong guildInviteId)
    {
        this.GuildId = guildId;
        this.GuildInviteId = guildInviteId;
    }

    public AtgenGuildInviteParamsList() { }
}

[MessagePackObject(true)]
public class AtgenHarvestBuildList
{
    public long BuildId { get; set; }
    public IEnumerable<AtgenAddHarvestList> AddHarvestList { get; set; }

    public AtgenHarvestBuildList(long buildId, IEnumerable<AtgenAddHarvestList> addHarvestList)
    {
        this.BuildId = buildId;
        this.AddHarvestList = addHarvestList;
    }

    public AtgenHarvestBuildList() { }
}

[MessagePackObject(true)]
public class AtgenHelperDetailList
{
    public ulong ViewerId { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsFriend { get; set; }
    public int GetManaPoint { get; set; }
    public int ApplySendStatus { get; set; }

    public AtgenHelperDetailList(
        ulong viewerId,
        bool isFriend,
        int getManaPoint,
        int applySendStatus
    )
    {
        this.ViewerId = viewerId;
        this.IsFriend = isFriend;
        this.GetManaPoint = getManaPoint;
        this.ApplySendStatus = applySendStatus;
    }

    public AtgenHelperDetailList() { }
}

[MessagePackObject(true)]
public class AtgenHonorList
{
    public int HonorId { get; set; }

    public AtgenHonorList(int honorId)
    {
        this.HonorId = honorId;
    }

    public AtgenHonorList() { }
}

[MessagePackObject(true)]
public class AtgenIngameWalker
{
    public int Skill2Level { get; set; }

    public AtgenIngameWalker(int skill2Level)
    {
        this.Skill2Level = skill2Level;
    }

    public AtgenIngameWalker() { }
}

[MessagePackObject(true)]
public class AtgenInquiryFaqList
{
    public int Id { get; set; }
    public string Question { get; set; }
    public string Answer { get; set; }

    public AtgenInquiryFaqList(int id, string question, string answer)
    {
        this.Id = id;
        this.Question = question;
        this.Answer = answer;
    }

    public AtgenInquiryFaqList() { }
}

[MessagePackObject(true)]
public class AtgenItemSummonRateList
{
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }
    public int EntityQuantity { get; set; }
    public string EntityRate { get; set; }

    public AtgenItemSummonRateList(
        EntityTypes entityType,
        int entityId,
        int entityQuantity,
        string entityRate
    )
    {
        this.EntityType = entityType;
        this.EntityId = entityId;
        this.EntityQuantity = entityQuantity;
        this.EntityRate = entityRate;
    }

    public AtgenItemSummonRateList() { }
}

[MessagePackObject(true)]
public class AtgenLatest
{
    public int Episode { get; set; }

    public AtgenLatest(int episode)
    {
        this.Episode = episode;
    }

    public AtgenLatest() { }
}

[MessagePackObject(true)]
public class AtgenLoginBonusList
{
    public int RewardCode { get; set; }
    public int LoginBonusId { get; set; }
    public int TotalLoginDay { get; set; }
    public int RewardDay { get; set; }
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }
    public int EntityQuantity { get; set; }
    public int EntityLevel { get; set; }
    public int EntityLimitBreakCount { get; set; }

    public AtgenLoginBonusList(
        int rewardCode,
        int loginBonusId,
        int totalLoginDay,
        int rewardDay,
        EntityTypes entityType,
        int entityId,
        int entityQuantity,
        int entityLevel,
        int entityLimitBreakCount
    )
    {
        this.RewardCode = rewardCode;
        this.LoginBonusId = loginBonusId;
        this.TotalLoginDay = totalLoginDay;
        this.RewardDay = rewardDay;
        this.EntityType = entityType;
        this.EntityId = entityId;
        this.EntityQuantity = entityQuantity;
        this.EntityLevel = entityLevel;
        this.EntityLimitBreakCount = entityLimitBreakCount;
    }

    public AtgenLoginBonusList() { }
}

[MessagePackObject(true)]
public class AtgenLoginLotteryRewardList
{
    public int LoginLotteryId { get; set; }
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }
    public int EntityQuantity { get; set; }
    public int IsPickup { get; set; }
    public int IsGuaranteed { get; set; }

    public AtgenLoginLotteryRewardList(
        int loginLotteryId,
        EntityTypes entityType,
        int entityId,
        int entityQuantity,
        int isPickup,
        int isGuaranteed
    )
    {
        this.LoginLotteryId = loginLotteryId;
        this.EntityType = entityType;
        this.EntityId = entityId;
        this.EntityQuantity = entityQuantity;
        this.IsPickup = isPickup;
        this.IsGuaranteed = isGuaranteed;
    }

    public AtgenLoginLotteryRewardList() { }
}

[MessagePackObject(true)]
public class AtgenLostUnitList
{
    public int UnitNo { get; set; }
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }

    public AtgenLostUnitList(int unitNo, EntityTypes entityType, int entityId)
    {
        this.UnitNo = unitNo;
        this.EntityType = entityType;
        this.EntityId = entityId;
    }

    public AtgenLostUnitList() { }
}

[MessagePackObject(true)]
public class AtgenLotteryEntitySetList
{
    public int LotteryPrizeRank { get; set; }
    public string Rate { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> EntityList { get; set; }

    public AtgenLotteryEntitySetList(
        int lotteryPrizeRank,
        string rate,
        IEnumerable<AtgenBuildEventRewardEntityList> entityList
    )
    {
        this.LotteryPrizeRank = lotteryPrizeRank;
        this.Rate = rate;
        this.EntityList = entityList;
    }

    public AtgenLotteryEntitySetList() { }
}

[MessagePackObject(true)]
public class AtgenLotteryPrizeRankList
{
    public int LotteryPrizeRank { get; set; }
    public string TotalRate { get; set; }

    public AtgenLotteryPrizeRankList(int lotteryPrizeRank, string totalRate)
    {
        this.LotteryPrizeRank = lotteryPrizeRank;
        this.TotalRate = totalRate;
    }

    public AtgenLotteryPrizeRankList() { }
}

[MessagePackObject(true)]
public class AtgenLotteryResultList
{
    public int LotteryRank { get; set; }
    public int RankEntiryQuantity { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> EntityList { get; set; }

    public AtgenLotteryResultList(
        int lotteryRank,
        int rankEntiryQuantity,
        IEnumerable<AtgenBuildEventRewardEntityList> entityList
    )
    {
        this.LotteryRank = lotteryRank;
        this.RankEntiryQuantity = rankEntiryQuantity;
        this.EntityList = entityList;
    }

    public AtgenLotteryResultList() { }
}

[MessagePackObject(true)]
public class AtgenMainStoryMissionStateList
{
    public int MainStoryMissionId { get; set; }
    public int State { get; set; }

    public AtgenMainStoryMissionStateList(int mainStoryMissionId, int state)
    {
        this.MainStoryMissionId = mainStoryMissionId;
        this.State = state;
    }

    public AtgenMainStoryMissionStateList() { }
}

[MessagePackObject(true)]
public class AtgenMissionParamsList
{
    public int DailyMissionId { get; set; }

    [MessagePackFormatter(typeof(DayNoFormatter))]
    public DateOnly DayNo { get; set; }

    public AtgenMissionParamsList(int dailyMissionId, DateOnly dayNo)
    {
        this.DailyMissionId = dailyMissionId;
        this.DayNo = dayNo;
    }

    public AtgenMissionParamsList() { }
}

[MessagePackObject(true)]
public class AtgenMissionsClearSet
{
    public int Id { get; set; }
    public EntityTypes Type { get; set; }
    public int Quantity { get; set; }
    public int MissionNo { get; set; }

    public AtgenMissionsClearSet(int id, EntityTypes type, int quantity, int missionNo)
    {
        this.Id = id;
        this.Type = type;
        this.Quantity = quantity;
        this.MissionNo = missionNo;
    }

    public AtgenMissionsClearSet() { }
}

[MessagePackObject(true)]
public class AtgenMonthlyWallReceiveList
{
    public int QuestGroupId { get; set; }
    public RewardStatus IsReceiveReward { get; set; }

    public AtgenMonthlyWallReceiveList(int questGroupId, RewardStatus isReceiveReward)
    {
        this.QuestGroupId = questGroupId;
        this.IsReceiveReward = isReceiveReward;
    }

    public AtgenMonthlyWallReceiveList() { }
}

[MessagePackObject(true)]
public class AtgenMultiServer
{
    public string Host { get; set; }
    public string AppId { get; set; }

    public AtgenMultiServer(string host, string appId)
    {
        this.Host = host;
        this.AppId = appId;
    }

    public AtgenMultiServer() { }
}

[MessagePackObject(true)]
public class AtgenNAccountInfo
{
    public string Email { get; set; }
    public string Nickname { get; set; }

    public AtgenNAccountInfo(string email, string nickname)
    {
        this.Email = email;
        this.Nickname = nickname;
    }

    public AtgenNAccountInfo() { }
}

[MessagePackObject(true)]
public class AtgenNeedTradeEntityList
{
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }
    public int EntityQuantity { get; set; }
    public int LimitBreakCount { get; set; }

    public AtgenNeedTradeEntityList(
        EntityTypes entityType,
        int entityId,
        int entityQuantity,
        int limitBreakCount
    )
    {
        this.EntityType = entityType;
        this.EntityId = entityId;
        this.EntityQuantity = entityQuantity;
        this.LimitBreakCount = limitBreakCount;
    }

    public AtgenNeedTradeEntityList() { }
}

[MessagePackObject(true)]
public class AtgenNeedUnitList
{
    public EntityTypes Type { get; set; }
    public ulong KeyId { get; set; }

    public AtgenNeedUnitList(EntityTypes type, ulong keyId)
    {
        this.Type = type;
        this.KeyId = keyId;
    }

    public AtgenNeedUnitList() { }
}

[MessagePackObject(true)]
public class AtgenNormalMissionNotice
{
    public int IsUpdate { get; set; }
    public int AllMissionCount { get; set; }
    public int CompletedMissionCount { get; set; }
    public int ReceivableRewardCount { get; set; }
    public int PickupMissionCount { get; set; }
    public int CurrentMissionId { get; set; }
    public IEnumerable<int> NewCompleteMissionIdList { get; set; }

    public AtgenNormalMissionNotice(
        int isUpdate,
        int allMissionCount,
        int completedMissionCount,
        int receivableRewardCount,
        int pickupMissionCount,
        int currentMissionId,
        IEnumerable<int> newCompleteMissionIdList
    )
    {
        this.IsUpdate = isUpdate;
        this.AllMissionCount = allMissionCount;
        this.CompletedMissionCount = completedMissionCount;
        this.ReceivableRewardCount = receivableRewardCount;
        this.PickupMissionCount = pickupMissionCount;
        this.CurrentMissionId = currentMissionId;
        this.NewCompleteMissionIdList = newCompleteMissionIdList;
    }

    public AtgenNormalMissionNotice() { }
}

[MessagePackObject(true)]
public class AtgenNotReceivedMissionIdListWithDayNo
{
    public int DayNo { get; set; }
    public IEnumerable<int> NotReceivedMissionIdList { get; set; }

    public AtgenNotReceivedMissionIdListWithDayNo(
        int dayNo,
        IEnumerable<int> notReceivedMissionIdList
    )
    {
        this.DayNo = dayNo;
        this.NotReceivedMissionIdList = notReceivedMissionIdList;
    }

    public AtgenNotReceivedMissionIdListWithDayNo() { }
}

[MessagePackObject(true)]
public class AtgenOpinionList
{
    public string OpinionId { get; set; }
    public string OpinionText { get; set; }
    public int CreatedAt { get; set; }
    public int UpdatedAt { get; set; }

    public AtgenOpinionList(string opinionId, string opinionText, int createdAt, int updatedAt)
    {
        this.OpinionId = opinionId;
        this.OpinionText = opinionText;
        this.CreatedAt = createdAt;
        this.UpdatedAt = updatedAt;
    }

    public AtgenOpinionList() { }
}

[MessagePackObject(true)]
public class AtgenOpinionTypeList
{
    public int OpinionType { get; set; }
    public string Name { get; set; }

    public AtgenOpinionTypeList(int opinionType, string name)
    {
        this.OpinionType = opinionType;
        this.Name = name;
    }

    public AtgenOpinionTypeList() { }
}

[MessagePackObject(true)]
public class AtgenOption
{
    public int StrengthParamId { get; set; }
    public int StrengthAbilityId { get; set; }
    public int StrengthSkillId { get; set; }
    public int AbnormalStatusInvalidCount { get; set; }

    public AtgenOption(
        int strengthParamId,
        int strengthAbilityId,
        int strengthSkillId,
        int abnormalStatusInvalidCount
    )
    {
        this.StrengthParamId = strengthParamId;
        this.StrengthAbilityId = strengthAbilityId;
        this.StrengthSkillId = strengthSkillId;
        this.AbnormalStatusInvalidCount = abnormalStatusInvalidCount;
    }

    public AtgenOption() { }
}

[MessagePackObject(true)]
public class AtgenOwnDamageRankingList
{
    public int Rank { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsNew { get; set; }
    public int CharaId { get; set; }
    public long DamageValue { get; set; }

    public AtgenOwnDamageRankingList(int rank, bool isNew, int charaId, long damageValue)
    {
        this.Rank = rank;
        this.IsNew = isNew;
        this.CharaId = charaId;
        this.DamageValue = damageValue;
    }

    public AtgenOwnDamageRankingList() { }
}

[MessagePackObject(true)]
public class AtgenOwnRankingList
{
    public int Rank { get; set; }
    public int IsNew { get; set; }
    public int RankingId { get; set; }
    public ulong ViewerId { get; set; }
    public int QuestId { get; set; }
    public float ClearTime { get; set; }
    public int StartTime { get; set; }
    public int EndTime { get; set; }
    public string PartyHash { get; set; }
    public ulong ViewerId1 { get; set; }
    public ulong ViewerId2 { get; set; }
    public ulong ViewerId3 { get; set; }
    public ulong ViewerId4 { get; set; }
    public int CharaId1 { get; set; }
    public int CharaId2 { get; set; }
    public int CharaId3 { get; set; }
    public int CharaId4 { get; set; }
    public int CharaRarity1 { get; set; }
    public int CharaRarity2 { get; set; }
    public int CharaRarity3 { get; set; }
    public int CharaRarity4 { get; set; }
    public int CharaLevel1 { get; set; }
    public int CharaLevel2 { get; set; }
    public int CharaLevel3 { get; set; }
    public int CharaLevel4 { get; set; }

    public AtgenOwnRankingList(
        int rank,
        int isNew,
        int rankingId,
        ulong viewerId,
        int questId,
        float clearTime,
        int startTime,
        int endTime,
        string partyHash,
        ulong viewerId1,
        ulong viewerId2,
        ulong viewerId3,
        ulong viewerId4,
        int charaId1,
        int charaId2,
        int charaId3,
        int charaId4,
        int charaRarity1,
        int charaRarity2,
        int charaRarity3,
        int charaRarity4,
        int charaLevel1,
        int charaLevel2,
        int charaLevel3,
        int charaLevel4
    )
    {
        this.Rank = rank;
        this.IsNew = isNew;
        this.RankingId = rankingId;
        this.ViewerId = viewerId;
        this.QuestId = questId;
        this.ClearTime = clearTime;
        this.StartTime = startTime;
        this.EndTime = endTime;
        this.PartyHash = partyHash;
        this.ViewerId1 = viewerId1;
        this.ViewerId2 = viewerId2;
        this.ViewerId3 = viewerId3;
        this.ViewerId4 = viewerId4;
        this.CharaId1 = charaId1;
        this.CharaId2 = charaId2;
        this.CharaId3 = charaId3;
        this.CharaId4 = charaId4;
        this.CharaRarity1 = charaRarity1;
        this.CharaRarity2 = charaRarity2;
        this.CharaRarity3 = charaRarity3;
        this.CharaRarity4 = charaRarity4;
        this.CharaLevel1 = charaLevel1;
        this.CharaLevel2 = charaLevel2;
        this.CharaLevel3 = charaLevel3;
        this.CharaLevel4 = charaLevel4;
    }

    public AtgenOwnRankingList() { }
}

[MessagePackObject(true)]
public class AtgenPaid
{
    public string Code { get; set; }
    public int Total { get; set; }

    public AtgenPaid(string code, int total)
    {
        this.Code = code;
        this.Total = total;
    }

    public AtgenPaid() { }
}

[MessagePackObject(true)]
public class AtgenParamBonus
{
    public WeaponTypes WeaponType { get; set; }
    public float Hp { get; set; }
    public float Attack { get; set; }

    public AtgenParamBonus(WeaponTypes weaponType, float hp, float attack)
    {
        this.WeaponType = weaponType;
        this.Hp = hp;
        this.Attack = attack;
    }

    public AtgenParamBonus() { }
}

[MessagePackObject(true)]
public class AtgenPenaltyData
{
    public int ReportId { get; set; }
    public int Point { get; set; }
    public int PenaltyType { get; set; }
    public int PenaltyTextType { get; set; }
    public string PenaltyBody { get; set; }

    public AtgenPenaltyData(
        int reportId,
        int point,
        int penaltyType,
        int penaltyTextType,
        string penaltyBody
    )
    {
        this.ReportId = reportId;
        this.Point = point;
        this.PenaltyType = penaltyType;
        this.PenaltyTextType = penaltyTextType;
        this.PenaltyBody = penaltyBody;
    }

    public AtgenPenaltyData() { }
}

[MessagePackObject(true)]
public class AtgenPlayWallDetail
{
    public int WallId { get; set; }
    public int AfterWallLevel { get; set; }
    public int BeforeWallLevel { get; set; }

    public AtgenPlayWallDetail(int wallId, int afterWallLevel, int beforeWallLevel)
    {
        this.WallId = wallId;
        this.AfterWallLevel = afterWallLevel;
        this.BeforeWallLevel = beforeWallLevel;
    }

    public AtgenPlayWallDetail() { }
}

[MessagePackObject(true)]
public class AtgenPlusCountParamsList
{
    public PlusCountType PlusCountType { get; set; }
    public int PlusCount { get; set; }

    public AtgenPlusCountParamsList(PlusCountType plusCountType, int plusCount)
    {
        this.PlusCountType = plusCountType;
        this.PlusCount = plusCount;
    }

    public AtgenPlusCountParamsList() { }
}

[MessagePackObject(true)]
public class AtgenProductionRp
{
    public float Speed { get; set; }
    public int Max { get; set; }

    public AtgenProductionRp(float speed, int max)
    {
        this.Speed = speed;
        this.Max = max;
    }

    public AtgenProductionRp() { }
}

[MessagePackObject(true)]
public class AtgenProductLockList
{
    public int ShopType { get; set; }
    public int GoodsId { get; set; }
    public int IsLock { get; set; }
    public int ExpireTime { get; set; }

    public AtgenProductLockList(int shopType, int goodsId, int isLock, int expireTime)
    {
        this.ShopType = shopType;
        this.GoodsId = goodsId;
        this.IsLock = isLock;
        this.ExpireTime = expireTime;
    }

    public AtgenProductLockList() { }
}

[MessagePackObject(true)]
public class AtgenQuestBonus
{
    public int GoodsId { get; set; }
    public int EffectStartTime { get; set; }
    public int EffectEndTime { get; set; }

    public AtgenQuestBonus(int goodsId, int effectStartTime, int effectEndTime)
    {
        this.GoodsId = goodsId;
        this.EffectStartTime = effectStartTime;
        this.EffectEndTime = effectEndTime;
    }

    public AtgenQuestBonus() { }
}

[MessagePackObject(true)]
public class AtgenQuestDropInfo
{
    public IEnumerable<AtgenDuplicateEntityList> DropInfoList { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> HostDropInfoList { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> FeverDropInfoList { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> QuestBonusInfoList { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> CampaignExtraRewardInfoList { get; set; }
    public IEnumerable<AtgenQuestRebornBonusInfoList> QuestRebornBonusInfoList { get; set; }

    public AtgenQuestDropInfo(
        IEnumerable<AtgenDuplicateEntityList> dropInfoList,
        IEnumerable<AtgenDuplicateEntityList> hostDropInfoList,
        IEnumerable<AtgenDuplicateEntityList> feverDropInfoList,
        IEnumerable<AtgenDuplicateEntityList> questBonusInfoList,
        IEnumerable<AtgenDuplicateEntityList> campaignExtraRewardInfoList,
        IEnumerable<AtgenQuestRebornBonusInfoList> questRebornBonusInfoList
    )
    {
        this.DropInfoList = dropInfoList;
        this.HostDropInfoList = hostDropInfoList;
        this.FeverDropInfoList = feverDropInfoList;
        this.QuestBonusInfoList = questBonusInfoList;
        this.CampaignExtraRewardInfoList = campaignExtraRewardInfoList;
        this.QuestRebornBonusInfoList = questRebornBonusInfoList;
    }

    public AtgenQuestDropInfo() { }
}

[MessagePackObject(true)]
public class AtgenQuestRebornBonusInfoList
{
    public int RebornCount { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> BonusInfoList { get; set; }

    public AtgenQuestRebornBonusInfoList(
        int rebornCount,
        IEnumerable<AtgenDuplicateEntityList> bonusInfoList
    )
    {
        this.RebornCount = rebornCount;
        this.BonusInfoList = bonusInfoList;
    }

    public AtgenQuestRebornBonusInfoList() { }
}

[MessagePackObject(true)]
public class AtgenQuestStoryRewardList
{
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }
    public int EntityQuantity { get; set; }
    public int EntityLevel { get; set; }
    public int EntityLimitBreakCount { get; set; }

    public AtgenQuestStoryRewardList(
        EntityTypes entityType,
        int entityId,
        int entityQuantity,
        int entityLevel,
        int entityLimitBreakCount
    )
    {
        this.EntityType = entityType;
        this.EntityId = entityId;
        this.EntityQuantity = entityQuantity;
        this.EntityLevel = entityLevel;
        this.EntityLimitBreakCount = entityLimitBreakCount;
    }

    public AtgenQuestStoryRewardList() { }
}

[MessagePackObject(true)]
public class AtgenRarityGroupList
{
    public bool Pickup { get; set; }
    public int Rarity { get; set; }
    public string TotalRate { get; set; }
    public string CharaRate { get; set; }
    public string DragonRate { get; set; }
    public string AmuletRate { get; set; }

    public AtgenRarityGroupList(
        bool pickup,
        int rarity,
        string totalRate,
        string charaRate,
        string dragonRate,
        string amuletRate
    )
    {
        this.Pickup = pickup;
        this.Rarity = rarity;
        this.TotalRate = totalRate;
        this.CharaRate = charaRate;
        this.DragonRate = dragonRate;
        this.AmuletRate = amuletRate;
    }

    public AtgenRarityGroupList() { }
}

[MessagePackObject(true)]
public class AtgenRarityList
{
    public int Rarity { get; set; }
    public string TotalRate { get; set; }

    public AtgenRarityList(int rarity, string totalRate)
    {
        this.Rarity = rarity;
        this.TotalRate = totalRate;
    }

    public AtgenRarityList() { }
}

[MessagePackObject(true)]
public class AtgenReceiveQuestBonus
{
    public int TargetQuestId { get; set; }
    public int ReceiveBonusCount { get; set; }
    public float BonusFactor { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> QuestBonusEntityList { get; set; }

    public AtgenReceiveQuestBonus(
        int targetQuestId,
        int receiveBonusCount,
        float bonusFactor,
        IEnumerable<AtgenBuildEventRewardEntityList> questBonusEntityList
    )
    {
        this.TargetQuestId = targetQuestId;
        this.ReceiveBonusCount = receiveBonusCount;
        this.BonusFactor = bonusFactor;
        this.QuestBonusEntityList = questBonusEntityList;
    }

    public AtgenReceiveQuestBonus() { }
}

[MessagePackObject(true)]
public class AtgenRecoverData
{
    public UseItemEffect RecoverStaminaType { get; set; }
    public int RecoverStaminaPoint { get; set; }

    public AtgenRecoverData(UseItemEffect recoverStaminaType, int recoverStaminaPoint)
    {
        this.RecoverStaminaType = recoverStaminaType;
        this.RecoverStaminaPoint = recoverStaminaPoint;
    }

    public AtgenRecoverData() { }
}

[MessagePackObject(true)]
public class AtgenRedoableSummonResultUnitList
{
    public EntityTypes EntityType { get; set; }
    public int Id { get; set; }
    public int Rarity { get; set; }

    public AtgenRedoableSummonResultUnitList(EntityTypes entityType, int id, int rarity)
    {
        this.EntityType = entityType;
        this.Id = id;
        this.Rarity = rarity;
    }

    public AtgenRedoableSummonResultUnitList() { }
}

[MessagePackObject(true)]
public class AtgenRequestAbilityCrestSetData
{
    public AbilityCrests CrestSlotType1CrestId1 { get; set; }
    public AbilityCrests CrestSlotType1CrestId2 { get; set; }
    public AbilityCrests CrestSlotType1CrestId3 { get; set; }
    public AbilityCrests CrestSlotType2CrestId1 { get; set; }
    public AbilityCrests CrestSlotType2CrestId2 { get; set; }
    public AbilityCrests CrestSlotType3CrestId1 { get; set; }
    public AbilityCrests CrestSlotType3CrestId2 { get; set; }
    public long TalismanKeyId { get; set; }

    public AtgenRequestAbilityCrestSetData(
        AbilityCrests crestSlotType1CrestId1,
        AbilityCrests crestSlotType1CrestId2,
        AbilityCrests crestSlotType1CrestId3,
        AbilityCrests crestSlotType2CrestId1,
        AbilityCrests crestSlotType2CrestId2,
        AbilityCrests crestSlotType3CrestId1,
        AbilityCrests crestSlotType3CrestId2,
        long talismanKeyId
    )
    {
        this.CrestSlotType1CrestId1 = crestSlotType1CrestId1;
        this.CrestSlotType1CrestId2 = crestSlotType1CrestId2;
        this.CrestSlotType1CrestId3 = crestSlotType1CrestId3;
        this.CrestSlotType2CrestId1 = crestSlotType2CrestId1;
        this.CrestSlotType2CrestId2 = crestSlotType2CrestId2;
        this.CrestSlotType3CrestId1 = crestSlotType3CrestId1;
        this.CrestSlotType3CrestId2 = crestSlotType3CrestId2;
        this.TalismanKeyId = talismanKeyId;
    }

    public AtgenRequestAbilityCrestSetData() { }
}

[MessagePackObject(true)]
public class AtgenRequestCharaUnitSetData
{
    public long DragonKeyId { get; set; }
    public WeaponBodies WeaponBodyId { get; set; }
    public AbilityCrests CrestSlotType1CrestId1 { get; set; }
    public AbilityCrests CrestSlotType1CrestId2 { get; set; }
    public AbilityCrests CrestSlotType1CrestId3 { get; set; }
    public AbilityCrests CrestSlotType2CrestId1 { get; set; }
    public AbilityCrests CrestSlotType2CrestId2 { get; set; }
    public AbilityCrests CrestSlotType3CrestId1 { get; set; }
    public AbilityCrests CrestSlotType3CrestId2 { get; set; }
    public long TalismanKeyId { get; set; }

    public AtgenRequestCharaUnitSetData(
        long dragonKeyId,
        WeaponBodies weaponBodyId,
        AbilityCrests crestSlotType1CrestId1,
        AbilityCrests crestSlotType1CrestId2,
        AbilityCrests crestSlotType1CrestId3,
        AbilityCrests crestSlotType2CrestId1,
        AbilityCrests crestSlotType2CrestId2,
        AbilityCrests crestSlotType3CrestId1,
        AbilityCrests crestSlotType3CrestId2,
        long talismanKeyId
    )
    {
        this.DragonKeyId = dragonKeyId;
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

    public AtgenRequestCharaUnitSetData() { }
}

[MessagePackObject(true)]
public class AtgenRequestQuestMultipleList
{
    public int QuestId { get; set; }
    public int PlayCount { get; set; }
    public int BetCount { get; set; }

    public AtgenRequestQuestMultipleList(int questId, int playCount, int betCount)
    {
        this.QuestId = questId;
        this.PlayCount = playCount;
        this.BetCount = betCount;
    }

    public AtgenRequestQuestMultipleList() { }
}

[MessagePackObject(true)]
public class AtgenResultPrizeList
{
    public int SummonPrizeRank { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> EntityList { get; set; }

    public AtgenResultPrizeList(
        int summonPrizeRank,
        IEnumerable<AtgenBuildEventRewardEntityList> entityList
    )
    {
        this.SummonPrizeRank = summonPrizeRank;
        this.EntityList = entityList;
    }

    public AtgenResultPrizeList() { }
}

[MessagePackObject(true)]
public class AtgenResultUnitList
{
    public EntityTypes EntityType { get; set; }
    public int Id { get; set; }
    public int Rarity { get; set; }
    public bool IsNew { get; set; }
    public int PrevRarity { get; set; }
    public int DewPoint { get; set; }

    public AtgenResultUnitList(
        EntityTypes entityType,
        int id,
        int rarity,
        bool isNew,
        int prevRarity,
        int dewPoint
    )
    {
        this.EntityType = entityType;
        this.Id = id;
        this.Rarity = rarity;
        this.IsNew = isNew;
        this.PrevRarity = prevRarity;
        this.DewPoint = dewPoint;
    }

    public AtgenResultUnitList() { }
}

[MessagePackObject(true)]
public class AtgenRewardTalismanList
{
    public Talismans TalismanId { get; set; }
    public int TalismanAbilityId1 { get; set; }
    public int TalismanAbilityId2 { get; set; }
    public int TalismanAbilityId3 { get; set; }
    public int AdditionalHp { get; set; }
    public int AdditionalAttack { get; set; }

    public AtgenRewardTalismanList(
        Talismans talismanId,
        int talismanAbilityId1,
        int talismanAbilityId2,
        int talismanAbilityId3,
        int additionalHp,
        int additionalAttack
    )
    {
        this.TalismanId = talismanId;
        this.TalismanAbilityId1 = talismanAbilityId1;
        this.TalismanAbilityId2 = talismanAbilityId2;
        this.TalismanAbilityId3 = talismanAbilityId3;
        this.AdditionalHp = additionalHp;
        this.AdditionalAttack = additionalAttack;
    }

    public AtgenRewardTalismanList() { }
}

[MessagePackObject(true)]
public class AtgenRoomMemberList
{
    public ulong ViewerId { get; set; }

    public AtgenRoomMemberList(ulong viewerId)
    {
        this.ViewerId = viewerId;
    }

    public AtgenRoomMemberList() { }
}

[MessagePackObject(true)]
public class AtgenScoreMissionSuccessList
{
    public QuestCompleteType ScoreMissionCompleteType { get; set; }
    public int ScoreTargetValue { get; set; }
    public float CorrectionValue { get; set; }

    public AtgenScoreMissionSuccessList(
        QuestCompleteType scoreMissionCompleteType,
        int scoreTargetValue,
        float correctionValue
    )
    {
        this.ScoreMissionCompleteType = scoreMissionCompleteType;
        this.ScoreTargetValue = scoreTargetValue;
        this.CorrectionValue = correctionValue;
    }

    public AtgenScoreMissionSuccessList() { }
}

[MessagePackObject(true)]
public class AtgenScoringEnemyPointList
{
    public int ScoringEnemyId { get; set; }
    public int SmashCount { get; set; }
    public int Point { get; set; }

    public AtgenScoringEnemyPointList(int scoringEnemyId, int smashCount, int point)
    {
        this.ScoringEnemyId = scoringEnemyId;
        this.SmashCount = smashCount;
        this.Point = point;
    }

    public AtgenScoringEnemyPointList() { }
}

[MessagePackObject(true)]
public class AtgenShopGiftList
{
    public int DragonGiftId { get; set; }
    public int Price { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsBuy { get; set; }

    public AtgenShopGiftList(int dragonGiftId, int price, bool isBuy)
    {
        this.DragonGiftId = dragonGiftId;
        this.Price = price;
        this.IsBuy = isBuy;
    }

    public AtgenShopGiftList() { }
}

[MessagePackObject(true)]
public class AtgenStaminaBonus
{
    public int GoodsId { get; set; }
    public int LastBonusTime { get; set; }
    public int EffectStartTime { get; set; }
    public int EffectEndTime { get; set; }

    public AtgenStaminaBonus(int goodsId, int lastBonusTime, int effectStartTime, int effectEndTime)
    {
        this.GoodsId = goodsId;
        this.LastBonusTime = lastBonusTime;
        this.EffectStartTime = effectStartTime;
        this.EffectEndTime = effectEndTime;
    }

    public AtgenStaminaBonus() { }
}

[MessagePackObject(true)]
public class AtgenStoneBonus
{
    public int GoodsId { get; set; }
    public int BonusCount { get; set; }
    public int LastBonusTime { get; set; }

    public AtgenStoneBonus(int goodsId, int bonusCount, int lastBonusTime)
    {
        this.GoodsId = goodsId;
        this.BonusCount = bonusCount;
        this.LastBonusTime = lastBonusTime;
    }

    public AtgenStoneBonus() { }
}

[MessagePackObject(true)]
public class AtgenSummonPointTradeList
{
    public int TradeId { get; set; }
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }

    public AtgenSummonPointTradeList(int tradeId, EntityTypes entityType, int entityId)
    {
        this.TradeId = tradeId;
        this.EntityType = entityType;
        this.EntityId = entityId;
    }

    public AtgenSummonPointTradeList() { }
}

[MessagePackObject(true)]
public class AtgenSummonPrizeEntitySetList
{
    public int SummonPrizeRank { get; set; }
    public string Rate { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> EntityList { get; set; }

    public AtgenSummonPrizeEntitySetList(
        int summonPrizeRank,
        string rate,
        IEnumerable<AtgenBuildEventRewardEntityList> entityList
    )
    {
        this.SummonPrizeRank = summonPrizeRank;
        this.Rate = rate;
        this.EntityList = entityList;
    }

    public AtgenSummonPrizeEntitySetList() { }
}

[MessagePackObject(true)]
public class AtgenSummonPrizeRankList
{
    public int SummonPrizeRank { get; set; }
    public string TotalRate { get; set; }

    public AtgenSummonPrizeRankList(int summonPrizeRank, string totalRate)
    {
        this.SummonPrizeRank = summonPrizeRank;
        this.TotalRate = totalRate;
    }

    public AtgenSummonPrizeRankList() { }
}

[MessagePackObject(true)]
public class AtgenSupportAmulet
{
    public ulong AmuletKeyId { get; set; }
    public int AmuletId { get; set; }
    public int Level { get; set; }
    public int Attack { get; set; }
    public int Ability1Level { get; set; }
    public int Ability2Level { get; set; }
    public int Ability3Level { get; set; }
    public int HpPlusCount { get; set; }
    public int AttackPlusCount { get; set; }
    public int StatusPlusCount { get; set; }
    public int LimitBreakCount { get; set; }

    public AtgenSupportAmulet(
        ulong amuletKeyId,
        int amuletId,
        int level,
        int attack,
        int ability1Level,
        int ability2Level,
        int ability3Level,
        int hpPlusCount,
        int attackPlusCount,
        int statusPlusCount,
        int limitBreakCount
    )
    {
        this.AmuletKeyId = amuletKeyId;
        this.AmuletId = amuletId;
        this.Level = level;
        this.Attack = attack;
        this.Ability1Level = ability1Level;
        this.Ability2Level = ability2Level;
        this.Ability3Level = ability3Level;
        this.HpPlusCount = hpPlusCount;
        this.AttackPlusCount = attackPlusCount;
        this.StatusPlusCount = statusPlusCount;
        this.LimitBreakCount = limitBreakCount;
    }

    public AtgenSupportAmulet() { }
}

[MessagePackObject(true)]
public class AtgenSupportChara
{
    public Charas CharaId { get; set; }
    public int Level { get; set; }
    public int AdditionalMaxLevel { get; set; }
    public int Rarity { get; set; }
    public int Hp { get; set; }
    public int Attack { get; set; }
    public int HpPlusCount { get; set; }
    public int AttackPlusCount { get; set; }
    public int StatusPlusCount { get; set; }
    public int Ability1Level { get; set; }
    public int Ability2Level { get; set; }
    public int Ability3Level { get; set; }
    public int ExAbilityLevel { get; set; }
    public int ExAbility2Level { get; set; }
    public int Skill1Level { get; set; }
    public int Skill2Level { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsUnlockEditSkill { get; set; }

    public AtgenSupportChara(
        Charas charaId,
        int level,
        int additionalMaxLevel,
        int rarity,
        int hp,
        int attack,
        int hpPlusCount,
        int attackPlusCount,
        int statusPlusCount,
        int ability1Level,
        int ability2Level,
        int ability3Level,
        int exAbilityLevel,
        int exAbility2Level,
        int skill1Level,
        int skill2Level,
        bool isUnlockEditSkill
    )
    {
        this.CharaId = charaId;
        this.Level = level;
        this.AdditionalMaxLevel = additionalMaxLevel;
        this.Rarity = rarity;
        this.Hp = hp;
        this.Attack = attack;
        this.HpPlusCount = hpPlusCount;
        this.AttackPlusCount = attackPlusCount;
        this.StatusPlusCount = statusPlusCount;
        this.Ability1Level = ability1Level;
        this.Ability2Level = ability2Level;
        this.Ability3Level = ability3Level;
        this.ExAbilityLevel = exAbilityLevel;
        this.ExAbility2Level = exAbility2Level;
        this.Skill1Level = skill1Level;
        this.Skill2Level = skill2Level;
        this.IsUnlockEditSkill = isUnlockEditSkill;
    }

    public AtgenSupportChara() { }
}

[MessagePackObject(true)]
public class AtgenSupportCrestSlotType1List
{
    public AbilityCrests AbilityCrestId { get; set; }
    public int BuildupCount { get; set; }
    public int LimitBreakCount { get; set; }
    public int HpPlusCount { get; set; }
    public int AttackPlusCount { get; set; }
    public int EquipableCount { get; set; }

    public AtgenSupportCrestSlotType1List(
        AbilityCrests abilityCrestId,
        int buildupCount,
        int limitBreakCount,
        int hpPlusCount,
        int attackPlusCount,
        int equipableCount
    )
    {
        this.AbilityCrestId = abilityCrestId;
        this.BuildupCount = buildupCount;
        this.LimitBreakCount = limitBreakCount;
        this.HpPlusCount = hpPlusCount;
        this.AttackPlusCount = attackPlusCount;
        this.EquipableCount = equipableCount;
    }

    public AtgenSupportCrestSlotType1List() { }
}

[MessagePackObject(true)]
public class AtgenSupportData
{
    public ulong ViewerId { get; set; }
    public string Name { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsFriend { get; set; }
    public CharaList CharaData { get; set; }
    public DragonList DragonData { get; set; }
    public WeaponList WeaponData { get; set; }
    public AmuletList AmuletData { get; set; }
    public AmuletList Amulet2Data { get; set; }
    public GameWeaponSkin WeaponSkinData { get; set; }
    public GameWeaponBody WeaponBodyData { get; set; }
    public IEnumerable<GameAbilityCrest> CrestSlotType1CrestList { get; set; }
    public IEnumerable<GameAbilityCrest> CrestSlotType2CrestList { get; set; }
    public IEnumerable<GameAbilityCrest> CrestSlotType3CrestList { get; set; }
    public TalismanList TalismanData { get; set; }
    public IEnumerable<WeaponPassiveAbilityList> GameWeaponPassiveAbilityList { get; set; }

    public AtgenSupportData(
        ulong viewerId,
        string name,
        bool isFriend,
        CharaList charaData,
        DragonList dragonData,
        WeaponList weaponData,
        AmuletList amuletData,
        AmuletList amulet2Data,
        GameWeaponSkin weaponSkinData,
        GameWeaponBody weaponBodyData,
        IEnumerable<GameAbilityCrest> crestSlotType1CrestList,
        IEnumerable<GameAbilityCrest> crestSlotType2CrestList,
        IEnumerable<GameAbilityCrest> crestSlotType3CrestList,
        TalismanList talismanData,
        IEnumerable<WeaponPassiveAbilityList> gameWeaponPassiveAbilityList
    )
    {
        this.ViewerId = viewerId;
        this.Name = name;
        this.IsFriend = isFriend;
        this.CharaData = charaData;
        this.DragonData = dragonData;
        this.WeaponData = weaponData;
        this.AmuletData = amuletData;
        this.Amulet2Data = amulet2Data;
        this.WeaponSkinData = weaponSkinData;
        this.WeaponBodyData = weaponBodyData;
        this.CrestSlotType1CrestList = crestSlotType1CrestList;
        this.CrestSlotType2CrestList = crestSlotType2CrestList;
        this.CrestSlotType3CrestList = crestSlotType3CrestList;
        this.TalismanData = talismanData;
        this.GameWeaponPassiveAbilityList = gameWeaponPassiveAbilityList;
    }

    public AtgenSupportData() { }
}

[MessagePackObject(true)]
public class AtgenSupportDragon
{
    public ulong DragonKeyId { get; set; }
    public Dragons DragonId { get; set; }
    public int Level { get; set; }
    public int Hp { get; set; }
    public int Attack { get; set; }
    public int Skill1Level { get; set; }
    public int Ability1Level { get; set; }
    public int Ability2Level { get; set; }
    public int HpPlusCount { get; set; }
    public int AttackPlusCount { get; set; }
    public int StatusPlusCount { get; set; }
    public int LimitBreakCount { get; set; }

    public AtgenSupportDragon(
        ulong dragonKeyId,
        Dragons dragonId,
        int level,
        int hp,
        int attack,
        int skill1Level,
        int ability1Level,
        int ability2Level,
        int hpPlusCount,
        int attackPlusCount,
        int statusPlusCount,
        int limitBreakCount
    )
    {
        this.DragonKeyId = dragonKeyId;
        this.DragonId = dragonId;
        this.Level = level;
        this.Hp = hp;
        this.Attack = attack;
        this.Skill1Level = skill1Level;
        this.Ability1Level = ability1Level;
        this.Ability2Level = ability2Level;
        this.HpPlusCount = hpPlusCount;
        this.AttackPlusCount = attackPlusCount;
        this.StatusPlusCount = statusPlusCount;
        this.LimitBreakCount = limitBreakCount;
    }

    public AtgenSupportDragon() { }
}

[MessagePackObject(true)]
public class AtgenSupportReward
{
    public int ServeCount { get; set; }
    public int ManaPoint { get; set; }

    public AtgenSupportReward(int serveCount, int manaPoint)
    {
        this.ServeCount = serveCount;
        this.ManaPoint = manaPoint;
    }

    public AtgenSupportReward() { }
}

[MessagePackObject(true)]
public class AtgenSupportTalisman
{
    public ulong TalismanKeyId { get; set; }
    public Talismans TalismanId { get; set; }
    public int TalismanAbilityId1 { get; set; }
    public int TalismanAbilityId2 { get; set; }
    public int TalismanAbilityId3 { get; set; }
    public int AdditionalHp { get; set; }
    public int AdditionalAttack { get; set; }

    public AtgenSupportTalisman(
        ulong talismanKeyId,
        Talismans talismanId,
        int talismanAbilityId1,
        int talismanAbilityId2,
        int talismanAbilityId3,
        int additionalHp,
        int additionalAttack
    )
    {
        this.TalismanKeyId = talismanKeyId;
        this.TalismanId = talismanId;
        this.TalismanAbilityId1 = talismanAbilityId1;
        this.TalismanAbilityId2 = talismanAbilityId2;
        this.TalismanAbilityId3 = talismanAbilityId3;
        this.AdditionalHp = additionalHp;
        this.AdditionalAttack = additionalAttack;
    }

    public AtgenSupportTalisman() { }
}

[MessagePackObject(true)]
public class AtgenSupportUserDataDetail
{
    public UserSupportList UserSupportData { get; set; }
    public FortBonusList FortBonusList { get; set; }
    public IEnumerable<int> ManaCirclePieceIdList { get; set; }
    public int DragonReliabilityLevel { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsFriend { get; set; }
    public int ApplySendStatus { get; set; }

    public AtgenSupportUserDataDetail(
        UserSupportList userSupportData,
        FortBonusList fortBonusList,
        IEnumerable<int> manaCirclePieceIdList,
        int dragonReliabilityLevel,
        bool isFriend,
        int applySendStatus
    )
    {
        this.UserSupportData = userSupportData;
        this.FortBonusList = fortBonusList;
        this.ManaCirclePieceIdList = manaCirclePieceIdList;
        this.DragonReliabilityLevel = dragonReliabilityLevel;
        this.IsFriend = isFriend;
        this.ApplySendStatus = applySendStatus;
    }

    public AtgenSupportUserDataDetail() { }
}

[MessagePackObject(true)]
public class AtgenSupportUserDetailList
{
    public ulong ViewerId { get; set; }
    public int GettableManaPoint { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsFriend { get; set; }

    public AtgenSupportUserDetailList(ulong viewerId, int gettableManaPoint, bool isFriend)
    {
        this.ViewerId = viewerId;
        this.GettableManaPoint = gettableManaPoint;
        this.IsFriend = isFriend;
    }

    public AtgenSupportUserDetailList() { }
}

[MessagePackObject(true)]
public class AtgenSupportWeapon
{
    public ulong WeaponKeyId { get; set; }
    public int WeaponId { get; set; }
    public int Level { get; set; }
    public int Attack { get; set; }
    public int SkillLevel { get; set; }
    public int HpPlusCount { get; set; }
    public int AttackPlusCount { get; set; }
    public int StatusPlusCount { get; set; }
    public int LimitBreakCount { get; set; }

    public AtgenSupportWeapon(
        ulong weaponKeyId,
        int weaponId,
        int level,
        int attack,
        int skillLevel,
        int hpPlusCount,
        int attackPlusCount,
        int statusPlusCount,
        int limitBreakCount
    )
    {
        this.WeaponKeyId = weaponKeyId;
        this.WeaponId = weaponId;
        this.Level = level;
        this.Attack = attack;
        this.SkillLevel = skillLevel;
        this.HpPlusCount = hpPlusCount;
        this.AttackPlusCount = attackPlusCount;
        this.StatusPlusCount = statusPlusCount;
        this.LimitBreakCount = limitBreakCount;
    }

    public AtgenSupportWeapon() { }
}

[MessagePackObject(true)]
public class AtgenSupportWeaponBody
{
    public WeaponBodies WeaponBodyId { get; set; }
    public int BuildupCount { get; set; }
    public int LimitBreakCount { get; set; }
    public int LimitOverCount { get; set; }
    public int AdditionalEffectCount { get; set; }
    public int EquipableCount { get; set; }
    public int AdditionalCrestSlotType1Count { get; set; }
    public int AdditionalCrestSlotType2Count { get; set; }
    public int AdditionalCrestSlotType3Count { get; set; }

    public AtgenSupportWeaponBody(
        WeaponBodies weaponBodyId,
        int buildupCount,
        int limitBreakCount,
        int limitOverCount,
        int additionalEffectCount,
        int equipableCount,
        int additionalCrestSlotType1Count,
        int additionalCrestSlotType2Count,
        int additionalCrestSlotType3Count
    )
    {
        this.WeaponBodyId = weaponBodyId;
        this.BuildupCount = buildupCount;
        this.LimitBreakCount = limitBreakCount;
        this.LimitOverCount = limitOverCount;
        this.AdditionalEffectCount = additionalEffectCount;
        this.EquipableCount = equipableCount;
        this.AdditionalCrestSlotType1Count = additionalCrestSlotType1Count;
        this.AdditionalCrestSlotType2Count = additionalCrestSlotType2Count;
        this.AdditionalCrestSlotType3Count = additionalCrestSlotType3Count;
    }

    public AtgenSupportWeaponBody() { }
}

[MessagePackObject(true)]
public class AtgenTargetList
{
    public string TargetName { get; set; }
    public IEnumerable<long> TargetIdList { get; set; }

    public AtgenTargetList(string targetName, IEnumerable<long> targetIdList)
    {
        this.TargetName = targetName;
        this.TargetIdList = targetIdList;
    }

    public AtgenTargetList() { }
}

[MessagePackObject(true)]
public class AtgenTransitionResultData
{
    public ulong AbolishedViewerId { get; set; }
    public ulong LinkedViewerId { get; set; }

    public AtgenTransitionResultData(ulong abolishedViewerId, ulong linkedViewerId)
    {
        this.AbolishedViewerId = abolishedViewerId;
        this.LinkedViewerId = linkedViewerId;
    }

    public AtgenTransitionResultData() { }
}

[MessagePackObject(true)]
public class AtgenTreasureRecord
{
    public int AreaIdx { get; set; }
    public IEnumerable<int> DropObj { get; set; }
    public IEnumerable<int> Enemy { get; set; }
    public IEnumerable<AtgenEnemySmash> EnemySmash { get; set; }

    public AtgenTreasureRecord(
        int areaIdx,
        IEnumerable<int> dropObj,
        IEnumerable<int> enemy,
        IEnumerable<AtgenEnemySmash> enemySmash
    )
    {
        this.AreaIdx = areaIdx;
        this.DropObj = dropObj;
        this.Enemy = enemy;
        this.EnemySmash = enemySmash;
    }

    public AtgenTreasureRecord() { }
}

[MessagePackObject(true)]
public class AtgenUnit
{
    public IEnumerable<OddsUnitDetail> CharaOddsList { get; set; }
    public IEnumerable<OddsUnitDetail> DragonOddsList { get; set; }
    public IEnumerable<OddsUnitDetail> AmuletOddsList { get; set; }

    public AtgenUnit(
        IEnumerable<OddsUnitDetail> charaOddsList,
        IEnumerable<OddsUnitDetail> dragonOddsList,
        IEnumerable<OddsUnitDetail> amuletOddsList
    )
    {
        this.CharaOddsList = charaOddsList;
        this.DragonOddsList = dragonOddsList;
        this.AmuletOddsList = amuletOddsList;
    }

    public AtgenUnit() { }
}

[MessagePackObject(true)]
public class AtgenUnitData
{
    public Charas CharaId { get; set; }
    public int Skill1Level { get; set; }
    public int Skill2Level { get; set; }
    public int Ability1Level { get; set; }
    public int Ability2Level { get; set; }
    public int Ability3Level { get; set; }
    public int ExAbilityLevel { get; set; }
    public int ExAbility2Level { get; set; }
    public int BurstAttackLevel { get; set; }
    public int ComboBuildupCount { get; set; }

    public AtgenUnitData(
        Charas charaId,
        int skill1Level,
        int skill2Level,
        int ability1Level,
        int ability2Level,
        int ability3Level,
        int exAbilityLevel,
        int exAbility2Level,
        int burstAttackLevel,
        int comboBuildupCount
    )
    {
        this.CharaId = charaId;
        this.Skill1Level = skill1Level;
        this.Skill2Level = skill2Level;
        this.Ability1Level = ability1Level;
        this.Ability2Level = ability2Level;
        this.Ability3Level = ability3Level;
        this.ExAbilityLevel = exAbilityLevel;
        this.ExAbility2Level = exAbility2Level;
        this.BurstAttackLevel = burstAttackLevel;
        this.ComboBuildupCount = comboBuildupCount;
    }

    public AtgenUnitData() { }
}

[MessagePackObject(true)]
public class AtgenUnitList
{
    public int Id { get; set; }
    public string Rate { get; set; }

    public AtgenUnitList(int id, string rate)
    {
        this.Id = id;
        this.Rate = rate;
    }

    public AtgenUnitList() { }
}

[MessagePackObject(true)]
public class AtgenUseItemList
{
    public UseItem ItemId { get; set; }
    public int ItemQuantity { get; set; }

    public AtgenUseItemList(UseItem itemId, int itemQuantity)
    {
        this.ItemId = itemId;
        this.ItemQuantity = itemQuantity;
    }

    public AtgenUseItemList() { }
}

[MessagePackObject(true)]
public class AtgenUserBuildEventItemList
{
    public int UserBuildEventItem { get; set; }
    public int EventItemValue { get; set; }

    public AtgenUserBuildEventItemList(int userBuildEventItem, int eventItemValue)
    {
        this.UserBuildEventItem = userBuildEventItem;
        this.EventItemValue = eventItemValue;
    }

    public AtgenUserBuildEventItemList() { }
}

[MessagePackObject(true)]
public class AtgenUserClb01EventItemList
{
    public int UserClb01EventItem { get; set; }
    public int EventItemValue { get; set; }

    public AtgenUserClb01EventItemList(int userClb01EventItem, int eventItemValue)
    {
        this.UserClb01EventItem = userClb01EventItem;
        this.EventItemValue = eventItemValue;
    }

    public AtgenUserClb01EventItemList() { }
}

[MessagePackObject(true)]
public class AtgenUserCollectEventItemList
{
    public int UserCollectEventItem { get; set; }
    public int EventItemValue { get; set; }

    public AtgenUserCollectEventItemList(int userCollectEventItem, int eventItemValue)
    {
        this.UserCollectEventItem = userCollectEventItem;
        this.EventItemValue = eventItemValue;
    }

    public AtgenUserCollectEventItemList() { }
}

[MessagePackObject(true)]
public class AtgenUserEventTradeList
{
    public int EventTradeId { get; set; }
    public int TradeCount { get; set; }

    public AtgenUserEventTradeList(int eventTradeId, int tradeCount)
    {
        this.EventTradeId = eventTradeId;
        this.TradeCount = tradeCount;
    }

    public AtgenUserEventTradeList() { }
}

[MessagePackObject(true)]
public class AtgenUserItemSummon
{
    public int DailySummonCount { get; set; }
    public DateTimeOffset LastSummonTime { get; set; }

    public AtgenUserItemSummon(int dailySummonCount, DateTimeOffset lastSummonTime)
    {
        this.DailySummonCount = dailySummonCount;
        this.LastSummonTime = lastSummonTime;
    }

    public AtgenUserItemSummon() { }
}

[MessagePackObject(true)]
public class AtgenUserMazeEventItemList
{
    public int UserMazeEventItem { get; set; }
    public int EventItemValue { get; set; }

    public AtgenUserMazeEventItemList(int userMazeEventItem, int eventItemValue)
    {
        this.UserMazeEventItem = userMazeEventItem;
        this.EventItemValue = eventItemValue;
    }

    public AtgenUserMazeEventItemList() { }
}

[MessagePackObject(true)]
public class AtgenUserMazeEventItemList2
{
    public int EventItemId { get; set; }
    public int Quantity { get; set; }

    public AtgenUserMazeEventItemList2(int eventItemId, int quantity)
    {
        this.EventItemId = eventItemId;
        this.Quantity = quantity;
    }

    public AtgenUserMazeEventItemList2() { }
}

[MessagePackObject(true)]
public class AtgenUserWallRewardList
{
    public int QuestGroupId { get; set; }
    public int SumWallLevel { get; set; }
    public DateTimeOffset LastRewardDate { get; set; }
    public RewardStatus RewardStatus { get; set; }

    public AtgenUserWallRewardList(
        int questGroupId,
        int sumWallLevel,
        DateTimeOffset lastRewardDate,
        RewardStatus rewardStatus
    )
    {
        this.QuestGroupId = questGroupId;
        this.SumWallLevel = sumWallLevel;
        this.LastRewardDate = lastRewardDate;
        this.RewardStatus = rewardStatus;
    }

    public AtgenUserWallRewardList() { }
}

[MessagePackObject(true)]
public class AtgenVersionHash
{
    public string Region { get; set; }
    public string Lang { get; set; }
    public int EulaVersion { get; set; }
    public int PrivacyPolicyVersion { get; set; }

    public AtgenVersionHash(string region, string lang, int eulaVersion, int privacyPolicyVersion)
    {
        this.Region = region;
        this.Lang = lang;
        this.EulaVersion = eulaVersion;
        this.PrivacyPolicyVersion = privacyPolicyVersion;
    }

    public AtgenVersionHash() { }
}

[MessagePackObject(true)]
public class AtgenWalkerData
{
    public int ReliabilityLevel { get; set; }
    public int ReliabilityTotalExp { get; set; }
    public int LastContactTime { get; set; }
    public int Skill2Level { get; set; }

    public AtgenWalkerData(
        int reliabilityLevel,
        int reliabilityTotalExp,
        int lastContactTime,
        int skill2Level
    )
    {
        this.ReliabilityLevel = reliabilityLevel;
        this.ReliabilityTotalExp = reliabilityTotalExp;
        this.LastContactTime = lastContactTime;
        this.Skill2Level = skill2Level;
    }

    public AtgenWalkerData() { }
}

[MessagePackObject(true)]
public class AtgenWallDropReward
{
    public IEnumerable<AtgenBuildEventRewardEntityList> RewardEntityList { get; set; }
    public int TakeCoin { get; set; }
    public int TakeMana { get; set; }

    public AtgenWallDropReward(
        IEnumerable<AtgenBuildEventRewardEntityList> rewardEntityList,
        int takeCoin,
        int takeMana
    )
    {
        this.RewardEntityList = rewardEntityList;
        this.TakeCoin = takeCoin;
        this.TakeMana = takeMana;
    }

    public AtgenWallDropReward() { }
}

[MessagePackObject(true)]
public class AtgenWallUnitInfo
{
    public IEnumerable<PartySettingList> QuestPartySettingList { get; set; }
    public IEnumerable<UserSupportList> HelperList { get; set; }
    public IEnumerable<AtgenHelperDetailList> HelperDetailList { get; set; }

    public AtgenWallUnitInfo(
        IEnumerable<PartySettingList> questPartySettingList,
        IEnumerable<UserSupportList> helperList,
        IEnumerable<AtgenHelperDetailList> helperDetailList
    )
    {
        this.QuestPartySettingList = questPartySettingList;
        this.HelperList = helperList;
        this.HelperDetailList = helperDetailList;
    }

    public AtgenWallUnitInfo() { }
}

[MessagePackObject(true)]
public class AtgenWeaponKeyDataList
{
    public ulong KeyId { get; set; }
    public int TargetSetNum { get; set; }
    public int TargetWeaponNum { get; set; }

    public AtgenWeaponKeyDataList(ulong keyId, int targetSetNum, int targetWeaponNum)
    {
        this.KeyId = keyId;
        this.TargetSetNum = targetSetNum;
        this.TargetWeaponNum = targetWeaponNum;
    }

    public AtgenWeaponKeyDataList() { }
}

[MessagePackObject(true)]
public class AtgenWeaponSetList
{
    public int SelectWeaponId { get; set; }
    public IEnumerable<AtgenWeaponKeyDataList> WeaponKeyDataList { get; set; }

    public AtgenWeaponSetList(
        int selectWeaponId,
        IEnumerable<AtgenWeaponKeyDataList> weaponKeyDataList
    )
    {
        this.SelectWeaponId = selectWeaponId;
        this.WeaponKeyDataList = weaponKeyDataList;
    }

    public AtgenWeaponSetList() { }
}

[MessagePackObject(true)]
public class AtgenWebviewUrlList
{
    public string FunctionName { get; set; }
    public string Url { get; set; }

    public AtgenWebviewUrlList(string functionName, string url)
    {
        this.FunctionName = functionName;
        this.Url = url;
    }

    public AtgenWebviewUrlList() { }
}

[MessagePackObject(true)]
public class BattleRoyalCharaSkinList
{
    public int BattleRoyalCharaSkinId { get; set; }
    public int Gettime { get; set; }

    public BattleRoyalCharaSkinList(int battleRoyalCharaSkinId, int gettime)
    {
        this.BattleRoyalCharaSkinId = battleRoyalCharaSkinId;
        this.Gettime = gettime;
    }

    public BattleRoyalCharaSkinList() { }
}

[MessagePackObject(true)]
public class BattleRoyalCycleUserRecord
{
    public int EventId { get; set; }
    public int EventCycleId { get; set; }
    public int CycleTotalBattleRoyalPoint { get; set; }

    public BattleRoyalCycleUserRecord(int eventId, int eventCycleId, int cycleTotalBattleRoyalPoint)
    {
        this.EventId = eventId;
        this.EventCycleId = eventCycleId;
        this.CycleTotalBattleRoyalPoint = cycleTotalBattleRoyalPoint;
    }

    public BattleRoyalCycleUserRecord() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventItemList
{
    public int EventId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }

    public BattleRoyalEventItemList(int eventId, int itemId, int quantity)
    {
        this.EventId = eventId;
        this.ItemId = itemId;
        this.Quantity = quantity;
    }

    public BattleRoyalEventItemList() { }
}

[MessagePackObject(true)]
public class BattleRoyalEventUserRecord
{
    public int EventId { get; set; }
    public int TotalBattleRoyalPoint { get; set; }
    public int RankTopCount { get; set; }
    public int TopFourCount { get; set; }
    public int TotalKillCount { get; set; }
    public int MaxKillCount { get; set; }
    public int TotalAssistCount { get; set; }
    public int MaxAssistCount { get; set; }
    public int LastUseWeaponType { get; set; }
    public int UseSwdCount { get; set; }
    public int UseKatCount { get; set; }
    public int UseDagCount { get; set; }
    public int UseAxeCount { get; set; }
    public int UseLanCount { get; set; }
    public int UseBowCount { get; set; }
    public int UseRodCount { get; set; }
    public int UseCanCount { get; set; }
    public int UseGunCount { get; set; }

    public BattleRoyalEventUserRecord(
        int eventId,
        int totalBattleRoyalPoint,
        int rankTopCount,
        int topFourCount,
        int totalKillCount,
        int maxKillCount,
        int totalAssistCount,
        int maxAssistCount,
        int lastUseWeaponType,
        int useSwdCount,
        int useKatCount,
        int useDagCount,
        int useAxeCount,
        int useLanCount,
        int useBowCount,
        int useRodCount,
        int useCanCount,
        int useGunCount
    )
    {
        this.EventId = eventId;
        this.TotalBattleRoyalPoint = totalBattleRoyalPoint;
        this.RankTopCount = rankTopCount;
        this.TopFourCount = topFourCount;
        this.TotalKillCount = totalKillCount;
        this.MaxKillCount = maxKillCount;
        this.TotalAssistCount = totalAssistCount;
        this.MaxAssistCount = maxAssistCount;
        this.LastUseWeaponType = lastUseWeaponType;
        this.UseSwdCount = useSwdCount;
        this.UseKatCount = useKatCount;
        this.UseDagCount = useDagCount;
        this.UseAxeCount = useAxeCount;
        this.UseLanCount = useLanCount;
        this.UseBowCount = useBowCount;
        this.UseRodCount = useRodCount;
        this.UseCanCount = useCanCount;
        this.UseGunCount = useGunCount;
    }

    public BattleRoyalEventUserRecord() { }
}

[MessagePackObject(true)]
public class BattleRoyalResult
{
    public int BattleRoyalCycleId { get; set; }
    public int CharaId { get; set; }
    public int WeaponSkinId { get; set; }
    public int Ranking { get; set; }
    public int KillCount { get; set; }
    public int AssistCount { get; set; }
    public int TakeExp { get; set; }
    public int PlayerLevelUpFstone { get; set; }
    public int TakeAccumulatePoint { get; set; }
    public int TakeBattleRoyalPoint { get; set; }

    public BattleRoyalResult(
        int battleRoyalCycleId,
        int charaId,
        int weaponSkinId,
        int ranking,
        int killCount,
        int assistCount,
        int takeExp,
        int playerLevelUpFstone,
        int takeAccumulatePoint,
        int takeBattleRoyalPoint
    )
    {
        this.BattleRoyalCycleId = battleRoyalCycleId;
        this.CharaId = charaId;
        this.WeaponSkinId = weaponSkinId;
        this.Ranking = ranking;
        this.KillCount = killCount;
        this.AssistCount = assistCount;
        this.TakeExp = takeExp;
        this.PlayerLevelUpFstone = playerLevelUpFstone;
        this.TakeAccumulatePoint = takeAccumulatePoint;
        this.TakeBattleRoyalPoint = takeBattleRoyalPoint;
    }

    public BattleRoyalResult() { }
}

[MessagePackObject(true)]
public class BeginnerMissionList
{
    public int BeginnerMissionId { get; set; }
    public int Progress { get; set; }
    public int State { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset StartDate { get; set; }

    public BeginnerMissionList(
        int beginnerMissionId,
        int progress,
        int state,
        DateTimeOffset endDate,
        DateTimeOffset startDate
    )
    {
        this.BeginnerMissionId = beginnerMissionId;
        this.Progress = progress;
        this.State = state;
        this.EndDate = endDate;
        this.StartDate = startDate;
    }

    public BeginnerMissionList() { }
}

[MessagePackObject(true)]
public class BuildEventRewardList : IEventRewardList<BuildEventRewardList>
{
    public int EventId { get; set; }
    public int EventRewardId { get; set; }

    public BuildEventRewardList(int eventId, int eventRewardId)
    {
        this.EventId = eventId;
        this.EventRewardId = eventRewardId;
    }

    public static BuildEventRewardList FromDatabase(DbPlayerEventReward reward) =>
        new(reward.EventId, reward.RewardId);

    public BuildEventRewardList() { }
}

[MessagePackObject(true)]
public class BuildEventUserList
{
    public int BuildEventId { get; set; }
    public IEnumerable<AtgenUserBuildEventItemList> UserBuildEventItemList { get; set; }

    public BuildEventUserList(
        int buildEventId,
        IEnumerable<AtgenUserBuildEventItemList> userBuildEventItemList
    )
    {
        this.BuildEventId = buildEventId;
        this.UserBuildEventItemList = userBuildEventItemList;
    }

    public BuildEventUserList() { }
}

[MessagePackObject(true)]
public class BuildList
{
    public ulong BuildId { get; set; }
    public FortPlants PlantId { get; set; }
    public int Level { get; set; }
    public int FortPlantDetailId { get; set; }
    public int PositionX { get; set; }
    public int PositionZ { get; set; }
    public FortBuildStatus BuildStatus { get; set; }
    public DateTimeOffset BuildStartDate { get; set; }
    public DateTimeOffset BuildEndDate { get; set; }
    public TimeSpan RemainTime { get; set; }
    public TimeSpan LastIncomeTime { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool IsNew { get; set; }

    public BuildList(
        ulong buildId,
        FortPlants plantId,
        int level,
        int fortPlantDetailId,
        int positionX,
        int positionZ,
        FortBuildStatus buildStatus,
        DateTimeOffset buildStartDate,
        DateTimeOffset buildEndDate,
        TimeSpan remainTime,
        TimeSpan lastIncomeTime,
        bool isNew
    )
    {
        this.BuildId = buildId;
        this.PlantId = plantId;
        this.Level = level;
        this.FortPlantDetailId = fortPlantDetailId;
        this.PositionX = positionX;
        this.PositionZ = positionZ;
        this.BuildStatus = buildStatus;
        this.BuildStartDate = buildStartDate;
        this.BuildEndDate = buildEndDate;
        this.RemainTime = remainTime;
        this.LastIncomeTime = lastIncomeTime;
        this.IsNew = isNew;
    }

    public BuildList() { }
}

[MessagePackObject(true)]
public class CastleStoryList
{
    public int CastleStoryId { get; set; }
    public int IsRead { get; set; }

    public CastleStoryList(int castleStoryId, int isRead)
    {
        this.CastleStoryId = castleStoryId;
        this.IsRead = isRead;
    }

    public CastleStoryList() { }
}

[MessagePackObject(true)]
public class CharaFriendshipList
{
    public int CharaId { get; set; }
    public int AddPoint { get; set; }
    public int TotalPoint { get; set; }
    public int IsTemporary { get; set; }

    public CharaFriendshipList(int charaId, int addPoint, int totalPoint, int isTemporary)
    {
        this.CharaId = charaId;
        this.AddPoint = addPoint;
        this.TotalPoint = totalPoint;
        this.IsTemporary = isTemporary;
    }

    public CharaFriendshipList() { }
}

[MessagePackObject(true)]
public class CharaList
{
    public Charas CharaId { get; set; }
    public int Exp { get; set; }
    public int Level { get; set; }
    public int AdditionalMaxLevel { get; set; }
    public int Hp { get; set; }
    public int Attack { get; set; }
    public int ExAbilityLevel { get; set; }
    public int ExAbility2Level { get; set; }
    public int Ability1Level { get; set; }
    public int Ability2Level { get; set; }
    public int Ability3Level { get; set; }
    public int IsNew { get; set; }
    public int Skill1Level { get; set; }
    public int Skill2Level { get; set; }
    public int BurstAttackLevel { get; set; }
    public int Rarity { get; set; }
    public int LimitBreakCount { get; set; }
    public int HpPlusCount { get; set; }
    public int AttackPlusCount { get; set; }
    public int StatusPlusCount { get; set; }
    public int ComboBuildupCount { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool IsUnlockEditSkill { get; set; }
    public DateTimeOffset Gettime { get; set; }
    public IEnumerable<int> ManaCirclePieceIdList { get; set; }
    public int IsTemporary { get; set; }
    public int ListViewFlag { get; set; }

    public CharaList(
        Charas charaId,
        int exp,
        int level,
        int additionalMaxLevel,
        int hp,
        int attack,
        int exAbilityLevel,
        int exAbility2Level,
        int ability1Level,
        int ability2Level,
        int ability3Level,
        int isNew,
        int skill1Level,
        int skill2Level,
        int burstAttackLevel,
        int rarity,
        int limitBreakCount,
        int hpPlusCount,
        int attackPlusCount,
        int statusPlusCount,
        int comboBuildupCount,
        bool isUnlockEditSkill,
        DateTimeOffset gettime,
        IEnumerable<int> manaCirclePieceIdList,
        int isTemporary,
        int listViewFlag
    )
    {
        this.CharaId = charaId;
        this.Exp = exp;
        this.Level = level;
        this.AdditionalMaxLevel = additionalMaxLevel;
        this.Hp = hp;
        this.Attack = attack;
        this.ExAbilityLevel = exAbilityLevel;
        this.ExAbility2Level = exAbility2Level;
        this.Ability1Level = ability1Level;
        this.Ability2Level = ability2Level;
        this.Ability3Level = ability3Level;
        this.IsNew = isNew;
        this.Skill1Level = skill1Level;
        this.Skill2Level = skill2Level;
        this.BurstAttackLevel = burstAttackLevel;
        this.Rarity = rarity;
        this.LimitBreakCount = limitBreakCount;
        this.HpPlusCount = hpPlusCount;
        this.AttackPlusCount = attackPlusCount;
        this.StatusPlusCount = statusPlusCount;
        this.ComboBuildupCount = comboBuildupCount;
        this.IsUnlockEditSkill = isUnlockEditSkill;
        this.Gettime = gettime;
        this.ManaCirclePieceIdList = manaCirclePieceIdList;
        this.IsTemporary = isTemporary;
        this.ListViewFlag = listViewFlag;
    }

    public CharaList() { }
}

[MessagePackObject(true)]
public class CharaUnitSetList
{
    public Charas CharaId { get; set; }
    public IEnumerable<AtgenCharaUnitSetDetailList> CharaUnitSetDetailList { get; set; }

    public CharaUnitSetList(
        Charas charaId,
        IEnumerable<AtgenCharaUnitSetDetailList> charaUnitSetDetailList
    )
    {
        this.CharaId = charaId;
        this.CharaUnitSetDetailList = charaUnitSetDetailList;
    }

    public CharaUnitSetList() { }
}

[MessagePackObject(true)]
public class Clb01EventUserList
{
    public int EventId { get; set; }
    public IEnumerable<AtgenUserClb01EventItemList> UserClb01EventItemList { get; set; }

    public Clb01EventUserList(
        int eventId,
        IEnumerable<AtgenUserClb01EventItemList> userClb01EventItemList
    )
    {
        this.EventId = eventId;
        this.UserClb01EventItemList = userClb01EventItemList;
    }

    public Clb01EventUserList() { }
}

[MessagePackObject(true)]
public class CollectEventUserList
{
    public int EventId { get; set; }
    public IEnumerable<AtgenUserCollectEventItemList> UserCollectEventItemList { get; set; }

    public CollectEventUserList(
        int eventId,
        IEnumerable<AtgenUserCollectEventItemList> userCollectEventItemList
    )
    {
        this.EventId = eventId;
        this.UserCollectEventItemList = userCollectEventItemList;
    }

    public CollectEventUserList() { }
}

[MessagePackObject(true)]
public class CombatEventUserList
{
    public int EventId { get; set; }
    public int EventPoint { get; set; }
    public int ExchangeItem01 { get; set; }
    public int QuestUnlockItem01 { get; set; }
    public int StoryUnlockItem01 { get; set; }
    public int AdventItem01 { get; set; }

    public CombatEventUserList(
        int eventId,
        int eventPoint,
        int exchangeItem01,
        int questUnlockItem01,
        int storyUnlockItem01,
        int adventItem01
    )
    {
        this.EventId = eventId;
        this.EventPoint = eventPoint;
        this.ExchangeItem01 = exchangeItem01;
        this.QuestUnlockItem01 = questUnlockItem01;
        this.StoryUnlockItem01 = storyUnlockItem01;
        this.AdventItem01 = adventItem01;
    }

    public CombatEventUserList() { }
}

[MessagePackObject(true)]
public class ConvertedEntityList
{
    public EntityTypes BeforeEntityType { get; set; }
    public int BeforeEntityId { get; set; }
    public int BeforeEntityQuantity { get; set; }
    public EntityTypes AfterEntityType { get; set; }
    public int AfterEntityId { get; set; }
    public int AfterEntityQuantity { get; set; }

    public ConvertedEntityList(
        EntityTypes beforeEntityType,
        int beforeEntityId,
        int beforeEntityQuantity,
        EntityTypes afterEntityType,
        int afterEntityId,
        int afterEntityQuantity
    )
    {
        this.BeforeEntityType = beforeEntityType;
        this.BeforeEntityId = beforeEntityId;
        this.BeforeEntityQuantity = beforeEntityQuantity;
        this.AfterEntityType = afterEntityType;
        this.AfterEntityId = afterEntityId;
        this.AfterEntityQuantity = afterEntityQuantity;
    }

    public ConvertedEntityList() { }
}

[MessagePackObject(true)]
public class CraftList
{
    public int WeaponId { get; set; }
    public int IsNew { get; set; }

    public CraftList(int weaponId, int isNew)
    {
        this.WeaponId = weaponId;
        this.IsNew = isNew;
    }

    public CraftList() { }
}

[MessagePackObject(true)]
public class CurrentMainStoryMission
{
    public int MainStoryMissionGroupId { get; set; }
    public IEnumerable<AtgenMainStoryMissionStateList> MainStoryMissionStateList { get; set; }

    public CurrentMainStoryMission(
        int mainStoryMissionGroupId,
        IEnumerable<AtgenMainStoryMissionStateList> mainStoryMissionStateList
    )
    {
        this.MainStoryMissionGroupId = mainStoryMissionGroupId;
        this.MainStoryMissionStateList = mainStoryMissionStateList;
    }

    public CurrentMainStoryMission() { }
}

[MessagePackObject(true)]
public class DailyMissionList
{
    public int DailyMissionId { get; set; }
    public int Progress { get; set; }
    public MissionState State { get; set; }

    [MessagePackFormatter(typeof(DayNoFormatter))]
    public DateOnly DayNo { get; set; }

    public int WeeklyMissionId { get; set; }
    public int WeekNo { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public int IsLockReceiveReward { get; set; }
    public int IsPickup { get; set; }

    public DailyMissionList(
        int dailyMissionId,
        int progress,
        MissionState state,
        DateOnly dayNo,
        int weeklyMissionId,
        int weekNo,
        DateTimeOffset endDate,
        DateTimeOffset startDate,
        int isLockReceiveReward,
        int isPickup
    )
    {
        this.DailyMissionId = dailyMissionId;
        this.Progress = progress;
        this.State = state;
        this.DayNo = dayNo;
        this.WeeklyMissionId = weeklyMissionId;
        this.WeekNo = weekNo;
        this.EndDate = endDate;
        this.StartDate = startDate;
        this.IsLockReceiveReward = isLockReceiveReward;
        this.IsPickup = isPickup;
    }

    public DailyMissionList() { }
}

[MessagePackObject(true)]
public class DataHeader
{
    public int ResultCode { get; set; }

    public DataHeader(int resultCode)
    {
        this.ResultCode = resultCode;
    }

    public DataHeader() { }
}

[MessagePackObject(true)]
public class DeleteDataList
{
    public IEnumerable<AtgenDeleteDragonList> DeleteDragonList { get; set; }
    public IEnumerable<AtgenDeleteTalismanList> DeleteTalismanList { get; set; }
    public IEnumerable<AtgenDeleteWeaponList> DeleteWeaponList { get; set; }
    public IEnumerable<AtgenDeleteAmuletList> DeleteAmuletList { get; set; }

    public DeleteDataList(
        IEnumerable<AtgenDeleteDragonList> deleteDragonList,
        IEnumerable<AtgenDeleteTalismanList> deleteTalismanList,
        IEnumerable<AtgenDeleteWeaponList> deleteWeaponList,
        IEnumerable<AtgenDeleteAmuletList> deleteAmuletList
    )
    {
        this.DeleteDragonList = deleteDragonList;
        this.DeleteTalismanList = deleteTalismanList;
        this.DeleteWeaponList = deleteWeaponList;
        this.DeleteAmuletList = deleteAmuletList;
    }

    public DeleteDataList() { }
}

[MessagePackObject(true)]
public class DiamondData
{
    public int PaidDiamond { get; set; }
    public int FreeDiamond { get; set; }

    public DiamondData(int paidDiamond, int freeDiamond)
    {
        this.PaidDiamond = paidDiamond;
        this.FreeDiamond = freeDiamond;
    }

    public DiamondData() { }
}

[MessagePackObject(true)]
public class DmodeCharaList
{
    public Charas CharaId { get; set; }
    public int MaxFloorNum { get; set; }
    public int SelectServitorId { get; set; }
    public Charas SelectEditSkillCharaId1 { get; set; }
    public Charas SelectEditSkillCharaId2 { get; set; }
    public Charas SelectEditSkillCharaId3 { get; set; }
    public int MaxDmodeScore { get; set; }

    public DmodeCharaList(
        Charas charaId,
        int maxFloorNum,
        int selectServitorId,
        Charas selectEditSkillCharaId1,
        Charas selectEditSkillCharaId2,
        Charas selectEditSkillCharaId3,
        int maxDmodeScore
    )
    {
        this.CharaId = charaId;
        this.MaxFloorNum = maxFloorNum;
        this.SelectServitorId = selectServitorId;
        this.SelectEditSkillCharaId1 = selectEditSkillCharaId1;
        this.SelectEditSkillCharaId2 = selectEditSkillCharaId2;
        this.SelectEditSkillCharaId3 = selectEditSkillCharaId3;
        this.MaxDmodeScore = maxDmodeScore;
    }

    public DmodeCharaList() { }
}

[MessagePackObject(true)]
public class DmodeDungeonInfo
{
    public Charas CharaId { get; set; }
    public int FloorNum { get; set; }
    public int QuestTime { get; set; }
    public int DungeonScore { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsPlayEnd { get; set; }
    public DungeonState State { get; set; }

    public DmodeDungeonInfo(
        Charas charaId,
        int floorNum,
        int questTime,
        int dungeonScore,
        bool isPlayEnd,
        DungeonState state
    )
    {
        this.CharaId = charaId;
        this.FloorNum = floorNum;
        this.QuestTime = questTime;
        this.DungeonScore = dungeonScore;
        this.IsPlayEnd = isPlayEnd;
        this.State = state;
    }

    public DmodeDungeonInfo() { }
}

[MessagePackObject(true)]
public class DmodeDungeonItemList
{
    public int ItemNo { get; set; }
    public int ItemId { get; set; }
    public DmodeDungeonItemState ItemState { get; set; }
    public AtgenOption Option { get; set; }

    public DmodeDungeonItemList(
        int itemNo,
        int itemId,
        DmodeDungeonItemState itemState,
        AtgenOption option
    )
    {
        this.ItemNo = itemNo;
        this.ItemId = itemId;
        this.ItemState = itemState;
        this.Option = option;
    }

    public DmodeDungeonItemList() { }
}

[MessagePackObject(true)]
public class DmodeExpedition
{
    public Charas CharaId1 { get; set; }
    public Charas CharaId2 { get; set; }
    public Charas CharaId3 { get; set; }
    public Charas CharaId4 { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public int TargetFloorNum { get; set; }
    public ExpeditionState State { get; set; }

    public DmodeExpedition(
        Charas charaId1,
        Charas charaId2,
        Charas charaId3,
        Charas charaId4,
        DateTimeOffset startTime,
        int targetFloorNum,
        ExpeditionState state
    )
    {
        this.CharaId1 = charaId1;
        this.CharaId2 = charaId2;
        this.CharaId3 = charaId3;
        this.CharaId4 = charaId4;
        this.StartTime = startTime;
        this.TargetFloorNum = targetFloorNum;
        this.State = state;
    }

    public DmodeExpedition() { }
}

[MessagePackObject(true)]
public class DmodeFloorData
{
    public string UniqueKey { get; set; }
    public string FloorKey { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsEnd { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsPlayEnd { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsViewAreaStartEquipment { get; set; }
    public AtgenDmodeAreaInfo DmodeAreaInfo { get; set; }
    public AtgenDmodeUnitInfo DmodeUnitInfo { get; set; }
    public AtgenDmodeDungeonOdds DmodeDungeonOdds { get; set; }

    public DmodeFloorData(
        string uniqueKey,
        string floorKey,
        bool isEnd,
        bool isPlayEnd,
        bool isViewAreaStartEquipment,
        AtgenDmodeAreaInfo dmodeAreaInfo,
        AtgenDmodeUnitInfo dmodeUnitInfo,
        AtgenDmodeDungeonOdds dmodeDungeonOdds
    )
    {
        this.UniqueKey = uniqueKey;
        this.FloorKey = floorKey;
        this.IsEnd = isEnd;
        this.IsPlayEnd = isPlayEnd;
        this.IsViewAreaStartEquipment = isViewAreaStartEquipment;
        this.DmodeAreaInfo = dmodeAreaInfo;
        this.DmodeUnitInfo = dmodeUnitInfo;
        this.DmodeDungeonOdds = dmodeDungeonOdds;
    }

    public DmodeFloorData() { }
}

[MessagePackObject(true)]
public class DmodeInfo
{
    public int TotalMaxFloorNum { get; set; }
    public int RecoveryCount { get; set; }
    public DateTimeOffset RecoveryTime { get; set; }
    public int FloorSkipCount { get; set; }
    public DateTimeOffset FloorSkipTime { get; set; }
    public int DmodePoint1 { get; set; }
    public int DmodePoint2 { get; set; }

    [MessagePackFormatter((typeof(BoolToIntFormatter)))]
    public bool IsEntry { get; set; }

    public DmodeInfo(
        int totalMaxFloorNum,
        int recoveryCount,
        DateTimeOffset recoveryTime,
        int floorSkipCount,
        DateTimeOffset floorSkipTime,
        int dmodePoint1,
        int dmodePoint2,
        bool isEntry
    )
    {
        this.TotalMaxFloorNum = totalMaxFloorNum;
        this.RecoveryCount = recoveryCount;
        this.RecoveryTime = recoveryTime;
        this.FloorSkipCount = floorSkipCount;
        this.FloorSkipTime = floorSkipTime;
        this.DmodePoint1 = dmodePoint1;
        this.DmodePoint2 = dmodePoint2;
        this.IsEntry = isEntry;
    }

    public DmodeInfo() { }
}

[MessagePackObject(true)]
public class DmodeIngameData
{
    public string UniqueKey { get; set; }
    public int StartFloorNum { get; set; }
    public int TargetFloorNum { get; set; }
    public int RecoveryCount { get; set; }
    public DateTimeOffset RecoveryTime { get; set; }
    public int ServitorId { get; set; }
    public int DmodeLevelGroupId { get; set; }
    public AtgenUnitData UnitData { get; set; }
    public IEnumerable<DmodeServitorPassiveList> DmodeServitorPassiveList { get; set; }

    public DmodeIngameData(
        string uniqueKey,
        int startFloorNum,
        int targetFloorNum,
        int recoveryCount,
        DateTimeOffset recoveryTime,
        int servitorId,
        int dmodeLevelGroupId,
        AtgenUnitData unitData,
        IEnumerable<DmodeServitorPassiveList> dmodeServitorPassiveList
    )
    {
        this.UniqueKey = uniqueKey;
        this.StartFloorNum = startFloorNum;
        this.TargetFloorNum = targetFloorNum;
        this.RecoveryCount = recoveryCount;
        this.RecoveryTime = recoveryTime;
        this.ServitorId = servitorId;
        this.DmodeLevelGroupId = dmodeLevelGroupId;
        this.UnitData = unitData;
        this.DmodeServitorPassiveList = dmodeServitorPassiveList;
    }

    public DmodeIngameData() { }
}

[MessagePackObject(true)]
public class DmodeIngameResult
{
    public int FloorNum { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsRecordFloorNum { get; set; }
    public IEnumerable<Charas> CharaIdList { get; set; }
    public float QuestTime { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsViewQuestTime { get; set; }
    public int DmodeScore { get; set; }
    public IEnumerable<AtgenRewardTalismanList> RewardTalismanList { get; set; }
    public int TakeDmodePoint1 { get; set; }
    public int TakeDmodePoint2 { get; set; }
    public int TakePlayerExp { get; set; }
    public int PlayerLevelUpFstone { get; set; }
    public int ClearState { get; set; }

    public DmodeIngameResult(
        int floorNum,
        bool isRecordFloorNum,
        IEnumerable<Charas> charaIdList,
        float questTime,
        bool isViewQuestTime,
        int dmodeScore,
        IEnumerable<AtgenRewardTalismanList> rewardTalismanList,
        int takeDmodePoint1,
        int takeDmodePoint2,
        int takePlayerExp,
        int playerLevelUpFstone,
        int clearState
    )
    {
        this.FloorNum = floorNum;
        this.IsRecordFloorNum = isRecordFloorNum;
        this.CharaIdList = charaIdList;
        this.QuestTime = questTime;
        this.IsViewQuestTime = isViewQuestTime;
        this.DmodeScore = dmodeScore;
        this.RewardTalismanList = rewardTalismanList;
        this.TakeDmodePoint1 = takeDmodePoint1;
        this.TakeDmodePoint2 = takeDmodePoint2;
        this.TakePlayerExp = takePlayerExp;
        this.PlayerLevelUpFstone = playerLevelUpFstone;
        this.ClearState = clearState;
    }

    public DmodeIngameResult() { }
}

[MessagePackObject(true)]
public class DmodeOddsInfo
{
    public IEnumerable<AtgenDmodeDropObj> DmodeDropObj { get; set; }
    public IEnumerable<AtgenDmodeEnemy> DmodeEnemy { get; set; }

    public DmodeOddsInfo(
        IEnumerable<AtgenDmodeDropObj> dmodeDropObj,
        IEnumerable<AtgenDmodeEnemy> dmodeEnemy
    )
    {
        this.DmodeDropObj = dmodeDropObj;
        this.DmodeEnemy = dmodeEnemy;
    }

    public DmodeOddsInfo() { }
}

[MessagePackObject(true)]
public class DmodePlayRecord
{
    public string UniqueKey { get; set; }
    public string FloorKey { get; set; }
    public int FloorNum { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsFloorIncomplete { get; set; }
    public AtgenDmodeTreasureRecord DmodeTreasureRecord { get; set; }
    public IEnumerable<AtgenDmodeDungeonItemStateList> DmodeDungeonItemStateList { get; set; }
    public IEnumerable<AtgenDmodeDungeonItemOptionList> DmodeDungeonItemOptionList { get; set; }
    public IEnumerable<AtgenDmodeDragonUseList> DmodeDragonUseList { get; set; }
    public int[] EquipCrestItemNoSortList { get; set; }
    public IEnumerable<int> BagItemNoSortList { get; set; }
    public IEnumerable<int> SkillBagItemNoSortList { get; set; }
    public float QuestTime { get; set; }
    public int SelectDragonNo { get; set; }

    public DmodePlayRecord(
        string uniqueKey,
        string floorKey,
        int floorNum,
        bool isFloorIncomplete,
        AtgenDmodeTreasureRecord dmodeTreasureRecord,
        IEnumerable<AtgenDmodeDungeonItemStateList> dmodeDungeonItemStateList,
        IEnumerable<AtgenDmodeDungeonItemOptionList> dmodeDungeonItemOptionList,
        IEnumerable<AtgenDmodeDragonUseList> dmodeDragonUseList,
        int[] equipCrestItemNoSortList,
        IEnumerable<int> bagItemNoSortList,
        IEnumerable<int> skillBagItemNoSortList,
        float questTime,
        int selectDragonNo
    )
    {
        this.UniqueKey = uniqueKey;
        this.FloorKey = floorKey;
        this.FloorNum = floorNum;
        this.IsFloorIncomplete = isFloorIncomplete;
        this.DmodeTreasureRecord = dmodeTreasureRecord;
        this.DmodeDungeonItemStateList = dmodeDungeonItemStateList;
        this.DmodeDungeonItemOptionList = dmodeDungeonItemOptionList;
        this.DmodeDragonUseList = dmodeDragonUseList;
        this.EquipCrestItemNoSortList = equipCrestItemNoSortList;
        this.BagItemNoSortList = bagItemNoSortList;
        this.SkillBagItemNoSortList = skillBagItemNoSortList;
        this.QuestTime = questTime;
        this.SelectDragonNo = selectDragonNo;
    }

    public DmodePlayRecord() { }
}

[MessagePackObject(true)]
public class DmodeServitorPassiveList
{
    public DmodeServitorPassiveType PassiveNo { get; set; }
    public int PassiveLevel { get; set; }

    public DmodeServitorPassiveList(DmodeServitorPassiveType passiveNo, int passiveLevel)
    {
        this.PassiveNo = passiveNo;
        this.PassiveLevel = passiveLevel;
    }

    public DmodeServitorPassiveList() { }
}

[MessagePackObject(true)]
public class DmodeStoryList
{
    public int DmodeStoryId { get; set; }

    public int IsRead { get; set; }

    public DmodeStoryList(int dmodeStoryId, int isRead)
    {
        this.DmodeStoryId = dmodeStoryId;
        this.IsRead = isRead;
    }

    public DmodeStoryList() { }
}

[MessagePackObject(true)]
public class DragonGiftList
{
    public DragonGifts DragonGiftId { get; set; }
    public int Quantity { get; set; }

    public DragonGiftList(DragonGifts dragonGiftId, int quantity)
    {
        this.DragonGiftId = dragonGiftId;
        this.Quantity = quantity;
    }

    public DragonGiftList() { }
}

[MessagePackObject(true)]
public class DragonList
{
    public Dragons DragonId { get; set; }
    public ulong DragonKeyId { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool IsLock { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool IsNew { get; set; }
    public DateTimeOffset GetTime { get; set; }
    public int Skill1Level { get; set; }
    public int Ability1Level { get; set; }
    public int Ability2Level { get; set; }
    public int LimitBreakCount { get; set; }
    public int HpPlusCount { get; set; }
    public int AttackPlusCount { get; set; }
    public int StatusPlusCount { get; set; }

    public DragonList(
        Dragons dragonId,
        ulong dragonKeyId,
        int level,
        int exp,
        bool isLock,
        bool isNew,
        DateTimeOffset getTime,
        int skill1Level,
        int ability1Level,
        int ability2Level,
        int limitBreakCount,
        int hpPlusCount,
        int attackPlusCount,
        int statusPlusCount
    )
    {
        this.DragonId = dragonId;
        this.DragonKeyId = dragonKeyId;
        this.Level = level;
        this.Exp = exp;
        this.IsLock = isLock;
        this.IsNew = isNew;
        this.GetTime = getTime;
        this.Skill1Level = skill1Level;
        this.Ability1Level = ability1Level;
        this.Ability2Level = ability2Level;
        this.LimitBreakCount = limitBreakCount;
        this.HpPlusCount = hpPlusCount;
        this.AttackPlusCount = attackPlusCount;
        this.StatusPlusCount = statusPlusCount;
    }

    public DragonList() { }
}

[MessagePackObject(true)]
public class DragonReliabilityList
{
    public Dragons DragonId { get; set; }
    public int ReliabilityLevel { get; set; }
    public int ReliabilityTotalExp { get; set; }

    public DateTimeOffset Gettime { get; set; }

    public DateTimeOffset LastContactTime { get; set; }

    public DragonReliabilityList(
        Dragons dragonId,
        int reliabilityLevel,
        int reliabilityTotalExp,
        DateTimeOffset gettime,
        DateTimeOffset lastContactTime
    )
    {
        this.DragonId = dragonId;
        this.ReliabilityLevel = reliabilityLevel;
        this.ReliabilityTotalExp = reliabilityTotalExp;
        this.Gettime = gettime;
        this.LastContactTime = lastContactTime;
    }

    public DragonReliabilityList() { }
}

[MessagePackObject(true)]
public class DragonRewardEntityList
{
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }
    public int EntityQuantity { get; set; }
    public int IsOver { get; set; }

    public DragonRewardEntityList(
        EntityTypes entityType,
        int entityId,
        int entityQuantity,
        int isOver
    )
    {
        this.EntityType = entityType;
        this.EntityId = entityId;
        this.EntityQuantity = entityQuantity;
        this.IsOver = isOver;
    }

    public DragonRewardEntityList() { }
}

[MessagePackObject(true)]
public class DrillMissionGroupList
{
    public int DrillMissionGroupId { get; set; }

    public DrillMissionGroupList(int drillMissionGroupId)
    {
        this.DrillMissionGroupId = drillMissionGroupId;
    }

    public DrillMissionGroupList() { }
}

[MessagePackObject(true)]
public class DrillMissionList
{
    public int DrillMissionId { get; set; }
    public int Progress { get; set; }
    public int State { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset StartDate { get; set; }

    public DrillMissionList(
        int drillMissionId,
        int progress,
        int state,
        DateTimeOffset endDate,
        DateTimeOffset startDate
    )
    {
        this.DrillMissionId = drillMissionId;
        this.Progress = progress;
        this.State = state;
        this.EndDate = endDate;
        this.StartDate = startDate;
    }

    public DrillMissionList() { }
}

[MessagePackObject(true)]
public class EarnEventUserList
{
    public int EventId { get; set; }
    public int EventPoint { get; set; }
    public int ExchangeItem01 { get; set; }
    public int ExchangeItem02 { get; set; }
    public int AdventItemQuantity01 { get; set; }

    public EarnEventUserList(
        int eventId,
        int eventPoint,
        int exchangeItem01,
        int exchangeItem02,
        int adventItemQuantity01
    )
    {
        this.EventId = eventId;
        this.EventPoint = eventPoint;
        this.ExchangeItem01 = exchangeItem01;
        this.ExchangeItem02 = exchangeItem02;
        this.AdventItemQuantity01 = adventItemQuantity01;
    }

    public EarnEventUserList() { }
}

[MessagePackObject(true)]
public class EditSkillCharaData
{
    public Charas CharaId { get; set; }
    public int EditSkillLevel { get; set; }

    public EditSkillCharaData(Charas charaId, int editSkillLevel)
    {
        this.CharaId = charaId;
        this.EditSkillLevel = editSkillLevel;
    }

    public EditSkillCharaData() { }
}

[MessagePackObject(true)]
public class EmblemList
{
    public Emblems EmblemId { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsNew { get; set; }
    public DateTimeOffset Gettime { get; set; }

    public EmblemList(Emblems emblemId, bool isNew, DateTimeOffset gettime)
    {
        this.EmblemId = emblemId;
        this.IsNew = isNew;
        this.Gettime = gettime;
    }

    public EmblemList() { }
}

[MessagePackObject(true)]
public class EnemyBookList
{
    public int EnemyBookId { get; set; }
    public int PieceCount { get; set; }
    public int KillCount { get; set; }

    public EnemyBookList(int enemyBookId, int pieceCount, int killCount)
    {
        this.EnemyBookId = enemyBookId;
        this.PieceCount = pieceCount;
        this.KillCount = killCount;
    }

    public EnemyBookList() { }
}

[MessagePackObject(true)]
public class EnemyDamageHistory
{
    public IEnumerable<int> Damage { get; set; }
    public IEnumerable<int> Combo { get; set; }

    public EnemyDamageHistory(IEnumerable<int> damage, IEnumerable<int> combo)
    {
        this.Damage = damage;
        this.Combo = combo;
    }

    public EnemyDamageHistory() { }
}

[MessagePackObject(true)]
public class EnemyDropList
{
    public int Coin { get; set; }
    public int Mana { get; set; }
    public List<AtgenDropList> DropList { get; set; } = new();

    public EnemyDropList(int coin, int mana, List<AtgenDropList> dropList)
    {
        this.Coin = coin;
        this.Mana = mana;
        this.DropList = dropList;
    }

    public EnemyDropList() { }
}

[MessagePackObject(true)]
public class EntityResult
{
    public IEnumerable<AtgenBuildEventRewardEntityList> OverDiscardEntityList { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> OverPresentEntityList { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> OverPresentLimitEntityList { get; set; }
    public IEnumerable<AtgenDuplicateEntityList> NewGetEntityList { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; } =
        new List<ConvertedEntityList>();

    public EntityResult(
        IEnumerable<AtgenBuildEventRewardEntityList> overDiscardEntityList,
        IEnumerable<AtgenBuildEventRewardEntityList> overPresentEntityList,
        IEnumerable<AtgenBuildEventRewardEntityList> overPresentLimitEntityList,
        IEnumerable<AtgenDuplicateEntityList> newGetEntityList,
        IEnumerable<ConvertedEntityList> convertedEntityList
    )
    {
        this.OverDiscardEntityList = overDiscardEntityList;
        this.OverPresentEntityList = overPresentEntityList;
        this.OverPresentLimitEntityList = overPresentLimitEntityList;
        this.NewGetEntityList = newGetEntityList;
        this.ConvertedEntityList = convertedEntityList;
    }

    public EntityResult() { }
}

[MessagePackObject(true)]
public class EquipStampList
{
    public int Slot { get; set; }
    public int StampId { get; set; }

    public EquipStampList(int slot, int stampId)
    {
        this.Slot = slot;
        this.StampId = stampId;
    }

    public EquipStampList() { }
}

[MessagePackObject(true)]
public class EventAbilityCharaList
{
    public int CharaId { get; set; }
    public int AbilityId1 { get; set; }
    public int AbilityId2 { get; set; }

    public EventAbilityCharaList(int charaId, int abilityId1, int abilityId2)
    {
        this.CharaId = charaId;
        this.AbilityId1 = abilityId1;
        this.AbilityId2 = abilityId2;
    }

    public EventAbilityCharaList() { }
}

[MessagePackObject(true)]
public class EventCycleRewardList
{
    public int EventCycleId { get; set; }
    public int EventCycleRewardId { get; set; }

    public EventCycleRewardList(int eventCycleId, int eventCycleRewardId)
    {
        this.EventCycleId = eventCycleId;
        this.EventCycleRewardId = eventCycleRewardId;
    }

    public EventCycleRewardList() { }
}

[MessagePackObject(true)]
public class EventDamageRanking
{
    public int EventId { get; set; }
    public IEnumerable<AtgenOwnDamageRankingList> OwnDamageRankingList { get; set; }

    public EventDamageRanking(
        int eventId,
        IEnumerable<AtgenOwnDamageRankingList> ownDamageRankingList
    )
    {
        this.EventId = eventId;
        this.OwnDamageRankingList = ownDamageRankingList;
    }

    public EventDamageRanking() { }
}

[MessagePackObject(true)]
public class EventPassiveList
{
    public int EventId { get; set; }
    public IEnumerable<AtgenEventPassiveUpList> EventPassiveGrowList { get; set; }

    public EventPassiveList(int eventId, IEnumerable<AtgenEventPassiveUpList> eventPassiveGrowList)
    {
        this.EventId = eventId;
        this.EventPassiveGrowList = eventPassiveGrowList;
    }

    public EventPassiveList() { }
}

[MessagePackObject(true)]
public class EventStoryList
{
    public int EventStoryId { get; set; }
    public int State { get; set; }

    public EventStoryList(int eventStoryId, int state)
    {
        this.EventStoryId = eventStoryId;
        this.State = state;
    }

    public EventStoryList() { }
}

[MessagePackObject(true)]
public class EventTradeList
{
    public int EventTradeId { get; set; }
    public int TradeGroupId { get; set; }
    public int TabGroupId { get; set; }
    public int Priority { get; set; }
    public int IsLockView { get; set; }
    public DateTimeOffset CommenceDate { get; set; }
    public DateTimeOffset CompleteDate { get; set; }
    public int ResetType { get; set; }
    public int Limit { get; set; }
    public int ReadStoryCount { get; set; }
    public int ClearTargetQuestId { get; set; }
    public EntityTypes DestinationEntityType { get; set; }
    public int DestinationEntityId { get; set; }
    public int DestinationEntityQuantity { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList> NeedEntityList { get; set; }

    public EventTradeList(
        int eventTradeId,
        int tradeGroupId,
        int tabGroupId,
        int priority,
        int isLockView,
        DateTimeOffset commenceDate,
        DateTimeOffset completeDate,
        int resetType,
        int limit,
        int readStoryCount,
        int clearTargetQuestId,
        EntityTypes destinationEntityType,
        int destinationEntityId,
        int destinationEntityQuantity,
        IEnumerable<AtgenBuildEventRewardEntityList> needEntityList
    )
    {
        this.EventTradeId = eventTradeId;
        this.TradeGroupId = tradeGroupId;
        this.TabGroupId = tabGroupId;
        this.Priority = priority;
        this.IsLockView = isLockView;
        this.CommenceDate = commenceDate;
        this.CompleteDate = completeDate;
        this.ResetType = resetType;
        this.Limit = limit;
        this.ReadStoryCount = readStoryCount;
        this.ClearTargetQuestId = clearTargetQuestId;
        this.DestinationEntityType = destinationEntityType;
        this.DestinationEntityId = destinationEntityId;
        this.DestinationEntityQuantity = destinationEntityQuantity;
        this.NeedEntityList = needEntityList;
    }

    public EventTradeList() { }
}

[MessagePackObject(true)]
public class ExchangeTicketList
{
    public int ExchangeTicketId { get; set; }
    public int Quantity { get; set; }

    public ExchangeTicketList(int exchangeTicketId, int quantity)
    {
        this.ExchangeTicketId = exchangeTicketId;
        this.Quantity = quantity;
    }

    public ExchangeTicketList() { }
}

[MessagePackObject(true)]
public class ExHunterEventUserList
{
    public int EventId { get; set; }
    public int BoxSummonPoint { get; set; }
    public int ExHunterPoint1 { get; set; }
    public int ExHunterPoint2 { get; set; }
    public int ExHunterPoint3 { get; set; }
    public int AdventItemQuantity1 { get; set; }
    public int AdventItemQuantity2 { get; set; }
    public int UltimateKeyCount { get; set; }
    public int ExchangeItem1 { get; set; }
    public int ExchangeItem2 { get; set; }

    public ExHunterEventUserList(
        int eventId,
        int boxSummonPoint,
        int exHunterPoint1,
        int exHunterPoint2,
        int exHunterPoint3,
        int adventItemQuantity1,
        int adventItemQuantity2,
        int ultimateKeyCount,
        int exchangeItem1,
        int exchangeItem2
    )
    {
        this.EventId = eventId;
        this.BoxSummonPoint = boxSummonPoint;
        this.ExHunterPoint1 = exHunterPoint1;
        this.ExHunterPoint2 = exHunterPoint2;
        this.ExHunterPoint3 = exHunterPoint3;
        this.AdventItemQuantity1 = adventItemQuantity1;
        this.AdventItemQuantity2 = adventItemQuantity2;
        this.UltimateKeyCount = ultimateKeyCount;
        this.ExchangeItem1 = exchangeItem1;
        this.ExchangeItem2 = exchangeItem2;
    }

    public ExHunterEventUserList() { }
}

[MessagePackObject(true)]
public class ExRushEventUserList
{
    public int EventId { get; set; }
    public int ExRushItem1 { get; set; }
    public int ExRushItem2 { get; set; }

    public ExRushEventUserList(int eventId, int exRushItem1, int exRushItem2)
    {
        this.EventId = eventId;
        this.ExRushItem1 = exRushItem1;
        this.ExRushItem2 = exRushItem2;
    }

    public ExRushEventUserList() { }
}

[MessagePackObject(true)]
public class FortBonusList
{
    public IEnumerable<AtgenParamBonus> ParamBonus { get; set; }
    public IEnumerable<AtgenParamBonus> ParamBonusByWeapon { get; set; }
    public IEnumerable<AtgenElementBonus> ElementBonus { get; set; }
    public AtgenAllBonus AllBonus { get; set; }
    public IEnumerable<AtgenElementBonus> CharaBonusByAlbum { get; set; }
    public IEnumerable<AtgenDragonBonus> DragonBonus { get; set; }
    public AtgenDragonTimeBonus DragonTimeBonus { get; set; }
    public IEnumerable<AtgenElementBonus> DragonBonusByAlbum { get; set; }

    public FortBonusList(
        IEnumerable<AtgenParamBonus> paramBonus,
        IEnumerable<AtgenParamBonus> paramBonusByWeapon,
        IEnumerable<AtgenElementBonus> elementBonus,
        AtgenAllBonus allBonus,
        IEnumerable<AtgenElementBonus> charaBonusByAlbum,
        IEnumerable<AtgenDragonBonus> dragonBonus,
        AtgenDragonTimeBonus dragonTimeBonus,
        IEnumerable<AtgenElementBonus> dragonBonusByAlbum
    )
    {
        this.ParamBonus = paramBonus;
        this.ParamBonusByWeapon = paramBonusByWeapon;
        this.ElementBonus = elementBonus;
        this.AllBonus = allBonus;
        this.CharaBonusByAlbum = charaBonusByAlbum;
        this.DragonBonus = dragonBonus;
        this.DragonTimeBonus = dragonTimeBonus;
        this.DragonBonusByAlbum = dragonBonusByAlbum;
    }

    public FortBonusList() { }
}

[MessagePackObject(true)]
public class FortDetail
{
    public int MaxCarpenterCount { get; set; }
    public int CarpenterNum { get; set; }
    public int WorkingCarpenterNum { get; set; }

    public FortDetail(int maxCarpenterCount, int carpenterNum, int workingCarpenterNum)
    {
        this.MaxCarpenterCount = maxCarpenterCount;
        this.CarpenterNum = carpenterNum;
        this.WorkingCarpenterNum = workingCarpenterNum;
    }

    public FortDetail() { }
}

[MessagePackObject(true)]
public class FortPlantList
{
    public int PlantId { get; set; }
    public int IsNew { get; set; }

    public FortPlantList(int plantId, int isNew)
    {
        this.PlantId = plantId;
        this.IsNew = isNew;
    }

    public FortPlantList() { }
}

[MessagePackObject(true)]
public class FriendNotice
{
    public int FriendNewCount { get; set; }
    public int ApplyNewCount { get; set; }

    public FriendNotice(int friendNewCount, int applyNewCount)
    {
        this.FriendNewCount = friendNewCount;
        this.ApplyNewCount = applyNewCount;
    }

    public FriendNotice() { }
}

[MessagePackObject(true)]
public class FunctionalMaintenanceList
{
    public int FunctionalMaintenanceType { get; set; }

    public FunctionalMaintenanceList(int functionalMaintenanceType)
    {
        this.FunctionalMaintenanceType = functionalMaintenanceType;
    }

    public FunctionalMaintenanceList() { }
}

[MessagePackObject(true)]
public class GameAbilityCrest
{
    public AbilityCrests AbilityCrestId { get; set; }
    public int BuildupCount { get; set; }
    public int LimitBreakCount { get; set; }
    public int EquipableCount { get; set; }
    public int Ability1Level { get; set; }
    public int Ability2Level { get; set; }
    public int HpPlusCount { get; set; }
    public int AttackPlusCount { get; set; }

    public GameAbilityCrest(
        AbilityCrests abilityCrestId,
        int buildupCount,
        int limitBreakCount,
        int equipableCount,
        int ability1Level,
        int ability2Level,
        int hpPlusCount,
        int attackPlusCount
    )
    {
        this.AbilityCrestId = abilityCrestId;
        this.BuildupCount = buildupCount;
        this.LimitBreakCount = limitBreakCount;
        this.EquipableCount = equipableCount;
        this.Ability1Level = ability1Level;
        this.Ability2Level = ability2Level;
        this.HpPlusCount = hpPlusCount;
        this.AttackPlusCount = attackPlusCount;
    }

    public GameAbilityCrest() { }
}

[MessagePackObject(true)]
public class GameWeaponBody
{
    public WeaponBodies WeaponBodyId { get; set; }
    public int BuildupCount { get; set; }
    public int LimitBreakCount { get; set; }
    public int LimitOverCount { get; set; }
    public int SkillNo { get; set; }
    public int SkillLevel { get; set; }
    public int Ability1Level { get; set; }
    public int Ability2Level { get; set; }
    public int EquipableCount { get; set; }
    public int AdditionalCrestSlotType1Count { get; set; }
    public int AdditionalCrestSlotType2Count { get; set; }
    public int AdditionalCrestSlotType3Count { get; set; }
    public int AdditionalEffectCount { get; set; }

    public GameWeaponBody(
        WeaponBodies weaponBodyId,
        int buildupCount,
        int limitBreakCount,
        int limitOverCount,
        int skillNo,
        int skillLevel,
        int ability1Level,
        int ability2Level,
        int equipableCount,
        int additionalCrestSlotType1Count,
        int additionalCrestSlotType2Count,
        int additionalCrestSlotType3Count,
        int additionalEffectCount
    )
    {
        this.WeaponBodyId = weaponBodyId;
        this.BuildupCount = buildupCount;
        this.LimitBreakCount = limitBreakCount;
        this.LimitOverCount = limitOverCount;
        this.SkillNo = skillNo;
        this.SkillLevel = skillLevel;
        this.Ability1Level = ability1Level;
        this.Ability2Level = ability2Level;
        this.EquipableCount = equipableCount;
        this.AdditionalCrestSlotType1Count = additionalCrestSlotType1Count;
        this.AdditionalCrestSlotType2Count = additionalCrestSlotType2Count;
        this.AdditionalCrestSlotType3Count = additionalCrestSlotType3Count;
        this.AdditionalEffectCount = additionalEffectCount;
    }

    public GameWeaponBody() { }
}

[MessagePackObject(true)]
public class GameWeaponSkin
{
    public int WeaponSkinId { get; set; }

    public GameWeaponSkin(int weaponSkinId)
    {
        this.WeaponSkinId = weaponSkinId;
    }

    public GameWeaponSkin() { }
}

[MessagePackObject(true)]
public class GatherItemList
{
    public int GatherItemId { get; set; }
    public int Quantity { get; set; }
    public int QuestTakeWeeklyQuantity { get; set; }
    public int QuestLastWeeklyResetTime { get; set; }

    public GatherItemList(
        int gatherItemId,
        int quantity,
        int questTakeWeeklyQuantity,
        int questLastWeeklyResetTime
    )
    {
        this.GatherItemId = gatherItemId;
        this.Quantity = quantity;
        this.QuestTakeWeeklyQuantity = questTakeWeeklyQuantity;
        this.QuestLastWeeklyResetTime = questLastWeeklyResetTime;
    }

    public GatherItemList() { }
}

[MessagePackObject(true)]
public class GrowMaterialList
{
    public EntityTypes Type { get; set; }
    public int Id { get; set; }
    public int Quantity { get; set; }

    public GrowMaterialList(EntityTypes type, int id, int quantity)
    {
        this.Type = type;
        this.Id = id;
        this.Quantity = quantity;
    }

    public GrowMaterialList() { }
}

[MessagePackObject(true)]
public class GrowRecord
{
    public int TakePlayerExp { get; set; }
    public int TakeCharaExp { get; set; }
    public int TakeMana { get; set; }
    public float BonusFactor { get; set; }
    public float ManaBonusFactor { get; set; }
    public IEnumerable<AtgenCharaGrowRecord> CharaGrowRecord { get; set; } =
        Enumerable.Empty<AtgenCharaGrowRecord>();
    public IEnumerable<CharaFriendshipList> CharaFriendshipList { get; set; } =
        Enumerable.Empty<CharaFriendshipList>();

    public GrowRecord(
        int takePlayerExp,
        int takeCharaExp,
        int takeMana,
        float bonusFactor,
        float manaBonusFactor,
        IEnumerable<AtgenCharaGrowRecord> charaGrowRecord,
        IEnumerable<CharaFriendshipList> charaFriendshipList
    )
    {
        this.TakePlayerExp = takePlayerExp;
        this.TakeCharaExp = takeCharaExp;
        this.TakeMana = takeMana;
        this.BonusFactor = bonusFactor;
        this.ManaBonusFactor = manaBonusFactor;
        this.CharaGrowRecord = charaGrowRecord;
        this.CharaFriendshipList = charaFriendshipList;
    }

    public GrowRecord() { }
}

[MessagePackObject(true)]
public class GuildApplyList
{
    public ulong ViewerId { get; set; }
    public string UserName { get; set; }
    public int UserLevel { get; set; }
    public int MaxPartyPower { get; set; }
    public int ProfileEntityType { get; set; }
    public int ProfileEntityId { get; set; }
    public int ProfileEntityRarity { get; set; }
    public ulong GuildApplyId { get; set; }
    public int LastActiveTime { get; set; }

    public GuildApplyList(
        ulong viewerId,
        string userName,
        int userLevel,
        int maxPartyPower,
        int profileEntityType,
        int profileEntityId,
        int profileEntityRarity,
        ulong guildApplyId,
        int lastActiveTime
    )
    {
        this.ViewerId = viewerId;
        this.UserName = userName;
        this.UserLevel = userLevel;
        this.MaxPartyPower = maxPartyPower;
        this.ProfileEntityType = profileEntityType;
        this.ProfileEntityId = profileEntityId;
        this.ProfileEntityRarity = profileEntityRarity;
        this.GuildApplyId = guildApplyId;
        this.LastActiveTime = lastActiveTime;
    }

    public GuildApplyList() { }
}

[MessagePackObject(true)]
public class GuildChatMessageList
{
    public ulong ChatMessageId { get; set; }
    public ulong ViewerId { get; set; }
    public string UserName { get; set; }
    public int ProfileEntityType { get; set; }
    public int ProfileEntityId { get; set; }
    public int ProfileEntityRarity { get; set; }
    public int ChatMessageType { get; set; }
    public string ChatMessageText { get; set; }
    public int ChatMessageStampId { get; set; }
    public int ChatMessageSystemMessageId { get; set; }
    public int ChatMessageParamValue1 { get; set; }
    public int ChatMessageParamValue2 { get; set; }
    public int ChatMessageParamValue3 { get; set; }
    public int ChatMessageParamValue4 { get; set; }
    public int CreateTime { get; set; }

    public GuildChatMessageList(
        ulong chatMessageId,
        ulong viewerId,
        string userName,
        int profileEntityType,
        int profileEntityId,
        int profileEntityRarity,
        int chatMessageType,
        string chatMessageText,
        int chatMessageStampId,
        int chatMessageSystemMessageId,
        int chatMessageParamValue1,
        int chatMessageParamValue2,
        int chatMessageParamValue3,
        int chatMessageParamValue4,
        int createTime
    )
    {
        this.ChatMessageId = chatMessageId;
        this.ViewerId = viewerId;
        this.UserName = userName;
        this.ProfileEntityType = profileEntityType;
        this.ProfileEntityId = profileEntityId;
        this.ProfileEntityRarity = profileEntityRarity;
        this.ChatMessageType = chatMessageType;
        this.ChatMessageText = chatMessageText;
        this.ChatMessageStampId = chatMessageStampId;
        this.ChatMessageSystemMessageId = chatMessageSystemMessageId;
        this.ChatMessageParamValue1 = chatMessageParamValue1;
        this.ChatMessageParamValue2 = chatMessageParamValue2;
        this.ChatMessageParamValue3 = chatMessageParamValue3;
        this.ChatMessageParamValue4 = chatMessageParamValue4;
        this.CreateTime = createTime;
    }

    public GuildChatMessageList() { }
}

[MessagePackObject(true)]
public class GuildData
{
    public int GuildId { get; set; }
    public string GuildName { get; set; }
    public int GuildEmblemId { get; set; }
    public string GuildIntroduction { get; set; }
    public int JoiningConditionType { get; set; }
    public int ActivityPolicyType { get; set; }
    public string GuildBoard { get; set; }
    public int GuildMemberCount { get; set; }
    public int IsPenaltyGuildName { get; set; }
    public int IsPenaltyGuildIntroduction { get; set; }
    public int IsPenaltyGuildBoard { get; set; }

    public GuildData(
        int guildId,
        string guildName,
        int guildEmblemId,
        string guildIntroduction,
        int joiningConditionType,
        int activityPolicyType,
        string guildBoard,
        int guildMemberCount,
        int isPenaltyGuildName,
        int isPenaltyGuildIntroduction,
        int isPenaltyGuildBoard
    )
    {
        this.GuildId = guildId;
        this.GuildName = guildName;
        this.GuildEmblemId = guildEmblemId;
        this.GuildIntroduction = guildIntroduction;
        this.JoiningConditionType = joiningConditionType;
        this.ActivityPolicyType = activityPolicyType;
        this.GuildBoard = guildBoard;
        this.GuildMemberCount = guildMemberCount;
        this.IsPenaltyGuildName = isPenaltyGuildName;
        this.IsPenaltyGuildIntroduction = isPenaltyGuildIntroduction;
        this.IsPenaltyGuildBoard = isPenaltyGuildBoard;
    }

    public GuildData() { }
}

[MessagePackObject(true)]
public class GuildInviteReceiveList
{
    public ulong GuildInviteId { get; set; }
    public ulong SendViewerId { get; set; }
    public string SendUserName { get; set; }
    public int SendMaxPartyPower { get; set; }
    public int SendProfileEntityType { get; set; }
    public int SendProfileEntityId { get; set; }
    public int SendProfileEntityRarity { get; set; }
    public int SendLastActiveTime { get; set; }
    public int GuildInviteMessageId { get; set; }
    public GuildData GuildData { get; set; }

    public GuildInviteReceiveList(
        ulong guildInviteId,
        ulong sendViewerId,
        string sendUserName,
        int sendMaxPartyPower,
        int sendProfileEntityType,
        int sendProfileEntityId,
        int sendProfileEntityRarity,
        int sendLastActiveTime,
        int guildInviteMessageId,
        GuildData guildData
    )
    {
        this.GuildInviteId = guildInviteId;
        this.SendViewerId = sendViewerId;
        this.SendUserName = sendUserName;
        this.SendMaxPartyPower = sendMaxPartyPower;
        this.SendProfileEntityType = sendProfileEntityType;
        this.SendProfileEntityId = sendProfileEntityId;
        this.SendProfileEntityRarity = sendProfileEntityRarity;
        this.SendLastActiveTime = sendLastActiveTime;
        this.GuildInviteMessageId = guildInviteMessageId;
        this.GuildData = guildData;
    }

    public GuildInviteReceiveList() { }
}

[MessagePackObject(true)]
public class GuildInviteSendList
{
    public ulong GuildInviteId { get; set; }
    public ulong SendViewerId { get; set; }
    public string SendUserName { get; set; }
    public ulong ReceiveViewerId { get; set; }
    public string ReceiveUserName { get; set; }
    public int ReceiveUserLevel { get; set; }
    public int ReceiveMaxPartyPower { get; set; }
    public int ReceiveProfileEntityType { get; set; }
    public int ReceiveProfileEntityId { get; set; }
    public int ReceiveProfileEntityRarity { get; set; }
    public int ReceiveLastActiveTime { get; set; }
    public int GuildInviteMessageId { get; set; }
    public int LimitTime { get; set; }

    public GuildInviteSendList(
        ulong guildInviteId,
        ulong sendViewerId,
        string sendUserName,
        ulong receiveViewerId,
        string receiveUserName,
        int receiveUserLevel,
        int receiveMaxPartyPower,
        int receiveProfileEntityType,
        int receiveProfileEntityId,
        int receiveProfileEntityRarity,
        int receiveLastActiveTime,
        int guildInviteMessageId,
        int limitTime
    )
    {
        this.GuildInviteId = guildInviteId;
        this.SendViewerId = sendViewerId;
        this.SendUserName = sendUserName;
        this.ReceiveViewerId = receiveViewerId;
        this.ReceiveUserName = receiveUserName;
        this.ReceiveUserLevel = receiveUserLevel;
        this.ReceiveMaxPartyPower = receiveMaxPartyPower;
        this.ReceiveProfileEntityType = receiveProfileEntityType;
        this.ReceiveProfileEntityId = receiveProfileEntityId;
        this.ReceiveProfileEntityRarity = receiveProfileEntityRarity;
        this.ReceiveLastActiveTime = receiveLastActiveTime;
        this.GuildInviteMessageId = guildInviteMessageId;
        this.LimitTime = limitTime;
    }

    public GuildInviteSendList() { }
}

[MessagePackObject(true)]
public class GuildMemberList
{
    public ulong ViewerId { get; set; }
    public string UserName { get; set; }
    public int UserLevel { get; set; }
    public int MaxPartyPower { get; set; }
    public int ProfileEntityType { get; set; }
    public int ProfileEntityId { get; set; }
    public int ProfileEntityRarity { get; set; }
    public int LastActiveTime { get; set; }
    public int LastGuildActiveTime { get; set; }
    public int LastAttendTime { get; set; }
    public int GuildPositionType { get; set; }
    public int TemporaryEndTime { get; set; }

    public GuildMemberList(
        ulong viewerId,
        string userName,
        int userLevel,
        int maxPartyPower,
        int profileEntityType,
        int profileEntityId,
        int profileEntityRarity,
        int lastActiveTime,
        int lastGuildActiveTime,
        int lastAttendTime,
        int guildPositionType,
        int temporaryEndTime
    )
    {
        this.ViewerId = viewerId;
        this.UserName = userName;
        this.UserLevel = userLevel;
        this.MaxPartyPower = maxPartyPower;
        this.ProfileEntityType = profileEntityType;
        this.ProfileEntityId = profileEntityId;
        this.ProfileEntityRarity = profileEntityRarity;
        this.LastActiveTime = lastActiveTime;
        this.LastGuildActiveTime = lastGuildActiveTime;
        this.LastAttendTime = lastAttendTime;
        this.GuildPositionType = guildPositionType;
        this.TemporaryEndTime = temporaryEndTime;
    }

    public GuildMemberList() { }
}

[MessagePackObject(true)]
public class GuildNotice
{
    public int GuildApplyCount { get; set; }
    public int IsUpdateGuildBoard { get; set; }
    public int IsUpdateGuildApplyReply { get; set; }
    public int IsUpdateGuild { get; set; }
    public int IsUpdateGuildInvite { get; set; }

    public GuildNotice(
        int guildApplyCount,
        int isUpdateGuildBoard,
        int isUpdateGuildApplyReply,
        int isUpdateGuild,
        int isUpdateGuildInvite
    )
    {
        this.GuildApplyCount = guildApplyCount;
        this.IsUpdateGuildBoard = isUpdateGuildBoard;
        this.IsUpdateGuildApplyReply = isUpdateGuildApplyReply;
        this.IsUpdateGuild = isUpdateGuild;
        this.IsUpdateGuildInvite = isUpdateGuildInvite;
    }

    public GuildNotice() { }
}

[MessagePackObject(true)]
public class IngameData
{
    public ulong ViewerId { get; set; }
    public string DungeonKey { get; set; }
    public DungeonTypes DungeonType { get; set; }
    public QuestPlayType PlayType { get; set; }
    public int QuestId { get; set; }
    public int BonusType { get; set; }
    public int ContinueLimit { get; set; }
    public int ContinueCount { get; set; }
    public int RebornLimit { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public PartyInfo PartyInfo { get; set; }
    public IEnumerable<AreaInfoList> AreaInfoList { get; set; }
    public int UseStone { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsHost { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsFeverTime { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsBotTutorial { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsReceivableCarryBonus { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsUseEventCharaAbility { get; set; }
    public IEnumerable<EventAbilityCharaList> EventAbilityCharaList { get; set; } =
        Enumerable.Empty<EventAbilityCharaList>();
    public IEnumerable<ulong> FirstClearViewerIdList { get; set; } = Enumerable.Empty<ulong>();
    public int MultiDisconnectType { get; set; }
    public int RepeatState { get; set; }
    public AtgenIngameWalker IngameWalker { get; set; }

    public IngameData(
        ulong viewerId,
        string dungeonKey,
        DungeonTypes dungeonType,
        QuestPlayType playType,
        int questId,
        int bonusType,
        int continueLimit,
        int continueCount,
        int rebornLimit,
        DateTimeOffset startTime,
        PartyInfo partyInfo,
        IEnumerable<AreaInfoList> areaInfoList,
        int useStone,
        bool isHost,
        bool isFeverTime,
        bool isBotTutorial,
        bool isReceivableCarryBonus,
        bool isUseEventCharaAbility,
        IEnumerable<EventAbilityCharaList> eventAbilityCharaList,
        IEnumerable<ulong> firstClearViewerIdList,
        int multiDisconnectType,
        int repeatState,
        AtgenIngameWalker ingameWalker
    )
    {
        this.ViewerId = viewerId;
        this.DungeonKey = dungeonKey;
        this.DungeonType = dungeonType;
        this.PlayType = playType;
        this.QuestId = questId;
        this.BonusType = bonusType;
        this.ContinueLimit = continueLimit;
        this.ContinueCount = continueCount;
        this.RebornLimit = rebornLimit;
        this.StartTime = startTime;
        this.PartyInfo = partyInfo;
        this.AreaInfoList = areaInfoList;
        this.UseStone = useStone;
        this.IsHost = isHost;
        this.IsFeverTime = isFeverTime;
        this.IsBotTutorial = isBotTutorial;
        this.IsReceivableCarryBonus = isReceivableCarryBonus;
        this.IsUseEventCharaAbility = isUseEventCharaAbility;
        this.EventAbilityCharaList = eventAbilityCharaList;
        this.FirstClearViewerIdList = firstClearViewerIdList;
        this.MultiDisconnectType = multiDisconnectType;
        this.RepeatState = repeatState;
        this.IngameWalker = ingameWalker;
    }

    public IngameData() { }
}

[MessagePackObject(true)]
public class IngameQuestData
{
    public int QuestId { get; set; }
    public int PlayCount { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsMissionClear1 { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsMissionClear2 { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsMissionClear3 { get; set; }

    public IngameQuestData(
        int questId,
        int playCount,
        bool isMissionClear1,
        bool isMissionClear2,
        bool isMissionClear3
    )
    {
        this.QuestId = questId;
        this.PlayCount = playCount;
        this.IsMissionClear1 = isMissionClear1;
        this.IsMissionClear2 = isMissionClear2;
        this.IsMissionClear3 = isMissionClear3;
    }

    public IngameQuestData() { }
}

[MessagePackObject(true)]
public class IngameResultData
{
    public string DungeonKey { get; set; }
    public QuestPlayType PlayType { get; set; }
    public int QuestId { get; set; }
    public RewardRecord RewardRecord { get; set; } = new();
    public GrowRecord GrowRecord { get; set; } = new();
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsClear { get; set; }
    public int State { get; set; }
    public int DungeonSkipType { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsHost { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsFeverTime { get; set; }
    public int WaveCount { get; set; }
    public int CurrentPlayCount { get; set; }
    public int RebornCount { get; set; }
    public IEnumerable<PartySettingList> QuestPartySettingList { get; set; } =
        Enumerable.Empty<PartySettingList>();
    public IEnumerable<UserSupportList> HelperList { get; set; } =
        Enumerable.Empty<UserSupportList>();
    public IEnumerable<AtgenScoringEnemyPointList> ScoringEnemyPointList { get; set; } =
        Enumerable.Empty<AtgenScoringEnemyPointList>();
    public IEnumerable<AtgenHelperDetailList> HelperDetailList { get; set; } =
        Enumerable.Empty<AtgenHelperDetailList>();
    public IEnumerable<AtgenScoreMissionSuccessList> ScoreMissionSuccessList { get; set; } =
        Enumerable.Empty<AtgenScoreMissionSuccessList>();
    public IEnumerable<AtgenBonusFactorList> BonusFactorList { get; set; } =
        Enumerable.Empty<AtgenBonusFactorList>();
    public IEnumerable<AtgenEventPassiveUpList> EventPassiveUpList { get; set; } =
        Enumerable.Empty<AtgenEventPassiveUpList>();
    public float ClearTime { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsBestClearTime { get; set; }
    public long TotalPlayDamage { get; set; }
    public IEnumerable<ConvertedEntityList> ConvertedEntityList { get; set; } =
        Enumerable.Empty<ConvertedEntityList>();

    public IngameResultData(
        string dungeonKey,
        QuestPlayType playType,
        int questId,
        RewardRecord rewardRecord,
        GrowRecord growRecord,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        bool isClear,
        int state,
        int dungeonSkipType,
        bool isHost,
        bool isFeverTime,
        int waveCount,
        int currentPlayCount,
        int rebornCount,
        IEnumerable<PartySettingList> questPartySettingList,
        IEnumerable<UserSupportList> helperList,
        IEnumerable<AtgenScoringEnemyPointList> scoringEnemyPointList,
        IEnumerable<AtgenHelperDetailList> helperDetailList,
        IEnumerable<AtgenScoreMissionSuccessList> scoreMissionSuccessList,
        IEnumerable<AtgenBonusFactorList> bonusFactorList,
        IEnumerable<AtgenEventPassiveUpList> eventPassiveUpList,
        float clearTime,
        bool isBestClearTime,
        long totalPlayDamage,
        IEnumerable<ConvertedEntityList> convertedEntityList
    )
    {
        this.DungeonKey = dungeonKey;
        this.PlayType = playType;
        this.QuestId = questId;
        this.RewardRecord = rewardRecord;
        this.GrowRecord = growRecord;
        this.StartTime = startTime;
        this.EndTime = endTime;
        this.IsClear = isClear;
        this.State = state;
        this.DungeonSkipType = dungeonSkipType;
        this.IsHost = isHost;
        this.IsFeverTime = isFeverTime;
        this.WaveCount = waveCount;
        this.CurrentPlayCount = currentPlayCount;
        this.RebornCount = rebornCount;
        this.QuestPartySettingList = questPartySettingList;
        this.HelperList = helperList;
        this.ScoringEnemyPointList = scoringEnemyPointList;
        this.HelperDetailList = helperDetailList;
        this.ScoreMissionSuccessList = scoreMissionSuccessList;
        this.BonusFactorList = bonusFactorList;
        this.EventPassiveUpList = eventPassiveUpList;
        this.ClearTime = clearTime;
        this.IsBestClearTime = isBestClearTime;
        this.TotalPlayDamage = totalPlayDamage;
        this.ConvertedEntityList = convertedEntityList;
    }

    public IngameResultData() { }
}

[MessagePackObject(true)]
public class IngameWallData
{
    public int WallId { get; set; }
    public int WallLevel { get; set; }

    public IngameWallData(int wallId, int wallLevel)
    {
        this.WallId = wallId;
        this.WallLevel = wallLevel;
    }

    public IngameWallData() { }
}

[MessagePackObject(true)]
public class ItemList
{
    public UseItem ItemId { get; set; }
    public int Quantity { get; set; }

    public ItemList(UseItem itemId, int quantity)
    {
        this.ItemId = itemId;
        this.Quantity = quantity;
    }

    public ItemList() { }
}

[MessagePackObject(true)]
public class LimitBreakGrowList
{
    public int LimitBreakCount { get; set; }
    public int LimitBreakItemType { get; set; }
    public ulong TargetId { get; set; }

    public LimitBreakGrowList(int limitBreakCount, int limitBreakItemType, ulong targetId)
    {
        this.LimitBreakCount = limitBreakCount;
        this.LimitBreakItemType = limitBreakItemType;
        this.TargetId = targetId;
    }

    public LimitBreakGrowList() { }
}

[MessagePackObject(true)]
public class LotteryOddsRate
{
    public IEnumerable<AtgenLotteryPrizeRankList> LotteryPrizeRankList { get; set; }
    public IEnumerable<AtgenLotteryEntitySetList> LotteryEntitySetList { get; set; }

    public LotteryOddsRate(
        IEnumerable<AtgenLotteryPrizeRankList> lotteryPrizeRankList,
        IEnumerable<AtgenLotteryEntitySetList> lotteryEntitySetList
    )
    {
        this.LotteryPrizeRankList = lotteryPrizeRankList;
        this.LotteryEntitySetList = lotteryEntitySetList;
    }

    public LotteryOddsRate() { }
}

[MessagePackObject(true)]
public class LotteryOddsRateList
{
    public LotteryOddsRate Normal { get; set; }
    public LotteryOddsRate Guarantee { get; set; }

    public LotteryOddsRateList(LotteryOddsRate normal, LotteryOddsRate guarantee)
    {
        this.Normal = normal;
        this.Guarantee = guarantee;
    }

    public LotteryOddsRateList() { }
}

[MessagePackObject(true)]
public class LotteryTicketList
{
    public int LotteryTicketId { get; set; }
    public int Quantity { get; set; }

    public LotteryTicketList(int lotteryTicketId, int quantity)
    {
        this.LotteryTicketId = lotteryTicketId;
        this.Quantity = quantity;
    }

    public LotteryTicketList() { }
}

[MessagePackObject(true)]
public class MainStoryMissionList
{
    public int MainStoryMissionId { get; set; }
    public int Progress { get; set; }
    public int State { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset StartDate { get; set; }

    public MainStoryMissionList(
        int mainStoryMissionId,
        int progress,
        int state,
        DateTimeOffset endDate,
        DateTimeOffset startDate
    )
    {
        this.MainStoryMissionId = mainStoryMissionId;
        this.Progress = progress;
        this.State = state;
        this.EndDate = endDate;
        this.StartDate = startDate;
    }

    public MainStoryMissionList() { }
}

[MessagePackObject(true)]
public class MaterialList
{
    public Materials MaterialId { get; set; }
    public int Quantity { get; set; }

    public MaterialList(Materials materialId, int quantity)
    {
        this.MaterialId = materialId;
        this.Quantity = quantity;
    }

    public MaterialList() { }
}

[MessagePackObject(true)]
public class MazeEventUserList
{
    public int EventId { get; set; }
    public IEnumerable<AtgenUserMazeEventItemList> UserMazeEventItemList { get; set; }

    public MazeEventUserList(
        int eventId,
        IEnumerable<AtgenUserMazeEventItemList> userMazeEventItemList
    )
    {
        this.EventId = eventId;
        this.UserMazeEventItemList = userMazeEventItemList;
    }

    public MazeEventUserList() { }
}

[MessagePackObject(true)]
public class MemoryEventMissionList
{
    public int MemoryEventMissionId { get; set; }
    public int Progress { get; set; }
    public int State { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset StartDate { get; set; }

    public MemoryEventMissionList(
        int memoryEventMissionId,
        int progress,
        int state,
        DateTimeOffset endDate,
        DateTimeOffset startDate
    )
    {
        this.MemoryEventMissionId = memoryEventMissionId;
        this.Progress = progress;
        this.State = state;
        this.EndDate = endDate;
        this.StartDate = startDate;
    }

    public MemoryEventMissionList() { }
}

[MessagePackObject(true)]
public class MissionNotice
{
    public AtgenNormalMissionNotice NormalMissionNotice { get; set; }
    public AtgenNormalMissionNotice DailyMissionNotice { get; set; }
    public AtgenNormalMissionNotice PeriodMissionNotice { get; set; }
    public AtgenNormalMissionNotice BeginnerMissionNotice { get; set; }
    public AtgenNormalMissionNotice SpecialMissionNotice { get; set; }
    public AtgenNormalMissionNotice MainStoryMissionNotice { get; set; }
    public AtgenNormalMissionNotice MemoryEventMissionNotice { get; set; }
    public AtgenNormalMissionNotice DrillMissionNotice { get; set; }
    public AtgenNormalMissionNotice AlbumMissionNotice { get; set; }

    public MissionNotice(
        AtgenNormalMissionNotice normalMissionNotice,
        AtgenNormalMissionNotice dailyMissionNotice,
        AtgenNormalMissionNotice periodMissionNotice,
        AtgenNormalMissionNotice beginnerMissionNotice,
        AtgenNormalMissionNotice specialMissionNotice,
        AtgenNormalMissionNotice mainStoryMissionNotice,
        AtgenNormalMissionNotice memoryEventMissionNotice,
        AtgenNormalMissionNotice drillMissionNotice,
        AtgenNormalMissionNotice albumMissionNotice
    )
    {
        this.NormalMissionNotice = normalMissionNotice;
        this.DailyMissionNotice = dailyMissionNotice;
        this.PeriodMissionNotice = periodMissionNotice;
        this.BeginnerMissionNotice = beginnerMissionNotice;
        this.SpecialMissionNotice = specialMissionNotice;
        this.MainStoryMissionNotice = mainStoryMissionNotice;
        this.MemoryEventMissionNotice = memoryEventMissionNotice;
        this.DrillMissionNotice = drillMissionNotice;
        this.AlbumMissionNotice = albumMissionNotice;
    }

    public MissionNotice() { }
}

[MessagePackObject(true)]
public class MuseumDragonList
{
    public int State { get; set; }
    public int DragonId { get; set; }

    public MuseumDragonList(int state, int dragonId)
    {
        this.State = state;
        this.DragonId = dragonId;
    }

    public MuseumDragonList() { }
}

[MessagePackObject(true)]
public class MuseumList
{
    public int State { get; set; }
    public int CharaId { get; set; }

    public MuseumList(int state, int charaId)
    {
        this.State = state;
        this.CharaId = charaId;
    }

    public MuseumList() { }
}

[MessagePackObject(true)]
public class NormalMissionList
{
    public int NormalMissionId { get; set; }
    public int Progress { get; set; }
    public int State { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset StartDate { get; set; }

    public NormalMissionList(
        int normalMissionId,
        int progress,
        int state,
        DateTimeOffset endDate,
        DateTimeOffset startDate
    )
    {
        this.NormalMissionId = normalMissionId;
        this.Progress = progress;
        this.State = state;
        this.EndDate = endDate;
        this.StartDate = startDate;
    }

    public NormalMissionList() { }
}

[MessagePackObject(true)]
public class OddsInfo
{
    public int AreaIndex { get; set; }
    public int ReactionObjCount { get; set; }
    public IEnumerable<AtgenDropObj> DropObj { get; set; }
    public IEnumerable<AtgenEnemy> Enemy { get; set; }
    public IEnumerable<AtgenGrade> Grade { get; set; }

    public OddsInfo(
        int areaIndex,
        int reactionObjCount,
        IEnumerable<AtgenDropObj> dropObj,
        IEnumerable<AtgenEnemy> enemy,
        IEnumerable<AtgenGrade> grade
    )
    {
        this.AreaIndex = areaIndex;
        this.ReactionObjCount = reactionObjCount;
        this.DropObj = dropObj;
        this.Enemy = enemy;
        this.Grade = grade;
    }

    public OddsInfo() { }
}

[MessagePackObject(true)]
public class OddsRate
{
    public IEnumerable<AtgenRarityList> RarityList { get; set; }
    public IEnumerable<AtgenRarityGroupList> RarityGroupList { get; set; }
    public AtgenUnit Unit { get; set; }

    public OddsRate(
        IEnumerable<AtgenRarityList> rarityList,
        IEnumerable<AtgenRarityGroupList> rarityGroupList,
        AtgenUnit unit
    )
    {
        this.RarityList = rarityList;
        this.RarityGroupList = rarityGroupList;
        this.Unit = unit;
    }

    public OddsRate() { }
}

[MessagePackObject(true)]
public class OddsRateList
{
    public int RequiredCountToNext { get; set; }
    public OddsRate Normal { get; set; }
    public OddsRate Guarantee { get; set; }

    public OddsRateList(int requiredCountToNext, OddsRate normal, OddsRate guarantee)
    {
        this.RequiredCountToNext = requiredCountToNext;
        this.Normal = normal;
        this.Guarantee = guarantee;
    }

    public OddsRateList() { }
}

[MessagePackObject(true)]
public class OddsUnitDetail
{
    public bool Pickup { get; set; }
    public int Rarity { get; set; }
    public IEnumerable<AtgenUnitList> UnitList { get; set; }

    public OddsUnitDetail(bool pickup, int rarity, IEnumerable<AtgenUnitList> unitList)
    {
        this.Pickup = pickup;
        this.Rarity = rarity;
        this.UnitList = unitList;
    }

    public OddsUnitDetail() { }
}

[MessagePackObject(true)]
public class OptionData
{
    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsEnableAutoLockUnit { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsAutoLockDragonSr { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsAutoLockDragonSsr { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsAutoLockWeaponSr { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsAutoLockWeaponSsr { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsAutoLockWeaponSssr { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsAutoLockAmuletSr { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsAutoLockAmuletSsr { get; set; }

    public OptionData(
        bool isEnableAutoLockUnit,
        bool isAutoLockDragonSr,
        bool isAutoLockDragonSsr,
        bool isAutoLockWeaponSr,
        bool isAutoLockWeaponSsr,
        bool isAutoLockWeaponSssr,
        bool isAutoLockAmuletSr,
        bool isAutoLockAmuletSsr
    )
    {
        this.IsEnableAutoLockUnit = isEnableAutoLockUnit;
        this.IsAutoLockDragonSr = isAutoLockDragonSr;
        this.IsAutoLockDragonSsr = isAutoLockDragonSsr;
        this.IsAutoLockWeaponSr = isAutoLockWeaponSr;
        this.IsAutoLockWeaponSsr = isAutoLockWeaponSsr;
        this.IsAutoLockWeaponSssr = isAutoLockWeaponSssr;
        this.IsAutoLockAmuletSr = isAutoLockAmuletSr;
        this.IsAutoLockAmuletSsr = isAutoLockAmuletSsr;
    }

    public OptionData() { }
}

[MessagePackObject(true)]
public class PartyInfo
{
    public IEnumerable<PartyUnitList> PartyUnitList { get; set; }
    public FortBonusList FortBonusList { get; set; }
    public AtgenEventBoost EventBoost { get; set; }
    public AtgenSupportData SupportData { get; set; }
    public IEnumerable<AtgenEventPassiveUpList> EventPassiveGrowList { get; set; }

    public PartyInfo(
        IEnumerable<PartyUnitList> partyUnitList,
        FortBonusList fortBonusList,
        AtgenEventBoost eventBoost,
        AtgenSupportData supportData,
        IEnumerable<AtgenEventPassiveUpList> eventPassiveGrowList
    )
    {
        this.PartyUnitList = partyUnitList;
        this.FortBonusList = fortBonusList;
        this.EventBoost = eventBoost;
        this.SupportData = supportData;
        this.EventPassiveGrowList = eventPassiveGrowList;
    }

    public PartyInfo() { }
}

[MessagePackObject(true)]
public class PartyList
{
    public int PartyNo { get; set; }

    [MaxLength(20)]
    public string PartyName { get; set; }
    public IEnumerable<PartySettingList> PartySettingList { get; set; }

    public PartyList(int partyNo, string partyName, IEnumerable<PartySettingList> partySettingList)
    {
        this.PartyNo = partyNo;
        this.PartyName = partyName;
        this.PartySettingList = partySettingList;
    }

    public PartyList() { }
}

[MessagePackObject(true)]
public class PartyPowerData
{
    public int MaxPartyPower { get; set; }

    public PartyPowerData(int maxPartyPower)
    {
        this.MaxPartyPower = maxPartyPower;
    }

    public PartyPowerData() { }
}

[MessagePackObject(true)]
public class PartySettingList
{
    public int UnitNo { get; set; }
    public Charas CharaId { get; set; }
    public ulong EquipWeaponKeyId { get; set; }
    public ulong EquipDragonKeyId { get; set; }
    public ulong EquipAmuletKeyId { get; set; }
    public ulong EquipAmulet2KeyId { get; set; }
    public int EquipSkinWeaponId { get; set; }
    public WeaponBodies EquipWeaponBodyId { get; set; }
    public int EquipWeaponSkinId { get; set; }
    public AbilityCrests EquipCrestSlotType1CrestId1 { get; set; }
    public AbilityCrests EquipCrestSlotType1CrestId2 { get; set; }
    public AbilityCrests EquipCrestSlotType1CrestId3 { get; set; }
    public AbilityCrests EquipCrestSlotType2CrestId1 { get; set; }
    public AbilityCrests EquipCrestSlotType2CrestId2 { get; set; }
    public AbilityCrests EquipCrestSlotType3CrestId1 { get; set; }
    public AbilityCrests EquipCrestSlotType3CrestId2 { get; set; }
    public ulong EquipTalismanKeyId { get; set; }
    public Charas EditSkill1CharaId { get; set; }
    public Charas EditSkill2CharaId { get; set; }

    public PartySettingList(
        int unitNo,
        Charas charaId,
        ulong equipWeaponKeyId,
        ulong equipDragonKeyId,
        ulong equipAmuletKeyId,
        ulong equipAmulet2KeyId,
        int equipSkinWeaponId,
        WeaponBodies equipWeaponBodyId,
        int equipWeaponSkinId,
        AbilityCrests equipCrestSlotType1CrestId1,
        AbilityCrests equipCrestSlotType1CrestId2,
        AbilityCrests equipCrestSlotType1CrestId3,
        AbilityCrests equipCrestSlotType2CrestId1,
        AbilityCrests equipCrestSlotType2CrestId2,
        AbilityCrests equipCrestSlotType3CrestId1,
        AbilityCrests equipCrestSlotType3CrestId2,
        ulong equipTalismanKeyId,
        Charas editSkill1CharaId,
        Charas editSkill2CharaId
    )
    {
        this.UnitNo = unitNo;
        this.CharaId = charaId;
        this.EquipWeaponKeyId = equipWeaponKeyId;
        this.EquipDragonKeyId = equipDragonKeyId;
        this.EquipAmuletKeyId = equipAmuletKeyId;
        this.EquipAmulet2KeyId = equipAmulet2KeyId;
        this.EquipSkinWeaponId = equipSkinWeaponId;
        this.EquipWeaponBodyId = equipWeaponBodyId;
        this.EquipWeaponSkinId = equipWeaponSkinId;
        this.EquipCrestSlotType1CrestId1 = equipCrestSlotType1CrestId1;
        this.EquipCrestSlotType1CrestId2 = equipCrestSlotType1CrestId2;
        this.EquipCrestSlotType1CrestId3 = equipCrestSlotType1CrestId3;
        this.EquipCrestSlotType2CrestId1 = equipCrestSlotType2CrestId1;
        this.EquipCrestSlotType2CrestId2 = equipCrestSlotType2CrestId2;
        this.EquipCrestSlotType3CrestId1 = equipCrestSlotType3CrestId1;
        this.EquipCrestSlotType3CrestId2 = equipCrestSlotType3CrestId2;
        this.EquipTalismanKeyId = equipTalismanKeyId;
        this.EditSkill1CharaId = editSkill1CharaId;
        this.EditSkill2CharaId = editSkill2CharaId;
    }

    public PartySettingList() { }

    public static PartySettingList Empty(int unitNo) => new() { UnitNo = unitNo };

    public IEnumerable<AbilityCrests> GetAbilityCrestList() =>
        new List<AbilityCrests>()
        {
            this.EquipCrestSlotType1CrestId1,
            this.EquipCrestSlotType1CrestId2,
            this.EquipCrestSlotType1CrestId3,
            this.EquipCrestSlotType2CrestId1,
            this.EquipCrestSlotType2CrestId2,
            this.EquipCrestSlotType3CrestId1,
            this.EquipCrestSlotType3CrestId2
        };
}

#nullable enable

[MessagePackObject(true)]
public class PartyUnitList
{
    public int Position { get; set; }
    public CharaList? CharaData { get; set; } = new();
    public DragonList? DragonData { get; set; } = new();
    public GameWeaponSkin? WeaponSkinData { get; set; } = new();
    public GameWeaponBody? WeaponBodyData { get; set; } = new();
    public IEnumerable<GameAbilityCrest> CrestSlotType1CrestList { get; set; } =
        new List<GameAbilityCrest>();
    public IEnumerable<GameAbilityCrest> CrestSlotType2CrestList { get; set; } =
        new List<GameAbilityCrest>();
    public IEnumerable<GameAbilityCrest> CrestSlotType3CrestList { get; set; } =
        new List<GameAbilityCrest>();
    public TalismanList? TalismanData { get; set; } = new();
    public EditSkillCharaData? EditSkill1CharaData { get; set; } = new();
    public EditSkillCharaData? EditSkill2CharaData { get; set; } = new();
    public IEnumerable<WeaponPassiveAbilityList> GameWeaponPassiveAbilityList { get; set; } =
        new List<WeaponPassiveAbilityList>();
    public int DragonReliabilityLevel { get; set; } = 0;

    public PartyUnitList(
        int position,
        CharaList charaData,
        DragonList dragonData,
        GameWeaponSkin weaponSkinData,
        GameWeaponBody weaponBodyData,
        IEnumerable<GameAbilityCrest> crestSlotType1CrestList,
        IEnumerable<GameAbilityCrest> crestSlotType2CrestList,
        IEnumerable<GameAbilityCrest> crestSlotType3CrestList,
        TalismanList talismanData,
        EditSkillCharaData editSkill1CharaData,
        EditSkillCharaData editSkill2CharaData,
        IEnumerable<WeaponPassiveAbilityList> gameWeaponPassiveAbilityList,
        int dragonReliabilityLevel
    )
    {
        this.Position = position;
        this.CharaData = charaData;
        this.DragonData = dragonData;
        this.WeaponSkinData = weaponSkinData;
        this.WeaponBodyData = weaponBodyData;
        this.CrestSlotType1CrestList = crestSlotType1CrestList;
        this.CrestSlotType2CrestList = crestSlotType2CrestList;
        this.CrestSlotType3CrestList = crestSlotType3CrestList;
        this.TalismanData = talismanData;
        this.EditSkill1CharaData = editSkill1CharaData;
        this.EditSkill2CharaData = editSkill2CharaData;
        this.GameWeaponPassiveAbilityList = gameWeaponPassiveAbilityList;
        this.DragonReliabilityLevel = dragonReliabilityLevel;
    }

    public PartyUnitList() { }
}

#nullable disable

/// <summary>
/// Contains the cost of the summon and the amount held of the relevant currency
/// Mostly likely only relevant for Diamantium
/// </summary>
/// <param name="target_hold_quantity">Total relevant currency held</param>
/// <param name="target_cost">Relevant currency cost</param>
[MessagePackObject(true)]
public class PaymentTarget
{
    public int TargetHoldQuantity { get; set; }
    public int TargetCost { get; set; }

    public PaymentTarget(int targetHoldQuantity, int targetCost)
    {
        this.TargetHoldQuantity = targetHoldQuantity;
        this.TargetCost = targetCost;
    }

    public PaymentTarget() { }
}

[MessagePackObject(true)]
public class PeriodMissionList
{
    public int PeriodMissionId { get; set; }
    public int Progress { get; set; }
    public int State { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset StartDate { get; set; }

    public PeriodMissionList(
        int periodMissionId,
        int progress,
        int state,
        DateTimeOffset endDate,
        DateTimeOffset startDate
    )
    {
        this.PeriodMissionId = periodMissionId;
        this.Progress = progress;
        this.State = state;
        this.EndDate = endDate;
        this.StartDate = startDate;
    }

    public PeriodMissionList() { }
}

[MessagePackObject(true)]
public class PlayRecord
{
    public IEnumerable<AtgenTreasureRecord> TreasureRecord { get; set; }
    public float Time { get; set; }
    public int DownCount { get; set; }
    public int TrapCount { get; set; }
    public int BadStatus { get; set; }
    public int DragonPillarCount { get; set; }
    public int DragonTransformCount { get; set; }
    public int DamageCount { get; set; }
    public int SkillCount { get; set; }
    public int GuardBrokenCount { get; set; }
    public int BreakCount { get; set; }
    public int GiveDamage { get; set; }
    public int MaxComboCount { get; set; }
    public int IsClear { get; set; }
    public int ClearState { get; set; }
    public int Wave { get; set; }
    public int ReactionTouchCnt { get; set; }
    public int GradePoint { get; set; }
    public int RebornCount { get; set; }
    public int VisitPrivateHouse { get; set; }
    public int ProtectionDamage { get; set; }
    public int RemainingTime { get; set; }
    public int LowerDrawbridgeCount { get; set; }
    public IEnumerable<int> LiveUnitNoList { get; set; }
    public long TotalPlayDamage { get; set; }
    public IEnumerable<AtgenDamageRecord> DamageRecord { get; set; }
    public IEnumerable<AtgenDamageRecord> DragonDamageRecord { get; set; }
    public AtgenBattleRoyalRecord BattleRoyalRecord { get; set; }
    public int MaxDamage { get; set; }
    public int MaxCriticalDamage { get; set; }
    public int Dps { get; set; }
    public int PlayContinueCount { get; set; }

    public PlayRecord(
        IEnumerable<AtgenTreasureRecord> treasureRecord,
        float time,
        int downCount,
        int trapCount,
        int badStatus,
        int dragonPillarCount,
        int dragonTransformCount,
        int damageCount,
        int skillCount,
        int guardBrokenCount,
        int breakCount,
        int giveDamage,
        int maxComboCount,
        int isClear,
        int clearState,
        int wave,
        int reactionTouchCnt,
        int gradePoint,
        int rebornCount,
        int visitPrivateHouse,
        int protectionDamage,
        int remainingTime,
        int lowerDrawbridgeCount,
        IEnumerable<int> liveUnitNoList,
        long totalPlayDamage,
        IEnumerable<AtgenDamageRecord> damageRecord,
        IEnumerable<AtgenDamageRecord> dragonDamageRecord,
        AtgenBattleRoyalRecord battleRoyalRecord,
        int maxDamage,
        int maxCriticalDamage,
        int dps,
        int playContinueCount
    )
    {
        this.TreasureRecord = treasureRecord;
        this.Time = time;
        this.DownCount = downCount;
        this.TrapCount = trapCount;
        this.BadStatus = badStatus;
        this.DragonPillarCount = dragonPillarCount;
        this.DragonTransformCount = dragonTransformCount;
        this.DamageCount = damageCount;
        this.SkillCount = skillCount;
        this.GuardBrokenCount = guardBrokenCount;
        this.BreakCount = breakCount;
        this.GiveDamage = giveDamage;
        this.MaxComboCount = maxComboCount;
        this.IsClear = isClear;
        this.ClearState = clearState;
        this.Wave = wave;
        this.ReactionTouchCnt = reactionTouchCnt;
        this.GradePoint = gradePoint;
        this.RebornCount = rebornCount;
        this.VisitPrivateHouse = visitPrivateHouse;
        this.ProtectionDamage = protectionDamage;
        this.RemainingTime = remainingTime;
        this.LowerDrawbridgeCount = lowerDrawbridgeCount;
        this.LiveUnitNoList = liveUnitNoList;
        this.TotalPlayDamage = totalPlayDamage;
        this.DamageRecord = damageRecord;
        this.DragonDamageRecord = dragonDamageRecord;
        this.BattleRoyalRecord = battleRoyalRecord;
        this.MaxDamage = maxDamage;
        this.MaxCriticalDamage = maxCriticalDamage;
        this.Dps = dps;
        this.PlayContinueCount = playContinueCount;
    }

    public PlayRecord() { }
}

[MessagePackObject(true)]
public class PresentDetailList
{
    public ulong PresentId { get; set; }
    public int MasterId { get; set; }
    public int State { get; set; }
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }
    public int EntityQuantity { get; set; }
    public int EntityLevel { get; set; }
    public int EntityLimitBreakCount { get; set; }
    public int EntityStatusPlusCount { get; set; }
    public PresentMessage MessageId { get; set; }
    public int MessageParamValue1 { get; set; }
    public int MessageParamValue2 { get; set; }
    public int MessageParamValue3 { get; set; }
    public int MessageParamValue4 { get; set; }
    public int ExtraParameter1 { get; set; }
    public int ExtraParameter2 { get; set; }
    public int ExtraParameter3 { get; set; }
    public int ExtraParameter4 { get; set; }
    public int ExtraParameter5 { get; set; }
    public DateTimeOffset ReceiveLimitTime { get; set; }
    public DateTimeOffset CreateTime { get; set; }

    // UNKNOWN
    /// <summary>
    /// Response type for present data re0presentation
    /// </summary>
    /// <param name="presentId">duh</param>
    /// <param name="masterId">Literally no idea</param>
    /// <param name="state">Literally no idea</param>
    /// <param name="entityType">Entity type as found in <seealso cref="EntityTypes"/></param>
    /// <param name="entityId">duh</param>
    /// <param name="entityQuantity">Amount if entity is quantifiable</param>
    /// <param name="entityLevel">Level if entity is chara or dragon</param>
    /// <param name="entityLimitBreakCount">LB count if entity is chara or dragon</param>
    /// <param name="entityStatusPlusCount">Probably augments for chara or dragon entities</param>
    /// <param name="messageId">Something about message?</param>
    /// <param name="messageParamValue1">params about displayed message</param>
    /// <param name="messageParamValue2">params about displayed message</param>
    /// <param name="messageParamValue3">params about displayed message</param>
    /// <param name="messageParamValue4">params about displayed message</param>
    /// <param name="extraParameter1">literally no idea</param>
    /// <param name="extraParameter2"></param>
    /// <param name="extraParameter3"></param>
    /// <param name="extraParameter4"></param>
    /// <param name="extraParameter5"></param>
    /// <param name="receiveLimitTime">Receivable until (probably just for the client to show)</param>
    /// <param name="createTime">Creation time of the present</param>
    public PresentDetailList(
        ulong presentId,
        int masterId,
        int state,
        EntityTypes entityType,
        int entityId,
        int entityQuantity,
        int entityLevel,
        int entityLimitBreakCount,
        int entityStatusPlusCount,
        PresentMessage messageId,
        int messageParamValue1,
        int messageParamValue2,
        int messageParamValue3,
        int messageParamValue4,
        int extraParameter1,
        int extraParameter2,
        int extraParameter3,
        int extraParameter4,
        int extraParameter5,
        DateTimeOffset receiveLimitTime,
        DateTimeOffset createTime
    )
    {
        this.PresentId = presentId;
        this.MasterId = masterId;
        this.State = state;
        this.EntityType = entityType;
        this.EntityId = entityId;
        this.EntityQuantity = entityQuantity;
        this.EntityLevel = entityLevel;
        this.EntityLimitBreakCount = entityLimitBreakCount;
        this.EntityStatusPlusCount = entityStatusPlusCount;
        this.MessageId = messageId;
        this.MessageParamValue1 = messageParamValue1;
        this.MessageParamValue2 = messageParamValue2;
        this.MessageParamValue3 = messageParamValue3;
        this.MessageParamValue4 = messageParamValue4;
        this.ExtraParameter1 = extraParameter1;
        this.ExtraParameter2 = extraParameter2;
        this.ExtraParameter3 = extraParameter3;
        this.ExtraParameter4 = extraParameter4;
        this.ExtraParameter1 = extraParameter5;
        this.ReceiveLimitTime = receiveLimitTime;
        this.CreateTime = createTime;
    }

    public PresentDetailList() { }
}

[MessagePackObject(true)]
public class PresentHistoryList
{
    public ulong Id { get; set; }
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }
    public int EntityQuantity { get; set; }
    public int EntityLevel { get; set; }
    public int EntityLimitBreakCount { get; set; }
    public int EntityStatusPlusCount { get; set; }
    public int MessageId { get; set; }
    public int MessageParamValue1 { get; set; }
    public int MessageParamValue2 { get; set; }
    public int MessageParamValue3 { get; set; }
    public int MessageParamValue4 { get; set; }

    public DateTimeOffset CreateTime { get; set; }

    public PresentHistoryList(
        ulong id,
        EntityTypes entityType,
        int entityId,
        int entityQuantity,
        int entityLevel,
        int entityLimitBreakCount,
        int entityStatusPlusCount,
        int messageId,
        int messageParamValue1,
        int messageParamValue2,
        int messageParamValue3,
        int messageParamValue4,
        DateTimeOffset createTime
    )
    {
        this.Id = id;
        this.EntityType = entityType;
        this.EntityId = entityId;
        this.EntityQuantity = entityQuantity;
        this.EntityLevel = entityLevel;
        this.EntityLimitBreakCount = entityLimitBreakCount;
        this.EntityStatusPlusCount = entityStatusPlusCount;
        this.MessageId = messageId;
        this.MessageParamValue1 = messageParamValue1;
        this.MessageParamValue2 = messageParamValue2;
        this.MessageParamValue3 = messageParamValue3;
        this.MessageParamValue4 = messageParamValue4;
        this.CreateTime = createTime;
    }

    public PresentHistoryList() { }
}

[MessagePackObject(true)]
public class PresentNotice
{
    public int PresentLimitCount { get; set; }
    public int PresentCount { get; set; }

    public PresentNotice(int presentLimitCount, int presentCount)
    {
        this.PresentLimitCount = presentLimitCount;
        this.PresentCount = presentCount;
    }

    public PresentNotice() { }
}

[MessagePackObject(true)]
public class ProductList
{
    public int Id { get; set; }
    public string Sku { get; set; }
    public int PaidDiamondQuantity { get; set; }
    public int FreeDiamondQuantity { get; set; }
    public int Price { get; set; }

    public ProductList(
        int id,
        string sku,
        int paidDiamondQuantity,
        int freeDiamondQuantity,
        int price
    )
    {
        this.Id = id;
        this.Sku = sku;
        this.PaidDiamondQuantity = paidDiamondQuantity;
        this.FreeDiamondQuantity = freeDiamondQuantity;
        this.Price = price;
    }

    public ProductList() { }
}

[MessagePackObject(true)]
public class QuestCarryList
{
    public int QuestCarryId { get; set; }
    public int ReceiveCount { get; set; }
    public int LastReceiveTime { get; set; }

    public QuestCarryList(int questCarryId, int receiveCount, int lastReceiveTime)
    {
        this.QuestCarryId = questCarryId;
        this.ReceiveCount = receiveCount;
        this.LastReceiveTime = lastReceiveTime;
    }

    public QuestCarryList() { }
}

[MessagePackObject(true)]
public class QuestEntryConditionList
{
    public int QuestEntryConditionId { get; set; }

    public QuestEntryConditionList(int questEntryConditionId)
    {
        this.QuestEntryConditionId = questEntryConditionId;
    }

    public QuestEntryConditionList() { }
}

[MessagePackObject(true)]
public class QuestEventList
{
    public int QuestEventId { get; set; }
    public int DailyPlayCount { get; set; }
    public int WeeklyPlayCount { get; set; }
    public int QuestBonusReceiveCount { get; set; }
    public int QuestBonusStackCount { get; set; }
    public DateTimeOffset QuestBonusStackTime { get; set; }
    public int QuestBonusReserveCount { get; set; }
    public DateTimeOffset QuestBonusReserveTime { get; set; }
    public DateTimeOffset LastDailyResetTime { get; set; }
    public DateTimeOffset LastWeeklyResetTime { get; set; }

    public QuestEventList(
        int questEventId,
        int dailyPlayCount,
        int weeklyPlayCount,
        int questBonusReceiveCount,
        int questBonusStackCount,
        DateTimeOffset questBonusStackTime,
        int questBonusReserveCount,
        DateTimeOffset questBonusReserveTime,
        DateTimeOffset lastDailyResetTime,
        DateTimeOffset lastWeeklyResetTime
    )
    {
        this.QuestEventId = questEventId;
        this.DailyPlayCount = dailyPlayCount;
        this.WeeklyPlayCount = weeklyPlayCount;
        this.QuestBonusReceiveCount = questBonusReceiveCount;
        this.QuestBonusStackCount = questBonusStackCount;
        this.QuestBonusStackTime = questBonusStackTime;
        this.QuestBonusReserveCount = questBonusReserveCount;
        this.QuestBonusReserveTime = questBonusReserveTime;
        this.LastDailyResetTime = lastDailyResetTime;
        this.LastWeeklyResetTime = lastWeeklyResetTime;
    }

    public QuestEventList() { }
}

[MessagePackObject(true)]
public class QuestEventScheduleList
{
    public int QuestGroupId { get; set; }
    public int EventScheduleType { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public QuestGroupIntervalType IntervalType { get; set; }
    public string FeverTimeStart1 { get; set; }
    public string FeverTimeEnd1 { get; set; }
    public string FeverTimeStart2 { get; set; }
    public string FeverTimeEnd2 { get; set; }
    public string FeverTimeStart3 { get; set; }
    public string FeverTimeEnd3 { get; set; }

    public QuestEventScheduleList(
        int questGroupId,
        int eventScheduleType,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        QuestGroupIntervalType intervalType,
        string feverTimeStart1,
        string feverTimeEnd1,
        string feverTimeStart2,
        string feverTimeEnd2,
        string feverTimeStart3,
        string feverTimeEnd3
    )
    {
        this.QuestGroupId = questGroupId;
        this.EventScheduleType = eventScheduleType;
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.IntervalType = intervalType;
        this.FeverTimeStart1 = feverTimeStart1;
        this.FeverTimeEnd1 = feverTimeEnd1;
        this.FeverTimeStart2 = feverTimeStart2;
        this.FeverTimeEnd2 = feverTimeEnd2;
        this.FeverTimeStart3 = feverTimeStart3;
        this.FeverTimeEnd3 = feverTimeEnd3;
    }

    public QuestEventScheduleList() { }
}

[MessagePackObject(true)]
public class QuestList
{
    public int QuestId { get; set; }
    public int State { get; set; }
    public int IsMissionClear1 { get; set; }
    public int IsMissionClear2 { get; set; }
    public int IsMissionClear3 { get; set; }
    public int DailyPlayCount { get; set; }
    public int WeeklyPlayCount { get; set; }
    public int PlayCount { get; set; }
    public DateTimeOffset LastDailyResetTime { get; set; }
    public DateTimeOffset LastWeeklyResetTime { get; set; }
    public int IsAppear { get; set; }
    public float BestClearTime { get; set; }

    public QuestList(
        int questId,
        int state,
        int isMissionClear1,
        int isMissionClear2,
        int isMissionClear3,
        int dailyPlayCount,
        int weeklyPlayCount,
        int playCount,
        DateTimeOffset lastDailyResetTime,
        DateTimeOffset lastWeeklyResetTime,
        int isAppear,
        float bestClearTime
    )
    {
        this.QuestId = questId;
        this.State = state;
        this.IsMissionClear1 = isMissionClear1;
        this.IsMissionClear2 = isMissionClear2;
        this.IsMissionClear3 = isMissionClear3;
        this.DailyPlayCount = dailyPlayCount;
        this.WeeklyPlayCount = weeklyPlayCount;
        this.PlayCount = playCount;
        this.LastDailyResetTime = lastDailyResetTime;
        this.LastWeeklyResetTime = lastWeeklyResetTime;
        this.IsAppear = isAppear;
        this.BestClearTime = bestClearTime;
    }

    public QuestList() { }
}

[MessagePackObject(true)]
public class QuestScheduleDetailList
{
    public int ScheduleDetailId { get; set; }
    public int ScheduleGroupId { get; set; }
    public int DropBonusPercent { get; set; }
    public int LimitShopGoodsType { get; set; }
    public QuestGroupIntervalType IntervalType { get; set; }

    [JsonConverter(typeof(DateTimeUnixJsonConverter))]
    public DateTimeOffset StartDate { get; set; }

    [JsonConverter(typeof(DateTimeUnixJsonConverter))]
    public DateTimeOffset EndDate { get; set; }

    public QuestScheduleDetailList(
        int scheduleDetailId,
        int scheduleGroupId,
        int dropBonusPercent,
        int limitShopGoodsType,
        QuestGroupIntervalType intervalType,
        DateTimeOffset startDate,
        DateTimeOffset endDate
    )
    {
        this.ScheduleDetailId = scheduleDetailId;
        this.ScheduleGroupId = scheduleGroupId;
        this.DropBonusPercent = dropBonusPercent;
        this.LimitShopGoodsType = limitShopGoodsType;
        this.IntervalType = intervalType;
        this.StartDate = startDate;
        this.EndDate = endDate;
    }

    public QuestScheduleDetailList() { }
}

[MessagePackObject(true)]
public class QuestStoryList
{
    public int QuestStoryId { get; set; }
    public int State { get; set; }

    public QuestStoryList(int questStoryId, int state)
    {
        this.QuestStoryId = questStoryId;
        this.State = state;
    }

    public QuestStoryList() { }
}

[MessagePackObject(true)]
public class QuestTreasureList
{
    public int QuestTreasureId { get; set; }

    public QuestTreasureList(int questTreasureId)
    {
        this.QuestTreasureId = questTreasureId;
    }

    public QuestTreasureList() { }
}

[MessagePackObject(true)]
public class QuestWallList
{
    public int WallId { get; set; }
    public int WallLevel { get; set; }
    public int IsStartNextLevel { get; set; }

    public QuestWallList(int wallId, int wallLevel, int isStartNextLevel)
    {
        this.WallId = wallId;
        this.WallLevel = wallLevel;
        this.IsStartNextLevel = isStartNextLevel;
    }

    public QuestWallList() { }
}

[MessagePackObject(true)]
public class RaidEventRewardList : IEventRewardList<RaidEventRewardList>
{
    public int RaidEventId { get; set; }
    public int RaidEventRewardId { get; set; }

    public RaidEventRewardList(int raidEventId, int raidEventRewardId)
    {
        this.RaidEventId = raidEventId;
        this.RaidEventRewardId = raidEventRewardId;
    }

    public static RaidEventRewardList FromDatabase(DbPlayerEventReward reward) =>
        new(reward.EventId, reward.RewardId);

    public RaidEventRewardList() { }
}

[MessagePackObject(true)]
public class RaidEventUserList
{
    public int RaidEventId { get; set; }
    public int BoxSummonPoint { get; set; }
    public int RaidPoint1 { get; set; }
    public int RaidPoint2 { get; set; }
    public int RaidPoint3 { get; set; }
    public int AdventItemQuantity1 { get; set; }
    public int AdventItemQuantity2 { get; set; }
    public int UltimateKeyCount { get; set; }
    public int ExchangeItemCount { get; set; }
    public int ExchangeItemCount2 { get; set; }

    public RaidEventUserList(
        int raidEventId,
        int boxSummonPoint,
        int raidPoint1,
        int raidPoint2,
        int raidPoint3,
        int adventItemQuantity1,
        int adventItemQuantity2,
        int ultimateKeyCount,
        int exchangeItemCount,
        int exchangeItemCount2
    )
    {
        this.RaidEventId = raidEventId;
        this.BoxSummonPoint = boxSummonPoint;
        this.RaidPoint1 = raidPoint1;
        this.RaidPoint2 = raidPoint2;
        this.RaidPoint3 = raidPoint3;
        this.AdventItemQuantity1 = adventItemQuantity1;
        this.AdventItemQuantity2 = adventItemQuantity2;
        this.UltimateKeyCount = ultimateKeyCount;
        this.ExchangeItemCount = exchangeItemCount;
        this.ExchangeItemCount2 = exchangeItemCount2;
    }

    public RaidEventUserList() { }
}

[MessagePackObject(true)]
public class RankingTierRewardList
{
    public int RankingGroupId { get; set; }
    public int TierRewardId { get; set; }

    public RankingTierRewardList(int rankingGroupId, int tierRewardId)
    {
        this.RankingGroupId = rankingGroupId;
        this.TierRewardId = tierRewardId;
    }

    public RankingTierRewardList() { }
}

[MessagePackObject(true)]
public class RedoableSummonOddsRateList
{
    public OddsRate Normal { get; set; }
    public OddsRate Guarantee { get; set; }

    public RedoableSummonOddsRateList(OddsRate normal, OddsRate guarantee)
    {
        this.Normal = normal;
        this.Guarantee = guarantee;
    }

    public RedoableSummonOddsRateList() { }
}

[MessagePackObject(true)]
public class RepeatData
{
    public string RepeatKey { get; set; }
    public int RepeatCount { get; set; }
    public int RepeatState { get; set; }

    public RepeatData(string repeatKey, int repeatCount, int repeatState)
    {
        this.RepeatKey = repeatKey;
        this.RepeatCount = repeatCount;
        this.RepeatState = repeatState;
    }

    public RepeatData() { }
}

[MessagePackObject(true)]
public class RepeatSetting
{
    public RepeatSettingType RepeatType { get; set; }
    public int RepeatCount { get; set; }
    public List<UseItem> UseItemList { get; set; }

    public RepeatSetting(RepeatSettingType repeatType, int repeatCount, List<UseItem> useItemList)
    {
        this.RepeatType = repeatType;
        this.RepeatCount = repeatCount;
        this.UseItemList = useItemList;
    }

    public RepeatSetting() { }
}

[MessagePackObject(true)]
public class ResponseCommon
{
    public DataHeader DataHeaders { get; set; }

    public ResponseCommon(DataHeader dataHeaders)
    {
        this.DataHeaders = dataHeaders;
    }

    public ResponseCommon() { }
}

[MessagePackObject(true)]
public class RewardRecord
{
    public List<AtgenDropAll> DropAll { get; set; } = new List<AtgenDropAll>();
    public IEnumerable<AtgenFirstClearSet> FirstClearSet { get; set; } =
        Enumerable.Empty<AtgenFirstClearSet>();
    public IEnumerable<AtgenFirstClearSet> MissionComplete { get; set; } =
        Enumerable.Empty<AtgenFirstClearSet>();
    public IEnumerable<AtgenMissionsClearSet> MissionsClearSet { get; set; } =
        Enumerable.Empty<AtgenMissionsClearSet>();
    public IEnumerable<AtgenFirstClearSet> QuestBonusList { get; set; } =
        Enumerable.Empty<AtgenFirstClearSet>();
    public IEnumerable<AtgenFirstClearSet> ChallengeQuestBonusList { get; set; } =
        Enumerable.Empty<AtgenFirstClearSet>();
    public IEnumerable<AtgenFirstClearSet> CampaignExtraRewardList { get; set; } =
        Enumerable.Empty<AtgenFirstClearSet>();
    public IEnumerable<AtgenEnemyPiece> EnemyPiece { get; set; } =
        Enumerable.Empty<AtgenEnemyPiece>();
    public AtgenFirstMeeting FirstMeeting { get; set; } = new();
    public IEnumerable<AtgenFirstClearSet> CarryBonus { get; set; } =
        Enumerable.Empty<AtgenFirstClearSet>();
    public IEnumerable<AtgenFirstClearSet> RebornBonus { get; set; } =
        Enumerable.Empty<AtgenFirstClearSet>();
    public IEnumerable<AtgenFirstClearSet> WeeklyLimitRewardList { get; set; } =
        Enumerable.Empty<AtgenFirstClearSet>();
    public int TakeCoin { get; set; }
    public float ShopQuestBonusFactor { get; set; }
    public int PlayerLevelUpFstone { get; set; }
    public int TakeAccumulatePoint { get; set; }
    public int TakeBoostAccumulatePoint { get; set; }
    public int TakeAstralItemQuantity { get; set; }

    public RewardRecord(
        List<AtgenDropAll> dropAll,
        IEnumerable<AtgenFirstClearSet> firstClearSet,
        IEnumerable<AtgenFirstClearSet> missionComplete,
        IEnumerable<AtgenMissionsClearSet> missionsClearSet,
        IEnumerable<AtgenFirstClearSet> questBonusList,
        IEnumerable<AtgenFirstClearSet> challengeQuestBonusList,
        IEnumerable<AtgenFirstClearSet> campaignExtraRewardList,
        IEnumerable<AtgenEnemyPiece> enemyPiece,
        AtgenFirstMeeting firstMeeting,
        IEnumerable<AtgenFirstClearSet> carryBonus,
        IEnumerable<AtgenFirstClearSet> rebornBonus,
        IEnumerable<AtgenFirstClearSet> weeklyLimitRewardList,
        int takeCoin,
        float shopQuestBonusFactor,
        int playerLevelUpFstone,
        int takeAccumulatePoint,
        int takeBoostAccumulatePoint,
        int takeAstralItemQuantity
    )
    {
        this.DropAll = dropAll;
        this.FirstClearSet = firstClearSet;
        this.MissionComplete = missionComplete;
        this.MissionsClearSet = missionsClearSet;
        this.QuestBonusList = questBonusList;
        this.ChallengeQuestBonusList = challengeQuestBonusList;
        this.CampaignExtraRewardList = campaignExtraRewardList;
        this.EnemyPiece = enemyPiece;
        this.FirstMeeting = firstMeeting;
        this.CarryBonus = carryBonus;
        this.RebornBonus = rebornBonus;
        this.WeeklyLimitRewardList = weeklyLimitRewardList;
        this.TakeCoin = takeCoin;
        this.ShopQuestBonusFactor = shopQuestBonusFactor;
        this.PlayerLevelUpFstone = playerLevelUpFstone;
        this.TakeAccumulatePoint = takeAccumulatePoint;
        this.TakeBoostAccumulatePoint = takeBoostAccumulatePoint;
        this.TakeAstralItemQuantity = takeAstralItemQuantity;
    }

    public RewardRecord() { }
}

[MessagePackObject(true)]
public class RewardReliabilityList
{
    public IEnumerable<DragonRewardEntityList> LevelupEntityList { get; set; }
    public int Level { get; set; }
    public int IsReleaseStory { get; set; }

    public RewardReliabilityList(
        IEnumerable<DragonRewardEntityList> levelupEntityList,
        int level,
        int isReleaseStory
    )
    {
        this.LevelupEntityList = levelupEntityList;
        this.Level = level;
        this.IsReleaseStory = isReleaseStory;
    }

    public RewardReliabilityList() { }
}

[MessagePackObject(true)]
public class RoomList
{
    public int RoomId { get; set; }
    public string RoomName { get; set; }
    public string Region { get; set; }
    public string ClusterName { get; set; }
    public string Language { get; set; }
    public RoomStatuses Status { get; set; }
    public int EntryType { get; set; }
    public int EntryGuildId { get; set; }
    public ulong HostViewerId { get; set; }
    public string HostName { get; set; }
    public int HostLevel { get; set; }
    public Charas LeaderCharaId { get; set; }
    public int LeaderCharaLevel { get; set; }
    public int LeaderCharaRarity { get; set; }
    public int QuestId { get; set; }
    public QuestTypes QuestType { get; set; }
    public IEnumerable<AtgenRoomMemberList> RoomMemberList { get; set; }
    public DateTimeOffset StartEntryTime { get; set; }
    public int MemberNum { get; set; }
    public AtgenEntryConditions EntryConditions { get; set; }
    public int CompatibleId { get; set; }

    public RoomList(
        int roomId,
        string roomName,
        string region,
        string clusterName,
        string language,
        RoomStatuses status,
        int entryType,
        int entryGuildId,
        ulong hostViewerId,
        string hostName,
        int hostLevel,
        Charas leaderCharaId,
        int leaderCharaLevel,
        int leaderCharaRarity,
        int questId,
        QuestTypes questType,
        IEnumerable<AtgenRoomMemberList> roomMemberList,
        DateTimeOffset startEntryTime,
        int memberNum,
        AtgenEntryConditions entryConditions,
        int compatibleId
    )
    {
        this.RoomId = roomId;
        this.RoomName = roomName;
        this.Region = region;
        this.ClusterName = clusterName;
        this.Language = language;
        this.Status = status;
        this.EntryType = entryType;
        this.EntryGuildId = entryGuildId;
        this.HostViewerId = hostViewerId;
        this.HostName = hostName;
        this.HostLevel = hostLevel;
        this.LeaderCharaId = leaderCharaId;
        this.LeaderCharaLevel = leaderCharaLevel;
        this.LeaderCharaRarity = leaderCharaRarity;
        this.QuestId = questId;
        this.QuestType = questType;
        this.RoomMemberList = roomMemberList;
        this.StartEntryTime = startEntryTime;
        this.MemberNum = memberNum;
        this.EntryConditions = entryConditions;
        this.CompatibleId = compatibleId;
    }

    public RoomList() { }
}

[MessagePackObject(true)]
public class SearchClearPartyCharaList
{
    public int QuestId { get; set; }
    public IEnumerable<AtgenArchivePartyCharaList> ArchivePartyCharaList { get; set; }

    public SearchClearPartyCharaList(
        int questId,
        IEnumerable<AtgenArchivePartyCharaList> archivePartyCharaList
    )
    {
        this.QuestId = questId;
        this.ArchivePartyCharaList = archivePartyCharaList;
    }

    public SearchClearPartyCharaList() { }
}

[MessagePackObject(true)]
public class SearchClearPartyList
{
    public IEnumerable<AtgenArchivePartyUnitList> ArchivePartyUnitList { get; set; }

    public SearchClearPartyList(IEnumerable<AtgenArchivePartyUnitList> archivePartyUnitList)
    {
        this.ArchivePartyUnitList = archivePartyUnitList;
    }

    public SearchClearPartyList() { }
}

[MessagePackObject(true)]
public class SettingSupport
{
    public Charas CharaId { get; set; }
    public ulong EquipDragonKeyId { get; set; }
    public ulong EquipWeaponKeyId { get; set; }
    public ulong EquipAmuletKeyId { get; set; }
    public ulong EquipAmulet2KeyId { get; set; }
    public int EquipWeaponBodyId { get; set; }
    public int EquipCrestSlotType1CrestId1 { get; set; }
    public int EquipCrestSlotType1CrestId2 { get; set; }
    public int EquipCrestSlotType1CrestId3 { get; set; }
    public int EquipCrestSlotType2CrestId1 { get; set; }
    public int EquipCrestSlotType2CrestId2 { get; set; }
    public int EquipCrestSlotType3CrestId1 { get; set; }
    public int EquipCrestSlotType3CrestId2 { get; set; }
    public ulong EquipTalismanKeyId { get; set; }

    public SettingSupport(
        Charas charaId,
        ulong equipDragonKeyId,
        ulong equipWeaponKeyId,
        ulong equipAmuletKeyId,
        ulong equipAmulet2KeyId,
        int equipWeaponBodyId,
        int equipCrestSlotType1CrestId1,
        int equipCrestSlotType1CrestId2,
        int equipCrestSlotType1CrestId3,
        int equipCrestSlotType2CrestId1,
        int equipCrestSlotType2CrestId2,
        int equipCrestSlotType3CrestId1,
        int equipCrestSlotType3CrestId2,
        ulong equipTalismanKeyId
    )
    {
        this.CharaId = charaId;
        this.EquipDragonKeyId = equipDragonKeyId;
        this.EquipWeaponKeyId = equipWeaponKeyId;
        this.EquipAmuletKeyId = equipAmuletKeyId;
        this.EquipAmulet2KeyId = equipAmulet2KeyId;
        this.EquipWeaponBodyId = equipWeaponBodyId;
        this.EquipCrestSlotType1CrestId1 = equipCrestSlotType1CrestId1;
        this.EquipCrestSlotType1CrestId2 = equipCrestSlotType1CrestId2;
        this.EquipCrestSlotType1CrestId3 = equipCrestSlotType1CrestId3;
        this.EquipCrestSlotType2CrestId1 = equipCrestSlotType2CrestId1;
        this.EquipCrestSlotType2CrestId2 = equipCrestSlotType2CrestId2;
        this.EquipCrestSlotType3CrestId1 = equipCrestSlotType3CrestId1;
        this.EquipCrestSlotType3CrestId2 = equipCrestSlotType3CrestId2;
        this.EquipTalismanKeyId = equipTalismanKeyId;
    }

    public SettingSupport() { }
}

[MessagePackObject(true)]
public class ShopNotice
{
    [JsonConverter(typeof(BoolIntJsonConverter))]
    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsShopNotification { get; set; }

    public ShopNotice(bool isShopNotification)
    {
        this.IsShopNotification = isShopNotification;
    }

    public ShopNotice() { }
}

[MessagePackObject(true)]
public class ShopPurchaseList
{
    public int GoodsId { get; set; }
    public DateTimeOffset LastBuyTime { get; set; }
    public DateTimeOffset EffectStartTime { get; set; }
    public DateTimeOffset EffectEndTime { get; set; }
    public int BuyCount { get; set; }

    public ShopPurchaseList(
        int goodsId,
        DateTimeOffset lastBuyTime,
        DateTimeOffset effectStartTime,
        DateTimeOffset effectEndTime,
        int buyCount
    )
    {
        this.GoodsId = goodsId;
        this.LastBuyTime = lastBuyTime;
        this.EffectStartTime = effectStartTime;
        this.EffectEndTime = effectEndTime;
        this.BuyCount = buyCount;
    }

    public ShopPurchaseList() { }
}

[MessagePackObject(true)]
public class SimpleEventUserList
{
    public int EventId { get; set; }
    public int SimpleEventItem1 { get; set; }
    public int SimpleEventItem2 { get; set; }
    public int SimpleEventItem3 { get; set; }

    public SimpleEventUserList(
        int eventId,
        int simpleEventItem1,
        int simpleEventItem2,
        int simpleEventItem3
    )
    {
        this.EventId = eventId;
        this.SimpleEventItem1 = simpleEventItem1;
        this.SimpleEventItem2 = simpleEventItem2;
        this.SimpleEventItem3 = simpleEventItem3;
    }

    public SimpleEventUserList() { }
}

[MessagePackObject(true)]
public class SkinWeaponData
{
    public int WeaponId { get; set; }

    public SkinWeaponData(int weaponId)
    {
        this.WeaponId = weaponId;
    }

    public SkinWeaponData() { }
}

[MessagePackObject(true)]
public class SpecialMissionList
{
    public int SpecialMissionId { get; set; }
    public int Progress { get; set; }
    public int State { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset StartDate { get; set; }

    public SpecialMissionList(
        int specialMissionId,
        int progress,
        int state,
        DateTimeOffset endDate,
        DateTimeOffset startDate
    )
    {
        this.SpecialMissionId = specialMissionId;
        this.Progress = progress;
        this.State = state;
        this.EndDate = endDate;
        this.StartDate = startDate;
    }

    public SpecialMissionList() { }
}

[MessagePackObject(true)]
public class StampList
{
    public int StampId { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsNew { get; set; }

    public StampList(int stampId, bool isNew)
    {
        this.StampId = stampId;
        this.IsNew = isNew;
    }

    public StampList() { }
}

/// UNKNOWN: params: summon_point_id(why?)
/// <summary>
/// A summon history entry
/// </summary>
/// <param name="key_id">Unique Id of the summon</param>
/// <param name="summon_id">Banner Id</param>
/// <param name="summon_exec_type">Distinguishing 1x from 10x</param>
/// <param name="exec_date">Summon date</param>
/// <param name="payment_type">Summon currency type</param>
/// <param name="summon_prize_rank">Summon prize rank obtained for this summon</param>
/// <param name="summon_point_id">Summon point entry id (unknown why needed since <see cref="SummonPoint"/> exists)</param>
/// <param name="summon_point">Amount of summon points received</param>
/// <param name="get_dew_point_quantity">Amount of Dew points received</param>
[MessagePackObject(true)]
public class SummonHistoryList
{
    public int KeyId { get; set; }
    public int SummonId { get; set; }
    public SummonExecTypes SummonExecType { get; set; }
    public DateTimeOffset ExecDate { get; set; }
    public PaymentTypes PaymentType { get; set; }
    public EntityTypes EntityType { get; set; }
    public int EntityId { get; set; }
    public int EntityQuantity { get; set; }
    public int EntityLevel { get; set; }
    public int EntityRarity { get; set; }
    public int EntityLimitBreakCount { get; set; }
    public int EntityHpPlusCount { get; set; }
    public int EntityAttackPlusCount { get; set; }
    public int SummonPrizeRank { get; set; }
    public int SummonPointId { get; set; }
    public int SummonPoint { get; set; }
    public int GetDewPointQuantity { get; set; }

    public SummonHistoryList(
        int keyId,
        int summonId,
        SummonExecTypes summonExecType,
        DateTimeOffset execDate,
        PaymentTypes paymentType,
        EntityTypes entityType,
        int entityId,
        int entityQuantity,
        int entityLevel,
        int entityRarity,
        int entityLimitBreakCount,
        int entityHpPlusCount,
        int entityAttackPlusCount,
        int summonPrizeRank,
        int summonPointId,
        int summonPoint,
        int getDewPointQuantity
    )
    {
        this.KeyId = keyId;
        this.SummonId = summonId;
        this.SummonExecType = summonExecType;
        this.ExecDate = execDate;
        this.PaymentType = paymentType;
        this.EntityType = entityType;
        this.EntityId = entityId;
        this.EntityQuantity = entityQuantity;
        this.EntityLevel = entityLevel;
        this.EntityRarity = entityRarity;
        this.EntityLimitBreakCount = entityLimitBreakCount;
        this.EntityHpPlusCount = entityHpPlusCount;
        this.EntityAttackPlusCount = entityAttackPlusCount;
        this.SummonPrizeRank = summonPrizeRank;
        this.SummonPointId = summonPointId;
        this.SummonPoint = summonPoint;
        this.GetDewPointQuantity = getDewPointQuantity;
    }

    public SummonHistoryList() { }
}

/// UNKNOWN: params: priority, summon_type, status, daily(unsure), campaign_type, [x]_rest
/// <summary>
/// Banner Data<br/>
/// This is composed from static banner data and DB saved player-banner data
/// </summary>
/// <param name="summon_id">Banner Id</param>
/// <param name="priority">Unknown</param>
/// <param name="summon_type">Unknown, maybe for special banners like platinum only banners</param>
/// <param name="single_crystal">1x summon Wyrmite cost (Negative numbers won't allow summons, 0 for default)</param>
/// <param name="single_diamond">Client uses <see cref="SingleCrystal"/> for displaying both wyrmite and diamantium cost<br/>Most likely 1x summon Diamantium cost (Negative numbers won't allow summons, 0 for default)</param>
/// <param name="multi_crystal">10x summon Wyrmite cost (Negative numbers won't allow summons, 0 for default)</param>
/// <param name="multi_diamond">Client uses <see cref="MultiCrystal"/> for displaying both wyrmite and diamantium cost<br/>Most likely 10x summon Diamantium cost (Negative numbers won't allow summons, 0 for default)</param>
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
/// <param name="beginner_campaign_count_rest">Begginer banner has a free tenfold available(only if <see cref="IsBeginnerCampaign"/> is set)</param>
/// <param name="consecution_campaign_count_rest">Unknown</param>
[MessagePackObject(true)]
public class SummonList
{
    public int SummonId { get; set; }
    public int SummonType { get; set; }
    public int SummonGroupId { get; set; }
    public int SingleCrystal { get; set; }
    public int SingleDiamond { get; set; }
    public int MultiCrystal { get; set; }
    public int MultiDiamond { get; set; }
    public int LimitedCrystal { get; set; }
    public int LimitedDiamond { get; set; }
    public int SummonPointId { get; set; }
    public int AddSummonPoint { get; set; }
    public int AddSummonPointStone { get; set; }
    public int ExchangeSummonPoint { get; set; }
    public int Status { get; set; }
    public DateTimeOffset CommenceDate { get; set; }
    public DateTimeOffset CompleteDate { get; set; }
    public int DailyCount { get; set; }
    public int DailyLimit { get; set; }
    public int TotalCount { get; set; }
    public int TotalLimit { get; set; }
    public int CampaignType { get; set; }
    public int FreeCountRest { get; set; }
    public int IsBeginnerCampaign { get; set; }
    public int BeginnerCampaignCountRest { get; set; }
    public int ConsecutionCampaignCountRest { get; set; }

    /// UNKNOWN: params: priority, summon_type, status, daily(unsure), campaign_type, [x]_rest
    /// <summary>
    /// Banner Data<br/>
    /// This is composed from static banner data and DB saved player-banner data
    /// </summary>
    /// <param name="summonId">Banner Id</param>
    /// <param name="priority">Unknown</param>
    /// <param name="summonType">Unknown, maybe for special banners like platinum only banners</param>
    /// <param name="singleCrystal">1x summon Wyrmite cost (Negative numbers won't allow summons, 0 for default)</param>
    /// <param name="singleDiamond">Client uses <see cref="SingleCrystal"/> for displaying both wyrmite and diamantium cost<br/>Most likely 1x summon Diamantium cost (Negative numbers won't allow summons, 0 for default)</param>
    /// <param name="multiCrystal">10x summon Wyrmite cost (Negative numbers won't allow summons, 0 for default)</param>
    /// <param name="multiDiamond">Client uses <see cref="MultiCrystal"/> for displaying both wyrmite and diamantium cost<br/>Most likely 10x summon Diamantium cost (Negative numbers won't allow summons, 0 for default)</param>
    /// <param name="limitedCrystal">Unknown: Presumably Wyrmite cost of the limited 1x summon button but it never existed</param>
    /// <param name="limitedDiamond">Diamantium cost of the limited 1x summon button</param>
    /// <param name="addSummonPoint">Summon points for a 1x Wyrmite summon</param>
    /// <param name="addSummonPointStone">Summon points for a 1x Diamantium summon</param>
    /// <param name="exchangeSummonPoint">Summon point cost for sparking, the client doesn't seem to care though</param>
    /// <param name="status">Unknown function, maybe just active = 1, inactive = 0 but no change in normal banner</param>
    /// <param name="commenceDate">Banner start date</param>
    /// <param name="completeDate">Banner end date</param>
    /// <param name="dailyCount">Currently used summons for the daily discounted diamantium summon</param>
    /// <param name="dailyLimit">Total limit for the daily discounted diamantium summon</param>
    /// <param name="totalLimit">Total amount of summons limit(seems ignored for normal banners)</param>
    /// <param name="totalCount">Current total amount of summons(seems ignored for normal banners)</param>
    /// <param name="campaignType">Unknown, maybe used for </param>
    /// <param name="freeCountRest">Most likely free summons for certain banner/campaign types</param>
    /// <param name="isBeginnerCampaign">If this banner is part of the beginner campaign</param>
    /// <param name="beginnerCampaignCountRest">Begginer banner has a free tenfold available(only if <see cref="IsBeginnerCampaign"/> is set)</param>
    /// <param name="consecutionCampaignCountRest">Unknown</param>
    public SummonList(
        int summonId,
        int summonType,
        int summonGroupId,
        int singleCrystal,
        int singleDiamond,
        int multiCrystal,
        int multiDiamond,
        int limitedCrystal,
        int limitedDiamond,
        int summonPointId,
        int addSummonPoint,
        int addSummonPointStone,
        int exchangeSummonPoint,
        int status,
        DateTimeOffset commenceDate,
        DateTimeOffset completeDate,
        int dailyCount,
        int dailyLimit,
        int totalCount,
        int totalLimit,
        int campaignType,
        int freeCountRest,
        int isBeginnerCampaign,
        int beginnerCampaignCountRest,
        int consecutionCampaignCountRest
    )
    {
        this.SummonId = summonId;
        this.SummonType = summonType;
        this.SummonGroupId = summonGroupId;
        this.SingleCrystal = singleCrystal;
        this.SingleDiamond = singleDiamond;
        this.MultiCrystal = multiCrystal;
        this.MultiDiamond = multiDiamond;
        this.LimitedCrystal = limitedCrystal;
        this.LimitedDiamond = limitedDiamond;
        this.SummonPointId = summonPointId;
        this.AddSummonPoint = addSummonPoint;
        this.AddSummonPointStone = addSummonPointStone;
        this.ExchangeSummonPoint = exchangeSummonPoint;
        this.Status = status;
        this.CommenceDate = commenceDate;
        this.CompleteDate = completeDate;
        this.DailyCount = dailyCount;
        this.DailyLimit = dailyLimit;
        this.TotalCount = totalCount;
        this.TotalLimit = totalLimit;
        this.CampaignType = campaignType;
        this.FreeCountRest = freeCountRest;
        this.IsBeginnerCampaign = isBeginnerCampaign;
        this.BeginnerCampaignCountRest = beginnerCampaignCountRest;
        this.ConsecutionCampaignCountRest = consecutionCampaignCountRest;
    }

    public SummonList() { }
}

[MessagePackObject(true)]
public class SummonPointList
{
    public int SummonPointId { get; set; }
    public int SummonPoint { get; set; }
    public int CsSummonPoint { get; set; }
    public int CsPointTermMinDate { get; set; }
    public int CsPointTermMaxDate { get; set; }

    public SummonPointList(
        int summonPointId,
        int summonPoint,
        int csSummonPoint,
        int csPointTermMinDate,
        int csPointTermMaxDate
    )
    {
        this.SummonPointId = summonPointId;
        this.SummonPoint = summonPoint;
        this.CsSummonPoint = csSummonPoint;
        this.CsPointTermMinDate = csPointTermMinDate;
        this.CsPointTermMaxDate = csPointTermMaxDate;
    }

    public SummonPointList() { }
}

[MessagePackObject(true)]
public class SummonPrizeOddsRate
{
    public IEnumerable<AtgenSummonPrizeRankList> SummonPrizeRankList { get; set; }
    public IEnumerable<AtgenSummonPrizeEntitySetList> SummonPrizeEntitySetList { get; set; }

    public SummonPrizeOddsRate(
        IEnumerable<AtgenSummonPrizeRankList> summonPrizeRankList,
        IEnumerable<AtgenSummonPrizeEntitySetList> summonPrizeEntitySetList
    )
    {
        this.SummonPrizeRankList = summonPrizeRankList;
        this.SummonPrizeEntitySetList = summonPrizeEntitySetList;
    }

    public SummonPrizeOddsRate() { }
}

[MessagePackObject(true)]
public class SummonPrizeOddsRateList
{
    public SummonPrizeOddsRate Normal { get; set; }
    public SummonPrizeOddsRate Guarantee { get; set; }

    public SummonPrizeOddsRateList(SummonPrizeOddsRate normal, SummonPrizeOddsRate guarantee)
    {
        this.Normal = normal;
        this.Guarantee = guarantee;
    }

    public SummonPrizeOddsRateList() { }
}

[MessagePackObject]
public class SummonTicketList
{
    [Key("key_id")]
    public long KeyId { get; set; }

    [Key("summon_ticket_id")]
    public SummonTickets SummonTicketId { get; set; }

    [Key("quantity")]
    public int Quantity { get; set; }

    [Key("use_limit_time")]
    public DateTimeOffset UseLimitTime { get; set; }

    public SummonTicketList(
        long keyId,
        SummonTickets summonTicketId,
        int quantity,
        DateTimeOffset useLimitTime
    )
    {
        this.KeyId = keyId;
        this.SummonTicketId = summonTicketId;
        this.Quantity = quantity;
        this.UseLimitTime = useLimitTime;
    }

    public SummonTicketList() { }
}

[MessagePackObject(true)]
public class TalismanList
{
    public ulong TalismanKeyId { get; set; }
    public Talismans TalismanId { get; set; }
    public int TalismanAbilityId1 { get; set; }
    public int TalismanAbilityId2 { get; set; }
    public int TalismanAbilityId3 { get; set; }
    public int AdditionalHp { get; set; }
    public int AdditionalAttack { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool IsNew { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool IsLock { get; set; }
    public DateTimeOffset Gettime { get; set; }

    public TalismanList(
        ulong talismanKeyId,
        Talismans talismanId,
        int talismanAbilityId1,
        int talismanAbilityId2,
        int talismanAbilityId3,
        int additionalHp,
        int additionalAttack,
        bool isNew,
        bool isLock,
        DateTimeOffset gettime
    )
    {
        this.TalismanKeyId = talismanKeyId;
        this.TalismanId = talismanId;
        this.TalismanAbilityId1 = talismanAbilityId1;
        this.TalismanAbilityId2 = talismanAbilityId2;
        this.TalismanAbilityId3 = talismanAbilityId3;
        this.AdditionalHp = additionalHp;
        this.AdditionalAttack = additionalAttack;
        this.IsNew = isNew;
        this.IsLock = isLock;
        this.Gettime = gettime;
    }

    public TalismanList() { }
}

[MessagePackObject(true)]
public class TimeAttackRankingData
{
    public int RankingId { get; set; }
    public IEnumerable<AtgenOwnRankingList> OwnRankingList { get; set; }

    public TimeAttackRankingData(int rankingId, IEnumerable<AtgenOwnRankingList> ownRankingList)
    {
        this.RankingId = rankingId;
        this.OwnRankingList = ownRankingList;
    }

    public TimeAttackRankingData() { }
}

[MessagePackObject(true)]
public class TreasureTradeList
{
    public int Priority { get; set; }
    public int TreasureTradeId { get; set; }
    public int TabGroupId { get; set; }
    public int IsLockView { get; set; }
    public DateTimeOffset CommenceDate { get; set; }
    public DateTimeOffset CompleteDate { get; set; }
    public int ResetType { get; set; }
    public int Limit { get; set; }
    public IEnumerable<AtgenNeedTradeEntityList> NeedTradeEntityList { get; set; }
    public EntityTypes DestinationEntityType { get; set; }
    public int DestinationEntityId { get; set; }
    public int DestinationEntityQuantity { get; set; }
    public int DestinationLimitBreakCount { get; set; }

    public TreasureTradeList(
        int priority,
        int treasureTradeId,
        int tabGroupId,
        int isLockView,
        DateTimeOffset commenceDate,
        DateTimeOffset completeDate,
        int resetType,
        int limit,
        IEnumerable<AtgenNeedTradeEntityList> needTradeEntityList,
        EntityTypes destinationEntityType,
        int destinationEntityId,
        int destinationEntityQuantity,
        int destinationLimitBreakCount
    )
    {
        this.Priority = priority;
        this.TreasureTradeId = treasureTradeId;
        this.TabGroupId = tabGroupId;
        this.IsLockView = isLockView;
        this.CommenceDate = commenceDate;
        this.CompleteDate = completeDate;
        this.ResetType = resetType;
        this.Limit = limit;
        this.NeedTradeEntityList = needTradeEntityList;
        this.DestinationEntityType = destinationEntityType;
        this.DestinationEntityId = destinationEntityId;
        this.DestinationEntityQuantity = destinationEntityQuantity;
        this.DestinationLimitBreakCount = destinationLimitBreakCount;
    }

    public TreasureTradeList() { }
}

[MessagePackObject(true)]
public class UnitStoryList
{
    public int UnitStoryId { get; set; }
    public int IsRead { get; set; }

    public UnitStoryList(int unitStoryId, int isRead)
    {
        this.UnitStoryId = unitStoryId;
        this.IsRead = isRead;
    }

    public UnitStoryList() { }
}

[MessagePackObject(true)]
public class UnusedType { }

[MessagePackObject(true)]
public class UpdateDataList
{
    public UserData UserData { get; set; }
    public DiamondData DiamondData { get; set; }
    public PartyPowerData PartyPowerData { get; set; }
    public IEnumerable<CharaList> CharaList { get; set; }
    public IEnumerable<DragonList> DragonList { get; set; }
    public IEnumerable<WeaponList> WeaponList { get; set; }
    public IEnumerable<AmuletList> AmuletList { get; set; }
    public IEnumerable<WeaponSkinList> WeaponSkinList { get; set; }
    public IEnumerable<WeaponBodyList> WeaponBodyList { get; set; }
    public IEnumerable<WeaponPassiveAbilityList> WeaponPassiveAbilityList { get; set; }
    public IEnumerable<AbilityCrestList> AbilityCrestList { get; set; }
    public IEnumerable<AbilityCrestSetList> AbilityCrestSetList { get; set; }
    public IEnumerable<TalismanList> TalismanList { get; set; }

    private IEnumerable<PartyList> partyList;
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
    public IEnumerable<MuseumList> MuseumList { get; set; }
    public IEnumerable<AlbumDragonData> AlbumDragonList { get; set; }
    public IEnumerable<AlbumWeaponList> AlbumWeaponList { get; set; }
    public IEnumerable<EnemyBookList> EnemyBookList { get; set; }
    public IEnumerable<ItemList> ItemList { get; set; }
    public IEnumerable<AstralItemList> AstralItemList { get; set; }
    public IEnumerable<MaterialList> MaterialList { get; set; }
    public IEnumerable<QuestList> QuestList { get; set; }
    public IEnumerable<QuestEventList> QuestEventList { get; set; }
    public IEnumerable<DragonGiftList> DragonGiftList { get; set; }
    public IEnumerable<DragonReliabilityList> DragonReliabilityList { get; set; }
    public IEnumerable<UnitStoryList> UnitStoryList { get; set; }
    public IEnumerable<CastleStoryList> CastleStoryList { get; set; }
    public IEnumerable<QuestStoryList> QuestStoryList { get; set; }
    public IEnumerable<QuestTreasureList> QuestTreasureList { get; set; }
    public IEnumerable<QuestWallList> QuestWallList { get; set; }
    public IEnumerable<QuestCarryList> QuestCarryList { get; set; }
    public IEnumerable<QuestEntryConditionList> QuestEntryConditionList { get; set; }
    public IEnumerable<SummonTicketList> SummonTicketList { get; set; }
    public IEnumerable<SummonPointList> SummonPointList { get; set; }
    public IEnumerable<LotteryTicketList> LotteryTicketList { get; set; }
    public IEnumerable<ExchangeTicketList> ExchangeTicketList { get; set; }
    public IEnumerable<GatherItemList> GatherItemList { get; set; }
    public IEnumerable<BuildList> BuildList { get; set; }
    public IEnumerable<FortPlantList> FortPlantList { get; set; }
    public FortBonusList FortBonusList { get; set; }
    public IEnumerable<CraftList> CraftList { get; set; }
    public CurrentMainStoryMission CurrentMainStoryMission { get; set; }
    public IEnumerable<CharaUnitSetList> CharaUnitSetList { get; set; }
    public UserGuildData UserGuildData { get; set; }
    public GuildData GuildData { get; set; }
    public IEnumerable<BattleRoyalCharaSkinList> BattleRoyalCharaSkinList { get; set; }
    public DmodeInfo DmodeInfo { get; set; }
    public IEnumerable<DmodeStoryList> DmodeStoryList { get; set; }
    public PresentNotice PresentNotice { get; set; }
    public FriendNotice FriendNotice { get; set; }
    public MissionNotice MissionNotice { get; set; }
    public GuildNotice GuildNotice { get; set; }
    public ShopNotice ShopNotice { get; set; }
    public AlbumPassiveNotice AlbumPassiveNotice { get; set; }
    public IEnumerable<RaidEventUserList> RaidEventUserList { get; set; }
    public IEnumerable<MazeEventUserList> MazeEventUserList { get; set; }
    public IEnumerable<BuildEventUserList> BuildEventUserList { get; set; }
    public IEnumerable<CollectEventUserList> CollectEventUserList { get; set; }
    public IEnumerable<Clb01EventUserList> Clb01EventUserList { get; set; }
    public IEnumerable<ExRushEventUserList> ExRushEventUserList { get; set; }
    public IEnumerable<SimpleEventUserList> SimpleEventUserList { get; set; }
    public IEnumerable<ExHunterEventUserList> ExHunterEventUserList { get; set; }
    public IEnumerable<CombatEventUserList> CombatEventUserList { get; set; }
    public IEnumerable<BattleRoyalEventItemList> BattleRoyalEventItemList { get; set; }
    public IEnumerable<BattleRoyalEventUserRecord> BattleRoyalEventUserRecord { get; set; }
    public IEnumerable<BattleRoyalCycleUserRecord> BattleRoyalCycleUserRecord { get; set; }
    public IEnumerable<EarnEventUserList> EarnEventUserList { get; set; }
    public IEnumerable<EventPassiveList> EventPassiveList { get; set; }
    public IEnumerable<FunctionalMaintenanceList> FunctionalMaintenanceList { get; set; }

    public UpdateDataList(
        UserData userData,
        DiamondData diamondData,
        PartyPowerData partyPowerData,
        IEnumerable<CharaList> charaList,
        IEnumerable<DragonList> dragonList,
        IEnumerable<WeaponList> weaponList,
        IEnumerable<AmuletList> amuletList,
        IEnumerable<WeaponSkinList> weaponSkinList,
        IEnumerable<WeaponBodyList> weaponBodyList,
        IEnumerable<WeaponPassiveAbilityList> weaponPassiveAbilityList,
        IEnumerable<AbilityCrestList> abilityCrestList,
        IEnumerable<AbilityCrestSetList> abilityCrestSetList,
        IEnumerable<TalismanList> talismanList,
        IEnumerable<PartyList> partyList,
        IEnumerable<MuseumList> museumList,
        IEnumerable<AlbumDragonData> albumDragonList,
        IEnumerable<AlbumWeaponList> albumWeaponList,
        IEnumerable<EnemyBookList> enemyBookList,
        IEnumerable<ItemList> itemList,
        IEnumerable<AstralItemList> astralItemList,
        IEnumerable<MaterialList> materialList,
        IEnumerable<QuestList> questList,
        IEnumerable<QuestEventList> questEventList,
        IEnumerable<DragonGiftList> dragonGiftList,
        IEnumerable<DragonReliabilityList> dragonReliabilityList,
        IEnumerable<UnitStoryList> unitStoryList,
        IEnumerable<CastleStoryList> castleStoryList,
        IEnumerable<QuestStoryList> questStoryList,
        IEnumerable<QuestTreasureList> questTreasureList,
        IEnumerable<QuestWallList> questWallList,
        IEnumerable<QuestCarryList> questCarryList,
        IEnumerable<QuestEntryConditionList> questEntryConditionList,
        IEnumerable<SummonTicketList> summonTicketList,
        IEnumerable<SummonPointList> summonPointList,
        IEnumerable<LotteryTicketList> lotteryTicketList,
        IEnumerable<ExchangeTicketList> exchangeTicketList,
        IEnumerable<GatherItemList> gatherItemList,
        IEnumerable<BuildList> buildList,
        IEnumerable<FortPlantList> fortPlantList,
        FortBonusList fortBonusList,
        IEnumerable<CraftList> craftList,
        CurrentMainStoryMission currentMainStoryMission,
        IEnumerable<CharaUnitSetList> charaUnitSetList,
        UserGuildData userGuildData,
        GuildData guildData,
        IEnumerable<BattleRoyalCharaSkinList> battleRoyalCharaSkinList,
        DmodeInfo dmodeInfo,
        IEnumerable<DmodeStoryList> dmodeStoryList,
        PresentNotice presentNotice,
        FriendNotice friendNotice,
        MissionNotice missionNotice,
        GuildNotice guildNotice,
        ShopNotice shopNotice,
        AlbumPassiveNotice albumPassiveNotice,
        IEnumerable<RaidEventUserList> raidEventUserList,
        IEnumerable<MazeEventUserList> mazeEventUserList,
        IEnumerable<BuildEventUserList> buildEventUserList,
        IEnumerable<CollectEventUserList> collectEventUserList,
        IEnumerable<Clb01EventUserList> clb01EventUserList,
        IEnumerable<ExRushEventUserList> exRushEventUserList,
        IEnumerable<SimpleEventUserList> simpleEventUserList,
        IEnumerable<ExHunterEventUserList> exHunterEventUserList,
        IEnumerable<CombatEventUserList> combatEventUserList,
        IEnumerable<BattleRoyalEventItemList> battleRoyalEventItemList,
        IEnumerable<BattleRoyalEventUserRecord> battleRoyalEventUserRecord,
        IEnumerable<BattleRoyalCycleUserRecord> battleRoyalCycleUserRecord,
        IEnumerable<EarnEventUserList> earnEventUserList,
        IEnumerable<EventPassiveList> eventPassiveList,
        IEnumerable<FunctionalMaintenanceList> functionalMaintenanceList
    )
    {
        this.UserData = userData;
        this.DiamondData = diamondData;
        this.PartyPowerData = partyPowerData;
        this.CharaList = charaList;
        this.DragonList = dragonList;
        this.WeaponList = weaponList;
        this.AmuletList = amuletList;
        this.WeaponSkinList = weaponSkinList;
        this.WeaponBodyList = weaponBodyList;
        this.WeaponPassiveAbilityList = weaponPassiveAbilityList;
        this.AbilityCrestList = abilityCrestList;
        this.AbilityCrestSetList = abilityCrestSetList;
        this.TalismanList = talismanList;
        this.PartyList = partyList;
        this.MuseumList = museumList;
        this.AlbumDragonList = albumDragonList;
        this.AlbumWeaponList = albumWeaponList;
        this.EnemyBookList = enemyBookList;
        this.ItemList = itemList;
        this.AstralItemList = astralItemList;
        this.MaterialList = materialList;
        this.QuestList = questList;
        this.QuestEventList = questEventList;
        this.DragonGiftList = dragonGiftList;
        this.DragonReliabilityList = dragonReliabilityList;
        this.UnitStoryList = unitStoryList;
        this.CastleStoryList = castleStoryList;
        this.QuestStoryList = questStoryList;
        this.QuestTreasureList = questTreasureList;
        this.QuestWallList = questWallList;
        this.QuestCarryList = questCarryList;
        this.QuestEntryConditionList = questEntryConditionList;
        this.SummonTicketList = summonTicketList;
        this.SummonPointList = summonPointList;
        this.LotteryTicketList = lotteryTicketList;
        this.ExchangeTicketList = exchangeTicketList;
        this.GatherItemList = gatherItemList;
        this.BuildList = buildList;
        this.FortPlantList = fortPlantList;
        this.FortBonusList = fortBonusList;
        this.CraftList = craftList;
        this.CurrentMainStoryMission = currentMainStoryMission;
        this.CharaUnitSetList = charaUnitSetList;
        this.UserGuildData = userGuildData;
        this.GuildData = guildData;
        this.BattleRoyalCharaSkinList = battleRoyalCharaSkinList;
        this.DmodeInfo = dmodeInfo;
        this.DmodeStoryList = dmodeStoryList;
        this.PresentNotice = presentNotice;
        this.FriendNotice = friendNotice;
        this.MissionNotice = missionNotice;
        this.GuildNotice = guildNotice;
        this.ShopNotice = shopNotice;
        this.AlbumPassiveNotice = albumPassiveNotice;
        this.RaidEventUserList = raidEventUserList;
        this.MazeEventUserList = mazeEventUserList;
        this.BuildEventUserList = buildEventUserList;
        this.CollectEventUserList = collectEventUserList;
        this.Clb01EventUserList = clb01EventUserList;
        this.ExRushEventUserList = exRushEventUserList;
        this.SimpleEventUserList = simpleEventUserList;
        this.ExHunterEventUserList = exHunterEventUserList;
        this.CombatEventUserList = combatEventUserList;
        this.BattleRoyalEventItemList = battleRoyalEventItemList;
        this.BattleRoyalEventUserRecord = battleRoyalEventUserRecord;
        this.BattleRoyalCycleUserRecord = battleRoyalCycleUserRecord;
        this.EarnEventUserList = earnEventUserList;
        this.EventPassiveList = eventPassiveList;
        this.FunctionalMaintenanceList = functionalMaintenanceList;
    }

    public UpdateDataList() { }
}

[MessagePackObject(true)]
public class UserAbilityCrestTradeList
{
    public int AbilityCrestTradeId { get; set; }
    public int TradeCount { get; set; }

    public UserAbilityCrestTradeList(int abilityCrestTradeId, int tradeCount)
    {
        this.AbilityCrestTradeId = abilityCrestTradeId;
        this.TradeCount = tradeCount;
    }

    public UserAbilityCrestTradeList() { }
}

[MessagePackObject(true)]
public class UserAmuletTradeList
{
    public int AmuletTradeId { get; set; }
    public int TradeCount { get; set; }

    public UserAmuletTradeList(int amuletTradeId, int tradeCount)
    {
        this.AmuletTradeId = amuletTradeId;
        this.TradeCount = tradeCount;
    }

    public UserAmuletTradeList() { }
}

[MessagePackObject(true)]
public class UserData
{
    public ulong ViewerId { get; set; }

    [MaxLength(10)]
    public string Name { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    public long Coin { get; set; }
    public int Crystal { get; set; }
    public int DewPoint { get; set; }
    public int StaminaSingle { get; set; }
    public DateTimeOffset LastStaminaSingleUpdateTime { get; set; }
    public int StaminaSingleSurplusSecond { get; set; }
    public int StaminaMulti { get; set; }
    public DateTimeOffset LastStaminaMultiUpdateTime { get; set; }
    public int StaminaMultiSurplusSecond { get; set; }
    public int MaxDragonQuantity { get; set; }
    public int MaxWeaponQuantity { get; set; }
    public int MaxAmuletQuantity { get; set; }
    public int QuestSkipPoint { get; set; }
    public int BuildTimePoint { get; set; }
    public int AgeGroup { get; set; }
    public int MainPartyNo { get; set; }
    public Emblems EmblemId { get; set; }
    public int ActiveMemoryEventId { get; set; }
    public int ManaPoint { get; set; }
    public DateTimeOffset LastLoginTime { get; set; }
    public int TutorialStatus { get; set; }
    public IEnumerable<int> TutorialFlagList { get; set; }
    public int PrologueEndTime { get; set; }
    public DateTimeOffset FortOpenTime { get; set; }
    public DateTimeOffset CreateTime { get; set; }
    public int IsOptin { get; set; }

    public UserData(
        ulong viewerId,
        string name,
        int level,
        int exp,
        long coin,
        int crystal,
        int dewPoint,
        int staminaSingle,
        DateTimeOffset lastStaminaSingleUpdateTime,
        int staminaSingleSurplusSecond,
        int staminaMulti,
        DateTimeOffset lastStaminaMultiUpdateTime,
        int staminaMultiSurplusSecond,
        int maxDragonQuantity,
        int maxWeaponQuantity,
        int maxAmuletQuantity,
        int questSkipPoint,
        int buildTimePoint,
        int ageGroup,
        int mainPartyNo,
        Emblems emblemId,
        int activeMemoryEventId,
        int manaPoint,
        DateTimeOffset lastLoginTime,
        int tutorialStatus,
        IEnumerable<int> tutorialFlagList,
        int prologueEndTime,
        DateTimeOffset fortOpenTime,
        DateTimeOffset createTime,
        int isOptin
    )
    {
        this.ViewerId = viewerId;
        this.Name = name;
        this.Level = level;
        this.Exp = exp;
        this.Coin = coin;
        this.Crystal = crystal;
        this.DewPoint = dewPoint;
        this.StaminaSingle = staminaSingle;
        this.LastStaminaSingleUpdateTime = lastStaminaSingleUpdateTime;
        this.StaminaSingleSurplusSecond = staminaSingleSurplusSecond;
        this.StaminaMulti = staminaMulti;
        this.LastStaminaMultiUpdateTime = lastStaminaMultiUpdateTime;
        this.StaminaMultiSurplusSecond = staminaMultiSurplusSecond;
        this.MaxDragonQuantity = maxDragonQuantity;
        this.MaxWeaponQuantity = maxWeaponQuantity;
        this.MaxAmuletQuantity = maxAmuletQuantity;
        this.QuestSkipPoint = questSkipPoint;
        this.BuildTimePoint = buildTimePoint;
        this.AgeGroup = ageGroup;
        this.MainPartyNo = mainPartyNo;
        this.EmblemId = emblemId;
        this.ActiveMemoryEventId = activeMemoryEventId;
        this.ManaPoint = manaPoint;
        this.LastLoginTime = lastLoginTime;
        this.TutorialStatus = tutorialStatus;
        this.TutorialFlagList = tutorialFlagList;
        this.PrologueEndTime = prologueEndTime;
        this.FortOpenTime = fortOpenTime;
        this.CreateTime = createTime;
        this.IsOptin = isOptin;
    }

    public UserData() { }
}

[MessagePackObject(true)]
public class UserEventItemData
{
    public IEnumerable<AtgenUserMazeEventItemList2> UserMazeEventItemList { get; set; }

    public UserEventItemData(IEnumerable<AtgenUserMazeEventItemList2> userMazeEventItemList)
    {
        this.UserMazeEventItemList = userMazeEventItemList;
    }

    public UserEventItemData() { }
}

[MessagePackObject(true)]
public class UserEventLocationRewardList : IEventRewardList<UserEventLocationRewardList>
{
    public int EventId { get; set; }
    public int LocationRewardId { get; set; }

    public UserEventLocationRewardList(int eventId, int locationRewardId)
    {
        this.EventId = eventId;
        this.LocationRewardId = locationRewardId;
    }

    public UserEventLocationRewardList() { }

    public static UserEventLocationRewardList FromDatabase(DbPlayerEventReward reward) =>
        new(reward.EventId, reward.RewardId);
}

[MessagePackObject(true)]
public class UserGuildData
{
    public int GuildId { get; set; }
    public ulong GuildApplyId { get; set; }
    public int PenaltyEndTime { get; set; }
    public int GuildPushNotificationTypeRunning { get; set; }
    public int GuildPushNotificationTypeSuspending { get; set; }
    public int ProfileEntityType { get; set; }
    public int ProfileEntityId { get; set; }
    public int ProfileEntityRarity { get; set; }
    public int IsEnableInviteReceive { get; set; }
    public int IsEnableInviteSend { get; set; }
    public int LastAttendTime { get; set; }

    public UserGuildData(
        int guildId,
        ulong guildApplyId,
        int penaltyEndTime,
        int guildPushNotificationTypeRunning,
        int guildPushNotificationTypeSuspending,
        int profileEntityType,
        int profileEntityId,
        int profileEntityRarity,
        int isEnableInviteReceive,
        int isEnableInviteSend,
        int lastAttendTime
    )
    {
        this.GuildId = guildId;
        this.GuildApplyId = guildApplyId;
        this.PenaltyEndTime = penaltyEndTime;
        this.GuildPushNotificationTypeRunning = guildPushNotificationTypeRunning;
        this.GuildPushNotificationTypeSuspending = guildPushNotificationTypeSuspending;
        this.ProfileEntityType = profileEntityType;
        this.ProfileEntityId = profileEntityId;
        this.ProfileEntityRarity = profileEntityRarity;
        this.IsEnableInviteReceive = isEnableInviteReceive;
        this.IsEnableInviteSend = isEnableInviteSend;
        this.LastAttendTime = lastAttendTime;
    }

    public UserGuildData() { }
}

[MessagePackObject(true)]
public class UserRedoableSummonData
{
    public int IsFixedResult { get; set; }
    public IEnumerable<AtgenRedoableSummonResultUnitList> RedoableSummonResultUnitList { get; set; }

    public UserRedoableSummonData(
        int isFixedResult,
        IEnumerable<AtgenRedoableSummonResultUnitList> redoableSummonResultUnitList
    )
    {
        this.IsFixedResult = isFixedResult;
        this.RedoableSummonResultUnitList = redoableSummonResultUnitList;
    }

    public UserRedoableSummonData() { }
}

/// <summary>
/// Updated Summon Banner Data
/// </summary>
/// <param name="summon_id">Id of the summon banner</param>
/// <param name="summon_count">Total Amount of times a player pressed summon on a banner<br/>
/// <b>Not the same as amount of this summon</b></param>
/// <param name="campaign_type">Type of banner.<br/> 0 for default, other values unknown</param>
/// UNKNOWN param: free_count_rest
/// <param name="free_count_rest">Unknown</param>
/// <param name="is_beginner_campaign">Flag for if this banner is part of the beginner campaign</param>
/// <param name="beginner_campaign_count_rest">Free 10x availabe if is beginner campaign</param>
/// UNKNOWN param: consecution_campaign_count_rest
/// <param name="consecution_campaign_count_rest">Unknown</param>
[MessagePackObject(true)]
public class UserSummonList
{
    public int SummonId { get; set; }
    public int SummonCount { get; set; }
    public int CampaignType { get; set; }
    public int FreeCountRest { get; set; }
    public int IsBeginnerCampaign { get; set; }
    public int BeginnerCampaignCountRest { get; set; }
    public int ConsecutionCampaignCountRest { get; set; }

    public UserSummonList(
        int summonId,
        int summonCount,
        int campaignType,
        int freeCountRest,
        int isBeginnerCampaign,
        int beginnerCampaignCountRest,
        int consecutionCampaignCountRest
    )
    {
        this.SummonId = summonId;
        this.SummonCount = summonCount;
        this.CampaignType = campaignType;
        this.FreeCountRest = freeCountRest;
        this.IsBeginnerCampaign = isBeginnerCampaign;
        this.BeginnerCampaignCountRest = beginnerCampaignCountRest;
        this.ConsecutionCampaignCountRest = consecutionCampaignCountRest;
    }

    public UserSummonList() { }
}

[MessagePackObject(true)]
public class UserSupportList
{
    public ulong ViewerId { get; set; }
    public string Name { get; set; }
    public int Level { get; set; }

    public DateTimeOffset LastLoginDate { get; set; }
    public Emblems EmblemId { get; set; }
    public int MaxPartyPower { get; set; }
    public AtgenGuild Guild { get; set; }
    public AtgenSupportChara SupportChara { get; set; }
    public AtgenSupportWeapon SupportWeapon { get; set; }
    public AtgenSupportDragon SupportDragon { get; set; }
    public AtgenSupportWeaponBody SupportWeaponBody { get; set; }
    public IEnumerable<AtgenSupportCrestSlotType1List> SupportCrestSlotType1List { get; set; }
    public IEnumerable<AtgenSupportCrestSlotType1List> SupportCrestSlotType2List { get; set; }
    public IEnumerable<AtgenSupportCrestSlotType1List> SupportCrestSlotType3List { get; set; }
    public AtgenSupportTalisman SupportTalisman { get; set; }
    public AtgenSupportAmulet SupportAmulet { get; set; }
    public AtgenSupportAmulet SupportAmulet2 { get; set; }

    public UserSupportList(
        ulong viewerId,
        string name,
        int level,
        DateTimeOffset lastLoginDate,
        Emblems emblemId,
        int maxPartyPower,
        AtgenGuild guild,
        AtgenSupportChara supportChara,
        AtgenSupportWeapon supportWeapon,
        AtgenSupportDragon supportDragon,
        AtgenSupportWeaponBody supportWeaponBody,
        IEnumerable<AtgenSupportCrestSlotType1List> supportCrestSlotType1List,
        IEnumerable<AtgenSupportCrestSlotType1List> supportCrestSlotType2List,
        IEnumerable<AtgenSupportCrestSlotType1List> supportCrestSlotType3List,
        AtgenSupportTalisman supportTalisman,
        AtgenSupportAmulet supportAmulet,
        AtgenSupportAmulet supportAmulet2
    )
    {
        this.ViewerId = viewerId;
        this.Name = name;
        this.Level = level;
        this.LastLoginDate = lastLoginDate;
        this.EmblemId = emblemId;
        this.MaxPartyPower = maxPartyPower;
        this.Guild = guild;
        this.SupportChara = supportChara;
        this.SupportWeapon = supportWeapon;
        this.SupportDragon = supportDragon;
        this.SupportWeaponBody = supportWeaponBody;
        this.SupportCrestSlotType1List = supportCrestSlotType1List;
        this.SupportCrestSlotType2List = supportCrestSlotType2List;
        this.SupportCrestSlotType3List = supportCrestSlotType3List;
        this.SupportTalisman = supportTalisman;
        this.SupportAmulet = supportAmulet;
        this.SupportAmulet2 = supportAmulet2;
    }

    public UserSupportList() { }
}

[MessagePackObject(true)]
public class UserTreasureTradeList
{
    public int TreasureTradeId { get; set; }
    public int TradeCount { get; set; }
    public DateTimeOffset LastResetTime { get; set; }

    public UserTreasureTradeList(int treasureTradeId, int tradeCount, DateTimeOffset lastResetTime)
    {
        this.TreasureTradeId = treasureTradeId;
        this.TradeCount = tradeCount;
        this.LastResetTime = lastResetTime;
    }

    public UserTreasureTradeList() { }
}

[MessagePackObject(true)]
public class WalletBalance
{
    public int Total { get; set; }
    public int Free { get; set; }
    public IEnumerable<AtgenPaid> Paid { get; set; }

    public WalletBalance(int total, int free, IEnumerable<AtgenPaid> paid)
    {
        this.Total = total;
        this.Free = free;
        this.Paid = paid;
    }

    public WalletBalance() { }
}

[MessagePackObject(true)]
public class WeaponBodyList
{
    public WeaponBodies WeaponBodyId { get; set; }
    public int BuildupCount { get; set; }
    public int LimitBreakCount { get; set; }
    public int LimitOverCount { get; set; }
    public int EquipableCount { get; set; }
    public int AdditionalCrestSlotType1Count { get; set; }
    public int AdditionalCrestSlotType2Count { get; set; }
    public int AdditionalCrestSlotType3Count { get; set; }
    public int AdditionalEffectCount { get; set; }
    public IEnumerable<int> UnlockWeaponPassiveAbilityNoList { get; set; }
    public int FortPassiveCharaWeaponBuildupCount { get; set; }

    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool IsNew { get; set; }
    public DateTimeOffset Gettime { get; set; }
    public int SkillNo { get; set; }
    public int SkillLevel { get; set; }
    public int Ability1Level { get; set; }
    public int Ability2Levell { get; set; }

    public WeaponBodyList(
        WeaponBodies weaponBodyId,
        int buildupCount,
        int limitBreakCount,
        int limitOverCount,
        int equipableCount,
        int additionalCrestSlotType1Count,
        int additionalCrestSlotType2Count,
        int additionalCrestSlotType3Count,
        int additionalEffectCount,
        IEnumerable<int> unlockWeaponPassiveAbilityNoList,
        int fortPassiveCharaWeaponBuildupCount,
        bool isNew,
        DateTimeOffset gettime,
        int skillNo,
        int skillLevel,
        int ability1Level,
        int ability2Levell
    )
    {
        this.WeaponBodyId = weaponBodyId;
        this.BuildupCount = buildupCount;
        this.LimitBreakCount = limitBreakCount;
        this.LimitOverCount = limitOverCount;
        this.EquipableCount = equipableCount;
        this.AdditionalCrestSlotType1Count = additionalCrestSlotType1Count;
        this.AdditionalCrestSlotType2Count = additionalCrestSlotType2Count;
        this.AdditionalCrestSlotType3Count = additionalCrestSlotType3Count;
        this.AdditionalEffectCount = additionalEffectCount;
        this.UnlockWeaponPassiveAbilityNoList = unlockWeaponPassiveAbilityNoList;
        this.FortPassiveCharaWeaponBuildupCount = fortPassiveCharaWeaponBuildupCount;
        this.IsNew = isNew;
        this.Gettime = gettime;
        this.SkillNo = skillNo;
        this.SkillLevel = skillLevel;
        this.Ability1Level = ability1Level;
        this.Ability2Levell = ability2Levell;
    }

    public WeaponBodyList() { }
}

[MessagePackObject(true)]
public class WeaponList
{
    public int WeaponId { get; set; }
    public ulong WeaponKeyId { get; set; }
    public int IsLock { get; set; }
    public int IsNew { get; set; }
    public int Gettime { get; set; }
    public int SkillLevel { get; set; }
    public int HpPlusCount { get; set; }
    public int AttackPlusCount { get; set; }
    public int StatusPlusCount { get; set; }
    public int Level { get; set; }
    public int LimitBreakCount { get; set; }
    public int Exp { get; set; }

    public WeaponList(
        int weaponId,
        ulong weaponKeyId,
        int isLock,
        int isNew,
        int gettime,
        int skillLevel,
        int hpPlusCount,
        int attackPlusCount,
        int statusPlusCount,
        int level,
        int limitBreakCount,
        int exp
    )
    {
        this.WeaponId = weaponId;
        this.WeaponKeyId = weaponKeyId;
        this.IsLock = isLock;
        this.IsNew = isNew;
        this.Gettime = gettime;
        this.SkillLevel = skillLevel;
        this.HpPlusCount = hpPlusCount;
        this.AttackPlusCount = attackPlusCount;
        this.StatusPlusCount = statusPlusCount;
        this.Level = level;
        this.LimitBreakCount = limitBreakCount;
        this.Exp = exp;
    }

    public WeaponList() { }
}

[MessagePackObject(true)]
public class WeaponPassiveAbilityList
{
    public int WeaponPassiveAbilityId { get; set; }

    public WeaponPassiveAbilityList(int weaponPassiveAbilityId)
    {
        this.WeaponPassiveAbilityId = weaponPassiveAbilityId;
    }

    public WeaponPassiveAbilityList() { }
}

[MessagePackObject(true)]
public class WeaponSkinList
{
    public int WeaponSkinId { get; set; }

    [JsonConverter(typeof(BoolIntJsonConverter))]
    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool IsNew { get; set; }
    public DateTimeOffset Gettime { get; set; }

    public WeaponSkinList(int weaponSkinId, bool isNew, DateTimeOffset gettime)
    {
        this.WeaponSkinId = weaponSkinId;
        this.IsNew = isNew;
        this.Gettime = gettime;
    }

    public WeaponSkinList() { }
}
