using DragaliaAPI.Models.Responses;
using MessagePack;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class LoginVerifyJwsTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public LoginVerifyJwsTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task VerifyJws_ReturnsOK()
    {
        VerifyJwsResponse expectedResponse = new(new VerifyJwsData());

        var data = new { jws_result = "unused" };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/login/verify_jws", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}
