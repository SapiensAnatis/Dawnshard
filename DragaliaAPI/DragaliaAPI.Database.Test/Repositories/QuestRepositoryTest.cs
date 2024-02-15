using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Test.Utils;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class QuestRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IQuestRepository questRepository;

    public QuestRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;

        this.questRepository = new QuestRepository(
            fixture.ApiContext,
            IdentityTestUtils.MockPlayerDetailsService.Object
        );

        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task Quests_FiltersByAccountId()
    {
        await this.fixture.AddRangeToDatabase(
            new List<DbQuest>()
            {
                new() { ViewerId = ViewerId, QuestId = 1 },
                new() { ViewerId = ViewerId + 1, QuestId = 2 }
            }
        );

        this.questRepository.Quests.Should()
            .BeEquivalentTo(
                new List<DbQuest>()
                {
                    new() { ViewerId = ViewerId, QuestId = 1 },
                }
            )
            .And.BeEquivalentTo(this.questRepository.Quests);
    }
}
