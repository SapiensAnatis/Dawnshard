using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models;

[MemoryPackable]
public partial record AbilityLimitedGroup(int Id, double MaxLimitedValue);
