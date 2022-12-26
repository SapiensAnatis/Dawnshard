using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Services.Helpers;
using DragaliaAPI.Services.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

namespace DragaliaAPI.Services;

public class AuthService : IAuthService
{
    private readonly IBaasRequestHelper baasRequestHelper;
    private readonly ISessionService sessionService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IDeviceAccountRepository deviceAccountRepository;
    private readonly IOptionsMonitor<DragaliaAuthOptions> options;
    private readonly ILogger<AuthService> logger;

    public AuthService(
        IBaasRequestHelper baasRequestHelper,
        ISessionService sessionService,
        IUserDataRepository userDataRepository,
        IDeviceAccountRepository deviceAccountRepository,
        IOptionsMonitor<DragaliaAuthOptions> options,
        ILogger<AuthService> logger
    )
    {
        this.baasRequestHelper = baasRequestHelper;
        this.sessionService = sessionService;
        this.userDataRepository = userDataRepository;
        this.deviceAccountRepository = deviceAccountRepository;
        this.options = options;
        this.logger = logger;
    }

    public async Task<(long viewerId, string sessionId)> DoAuth(string idToken)
    {
        (long viewerId, string sessionId) result = this.options.CurrentValue.UseLegacyLogin
            ? await this.DoLegacyAuth(idToken)
            : await this.DoBaasAuth(idToken);

        this.logger.LogInformation(
            "Authenticated user with viewer ID {viewerid} and issued session ID {sid}",
            result.viewerId,
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
        string id = ((JwtSecurityToken)result.SecurityToken).Subject;

        long viewerId = await this.DoLogin(id);
        string sessionId = await this.sessionService.CreateSession(id, idToken);

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
                ValidAudience = this.options.CurrentValue.TokenAudience,
                ValidIssuer = this.options.CurrentValue.TokenIssuer,
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

    private async Task<long> DoLogin(string accountId)
    {
        IQueryable<DbPlayerUserData> userDataQuery = this.userDataRepository.GetUserData(accountId);
        if (!userDataQuery.Any())
        {
            await this.deviceAccountRepository.CreateNewSavefile(accountId);
            await this.deviceAccountRepository.SaveChangesAsync();
        }

        return await userDataQuery.Select(x => x.ViewerId).SingleAsync();
    }
}
