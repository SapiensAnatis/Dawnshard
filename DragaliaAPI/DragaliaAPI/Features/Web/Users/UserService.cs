using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities.Owned;
using DragaliaAPI.Features.Web.Settings;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Web.Users;

internal sealed class UserService(
    IPlayerIdentityService playerIdentityService,
    SettingsService settingsService,
    ApiContext apiContext
)
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

    public async Task<UserProfile> GetUserProfile(CancellationToken cancellationToken)
    {
        var userData = await apiContext
            .PlayerUserData.Select(x => new
            {
                LastSaveImportTime = x.Owner!.LastSavefileImportTime,
                LastLoginTime = x.LastLoginTime,
            })
            .FirstAsync(cancellationToken);

        PlayerSettings settings = await settingsService.GetSettings(cancellationToken);

        return new UserProfile()
        {
            LastSaveImportTime = userData.LastSaveImportTime,
            LastLoginTime = userData.LastLoginTime,
            Settings = settings,
        };
    }
}
