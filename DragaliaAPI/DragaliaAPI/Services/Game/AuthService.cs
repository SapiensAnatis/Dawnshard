using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog.Context;

namespace DragaliaAPI.Services.Game;

public class AuthService(
    IBaasApi baasRequestHelper,
    ISessionService sessionService,
    ISavefileService savefileService,
    IPlayerIdentityService playerIdentityService,
    IUserDataRepository userDataRepository,
    ApiContext apiContext,
    IOptionsMonitor<LoginOptions> loginOptions,
    IOptionsMonitor<BaasOptions> baasOptions,
    ILogger<AuthService> logger,
    TimeProvider dateTimeProvider
) : IAuthService
{
    private readonly IBaasApi baasRequestHelper = baasRequestHelper;
    private readonly ISessionService sessionService = sessionService;
    private readonly ISavefileService savefileService = savefileService;
    private readonly IPlayerIdentityService playerIdentityService = playerIdentityService;
    private readonly IUserDataRepository userDataRepository = userDataRepository;
    private readonly ApiContext apiContext = apiContext;
    private readonly IOptionsMonitor<LoginOptions> loginOptions = loginOptions;
    private readonly IOptionsMonitor<BaasOptions> baasOptions = baasOptions;
    private readonly ILogger<AuthService> logger = logger;
    private readonly TimeProvider dateTimeProvider = dateTimeProvider;
    private readonly JwtSecurityTokenHandler tokenHandler = new();

    public async Task<(long viewerId, string sessionId)> DoAuth(string idToken)
    {
        (long viewerId, string sessionId) result = this.loginOptions.CurrentValue.UseBaasLogin
            ? await this.DoBaasAuth(idToken)
#pragma warning disable CS0618 // Type or member is obsolete
            : await this.DoLegacyAuth(idToken);
#pragma warning restore CS0618 // Type or member is obsolete

        return result;
    }

    [Obsolete(ObsoleteReasons.BaaS)]
    private async Task<(long viewerId, string sessionId)> DoLegacyAuth(string idToken)
    {
        string sessionId = await this.sessionService.ActivateSession(idToken);
        Session session = await this.sessionService.LoadSessionSessionId(sessionId);
        string deviceAccountId = session.DeviceAccountId;
        long viewerId = session.ViewerId;

        IQueryable<DbPlayerUserData> playerInfo;

        using (this.playerIdentityService.StartUserImpersonation(viewerId, deviceAccountId))
        {
            playerInfo = this.userDataRepository.UserData;
        }

        return (await playerInfo.Select(x => x.ViewerId).SingleAsync(), sessionId);
    }

    private async Task<(long viewerId, string sessionId)> DoBaasAuth(string idToken)
    {
        TokenValidationResult result = await this.ValidateToken(idToken);
        JwtSecurityToken jwt = (JwtSecurityToken)result.SecurityToken;

        using IDisposable accIdLog = LogContext.PushProperty(
            CustomClaimType.AccountId,
            jwt.Subject
        );

        DbPlayer? player = await this
            .apiContext.Players.AsNoTracking()
            .Include(x => x.UserData)
            .FirstOrDefaultAsync(x => x.AccountId == jwt.Subject);

        player ??= await this.savefileService.Create(jwt.Subject);

        using IDisposable ctx = this.playerIdentityService.StartUserImpersonation(
            player.ViewerId,
            player.AccountId
        );

        if (GetPendingSaveImport(jwt, player.UserData))
            await this.ImportSave(idToken);

        string sessionId = await this.sessionService.CreateSession(
            idToken,
            player.AccountId,
            player.ViewerId,
            this.dateTimeProvider.GetUtcNow()
        );

        this.logger.LogInformation(
            "Authenticated user with viewer ID {id} and issued session ID {sid}",
            player.ViewerId,
            sessionId
        );

        return new(player.ViewerId, sessionId);
    }

    private async Task ImportSave(string idToken)
    {
        try
        {
            LoadIndexResponse pendingSave = await this.baasRequestHelper.GetSavefile(idToken);

            this.logger.LogDebug("UserData: {@userData}", pendingSave.UserData);
            await this.savefileService.ThreadSafeImport(pendingSave);
        }
        catch (Exception e) when (e is JsonException or AutoMapperMappingException)
        {
            this.logger.LogInformation(e, "Savefile was invalid.");
        }
        catch (Exception e)
        {
            this.logger.LogWarning(e, "Error importing save");
            // Let them log in regardless
        }
    }

    private async Task<TokenValidationResult> ValidateToken(string idToken)
    {
        TokenValidationResult validationResult = await this.tokenHandler.ValidateTokenAsync(
            idToken,
            new()
            {
                IssuerSigningKeys = await this.baasRequestHelper.GetKeys(),
                ValidAudience = this.baasOptions.CurrentValue.TokenAudience,
                ValidIssuer = this.baasOptions.CurrentValue.TokenIssuer,
            }
        );

        if (!validationResult.IsValid)
        {
            string idTokenTrace = idToken[^5..];
            string? accountId = (validationResult.SecurityToken as JwtSecurityToken)?.Subject;

            LogContext.PushProperty(CustomClaimType.AccountId, accountId);

            if (validationResult.Exception is SecurityTokenExpiredException)
            {
                // Go to ExceptionHandlerMiddleware to make the client receive a 400 and login again
                logger.LogInformation(
                    "ID token ..{token} was expired: {@validationResult}. Sending client to request a new one.",
                    idTokenTrace,
                    validationResult
                );

                throw validationResult.Exception;
            }
            else
            {
                logger.LogDebug(
                    "ID token ..{token} was invalid: {@validationResult}",
                    idTokenTrace,
                    validationResult
                );
                throw new DragaliaException(
                    ResultCode.IdTokenError,
                    "Failed to validate BaaS token!",
                    validationResult.Exception
                );
            }
        }

        return validationResult;
    }

    private bool GetPendingSaveImport(JwtSecurityToken token, DbPlayerUserData? userData)
    {
        this.logger.LogInformation("Polling save import for user {id}...", token.Subject);

        if (!token.Payload.TryGetValue("sav:a", out object? saveAvailableObj))
            return false;

        bool? saveAvailable = saveAvailableObj as bool?;
        if (saveAvailable != true)
        {
            this.logger.LogInformation("No savefile was available to import.");
            return false;
        }

        string? saveTimestampStr = token.Claims.FirstOrDefault(x => x.Type == "sav:ts")?.Value;
        if (saveTimestampStr == null)
            return false;

        DateTimeOffset saveDateTime = DateTimeOffset.FromUnixTimeSeconds(
            long.Parse(saveTimestampStr)
        );

        DateTimeOffset lastImportTime = userData?.LastSaveImportTime ?? DateTimeOffset.MinValue;
        if (lastImportTime >= saveDateTime)
        {
            this.logger.LogInformation(
                "The remote savefile was older. (stored: {t1}, remote: {t2})",
                lastImportTime,
                saveDateTime
            );

            return false;
        }

        this.logger.LogInformation(
            "Detected pending save import for user {id} (stored: {t1}, remote: {t2})",
            token.Subject,
            lastImportTime,
            saveDateTime
        );
        return true;
    }
}
