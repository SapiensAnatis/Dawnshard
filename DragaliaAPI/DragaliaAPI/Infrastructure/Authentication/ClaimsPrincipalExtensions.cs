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
        ClaimsIdentity dawnshardIdentity = new(
            [
                new Claim(CustomClaimType.AccountId, accountId),
                new Claim(CustomClaimType.ViewerId, viewerId.ToString()),
            ]
        )
        {
            Label = AuthConstants.IdentityLabels.Dawnshard,
        };

        if (playerName is not null)
        {
            dawnshardIdentity.AddClaim(new Claim(CustomClaimType.PlayerName, playerName));
        }

        if (isAdmin)
        {
            dawnshardIdentity.AddClaim(new Claim(CustomClaimType.IsAdmin, "true"));
        }

        claimsPrincipal.AddIdentity(dawnshardIdentity);
    }
}
