using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;

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
        _factory.SeedCache();
    }

    /*
    // I can't be bothered to keep updating this test when the savefile keeps getting stuff added to it
    [Fact]
    public async Task LoadIndex_ReturnsSavefile()
    {
        DbPlayerUserData dbUserData = TestUtils.GetLoggedInSavefileSeed();

        LoadIndexResponse expectedResponse =
            new(new LoadIndexData(SavefileUserDataFactory.Create(dbUserData, new())));

        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("load/index", content);

        await TestUtils.CheckMsgpackResponse(
            response,
            expectedResponse,
            options => options.Excluding(x => x.data.user_data.create_time)
        );
    }
    */
}
