using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

public class FortRepository : IFortRepository
{
    public const int MaximumCarpenterNum = 5;

    private readonly ApiContext apiContext;
    private readonly IPlayerDetailsService playerDetailsService;
    private readonly ILogger<FortRepository> logger;

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

    [Obsolete(ObsoleteReasons.UsePlayerDetailsService)]
    public IQueryable<DbFortBuild> GetBuilds(string accountId) =>
        this.apiContext.PlayerFortBuilds.Where(x => x.DeviceAccountId == accountId);

    public IQueryable<DbFortBuild> Builds =>
        this.apiContext.PlayerFortBuilds.Where(
            x => x.DeviceAccountId == this.playerDetailsService.AccountId
        );

    public IQueryable<DbFortDetail> Details =>
        this.apiContext.PlayerFortDetails.Where(
            x => x.DeviceAccountId == this.playerDetailsService.AccountId
        );

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

    public async Task UpdateFortMaximumCarpenter(string accountId, int carpenterNum)
    {
        DbFortDetail fortDetail = await apiContext.PlayerFortDetails
            .Where(x => x.DeviceAccountId == accountId)
            .FirstAsync();
        fortDetail.CarpenterNum = carpenterNum;
        apiContext.Entry(fortDetail).State = EntityState.Modified;
    }

    public async Task<DbFortBuild> GetBuilding(string accountId, long buildId)
    {
        DbFortBuild? fort = await apiContext.PlayerFortBuilds
            .Where(x => x.DeviceAccountId == accountId && x.BuildId == buildId)
            .FirstOrDefaultAsync();

        if (fort == null)
        {
            throw new InvalidOperationException(
                $"Could not get building {buildId} for account {accountId}."
            );
        }

        return fort;
    }

    public async Task AddBuild(DbFortBuild build)
    {
        await apiContext.PlayerFortBuilds.AddAsync(build);
    }

    public void UpdateBuild(DbFortBuild build)
    {
        apiContext.Entry(build).State = EntityState.Modified;
    }

    public void DeleteBuild(DbFortBuild build)
    {
        apiContext.Entry(build).State = EntityState.Deleted;
    }

    public async Task<DbFortBuild> UpgradeAtOnce(DbPlayerUserData userData, string accountId, 
        long buildId, PaymentTypes paymentType)
    {
        // Get building
        DbFortBuild build = await GetBuilding(
            accountId, buildId
        );

        if (build.BuildEndDate == DateTimeOffset.UnixEpoch)
        {
            throw new InvalidOperationException(
                $"This building is not currently being upgraded."
            );
        }

        int paymentHeld = GetUpgradePaymentHeld(userData, paymentType);
        int paymentCost = GetUpgradePaymentCost(
            paymentType,
            build.BuildStartDate,
            build.BuildEndDate
        );

        if (paymentHeld < paymentCost)
        {
            throw new InvalidOperationException(
                $"User did not have enough {paymentType}."
            );
        }

        ConsumePaymentCost(userData, paymentType, paymentCost);

        // Update build
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;
        UpdateBuild(build);

        return build;
    }

    public async Task<DbFortBuild> CancelUpgrade(string accountId, long buildId)
    {
        // Get building
        DbFortBuild build = await this.GetBuilding(accountId, buildId);

        if (build.BuildEndDate == DateTimeOffset.UnixEpoch)
        {
            throw new InvalidOperationException(
                $"This building is not currently being upgraded."
            );
        }

        // Cancel build
        build.Level--;
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;

        if (build.Level > 0)
        {
            this.UpdateBuild(build);
        }
        else
        {
            this.DeleteBuild(build);
        }

        return build;
    }

    public async Task<DbFortDetail> UpdateCarpenterUsage(string accountId)
    {
        int workingCarpenter = 0;
        List<DbFortBuild> AccountBuilds = Builds.Where(x => x.DeviceAccountId == accountId).ToList();
        await Parallel.ForEachAsync(AccountBuilds, async (item, cancellationToken) => {
            if (item.BuildEndDate != DateTimeOffset.UnixEpoch) 
                workingCarpenter++;
        });

        return await this.UpdateFortWorkingCarpenter(
            accountId,
            workingCarpenter
        );
    }

    private async Task<DbFortDetail> UpdateFortWorkingCarpenter(string accountId, int working_carpenter_num)
    {
        DbFortDetail fortDetail = await apiContext.PlayerFortDetails
            .Where(x => x.DeviceAccountId == accountId)
            .FirstAsync();

        fortDetail.WorkingCarpenterNum = working_carpenter_num;

        return fortDetail;
    }

    private int GetUpgradePaymentHeld(DbPlayerUserData userData, PaymentTypes paymentType)
    {
        return paymentType switch
        {
            PaymentTypes.Wyrmite => userData.Crystal,
            PaymentTypes.Diamantium => 0,// TODO How do I diamantium?
            PaymentTypes.HalidomHustleHammer => userData.BuildTimePoint,
            _ => throw new InvalidOperationException(
                    $"Invalid payment type for this operation."
                ),
        };
    }

    private int GetUpgradePaymentCost(PaymentTypes paymentType, DateTimeOffset BuildStartDate, DateTimeOffset BuildEndDate)
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
            return (int)Math.Floor((BuildEndDate - BuildStartDate).TotalMinutes / 12);
        }
    }

    public void ConsumePaymentCost(DbPlayerUserData userData, PaymentTypes paymentType, int paymentCost)
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
                throw new InvalidOperationException(
                    $"User did not have enough {paymentType}."
                );
        }
    }
}
