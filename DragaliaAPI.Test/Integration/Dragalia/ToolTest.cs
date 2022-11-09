using DragaliaAPI.Models.Responses;
using DragaliaAPI.Models.Responses.Base;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

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
    public async Task ServiceStatus_ReturnsCorrectJSON()
    {
        ServiceStatusResponse expectedResponse = new(new ServiceStatusData(1));

        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("tool/get_service_status", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }

    [Fact]
    public async Task Signup_CorrectIdToken_ReturnsOKResponse()
    {
        SignupResponse expectedResponse = new(new SignupData(1));

        var data = new { id_token = "id_token" };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/tool/signup", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }

    [Fact]
    public async Task Signup_IncorrectIdToken_ReturnsErrorResponse()
    {
        ServerErrorResponse expectedResponse = new();

        var data = new { id_token = "wrong_id_token" };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/tool/signup", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }

    [Fact]
    public async Task Auth_CorrectIdToken_ReturnsOKResponse()
    {
        ToolAuthResponse expectedResponse =
            new(new AuthResponseData(1, "prepared_session_id", "placeholder nonce"));

        var data = new { uuid = "unused", id_token = "id_token" };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/tool/auth", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }

    [Fact]
    public async Task Auth_CalledTwice_ReturnsSameSessionId()
    {
        ToolAuthResponse expectedResponse =
            new(new AuthResponseData(1, "prepared_session_id", "placeholder nonce"));

        var data = new { uuid = "unused", id_token = "id_token" };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/tool/auth", content);
        HttpResponseMessage responseTwo = await client.PostAsync("/tool/auth", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
        await TestUtils.CheckMsgpackResponse(responseTwo, expectedResponse);
    }

    [Fact]
    public async Task Auth_IncorrectIdToken_ReturnsErrorResponse()
    {
        ServerErrorResponse expectedResponse = new();

        var data = new { uuid = "unused", id_token = "wrong_id_token" };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/tool/auth", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}
