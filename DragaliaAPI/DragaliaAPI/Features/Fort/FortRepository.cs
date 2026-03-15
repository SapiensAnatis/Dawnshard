using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Fort;

public partial class FortRepository(
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
        Log.InitializingFort(logger);

        if (
            !await apiContext.PlayerFortDetails.AnyAsync(x =>
                x.ViewerId == playerIdentityService.ViewerId
            )
        )
        {
            Log.InitializingPlayerFortDetail(logger);
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
            Log.InitializingHalidom(logger);
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
        Log.AddingSmithyToHalidom(logger);

        if (!await apiContext.PlayerFortBuilds.AnyAsync(x => x.PlantId == FortPlants.Smithy))
        {
            Log.InitializingSmithy(logger);
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

        Log.GrantingDojos(logger);

        foreach (FortPlants plant in plants)
        {
            await AddToStorage(plant, quantity: 2, isTotalQuantity: true);
        }
    }

    public async Task AddDragontree()
    {
        if (!await apiContext.PlayerFortBuilds.AnyAsync(x => x.PlantId == FortPlants.Dragontree))
        {
            Log.AddingDragontreeToStorage(logger);
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
            Log.CouldNotFindDetailsForPlayerCreatingAnew(logger);

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
            Log.FailedBuildLevelCheckRequestedPlantAtLevelButHadLevel(
                logger,
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
        Log.AddingCopiesOfToStorageIsTotalQuantity(logger, quantity, plant, isTotalQuantity);

        int startQuantity = isTotalQuantity
            ? await apiContext.PlayerFortBuilds.Where(x => x.PlantId == plant).CountAsync()
            : 0;

        Log.UserAlreadyOwnsCopies(logger, startQuantity);

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

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Initializing fort.")]
        public static partial void InitializingFort(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Initializing PlayerFortDetail.")]
        public static partial void InitializingPlayerFortDetail(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Initializing Halidom.")]
        public static partial void InitializingHalidom(ILogger logger);

        [LoggerMessage(LogLevel.Information, "Adding smithy to halidom.")]
        public static partial void AddingSmithyToHalidom(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Initializing Smithy.")]
        public static partial void InitializingSmithy(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Granting dojos.")]
        public static partial void GrantingDojos(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Adding dragontree to storage.")]
        public static partial void AddingDragontreeToStorage(ILogger logger);

        [LoggerMessage(LogLevel.Information, "Could not find details for player, creating anew...")]
        public static partial void CouldNotFindDetailsForPlayerCreatingAnew(ILogger logger);

        [LoggerMessage(
            LogLevel.Debug,
            "Failed build level check: requested plant {plant} at level {requestLevel}, but had level {actualLevel}"
        )]
        public static partial void FailedBuildLevelCheckRequestedPlantAtLevelButHadLevel(
            ILogger logger,
            FortPlants plant,
            int requestLevel,
            int actualLevel
        );

        [LoggerMessage(
            LogLevel.Debug,
            "Adding {quantity} copies of {plant} to storage (isTotalQuantity: {isTotalQuantity})"
        )]
        public static partial void AddingCopiesOfToStorageIsTotalQuantity(
            ILogger logger,
            int quantity,
            FortPlants plant,
            bool isTotalQuantity
        );

        [LoggerMessage(LogLevel.Debug, "User already owns {startQuantity} copies.")]
        public static partial void UserAlreadyOwnsCopies(ILogger logger, int startQuantity);
    }
}
