using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Savefile;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class LoadIndexTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public LoadIndexTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
        var cache = _factory.Services.GetRequiredService<IDistributedCache>();
        TestUtils.InitializeCacheForTests(cache);
    }

    [Fact]
    public async Task LoadIndex_ReturnsSavefile()
    {
        LoadIndexResponse expectedResponse;

        using (var scope = _factory.Services.CreateScope())
        {
            ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
            DbSavefileUserData dbUserData = apiContext.SavefileUserData.First(
                x => x.DeviceAccountId == "logged_in_id"
            );
            SavefileUserData userData = SavefileUserDataFactory.Create(dbUserData, new());
            expectedResponse = new(new(userData));
        }

        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("load/index", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}
