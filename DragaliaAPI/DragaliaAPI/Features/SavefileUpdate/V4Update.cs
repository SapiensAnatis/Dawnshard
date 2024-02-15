using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V4Update : ISavefileUpdate
{
    private readonly IFortRepository fortRepository;
    private readonly IStoryRepository storyRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly ITutorialService tutorialService;

    public V4Update(
        IFortRepository fortRepository,
        IStoryRepository storyRepository,
        IUserDataRepository userDataRepository,
        ITutorialService tutorialService
    )
    {
        this.fortRepository = fortRepository;
        this.storyRepository = storyRepository;
        this.userDataRepository = userDataRepository;
        this.tutorialService = tutorialService;
    }

    public int SavefileVersion => 4;

    private static readonly (int StoryId, int Status)[] ChapterTutorialStatus =
    {
        (1000111, 11001),
        (1000311, 30202),
        (1000412, 40101),
        (1000509, 50201),
        (1000607, 60999)
    };

    public async Task Apply()
    {
        DbPlayerUserData userData = await this.userDataRepository.UserData.SingleAsync();

        foreach ((int storyId, int status) in ChapterTutorialStatus)
        {
            if (await this.storyRepository.HasReadQuestStory(storyId))
                await this.tutorialService.UpdateTutorialStatus(status);
        }

        if (userData.TutorialFlagList.Contains(1001))
        {
            await this.fortRepository.AddDragontree();
        }

        if (await this.storyRepository.HasReadQuestStory(1000311))
        {
            await this.tutorialService.AddTutorialFlag(1005);
        }

        if (await this.storyRepository.HasReadQuestStory(1000909))
        {
            await this.tutorialService.AddTutorialFlag(1010);
        }

        if (await this.storyRepository.HasReadQuestStory(1001009))
        {
            await this.tutorialService.AddTutorialFlag(1014);
            await this.tutorialService.AddTutorialFlag(1016);
        }

        if (await this.storyRepository.HasReadQuestStory(1001610))
        {
            await this.tutorialService.AddTutorialFlag(1028);
        }

        if (await this.storyRepository.HasReadQuestStory(1001613))
        {
            await this.tutorialService.AddTutorialFlag(1030);
        }
    }
}
