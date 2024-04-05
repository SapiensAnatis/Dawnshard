using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V20UpdateTest : SavefileUpdateTestFixture
{
    private const int HarleStoryId = 2044303;
    private const int OrigaStoryId = 2046203;

    protected override int StartingVersion => 19;

    public V20UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V20Update_StoriesCompleted_AddsMissingCompendiumCharacters()
    {
        await this.AddRangeToDatabase(
            [
                new DbPlayerStoryState()
                {
                    ViewerId = this.ViewerId,
                    StoryId = HarleStoryId,
                    StoryType = StoryTypes.Quest,
                    State = StoryState.Read
                },
                new DbPlayerStoryState()
                {
                    ViewerId = this.ViewerId,
                    StoryId = OrigaStoryId,
                    StoryType = StoryTypes.Quest,
                    State = StoryState.Read
                }
            ]
        );

        await this.LoadIndex();

        this.ApiContext.PlayerPresents.AsNoTracking()
            .Should()
            .Contain(x => x.ViewerId == this.ViewerId && x.EntityId == (int)Charas.Harle);
        this.ApiContext.PlayerPresents.AsNoTracking()
            .Should()
            .Contain(x => x.ViewerId == this.ViewerId && x.EntityId == (int)Charas.Origa);
    }

    [Fact]
    public async Task V20Update_StoriesNotCompleted_DoesNotAddCharacters()
    {
        this.ApiContext.PlayerStoryState.AddRange(
            [
                new DbPlayerStoryState()
                {
                    Owner = new DbPlayer() { AccountId = "other player" },
                    StoryId = HarleStoryId,
                    StoryType = StoryTypes.Quest,
                    State = StoryState.Read
                },
                new DbPlayerStoryState()
                {
                    ViewerId = this.ViewerId,
                    StoryId = OrigaStoryId,
                    StoryType = StoryTypes.Quest,
                    State = StoryState.Unlocked
                }
            ]
        );
        await this.ApiContext.SaveChangesAsync();
        await this.LoadIndex();

        this.ApiContext.PlayerPresents.AsNoTracking()
            .Should()
            .NotContain(x => x.ViewerId == this.ViewerId && x.EntityId == (int)Charas.Harle);
        this.ApiContext.PlayerPresents.AsNoTracking()
            .Should()
            .NotContain(x => x.ViewerId == this.ViewerId && x.EntityId == (int)Charas.Origa);
    }

    [Fact]
    public async Task V20Update_StoriesCompleted_CharactersOwned_DoesNotAddCharacters()
    {
        await this.AddRangeToDatabase(
            [
                new DbPlayerStoryState()
                {
                    ViewerId = this.ViewerId,
                    StoryId = HarleStoryId,
                    StoryType = StoryTypes.Quest,
                    State = StoryState.Read
                },
                new DbPlayerStoryState()
                {
                    ViewerId = this.ViewerId,
                    StoryId = OrigaStoryId,
                    StoryType = StoryTypes.Quest,
                    State = StoryState.Read
                },
                new DbPlayerCharaData(this.ViewerId, Charas.Harle),
                new DbPlayerCharaData(this.ViewerId, Charas.Origa),
            ]
        );

        await this.LoadIndex();

        this.ApiContext.PlayerPresents.AsNoTracking()
            .Should()
            .NotContain(x => x.ViewerId == this.ViewerId && x.EntityId == (int)Charas.Harle);
        this.ApiContext.PlayerPresents.AsNoTracking()
            .Should()
            .NotContain(x => x.ViewerId == this.ViewerId && x.EntityId == (int)Charas.Origa);
    }
}
