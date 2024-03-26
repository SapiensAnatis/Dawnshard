using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeCharaLevel(int Id, int Level, int NecessaryExp, int TotalExp);
