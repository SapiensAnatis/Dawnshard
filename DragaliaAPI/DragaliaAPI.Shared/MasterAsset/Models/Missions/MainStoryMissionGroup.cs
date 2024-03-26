using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

[MemoryPackable]
public partial record MainStoryMissionGroup(
    int Id,
    string Text,
    string LockText,
    int SortId,
    string AnimationInfo
);
