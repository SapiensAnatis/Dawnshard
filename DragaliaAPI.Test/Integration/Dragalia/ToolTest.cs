using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using MessagePack;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.ToolController"/>
/// </summary>
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
                new ToolSignupRequest() { id_token = "id_token" }
            )
        ).data;

        response.viewer_id.Should().Be(1);
    }

    [Fact]
    public async Task Signup_IncorrectIdToken_ReturnsErrorResponse()
    {
        DragaliaResponse<ResultCodeData> response = await client.PostMsgpack<ResultCodeData>(
            "/tool/signup",
            new ToolSignupRequest() { id_token = "wrong_id_token" }
        );

        response
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>(
                    new DataHeaders(ResultCode.SessionError),
                    new ResultCodeData(ResultCode.SessionError)
                )
            );
    }

    [Fact]
    public async Task Auth_CorrectIdToken_ReturnsOKResponse()
    {
        ToolAuthData response = (
            await client.PostMsgpack<ToolAuthData>(
                "/tool/auth",
                new ToolAuthRequest() { id_token = "id_token" }
            )
        ).data;

        response
            .Should()
            .BeEquivalentTo(
                new ToolAuthData()
                {
                    viewer_id = 1,
                    session_id = "prepared_session_id",
                    nonce = "placeholder nonce"
                }
            );
    }

    [Fact]
    public async Task Auth_CalledTwice_ReturnsSameSessionId()
    {
        ToolAuthData expectedResponse = new(1, "prepared_session_id", "placeholder nonce");

        ToolAuthRequest data = new() { uuid = "unused", id_token = "id_token" };

        ToolAuthData response = (await client.PostMsgpack<ToolAuthData>("/tool/auth", data)).data;
        ToolAuthData response2 = (await client.PostMsgpack<ToolAuthData>("/tool/auth", data)).data;

        response.Should().BeEquivalentTo(expectedResponse);
        response2.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Auth_IncorrectIdToken_ReturnsErrorResponse()
    {
        DragaliaResponse<ResultCodeData> response = await client.PostMsgpack<ResultCodeData>(
            "/tool/auth",
            new ToolAuthRequest() { id_token = "wrong_id_token" }
        );

        response
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>(
                    new DataHeaders(ResultCode.SessionError),
                    new ResultCodeData(ResultCode.SessionError)
                )
            );
    }
}
