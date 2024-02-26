using DragaliaAPI.Database.Entities;
using DragaliaAPI.Services.Game;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V1UpdateTest : SavefileUpdateTestFixture
{
    public V1UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V1Update_NoFort_StoryComplete_Adds()
    {
        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                ViewerId = ViewerId,
                StoryType = StoryTypes.Quest,
                State = StoryState.Read,
                StoryId = TutorialService.TutorialStoryIds.Halidom
            }
        );

        LoadIndexResponse data = (
            await this.Client.PostMsgpack<LoadIndexResponse>("/load/index")
        ).Data;

        data.BuildList.Should().Contain(x => x.PlantId == FortPlants.TheHalidom);

        this.ApiContext.PlayerFortBuilds.Should()
            .Contain(x => x.PlantId == FortPlants.TheHalidom && x.ViewerId == ViewerId);

        this.GetSavefileVersion().Should().Be(this.MaxVersion);
    }

    [Fact]
    public async Task V1Update_NoSmithy_StoryComplete_Adds()
    {
        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                ViewerId = ViewerId,
                StoryType = StoryTypes.Quest,
                State = StoryState.Read,
                StoryId = TutorialService.TutorialStoryIds.Smithy
            }
        );

        LoadIndexResponse data = (
            await this.Client.PostMsgpack<LoadIndexResponse>("/load/index")
        ).Data;

        data.BuildList.Should().Contain(x => x.PlantId == FortPlants.Smithy);

        this.ApiContext.PlayerFortBuilds.Should()
            .Contain(x => x.PlantId == FortPlants.Smithy && x.ViewerId == ViewerId);

        this.GetSavefileVersion().Should().Be(this.MaxVersion);
    }

    [Fact]
    public async Task V1Update_NoDracoliths_StoryComplete_Adds()
    {
        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                ViewerId = ViewerId,
                StoryType = StoryTypes.Quest,
                State = StoryState.Read,
                StoryId = 1000808
            }
        );

        LoadIndexResponse data = (
            await this.Client.PostMsgpack<LoadIndexResponse>("/load/index")
        ).Data;

        data.BuildList.Should().Contain(x => x.PlantId == FortPlants.FlameDracolith);

        this.ApiContext.PlayerFortBuilds.Should()
            .Contain(x => x.PlantId == FortPlants.FlameDracolith);

        this.GetSavefileVersion().Should().Be(this.MaxVersion);
    }

    [Fact]
    public async Task V1Update_NoDojos_TutorialComplete_Adds()
    {
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(u =>
            u.SetProperty(e => e.TutorialStatus, TutorialService.TutorialStatusIds.Dojos)
        );

        LoadIndexResponse data = (
            await this.Client.PostMsgpack<LoadIndexResponse>("/load/index")
        ).Data;

        data.BuildList.Should().Contain(x => x.PlantId == FortPlants.SwordDojo);

        this.ApiContext.PlayerFortBuilds.Should().Contain(x => x.PlantId == FortPlants.SwordDojo);

        this.GetSavefileVersion().Should().Be(this.MaxVersion);

        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(u =>
            u.SetProperty(e => e.TutorialStatus, 0)
        );
    }

    [Fact]
    public async Task V1Update_StoryAndTutorialIncomplete_DoesNothing()
    {
        await this.ApiContext.PlayerFortBuilds.ExecuteDeleteAsync();
        await this.ApiContext.PlayerStoryState.ExecuteDeleteAsync();

        LoadIndexResponse data = (
            await this.Client.PostMsgpack<LoadIndexResponse>("/load/index")
        ).Data;

        data.BuildList.Should().BeEmpty();
        this.ApiContext.PlayerFortBuilds.Should().BeEmpty();

        this.GetSavefileVersion().Should().Be(this.MaxVersion);
    }
}
