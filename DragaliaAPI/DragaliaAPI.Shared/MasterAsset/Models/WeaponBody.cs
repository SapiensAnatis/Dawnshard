using System.Collections.Frozen;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

/// <summary>
/// Weapon body MasterAsset model.
/// </summary>
/// <param name="Id">Unique weapon ID.</param>
/// <param name="WeaponSeriesId">Series the weapon belongs to</param>
/// <param name="WeaponType">Weapon type (sword, bow, etc...)</param>
/// <param name="Rarity">Weapon rarity.</param>
/// <param name="ElementalType">Weapon element.</param>
/// <param name="MaxLimitOverCount">Max unbind count.</param>
/// <param name="MaxEquipableCount">Max copy count/</param>
/// <param name="BaseHp"></param>
/// <param name="MaxHp1"></param>
/// <param name="MaxHp2"></param>
/// <param name="MaxHp3"></param>
/// <param name="BaseAtk"></param>
/// <param name="MaxAtk1"></param>
/// <param name="MaxAtk2"></param>
/// <param name="MaxAtk3"></param>
/// <param name="LimitOverCountPartyPower1"></param>
/// <param name="LimitOverCountPartyPower2"></param>
/// <param name="CrestSlotType1BaseCount"></param>
/// <param name="CrestSlotType1MaxCount"></param>
/// <param name="CrestSlotType2BaseCount"></param>
/// <param name="CrestSlotType2MaxCount"></param>
/// <param name="CrestSlotType3BaseCount"></param>
/// <param name="CrestSlotType3MaxCount"></param>
/// <param name="WeaponPassiveAbilityGroupId"></param>
/// <param name="WeaponBodyBuildupGroupId"></param>
/// <param name="MaxWeaponPassiveCharaCount">Seems to be 0/1 for bonus available/unavailable respectively.</param>
/// <param name="WeaponPassiveEffHp">Weapon bonus HP magnitude</param>
/// <param name="WeaponPassiveEffAtk">Weapon bonus Str magnitude</param>
/// <param name="RewardWeaponSkinId1">First weapon skin available from upgrading</param>
/// <param name="RewardWeaponSkinId2"></param>
/// <param name="RewardWeaponSkinId3"></param>
/// <param name="RewardWeaponSkinId4"></param>
/// <param name="RewardWeaponSkinId5"></param>
/// <param name="NeedFortCraftLevel">Required Smithy level to craft this weapon</param>
public record WeaponBody(
    WeaponBodies Id,
    WeaponSeries WeaponSeriesId,
    WeaponTypes WeaponType,
    int Rarity,
    UnitElement ElementalType,
    int MaxLimitOverCount,
    int WeaponPassiveAbilityGroupId,
    int WeaponBodyBuildupGroupId,
    int MaxWeaponPassiveCharaCount,
    float WeaponPassiveEffHp,
    float WeaponPassiveEffAtk,
    int Abilities11,
    int Abilities12,
    int Abilities13,
    int Abilities21,
    int Abilities22,
    int Abilities23,
    int ChangeSkillId1,
    int ChangeSkillId2,
    int ChangeSkillId3,
    int RewardWeaponSkinId1,
    int RewardWeaponSkinId2,
    int RewardWeaponSkinId3,
    int RewardWeaponSkinId4,
    int RewardWeaponSkinId5,
    int NeedFortCraftLevel,
    WeaponBodies NeedCreateWeaponBodyId1,
    WeaponBodies NeedCreateWeaponBodyId2,
    long CreateCoin,
    Materials CreateEntityId1,
    int CreateEntityQuantity1,
    Materials CreateEntityId2,
    int CreateEntityQuantity2,
    Materials CreateEntityId3,
    int CreateEntityQuantity3,
    Materials CreateEntityId4,
    int CreateEntityQuantity4,
    Materials CreateEntityId5,
    int CreateEntityQuantity5,
    int LimitOverCountPartyPower1,
    int LimitOverCountPartyPower2,
    int BaseHp,
    int MaxHp1,
    int MaxHp2,
    int MaxHp3,
    int BaseAtk,
    int MaxAtk1,
    int MaxAtk2,
    int MaxAtk3
)
{
    public FrozenDictionary<Materials, int> CreateMaterialMap { get; } =
        new List<KeyValuePair<Materials, int>>()
        {
            new(CreateEntityId1, CreateEntityQuantity1),
            new(CreateEntityId2, CreateEntityQuantity2),
            new(CreateEntityId3, CreateEntityQuantity3),
            new(CreateEntityId4, CreateEntityQuantity4),
            new(CreateEntityId5, CreateEntityQuantity5),
        }
            .Where(x => x.Key != Materials.Empty)
            .ToFrozenDictionary(x => x.Key, x => x.Value);

    /// <summary>
    /// Get the row id in the WeaponBodyBuildupGroup table corresponding to a particular operation and step
    /// for upgrading this weapon.
    /// <remarks>
    /// Covers unbinding, copies, weapon bonuses, slots, refinement -- all except
    /// stats and passives which are defined separately in this class.
    /// </remarks>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public int GetBuildupGroupId(BuildupPieceTypes type, int step) =>
        int.Parse($"{this.WeaponBodyBuildupGroupId}{(int)type:00}{step:00}");

    /// <summary>
    /// Get the row id in the WeaponBodyBuildupLevel table corresponding to a particular level up of this weapon.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <returns>The row id.</returns>
    public int GetBuildupLevelId(int level) => int.Parse($"{this.Rarity}010{level:00}");

    /// <summary>
    /// Get the row id in the WeaponPassiveAbility table corresponding to a passive ability of this weapon.
    /// </summary>
    /// <param name="abilityNo">The ability number.</param>
    /// <returns>The row id.</returns>
    public int GetPassiveAbilityId(int abilityNo) =>
        int.Parse($"{this.WeaponPassiveAbilityGroupId}{abilityNo:00}");

    public readonly int[] Hp = { BaseHp, MaxHp1, MaxHp2, MaxHp3 };

    public readonly int[] Atk = { BaseAtk, MaxAtk1, MaxAtk2, MaxAtk3 };

    public readonly int[][] Abilities =
    {
        new[] { Abilities11, Abilities12, Abilities13 },
        new[] { Abilities21, Abilities22, Abilities23 }
    };

    public int GetAbility(int abilityNo, int level)
    {
        int[] pool = Abilities[abilityNo - 1];
        return level < 1 || level > 3 ? 0 : pool[level - 1];
    }
};
