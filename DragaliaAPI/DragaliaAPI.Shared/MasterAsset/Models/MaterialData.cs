using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record MaterialData(
    Materials Id,
    MaterialCategory Category,
    MaterialRarities MaterialRarity,
    int Exp,
    int Plus
);
