namespace DragaliaAPI.Shared.MasterAsset.Models.Story;

using MemoryPack;

[MemoryPackable]
public record StoryData(int id, int[] storyIds);
