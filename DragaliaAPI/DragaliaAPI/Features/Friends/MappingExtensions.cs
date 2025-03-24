using System.Linq.Expressions;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Friends;

public static class MappingExtensions
{
    private static readonly Expression<
        Func<DbPlayerHelper, SettingSupport>
    > MapToSettingSupportExpression = x => new SettingSupport()
    {
        CharaId = x.CharaId,
        EquipDragonKeyId = (ulong)(x.EquipDragonKeyId ?? 0),
        EquipWeaponBodyId = x.EquipWeaponBodyId ?? 0,
        EquipCrestSlotType1CrestId1 = x.EquipCrestSlotType1CrestId1 ?? 0,
        EquipCrestSlotType1CrestId2 = x.EquipCrestSlotType1CrestId2 ?? 0,
        EquipCrestSlotType1CrestId3 = x.EquipCrestSlotType1CrestId3 ?? 0,
        EquipCrestSlotType2CrestId1 = x.EquipCrestSlotType2CrestId1 ?? 0,
        EquipCrestSlotType2CrestId2 = x.EquipCrestSlotType2CrestId2 ?? 0,
        EquipCrestSlotType3CrestId1 = x.EquipCrestSlotType3CrestId1 ?? 0,
        EquipCrestSlotType3CrestId2 = x.EquipCrestSlotType3CrestId2 ?? 0,
        EquipTalismanKeyId = (ulong)(x.EquipTalismanKeyId ?? 0),
    };

    private static readonly Func<DbPlayerHelper, SettingSupport> MapToSettingSupportFunc =
        MapToSettingSupportExpression.Compile();

    public static SettingSupport MapToSettingSupport(this DbPlayerHelper helper) =>
        MapToSettingSupportFunc(helper);

    public static IQueryable<SettingSupport> ProjectToSettingSupport(
        this IQueryable<DbPlayerHelper> helpers
    )
    {
        return helpers.Select(MapToSettingSupportExpression);
    }

    public static AtgenSupportChara MapToSupportChara(this DbPlayerCharaData dbPlayerCharaData)
    {
        return new()
        {
            CharaId = dbPlayerCharaData.CharaId,
            Level = dbPlayerCharaData.Level,
            AdditionalMaxLevel = dbPlayerCharaData.AdditionalMaxLevel,
            Rarity = dbPlayerCharaData.Rarity,
            Hp = dbPlayerCharaData.Hp,
            Attack = dbPlayerCharaData.Attack,
            HpPlusCount = dbPlayerCharaData.HpPlusCount,
            AttackPlusCount = dbPlayerCharaData.AttackPlusCount,
            StatusPlusCount = 0,
            Ability1Level = dbPlayerCharaData.Ability1Level,
            Ability2Level = dbPlayerCharaData.Ability2Level,
            Ability3Level = dbPlayerCharaData.Ability3Level,
            ExAbilityLevel = dbPlayerCharaData.ExAbilityLevel,
            ExAbility2Level = dbPlayerCharaData.ExAbility2Level,
            Skill1Level = dbPlayerCharaData.Skill1Level,
            Skill2Level = dbPlayerCharaData.Skill2Level,
            IsUnlockEditSkill = dbPlayerCharaData.IsUnlockEditSkill,
        };
    }

    public static AtgenSupportDragon MapToSupportDragon(this DbPlayerDragonData? dbPlayerDragonData)
    {
        // Where fields are null, the official servers returned a 'partial object' in which only the ID was set to 0.
        // Not sure if we need to as well, but better to verge on the side of authenticity.
        if (dbPlayerDragonData is null)
        {
            return new() { DragonKeyId = 0 };
        }

        return new AtgenSupportDragon
        {
            DragonKeyId = (ulong)dbPlayerDragonData.DragonKeyId,
            DragonId = dbPlayerDragonData.DragonId,
            Level = dbPlayerDragonData.Level,
            Hp = 0, // HP and attack are not in the regular dragon model, maybe unused?
            Attack = 0,
            Skill1Level = dbPlayerDragonData.Skill1Level,
            Ability1Level = dbPlayerDragonData.Ability1Level,
            Ability2Level = dbPlayerDragonData.Ability2Level,
            HpPlusCount = dbPlayerDragonData.HpPlusCount,
            AttackPlusCount = dbPlayerDragonData.AttackPlusCount,
            StatusPlusCount = 0,
            LimitBreakCount = dbPlayerDragonData.LimitBreakCount,
        };
    }

    public static AtgenSupportWeaponBody MapToSupportWeaponBody(this DbWeaponBody? dbWeaponBody)
    {
        if (dbWeaponBody is null)
        {
            return new() { WeaponBodyId = 0 };
        }

        return new AtgenSupportWeaponBody
        {
            WeaponBodyId = dbWeaponBody.WeaponBodyId,
            BuildupCount = dbWeaponBody.BuildupCount,
            LimitBreakCount = dbWeaponBody.LimitBreakCount,
            LimitOverCount = dbWeaponBody.LimitOverCount,
            AdditionalEffectCount = dbWeaponBody.AdditionalEffectCount,
            EquipableCount = dbWeaponBody.EquipableCount,
            AdditionalCrestSlotType1Count = dbWeaponBody.AdditionalCrestSlotType1Count,
            AdditionalCrestSlotType2Count = dbWeaponBody.AdditionalCrestSlotType2Count,
            AdditionalCrestSlotType3Count = dbWeaponBody.AdditionalCrestSlotType3Count,
        };
    }

    public static AtgenSupportCrestSlotType1List MapToSupportAbilityCrest(
        this DbAbilityCrest? dbAbilityCrest
    )
    {
        if (dbAbilityCrest is null)
        {
            return new() { AbilityCrestId = 0 };
        }

        return new AtgenSupportCrestSlotType1List(
            dbAbilityCrest.AbilityCrestId,
            dbAbilityCrest.BuildupCount,
            dbAbilityCrest.LimitBreakCount,
            dbAbilityCrest.HpPlusCount,
            dbAbilityCrest.AttackPlusCount,
            dbAbilityCrest.EquipableCount
        );
    }

    public static AtgenSupportTalisman MapToSupportTalisman(this DbTalisman? dbTalisman)
    {
        if (dbTalisman is null)
        {
            return new() { TalismanId = 0 };
        }

        return new AtgenSupportTalisman
        {
            TalismanKeyId = (ulong)dbTalisman.TalismanKeyId,
            TalismanId = dbTalisman.TalismanId,
            TalismanAbilityId1 = dbTalisman.TalismanAbilityId1,
            TalismanAbilityId2 = dbTalisman.TalismanAbilityId2,
            TalismanAbilityId3 = dbTalisman.TalismanAbilityId3,
            AdditionalHp = dbTalisman.AdditionalHp,
            AdditionalAttack = dbTalisman.AdditionalAttack,
        };
    }

    public static IQueryable<HelperProjection> ProjectToHelperProjection(
        this IQueryable<DbPlayerHelper> helpers
    )
    {
        return helpers.Select(x => new HelperProjection(
            x.EquippedChara!,
            x.EquippedDragon,
            x.EquippedWeaponBody,
            x.EquippedCrestSlotType1Crest1,
            x.EquippedCrestSlotType1Crest2,
            x.EquippedCrestSlotType1Crest3,
            x.EquippedCrestSlotType2Crest1,
            x.EquippedCrestSlotType2Crest2,
            x.EquippedCrestSlotType3Crest1,
            x.EquippedCrestSlotType3Crest2,
            x.EquippedTalisman,
            x.Owner!.UserData!,
            x.Owner!.DragonReliabilityList.FirstOrDefault(y =>
                y.DragonId == x.EquippedDragon!.DragonId
            ),
            x.Owner!.PartyPower!
        ));
    }

    public static UserSupportList MapToUserSupportList(this HelperProjection helper)
    {
        UserSupportList mappedHelper = new()
        {
            SupportChara = helper.EquippedChara.MapToSupportChara(),
        };

        mappedHelper.SupportDragon = helper.EquippedDragon.MapToSupportDragon();
        mappedHelper.SupportWeaponBody = helper.EquippedWeaponBody.MapToSupportWeaponBody();

        DbAbilityCrest?[] slot1Crests =
        [
            helper.EquippedCrestSlotType1Crest1,
            helper.EquippedCrestSlotType1Crest2,
            helper.EquippedCrestSlotType1Crest3,
        ];

        DbAbilityCrest?[] slot2Crests =
        [
            helper.EquippedCrestSlotType2Crest1,
            helper.EquippedCrestSlotType2Crest2,
        ];

        DbAbilityCrest?[] slot3Crests =
        [
            helper.EquippedCrestSlotType3Crest1,
            helper.EquippedCrestSlotType3Crest2,
        ];

        mappedHelper.SupportCrestSlotType1List = slot1Crests
            .Select(x => x.MapToSupportAbilityCrest())
            .ToList();
        mappedHelper.SupportCrestSlotType2List = slot2Crests
            .Select(x => x.MapToSupportAbilityCrest())
            .ToList();
        mappedHelper.SupportCrestSlotType3List = slot3Crests
            .Select(x => x.MapToSupportAbilityCrest())
            .ToList();

        mappedHelper.SupportTalisman = helper.EquippedTalisman.MapToSupportTalisman();

        mappedHelper.ViewerId = (ulong)helper.UserData.ViewerId;
        mappedHelper.Name = helper.UserData.Name;
        mappedHelper.Level = helper.UserData.Level;
        mappedHelper.LastLoginDate = helper.UserData.LastLoginTime;
        mappedHelper.EmblemId = helper.UserData.EmblemId;
        mappedHelper.MaxPartyPower = helper.PartyPower?.MaxPartyPower ?? 0;
        mappedHelper.Guild = new() { GuildId = 0 };

        return mappedHelper;
    }
}
