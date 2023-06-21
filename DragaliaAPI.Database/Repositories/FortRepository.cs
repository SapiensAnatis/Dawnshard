using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

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
        this.apiContext.PlayerFortBuilds.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public async Task InitializeFort()
    {
        this.logger.LogInformation("Initializing fort.");

        if (
            !await this.apiContext.PlayerFortDetails.AnyAsync(
                x => x.DeviceAccountId == this.playerIdentityService.AccountId
            )
        )
        {
            this.logger.LogDebug("Initializing PlayerFortDetail.");
            await this.apiContext.PlayerFortDetails.AddAsync(
                new DbFortDetail()
                {
                    DeviceAccountId = this.playerIdentityService.AccountId,
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
                    DeviceAccountId = this.playerIdentityService.AccountId,
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
                    DeviceAccountId = this.playerIdentityService.AccountId,
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
            FortPlants.SwordDojo,
            FortPlants.ManacasterDojo,
            FortPlants.SwordDojo,
            FortPlants.StaffDojo,
            FortPlants.WandDojo
        };

        this.logger.LogDebug("Granting dojos.");

        foreach (FortPlants plant in plants)
        {
            int currentAmount = await this.Builds.CountAsync(x => x.PlantId == plant);
            if (currentAmount >= 2)
                continue;

            for (int i = currentAmount; i != 2; i++)
            {
                await AddToStorage(plant, 1);
            }
        }
    }

    public async Task AddDragontree()
    {
        if (!await this.Builds.AnyAsync(x => x.PlantId == FortPlants.Dragontree))
        {
            this.logger.LogDebug("Adding dragontree to storage.");
            await this.AddToStorage(FortPlants.Dragontree, 1);
        }
    }

    public async Task<DbFortDetail> GetFortDetail()
    {
        DbFortDetail? details = await this.apiContext.PlayerFortDetails.FindAsync(
            this.playerIdentityService.AccountId
        );

        if (details == null)
        {
            this.logger.LogInformation("Could not find details for player, creating anew...");

            details = (
                await this.apiContext.PlayerFortDetails.AddAsync(
                    new()
                    {
                        DeviceAccountId = this.playerIdentityService.AccountId,
                        CarpenterNum = DefaultCarpenters
                    }
                )
            ).Entity;
        }

        return details;
    }

    public async Task<bool> CheckPlantLevel(FortPlants plant, int requiredLevel)
    {
        int level = await this.Builds
            .Where(x => x.PlantId == plant)
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

    public async Task GetFortPlantIdList(IEnumerable<int> fort_plant_id_list)
    {
        // What do
    }

    public async Task UpdateFortMaximumCarpenter(int carpenterNum)
    {
        DbFortDetail fortDetail =
            await apiContext.PlayerFortDetails.FindAsync(this.playerIdentityService.AccountId)
            ?? throw new InvalidOperationException("Missing FortDetails!");

        fortDetail.CarpenterNum = carpenterNum;
    }

    public async Task<DbFortBuild> GetBuilding(long buildId)
    {
        DbFortBuild? fort = await this.Builds
            .Where(x => x.BuildId == buildId)
            .FirstOrDefaultAsync();

        if (fort is null)
        {
            throw new InvalidOperationException(
                $"Could not get building {buildId} for account {this.playerIdentityService.AccountId}."
            );
        }

        return fort;
    }

    public async Task AddBuild(DbFortBuild build)
    {
        await apiContext.PlayerFortBuilds.AddAsync(build);
    }

    public async Task AddToStorage(FortPlants plant, int level)
    {
        await this.apiContext.PlayerFortBuilds.AddAsync(
            new DbFortBuild
            {
                DeviceAccountId = this.playerIdentityService.AccountId,
                PlantId = plant,
                Level = level,
                PositionX = -1,
                PositionZ = -1,
                BuildStartDate = DateTimeOffset.UnixEpoch,
                BuildEndDate = DateTimeOffset.UnixEpoch,
                LastIncomeDate = DateTimeOffset.UnixEpoch
            }
        );
    }

    public void DeleteBuild(DbFortBuild build)
    {
        apiContext.PlayerFortBuilds.Remove(build);
    }

    public async Task<int> GetActiveCarpenters()
    {
        // TODO: remove this when testcontainers gets merged in
        return this.apiContext.Database.IsSqlite()
            ? (await this.Builds.ToListAsync()).Count(
                x => x.BuildEndDate != DateTimeOffset.UnixEpoch
            )
            : await this.Builds.CountAsync(x => x.BuildEndDate != DateTimeOffset.UnixEpoch);
    }
}
