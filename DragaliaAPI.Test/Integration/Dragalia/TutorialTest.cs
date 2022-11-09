using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Models.Requests;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DragaliaAPI.Database.Utils;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.TutorialController"/>
/// </summary>
public class TutorialTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public TutorialTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task TutorialUpdateStep_UpdatesDB()
    {
        int step = 2;

        var data = new { step = step };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/tutorial/update_step", content);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
        apiContext.PlayerUserData
            .First(x => x.DeviceAccountId == "logged_in_id")
            .TutorialStatus.Should()
            .Be(step);
    }

    [Fact]
    public async Task TutorialUpdateStep_ReturnsCorrectResponse()
    {
        int step = 2;
        using IServiceScope scope = fixture.Services.CreateScope();
        IUserDataRepository userDataRepository =
            scope.ServiceProvider.GetRequiredService<IUserDataRepository>();
        DbPlayerUserData dbUserData = await userDataRepository
            .GetUserData("logged_in_id")
            .SingleAsync();

        UserData userData = SavefileUserDataFactory.Create(dbUserData);
        UpdateDataList updateData = new() { user_data = userData with { tutorial_status = step } };
        TutorialUpdateStepResponse expectedResponse = new(new(step, updateData));

        var data = new { step };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/tutorial/update_step", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }

    [Fact]
    public async Task TutorialUpdateFlags_UpdatesDB()
    {
        int flag = 1020;

        UpdateFlagsRequest request = new(flag);
        byte[] payload = MessagePackSerializer.Serialize(request);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/tutorial/update_flags", content);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();

        TutorialFlagUtil
            .ConvertIntToFlagIntList(
                apiContext.PlayerUserData
                    .First(x => x.DeviceAccountId == "logged_in_id")
                    .TutorialFlag
            )
            .Should()
            .BeEquivalentTo(new List<int>() { flag });
    }
}
