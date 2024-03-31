using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V14UpdateTest : SavefileUpdateTestFixture
{
    public V14UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V14Update_DoesNotConflictWithV10Update()
    {
        await this
            .ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdateAsync(entity =>
                entity.SetProperty(e => e.EmblemId, Emblems.HotBloodedInstructor)
            );

        int cellieraCh5 = MasterAsset.CharaStories[(int)Charas.Celliera].StoryIds[^1];

        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                StoryId = cellieraCh5,
                StoryType = StoryTypes.Chara,
                State = StoryState.Read
            }
        );

        await this.Invoking(x => x.LoadIndex()).Should().NotThrowAsync();

        this.GetSavefileVersion().Should().Be(this.MaxVersion);
    }
}
