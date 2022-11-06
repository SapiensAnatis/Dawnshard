using DragaliaAPI.Database;
using DragaliaAPI.Models.Responses;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class UpdateNamechangeTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UpdateNamechangeTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
        _serviceScopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        var cache = _factory.Services.GetRequiredService<IDistributedCache>();
        TestUtils.InitializeCacheForTests(cache);
    }

    [Fact]
    public async Task UpdateNamechange_UpdatesDB()
    {
        string newName = "Euden 2";

        var data = new { name = newName };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("/update/namechange", content);

        using IServiceScope scope = _serviceScopeFactory.CreateScope();
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
        UpdateNamechangeResponse expectedResponse = new(new(newName));

        var data = new { name = newName };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("/update/namechange", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}
