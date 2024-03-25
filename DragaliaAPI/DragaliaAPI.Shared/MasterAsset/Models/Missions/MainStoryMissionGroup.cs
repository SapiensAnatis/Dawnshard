namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

using MemoryPack;

[MemoryPackable]
public record MainStoryMissionGroup(
    int Id,
    string Text,
    string LockText,
    int SortId,
    string AnimationInfo
);
