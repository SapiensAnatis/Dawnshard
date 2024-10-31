namespace DragaliaAPI.Infrastructure.Authentication;

public static class AuthConstants
{
    public static class PolicyNames
    {
        public const string RequireValidWebJwt = "RequireValidWebJwt";

        public const string RequireDawnshardIdentity = "RequireDawnshardIdentity";
    }

    public static class IdentityLabels
    {
        public const string Dawnshard = "Dawnshard";
    }

    public static class SchemeNames
    {
        public const string WebJwt = "WebJwt";

        public const string GameJwt = "GameJwt";

        public const string Session = "SessionAuthentication";

        public const string Developer = "DeveloperAuthentication";

        public const string Zena = "ZenaAuthentication";
    }
}
