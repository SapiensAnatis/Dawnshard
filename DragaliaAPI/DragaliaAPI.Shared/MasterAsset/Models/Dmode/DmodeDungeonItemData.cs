using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeDungeonItemData(
    int Id,
    int Rarity,
    DmodeDungeonItemType DmodeDungeonItemType,
    int DungeonItemTargetId,
    int UseCount,
    int SellDmodePoint1,
    int SellDmodePoint2
);
