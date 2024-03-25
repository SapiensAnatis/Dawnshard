namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

using MemoryPack;

[MemoryPackable]
public record DmodeCharaLevel(int Id, int Level, int NecessaryExp, int TotalExp);
