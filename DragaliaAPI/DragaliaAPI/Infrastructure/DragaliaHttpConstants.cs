using System.Collections.Frozen;

namespace DragaliaAPI.Infrastructure;

public static class DragaliaHttpConstants
{
    public static class RoutePrefixes
    {
        public const string Android = "/2.19.0_20220714193707";

        public const string Ios = "/2.19.0_20220719103923";

        public static IReadOnlySet<string> List { get; } =
            new List<string>() { Android, Ios }.ToFrozenSet();
    }

    public static class Headers
    {
        /// <summary>
        /// Header containing the current session ID.
        /// </summary>
        public const string SessionId = "SID";

        /// <summary>
        /// Header containing the request's token, which is the same when a request is retried due to a network error.
        /// </summary>
        public const string RequestToken = "Request-Token";

        /// <summary>
        /// Header containing a unique device identifier.
        /// </summary>
        public const string DeviceId = "DeviceId";
    }
}
