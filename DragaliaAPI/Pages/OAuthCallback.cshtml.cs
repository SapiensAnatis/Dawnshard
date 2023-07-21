using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using DragaliaAPI.Blazor.Authentication;
using DragaliaAPI.Database;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Npgsql.Internal.TypeHandlers.NetworkHandlers;

namespace DragaliaAPI.Blazor.Pages;

public class OAuthCallbackModel : PageModel
{
    private readonly IOptionsMonitor<BaasOptions> options;
    private readonly ApiContext apiContext;

    public OAuthCallbackModel(IOptionsMonitor<BaasOptions> options, ApiContext apiContext)
    {
        this.options = options;
        this.apiContext = apiContext;
    }

    public async Task<IActionResult> OnGet()
    {
        if (
            !this.HttpContext.Request.Query.TryGetValue(
                "session_token_code",
                out StringValues sessionTokenCodeValues
            )
        )
        {
            return this.Unauthorized();
        }

        string? sessionTokenCode = sessionTokenCodeValues.FirstOrDefault();
        if (sessionTokenCode is null)
            return this.Unauthorized();

        ClaimsPrincipal principal = await this.Authenticate(sessionTokenCode);

        await this.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        if (
            !this.HttpContext.Request.Query.TryGetValue(
                Constants.QueryParams.OriginalPage,
                out StringValues queryValues
            ) || queryValues.FirstOrDefault() is not string originalPage
        )
        {
            return this.LocalRedirect("~/");
        }

        return this.LocalRedirect($"~/{originalPage}");
    }

    private async Task<ClaimsPrincipal> Authenticate(string sessionTokenCode)
    {
        using HttpClient httpClient = new();
        httpClient.BaseAddress = this.options.CurrentValue.BaasUrlParsed;

        Dictionary<string, string> sessionTokenParameters =
            new()
            {
                { "client_id", Constants.ClientId },
                { "session_token_code", sessionTokenCode },
                { "session_token_code_verifier", Constants.ChallengeString }
            };

        HttpResponseMessage sessionTokenResponse = await httpClient.PostAsync(
            "connect/1.0.0/api/session_token",
            new FormUrlEncodedContent(sessionTokenParameters)
        );

        SessionTokenResponse? sessionToken =
            await sessionTokenResponse.Content.ReadFromJsonAsync<SessionTokenResponse>()
            ?? throw new JsonException("Null SessionTokenResponse");

        SdkTokenRequest sdkTokenRequest = new(Constants.ClientId, sessionToken.SessionToken);

        HttpResponseMessage sdkTokenResponse = await httpClient.PostAsJsonAsync(
            "/1.0.0/gateway/sdk/token",
            sdkTokenRequest
        );

        SdkTokenResponse sdkToken =
            await sdkTokenResponse.Content.ReadFromJsonAsync<SdkTokenResponse>()
            ?? throw new JsonException("Null SdkTokenResponse");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            sdkToken.IdToken
        );

        HttpResponseMessage userIdResponse = await httpClient.GetAsync("/gameplay/v1/user");
        userIdResponse.EnsureSuccessStatusCode();

        UserIdResponse userId =
            await userIdResponse.Content.ReadFromJsonAsync<UserIdResponse>()
            ?? throw new JsonException("Null UserIdResponse");

        ClaimsIdentity identity = new("OAuth");

        identity.AddClaim(new Claim(CustomClaimType.AccountId, userId.UserId));

        // TODO: handle users without a save
        (string playerName, long viewerId) = await this.apiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == userId.UserId)
            .Select(x => new ValueTuple<string, long>(x.Name, x.ViewerId))
            .FirstAsync();

        identity.AddClaim(new Claim(CustomClaimType.AccountId, userId.UserId));
        identity.AddClaim(new Claim(CustomClaimType.PlayerName, playerName));
        identity.AddClaim(new Claim(CustomClaimType.ViewerId, viewerId.ToString()));

        return new ClaimsPrincipal(identity);
    }
}

file record SessionTokenResponse(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("session_token")] string SessionToken
);

file record SdkTokenRequest(
    [property: JsonPropertyName("client_id")] string ClientId,
    [property: JsonPropertyName("session_token")] string SessionToken
);

file record SdkTokenResponse(
    string AccessToken,
    object? Error,
    uint ExpiresIn,
    string IdToken,
    string SessionToken,
    object? TermsAgreement
);

file record UserIdResponse([property: JsonPropertyName("userId")] string UserId);
