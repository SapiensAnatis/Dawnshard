namespace DragaliaAPI.Features.Web;

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
        public const string WebJwtScheme = nameof(WebJwtScheme);
    }
}
