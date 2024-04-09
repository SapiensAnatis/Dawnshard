using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class SummonRepository : BaseRepository, ISummonRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;

    public SummonRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
        : base(apiContext)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
    }

    public IQueryable<DbPlayerSummonHistory> SummonHistory =>
        this.apiContext.PlayerSummonHistory.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId
        );

    public async Task AddSummonHistory(DbPlayerSummonHistory summonHistory)
    {
        await apiContext.PlayerSummonHistory.AddRangeAsync(summonHistory);
    }
}
