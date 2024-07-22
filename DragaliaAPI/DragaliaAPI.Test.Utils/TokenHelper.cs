using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Test.Utils;

public static class TokenHelper
{
    /// <summary>
    /// Consistent security key.
    /// </summary>
    public static IList<SecurityKey> SecurityKeys { get; } = new List<SecurityKey>();

    static TokenHelper()
    {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText("RSA_key.pem").AsSpan());
        SecurityKeys.Add(new RsaSecurityKey(rsa) { KeyId = "key" });
    }

    public static string GetToken(
        string accountId,
        DateTimeOffset expiryTime,
        string issuer = "LukeFZ",
        string audience = "baas-Id",
        bool savefileAvailable = false,
        DateTimeOffset? savefileTime = null
    )
    {
        SecurityTokenDescriptor descriptor =
            new()
            {
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    SecurityKeys.First(),
                    SecurityAlgorithms.RsaSha256
                ),
                Claims = new Dictionary<string, object>() { ["sub"] = accountId },
                Expires = expiryTime.UtcDateTime
            };

        if (savefileAvailable && savefileTime is not null)
        {
            descriptor.Claims.Add("sav:a", savefileAvailable);
            descriptor.Claims.Add("sav:ts", savefileTime.Value.ToUnixTimeSeconds());
        }

        JsonWebTokenHandler handler = new() { SetDefaultTimesOnTokenCreation = false };
        return handler.CreateToken(descriptor);
    }
}
