using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V7Update : ISavefileUpdate
{
    private readonly IStoryRepository storyRepository;
    private readonly ITutorialService tutorialService;

    public V7Update(IStoryRepository storyRepository, ITutorialService tutorialService)
    {
        this.storyRepository = storyRepository;
        this.tutorialService = tutorialService;
    }

    public int SavefileVersion => 7;

    public async Task Apply()
    {
        if (await storyRepository.HasReadQuestStory(1001610))
        {
            await tutorialService.AddTutorialFlag(1006);
        }
    }
}
