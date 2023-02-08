namespace DragaliaAPI.Shared.PlayerDetails;

/// <summary>
/// Custom authentication claim types.
/// </summary>
public static class CustomClaimType
{
    /// <summary>
    /// Primary key; BaaS ID / DeviceAccountId
    /// </summary>
    public const string AccountId = "AccountId";

    /// <summary>
    /// Viewer ID; publically facing 'friend request' ID
    /// </summary>
    public const string ViewerId = "ViewerId";
}
