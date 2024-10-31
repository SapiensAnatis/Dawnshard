﻿namespace DragaliaAPI.Shared.PlayerDetails;

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
    /// Viewer ID; publicly facing 'friend request' ID
    /// </summary>
    public const string ViewerId = "ViewerId";

    /// <summary>
    /// Player name from user_data.
    /// </summary>
    public const string PlayerName = "PlayerName";
}
