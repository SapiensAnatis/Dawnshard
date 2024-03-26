using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models;

[MemoryPackable]
public partial record AbilityCrestRarity(
    int Id,
    int MaxLimitLevelByLimitBreak0,
    int MaxLimitLevelByLimitBreak1,
    int MaxLimitLevelByLimitBreak2,
    int MaxLimitLevelByLimitBreak3,
    int MaxLimitLevelByLimitBreak4,
    int MaxHpPlusCount,
    int MaxAtkPlusCount
);
