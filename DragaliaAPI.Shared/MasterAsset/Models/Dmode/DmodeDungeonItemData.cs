using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

public record DmodeDungeonItemData(
    int Id,
    int Rarity,
    DmodeDungeonItemType DmodeDungeonItemType,
    int DungeonItemTargetId,
    int UseCount,
    int SellDmodePoint1,
    int SellDmodePoint2
);
