using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

namespace DragaliaAPI.Services;

public class AuthService : IAuthService
{
    private readonly ISessionService sessionService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IDeviceAccountRepository deviceAccountRepository;
    private readonly HttpClient client;
    private readonly ILogger<AuthService> logger;
    private readonly string? baasUrl;
    private readonly bool useLegacyLogin;

    private static TokenValidationParameters? ValidationParams;

    private const string KeySetEndpoint = "/.well-known/jwks.json";
    private const string TokenAudience = "baas-Id";
    private const string TokenIssuer = "LukeFZ";

    public AuthService(
        ISessionService sessionService,
        IUserDataRepository userDataRepository,
        IDeviceAccountRepository deviceAccountRepository,
        IConfiguration configuration,
        HttpClient client,
        ILogger<AuthService> logger
    )
    {
        this.sessionService = sessionService;
        this.userDataRepository = userDataRepository;
        this.deviceAccountRepository = deviceAccountRepository;
        this.client = client;
        this.logger = logger;
        IConfigurationSection authSection = configuration.GetRequiredSection("DragaliaAuth");
        this.useLegacyLogin = authSection.GetValue<bool>("UseLegacyLogin");
        this.baasUrl = authSection.GetValue<string>("BaasUrl");
    }

    public async Task<(long viewerId, string sessionId)> DoAuth(string idToken)
    {
        return this.useLegacyLogin
            ? await this.DoLegacyAuth(idToken)
            : await this.DoBaasAuth(idToken);
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
        ValidationParams ??= await this.ConstructValidationParameters();

        JwtSecurityTokenHandler handler = new();
        TokenValidationResult validationResult = await handler.ValidateTokenAsync(
            idToken,
            ValidationParams
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

    private async Task<TokenValidationParameters> ConstructValidationParameters()
    {
        HttpResponseMessage keySetResponse = await this.client.GetAsync(
            this.baasUrl + KeySetEndpoint
        );

        if (!keySetResponse.IsSuccessStatusCode)
        {
            logger.LogError("Received failure response from BaaS: {@response}", keySetResponse);

            throw new DragaliaException(
                Models.ResultCode.COMMON_AUTH_ERROR,
                "Received failure response from BaaS"
            );
        }

        JsonWebKeySet keySet = new(await keySetResponse.Content.ReadAsStringAsync());
        return new TokenValidationParameters()
        {
            IssuerSigningKeys = keySet.GetSigningKeys(),
            ValidAudience = TokenAudience,
            ValidIssuer = TokenIssuer,
        };
    }
}
