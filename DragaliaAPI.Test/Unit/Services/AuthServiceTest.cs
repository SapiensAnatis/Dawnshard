using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Services.Helpers;
using DragaliaAPI.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MockQueryable.Moq;
using Newtonsoft.Json;

namespace DragaliaAPI.Test.Unit.Services;

public class AuthServiceTest
{
    private readonly AuthService authService;

    private readonly Mock<IBaasRequestHelper> mockBaasRequestHelper;
    private readonly Mock<ISessionService> mockSessionService;
    private readonly Mock<ISavefileService> mockSavefileService;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IOptionsMonitor<LoginOptions>> mockLoginOptions;
    private readonly Mock<IOptionsMonitor<BaasOptions>> mockBaasOptions;
    private readonly Mock<ILogger<AuthService>> mockLogger;

    private const string AccountId = "account id";

    public AuthServiceTest()
    {
        this.mockBaasRequestHelper = new(MockBehavior.Strict);
        this.mockSessionService = new(MockBehavior.Strict);
        this.mockSavefileService = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockBaasOptions = new(MockBehavior.Strict);
        this.mockLoginOptions = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.authService = new(
            this.mockBaasRequestHelper.Object,
            this.mockSessionService.Object,
            this.mockSavefileService.Object,
            this.mockUserDataRepository.Object,
            this.mockLoginOptions.Object,
            this.mockBaasOptions.Object,
            this.mockLogger.Object
        );

        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TestUtils.SecurityKeys);
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
            .ReturnsAsync(new Session("session id", "token", "device account id", 1));
        this.mockUserDataRepository
            .Setup(x => x.GetUserData("device account id"))
            .Returns(new List<DbPlayerUserData>()
                {
                    new() { DeviceAccountId = "id", ViewerId = 1 }
                }.AsQueryable().BuildMock());

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
        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TestUtils.SecurityKeys);

        string token = TestUtils.TokenToString(
            TestUtils.GetToken(
                this.mockBaasOptions.Object.CurrentValue.TokenIssuer,
                this.mockBaasOptions.Object.CurrentValue.TokenAudience,
                DateTime.UtcNow.AddHours(1),
                AccountId
            )
        );

        this.mockUserDataRepository
            .Setup(x => x.GetUserData(AccountId))
            .Returns(new List<DbPlayerUserData>()
                {
                    new() { DeviceAccountId = "id", ViewerId = 1 }
                }.AsQueryable().BuildMock());
        this.mockSessionService
            .Setup(x => x.CreateSession(token, AccountId, 1))
            .ReturnsAsync("session id");

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
        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TestUtils.SecurityKeys);

        string token = TestUtils.TokenToString(
            TestUtils.GetToken(
                this.mockBaasOptions.Object.CurrentValue.TokenIssuer,
                this.mockBaasOptions.Object.CurrentValue.TokenAudience,
                DateTime.UtcNow.AddHours(-1),
                AccountId
            )
        );

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

        string token = TestUtils.TokenToString(
            TestUtils.GetToken(
                this.mockBaasOptions.Object.CurrentValue.TokenIssuer,
                this.mockBaasOptions.Object.CurrentValue.TokenAudience,
                DateTime.UtcNow.AddHours(1),
                AccountId,
                savefileAvailable: true,
                savefileTime: DateTimeOffset.UtcNow
            )
        );

        LoadIndexData importSavefile = new() { user_data = new() { name = "Euden 2" } };

        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = true });

        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TestUtils.SecurityKeys);
        this.mockBaasRequestHelper.Setup(x => x.GetSavefile(token)).ReturnsAsync(importSavefile);

        this.mockUserDataRepository
            .Setup(x => x.GetUserData(AccountId))
            .Returns(new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = AccountId,
                        Name = "Euden",
                        ViewerId = 1,
                        LastSaveImportTime = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(1)
                    }
                }.AsQueryable().BuildMock());

        this.mockSessionService
            .Setup(x => x.CreateSession(token, AccountId, 1))
            .ReturnsAsync("session id");

        this.mockSavefileService
            .Setup(x => x.ThreadSafeImport(AccountId, importSavefile))
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

        string token = TestUtils.TokenToString(
            TestUtils.GetToken(
                this.mockBaasOptions.Object.CurrentValue.TokenIssuer,
                this.mockBaasOptions.Object.CurrentValue.TokenAudience,
                DateTime.UtcNow.AddHours(1),
                AccountId,
                savefileAvailable: true,
                savefileTime: DateTimeOffset.UtcNow
            )
        );

        LoadIndexData importSavefile = new() { user_data = new() { name = "Euden 2" } };

        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = true });

        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TestUtils.SecurityKeys);
        this.mockBaasRequestHelper
            .Setup(x => x.GetSavefile(token))
            .ThrowsAsync(new JsonException());

        this.mockUserDataRepository
            .Setup(x => x.GetUserData(AccountId))
            .Returns(new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = AccountId,
                        Name = "Euden",
                        ViewerId = 1,
                        LastSaveImportTime = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(1)
                    }
                }.AsQueryable().BuildMock());

        this.mockSessionService
            .Setup(x => x.CreateSession(token, AccountId, 1))
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

        string token = TestUtils.TokenToString(
            TestUtils.GetToken(
                this.mockBaasOptions.Object.CurrentValue.TokenIssuer,
                this.mockBaasOptions.Object.CurrentValue.TokenAudience,
                DateTime.UtcNow.AddHours(1),
                AccountId,
                savefileAvailable: true,
                savefileTime: DateTimeOffset.UtcNow - TimeSpan.FromMinutes(5)
            )
        );

        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = true });

        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TestUtils.SecurityKeys);

        this.mockUserDataRepository
            .Setup(x => x.GetUserData(AccountId))
            .Returns(new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = AccountId,
                        Name = "Euden",
                        ViewerId = 1,
                        LastSaveImportTime = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(2),
                    }
                }.AsQueryable().BuildMock());

        this.mockSessionService
            .Setup(x => x.CreateSession(token, AccountId, 1))
            .ReturnsAsync("session id");

        await this.authService.DoAuth(token);

        this.mockBaasOptions.VerifyAll();
        this.mockLoginOptions.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockUserDataRepository.Verify(x => x.UpdateSaveImportTime(AccountId), Times.Never);
        this.mockSessionService.VerifyAll();
        this.mockSavefileService.Verify(
            x => x.Import(AccountId, It.IsAny<LoadIndexData>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DoAuth_NoSavefileUploaded_DoesNotImportSave()
    {
        this.mockBaasOptions
            .Setup(x => x.CurrentValue)
            .Returns(new BaasOptions() { TokenAudience = "audience", TokenIssuer = "issuer" });

        string token = TestUtils.TokenToString(
            TestUtils.GetToken(
                this.mockBaasOptions.Object.CurrentValue.TokenIssuer,
                this.mockBaasOptions.Object.CurrentValue.TokenAudience,
                DateTime.UtcNow.AddHours(1),
                AccountId,
                savefileAvailable: false,
                savefileTime: DateTimeOffset.MaxValue
            )
        );

        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new LoginOptions() { UseBaasLogin = true });

        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TestUtils.SecurityKeys);

        this.mockUserDataRepository
            .Setup(x => x.GetUserData(AccountId))
            .Returns(new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = AccountId,
                        Name = "Euden",
                        ViewerId = 1,
                        LastSaveImportTime = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(2),
                    }
                }.AsQueryable().BuildMock());

        this.mockSessionService
            .Setup(x => x.CreateSession(token, AccountId, 1))
            .ReturnsAsync("session id");

        await this.authService.DoAuth(token);

        this.mockBaasOptions.VerifyAll();
        this.mockLoginOptions.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockUserDataRepository.Verify(x => x.UpdateSaveImportTime(AccountId), Times.Never);
        this.mockSessionService.VerifyAll();
        this.mockSavefileService.Verify(
            x => x.Import(AccountId, It.IsAny<LoadIndexData>()),
            Times.Never
        );
    }
}
