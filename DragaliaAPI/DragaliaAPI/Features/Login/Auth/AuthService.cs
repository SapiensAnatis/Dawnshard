using System.Diagnostics;
using System.Security.Claims;
using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Login.Savefile;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Serilog.Context;

namespace DragaliaAPI.Features.Login.Auth;

internal sealed partial class AuthService(
    IBaasApi baasRequestHelper,
    ISessionService sessionService,
    ISavefileService savefileService,
    IPlayerIdentityService playerIdentityService,
    ApiContext apiContext,
    ILogger<AuthService> logger,
    TimeProvider dateTimeProvider
) : IAuthService
{
    public async Task<DbPlayer> DoSignup(ClaimsPrincipal claimsPrincipal)
    {
        string subject =
            claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Unable to retrieve subject");

        DbPlayer newPlayer = await savefileService.Create(subject);

        return newPlayer;
    }

    public async Task<AuthResult> DoLogin(ClaimsPrincipal claimsPrincipal)
    {
        string subject =
            claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Unable to retrieve subject");

        using IDisposable accIdLog = LogContext.PushProperty(CustomClaimType.AccountId, subject);

        CaseSensitiveClaimsIdentity jwtIdentity = claimsPrincipal
            .Identities.OfType<CaseSensitiveClaimsIdentity>()
            .Single();

        JsonWebToken token = (JsonWebToken)jwtIdentity.SecurityToken;

        var player = await apiContext
            .Players.IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.AccountId == subject)
            .Select(x => new { x.AccountId, x.ViewerId })
            .FirstAsync();

        using IDisposable ctx = playerIdentityService.StartUserImpersonation(
            player.ViewerId,
            player.AccountId
        );

        string sessionId = await sessionService.CreateSession(
            token.EncodedToken,
            player.AccountId,
            player.ViewerId,
            dateTimeProvider.GetUtcNow()
        );

        Log.SuccessfullyAuthenticated(logger, player.ViewerId, sessionId);

        return new AuthResult { ViewerId = player.ViewerId, SessionId = sessionId };
    }

    public async Task ImportSaveIfPending(ClaimsPrincipal claimsPrincipal)
    {
        string subject =
            claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnreachableException("Unable to retrieve subject");

        CaseSensitiveClaimsIdentity jwtIdentity = claimsPrincipal
            .Identities.OfType<CaseSensitiveClaimsIdentity>()
            .Single();

        JsonWebToken token = (JsonWebToken)jwtIdentity.SecurityToken;

        Log.PollingSaveImport(logger, subject);

        DateTimeOffset? remoteSavefileDate = GetRemoteSavefileDate(subject, claimsPrincipal);

        if (remoteSavefileDate is null)
        {
            Log.NoSaveAvailable(logger);
            return;
        }

        Log.FoundRemoteSavefile(logger, remoteSavefileDate.Value);

        DateTimeOffset localSavefileDate = await apiContext
            .Players.IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.AccountId == subject)
            .Select(x => x.UserData!.LastSaveImportTime)
            .FirstAsync();

        if (localSavefileDate >= remoteSavefileDate)
        {
            Log.LocalSavefileWasNewer(
                logger,
                remoteSavefileDate: remoteSavefileDate.Value,
                localSavefileDate: localSavefileDate
            );
            return;
        }

        try
        {
            LoadIndexResponse pendingSave = await baasRequestHelper.GetSavefile(token.EncodedToken);
            await savefileService.ThreadSafeImport(pendingSave);
        }
        catch (Exception e) when (e is JsonException or AutoMapperMappingException)
        {
            Log.SavefileInvalid(logger, e);
        }
        catch (Exception e)
        {
            Log.UnknwonSavefileImportError(logger, e);
            // Let them log in regardless
        }
    }

    private static DateTimeOffset? GetRemoteSavefileDate(
        string subject,
        ClaimsPrincipal claimsPrincipal
    )
    {
        if (claimsPrincipal.FindFirstValue("sav:a") != "true")
        {
            return null;
        }

        string? saveTimestampStr = claimsPrincipal.FindFirstValue("sav:ts");
        if (!long.TryParse(saveTimestampStr, out long saveUnixTimestamp))
        {
            throw new InvalidOperationException(
                $"Failed to parse sav:ts value: {saveTimestampStr}"
            );
        }

        return DateTimeOffset.FromUnixTimeSeconds(saveUnixTimestamp);
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Polling save import for user {AccountId}...")]
        public static partial void PollingSaveImport(ILogger logger, string accountId);

        [LoggerMessage(LogLevel.Information, "No savefile was available to import.")]
        public static partial void NoSaveAvailable(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Found remote savefile with date {RemoteSavefileDate}.")]
        public static partial void FoundRemoteSavefile(
            ILogger logger,
            DateTimeOffset remoteSavefileDate
        );

        [LoggerMessage(
            LogLevel.Information,
            "The remote savefile ({RemoteSavefileDate}) was older than the local savefile ({LocalSavefileDate})."
        )]
        public static partial void LocalSavefileWasNewer(
            ILogger logger,
            DateTimeOffset remoteSavefileDate,
            DateTimeOffset localSavefileDate
        );

        [LoggerMessage(
            LogLevel.Information,
            "Authenticated user with viewer ID {ViewerId} and issued session ID {SessionId}."
        )]
        public static partial void SuccessfullyAuthenticated(
            ILogger logger,
            long viewerId,
            string sessionId
        );

        [LoggerMessage(LogLevel.Information, "Savefile was invalid")]
        public static partial void SavefileInvalid(ILogger logger, Exception ex);

        [LoggerMessage(LogLevel.Warning, "Error importing save")]
        public static partial void UnknwonSavefileImportError(ILogger logger, Exception ex);
    }
}
