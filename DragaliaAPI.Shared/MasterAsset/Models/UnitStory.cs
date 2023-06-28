using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record UnitStory(
    int Id,
    int ReleaseTriggerId,
    int UnlockTriggerStoryId,
    int UnlockQuestStoryId
)
{
    public StoryTypes Type =>
        this.ReleaseTriggerId > 20000000 ? StoryTypes.Dragon : StoryTypes.Chara;

    public bool IsFirstEpisode => this.UnlockTriggerStoryId == default;
}
