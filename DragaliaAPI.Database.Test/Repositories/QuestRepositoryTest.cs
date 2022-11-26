using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

public class QuestRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IQuestRepository questRepository;

    public QuestRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.questRepository = new QuestRepository(fixture.ApiContext);
    }

    [Fact]
    public async Task GetQuestStoryList_ReturnsOnlyPlayerQuestStoryData()
    {
        DbPlayerStoryState playerStoryState =
            new()
            {
                DeviceAccountId = DeviceAccountId,
                State = 1,
                StoryId = 2,
                StoryType = StoryTypes.Quest
            };

        await this.fixture.AddRangeToDatabase(
            new List<DbPlayerStoryState>()
            {
                playerStoryState,
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    State = 2,
                    StoryId = 10,
                    StoryType = StoryTypes.Castle
                },
                new()
                {
                    DeviceAccountId = "id 2",
                    State = 2,
                    StoryId = 10,
                    StoryType = StoryTypes.Quest
                }
            }
        );

        (await this.questRepository.GetQuestStoryList(DeviceAccountId).ToListAsync())
            .Should()
            .AllSatisfy(x => x.DeviceAccountId.Should().Be(DeviceAccountId));
    }

    [Fact]
    public async Task UpdateQuestStory_NewStory_UpdatesDatabase()
    {
        await this.questRepository.UpdateQuestStory(DeviceAccountId, 1, 2);
        await this.questRepository.SaveChangesAsync();

        this.fixture.ApiContext.PlayerStoryState
            .Single(x => x.DeviceAccountId == DeviceAccountId && x.StoryId == 1)
            .Should()
            .BeEquivalentTo(
                new DbPlayerStoryState()
                {
                    DeviceAccountId = DeviceAccountId,
                    StoryId = 1,
                    State = 2,
                    StoryType = StoryTypes.Quest
                }
            );
    }

    [Fact]
    public async Task UpdateQuestStory_ExistingStory_UpdatesDatabase()
    {
        await this.fixture.AddToDatabase(
            new DbPlayerStoryState()
            {
                DeviceAccountId = DeviceAccountId,
                StoryId = 3,
                State = 0,
                StoryType = StoryTypes.Quest
            }
        );

        await this.questRepository.UpdateQuestStory(DeviceAccountId, 3, 2);
        await this.questRepository.SaveChangesAsync();

        this.fixture.ApiContext.PlayerStoryState
            .Single(x => x.DeviceAccountId == DeviceAccountId && x.StoryId == 3)
            .Should()
            .BeEquivalentTo(
                new DbPlayerStoryState()
                {
                    DeviceAccountId = DeviceAccountId,
                    StoryId = 3,
                    State = 2,
                    StoryType = StoryTypes.Quest
                }
            );
    }
}
