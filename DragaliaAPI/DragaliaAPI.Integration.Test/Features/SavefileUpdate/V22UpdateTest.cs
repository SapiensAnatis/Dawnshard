using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V22UpdateTest : SavefileUpdateTestFixture
{
    protected override int StartingVersion => 21;
    private const int Chapter10LastStoryId = 1001009;

    public V22UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V22Update_Chapter10Completed_GrantsRewards()
    {
        await this
            .ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdateAsync(u =>
                u.SetProperty(e => e.Level, 30).SetProperty(e => e.Exp, 18990)
            );

        await this
            .ApiContext.PlayerPresents.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteDeleteAsync();

        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                ViewerId = this.ViewerId,
                StoryId = Chapter10LastStoryId,
                StoryType = StoryTypes.Quest,
                State = StoryState.Read
            }
        );

        await this.LoadIndex();

        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.FirstAsync(x =>
            x.ViewerId == this.ViewerId
        );

        userData.Level.Should().Be(65);
        userData.Exp.Should().Be(88980);

        List<DbPlayerPresent> presentData = await this
            .ApiContext.PlayerPresents.Where(x => x.ViewerId == this.ViewerId)
            .ToListAsync();

        List<QuestStoryReward> rewards = MasterAsset
            .QuestStoryRewardInfo.Enumerable.Where(x => x.Id == Chapter10LastStoryId)
            .First()
            .Rewards.Where(x => x.Type is EntityTypes.Material or EntityTypes.HustleHammer)
            .ToList();

        presentData.Count.Should().Be(rewards.Count);

        foreach (QuestStoryReward reward in rewards)
        {
            DbPlayerPresent present = presentData.First(x => x.EntityType == reward.Type);
            present.EntityQuantity.Should().Be(present.EntityQuantity);
        }
    }

    [Fact]
    public async Task V22Update_Chapter10NotCompleted_DoesNotGrantRewards()
    {
        await this
            .ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdateAsync(u =>
                u.SetProperty(e => e.Level, 30).SetProperty(e => e.Exp, 18990)
            );

        await this
            .ApiContext.PlayerStoryState.Where(x =>
                x.ViewerId == this.ViewerId && x.StoryId == Chapter10LastStoryId
            )
            .ExecuteDeleteAsync();

        await this
            .ApiContext.PlayerPresents.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteDeleteAsync();

        await this.LoadIndex();

        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.FirstAsync(x =>
            x.ViewerId == this.ViewerId
        );

        userData.Level.Should().Be(30);
        userData.Exp.Should().Be(18990);

        List<DbPlayerPresent> presentData = await this
            .ApiContext.PlayerPresents.Where(x => x.ViewerId == this.ViewerId)
            .ToListAsync();

        presentData.Count.Should().Be(0);
    }
}
