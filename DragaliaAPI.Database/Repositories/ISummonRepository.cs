using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface ISummonRepository
{
    Task<DbPlayerBannerData> AddPlayerBannerData(string deviceAccountId, int bannerId);
    Task AddSummonHistory(IEnumerable<DbPlayerSummonHistory> summonHistory);
    Task<DbPlayerBannerData> GetPlayerBannerData(string deviceAccountId, int bannerId);
    Task<List<DbPlayerSummonHistory>> GetSummonHistory(string deviceAccountId);
}
