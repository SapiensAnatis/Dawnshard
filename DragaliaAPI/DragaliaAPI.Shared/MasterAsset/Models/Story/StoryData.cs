using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Story;

[MemoryPackable]
public partial record StoryData(int id, int[] storyIds);
