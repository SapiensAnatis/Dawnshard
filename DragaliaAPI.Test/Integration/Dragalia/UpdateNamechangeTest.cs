using DragaliaAPI.Database;
using DragaliaAPI.Models.Generated;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class UpdateNamechangeTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public UpdateNamechangeTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task UpdateNamechange_UpdatesDB()
    {
        string newName = "Euden 2";

        await client.PostMsgpack<UpdateNamechangeData>(
            "/update/namechange",
            new UpdateNamechangeRequest() { name = newName }
        );

        using IServiceScope scope = fixture.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
        apiContext.PlayerUserData
            .First(x => x.DeviceAccountId == "logged_in_id")
            .Name.Should()
            .Be(newName);
    }

    [Fact]
    public async Task UpdateNamechange_ReturnsCorrectResponse()
    {
        string newName = "Euden 2";
        UpdateNamechangeData response = (
            await client.PostMsgpack<UpdateNamechangeData>(
                "/update/namechange",
                new UpdateNamechangeRequest() { name = newName }
            )
        ).data;

        response.checked_name.Should().Be(newName);
    }
}
