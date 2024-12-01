using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Tutorial;
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
                StoryId = TutorialService.TutorialStoryIds.Halidom,
            }
        );

        LoadIndexResponse data = (
            await this.Client.PostMsgpack<LoadIndexResponse>(
                "/load/index",
                cancellationToken: TestContext.Current.CancellationToken
            )
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
                StoryId = TutorialService.TutorialStoryIds.Smithy,
            }
        );

        LoadIndexResponse data = (
            await this.Client.PostMsgpack<LoadIndexResponse>(
                "/load/index",
                cancellationToken: TestContext.Current.CancellationToken
            )
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
                StoryId = 1000808,
            }
        );

        LoadIndexResponse data = (
            await this.Client.PostMsgpack<LoadIndexResponse>(
                "/load/index",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        data.BuildList.Should().Contain(x => x.PlantId == FortPlants.FlameDracolith);

        this.ApiContext.PlayerFortBuilds.Should()
            .Contain(x => x.PlantId == FortPlants.FlameDracolith);

        this.GetSavefileVersion().Should().Be(this.MaxVersion);
    }

    [Fact]
    public async Task V1Update_NoDojos_TutorialComplete_Adds()
    {
        await this.ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId).ExecuteUpdateAsync(
            u => u.SetProperty(e => e.TutorialStatus, TutorialService.TutorialStatusIds.Dojos),
            cancellationToken: TestContext.Current.CancellationToken
        );

        LoadIndexResponse data = (
            await this.Client.PostMsgpack<LoadIndexResponse>(
                "/load/index",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        data.BuildList.Should().Contain(x => x.PlantId == FortPlants.SwordDojo);

        this.ApiContext.PlayerFortBuilds.Should().Contain(x => x.PlantId == FortPlants.SwordDojo);

        this.GetSavefileVersion().Should().Be(this.MaxVersion);

        await this.ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId).ExecuteUpdateAsync(
            u => u.SetProperty(e => e.TutorialStatus, 0),
            cancellationToken: TestContext.Current.CancellationToken
        );
    }

    [Fact]
    public async Task V1Update_StoryAndTutorialIncomplete_DoesNothing()
    {
        await this.ApiContext.PlayerFortBuilds.ExecuteDeleteAsync(
            cancellationToken: TestContext.Current.CancellationToken
        );
        await this.ApiContext.PlayerStoryState.ExecuteDeleteAsync(
            cancellationToken: TestContext.Current.CancellationToken
        );

        LoadIndexResponse data = (
            await this.Client.PostMsgpack<LoadIndexResponse>(
                "/load/index",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        data.BuildList.Should().BeEmpty();
        this.ApiContext.PlayerFortBuilds.Should().BeEmpty();

        this.GetSavefileVersion().Should().Be(this.MaxVersion);
    }
}
