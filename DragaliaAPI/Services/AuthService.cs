﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using System.IdentityModel.Tokens.Jwt;

namespace DragaliaAPI.Services;

public class AuthService : IAuthService
{
    private readonly IBaasRequestHelper baasRequestHelper;
    private readonly ISessionService sessionService;
    private readonly ISavefileService savefileService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IOptionsMonitor<LoginOptions> loginOptions;
    private readonly IOptionsMonitor<BaasOptions> baasOptions;
    private readonly ILogger<AuthService> logger;
    private readonly JwtSecurityTokenHandler TokenHandler = new();

    public AuthService(
        IBaasRequestHelper baasRequestHelper,
        ISessionService sessionService,
        ISavefileService savefileService,
        IUserDataRepository userDataRepository,
        IOptionsMonitor<LoginOptions> loginOptions,
        IOptionsMonitor<BaasOptions> baasOptions,
        ILogger<AuthService> logger
    )
    {
        this.baasRequestHelper = baasRequestHelper;
        this.sessionService = sessionService;
        this.savefileService = savefileService;
        this.userDataRepository = userDataRepository;
        this.loginOptions = loginOptions;
        this.baasOptions = baasOptions;
        this.logger = logger;
    }

    public async Task<(long viewerId, string sessionId)> DoAuth(string idToken)
    {
        (long viewerId, string sessionId) result = this.loginOptions.CurrentValue.UseBaasLogin
            ? await this.DoBaasAuth(idToken)
#pragma warning disable CS0618 // Type or member is obsolete
            : await this.DoLegacyAuth(idToken);
#pragma warning restore CS0618 // Type or member is obsolete

        return result;
    }

    [Obsolete("From pre-BaaS login flow")]
    private async Task<(long viewerId, string sessionId)> DoLegacyAuth(string idToken)
    {
        string sessionId;
        string deviceAccountId;

        sessionId = await this.sessionService.ActivateSession(idToken);
        deviceAccountId = (
            await this.sessionService.LoadSessionSessionId(sessionId)
        ).DeviceAccountId;

        IQueryable<DbPlayerUserData> playerInfo = this.userDataRepository.GetUserData(
            deviceAccountId
        );

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

        if (
            GetPendingSaveImport(
                jwt,
                await this.userDataRepository.GetUserData(jwt.Subject).SingleOrDefaultAsync()
            )
        )
        {
            try
            {
                LoadIndexData pendingSave = await this.baasRequestHelper.GetSavefile(idToken);
                this.logger.LogDebug("UserData: {@userData}", pendingSave.user_data);
                await this.savefileService.ThreadSafeImport(jwt.Subject, pendingSave);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Error importing save");
                // Let them log in regardless
            }
        }

        long viewerId = await this.DoLogin(jwt.Subject);
        string sessionId = await this.sessionService.CreateSession(idToken, jwt.Subject, viewerId);

        using IDisposable vIdLog = LogContext.PushProperty(
            CustomClaimType.ViewerId,
            viewerId.ToString()
        );

        this.logger.LogInformation(
            "Authenticated user with viewer ID {id} and issued session ID {sid}",
            viewerId,
            sessionId
        );

        return new(viewerId, sessionId);
    }

    private async Task<TokenValidationResult> ValidateToken(string idToken)
    {
        TokenValidationResult validationResult = await TokenHandler.ValidateTokenAsync(
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
            logger.LogError("ID token was invalid: {@validationResult}", validationResult);

            if (validationResult.Exception is SecurityTokenExpiredException)
            {
                // Return a 400 to make the client call /login again
                logger.LogInformation(
                    "The token was expired. Sending client to request a new one."
                );
                throw new SessionException();
            }
            else
            {
                throw new DragaliaException(
                    Models.ResultCode.IdTokenError,
                    "Failed to validate BaaS token!"
                );
            }
        }

        return validationResult;
    }

    private async Task<long> DoLogin(string accountId)
    {
        IQueryable<DbPlayerUserData> userDataQuery = this.userDataRepository.GetUserData(accountId);

        DbPlayerUserData? userData = await userDataQuery.SingleOrDefaultAsync();

        if (userData is null)
            await this.savefileService.Create(accountId);

        return await userDataQuery.Select(x => x.ViewerId).SingleAsync();
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

        if (!token.Payload.TryGetValue("sav:ts", out object? saveTimestampObj))
            return false;

        DateTimeOffset saveDateTime = DateTimeOffset.FromUnixTimeSeconds((long)saveTimestampObj);
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
