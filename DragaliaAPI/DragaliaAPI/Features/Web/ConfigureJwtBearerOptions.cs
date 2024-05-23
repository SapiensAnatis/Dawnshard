using DragaliaAPI.Models.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Web;

// Sourced from: https://gist.github.com/xiaomi7732/20ff2ad11b085a851759d3835b95c8d7

/// <summary>
/// Allows setting additional properties on <see cref="JwtBearerOptions"/> using DI.
/// </summary>
/// <remarks>
/// Sourced from: <see href="https://gist.github.com/xiaomi7732/20ff2ad11b085a851759d3835b95c8d7"/>
/// </remarks>
public class ConfigureJwtBearerOptions(
    BaasConfigurationManager baasConfigurationManager,
    IOptions<BaasOptions> baasOptions
) : IConfigureNamedOptions<JwtBearerOptions>
{
    // Never called
    public void Configure(JwtBearerOptions options) =>
        this.Configure(JwtBearerDefaults.AuthenticationScheme, options);

    public void Configure(string? name, JwtBearerOptions options)
    {
        options.ConfigurationManager = baasConfigurationManager;
        options.TokenValidationParameters = new()
        {
            ValidAudience = baasOptions.Value.TokenAudience,
            ValidIssuer = baasOptions.Value.TokenIssuer,
        };
    }
}
