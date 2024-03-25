namespace DragaliaAPI.Shared.MasterAsset.Models;

using MemoryPack;

[MemoryPackable]
public record AbilityLimitedGroup(int Id, double MaxLimitedValue);
