using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

[UsedImplicitly]
public class V18Update(
    IWallService wallService,
    IStoryRepository storyRepository,
    IMissionRepository missionRepository,
    ILogger<V18Update> logger
) : ISavefileUpdate
{
    private readonly IWallService wallService = wallService;
    private readonly IStoryRepository storyRepository = storyRepository;
    private readonly IMissionRepository missionRepository = missionRepository;
    private readonly ILogger<V18Update> logger = logger;

    public int SavefileVersion => 18;

    public async Task Apply()
    {
        if (
            !await this.storyRepository.HasReadQuestStory(
                TutorialService.TutorialStoryIds.MercurialGauntlet
            )
        )
        {
            this.logger.LogInformation("Player has not unlocked wall yet, skipping...");
            return;
        }

        if (
            await this.missionRepository.Missions.AnyAsync(x =>
                x.Type == MissionType.Normal && x.Id == 10010101 // Clear The Mercurial Gauntlet
            )
        )
        {
            this.logger.LogInformation("Player has already unlocked wall missions, skipping...");
            return;
        }

        this.logger.LogInformation("Initializing wall missions");
        await this.wallService.InitializeWallMissions();
    }
}
