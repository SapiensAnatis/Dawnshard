using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V25UpdateTest : SavefileUpdateTestFixture
{
    protected override int StartingVersion => 24;

    public V25UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V25Update_AddsCorrectMissingDragonStories()
    {
        await this.AddRangeToDatabase([
            new DbPlayerDragonReliability() { DragonId = DragonId.Marishiten, Level = 15 },
            new DbPlayerDragonReliability() { DragonId = DragonId.Arctos, Level = 13 },
            new DbPlayerDragonReliability() { DragonId = DragonId.Shinobi, Level = 17 },
        ]);

        StoryData marishitenStories = MasterAsset.DragonStories[(int)DragonId.Marishiten];
        StoryData arctosStories = MasterAsset.DragonStories[(int)DragonId.Arctos];
        StoryData shinobiStories = MasterAsset.DragonStories[(int)DragonId.Shinobi];

        await this.AddRangeToDatabase([
            new DbPlayerStoryState()
            {
                StoryId = marishitenStories.StoryIds[0],
                StoryType = StoryTypes.Dragon,
            },
            new DbPlayerStoryState()
            {
                StoryId = marishitenStories.StoryIds[1],
                StoryType = StoryTypes.Dragon,
            },
            new DbPlayerStoryState()
            {
                StoryId = arctosStories.StoryIds[0],
                StoryType = StoryTypes.Dragon,
            },
            new DbPlayerStoryState()
            {
                StoryId = shinobiStories.StoryIds[0], // missing second Shinobi story
                StoryType = StoryTypes.Dragon,
            },
        ]);

        this.ApiContext.ChangeTracker.Clear();

        await this.LoadIndex();

        List<DbPlayerStoryState> dragonStories = ApiContext
            .PlayerStoryState.Where(x => x.StoryType == StoryTypes.Dragon)
            .ToList();

        dragonStories
            .Should()
            .Contain(x => x.StoryId == shinobiStories.StoryIds[1])
            .Which.Should()
            .BeEquivalentTo(
                new DbPlayerStoryState()
                {
                    ViewerId = this.ViewerId,
                    StoryId = shinobiStories.StoryIds[1],
                    StoryType = StoryTypes.Dragon,
                    State = StoryState.Unlocked,
                }
            );

        dragonStories.Should().NotContain(x => x.StoryId == arctosStories.StoryIds[1]);
    }
}
