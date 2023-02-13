using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

public class FortRepository : BaseRepository, IFortRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerDetailsService playerDetailsService;
    private readonly ILogger<FortRepository> logger;

    public FortRepository(
        ApiContext apiContext,
        IPlayerDetailsService playerDetailsService,
        ILogger<FortRepository> logger
    ) 
        : base(apiContext)
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

    public async Task UpdateFortCarpenterNum(string accountId, int carpenterNum)
    {
        DbFortDetail fortDetail = await apiContext.PlayerFortDetails
            .Where(x => x.DeviceAccountId == accountId)
            .FirstAsync();
        fortDetail.CarpenterNum = carpenterNum;
        apiContext.Entry(fortDetail).State = EntityState.Modified;
    }

    public async Task UpdateFortWorkingCarpenter(string accountId, int working_carpenter_num)
    {
        DbFortDetail fortDetail = await apiContext.PlayerFortDetails
            .Where(x => x.DeviceAccountId == accountId)
            .FirstAsync();
        fortDetail.WorkingCarpenterNum = working_carpenter_num;
        apiContext.Entry(fortDetail).State = EntityState.Modified;
    }

    public async Task<DbFortBuild> GetBuilding(string accountId, long buildId)
    {
        return await apiContext.PlayerFortBuilds
            .Where(x => x.DeviceAccountId == accountId && x.BuildId == buildId)
            .FirstAsync();
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
}
