using System.Collections.Immutable;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Story;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

/// <summary>
/// Grants buildings awarded by the main quest.
/// </summary>
public class V1Update : ISavefileUpdate
{
    private readonly IFortRepository fortRepository;
    private readonly IStoryRepository storyRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly ILogger<V1Update> logger;

    public V1Update(
        IFortRepository fortRepository,
        IStoryRepository storyRepository,
        IUserDataRepository userDataRepository,
        ILogger<V1Update> logger
    )
    {
        this.fortRepository = fortRepository;
        this.storyRepository = storyRepository;
        this.userDataRepository = userDataRepository;
        this.logger = logger;
    }

    public int SavefileVersion => 1;

    private static readonly ImmutableDictionary<int, FortPlants> QuestStoryFortPlantRewards =
        new Dictionary<int, FortPlants>()
        {
            { 1000607, FortPlants.WindDracolith },
            { 1000709, FortPlants.WaterDracolith },
            { 1000808, FortPlants.FlameDracolith },
            { 1000909, FortPlants.LightDracolith },
            { 1001009, FortPlants.ShadowDracolith },
        }.ToImmutableDictionary();

    public async Task Apply()
    {
        if (await this.storyRepository.HasReadQuestStory(TutorialService.TutorialStoryIds.Halidom))
        {
            await this.fortRepository.InitializeFort();
        }

        if (await this.storyRepository.HasReadQuestStory(TutorialService.TutorialStoryIds.Smithy))
        {
            await this.fortRepository.InitializeSmithy();
        }

        // Add dracoliths
        foreach ((int storyId, FortPlants plantId) in QuestStoryFortPlantRewards)
        {
            if (await this.storyRepository.HasReadQuestStory(storyId))
            {
                if (await this.fortRepository.Builds.AnyAsync(x => x.PlantId == plantId))
                {
                    this.logger.LogDebug("Skipping plant {plant} as already owned", plantId);
                    continue;
                }

                this.logger.LogDebug("Adding facility {plant}", plantId);
                await this.fortRepository.AddToStorage(plantId, quantity: 1, isTotalQuantity: true);
            }
        }

        if (
            (await this.userDataRepository.UserData.SingleAsync()).TutorialStatus
            >= TutorialService.TutorialStatusIds.Dojos
        )
        {
            this.logger.LogDebug("Adding dojos");
            await this.fortRepository.AddDojos();
        }
    }
}
