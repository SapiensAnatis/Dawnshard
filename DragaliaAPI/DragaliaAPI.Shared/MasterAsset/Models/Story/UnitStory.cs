using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Story;

[MemoryPackable]
public partial record UnitStory(
    int Id,
    int ReleaseTriggerId,
    int UnlockTriggerStoryId,
    int UnlockQuestStoryId
)
{
    public StoryTypes Type => ReleaseTriggerId > 20000000 ? StoryTypes.Dragon : StoryTypes.Chara;

    public bool IsFirstEpisode => UnlockTriggerStoryId == default;
}
