using DragaliaAPI.Database.Entities.Owned;

namespace DragaliaAPI.Features.Web.Users;

internal sealed class User
{
    public long ViewerId { get; init; }

    public required string Name { get; init; }

    public bool IsAdmin { get; init; }
}

internal sealed class UserProfile
{
    public DateTimeOffset? LastSaveImportTime { get; init; }

    public DateTimeOffset LastLoginTime { get; init; }

    public required PlayerSettings Settings { get; init; }
}
