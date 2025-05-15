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

    public static IQueryable<HelperProjection> ProjectToHelperProjection(
        this IQueryable<DbPlayerHelper> helpers
    )
    {
        return helpers.Select(x => new HelperProjection(
            new AtgenSupportChara()
            {
                CharaId = x.EquippedChara!.CharaId,
                Level = x.EquippedChara.Level,
                AdditionalMaxLevel = x.EquippedChara.AdditionalMaxLevel,
                Rarity = x.EquippedChara.Rarity,
                Hp = x.EquippedChara.Hp,
                Attack = x.EquippedChara.Attack,
                HpPlusCount = x.EquippedChara.HpPlusCount,
                AttackPlusCount = x.EquippedChara.AttackPlusCount,
                StatusPlusCount = 0,
                Ability1Level = x.EquippedChara.Ability1Level,
                Ability2Level = x.EquippedChara.Ability2Level,
                Ability3Level = x.EquippedChara.Ability3Level,
                ExAbilityLevel = x.EquippedChara.ExAbilityLevel,
                ExAbility2Level = x.EquippedChara.ExAbility2Level,
                Skill1Level = x.EquippedChara.Skill1Level,
                Skill2Level = x.EquippedChara.Skill2Level,
                IsUnlockEditSkill = x.EquippedChara.IsUnlockEditSkill,
            },
            x.EquippedDragon != null
                ? new AtgenSupportDragon
                {
                    DragonKeyId = (ulong)x.EquippedDragon.DragonKeyId,
                    DragonId = x.EquippedDragon.DragonId,
                    Level = x.EquippedDragon.Level,
                    Hp = 0, // HP and attack are not in the regular dragon model, maybe unused?
                    Attack = 0,
                    Skill1Level = x.EquippedDragon.Skill1Level,
                    Ability1Level = x.EquippedDragon.Ability1Level,
                    Ability2Level = x.EquippedDragon.Ability2Level,
                    HpPlusCount = x.EquippedDragon.HpPlusCount,
                    AttackPlusCount = x.EquippedDragon.AttackPlusCount,
                    StatusPlusCount = 0,
                    LimitBreakCount = x.EquippedDragon.LimitBreakCount,
                }
                : null,
            x.EquippedWeaponBody != null
                ? new AtgenSupportWeaponBody
                {
                    WeaponBodyId = x.EquippedWeaponBody.WeaponBodyId,
                    BuildupCount = x.EquippedWeaponBody.BuildupCount,
                    LimitBreakCount = x.EquippedWeaponBody.LimitBreakCount,
                    LimitOverCount = x.EquippedWeaponBody.LimitOverCount,
                    AdditionalEffectCount = x.EquippedWeaponBody.AdditionalEffectCount,
                    EquipableCount = x.EquippedWeaponBody.EquipableCount,
                    AdditionalCrestSlotType1Count =
                        x.EquippedWeaponBody.AdditionalCrestSlotType1Count,
                    AdditionalCrestSlotType2Count =
                        x.EquippedWeaponBody.AdditionalCrestSlotType2Count,
                    AdditionalCrestSlotType3Count =
                        x.EquippedWeaponBody.AdditionalCrestSlotType3Count,
                }
                : null,
            x.EquippedCrestSlotType1Crest1 != null
                ? new AtgenSupportCrestSlotType1List()
                {
                    AbilityCrestId = x.EquippedCrestSlotType1Crest1.AbilityCrestId,
                    BuildupCount = x.EquippedCrestSlotType1Crest1.BuildupCount,
                    LimitBreakCount = x.EquippedCrestSlotType1Crest1.LimitBreakCount,
                    HpPlusCount = x.EquippedCrestSlotType1Crest1.HpPlusCount,
                    AttackPlusCount = x.EquippedCrestSlotType1Crest1.AttackPlusCount,
                    EquipableCount = x.EquippedCrestSlotType1Crest1.EquipableCount,
                }
                : null,
            x.EquippedCrestSlotType1Crest2 != null
                ? new AtgenSupportCrestSlotType1List()
                {
                    AbilityCrestId = x.EquippedCrestSlotType1Crest2.AbilityCrestId,
                    BuildupCount = x.EquippedCrestSlotType1Crest2.BuildupCount,
                    LimitBreakCount = x.EquippedCrestSlotType1Crest2.LimitBreakCount,
                    HpPlusCount = x.EquippedCrestSlotType1Crest2.HpPlusCount,
                    AttackPlusCount = x.EquippedCrestSlotType1Crest2.AttackPlusCount,
                    EquipableCount = x.EquippedCrestSlotType1Crest2.EquipableCount,
                }
                : null,
            x.EquippedCrestSlotType1Crest3 != null
                ? new AtgenSupportCrestSlotType1List()
                {
                    AbilityCrestId = x.EquippedCrestSlotType1Crest3.AbilityCrestId,
                    BuildupCount = x.EquippedCrestSlotType1Crest3.BuildupCount,
                    LimitBreakCount = x.EquippedCrestSlotType1Crest3.LimitBreakCount,
                    HpPlusCount = x.EquippedCrestSlotType1Crest3.HpPlusCount,
                    AttackPlusCount = x.EquippedCrestSlotType1Crest3.AttackPlusCount,
                    EquipableCount = x.EquippedCrestSlotType1Crest3.EquipableCount,
                }
                : null,
            x.EquippedCrestSlotType2Crest1 != null
                ? new AtgenSupportCrestSlotType1List()
                {
                    AbilityCrestId = x.EquippedCrestSlotType2Crest1.AbilityCrestId,
                    BuildupCount = x.EquippedCrestSlotType2Crest1.BuildupCount,
                    LimitBreakCount = x.EquippedCrestSlotType2Crest1.LimitBreakCount,
                    HpPlusCount = x.EquippedCrestSlotType2Crest1.HpPlusCount,
                    AttackPlusCount = x.EquippedCrestSlotType2Crest1.AttackPlusCount,
                    EquipableCount = x.EquippedCrestSlotType2Crest1.EquipableCount,
                }
                : null,
            x.EquippedCrestSlotType2Crest2 != null
                ? new AtgenSupportCrestSlotType1List()
                {
                    AbilityCrestId = x.EquippedCrestSlotType2Crest2.AbilityCrestId,
                    BuildupCount = x.EquippedCrestSlotType2Crest2.BuildupCount,
                    LimitBreakCount = x.EquippedCrestSlotType2Crest2.LimitBreakCount,
                    HpPlusCount = x.EquippedCrestSlotType2Crest2.HpPlusCount,
                    AttackPlusCount = x.EquippedCrestSlotType2Crest2.AttackPlusCount,
                    EquipableCount = x.EquippedCrestSlotType2Crest2.EquipableCount,
                }
                : null,
            x.EquippedCrestSlotType3Crest1 != null
                ? new AtgenSupportCrestSlotType1List()
                {
                    AbilityCrestId = x.EquippedCrestSlotType3Crest1.AbilityCrestId,
                    BuildupCount = x.EquippedCrestSlotType3Crest1.BuildupCount,
                    LimitBreakCount = x.EquippedCrestSlotType3Crest1.LimitBreakCount,
                    HpPlusCount = x.EquippedCrestSlotType3Crest1.HpPlusCount,
                    AttackPlusCount = x.EquippedCrestSlotType3Crest1.AttackPlusCount,
                    EquipableCount = x.EquippedCrestSlotType3Crest1.EquipableCount,
                }
                : null,
            x.EquippedCrestSlotType3Crest2 != null
                ? new AtgenSupportCrestSlotType1List()
                {
                    AbilityCrestId = x.EquippedCrestSlotType3Crest2.AbilityCrestId,
                    BuildupCount = x.EquippedCrestSlotType3Crest2.BuildupCount,
                    LimitBreakCount = x.EquippedCrestSlotType3Crest2.LimitBreakCount,
                    HpPlusCount = x.EquippedCrestSlotType3Crest2.HpPlusCount,
                    AttackPlusCount = x.EquippedCrestSlotType3Crest2.AttackPlusCount,
                    EquipableCount = x.EquippedCrestSlotType3Crest2.EquipableCount,
                }
                : null,
            x.EquippedTalisman != null
                ? new AtgenSupportTalisman
                {
                    TalismanKeyId = (ulong)x.EquippedTalisman.TalismanKeyId,
                    TalismanId = x.EquippedTalisman.TalismanId,
                    TalismanAbilityId1 = x.EquippedTalisman.TalismanAbilityId1,
                    TalismanAbilityId2 = x.EquippedTalisman.TalismanAbilityId2,
                    TalismanAbilityId3 = x.EquippedTalisman.TalismanAbilityId3,
                    AdditionalHp = x.EquippedTalisman.AdditionalHp,
                    AdditionalAttack = x.EquippedTalisman.AdditionalAttack,
                }
                : null,
            new UserDataProjection(
                x.Owner!.UserData!.ViewerId,
                x.Owner.UserData.Name,
                x.Owner.UserData.Level,
                x.Owner.UserData.LastLoginTime,
                x.Owner.UserData.EmblemId
            ),
            x.EquippedDragon != null
                ? x.Owner!.DragonReliabilityList.First(y =>
                    y.DragonId == x.EquippedDragon.DragonId
                ).Level
                : 0,
            x.Owner!.PartyPower!.MaxPartyPower,
            x.EquippedChara.ManaNodeUnlockCount
        ));
    }

    public static UserSupportList MapToUserSupportList(this HelperProjection helper)
    {
        UserSupportList mappedHelper = new()
        {
            SupportChara = helper.EquippedChara,
            SupportDragon = helper.EquippedDragon ?? new() { DragonKeyId = 0 },
            SupportWeaponBody = helper.EquippedWeaponBody ?? new() { WeaponBodyId = 0 },
            SupportCrestSlotType1List =
            [
                helper.EquippedCrestSlotType1Crest1 ?? new() { AbilityCrestId = 0 },
                helper.EquippedCrestSlotType1Crest2 ?? new() { AbilityCrestId = 0 },
                helper.EquippedCrestSlotType1Crest3 ?? new() { AbilityCrestId = 0 },
            ],
            SupportCrestSlotType2List =
            [
                helper.EquippedCrestSlotType2Crest1 ?? new() { AbilityCrestId = 0 },
                helper.EquippedCrestSlotType2Crest2 ?? new() { AbilityCrestId = 0 },
            ],
            SupportCrestSlotType3List =
            [
                helper.EquippedCrestSlotType3Crest1 ?? new() { AbilityCrestId = 0 },
                helper.EquippedCrestSlotType3Crest2 ?? new() { AbilityCrestId = 0 },
            ],
            SupportTalisman = helper.EquippedTalisman ?? new() { TalismanKeyId = 0 },
            ViewerId = (ulong)helper.UserData.ViewerId,
            Name = helper.UserData.Name,
            Level = helper.UserData.Level,
            LastLoginDate = helper.UserData.LastLoginDate,
            EmblemId = helper.UserData.EmblemId,
            MaxPartyPower = helper.PartyPower ?? 0,
            Guild = new() { GuildId = 0 },
        };

        return mappedHelper;
    }
}
