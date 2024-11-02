using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Fort;

public class FortRepository(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    TimeProvider timeProvider,
    ILogger<FortRepository> logger
) : IFortRepository
{
    private const int DefaultCarpenters = 2;

    [Obsolete("This entity has a global query filter, use ApiContext.PlayerFortBuilds instead.")]
    public IQueryable<DbFortBuild> Builds => apiContext.PlayerFortBuilds;

    public async Task InitializeFort()
    {
        logger.LogInformation("Initializing fort.");

        if (
            !await apiContext.PlayerFortDetails.AnyAsync(x =>
                x.ViewerId == playerIdentityService.ViewerId
            )
        )
        {
            logger.LogDebug("Initializing PlayerFortDetail.");
            await apiContext.PlayerFortDetails.AddAsync(
                new DbFortDetail()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    CarpenterNum = DefaultCarpenters,
                }
            );
        }

        if (!await apiContext.PlayerFortBuilds.AnyAsync(x => x.PlantId == FortPlants.TheHalidom))
        {
            logger.LogDebug("Initializing Halidom.");
            await apiContext.PlayerFortBuilds.AddAsync(
                new DbFortBuild()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    PlantId = FortPlants.TheHalidom,
                    PositionX = 16, // Default Halidom position
                    PositionZ = 17,
                    LastIncomeDate = DateTimeOffset.UtcNow,
                }
            );
        }
    }

    public async Task InitializeSmithy()
    {
        logger.LogInformation("Adding smithy to halidom.");

        if (!await apiContext.PlayerFortBuilds.AnyAsync(x => x.PlantId == FortPlants.Smithy))
        {
            logger.LogDebug("Initializing Smithy.");
            await apiContext.PlayerFortBuilds.AddAsync(
                new DbFortBuild
                {
                    ViewerId = playerIdentityService.ViewerId,
                    PlantId = FortPlants.Smithy,
                    PositionX = 21,
                    PositionZ = 3,
                    Level = 1,
                }
            );
        }
    }

    public async Task AddDojos()
    {
        FortPlants[] plants =
        {
            FortPlants.SwordDojo,
            FortPlants.AxeDojo,
            FortPlants.BladeDojo,
            FortPlants.BowDojo,
            FortPlants.DaggerDojo,
            FortPlants.LanceDojo,
            FortPlants.ManacasterDojo,
            FortPlants.StaffDojo,
            FortPlants.WandDojo,
        };

        logger.LogDebug("Granting dojos.");

        foreach (FortPlants plant in plants)
        {
            await AddToStorage(plant, quantity: 2, isTotalQuantity: true);
        }
    }

    public async Task AddDragontree()
    {
        if (!await apiContext.PlayerFortBuilds.AnyAsync(x => x.PlantId == FortPlants.Dragontree))
        {
            logger.LogDebug("Adding dragontree to storage.");
            await this.AddToStorage(FortPlants.Dragontree);
        }
    }

    public async Task<DbFortDetail> GetFortDetail()
    {
        DbFortDetail? details = await apiContext.PlayerFortDetails.FindAsync(
            playerIdentityService.ViewerId
        );

        if (details == null)
        {
            logger.LogInformation("Could not find details for player, creating anew...");

            details = (
                await apiContext.PlayerFortDetails.AddAsync(
                    new()
                    {
                        ViewerId = playerIdentityService.ViewerId,
                        CarpenterNum = DefaultCarpenters,
                    }
                )
            ).Entity;
        }

        return details;
    }

    public async Task<bool> CheckPlantLevel(FortPlants plant, int requiredLevel)
    {
        int level = await apiContext
            .PlayerFortBuilds.Where(x => x.PlantId == plant)
            .Select(x => x.Level)
            .FirstOrDefaultAsync();
        bool result = level >= requiredLevel;

        if (!result)
        {
            logger.LogDebug(
                "Failed build level check: requested plant {plant} at level {requestLevel}, but had level {actualLevel}",
                plant,
                requiredLevel,
                level
            );
        }

        return result;
    }

    public async Task UpdateFortMaximumCarpenter(int carpenterNum)
    {
        DbFortDetail fortDetail =
            await apiContext.PlayerFortDetails.FindAsync(playerIdentityService.ViewerId)
            ?? throw new InvalidOperationException("Missing FortDetails!");

        fortDetail.CarpenterNum = carpenterNum;
    }

    public async Task<DbFortBuild> GetBuilding(long buildId)
    {
        DbFortBuild? fort = await apiContext
            .PlayerFortBuilds.Where(x => x.BuildId == buildId)
            .FirstOrDefaultAsync();

        if (fort is null)
        {
            throw new InvalidOperationException($"Could not get building {buildId}");
        }

        return fort;
    }

    public async Task AddBuild(DbFortBuild build)
    {
        await apiContext.PlayerFortBuilds.AddAsync(build);
    }

    public async Task AddToStorage(
        FortPlants plant,
        int quantity = 1,
        bool isTotalQuantity = false,
        int? level = null
    )
    {
        logger.LogDebug(
            "Adding {quantity} copies of {plant} to storage (isTotalQuantity: {isTotalQuantity})",
            quantity,
            plant,
            isTotalQuantity
        );

        int startQuantity = isTotalQuantity
            ? await apiContext.PlayerFortBuilds.Where(x => x.PlantId == plant).CountAsync()
            : 0;

        logger.LogDebug("User already owns {startQuantity} copies.", startQuantity);

        if (startQuantity >= quantity)
            return;

        int actualLevel = level ?? MasterAssetUtils.GetInitialFortPlant(plant).Level;

        for (int i = startQuantity; i < quantity; i++)
        {
            await apiContext.PlayerFortBuilds.AddAsync(
                new DbFortBuild
                {
                    ViewerId = playerIdentityService.ViewerId,
                    PlantId = plant,
                    Level = actualLevel,
                    PositionX = -1,
                    PositionZ = -1,
                    BuildStartDate = DateTimeOffset.UnixEpoch,
                    BuildEndDate = DateTimeOffset.UnixEpoch,
                    LastIncomeDate = DateTimeOffset.UnixEpoch,
                }
            );
        }
    }

    public void DeleteBuild(DbFortBuild build)
    {
        apiContext.PlayerFortBuilds.Remove(build);
    }

    public async Task<int> GetActiveCarpenters() =>
        await apiContext.PlayerFortBuilds.CountAsync(x =>
            x.BuildEndDate > timeProvider.GetUtcNow()
        );
}
