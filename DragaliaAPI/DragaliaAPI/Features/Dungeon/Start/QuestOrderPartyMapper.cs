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

        foreach ((int index, QuestOrderParty orderParty) in orderPartyUnits.Index())
        {
            CharaData charaData = MasterAsset.CharaData[orderParty.CharaId];
            DragonData dragonData = MasterAsset.DragonData[orderParty.DragonId];

            ValidateCharaData(charaData, orderParty);
            ValidateCrests(orderParty);

            units.Add(
                new PartyUnitList
                {
                    Position = index,
                    CharaData = MapCharaData(orderParty, charaData),
                    DragonData = MapDragonData(orderParty),
                    DragonReliabilityLevel = dragonData.DefaultReliabilityLevel,
                    WeaponBodyData = new GameWeaponBody
                    {
                        WeaponBodyId = orderParty.WeaponBodyId,
                        BuildupCount = orderParty.WeaponBodyBuildupCount,
                        LimitBreakCount = orderParty.WeaponBodyLimitBreakCount,
                        LimitOverCount = orderParty.WeaponBodyLimitOverCount,
                    },
                    WeaponSkinData = new GameWeaponSkin(orderParty.WeaponSkinId),
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
                    UnitNo = index,
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

    private static void ValidateCharaData(CharaData charaData, QuestOrderParty orderParty)
    {
        if (charaData.HasManaSpiral)
        {
            throw new NotSupportedException(
                $"QuestOrderParty {orderParty.Id} uses character {orderParty.CharaId} which has a mana spiral. "
                    + "Fixed party characters with mana spirals are not currently supported."
            );
        }

        if (orderParty.ReleaseManaCircle != 50)
        {
            throw new NotSupportedException(
                $"QuestOrderParty {orderParty.Id} has ReleaseManaCircle={orderParty.ReleaseManaCircle}, expected 50. "
                    + "Only MC50 fixed party characters are currently supported."
            );
        }
    }

    private static CharaList MapCharaData(QuestOrderParty orderParty, CharaData charaData)
    {
        return new CharaList
        {
            CharaId = orderParty.CharaId,
            Level = orderParty.CharaLevel,
            Rarity = orderParty.CharaRarity,
            HpPlusCount = orderParty.CharaHpPlusCount,
            AttackPlusCount = orderParty.CharaAttackPlusCount,

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
