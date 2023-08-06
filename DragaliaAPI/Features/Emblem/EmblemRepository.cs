using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Helpers;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Emblem;

public class EmblemRepository(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    IDateTimeProvider dateTimeProvider
) : IEmblemRepository
{
    public IQueryable<DbEmblem> Emblems =>
        apiContext.Emblems.Where(x => x.DeviceAccountId == playerIdentityService.AccountId);

    public async Task<IEnumerable<DbEmblem>> GetEmblemsAsync()
    {
        return await Emblems.ToListAsync();
    }

    public DbEmblem AddEmblem(Emblems emblem)
    {
        return apiContext.Emblems
            .Add(
                new DbEmblem
                {
                    DeviceAccountId = playerIdentityService.AccountId,
                    EmblemId = emblem,
                    GetTime = dateTimeProvider.UtcNow,
                    IsNew = true
                }
            )
            .Entity;
    }

    public async Task<bool> HasEmblem(Emblems emblem)
    {
        return await apiContext.Emblems.FindAsync(playerIdentityService.AccountId, emblem) != null;
    }
}
