using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

public class FortRepository : IFortRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerDetailsService playerDetailsService;
    private readonly ILogger<FortRepository> logger;

    private const int DefaultCarpenters = 2;

    public FortRepository(
        ApiContext apiContext,
        IPlayerDetailsService playerDetailsService,
        ILogger<FortRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerDetailsService = playerDetailsService;
        this.logger = logger;
    }

    //[Obsolete(ObsoleteReasons.UsePlayerDetailsService)]
    //public async Task<IEnumerable<DbFortBuild>> GetBuilds() =>
    //    await this.apiContext.PlayerFortBuilds
    //        .Where(x => x.DeviceAccountId == this.playerDetailsService.AccountId)
    //        .ToListAsync();

    public IQueryable<DbFortBuild> Builds =>
        this.apiContext.PlayerFortBuilds.Where(
            x => x.DeviceAccountId == this.playerDetailsService.AccountId
        );

    public async Task<DbFortDetail> GetFortDetail()
    {
        DbFortDetail? details = await this.apiContext.PlayerFortDetails.FindAsync(
            this.playerDetailsService.AccountId
        );

        if (details is null)
        {
            this.logger.LogInformation("Could not find details for player, creating anew...");

            details = (
                await this.apiContext.PlayerFortDetails.AddAsync(
                    new()
                    {
                        DeviceAccountId = this.playerDetailsService.AccountId,
                        CarpenterNum = DefaultCarpenters
                    }
                )
            ).Entity;

            await this.apiContext.SaveChangesAsync();
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
            await apiContext.PlayerFortDetails.FindAsync(this.playerDetailsService.AccountId)
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
                $"Could not get building {buildId} for account {this.playerDetailsService.AccountId}."
            );
        }

        return fort;
    }

    public async Task AddBuild(DbFortBuild build)
    {
        await apiContext.PlayerFortBuilds.AddAsync(build);
    }

    public void DeleteBuild(DbFortBuild build)
    {
        apiContext.Entry(build).State = EntityState.Deleted;
    }

    public async Task<DbFortBuild> UpgradeAtOnce(
        DbPlayerUserData userData,
        long buildId,
        PaymentTypes paymentType
    )
    {
        // Get building
        DbFortBuild build = await GetBuilding(buildId);

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

        // Update build
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;

        return build;
    }

    public async Task<int> GetActiveCarpenters()
    {
        // TODO: remove this when testcontainers gets merged in
        return this.apiContext.Database.IsSqlite()
            ? (await this.Builds.ToListAsync()).Count(x => x.BuildEndDate > DateTimeOffset.UtcNow)
            : await this.Builds.CountAsync(x => x.BuildEndDate > DateTimeOffset.UtcNow);
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
                throw new InvalidOperationException($"User did not have enough {paymentType}.");
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
