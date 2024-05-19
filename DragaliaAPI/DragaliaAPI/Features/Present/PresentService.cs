using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Present;

/// <summary>
/// Base present service to be used by other features to check/add present data.
/// </summary>
public class PresentService(IPlayerIdentityService playerIdentityService, ApiContext apiContext)
    : IPresentService
{
    private readonly List<Present> addedPresents = [];

    public IReadOnlyList<Present> AddedPresents => this.addedPresents;

    public async Task<PresentNotice> GetPresentNotice()
    {
        return new()
        {
            PresentCount = await apiContext.PlayerPresents.CountAsync(x =>
                x.ReceiveLimitTime == null
            ),
            PresentLimitCount = await apiContext.PlayerPresents.CountAsync(x =>
                x.ReceiveLimitTime != null
            ),
        };
    }

    public void AddPresent(Present present)
    {
        apiContext.PlayerPresents.Add(present.ToEntity(playerIdentityService.ViewerId));
        this.addedPresents.Add(present);
    }

    public void AddPresent(IEnumerable<Present> presents)
    {
        foreach (Present present in presents)
        {
            this.AddPresent(present);
        }
    }
}
