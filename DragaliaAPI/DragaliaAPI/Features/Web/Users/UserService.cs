using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities.Owned;
using DragaliaAPI.Features.Login.Auth;
using DragaliaAPI.Features.Web.Settings;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Web.Users;

internal sealed class UserService(
    IPlayerIdentityService playerIdentityService,
    ISessionService sessionService,
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

    public async Task<ImpersonationSession> GetImpersonationSession(
        CancellationToken cancellationToken
    )
    {
        Session? session = await sessionService.LoadImpersonationSession(
            playerIdentityService.AccountId
        );

        return new ImpersonationSession(session?.ViewerId);
    }

    public async Task ClearImpersonationSession(CancellationToken cancellationToken)
    {
        await sessionService.ClearUserImpersonation();
    }

    public async Task<string?> GetImpersonationTargetAccountId(
        long impersonatedViewerId,
        CancellationToken cancellationToken
    )
    {
        return await apiContext
            .Players.IgnoreQueryFilters()
            .Where(x => x.ViewerId == impersonatedViewerId)
            .Select(x => x.AccountId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ImpersonationSession> SetImpersonationSession(
        string impersonatedAccountId,
        long impersonatedViewerId,
        CancellationToken cancellationToken
    )
    {
        await sessionService.StartUserImpersonation(impersonatedAccountId, impersonatedViewerId);

        return new ImpersonationSession(impersonatedViewerId);
    }
}
