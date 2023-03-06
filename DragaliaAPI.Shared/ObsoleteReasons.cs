namespace DragaliaAPI.Shared;

public static class ObsoleteReasons
{
    public const string UsePlayerDetailsService =
        "There exists another overload/property which does not require DeviceAccountId.";

    public const string BaaS = "Used for pre-BaaS login flow and local development login";
}
