using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Present;

/// <summary>
/// Base present service to be used by other features to check/add present data.
/// </summary>
public class PresentService : IPresentService
{
    private readonly IPresentRepository presentRepository;
    private readonly IPlayerIdentityService playerIdentityService;

    public PresentService(
        IPresentRepository presentRepository,
        IPlayerIdentityService playerIdentityService
    )
    {
        this.presentRepository = presentRepository;
        this.playerIdentityService = playerIdentityService;
    }

    public async Task<PresentNotice> GetPresentNotice()
    {
        return new()
        {
            present_count = await this.presentRepository.Presents.CountAsync(
                x => x.ReceiveLimitTime == null
            ),
            present_limit_count = await this.presentRepository.Presents.CountAsync(
                x => x.ReceiveLimitTime != null
            ),
        };
    }

    public void AddPresent(Present present)
    {
        this.AddPresent(new[] { present });
    }

    public void AddPresent(IEnumerable<Present> presents)
    {
        this.presentRepository.AddPlayerPresents(
            presents.Select(x => x.ToEntity(this.playerIdentityService.ViewerId))
        );
    }
}
