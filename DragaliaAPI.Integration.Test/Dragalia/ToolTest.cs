using DragaliaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.ToolController"/>
/// </summary>
public class ToolTest : TestFixture
{
    private const string IdTokenHeader = "ID-TOKEN";

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

    [Theory]
    [InlineData("/tool/signup")]
    [InlineData("/tool/auth")]
    [InlineData("/tool/reauth")]
    public async Task Auth_CorrectIdToken_ReturnsOKResponse(string endpoint)
    {
        string token = TokenHelper
            .GetToken(DateTime.UtcNow + TimeSpan.FromMinutes(5), DeviceAccountId)
            .AsString();
        this.Client.DefaultRequestHeaders.Add(IdTokenHeader, token);

        ToolAuthData response = (
            await this.Client.PostMsgpack<ToolAuthData>(endpoint, new ToolAuthRequest() { })
        ).data;

        response.viewer_id.Should().Be((ulong)ViewerId);
    }

    [Fact]
    public async Task Auth_PendingImport_ImportsSave()
    {
        this.ApiContext.PlayerUserData.ExecuteUpdate(
            p => p.SetProperty(e => e.LastSaveImportTime, DateTimeOffset.UnixEpoch)
        );

        string token = TokenHelper
            .GetToken(
                DateTimeOffset.MaxValue,
                DeviceAccountId,
                savefileAvailable: true,
                savefileTime: DateTimeOffset.UtcNow
            )
            .AsString();
        this.Client.DefaultRequestHeaders.Add(IdTokenHeader, token);

        await this.Client.PostMsgpack<ToolAuthData>("tool/auth", new ToolAuthRequest() { });

        this.ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == this.ViewerId)
            .LastSaveImportTime.Should()
            .BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));

        this.ApiContext.PlayerCharaData.AsNoTracking()
            .Where(x => x.ViewerId == this.ViewerId)
            .Should()
            .HaveCountGreaterThan(200);
    }

    [Fact]
    public async Task Auth_ExpiredIdToken_ReturnsRefreshRequest()
    {
        string token = TokenHelper
            .GetToken(DateTime.UtcNow - TimeSpan.FromHours(5), DeviceAccountId)
            .AsString();
        this.Client.DefaultRequestHeaders.Add(IdTokenHeader, token);
        this.Client.DefaultRequestHeaders.Add("DeviceId", "id");

        HttpResponseMessage response = await this.Client.PostMsgpackBasic(
            "/tool/auth",
            new ToolAuthRequest() { }
        );

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response.Headers.Should().ContainKey("Is-Required-Refresh-Id-Token");
        response
            .Headers.GetValues("Is-Required-Refresh-Id-Token")
            .Should()
            .BeEquivalentTo(new List<string>() { "true" });
    }

    [Theory]
    [InlineData("/tool/signup")]
    [InlineData("/tool/auth")]
    [InlineData("/tool/reauth")]
    public async Task Auth_InvalidIdToken_ReturnsIdTokenError(string endpoint)
    {
        string token = "im blue dabba dee dabba doo";
        this.Client.DefaultRequestHeaders.Add(IdTokenHeader, token);

        DragaliaResponse<ResultCodeData> response = await this.Client.PostMsgpack<ResultCodeData>(
            endpoint,
            new ToolAuthRequest() { },
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
}
