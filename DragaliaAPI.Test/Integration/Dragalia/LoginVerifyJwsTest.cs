using DragaliaAPI.Models.Generated;
using MessagePack;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.LoginController"/>
/// </summary>
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
        LoginVerifyJwsData response = (
            await client.PostMsgpack<LoginVerifyJwsData>(
                "/update/namechange",
                new LoginVerifyJwsRequest()
            )
        ).data;

        response.Should().BeEquivalentTo(new LoginVerifyJwsData());
    }
}
