using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

namespace DragaliaAPI.Services;

public class AuthService : IAuthService
{
    private readonly IBaasRequestHelper baasRequestHelper;
    private readonly ISessionService sessionService;
    private readonly ISavefileService savefileService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IDeviceAccountRepository deviceAccountRepository;
    private readonly IOptionsMonitor<LoginOptions> loginOptions;
    private readonly IOptionsMonitor<BaasOptions> baasOptions;
    private readonly ILogger<AuthService> logger;

    public AuthService(
        IBaasRequestHelper baasRequestHelper,
        ISessionService sessionService,
        ISavefileService savefileService,
        IUserDataRepository userDataRepository,
        IDeviceAccountRepository deviceAccountRepository,
        IOptionsMonitor<LoginOptions> loginOptions,
        IOptionsMonitor<BaasOptions> baasOptions,
        ILogger<AuthService> logger
    )
    {
        this.baasRequestHelper = baasRequestHelper;
        this.sessionService = sessionService;
        this.savefileService = savefileService;
        this.userDataRepository = userDataRepository;
        this.deviceAccountRepository = deviceAccountRepository;
        this.loginOptions = loginOptions;
        this.baasOptions = baasOptions;
        this.logger = logger;
    }

    public async Task<(long viewerId, string sessionId)> DoAuth(string idToken)
    {
        (long viewerId, string sessionId) result = this.loginOptions.CurrentValue.UseBaasLogin
            ? await this.DoBaasAuth(idToken)
            : await this.DoLegacyAuth(idToken);

        this.logger.LogInformation(
            "Authenticated user with viewer ID {viewerid} using token ...{token} and issued session ID {sid}",
            result.viewerId,
            idToken[^5..],
            result.sessionId
        );

        return result;
    }

    private async Task<(long viewerId, string sessionId)> DoLegacyAuth(string idToken)
    {
        string sessionId;
        string deviceAccountId;

        sessionId = await this.sessionService.ActivateSession(idToken);
        deviceAccountId = await this.sessionService.GetDeviceAccountId_SessionId(sessionId);

        IQueryable<DbPlayerUserData> playerInfo = this.userDataRepository.GetUserData(
            deviceAccountId
        );

        return (await playerInfo.Select(x => x.ViewerId).SingleAsync(), sessionId);
    }

    private async Task<(long viewerId, string sessionId)> DoBaasAuth(string idToken)
    {
        TokenValidationResult result = await this.ValidateToken(idToken);
        JwtSecurityToken jwt = (JwtSecurityToken)result.SecurityToken;

        long viewerId = await this.DoLogin(jwt);
        string sessionId = await this.sessionService.CreateSession(jwt.Subject, idToken);

        return new(viewerId, sessionId);
    }

    private async Task<TokenValidationResult> ValidateToken(string idToken)
    {
        JwtSecurityTokenHandler handler = new();
        TokenValidationResult validationResult = await handler.ValidateTokenAsync(
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
                    Models.ResultCode.COMMON_AUTH_ERROR,
                    "Failed to validate BaaS token!"
                );
            }
        }

        return validationResult;
    }

    private async Task<long> DoLogin(JwtSecurityToken jwt)
    {
        string accountId = jwt.Subject;

        IQueryable<DbPlayerUserData> userDataQuery = this.userDataRepository.GetUserData(accountId);
        if (!userDataQuery.Any())
        {
            await this.deviceAccountRepository.CreateNewSavefile(accountId);
            await this.deviceAccountRepository.SaveChangesAsync();
        }

        // A user needs to h

        return await userDataQuery.Select(x => x.ViewerId).SingleAsync();
    }

    private async Task PollSaveImport(JsonWebToken token, DbPlayerUserData userData)
    {
        //this.savefileService.Import();
    }
}
