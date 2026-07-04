using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V28UpdateTest : SavefileUpdateTestFixture
{
    protected override int StartingVersion => 27;

    public V28UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V28Update_UnlocksBothStoriesForDragonAtLevel30()
    {
        this.ApiContext.PlayerStoryState.Count(x => x.StoryType == StoryTypes.Dragon)
            .Should()
            .Be(0);

        await this.AddRangeToDatabase([
            new DbPlayerDragonReliability() { DragonId = DragonId.Arsene, Level = 30 },
        ]);

        StoryData arseneStories = MasterAsset.DragonStories[(int)DragonId.Arsene];

        this.ApiContext.ChangeTracker.Clear();

        await this.LoadIndex();

        List<DbPlayerStoryState> dragonStories = ApiContext
            .PlayerStoryState.Where(x => x.StoryType == StoryTypes.Dragon)
            .ToList();

        dragonStories
            .Should()
            .Contain(x => x.StoryId == arseneStories.StoryIds[0])
            .Which.Should()
            .BeEquivalentTo(
                new DbPlayerStoryState()
                {
                    ViewerId = this.ViewerId,
                    StoryId = arseneStories.StoryIds[0],
                    StoryType = StoryTypes.Dragon,
                    State = StoryState.Unlocked,
                }
            );

        dragonStories
            .Should()
            .Contain(x => x.StoryId == arseneStories.StoryIds[1])
            .Which.Should()
            .BeEquivalentTo(
                new DbPlayerStoryState()
                {
                    ViewerId = this.ViewerId,
                    StoryId = arseneStories.StoryIds[1],
                    StoryType = StoryTypes.Dragon,
                    State = StoryState.Unlocked,
                }
            );
    }

    [Fact]
    public async Task V28Update_DoesNotDuplicateExistingStories()
    {
        await this.AddRangeToDatabase([
            new DbPlayerDragonReliability() { DragonId = DragonId.Arsene, Level = 30 },
        ]);

        StoryData arseneStories = MasterAsset.DragonStories[(int)DragonId.Arsene];

        await this.AddRangeToDatabase([
            new DbPlayerStoryState()
            {
                StoryId = arseneStories.StoryIds[0],
                StoryType = StoryTypes.Dragon,
            },
            new DbPlayerStoryState()
            {
                StoryId = arseneStories.StoryIds[1],
                StoryType = StoryTypes.Dragon,
            },
        ]);

        this.ApiContext.ChangeTracker.Clear();

        await this.LoadIndex();

        ApiContext
            .PlayerStoryState.Where(x =>
                x.StoryType == StoryTypes.Dragon
                && (
                    x.StoryId == arseneStories.StoryIds[0] || x.StoryId == arseneStories.StoryIds[1]
                )
            )
            .Should()
            .HaveCount(2);
    }
}
