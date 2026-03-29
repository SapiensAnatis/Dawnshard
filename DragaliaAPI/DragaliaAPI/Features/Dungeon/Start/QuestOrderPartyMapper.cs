using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using LinqToDB;

namespace DragaliaAPI.Features.Dungeon.Start;

public static class QuestOrderPartyMapper
{
    public static List<PartyUnitList> MapToPartyUnitList(
        IEnumerable<QuestOrderParty> orderPartyUnits
    )
    {
        List<PartyUnitList> units = [];

        foreach ((int index, QuestOrderParty unit) in orderPartyUnits.Index())
        {
            CharaData charaData = MasterAsset.CharaData[unit.CharaId];

            ValidateChara(unit, charaData);
            ValidateCrests(unit);

            units.Add(
                new PartyUnitList
                {
                    Position = index + 1,
                    CharaData = MapCharaData(unit, charaData),
                    DragonData = MapDragonData(unit),
                    DragonReliabilityLevel = 30,
                    WeaponBodyData = new GameWeaponBody
                    {
                        WeaponBodyId = unit.WeaponBodyId,
                        BuildupCount = unit.WeaponBodyBuildupCount,
                        LimitBreakCount = unit.WeaponBodyLimitBreakCount,
                        LimitOverCount = unit.WeaponBodyLimitOverCount,
                    },
                    WeaponSkinData = new GameWeaponSkin(unit.WeaponSkinId),
                    CrestSlotType1CrestList = [],
                    CrestSlotType2CrestList = [],
                    CrestSlotType3CrestList = [],
                    TalismanData = new(),
                    EditSkill1CharaData = new(),
                    EditSkill2CharaData = new(),
                    GameWeaponPassiveAbilityList = [],
                }
            );
        }

        for (int i = units.Count; i < 4; i++)
        {
            units.Add(
                new PartyUnitList
                {
                    Position = i + 1,
                    CharaData = new(),
                    DragonData = new(),
                    WeaponSkinData = new(),
                    WeaponBodyData = new(),
                    CrestSlotType1CrestList = [],
                    CrestSlotType2CrestList = [],
                    CrestSlotType3CrestList = [],
                    TalismanData = new(),
                    EditSkill1CharaData = new(),
                    EditSkill2CharaData = new(),
                    GameWeaponPassiveAbilityList = [],
                }
            );
        }

        return units;
    }

    public static List<PartySettingList> MapToPartySettingList(
        IEnumerable<QuestOrderParty> orderPartyUnits
    )
    {
        List<PartySettingList> result = [];

        foreach ((int index, QuestOrderParty orderParty) in orderPartyUnits.Index())
        {
            result.Add(
                new PartySettingList
                {
                    UnitNo = index + 1,
                    CharaId = orderParty.CharaId,
                    EquipWeaponBodyId = orderParty.WeaponBodyId,
                    EquipWeaponSkinId = orderParty.WeaponSkinId,
                }
            );
        }

        return result;
    }

    private static void ValidateCrests(QuestOrderParty orderParty)
    {
        if (
            orderParty.CrestSlotType1AbilityCrestId1 != AbilityCrestId.Empty
            || orderParty.CrestSlotType1AbilityCrestId2 != AbilityCrestId.Empty
            || orderParty.CrestSlotType1AbilityCrestId3 != AbilityCrestId.Empty
            || orderParty.CrestSlotType2AbilityCrestId1 != AbilityCrestId.Empty
            || orderParty.CrestSlotType2AbilityCrestId2 != AbilityCrestId.Empty
            || orderParty.CrestSlotType3AbilityCrestId1 != AbilityCrestId.Empty
            || orderParty.CrestSlotType3AbilityCrestId2 != AbilityCrestId.Empty
        )
        {
            throw new NotSupportedException(
                $"QuestOrderParty {orderParty.Id} has non-empty wyrmprints, which is not currently supported."
            );
        }
    }

    private static void ValidateChara(QuestOrderParty fixedUnit, CharaData charaData)
    {
        if (charaData.HasManaSpiral)
        {
            throw new NotSupportedException(
                $"QuestOrderParty {fixedUnit.Id} uses character {fixedUnit.CharaId} which has a mana spiral. "
                    + "Fixed party characters with mana spirals are not currently supported."
            );
        }

        if (fixedUnit.ReleaseManaCircle != 50)
        {
            throw new NotSupportedException(
                $"QuestOrderParty {fixedUnit.Id} has ReleaseManaCircle={fixedUnit.ReleaseManaCircle}, expected 50. "
                    + "Only MC50 fixed party characters are currently supported."
            );
        }
    }

    private static CharaList MapCharaData(QuestOrderParty fixedUnit, CharaData charaData)
    {
        return new CharaList
        {
            CharaId = fixedUnit.CharaId,
            Level = fixedUnit.CharaLevel,
            Rarity = fixedUnit.CharaRarity,
            HpPlusCount = fixedUnit.CharaHpPlusCount,
            AttackPlusCount = fixedUnit.CharaAttackPlusCount,

            // We can assume the character is maxed out but not spiraled because of ValidateCharaData
            Hp = charaData.MaxHp, // Despite this, Euden in Fight For Humanity has like 80,000 HP; possibly ignored by the client
            Attack = charaData.MaxAtk,
            Skill1Level = 3,
            Skill2Level = 2,
            Ability1Level = charaData.MaxAbility1Level,
            Ability2Level = charaData.MaxAbility2Level,
            Ability3Level = charaData.MaxAbility3Level,
            ExAbilityLevel = 5,
            ExAbility2Level = 5,
            BurstAttackLevel = charaData.DefaultBurstAttackLevel,
            LimitBreakCount = 4,
            IsUnlockEditSkill = true,
        };
    }

    private static DragonList MapDragonData(QuestOrderParty orderParty)
    {
        return new DragonList
        {
            DragonId = orderParty.DragonId,
            Level = orderParty.DragonLevel,
            LimitBreakCount = orderParty.DragonLimitBreakCount,
            HpPlusCount = orderParty.DragonHpPlusCount,
            AttackPlusCount = orderParty.DragonAttackPlusCount,
            Skill1Level = 1 + (orderParty.DragonLimitBreakCount / 4),
            Ability1Level = orderParty.DragonLimitBreakCount + 1,
            Ability2Level = orderParty.DragonLimitBreakCount + 1,
        };
    }
}
