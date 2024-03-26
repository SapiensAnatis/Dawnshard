using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models;

[MemoryPackable]
public partial record WeaponBodyRarity(
    int Id,
    int MaxLimitLevelByLimitBreak4,
    int MaxLimitLevelByLimitBreak8,
    int MaxLimitLevelByLimitBreak9
);
