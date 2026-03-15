using System.Security.Claims;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static bool HasDawnshardIdentity(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.Identities.Any(x => x.Label == AuthConstants.IdentityLabels.Dawnshard);

    public static void InitializeDawnshardIdentity(
        this ClaimsPrincipal claimsPrincipal,
        string accountId,
        long viewerId,
        string? playerName = null,
        bool isAdmin = false
    )
    {
        ClaimsIdentity dawnshardIdentity = new([
            new(CustomClaimType.AccountId, accountId),
            new(CustomClaimType.ViewerId, viewerId.ToString()),
        ])
        {
            Label = AuthConstants.IdentityLabels.Dawnshard,
        };

        if (playerName is not null)
        {
            dawnshardIdentity.AddClaim(new(CustomClaimType.PlayerName, playerName));
        }

        if (isAdmin)
        {
            dawnshardIdentity.AddClaim(new(CustomClaimType.IsAdmin, "true"));
        }

        claimsPrincipal.AddIdentity(dawnshardIdentity);
    }
}
