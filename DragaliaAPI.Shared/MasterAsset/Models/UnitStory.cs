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
        Enum.IsDefined(typeof(Charas), this.ReleaseTriggerId)
            ? StoryTypes.Chara
            : StoryTypes.Dragon;

    public bool IsFirstEpisode => this.UnlockTriggerStoryId == default;
}
