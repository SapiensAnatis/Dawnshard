using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Story;

public interface IStoryService
{
    Task<bool> CheckStoryEligibility(StoryTypes type, int storyId);

    Task<IList<AtgenBuildEventRewardEntityList>> ReadStory(StoryTypes type, int storyId);

    EntityResult GetEntityResult();
}
