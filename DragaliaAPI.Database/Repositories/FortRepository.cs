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

        foreach (FortPlants plant in plants)
        {
            await AddToStorage(plant, 1);
            await AddToStorage(plant, 1);
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
            new DbFortBuild()
            {
                DeviceAccountId = this.playerIdentityService.AccountId,
                PlantId = plant,
                Level = level,
                PositionX = -1,
                PositionZ = -1
            }
        );
    }

    public void DeleteBuild(DbFortBuild build)
    {
        apiContext.PlayerFortBuilds.Remove(build);
    }

    public void ConsumeUpgradeAtOnceCost(
        DbPlayerUserData userData,
        DbFortBuild build,
        PaymentTypes paymentType
    )
    {
        if (build.BuildStatus is not FortBuildStatus.Construction)
        {
            throw new InvalidOperationException($"This building is not currently being upgraded.");
        }

        int paymentHeld = GetUpgradePaymentHeld(userData, paymentType);
        int paymentCost = GetUpgradePaymentCost(
            paymentType,
            build.BuildStartDate,
            build.BuildEndDate
        );

        if (paymentHeld < paymentCost)
        {
            throw new InvalidOperationException($"User did not have enough {paymentType}.");
        }

        ConsumePaymentCost(userData, paymentType, paymentCost);
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

    public void ConsumePaymentCost(
        DbPlayerUserData userData,
        PaymentTypes paymentType,
        int paymentCost
    )
    {
        switch (paymentType)
        {
            case PaymentTypes.Wyrmite:
                userData.Crystal -= paymentCost;
                break;
            case PaymentTypes.Diamantium:
                // TODO How do I diamantium?
                break;
            case PaymentTypes.HalidomHustleHammer:
                userData.BuildTimePoint -= paymentCost;
                break;
            default:
                throw new InvalidOperationException($"Invalid payment type {paymentType}.");
        }
    }

    private int GetUpgradePaymentHeld(DbPlayerUserData userData, PaymentTypes paymentType)
    {
        return paymentType switch
        {
            PaymentTypes.Wyrmite => userData.Crystal,
            PaymentTypes.Diamantium => 0, // TODO How do I diamantium?
            PaymentTypes.HalidomHustleHammer => userData.BuildTimePoint,
            _ => throw new InvalidOperationException($"Invalid payment type for this operation."),
        };
    }

    private int GetUpgradePaymentCost(
        PaymentTypes paymentType,
        DateTimeOffset BuildStartDate,
        DateTimeOffset BuildEndDate
    )
    {
        if (paymentType == PaymentTypes.HalidomHustleHammer)
        {
            return 1; // Only 1 Hammer is consumed
        }
        else
        {
            // Construction can be immediately completed by spending either Wyrmite or Diamantium,
            // where the amount required depends on the time left until construction is complete.
            // This amount scales at 1 per 12 minutes, or 5 per hour.
            // https://dragalialost.wiki/w/Facilities
            return (int)Math.Ceiling((BuildEndDate - BuildStartDate).TotalMinutes / 12);
        }
    }
}
