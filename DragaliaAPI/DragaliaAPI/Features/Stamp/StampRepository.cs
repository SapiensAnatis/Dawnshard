using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.Stamp;

public class StampRepository : IStampRepository
{
    private readonly ApiContext context;
    private readonly IPlayerIdentityService playerIdentityService;

    public StampRepository(ApiContext context, IPlayerIdentityService playerIdentityService)
    {
        this.context = context;
        this.playerIdentityService = playerIdentityService;
    }

    public IQueryable<DbEquippedStamp> EquippedStamps =>
        this.context.EquippedStamps.Where(x => x.ViewerId == this.playerIdentityService.ViewerId);

    public Task SetEquipStampList(IEnumerable<DbEquippedStamp> newStampList)
    {
        this.context.EquippedStamps.RemoveRange(this.EquippedStamps);
        this.context.AddRange(newStampList);

        return Task.CompletedTask;
    }
}
