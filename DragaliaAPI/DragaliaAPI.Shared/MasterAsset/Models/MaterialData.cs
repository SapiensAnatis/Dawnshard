using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models;

[MemoryPackable]
public partial record MaterialData(
    Materials Id,
    MaterialCategory Category,
    MaterialRarities MaterialRarity,
    int Exp,
    int Plus
);
