using Microsoft.AspNetCore.Authorization;

namespace DragaliaAPI.Features.Web;

public static class AuthorizationPolicies
{
    public static void RequireValidJwtPolicyBuilder(AuthorizationPolicyBuilder builder) =>
        builder
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(AuthConstants.SchemeNames.WebJwtScheme);

    public static void RequireDawnshardIdentityPolicyBuilder(AuthorizationPolicyBuilder builder) =>
        builder
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(AuthConstants.SchemeNames.WebJwtScheme)
            .RequireAssertion(ctx =>
                ctx.User.Identities.Any(x => x.Label == AuthConstants.IdentityLabels.Dawnshard)
            );
}
