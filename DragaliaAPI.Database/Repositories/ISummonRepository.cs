using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface ISummonRepository : IBaseRepository
{
    IQueryable<DbPlayerSummonHistory> SummonHistory { get; }

    Task AddSummonHistory(DbPlayerSummonHistory summonHistory);
    Task<DbPlayerBannerData> AddPlayerBannerData(int bannerId);
    Task<DbPlayerBannerData> GetPlayerBannerData(int bannerId);
}
