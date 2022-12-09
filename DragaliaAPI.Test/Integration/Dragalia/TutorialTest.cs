using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.TutorialController"/>
/// </summary>
[Collection("DragaliaIntegration")]
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

        TutorialUpdateStepData response = (
            await client.PostMsgpack<TutorialUpdateStepData>(
                "/tutorial/update_step",
                new TutorialUpdateStepRequest(step, 0, 0, 0)
            )
        ).data;

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

        UserData expUserData = fixture.Mapper.Map<UserData>(dbUserData);
        expUserData.tutorial_status = step;
        UpdateDataList expUpdateData = new() { user_data = expUserData };

        TutorialUpdateStepData response = (
            await client.PostMsgpack<TutorialUpdateStepData>(
                "/tutorial/update_step",
                new TutorialUpdateStepRequest(step, 0, 0, 0)
            )
        ).data;

        response.update_data_list.Should().BeEquivalentTo(expUpdateData);
    }

    [Fact]
    public async Task TutorialUpdateFlags_UpdatesDB()
    {
        int flag = 1020;

        TutorialUpdateFlagsData response = (
            await client.PostMsgpack<TutorialUpdateFlagsData>(
                "/tutorial/update_flags",
                new TutorialUpdateFlagsRequest() { flag_id = flag }
            )
        ).data;

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
