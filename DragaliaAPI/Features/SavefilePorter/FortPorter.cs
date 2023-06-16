using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefilePorter;

public class FortPorter : ISavefilePorter
{
    private readonly IFortRepository fortRepository;
    private readonly IStoryRepository storyRepository;

    public FortPorter(IFortRepository fortRepository, IStoryRepository storyRepository)
    {
        this.fortRepository = fortRepository;
        this.storyRepository = storyRepository;
    }

    public DateTime ModificationDate { get; } = new(2023, 06, 14);

    public async Task Port()
    {
        if (await this.storyRepository.HasReadQuestStory(TutorialService.TutorialStoryIds.Halidom))
        {
            await this.fortRepository.InitializeFort();
        }
    }
}
