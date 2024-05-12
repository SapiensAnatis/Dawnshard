using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json.Serialization;
using DragaliaAPI.Authentication;
using DragaliaAPI.Database;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace DragaliaAPI.Pages;

public class OAuthCallbackModel(
    IOptionsMonitor<BaasOptions> options,
    ApiContext apiContext,
    ILogger<OAuthCallbackModel> logger
) : PageModel
{
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

        AuthenticateResult result = await this.Authenticate(sessionTokenCode);

        if (!result.Succeeded)
        {
            logger.LogInformation("Authenticate result failure.");
            return this.StatusCode(401, $"Authentication failure: {result.Failure?.Message}");
        }

        await this.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            result.Principal
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

    private async Task<AuthenticateResult> Authenticate(string sessionTokenCode)
    {
        using HttpClient httpClient = new();
        httpClient.BaseAddress = options.CurrentValue.BaasUrlParsed;

        Dictionary<string, string> sessionTokenParameters =
            new()
            {
                { "client_id", options.CurrentValue.ClientId },
                { "session_token_code", sessionTokenCode },
                { "session_token_code_verifier", options.CurrentValue.ChallengeString }
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
        if (!userIdResponse.IsSuccessStatusCode)
        {
            logger.LogInformation(
                "/gameplay/v1/user returned non-success status {status}",
                userIdResponse.StatusCode
            );
            return AuthenticateResult.Fail("Failed to get user.");
        }

        UserIdResponse userId =
            await userIdResponse.Content.ReadFromJsonAsync<UserIdResponse>()
            ?? throw new JsonException("Null UserIdResponse");

        ClaimsIdentity identity = new("OAuth");

        identity.AddClaim(new Claim(CustomClaimType.AccountId, userId.UserId));

        var playerInfo = await apiContext
            .Players.IgnoreQueryFilters()
            .Include(x => x.UserData)
            .Where(x => x.AccountId == userId.UserId)
            .Select(x => new { x.UserData!.Name, x.ViewerId })
            .FirstOrDefaultAsync();

        if (playerInfo is null)
            return AuthenticateResult.Fail(
                "Account did not have an associated Dawnshard save file."
            );

        identity.AddClaim(new Claim(CustomClaimType.AccountId, userId.UserId));
        identity.AddClaim(new Claim(CustomClaimType.PlayerName, playerInfo.Name));
        identity.AddClaim(new Claim(CustomClaimType.ViewerId, playerInfo.ViewerId.ToString()));

        AuthenticationTicket ticket = new(new ClaimsPrincipal(identity), "BaaS");
        return AuthenticateResult.Success(ticket);
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
