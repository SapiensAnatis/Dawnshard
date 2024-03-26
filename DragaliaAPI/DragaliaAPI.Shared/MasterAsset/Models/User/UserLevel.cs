using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.User;

[MemoryPackable]
public partial record UserLevel(
    int Id,
    int NecessaryExp,
    int TotalExp,
    int StaminaSingle,
    int FriendCount
);
