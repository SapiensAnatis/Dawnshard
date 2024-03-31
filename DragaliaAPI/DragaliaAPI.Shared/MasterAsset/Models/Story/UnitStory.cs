using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Story;

public record UnitStory(
    int Id,
    int ReleaseTriggerId,
    int UnlockTriggerStoryId,
    int UnlockQuestStoryId
)
{
    [IgnoreMember]
    public StoryTypes Type => ReleaseTriggerId > 20000000 ? StoryTypes.Dragon : StoryTypes.Chara;

    [IgnoreMember]
    public bool IsFirstEpisode => UnlockTriggerStoryId == default;
}
