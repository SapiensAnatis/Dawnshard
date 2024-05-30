using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Quest;

public class QuestReadStoryTest : TestFixture
{
    public QuestReadStoryTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task ReadStory_ReturnCorrectResponse()
    {
        QuestReadStoryResponse response = (
            await this.Client.PostMsgpack<QuestReadStoryResponse>(
                "/quest/read_story",
                new QuestReadStoryRequest() { QuestStoryId = 1000106 }
            )
        ).Data;

        response.UpdateDataList.UserData.Should().NotBeNull();
        response
            .UpdateDataList.CharaList.Should()
            .ContainSingle()
            .And.Subject.Any(x => x.CharaId == Charas.Ranzal)
            .Should()
            .BeTrue();

        response
            .UpdateDataList.QuestStoryList.Should()
            .ContainEquivalentOf(
                new QuestStoryList() { QuestStoryId = 1000106, State = StoryState.Read }
            );
    }

    [Fact]
    public async Task ReadStory_GrantsReward_InventoryFull_SendsToGiftBox()
    {
        int midgardStoryId = 1000109;
        this.ApiContext.PlayerUserData.ExecuteUpdate(x =>
            x.SetProperty(e => e.MaxDragonQuantity, 0)
        );

        QuestReadStoryResponse response = (
            await this.Client.PostMsgpack<QuestReadStoryResponse>(
                "/quest/read_story",
                new QuestReadStoryRequest() { QuestStoryId = midgardStoryId }
            )
        ).Data;

        response.UpdateDataList.UserData.Should().NotBeNull();

        // Tutorial hack - only for Midgardsormr
        response
            .UpdateDataList.DragonReliabilityList.Should()
            .ContainSingle()
            .Which.DragonId.Should()
            .Be(Dragons.Midgardsormr);

        response
            .EntityResult.OverPresentEntityList.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new AtgenBuildEventRewardEntityList()
                {
                    EntityType = EntityTypes.Dragon,
                    EntityId = (int)Dragons.Midgardsormr,
                    EntityQuantity = 1,
                }
            );

        response
            .UpdateDataList.QuestStoryList.Should()
            .ContainEquivalentOf(
                new QuestStoryList() { QuestStoryId = midgardStoryId, State = StoryState.Read }
            );
    }

    [Fact]
    public async Task ReadStory_Midgardsormr_DoesNotAddReliabilityIfOwned()
    {
        int midgardStoryId = 1000109;
        this.ApiContext.PlayerUserData.ExecuteUpdate(x =>
            x.SetProperty(e => e.MaxDragonQuantity, 0)
        );

        await this.AddToDatabase(
            new DbPlayerDragonReliability() { DragonId = Dragons.Midgardsormr }
        );

        QuestReadStoryResponse response = (
            await this.Client.PostMsgpack<QuestReadStoryResponse>(
                "/quest/read_story",
                new QuestReadStoryRequest() { QuestStoryId = midgardStoryId }
            )
        ).Data;

        response.UpdateDataList.UserData.Should().NotBeNull();

        response.UpdateDataList.DragonReliabilityList.Should().BeNull();

        response
            .EntityResult.OverPresentEntityList.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new AtgenBuildEventRewardEntityList()
                {
                    EntityType = EntityTypes.Dragon,
                    EntityId = (int)Dragons.Midgardsormr,
                    EntityQuantity = 1,
                }
            );

        response
            .UpdateDataList.QuestStoryList.Should()
            .ContainEquivalentOf(
                new QuestStoryList() { QuestStoryId = midgardStoryId, State = StoryState.Read }
            );
    }

    [Fact]
    public async Task ReadStory_UpdatesDatabase()
    {
        QuestReadStoryResponse response = (
            await this.Client.PostMsgpack<QuestReadStoryResponse>(
                "/quest/read_story",
                new QuestReadStoryRequest() { QuestStoryId = 1001410 }
            )
        ).Data;

        this.ApiContext.PlayerStoryState.First(x =>
                x.ViewerId == this.ViewerId && x.StoryId == 1001410
            )
            .State.Should()
            .Be(StoryState.Read);

        List<DbPlayerStoryState> storyStates = await this
            .ApiContext.PlayerStoryState.Where(x => x.ViewerId == this.ViewerId)
            .ToListAsync();

        storyStates.Should().Contain(x => x.StoryId == 1001410 && x.State == StoryState.Read);
        this.ApiContext.PlayerCharaData.Any(x => x.CharaId == Charas.Zena).Should().BeTrue();
    }

    [Fact]
    public async Task ReadStory_TheLonePaladyn_SetsTutorialStatus()
    {
        /* https://github.com/SapiensAnatis/Dawnshard/issues/533 */

        int theLonePaladynStoryId = 1000103;

        QuestReadStoryResponse response = (
            await this.Client.PostMsgpack<QuestReadStoryResponse>(
                "/quest/read_story",
                new QuestReadStoryRequest() { QuestStoryId = theLonePaladynStoryId }
            )
        ).Data;

        response
            .UpdateDataList.QuestStoryList.Should()
            .ContainEquivalentOf(
                new QuestStoryList()
                {
                    QuestStoryId = theLonePaladynStoryId,
                    State = StoryState.Read
                }
            );

        response.UpdateDataList.UserData.TutorialStatus.Should().Be(10600);
    }

    [Fact]
    public async Task ReadStory_Chapter10Completion_GrantsRewards()
    {
        await this
            .ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdateAsync(u =>
                u.SetProperty(e => e.Level, 30).SetProperty(e => e.Exp, 18990)
            );

        StoryReadResponse data = (
            await this.Client.PostMsgpack<StoryReadResponse>(
                "/quest/read_story",
                new QuestReadStoryRequest() { QuestStoryId = 1001009 }
            )
        ).Data;

        data.UpdateDataList.UserData.Should().NotBeNull();
        data.UpdateDataList.QuestStoryList.Should()
            .BeEquivalentTo(
                new List<QuestStoryList>()
                {
                    new() { QuestStoryId = 1001009, State = StoryState.Read }
                }
            );
        data.UpdateDataList.UserData.Exp.Should().BeGreaterThanOrEqualTo(88980);
        data.UpdateDataList.UserData.Level.Should().BeGreaterThanOrEqualTo(65);
        this.ApiContext.PlayerPresents.Where(x =>
                x.ViewerId == this.ViewerId && x.EntityType == EntityTypes.HustleHammer
            )
            .First()
            .EntityQuantity.Should()
            .Be(350);
    }

    [Theory]
    [InlineData(2044303, Charas.Harle)]
    [InlineData(2046203, Charas.Origa)]
    [InlineData(2042704, Charas.Audric)]
    public async Task ReadCompendiumStory_GrantsCharacter(int storyId, Charas expectedChara)
    {
        QuestReadStoryResponse response = (
            await this.Client.PostMsgpack<QuestReadStoryResponse>(
                "/quest/read_story",
                new QuestReadStoryRequest() { QuestStoryId = storyId }
            )
        ).Data;

        response.UpdateDataList.CharaList.Should().Contain(x => x.CharaId == expectedChara);
    }
}
