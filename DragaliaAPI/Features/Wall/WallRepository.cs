using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset;
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
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public async Task InitializeWall()
    {
        this.logger.LogInformation("Initializing wall.");

        for (int element = 0; element < 5; element++)
        {
            await apiContext.PlayerQuestWalls.AddAsync(
                    new DbPlayerQuestWall()
                    {
                        DeviceAccountId = this.playerIdentityService.AccountId,
                        WallId = WallService.FlameWallId + element,
                        WallLevel = 0,
                        IsStartNextLevel = false,
                    }
            );
        }
    }

    public async Task<DbPlayerQuestWall> GetQuestWall(int wallId)
    {
        DbPlayerQuestWall? questWall = await this.QuestWalls
            .Where(x => x.WallId == wallId)
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
