using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Dragalia;

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
    public async Task ReadStory_UpdatesDatabase()
    {
        QuestReadStoryResponse response = (
            await this.Client.PostMsgpack<QuestReadStoryResponse>(
                "/quest/read_story",
                new QuestReadStoryRequest() { QuestStoryId = 1001410 }
            )
        ).Data;

        this.ApiContext.PlayerStoryState.First(x => x.ViewerId == ViewerId && x.StoryId == 1001410)
            .State.Should()
            .Be(StoryState.Read);

        List<DbPlayerStoryState> storyStates = await this
            .ApiContext.PlayerStoryState.Where(x => x.ViewerId == ViewerId)
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
