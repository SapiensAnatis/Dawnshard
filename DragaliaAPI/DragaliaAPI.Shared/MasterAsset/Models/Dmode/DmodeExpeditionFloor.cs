using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeExpeditionFloor(
    int Id,
    int FloorNum,
    int NeedTime,
    int RewardDmodePoint1,
    int RewardDmodePoint2
);
