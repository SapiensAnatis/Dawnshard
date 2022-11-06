using DragaliaAPI.Models.Responses;
using MessagePack;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class LoginIndexTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public LoginIndexTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }
}
