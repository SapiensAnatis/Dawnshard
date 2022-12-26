using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Services.Helpers;
using DragaliaAPI.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MockQueryable.Moq;

namespace DragaliaAPI.Test.Unit.Services;

[Collection("DragaliaIntegration")]
public class AuthServiceTest : IClassFixture<AuthServiceTestFixture>
{
    private readonly AuthService authService;

    private readonly Mock<IBaasRequestHelper> mockBaasRequestHelper;
    private readonly Mock<ISessionService> mockSessionService;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IDeviceAccountRepository> mockDeviceAccountRepository;
    private readonly Mock<IOptionsMonitor<DragaliaAuthOptions>> mockOptions;
    private readonly Mock<ILogger<AuthService>> mockLogger;
    private readonly AuthServiceTestFixture fixture;

    public AuthServiceTest(AuthServiceTestFixture fixture)
    {
        this.mockBaasRequestHelper = new(MockBehavior.Strict);
        this.mockSessionService = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockDeviceAccountRepository = new(MockBehavior.Strict);
        this.mockOptions = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.authService = new(
            this.mockBaasRequestHelper.Object,
            this.mockSessionService.Object,
            this.mockUserDataRepository.Object,
            this.mockDeviceAccountRepository.Object,
            this.mockOptions.Object,
            this.mockLogger.Object
        );

        this.fixture = fixture;
        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TestUtils.SecurityKeys);
    }

    [Fact]
    public async Task DoAuth_LegacyAuthEnabled_UsesLegacyAuth()
    {
        this.mockOptions
            .Setup(x => x.CurrentValue)
            .Returns(new DragaliaAuthOptions() { UseLegacyLogin = true });

        // These session service methods are not used in the new auth
        this.mockSessionService
            .Setup(x => x.ActivateSession("id token"))
            .ReturnsAsync("session id");
        this.mockSessionService
            .Setup(x => x.GetDeviceAccountId_SessionId("session id"))
            .ReturnsAsync("device account id");
        this.mockUserDataRepository
            .Setup(x => x.GetUserData("device account id"))
            .Returns(
                new List<DbPlayerUserData>() { new() { ViewerId = 1 } }.AsQueryable().BuildMock()
            );

        (await this.authService.DoAuth("id token")).Should().BeEquivalentTo((1, "session id"));

        this.mockSessionService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockDeviceAccountRepository.VerifyAll();
        this.mockOptions.VerifyAll();
    }

    [Fact]
    public async Task DoAuth_ValidToken_ReturnsViewerId()
    {
        this.mockOptions
            .Setup(x => x.CurrentValue)
            .Returns(
                new DragaliaAuthOptions()
                {
                    UseLegacyLogin = false,
                    TokenAudience = "audience",
                    TokenIssuer = "issuer"
                }
            );
        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TestUtils.SecurityKeys);

        string token = fixture.GetToken(
            this.mockOptions.Object.CurrentValue.TokenIssuer,
            this.mockOptions.Object.CurrentValue.TokenAudience,
            DateTime.UtcNow.AddHours(1),
            "account id"
        );

        this.mockUserDataRepository
            .Setup(x => x.GetUserData("account id"))
            .Returns(
                new List<DbPlayerUserData>() { new() { ViewerId = 1 } }.AsQueryable().BuildMock()
            );
        this.mockSessionService
            .Setup(x => x.CreateSession("account id", token))
            .ReturnsAsync("session id");

        (await this.authService.DoAuth(token)).Should().BeEquivalentTo((1, "session id"));

        this.mockSessionService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockDeviceAccountRepository.VerifyAll();
        this.mockOptions.VerifyAll();
        // Not verifying BaasRequestHelper as it is only sometimes used
    }

    [Fact]
    public async Task DoAuth_ExpiredToken_ThrowsSessionException()
    {
        this.mockOptions
            .Setup(x => x.CurrentValue)
            .Returns(
                new DragaliaAuthOptions()
                {
                    UseLegacyLogin = false,
                    TokenAudience = "audience",
                    TokenIssuer = "issuer"
                }
            );
        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TestUtils.SecurityKeys);

        string token = fixture.GetToken(
            this.mockOptions.Object.CurrentValue.TokenIssuer,
            this.mockOptions.Object.CurrentValue.TokenAudience,
            DateTime.UtcNow.AddHours(-1),
            "account id"
        );

        await this.authService
            .Invoking(x => x.DoAuth(token))
            .Should()
            .ThrowExactlyAsync<SessionException>();

        this.mockSessionService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockDeviceAccountRepository.VerifyAll();
        this.mockOptions.VerifyAll();
    }

    [Fact]
    public async Task DoAuth_InvalidToken_ThrowsDragaliaException()
    {
        this.mockOptions
            .Setup(x => x.CurrentValue)
            .Returns(
                new DragaliaAuthOptions()
                {
                    UseLegacyLogin = false,
                    TokenAudience = "audience",
                    TokenIssuer = "issuer"
                }
            );

        await this.authService
            .Invoking(
                x =>
                    x.DoAuth(
                        "We cry tears of mascara in the bathroom / Honey life is just a classroom / Ah-ah-ah-ah"
                    )
            )
            .Should()
            .ThrowExactlyAsync<DragaliaException>()
            .Where(x => x.Code == Models.ResultCode.COMMON_AUTH_ERROR);

        this.mockSessionService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockDeviceAccountRepository.VerifyAll();
        this.mockOptions.VerifyAll();
    }
}
