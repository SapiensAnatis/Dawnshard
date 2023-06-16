using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Dragalia;

public class LoginIndexTest : TestFixture
{
    public LoginIndexTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task LoginIndex_ReturnsSuccess()
    {
        await this.Client
            .Invoking(x => x.PostMsgpack<LoginIndexData>("/login/index", new()))
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task LoginIndex_AddsMissingBuildings()
    {
        await this.ApiContext.PlayerFortBuilds
            .Where(
                x =>
                    x.DeviceAccountId == DeviceAccountId
                    && (x.PlantId == FortPlants.Smithy || x.PlantId == FortPlants.TheHalidom)
            )
            .ExecuteDeleteAsync();

        await this.ApiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .ExecuteUpdateAsync(e => e.SetProperty(p => p.LastLoginTime, DateTimeOffset.UnixEpoch));

        this.ApiContext.PlayerStoryState.AddRange(
            new List<DbPlayerStoryState>()
            {
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    StoryType = StoryTypes.Quest,
                    StoryId = TutorialService.TutorialStoryIds.Halidom,
                    State = StoryState.Read,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    StoryType = StoryTypes.Quest,
                    StoryId = TutorialService.TutorialStoryIds.Smithy,
                    State = StoryState.Read,
                }
            }
        );
        await this.ApiContext.SaveChangesAsync();

        LoginIndexData data = (
            await this.Client.PostMsgpack<LoginIndexData>("/login/index", new LoginIndexRequest())
        ).data;

        data.update_data_list.build_list
            .Should()
            .Contain(x => x.plant_id == FortPlants.TheHalidom)
            .And.Contain(x => x.plant_id == FortPlants.Smithy);

        this.ApiContext.PlayerFortBuilds
            .Should()
            .Contain(
                x => x.DeviceAccountId == DeviceAccountId && x.PlantId == FortPlants.TheHalidom
            )
            .And.Contain(
                x => x.DeviceAccountId == DeviceAccountId && x.PlantId == FortPlants.Smithy
            );
    }
}
