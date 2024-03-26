using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

[MemoryPackable]
public partial record SpecialMissionGroup(int Id, int SortId, string Text);
