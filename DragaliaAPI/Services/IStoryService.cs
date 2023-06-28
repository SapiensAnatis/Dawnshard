using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Services;

public interface IStoryService
{
    Task<bool> CheckStoryEligibility(StoryTypes type, int storyId);
    Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadStory(StoryTypes type, int storyId);
}
