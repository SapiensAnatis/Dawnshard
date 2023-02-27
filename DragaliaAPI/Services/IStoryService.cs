using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Services;

public interface IStoryService
{
    Task<bool> CheckCastleStoryEligibility(int storyId);
    Task<bool> CheckUnitStoryEligibility(int storyId);
    Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadCastleStory(int storyId);
    Task<IEnumerable<AtgenQuestStoryRewardList>> ReadQuestStory(int storyId);
    Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadUnitStory(int storyId);
}
