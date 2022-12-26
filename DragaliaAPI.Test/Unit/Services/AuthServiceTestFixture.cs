using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Test.Unit.Services;

/// <summary>
/// Used to maintain the same key between each of the tests in AuthService.
/// Otherwise failures seemed to occur when running the tests at the same time.
/// </summary>
public class AuthServiceTestFixture
{
    public List<SecurityKey> SecurityKeys { get; } =
        new() { new RsaSecurityKey(RSA.Create(2048).ExportParameters(true)) };

    public string GetToken(string issuer, string audience, DateTime expiryTime, string accountId)
    {
        JwtSecurityToken tokenObject =
            new(
                issuer: issuer,
                audience: audience,
                expires: expiryTime,
                signingCredentials: new SigningCredentials(
                    this.SecurityKeys.First(),
                    SecurityAlgorithms.RsaSha256
                ),
                claims: new List<Claim>() { new Claim("sub", accountId) }
            );

        return new JwtSecurityTokenHandler().WriteToken(tokenObject);
    }
}
