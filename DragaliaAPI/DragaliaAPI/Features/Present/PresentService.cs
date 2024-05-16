using DragaliaAPI.Database;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Present;

/// <summary>
/// Base present service to be used by other features to check/add present data.
/// </summary>
public class PresentService(
    IPresentRepository presentRepository,
    IPlayerIdentityService playerIdentityService,
    ApiContext apiContext
) : IPresentService
{
    public async Task<PresentNotice> GetPresentNotice()
    {
        return new()
        {
            PresentCount = await presentRepository.Presents.CountAsync(x =>
                x.ReceiveLimitTime == null
            ),
            PresentLimitCount = await presentRepository.Presents.CountAsync(x =>
                x.ReceiveLimitTime != null
            ),
        };
    }

    public void AddPresent(Present present)
    {
        this.AddPresent(new[] { present });
    }

    public void AddPresent(IEnumerable<Present> presents)
    {
        presentRepository.AddPlayerPresents(
            presents.Select(x => x.ToEntity(playerIdentityService.ViewerId))
        );
    }

    public IEnumerable<AtgenBuildEventRewardEntityList> GetTrackedPresentList() =>
        apiContext.PlayerPresents.Local.Select(x => new AtgenBuildEventRewardEntityList(
            x.EntityType,
            x.EntityId,
            x.EntityQuantity
        ));
}
