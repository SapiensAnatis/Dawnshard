using DragaliaAPI.Database;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Web.Users;

public class UserService(IPlayerIdentityService playerIdentityService, ApiContext apiContext)
{
    public Task<User> GetUser(CancellationToken cancellationToken) =>
        apiContext
            .Players.Where(x => x.ViewerId == playerIdentityService.ViewerId)
            .Select(x => new User()
            {
                Name = x.UserData!.Name,
                ViewerId = x.ViewerId,
                IsAdmin = x.IsAdmin,
            })
            .FirstAsync(cancellationToken);

    public Task<UserProfile> GetUserProfile(CancellationToken cancellationToken) =>
        apiContext
            .PlayerUserData.Select(x => new UserProfile()
            {
                LastSaveImportTime = x.Owner!.LastSavefileImportTime,
                LastLoginTime = x.LastLoginTime,
            })
            .FirstAsync(cancellationToken);
}
