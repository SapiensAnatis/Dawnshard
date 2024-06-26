namespace DragaliaAPI.Features.Web.Account;

public class User
{
    public long ViewerId { get; init; }

    public required string Name { get; init; }
}

public class UserProfile
{
    public DateTimeOffset LastSaveImportTime { get; init; }

    public DateTimeOffset LastLoginTime { get; init; }
}
