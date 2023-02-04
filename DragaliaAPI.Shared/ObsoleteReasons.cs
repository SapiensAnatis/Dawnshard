namespace DragaliaAPI.Shared;

public static class ObsoleteReasons
{
    public const string UsePlayerDetailsService =
        "The other overload which does not require DeviceAccountId should be used.";

    public const string BaaS = "Used for pre-BaaS login flow and local development login";
}
