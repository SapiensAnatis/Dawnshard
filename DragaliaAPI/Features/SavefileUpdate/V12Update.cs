using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Services.Game;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V12Update(
    IWallRepository wallRepository,
    IWallService wallService,
    IStoryRepository storyRepository
) : ISavefileUpdate
{
    public int SavefileVersion => 12;

    public async Task Apply()
    {
        if (await storyRepository.HasReadQuestStory(TutorialService.TutorialStoryIds.MercurialGauntlet))
        {
            await wallRepository.InitializeWall();
        }
    }
}
