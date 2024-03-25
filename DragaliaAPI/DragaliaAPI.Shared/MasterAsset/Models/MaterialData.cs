using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

using MemoryPack;

[MemoryPackable]
public record MaterialData(
    Materials Id,
    MaterialCategory Category,
    MaterialRarities MaterialRarity,
    int Exp,
    int Plus
);
