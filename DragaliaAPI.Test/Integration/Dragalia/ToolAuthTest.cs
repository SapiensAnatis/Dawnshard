using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Responses;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class ToolAuthTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public ToolAuthTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task Auth_CorrectIdToken_ReturnsOKResponse()
    {
        ToolAuthResponse expectedResponse =
            new(new AuthResponseData(2, "prepared_session_id", "placeholder nonce"));

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
            new(new AuthResponseData(2, "prepared_session_id", "placeholder nonce"));

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
