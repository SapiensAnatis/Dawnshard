using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Wall;

public class WallRepository : IWallRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<WallRepository> logger;

    public WallRepository(
        ApiContext apiContext,
        IPlayerIdentityService playerIdentityService,
        ILogger<WallRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    public IQueryable<DbPlayerQuestWall> QuestWalls =>
        this.apiContext.PlayerQuestWalls.Where(
            x => x.ViewerId == this.playerIdentityService.ViewerId
        );

    public async Task InitializeWall()
    {
        if (await IsInitialized())
        {
            return;
        }

        this.logger.LogInformation("Initializing wall.");

        for (int element = 0; element < 5; element++)
        {
            await apiContext.PlayerQuestWalls.AddAsync(
                new DbPlayerQuestWall()
                {
                    ViewerId = this.playerIdentityService.ViewerId,
                    WallId = WallService.FlameWallId + element,
                    WallLevel = 0, // Indicates you have not completed level 1. Goes up to 80 upon completing level 80
                    IsStartNextLevel = false,
                }
            );
        }
    }

    public async Task<bool> IsInitialized()
    {
        return await this.QuestWalls.AnyAsync();
    }

    public async Task<DbPlayerQuestWall> GetQuestWall(int wallId)
    {
        DbPlayerQuestWall? questWall = await this.QuestWalls.Where(x => x.WallId == wallId)
            .FirstOrDefaultAsync();

        if (questWall is null)
        {
            throw new InvalidOperationException(
                $"Could not get questwall {questWall} for account {this.playerIdentityService.AccountId}."
            );
        }

        return questWall;
    }
}
