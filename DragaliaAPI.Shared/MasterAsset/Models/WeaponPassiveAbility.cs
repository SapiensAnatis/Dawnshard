using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record WeaponPassiveAbility(
    int Id,
    int WeaponPassiveAbilityGroupId,
    int WeaponPassiveAbilityNo,
    WeaponTypes WeaponType,
    UnitElement ElementalType,
    int UnlockConditionLimitBreakCount,
    int RewardWeaponSkinId1,
    int RewardWeaponSkinId2,
    long UnlockCoin,
    Materials UnlockMaterialId1,
    int UnlockMaterialQuantity1,
    Materials UnlockMaterialId2,
    int UnlockMaterialQuantity2,
    Materials UnlockMaterialId3,
    int UnlockMaterialQuantity3,
    Materials UnlockMaterialId4,
    int UnlockMaterialQuantity4,
    Materials UnlockMaterialId5,
    int UnlockMaterialQuantity5
)
{
    public Dictionary<Materials, int> MaterialMap { get; } =
        new List<KeyValuePair<Materials, int>>()
        {
            new(UnlockMaterialId1, UnlockMaterialQuantity1),
            new(UnlockMaterialId2, UnlockMaterialQuantity2),
            new(UnlockMaterialId3, UnlockMaterialQuantity3),
            new(UnlockMaterialId4, UnlockMaterialQuantity4),
            new(UnlockMaterialId5, UnlockMaterialQuantity5),
        }
            .Where(x => x.Key != Materials.Empty)
            .ToDictionary(x => x.Key, x => x.Value);
};
