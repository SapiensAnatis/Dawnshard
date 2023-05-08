using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

    public static string AsString(this JwtSecurityToken token)
    {
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static JwtSecurityToken GetToken(
        string issuer,
        string audience,
        DateTimeOffset expiryTime,
        string accountId
    )
    {
        return new(
            issuer: issuer,
            audience: audience,
            expires: expiryTime.UtcDateTime,
            signingCredentials: new SigningCredentials(
                SecurityKeys.First(),
                SecurityAlgorithms.RsaSha256
            ),
            claims: new List<Claim>() { new("sub", accountId) }
        );
    }

    public static JwtSecurityToken GetToken(DateTimeOffset expiryTime, string accountId)
    {
        return GetToken("LukeFZ", "baas-Id", expiryTime, accountId);
    }

    public static JwtSecurityToken GetToken(
        DateTimeOffset expiryTime,
        string accountId,
        bool savefileAvailable,
        DateTimeOffset savefileTime
    )
    {
        return GetToken(
            "LukeFZ",
            "baas-Id",
            expiryTime,
            accountId,
            savefileAvailable,
            savefileTime
        );
    }

    public static JwtSecurityToken GetToken(
        string issuer,
        string audience,
        DateTimeOffset expiryTime,
        string accountId,
        bool savefileAvailable,
        DateTimeOffset savefileTime
    )
    {
        JwtSecurityToken result = GetToken(issuer, audience, expiryTime, accountId);
        result.Payload.Add("sav:a", savefileAvailable);
        result.Payload.Add("sav:ts", savefileTime.ToUnixTimeSeconds());

        return result;
    }
}
