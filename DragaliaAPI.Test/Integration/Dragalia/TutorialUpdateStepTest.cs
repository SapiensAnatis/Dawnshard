using System;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Savefile;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using static System.Formats.Asn1.AsnWriter;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class TutorialUpdateStepTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public TutorialUpdateStepTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
        _factory.SeedCache();
    }

    [Fact]
    public async Task TutorialUpdateStep_UpdatesDB()
    {
        int step = 2;

        var data = new { step = step };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("/tutorial/update_step", content);

        using (var scope = _factory.Services.CreateScope())
        {
            ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
            apiContext.SavefileUserData
                .First(x => x.DeviceAccountId == "logged_in_id")
                .TutorialStatus.Should()
                .Be(step);
        }
    }

    [Fact]
    public async Task TutorialUpdateStep_ReturnsCorrectResponse()
    {
        int step = 2;
        DbSavefileUserData dbUserData = TestUtils.GetLoggedInSavefileSeed();

        SavefileUserData userData = SavefileUserDataFactory.Create(dbUserData, new());
        UpdateDataList updateData = new(userData with { tutorial_status = step });
        TutorialUpdateStepResponse expectedResponse = new(new(step, updateData));

        var data = new { step = step };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("/tutorial/update_step", content);

        await TestUtils.CheckMsgpackResponse(
            response,
            expectedResponse,
            options => options.Excluding(x => x.data.update_data_list.user_data.create_time)
        );
    }
}
