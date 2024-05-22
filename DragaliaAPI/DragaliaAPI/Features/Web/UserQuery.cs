using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace DragaliaAPI.Features.Web;

[Handler]
[MapGet("/api/user")]
[Authorize(WebAuthenticationHelper.PolicyName)]
public static partial class UserQuery
{
    public record Query;

    private static async ValueTask<User> HandleAsync(Query _, CancellationToken cancellationToken)
    {
        return new User(1, "a");
    }
}
