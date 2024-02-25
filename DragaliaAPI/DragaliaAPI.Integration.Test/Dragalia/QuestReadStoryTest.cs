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
        ).data;

        response.UpdateDataList.UserData.Should().NotBeNull();
        response
            .UpdateDataList.CharaList.Should()
            .ContainSingle()
            .And.Subject.Any(x => x.CharaId == Charas.Ranzal)
            .Should()
            .BeTrue();

        response
            .UpdateDataList.QuestStoryList.Should()
            .ContainEquivalentOf(new QuestStoryList() { QuestStoryId = 1000106, State = 1 });
    }

    [Fact]
    public async Task ReadStory_UpdatesDatabase()
    {
        QuestReadStoryResponse response = (
            await this.Client.PostMsgpack<QuestReadStoryResponse>(
                "/quest/read_story",
                new QuestReadStoryRequest() { QuestStoryId = 1001410 }
            )
        ).data;

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
        ).data;

        response
            .UpdateDataList.QuestStoryList.Should()
            .ContainEquivalentOf(
                new QuestStoryList() { QuestStoryId = theLonePaladynStoryId, State = 1 }
            );

        response.UpdateDataList.UserData.TutorialStatus.Should().Be(10600);
    }
}
