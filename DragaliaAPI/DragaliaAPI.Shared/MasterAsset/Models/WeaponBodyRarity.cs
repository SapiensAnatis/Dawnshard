namespace DragaliaAPI.Shared.MasterAsset.Models;

using MemoryPack;

[MemoryPackable]
public record WeaponBodyRarity(
    int Id,
    int MaxLimitLevelByLimitBreak4,
    int MaxLimitLevelByLimitBreak8,
    int MaxLimitLevelByLimitBreak9
);
