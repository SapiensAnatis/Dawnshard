using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Fort;

public class FortRepository : IFortRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<FortRepository> logger;

    private const int DefaultCarpenters = 2;

    public FortRepository(
        ApiContext apiContext,
        IPlayerIdentityService playerIdentityService,
        ILogger<FortRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    public IQueryable<DbFortBuild> Builds =>
        this.apiContext.PlayerFortBuilds.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId
        );

    public async Task InitializeFort()
    {
        this.logger.LogInformation("Initializing fort.");

        if (
            !await this.apiContext.PlayerFortDetails.AnyAsync(x =>
                x.ViewerId == this.playerIdentityService.ViewerId
            )
        )
        {
            this.logger.LogDebug("Initializing PlayerFortDetail.");
            await this.apiContext.PlayerFortDetails.AddAsync(
                new DbFortDetail()
                {
                    ViewerId = this.playerIdentityService.ViewerId,
                    CarpenterNum = DefaultCarpenters
                }
            );
        }

        if (!await this.Builds.AnyAsync(x => x.PlantId == FortPlants.TheHalidom))
        {
            this.logger.LogDebug("Initializing Halidom.");
            await apiContext.PlayerFortBuilds.AddAsync(
                new DbFortBuild()
                {
                    ViewerId = this.playerIdentityService.ViewerId,
                    PlantId = FortPlants.TheHalidom,
                    PositionX = 16, // Default Halidom position
                    PositionZ = 17,
                    LastIncomeDate = DateTimeOffset.UtcNow
                }
            );
        }
    }

    public async Task InitializeSmithy()
    {
        this.logger.LogInformation("Adding smithy to halidom.");

        if (!await this.Builds.AnyAsync(x => x.PlantId == FortPlants.Smithy))
        {
            this.logger.LogDebug("Initializing Smithy.");
            await this.apiContext.PlayerFortBuilds.AddAsync(
                new DbFortBuild
                {
                    ViewerId = this.playerIdentityService.ViewerId,
                    PlantId = FortPlants.Smithy,
                    PositionX = 21,
                    PositionZ = 3,
                    Level = 1
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
            FortPlants.WandDojo
        };

        this.logger.LogDebug("Granting dojos.");

        foreach (FortPlants plant in plants)
        {
            await AddToStorage(plant, quantity: 2, isTotalQuantity: true);
        }
    }

    public async Task AddDragontree()
    {
        if (!await this.Builds.AnyAsync(x => x.PlantId == FortPlants.Dragontree))
        {
            this.logger.LogDebug("Adding dragontree to storage.");
            await this.AddToStorage(FortPlants.Dragontree);
        }
    }

    public async Task<DbFortDetail> GetFortDetail()
    {
        DbFortDetail? details = await this.apiContext.PlayerFortDetails.FindAsync(
            this.playerIdentityService.ViewerId
        );

        if (details == null)
        {
            this.logger.LogInformation("Could not find details for player, creating anew...");

            details = (
                await this.apiContext.PlayerFortDetails.AddAsync(
                    new()
                    {
                        ViewerId = this.playerIdentityService.ViewerId,
                        CarpenterNum = DefaultCarpenters
                    }
                )
            ).Entity;
        }

        return details;
    }

    public async Task<bool> CheckPlantLevel(FortPlants plant, int requiredLevel)
    {
        int level = await this
            .Builds.Where(x => x.PlantId == plant)
            .Select(x => x.Level)
            .FirstOrDefaultAsync();
        bool result = level >= requiredLevel;

        if (!result)
        {
            this.logger.LogDebug(
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
            await apiContext.PlayerFortDetails.FindAsync(this.playerIdentityService.ViewerId)
            ?? throw new InvalidOperationException("Missing FortDetails!");

        fortDetail.CarpenterNum = carpenterNum;
    }

    public async Task<DbFortBuild> GetBuilding(long buildId)
    {
        DbFortBuild? fort = await this
            .Builds.Where(x => x.BuildId == buildId)
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
        this.logger.LogDebug(
            "Adding {quantity} copies of {plant} to storage (isTotalQuantity: {isTotalQuantity})",
            quantity,
            plant,
            isTotalQuantity
        );

        int startQuantity = isTotalQuantity
            ? await this.Builds.Where(x => x.PlantId == plant).CountAsync()
            : 0;

        this.logger.LogDebug("User already owns {startQuantity} copies.", startQuantity);

        if (startQuantity >= quantity)
            return;

        int actualLevel = level ?? MasterAssetUtils.GetInitialFortPlant(plant).Level;

        for (int i = startQuantity; i < quantity; i++)
        {
            await this.apiContext.PlayerFortBuilds.AddAsync(
                new DbFortBuild
                {
                    ViewerId = this.playerIdentityService.ViewerId,
                    PlantId = plant,
                    Level = actualLevel,
                    PositionX = -1,
                    PositionZ = -1,
                    BuildStartDate = DateTimeOffset.UnixEpoch,
                    BuildEndDate = DateTimeOffset.UnixEpoch,
                    LastIncomeDate = DateTimeOffset.UnixEpoch
                }
            );
        }
    }

    public void DeleteBuild(DbFortBuild build)
    {
        apiContext.PlayerFortBuilds.Remove(build);
    }

    public async Task<int> GetActiveCarpenters() =>
        await this.Builds.CountAsync(x => x.BuildEndDate != DateTimeOffset.UnixEpoch);
}
