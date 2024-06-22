using System.Collections.Frozen;

namespace DragaliaAPI.Infrastructure;

public static class DragaliaHttpConstants
{
    public static class RoutePrefixes
    {
        private const string Android = "/2.19.0_20220714193707";

        private const string Ios = "/2.19.0_20220719103923";

        public static IReadOnlySet<string> List { get; } =
            new List<string>() { Android, Ios }.ToFrozenSet();
    }

    public static class Headers
    {
        public const string SessionId = "SID";

        public const string RequestToken = "Request-Token";
    }
}
