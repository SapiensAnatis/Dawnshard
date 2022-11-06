using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Responses;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class SignupTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public SignupTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task Signup_CorrectIdToken_ReturnsOKResponse()
    {
        SignupResponse expectedResponse = new(new SignupData(2));

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
}
