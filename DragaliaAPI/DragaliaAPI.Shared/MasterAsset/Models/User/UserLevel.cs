namespace DragaliaAPI.Shared.MasterAsset.Models.User;

using MemoryPack;

[MemoryPackable]
public record UserLevel(int Id, int NecessaryExp, int TotalExp, int StaminaSingle, int FriendCount);
