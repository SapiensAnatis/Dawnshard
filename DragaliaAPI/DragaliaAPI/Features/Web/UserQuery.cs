using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;

namespace DragaliaAPI.Features.Web;

[Handler]
[MapGet("/api/user")]
public static partial class UserQuery
{
    public record Query;

    private static async ValueTask<User> HandleAsync(Query _, CancellationToken cancellationToken)
    {
        return new User(1, "a");
    }
}
