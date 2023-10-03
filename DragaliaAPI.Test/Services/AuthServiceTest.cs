using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Test.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MockQueryable.Moq;
using Newtonsoft.Json;

namespace DragaliaAPI.Test.Services;

public class AuthServiceTest
{
    private readonly AuthService authService;

    private readonly Mock<IBaasApi> mockBaasRequestHelper;
    private readonly Mock<ISessionService> mockSessionService;
    private readonly Mock<ISavefileService> mockSavefileService;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IOptionsMonitor<LoginOptions>> mockLoginOptions;
    private readonly Mock<IOptionsMonitor<BaasOptions>> mockBaasOptions;
    private readonly Mock<IDateTimeProvider> mockDateTimeProvider;
    private readonly Mock<ILogger<AuthService>> mockLogger;

    private const string AccountId = "account id";

    public AuthServiceTest()
    {
        this.mockBaasRequestHelper = new(MockBehavior.Strict);
        this.mockSessionService = new(MockBehavior.Strict);
        this.mockSavefileService = new(MockBehavior.Strict);
        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockBaasOptions = new(MockBehavior.Strict);
        this.mockLoginOptions = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);
        this.mockDateTimeProvider = new(MockBehavior.Strict);

        this.mockDateTimeProvider.SetupGet(x => x.UtcNow).Returns(DateTimeOffset.UnixEpoch);

        this.authService = new(
            this.mockBaasRequestHelper.Object,
            this.mockSessionService.Object,
            this.mockSavefileService.Object,
            this.mockPlayerIdentityService.Object,
            this.mockUserDataRepository.Object,
            this.mockLoginOptions.Object,
            this.mockBaasOptions.Object,
            this.mockLogger.Object,
            this.mockDateTimeProvider.Object
        );

        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TokenHelper.SecurityKeys);
    }

    [Fact]
    [Obsolete(ObsoleteReasons.BaaS)]
    public async Task DoAuth_LegacyAuthEnabled_UsesLegacyAuth()
    {
        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = false });

        // These session service methods are not used in the new auth
        this.mockSessionService
            .Setup(x => x.ActivateSession("id token"))
            .ReturnsAsync("session id");
        this.mockSessionService
            .Setup(x => x.LoadSessionSessionId("session id"))
            .ReturnsAsync(
                new Session("session id", "token", "device account id", 1, DateTimeOffset.UnixEpoch)
            );
        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>()
                {
                    new() { DeviceAccountId = "id", ViewerId = 1 }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockPlayerIdentityService
            .Setup(x => x.StartUserImpersonation("device account id", 1))
            .Returns(new Mock<IDisposable>(MockBehavior.Loose).Object);

        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("device account id");

        (await this.authService.DoAuth("id token")).Should().BeEquivalentTo((1, "session id"));

        this.mockSessionService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockLoginOptions.VerifyAll();
        this.mockBaasRequestHelper.Verify(x => x.GetKeys(), Times.Never);
    }

    [Fact]
    public async Task DoAuth_ValidToken_ReturnsViewerId()
    {
        this.mockBaasOptions
            .Setup(x => x.CurrentValue)
            .Returns(new BaasOptions() { TokenAudience = "audience", TokenIssuer = "issuer" });
        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = true });
        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TokenHelper.SecurityKeys);

        string token = TokenHelper
            .GetToken(
                this.mockBaasOptions.Object.CurrentValue.TokenIssuer,
                this.mockBaasOptions.Object.CurrentValue.TokenAudience,
                DateTimeOffset.UtcNow.AddHours(1),
                AccountId
            )
            .AsString();

        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>()
                {
                    new() { DeviceAccountId = "id", ViewerId = 1 }
                }
                    .AsQueryable()
                    .BuildMock()
            );
        this.mockSessionService
            .Setup(x => x.CreateSession(token, AccountId, 1, DateTimeOffset.UnixEpoch))
            .ReturnsAsync("session id");

        this.mockPlayerIdentityService
            .Setup(x => x.StartUserImpersonation(AccountId, null))
            .Returns(new Mock<IDisposable>(MockBehavior.Loose).Object);

        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns(AccountId);

        (await this.authService.DoAuth(token)).Should().BeEquivalentTo((1, "session id"));

        this.mockBaasOptions.VerifyAll();
        this.mockLoginOptions.VerifyAll();
        this.mockBaasRequestHelper.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockSessionService.VerifyAll();
    }

    [Fact]
    public async Task DoAuth_ExpiredToken_ThrowsSessionException()
    {
        this.mockBaasOptions
            .Setup(x => x.CurrentValue)
            .Returns(new BaasOptions() { TokenAudience = "audience", TokenIssuer = "issuer" });
        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = true });
        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TokenHelper.SecurityKeys);

        string token = TokenHelper
            .GetToken(
                this.mockBaasOptions.Object.CurrentValue.TokenIssuer,
                this.mockBaasOptions.Object.CurrentValue.TokenAudience,
                DateTimeOffset.UtcNow.AddHours(-1),
                AccountId
            )
            .AsString();

        await this.authService
            .Invoking(x => x.DoAuth(token))
            .Should()
            .ThrowExactlyAsync<SecurityTokenExpiredException>();

        this.mockBaasOptions.VerifyAll();
        this.mockLoginOptions.VerifyAll();
        this.mockBaasRequestHelper.VerifyAll();
    }

    [Fact]
    public async Task DoAuth_InvalidToken_ThrowsDragaliaException()
    {
        this.mockBaasOptions
            .Setup(x => x.CurrentValue)
            .Returns(new BaasOptions() { TokenAudience = "audience", TokenIssuer = "issuer" });
        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = true });
        await this.authService
            .Invoking(
                x =>
                    x.DoAuth(
                        "We cry tears of mascara in the bathroom / Honey life is just a classroom / Ah-ah-ah-ah"
                    )
            )
            .Should()
            .ThrowExactlyAsync<DragaliaException>()
            .Where(x => x.Code == Models.ResultCode.IdTokenError);

        this.mockBaasOptions.VerifyAll();
        this.mockLoginOptions.VerifyAll();
        this.mockBaasRequestHelper.VerifyAll();
    }

    [Fact]
    public async Task DoAuth_SavefileUploaded_IsNewer_ImportsSave()
    {
        this.mockBaasOptions
            .Setup(x => x.CurrentValue)
            .Returns(new BaasOptions() { TokenAudience = "audience", TokenIssuer = "issuer" });

        string token = TokenHelper
            .GetToken(
                this.mockBaasOptions.Object.CurrentValue.TokenIssuer,
                this.mockBaasOptions.Object.CurrentValue.TokenAudience,
                DateTimeOffset.UtcNow.AddHours(1),
                AccountId,
                savefileAvailable: true,
                savefileTime: DateTimeOffset.UtcNow
            )
            .AsString();

        LoadIndexData importSavefile =
            new()
            {
                user_data = new() { name = "Euden 2" },
                fort_bonus_list = null!
            };

        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = true });

        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TokenHelper.SecurityKeys);
        this.mockBaasRequestHelper.Setup(x => x.GetSavefile(token)).ReturnsAsync(importSavefile);

        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = AccountId,
                        Name = "Euden",
                        ViewerId = 1,
                        LastSaveImportTime = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(15)
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockPlayerIdentityService
            .Setup(x => x.StartUserImpersonation(AccountId, null))
            .Returns(new Mock<IDisposable>(MockBehavior.Loose).Object);

        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns(AccountId);

        this.mockSessionService
            .Setup(x => x.CreateSession(token, AccountId, 1, DateTimeOffset.UnixEpoch))
            .ReturnsAsync("session id");

        this.mockSavefileService
            .Setup(x => x.ThreadSafeImport(It.IsAny<LoadIndexData>()))
            .Returns(Task.CompletedTask);

        await this.authService.DoAuth(token);

        this.mockBaasOptions.VerifyAll();
        this.mockLoginOptions.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockSessionService.VerifyAll();
        this.mockSavefileService.VerifyAll();
    }

    [Fact]
    public async Task DoAuth_SavefileUploaded_IsNewer_SaveInvalid_HandlesException()
    {
        this.mockBaasOptions
            .Setup(x => x.CurrentValue)
            .Returns(new BaasOptions() { TokenAudience = "audience", TokenIssuer = "issuer" });

        string token = TokenHelper
            .GetToken(
                this.mockBaasOptions.Object.CurrentValue.TokenIssuer,
                this.mockBaasOptions.Object.CurrentValue.TokenAudience,
                DateTimeOffset.UtcNow.AddHours(1),
                AccountId,
                savefileAvailable: true,
                savefileTime: DateTimeOffset.UtcNow
            )
            .AsString();

        LoadIndexData importSavefile =
            new()
            {
                user_data = new() { name = "Euden 2" },
                fort_bonus_list = null!
            };

        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = true });

        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TokenHelper.SecurityKeys);
        this.mockBaasRequestHelper
            .Setup(x => x.GetSavefile(token))
            .ThrowsAsync(new JsonException());

        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = AccountId,
                        Name = "Euden",
                        ViewerId = 1,
                        LastSaveImportTime = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(1)
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockPlayerIdentityService
            .Setup(x => x.StartUserImpersonation(AccountId, null))
            .Returns(new Mock<IDisposable>(MockBehavior.Loose).Object);

        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns(AccountId);

        this.mockSessionService
            .Setup(x => x.CreateSession(token, AccountId, 1, DateTimeOffset.UnixEpoch))
            .ReturnsAsync("session id");

        await this.authService.Invoking(x => x.DoAuth(token)).Should().NotThrowAsync();

        this.mockBaasOptions.VerifyAll();
        this.mockLoginOptions.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockSessionService.VerifyAll();
        this.mockSavefileService.VerifyAll();
    }

    [Fact]
    public async Task DoAuth_SavefileUploaded_IsOlder_DoesNotImportSave()
    {
        this.mockBaasOptions
            .Setup(x => x.CurrentValue)
            .Returns(new BaasOptions() { TokenAudience = "audience", TokenIssuer = "issuer" });

        string token = TokenHelper
            .GetToken(
                this.mockBaasOptions.Object.CurrentValue.TokenIssuer,
                this.mockBaasOptions.Object.CurrentValue.TokenAudience,
                DateTimeOffset.UtcNow.AddHours(1),
                AccountId,
                savefileAvailable: true,
                savefileTime: DateTimeOffset.UtcNow - TimeSpan.FromMinutes(5)
            )
            .AsString();

        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = true });

        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TokenHelper.SecurityKeys);

        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = AccountId,
                        Name = "Euden",
                        ViewerId = 1,
                        LastSaveImportTime = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(2),
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockSessionService
            .Setup(x => x.CreateSession(token, AccountId, 1, DateTimeOffset.UnixEpoch))
            .ReturnsAsync("session id");

        this.mockPlayerIdentityService
            .Setup(x => x.StartUserImpersonation("account id", null))
            .Returns(new Mock<IDisposable>(MockBehavior.Loose).Object);

        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("account id");

        await this.authService.DoAuth(token);

        this.mockBaasOptions.VerifyAll();
        this.mockLoginOptions.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockUserDataRepository.Verify(x => x.UpdateSaveImportTime(), Times.Never);
        this.mockSessionService.VerifyAll();
        this.mockSavefileService.Verify(x => x.Import(It.IsAny<LoadIndexData>()), Times.Never);
    }

    [Fact]
    public async Task DoAuth_NoSavefileUploaded_DoesNotImportSave()
    {
        this.mockBaasOptions
            .Setup(x => x.CurrentValue)
            .Returns(new BaasOptions() { TokenAudience = "audience", TokenIssuer = "issuer" });

        string token = TokenHelper
            .GetToken(
                this.mockBaasOptions.Object.CurrentValue.TokenIssuer,
                this.mockBaasOptions.Object.CurrentValue.TokenAudience,
                DateTimeOffset.UtcNow.AddHours(1),
                AccountId,
                savefileAvailable: false,
                savefileTime: DateTimeOffset.MaxValue
            )
            .AsString();

        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = true });

        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TokenHelper.SecurityKeys);

        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = AccountId,
                        Name = "Euden",
                        ViewerId = 1,
                        LastSaveImportTime = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(2),
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockPlayerIdentityService
            .Setup(x => x.StartUserImpersonation(AccountId, null))
            .Returns(new Mock<IDisposable>(MockBehavior.Loose).Object);

        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns(AccountId);

        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns(AccountId);

        this.mockSessionService
            .Setup(x => x.CreateSession(token, AccountId, 1, DateTimeOffset.UnixEpoch))
            .ReturnsAsync("session id");

        await this.authService.DoAuth(token);

        this.mockBaasOptions.VerifyAll();
        this.mockLoginOptions.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockUserDataRepository.Verify(x => x.UpdateSaveImportTime(), Times.Never);
        this.mockSessionService.VerifyAll();
        this.mockSavefileService.Verify(x => x.Import(It.IsAny<LoadIndexData>()), Times.Never);
    }
}
