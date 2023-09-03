using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using NuGet.Common;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.ToolController"/>
/// </summary>
public class ToolTest : TestFixture
{
    public ToolTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.Client.DefaultRequestHeaders.Remove("SID");
        this.SetupSaveImport();
    }

    [Fact]
    public async Task ServiceStatus_ReturnsCorrectResponse()
    {
        ToolGetServiceStatusData response = (
            await this.Client.PostMsgpack<ToolGetServiceStatusData>(
                "tool/get_service_status",
                new ToolGetServiceStatusRequest()
            )
        ).data;

        response.service_status.Should().Be(1);
    }

    [Fact]
    public async Task Signup_CorrectIdToken_ReturnsOKResponse()
    {
        ToolSignupData response = (
            await this.Client.PostMsgpack<ToolSignupData>(
                "/tool/signup",
                new ToolSignupRequest()
                {
                    id_token = TokenHelper
                        .GetToken(DateTime.UtcNow + TimeSpan.FromMinutes(5), DeviceAccountId)
                        .AsString()
                }
            )
        ).data;

        response.viewer_id.Should().Be((ulong)ViewerId);
    }

    /*[Fact]
    public async Task Signup_ExpiredIdToken_ReturnsRefreshRequest()
    {
        HttpResponseMessage response = await this.Client.PostMsgpackBasic(
            "/tool/signup",
            new ToolAuthRequest()
            {
                id_token = TestUtils.TokenToString(
                    TestUtils.GetToken(
                        DateTime.UtcNow - TimeSpan.FromMinutes(5),
                        IntegrationTestFixture.DeviceAccountId
                    )
                )
            }
        );

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response.Headers.Should().ContainKey("Is-Required-Refresh-Id-Token");
        response.Headers
            .GetValues("Is-Required-Refresh-Id-Token")
            .Should()
            .BeEquivalentTo(new List<string>() { "true" });
    }*/

    [Fact]
    public async Task Signup_InvalidIdToken_ReturnsIdTokenError()
    {
        DragaliaResponse<ResultCodeData> response = await this.Client.PostMsgpack<ResultCodeData>(
            "/tool/signup",
            new ToolAuthRequest() { id_token = "im blue dabba dee dabba doo" },
            ensureSuccessHeader: false
        );

        response
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>(
                    new DataHeaders(ResultCode.IdTokenError),
                    new(ResultCode.IdTokenError)
                )
            );
    }

    [Fact]
    public async Task Auth_CorrectIdToken_ReturnsOKResponse()
    {
        ToolAuthData response = (
            await this.Client.PostMsgpack<ToolAuthData>(
                "/tool/auth",
                new ToolAuthRequest()
                {
                    id_token = TokenHelper
                        .GetToken(DateTimeOffset.UtcNow + TimeSpan.FromMinutes(5), DeviceAccountId)
                        .AsString()
                }
            )
        ).data;

        response.viewer_id.Should().Be((ulong)ViewerId);
        Guid.TryParse(response.session_id, out _).Should().BeTrue();
    }

    [Fact]
    public async Task Auth_CalledTwice_ReturnsSameSessionId()
    {
        ToolAuthRequest data =
            new()
            {
                uuid = "unused",
                id_token = TokenHelper
                    .GetToken(DateTime.UtcNow + TimeSpan.FromMinutes(5), DeviceAccountId)
                    .AsString()
            };

        ToolAuthData response = (
            await this.Client.PostMsgpack<ToolAuthData>("/tool/auth", data)
        ).data;
        ToolAuthData response2 = (
            await this.Client.PostMsgpack<ToolAuthData>("/tool/auth", data)
        ).data;

        response.viewer_id.Should().Be((ulong)ViewerId);
        Guid.TryParse(response.session_id, out _).Should().BeTrue();
        response2.viewer_id.Should().Be((ulong)ViewerId);
        Guid.TryParse(response2.session_id, out _).Should().BeTrue();
        response.session_id.Should().Be(response2.session_id);
    }

    /*[Fact]
    public async Task Auth_ExpiredIdToken_ReturnsRefreshRequest()
    {
        HttpResponseMessage response = await this.Client.PostMsgpackBasic(
            "/tool/auth",
            new ToolAuthRequest()
            {
                id_token = TestUtils.TokenToString(
                    TestUtils.GetToken(
                        DateTime.UtcNow - TimeSpan.FromMinutes(5),
                        IntegrationTestFixture.DeviceAccountId
                    )
                )
            }
        );

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response.Headers.Should().ContainKey("Is-Required-Refresh-Id-Token");
        response.Headers
            .GetValues("Is-Required-Refresh-Id-Token")
            .Should()
            .BeEquivalentTo(new List<string>() { "true" });
    }*/

    [Fact]
    public async Task Auth_InvalidIdToken_ReturnsIdTokenError()
    {
        DragaliaResponse<ResultCodeData> response = await this.Client.PostMsgpack<ResultCodeData>(
            "/tool/auth",
            new ToolAuthRequest() { id_token = "im blue dabba dee dabba doo" },
            ensureSuccessHeader: false
        );

        response
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>(
                    new DataHeaders(ResultCode.IdTokenError),
                    new(ResultCode.IdTokenError)
                )
            );
    }

    [Fact]
    public async Task Auth_ValidIdToken_PendingSavefile_ImportsSavefile()
    {
        string deviceAccountId = "save import id";

        this.ApiContext.PlayerUserData.Find(DeviceAccountId)!.LastSaveImportTime =
            DateTimeOffset.MinValue;
        await this.ApiContext.SaveChangesAsync();

        string token = TokenHelper
            .GetToken(
                DateTime.UtcNow + TimeSpan.FromMinutes(5),
                deviceAccountId,
                savefileAvailable: true,
                savefileTime: DateTime.UtcNow - TimeSpan.FromDays(1)
            )
            .AsString();

        ToolAuthRequest data = new() { uuid = "unused", id_token = token };

        await this.Client.PostMsgpack<ToolAuthData>("/tool/auth", data);

        DbPlayerUserData userData = this.ApiContext.PlayerUserData.Find(deviceAccountId)!;
        await this.ApiContext.Entry(userData).ReloadAsync();
        userData.Name.Should().Be("Jay");
    }

    [Fact]
    public async Task Auth_ValidIdToken_OldSavefile_DoesNotImportSavefile()
    {
        this.ApiContext.PlayerUserData.Find(DeviceAccountId)!.LastSaveImportTime = DateTime.UtcNow;
        await this.ApiContext.SaveChangesAsync();

        string token = TokenHelper
            .GetToken(
                DateTime.UtcNow + TimeSpan.FromMinutes(5),
                DeviceAccountId,
                savefileAvailable: true,
                savefileTime: DateTime.UtcNow - TimeSpan.FromDays(1)
            )
            .AsString();
        ToolAuthRequest data = new() { uuid = "unused", id_token = token };

        await this.Client.PostMsgpack<ToolAuthData>("/tool/auth", data);

        this.MockBaasApi.Verify(x => x.GetSavefile(token), Times.Never);

        DbPlayerUserData userData = this.ApiContext.PlayerUserData.Find(DeviceAccountId)!;
        this.ApiContext.Entry(userData).Reload();
        userData.Name.Should().Be("Euden");
    }

    /*[Fact]
     public async Task Auth_SaveImport_CalledTwice_DoesNotError()
     {
         // This test is useless because it passes without the thread safety mechanism.
         // Possibly because of the SQLite DB being fast.
         // Perhaps this should be revisited with proper end-to-end tests.
         string deviceAccountId = "save import id";

         this.ApiContext.PlayerUserData
             .Find(this.IntegrationTestFixture.DeviceAccountId)!
             .LastSaveImportTime = DateTime.MinValue;
         await this.ApiContext.SaveChangesAsync();

         string token = TestUtils.TokenToString(
             TestUtils.GetToken(
                 DateTime.UtcNow + TimeSpan.FromMinutes(5),
                 deviceAccountId,
                 savefileAvailable: true,
                 savefileTime: DateTime.UtcNow - TimeSpan.FromDays(1)
             )
         );

         Task<DragaliaResponse<ToolAuthData>> response1 = this.Client.PostMsgpack<ToolAuthData>(
             "/tool/auth",
             new ToolAuthRequest() { uuid = "unused", id_token = token },
             ensureSuccessHeader: false
         );

         await Task.Delay(100);

         Task<DragaliaResponse<ToolAuthData>> response2 = this.Client.PostMsgpack<ToolAuthData>(
             "/tool/auth",
             new ToolAuthRequest() { uuid = "unused", id_token = token },
             ensureSuccessHeader: false
         );

         DragaliaResponse<ToolAuthData>[] result = await Task.WhenAll(response1, response2);

         result[0].data_headers.result_code.Should().Be(ResultCode.Success);
         result[1].data_headers.result_code.Should().Be(ResultCode.Success);
     }*/
}
