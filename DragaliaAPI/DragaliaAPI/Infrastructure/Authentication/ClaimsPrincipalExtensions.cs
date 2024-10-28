using System.Security.Claims;

namespace DragaliaAPI.Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static bool HasDawnshardIdentity(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.Identities.Any(x => x.Label == AuthConstants.IdentityLabels.Dawnshard);
}
