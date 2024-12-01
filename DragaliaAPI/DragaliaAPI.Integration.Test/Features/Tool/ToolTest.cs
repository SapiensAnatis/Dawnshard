using System.Net;
using DragaliaAPI.Features.Tool;
using DragaliaAPI.Models;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Infrastructure.DragaliaHttpConstants;

namespace DragaliaAPI.Integration.Test.Features.Tool;

/// <summary>
/// Tests <see cref="ToolController"/>
/// </summary>
public class ToolTest : TestFixture
{
    public ToolTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.SetupSaveImport();
    }

    [Fact]
    public async Task ServiceStatus_ReturnsCorrectResponse()
    {
        this.Client.DefaultRequestHeaders.Clear();

        ToolGetServiceStatusResponse response = (
            await this.Client.PostMsgpack<ToolGetServiceStatusResponse>(
                "tool/get_service_status",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.ServiceStatus.Should().Be(1);
    }

    [Theory]
    [InlineData("/tool/auth")]
    [InlineData("/tool/reauth")]
    public async Task Auth_CorrectIdToken_ReturnsOKResponse(string endpoint)
    {
        string token = TokenHelper.GetToken(
            DeviceAccountId,
            DateTime.UtcNow + TimeSpan.FromMinutes(5)
        );
        this.Client.DefaultRequestHeaders.Add(Headers.IdToken, token);

        ToolAuthResponse response = (
            await this.Client.PostMsgpack<ToolAuthResponse>(
                endpoint,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.ViewerId.Should().Be((ulong)this.ViewerId);
    }

    [Theory]
    [InlineData("/tool/signup")]
    [InlineData("/tool/auth")]
    public async Task Auth_NoAccount_CreatesNewUser(string endpoint)
    {
        string token = TokenHelper.GetToken(
            $"new account {Guid.NewGuid()}",
            DateTime.UtcNow + TimeSpan.FromMinutes(5)
        );
        this.Client.DefaultRequestHeaders.Add(Headers.IdToken, token);

        ToolAuthResponse response = (
            await this.Client.PostMsgpack<ToolAuthResponse>(
                endpoint,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.ViewerId.Should().BeGreaterThan((ulong)this.ViewerId);
    }

    [Fact]
    public async Task Auth_PendingImport_ImportsSave()
    {
        this.ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId).ExecuteUpdate(p =>
            p.SetProperty(e => e.LastSaveImportTime, DateTimeOffset.UnixEpoch)
        );

        string token = TokenHelper.GetToken(
            DeviceAccountId,
            DateTimeOffset.MaxValue,
            savefileAvailable: true,
            savefileTime: DateTimeOffset.UtcNow
        );

        this.Client.DefaultRequestHeaders.Add(Headers.IdToken, token);

        await this.Client.PostMsgpack<ToolAuthResponse>(
            "tool/auth",
            cancellationToken: TestContext.Current.CancellationToken
        );

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
        string token = TokenHelper.GetToken(
            DeviceAccountId,
            DateTime.UtcNow - TimeSpan.FromHours(5)
        );

        this.Client.DefaultRequestHeaders.Clear();
        this.Client.DefaultRequestHeaders.Add(Headers.IdToken, token);
        this.Client.DefaultRequestHeaders.Add("DeviceId", "expired_device_id");

        HttpResponseMessage response = await this.Client.PostMsgpackBasic(
            "/tool/auth",
            new ToolAuthRequest() { }
        );

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response
            .Headers.Should()
            .ContainKey("Is-Required-Refresh-Id-Token")
            .WhoseValue.Should()
            .BeEquivalentTo("true");
    }

    [Fact]
    public async Task Auth_RepeatedExpiredIdToken_ReturnsAuthError()
    {
        string token = TokenHelper.GetToken(
            DeviceAccountId,
            DateTime.UtcNow - TimeSpan.FromHours(5)
        );

        this.Client.DefaultRequestHeaders.Add(Headers.IdToken, token);
        this.Client.DefaultRequestHeaders.Add("DeviceId", "id");

        await this.Client.PostMsgpackBasic("/tool/auth", new ToolAuthRequest() { });

        HttpResponseMessage secondResponse = await this.Client.PostMsgpackBasic(
            $"/tool/auth",
            new { }
        );

        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        DragaliaResponse<ResultCodeResponse> responseBody = MessagePackSerializer.Deserialize<
            DragaliaResponse<ResultCodeResponse>
        >(
            await secondResponse.Content.ReadAsByteArrayAsync(
                TestContext.Current.CancellationToken
            ),
            cancellationToken: TestContext.Current.CancellationToken
        );

        responseBody
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeResponse>(
                    new ResultCodeResponse(ResultCode.CommonAuthError),
                    new DataHeaders(ResultCode.CommonAuthError)
                )
            );
    }

    [Theory]
    [InlineData("/tool/signup")]
    [InlineData("/tool/auth")]
    [InlineData("/tool/reauth")]
    public async Task Auth_InvalidIdToken_ReturnsIdTokenError(string endpoint)
    {
        string token = "im blue dabba dee dabba doo";
        this.Client.DefaultRequestHeaders.Add(Headers.IdToken, token);

        DragaliaResponse<ResultCodeResponse> response =
            await this.Client.PostMsgpack<ResultCodeResponse>(
                endpoint,
                new ToolAuthRequest() { },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeResponse>(
                    new(ResultCode.IdTokenError),
                    new DataHeaders(ResultCode.IdTokenError)
                )
            );
    }
}
