using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login;

public class LoginBonusRepository(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
) : ILoginBonusRepository
{
    private IQueryable<DbLoginBonus> LoginBonuses { get; } =
        apiContext.LoginBonuses.Where(x => x.ViewerId == playerIdentityService.ViewerId);

    public async Task<DbLoginBonus> Get(int id)
    {
        return await this.LoginBonuses.FirstOrDefaultAsync(x => x.Id == id) ?? this.Add(id);
    }

    private DbLoginBonus Add(int id)
    {
        return apiContext
            .LoginBonuses.Add(
                new DbLoginBonus()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    CurrentDay = 0,
                    Id = id
                }
            )
            .Entity;
    }
}
