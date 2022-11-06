using DragaliaAPI.Models.Responses;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class ServiceStatusTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public ServiceStatusTest(IntegrationTestFixture fixture)
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
}
