using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class QuestRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IQuestRepository questRepository;

    private readonly Mock<IPlayerDetailsService> mockPlayerDetailsService;

    public QuestRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;

        this.mockPlayerDetailsService = new(MockBehavior.Strict);

        this.questRepository = new QuestRepository(
            fixture.ApiContext,
            this.mockPlayerDetailsService.Object
        );

        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task Quests_FiltersByAccountId()
    {
        this.mockPlayerDetailsService.SetupGet(x => x.AccountId).Returns("id");

        await this.fixture.AddRangeToDatabase(
            new List<DbQuest>()
            {
                new() { DeviceAccountId = "id", QuestId = 1 },
                new() { DeviceAccountId = "other id", QuestId = 2 }
            }
        );

        this.questRepository.Quests
            .Should()
            .BeEquivalentTo(
                new List<DbQuest>()
                {
                    new() { DeviceAccountId = "id", QuestId = 1 },
                }
            )
            .And.BeEquivalentTo(this.questRepository.GetQuests("id"));
    }

    [Fact]
    public async Task CompleteQuest_CompletesQuest()
    {
        DbQuest quest = await this.questRepository.CompleteQuest("id", 3, 1.0f);

        quest
            .Should()
            .BeEquivalentTo(
                new DbQuest()
                {
                    DeviceAccountId = "id",
                    QuestId = 3,
                    State = 3,
                    IsMissionClear1 = true,
                    IsMissionClear2 = true,
                    IsMissionClear3 = true,
                    PlayCount = 1,
                    DailyPlayCount = 1,
                    WeeklyPlayCount = 1,
                    IsAppear = true,
                    BestClearTime = 1.0f,
                }
            );
    }
}
