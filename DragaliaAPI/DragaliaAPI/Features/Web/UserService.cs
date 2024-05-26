using DragaliaAPI.Database;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Web;

public class UserService(IPlayerIdentityService playerIdentityService, ApiContext apiContext)
{
    public Task<User> GetUser(CancellationToken cancellationToken) =>
        apiContext
            .Players.Where(x => x.ViewerId == playerIdentityService.ViewerId)
            .Select(x => new User() { Name = x.UserData!.Name, ViewerId = x.ViewerId, })
            .FirstAsync(cancellationToken);
}

public class User
{
    public long ViewerId { get; init; }

    public required string Name { get; init; }
}
