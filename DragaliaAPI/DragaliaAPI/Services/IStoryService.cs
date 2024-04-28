using DragaliaAPI.DTO;
using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Services;

public interface IStoryService
{
    Task<bool> CheckStoryEligibility(StoryTypes type, int storyId);
    Task<IList<AtgenBuildEventRewardEntityList>> ReadStory(StoryTypes type, int storyId);
}
