namespace DragaliaAPI.Infrastructure.Authentication;

public static class AuthConstants
{
    public static class PolicyNames
    {
        public const string RequireValidJwt = nameof(RequireValidJwt);

        public const string RequireDawnshardIdentity = nameof(RequireDawnshardIdentity);
    }

    public static class IdentityLabels
    {
        public const string Dawnshard = nameof(Dawnshard);
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
