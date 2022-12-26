using System.Net.Http.Headers;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using MessagePack;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.ToolController"/>
/// </summary>
[Collection("DragaliaIntegration")]
public class ToolTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public ToolTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task ServiceStatus_ReturnsCorrectResponse()
    {
        ToolGetServiceStatusData response = (
            await client.PostMsgpack<ToolGetServiceStatusData>(
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
            await client.PostMsgpack<ToolSignupData>(
                "/tool/signup",
                new ToolSignupRequest() { id_token = this.fixture.BuildValidToken() }
            )
        ).data;

        response.viewer_id.Should().Be(1);
    }

    [Fact]
    public async Task Signup_ExpiredIdToken_ReturnsRefreshRequest()
    {
        HttpResponseMessage response = await client.PostMsgpackBasic(
            "/tool/signup",
            new ToolAuthRequest()
            {
                id_token = fixture.BuildValidToken(DateTime.UtcNow.AddHours(-1))
            }
        );

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response.Headers.Should().ContainKey("Is-Required-Refresh-Id-Token");
        response.Headers
            .GetValues("Is-Required-Refresh-Id-Token")
            .Should()
            .BeEquivalentTo(new List<string>() { "true" });
    }

    [Fact]
    public async Task Signup_InvalidIdToken_ReturnsResultCodeError()
    {
        DragaliaResponse<ResultCodeData> response = await client.PostMsgpack<ResultCodeData>(
            "/tool/signup",
            new ToolAuthRequest() { id_token = "im blue dabba dee dabba doo" }
        );

        response
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>()
                {
                    data_headers = new(ResultCode.COMMON_AUTH_ERROR),
                    data = new(ResultCode.COMMON_AUTH_ERROR)
                }
            );
    }

    [Fact]
    public async Task Auth_CorrectIdToken_ReturnsOKResponse()
    {
        ToolAuthData response = (
            await client.PostMsgpack<ToolAuthData>(
                "/tool/auth",
                new ToolAuthRequest() { id_token = this.fixture.BuildValidToken() }
            )
        ).data;

        response.viewer_id.Should().Be(1);
        Guid.TryParse(response.session_id, out _).Should().BeTrue();
    }

    [Fact]
    public async Task Auth_CalledTwice_ReturnsSameSessionId()
    {
        ToolAuthRequest data = new() { uuid = "unused", id_token = this.fixture.BuildValidToken() };

        ToolAuthData response = (await client.PostMsgpack<ToolAuthData>("/tool/auth", data)).data;
        ToolAuthData response2 = (await client.PostMsgpack<ToolAuthData>("/tool/auth", data)).data;

        response.viewer_id.Should().Be(1);
        Guid.TryParse(response.session_id, out _).Should().BeTrue();
        response2.viewer_id.Should().Be(1);
        Guid.TryParse(response2.session_id, out _).Should().BeTrue();
    }

    [Fact]
    public async Task Auth_ExpiredIdToken_ReturnsRefreshRequest()
    {
        HttpResponseMessage response = await client.PostMsgpackBasic(
            "/tool/auth",
            new ToolAuthRequest()
            {
                id_token = fixture.BuildValidToken(DateTime.UtcNow.AddHours(-1))
            }
        );

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response.Headers.Should().ContainKey("Is-Required-Refresh-Id-Token");
        response.Headers
            .GetValues("Is-Required-Refresh-Id-Token")
            .Should()
            .BeEquivalentTo(new List<string>() { "true" });
    }

    [Fact]
    public async Task Auth_InvalidIdToken_ReturnsResultCodeError()
    {
        DragaliaResponse<ResultCodeData> response = await client.PostMsgpack<ResultCodeData>(
            "/tool/auth",
            new ToolAuthRequest() { id_token = "im blue dabba dee dabba doo" }
        );

        response
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>()
                {
                    data_headers = new(ResultCode.COMMON_AUTH_ERROR),
                    data = new(ResultCode.COMMON_AUTH_ERROR)
                }
            );
    }
}
