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
        QuestReadStoryData response = (
            await this.Client.PostMsgpack<QuestReadStoryData>(
                "/quest/read_story",
                new QuestReadStoryRequest() { quest_story_id = 1000106 }
            )
        ).data;

        response.update_data_list.user_data.Should().NotBeNull();
        response
            .update_data_list.chara_list.Should()
            .ContainSingle()
            .And.Subject.Any(x => x.chara_id == Charas.Ranzal)
            .Should()
            .BeTrue();

        response
            .update_data_list.quest_story_list.Should()
            .ContainEquivalentOf(new QuestStoryList() { quest_story_id = 1000106, state = 1 });
    }

    [Fact]
    public async Task ReadStory_UpdatesDatabase()
    {
        QuestReadStoryData response = (
            await this.Client.PostMsgpack<QuestReadStoryData>(
                "/quest/read_story",
                new QuestReadStoryRequest() { quest_story_id = 1001410 }
            )
        ).data;

        this.ApiContext.PlayerStoryState.First(x => x.ViewerId == ViewerId && x.StoryId == 1001410)
            .State.Should()
            .Be(StoryState.Read);

        List<DbPlayerStoryState> storyStates = await this.ApiContext.PlayerStoryState.Where(
            x => x.ViewerId == ViewerId
        )
            .ToListAsync();

        storyStates.Should().Contain(x => x.StoryId == 1001410 && x.State == StoryState.Read);
        this.ApiContext.PlayerCharaData.Any(x => x.CharaId == Charas.Zena).Should().BeTrue();
    }
}
