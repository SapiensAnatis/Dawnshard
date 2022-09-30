using MessagePack;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class ToolAuthTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ToolAuthTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Auth_CorrectIdToken_ReturnsOKResponse()
    {
        AuthResponse expectedResponse = new(10000000002, "prepared_session_id");

        var data = new { uuid = "unused", id_token = "id_token" };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("/tool/auth", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }

    [Fact]
    public async Task Auth_IncorrectIdToken_ReturnsErrorResponse()
    {
        ServerErrorResponse expectedResponse = new();

        var data = new { uuid = "unused", id_token = "wrong_id_token" };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("/tool/auth", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}