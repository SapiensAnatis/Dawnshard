using System.Runtime.CompilerServices;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class PresentRepository : BaseRepository, IPresentRepository
{
    ApiContext apiContext;
    IUnitRepository unitRepository;

    public PresentRepository(ApiContext apiContext, IUnitRepository unitRepository)
        : base(apiContext)
    {
        this.apiContext = apiContext;
        this.unitRepository = unitRepository;
    }

    public IQueryable<DbPlayerPresentHistory> GetPlayerPresentHistory(string deviceAccountId)
    {
        return apiContext.PlayerPresentHistory.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public IQueryable<DbPlayerPresent> GetPlayerPresents(string deviceAccountId)
    {
        return apiContext.PlayerPresents.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public void AddPlayerPresents(IEnumerable<DbPlayerPresent> playerPresents)
    {
        apiContext.PlayerPresents.AddRange(playerPresents);
    }

    public async Task DeletePlayerPresents(string deviceAccountId, IEnumerable<long> presentIds)
    {
        ICollection<DbPlayerPresent> playerPresents = await apiContext.PlayerPresents
            .Where(x => x.DeviceAccountId == deviceAccountId && presentIds.Contains(x.PresentId))
            .ToListAsync();
        if (playerPresents.Count < presentIds.Count())
        {
            throw new ArgumentException(
                $"No presents present for ids: {string.Join(", ", presentIds.Except(playerPresents.Select(x => x.PresentId)))}"
            );
        }
        apiContext.RemoveRange(playerPresents);
    }

    public void AddPlayerPresentHistory(IEnumerable<DbPlayerPresentHistory> presentHistory)
    {
        apiContext.PlayerPresentHistory.AddRange(presentHistory);
    }
}
