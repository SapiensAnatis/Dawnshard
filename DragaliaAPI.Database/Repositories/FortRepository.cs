using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared;
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

    public async Task<bool> InitFortDetail(string accountId)
    {
        await apiContext.PlayerFortDetails.AddAsync(
            new DbFortDetail()
            {
                DeviceAccountId = accountId,
                CarpenterNum = 2,
                MaxCarpenterCount = 5,
                WorkingCarpenterNum = 0
            }
        );
        await apiContext.SaveChangesAsync();
        return true;
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
        apiContext.Remove(build);
    }

    public async Task<DbFortBuild> CancelUpgrade(string accountId, long buildId, int workingCarpenterNum)
    {
        // Get building
        DbFortBuild build = await this.GetBuilding(accountId, buildId);

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

    public async Task<DbFortDetail> IncrementCarpenterUsage(string accountId, int workingCarpenterNum)
    {
        return await this.UpdateFortWorkingCarpenter(
            accountId,
            workingCarpenterNum + 1
        );
    }

    public async Task<DbFortDetail> DecrementCarpenterUsage(string accountId, int workingCarpenterNum)
    {
        workingCarpenterNum--;
        if (workingCarpenterNum < 0)
        {
            workingCarpenterNum = 0;
        }

        return await this.UpdateFortWorkingCarpenter(
            accountId,
            workingCarpenterNum
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
}
