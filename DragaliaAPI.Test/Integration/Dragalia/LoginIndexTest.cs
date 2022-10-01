using MessagePack;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class LoginIndexTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public LoginIndexTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task VerifyJws_ReturnsOK()
    {
        VerifyJwsResponse expectedResponse = new(new VerifyJwsData());

        var data = new { jws_result = "unused" };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("/login/index", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}