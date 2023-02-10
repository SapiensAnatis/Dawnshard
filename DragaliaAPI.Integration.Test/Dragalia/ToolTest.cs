using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Test.Utils;
using NuGet.Common;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.ToolController"/>
/// </summary>
public class ToolTest : TestFixture
{
    public ToolTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

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
<<<<<<< HEAD:DragaliaAPI.Test/Integration/Dragalia/ToolTest.cs
                    id_token = TestUtils.TokenToString(
                        TestUtils.GetToken(
                            DateTime.UtcNow + TimeSpan.FromMinutes(5),
                            IntegrationTestFixture.DeviceAccountIdConst
                        )
                    )
=======
                    id_token = TokenHelper
                        .GetToken(DateTimeOffset.UtcNow + TimeSpan.FromMinutes(5), DeviceAccountId)
                        .AsString()
>>>>>>> bb29ecf (Attempt to use containers for tests):DragaliaAPI.Integration.Test/Dragalia/ToolTest.cs
                }
            )
        ).data;

        response.viewer_id.Should().Be(1);
    }

    /*[Fact]
    public async Task Signup_ExpiredIdToken_ReturnsRefreshRequest()
    {
        HttpResponseMessage response = await client.PostMsgpackBasic(
            "/tool/signup",
            new ToolAuthRequest()
            {
                id_token = TestUtils.TokenToString(
                    TestUtils.GetToken(
<<<<<<< HEAD:DragaliaAPI.Test/Integration/Dragalia/ToolTest.cs
                        DateTime.UtcNow - TimeSpan.FromMinutes(5),
                        IntegrationTestFixture.DeviceAccountId
=======
                        DateTimeOffset.UtcNow - TimeSpan.FromMinutes(5),
                        IntegrationDeviceAccountId
>>>>>>> bb29ecf (Attempt to use containers for tests):DragaliaAPI.Integration.Test/Dragalia/ToolTest.cs
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
<<<<<<< HEAD:DragaliaAPI.Test/Integration/Dragalia/ToolTest.cs
                    id_token = TestUtils.TokenToString(
                        TestUtils.GetToken(
                            DateTime.UtcNow + TimeSpan.FromMinutes(5),
                            IntegrationTestFixture.DeviceAccountIdConst
                        )
                    )
=======
                    id_token = TokenHelper
                        .GetToken(DateTimeOffset.UtcNow + TimeSpan.FromMinutes(5), DeviceAccountId)
                        .AsString()
>>>>>>> bb29ecf (Attempt to use containers for tests):DragaliaAPI.Integration.Test/Dragalia/ToolTest.cs
                }
            )
        ).data;

        response.viewer_id.Should().Be(1);
        Guid.TryParse(response.session_id, out _).Should().BeTrue();
    }

    [Fact]
    public async Task Auth_CalledTwice_ReturnsSameSessionId()
    {
        ToolAuthRequest data =
            new()
            {
                uuid = "unused",
<<<<<<< HEAD:DragaliaAPI.Test/Integration/Dragalia/ToolTest.cs
                id_token = TestUtils.TokenToString(
                    TestUtils.GetToken(
                        DateTime.UtcNow + TimeSpan.FromMinutes(5),
                        IntegrationTestFixture.DeviceAccountIdConst
                    )
                )
=======
                id_token = TokenHelper
                    .GetToken(DateTimeOffset.UtcNow + TimeSpan.FromMinutes(5), DeviceAccountId)
                    .AsString()
>>>>>>> bb29ecf (Attempt to use containers for tests):DragaliaAPI.Integration.Test/Dragalia/ToolTest.cs
            };

        ToolAuthData response = (
            await this.Client.PostMsgpack<ToolAuthData>("/tool/auth", data)
        ).data;
        ToolAuthData response2 = (
            await this.Client.PostMsgpack<ToolAuthData>("/tool/auth", data)
        ).data;

        response.viewer_id.Should().Be(1);
        Guid.TryParse(response.session_id, out _).Should().BeTrue();
        response2.viewer_id.Should().Be(1);
        Guid.TryParse(response2.session_id, out _).Should().BeTrue();
        response.session_id.Should().Be(response2.session_id);
    }

    /*[Fact]
    public async Task Auth_ExpiredIdToken_ReturnsRefreshRequest()
    {
        HttpResponseMessage response = await client.PostMsgpackBasic(
            "/tool/auth",
            new ToolAuthRequest()
            {
                id_token = TestUtils.TokenToString(
                    TestUtils.GetToken(
<<<<<<< HEAD:DragaliaAPI.Test/Integration/Dragalia/ToolTest.cs
                        DateTime.UtcNow - TimeSpan.FromMinutes(5),
                        IntegrationTestFixture.DeviceAccountId
=======
                        DateTimeOffset.UtcNow - TimeSpan.FromMinutes(5),
                        IntegrationDeviceAccountId
>>>>>>> bb29ecf (Attempt to use containers for tests):DragaliaAPI.Integration.Test/Dragalia/ToolTest.cs
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

<<<<<<< HEAD:DragaliaAPI.Test/Integration/Dragalia/ToolTest.cs
        this.fixture.ApiContext.PlayerUserData
            .Find(IntegrationTestFixture.DeviceAccountIdConst)!
            .LastSaveImportTime = DateTime.MinValue;
        await this.fixture.ApiContext.SaveChangesAsync();
=======
        this.ApiContext.PlayerUserData.Find(DeviceAccountId)!.LastSaveImportTime =
            DateTime.MinValue.ToUniversalTime();
        await this.ApiContext.SaveChangesAsync();
>>>>>>> bb29ecf (Attempt to use containers for tests):DragaliaAPI.Integration.Test/Dragalia/ToolTest.cs

        string token = TokenHelper
            .GetToken(
                DateTimeOffset.UtcNow + TimeSpan.FromMinutes(5),
                deviceAccountId,
                savefileAvailable: true,
                savefileTime: DateTimeOffset.UtcNow - TimeSpan.FromDays(1)
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
<<<<<<< HEAD:DragaliaAPI.Test/Integration/Dragalia/ToolTest.cs
        this.fixture.ApiContext.PlayerUserData
            .Find(IntegrationTestFixture.DeviceAccountIdConst)!
            .LastSaveImportTime = DateTime.UtcNow;
        await this.fixture.ApiContext.SaveChangesAsync();

        string token = TestUtils.TokenToString(
            TestUtils.GetToken(
                DateTime.UtcNow + TimeSpan.FromMinutes(5),
                IntegrationTestFixture.DeviceAccountIdConst,
=======
        this.ApiContext.PlayerUserData.Find(DeviceAccountId)!.LastSaveImportTime =
            DateTimeOffset.UtcNow;
        await this.ApiContext.SaveChangesAsync();

        string token = TokenHelper
            .GetToken(
                DateTimeOffset.UtcNow + TimeSpan.FromMinutes(5),
                DeviceAccountId,
>>>>>>> bb29ecf (Attempt to use containers for tests):DragaliaAPI.Integration.Test/Dragalia/ToolTest.cs
                savefileAvailable: true,
                savefileTime: DateTimeOffset.UtcNow - TimeSpan.FromDays(1)
            )
            .AsString();

        ToolAuthRequest data = new() { uuid = "unused", id_token = token };

        await this.Client.PostMsgpack<ToolAuthData>("/tool/auth", data);

        this.MockBaasRequestHelper.Verify(x => x.GetSavefile(token), Times.Never);

<<<<<<< HEAD:DragaliaAPI.Test/Integration/Dragalia/ToolTest.cs
        DbPlayerUserData userData = fixture.ApiContext.PlayerUserData.Find(
            IntegrationTestFixture.DeviceAccountIdConst
        )!;
        this.fixture.ApiContext.Entry(userData).Reload();
=======
        DbPlayerUserData userData = this.ApiContext.PlayerUserData.Find(DeviceAccountId)!;
        await this.ApiContext.Entry(userData).ReloadAsync();
>>>>>>> bb29ecf (Attempt to use containers for tests):DragaliaAPI.Integration.Test/Dragalia/ToolTest.cs
        userData.Name.Should().Be("Euden");
    }

    /*[Fact]
     public async Task Auth_SaveImport_CalledTwice_DoesNotError()
     {
         // This test is useless because it passes without the thread safety mechanism.
         // Possibly because of the SQLite DB being fast.
         // Perhaps this should be revisited with proper end-to-end tests.
         string deviceAccountId = "save import id";

         this.fixture.ApiContext.PlayerUserData
<<<<<<< HEAD:DragaliaAPI.Test/Integration/Dragalia/ToolTest.cs
             .Find(this.IntegrationTestFixture.DeviceAccountId)!
=======
             .Find(IntegrationDeviceAccountId)!
>>>>>>> bb29ecf (Attempt to use containers for tests):DragaliaAPI.Integration.Test/Dragalia/ToolTest.cs
             .LastSaveImportTime = DateTime.MinValue;
         await this.fixture.ApiContext.SaveChangesAsync();

         string token = TestUtils.TokenToString(
             TestUtils.GetToken(
                 DateTimeOffset.UtcNow + TimeSpan.FromMinutes(5),
                 deviceAccountId,
                 savefileAvailable: true,
                 savefileTime: DateTimeOffset.UtcNow - TimeSpan.FromDays(1)
             )
         );

         Task<DragaliaResponse<ToolAuthData>> response1 = client.PostMsgpack<ToolAuthData>(
             "/tool/auth",
             new ToolAuthRequest() { uuid = "unused", id_token = token },
             ensureSuccessHeader: false
         );

         await Task.Delay(100);

         Task<DragaliaResponse<ToolAuthData>> response2 = client.PostMsgpack<ToolAuthData>(
             "/tool/auth",
             new ToolAuthRequest() { uuid = "unused", id_token = token },
             ensureSuccessHeader: false
         );

         DragaliaResponse<ToolAuthData>[] result = await Task.WhenAll(response1, response2);

         result[0].data_headers.result_code.Should().Be(ResultCode.Success);
         result[1].data_headers.result_code.Should().Be(ResultCode.Success);
     }*/
}
