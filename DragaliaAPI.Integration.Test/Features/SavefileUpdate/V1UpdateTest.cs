using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V1UpdateTest : SavefileUpdateTestFixture
{
    public V1UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.ApiContext.PlayerFortBuilds.ExecuteDelete();
    }

    [Fact]
    public async Task V1Update_NoFort_StoryComplete_Adds()
    {
        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                DeviceAccountId = DeviceAccountId,
                StoryType = StoryTypes.Quest,
                State = StoryState.Read,
                StoryId = TutorialService.TutorialStoryIds.Halidom
            }
        );

        LoadIndexData data = (
            await this.Client.PostMsgpack<LoadIndexData>("/load/index", new LoadIndexRequest())
        ).data;

        data.build_list.Should().Contain(x => x.plant_id == FortPlants.TheHalidom);

        this.ApiContext.PlayerFortBuilds
            .Should()
            .Contain(
                x => x.PlantId == FortPlants.TheHalidom && x.DeviceAccountId == DeviceAccountId
            );

        this.GetSavefileVersion().Should().Be(this.MaxVersion);

        await this.ApiContext.PlayerStoryState.ExecuteDeleteAsync();
    }

    [Fact]
    public async Task V1Update_NoSmithy_StoryComplete_Adds()
    {
        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                DeviceAccountId = DeviceAccountId,
                StoryType = StoryTypes.Quest,
                State = StoryState.Read,
                StoryId = TutorialService.TutorialStoryIds.Smithy
            }
        );

        LoadIndexData data = (
            await this.Client.PostMsgpack<LoadIndexData>("/load/index", new LoadIndexRequest())
        ).data;

        data.build_list.Should().Contain(x => x.plant_id == FortPlants.Smithy);

        this.ApiContext.PlayerFortBuilds
            .Should()
            .Contain(x => x.PlantId == FortPlants.Smithy && x.DeviceAccountId == DeviceAccountId);

        this.GetSavefileVersion().Should().Be(this.MaxVersion);

        await this.ApiContext.PlayerStoryState.ExecuteDeleteAsync();
    }

    [Fact]
    public async Task V1Update_NoDracoliths_StoryComplete_Adds()
    {
        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                DeviceAccountId = DeviceAccountId,
                StoryType = StoryTypes.Quest,
                State = StoryState.Read,
                StoryId = 1000808
            }
        );

        LoadIndexData data = (
            await this.Client.PostMsgpack<LoadIndexData>("/load/index", new LoadIndexRequest())
        ).data;

        data.build_list.Should().Contain(x => x.plant_id == FortPlants.FlameDracolith);

        this.ApiContext.PlayerFortBuilds
            .Should()
            .Contain(x => x.PlantId == FortPlants.FlameDracolith);

        this.GetSavefileVersion().Should().Be(this.MaxVersion);

        await this.ApiContext.PlayerStoryState.ExecuteDeleteAsync();
    }

    [Fact]
    public async Task V1Update_NoDojos_TutorialComplete_Adds()
    {
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            u => u.SetProperty(e => e.TutorialStatus, TutorialService.TutorialStatusIds.Dojos)
        );

        LoadIndexData data = (
            await this.Client.PostMsgpack<LoadIndexData>("/load/index", new LoadIndexRequest())
        ).data;

        data.build_list.Should().Contain(x => x.plant_id == FortPlants.SwordDojo);

        this.ApiContext.PlayerFortBuilds.Should().Contain(x => x.PlantId == FortPlants.SwordDojo);

        this.GetSavefileVersion().Should().Be(this.MaxVersion);

        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            u => u.SetProperty(e => e.TutorialStatus, 0)
        );
    }

    [Fact]
    public async Task V1Update_StoryAndTutorialIncomplete_DoesNothing()
    {
        LoadIndexData data = (
            await this.Client.PostMsgpack<LoadIndexData>("/load/index", new LoadIndexRequest())
        ).data;

        data.build_list.Should().BeEmpty();
        this.ApiContext.PlayerFortBuilds.Should().BeEmpty();

        this.GetSavefileVersion().Should().Be(this.MaxVersion);
    }
}
