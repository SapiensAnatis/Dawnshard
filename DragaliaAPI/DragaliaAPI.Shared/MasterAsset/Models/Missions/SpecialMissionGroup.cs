namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

using MemoryPack;

[MemoryPackable]
public record SpecialMissionGroup(int Id, int SortId, string Text);
