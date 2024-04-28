using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record MaterialData(
    Materials Id,
    MaterialCategory Category,
    MaterialRarities MaterialRarity,
    int Exp,
    int Plus
);
