using DragaliaAPI.Features.Story;
using DragaliaAPI.Features.Tutorial;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

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
        if (await this.storyRepository.HasReadQuestStory(1001610))
        {
            await this.tutorialService.AddTutorialFlag(1006);
        }
    }
}
