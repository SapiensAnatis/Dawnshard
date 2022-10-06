namespace DragaliaAPI.Test.Integration.Dragalia;

public class RedoableSummonTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public RedoableSummonTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task RedoableSummonGetData_ReturnsData()
    {
        RedoableSummonGetDataResponse expectedResponse =
            new(RedoableSummonGetDataFactory.CreateData());

        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("redoable_summon/get_data", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }

    [Fact]
    public async Task RedoableSummonPreExec_ReturnsResult()
    {
        RedoableSummonPreExecResponse expectedResponse =
            new(RedoableSummonPreExecFactory.CreateData());

        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("redoable_summon/pre_exec", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}
