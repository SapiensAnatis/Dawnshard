using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Present;

public class PresentRepository : IPresentRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;

    public PresentRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
    }

    public IQueryable<DbPlayerPresentHistory> PresentHistory =>
        apiContext.PlayerPresentHistory.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public IQueryable<DbPlayerPresent> Presents =>
        apiContext.PlayerPresents.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public void AddPlayerPresents(IEnumerable<DbPlayerPresent> playerPresents)
    {
        apiContext.PlayerPresents.AddRange(playerPresents);
    }

    public async Task DeletePlayerPresents(IEnumerable<long> presentIds)
    {
        ICollection<DbPlayerPresent> playerPresents = await apiContext.PlayerPresents
            .Where(
                x =>
                    x.DeviceAccountId == this.playerIdentityService.AccountId
                    && presentIds.Contains(x.PresentId)
            )
            .ToListAsync();

        if (playerPresents.Count < presentIds.Count())
        {
            throw new ArgumentException(
                $"No presents present for ids: {string.Join(", ", presentIds.Except(playerPresents.Select(x => x.PresentId)))}"
            );
        }

        apiContext.RemoveRange(playerPresents);
    }

    public void AddPlayerPresentHistory(DbPlayerPresentHistory presentHistory)
    {
        apiContext.PlayerPresentHistory.Add(presentHistory);
    }
}
