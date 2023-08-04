using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Features.Dungeon.Record;

public class DungeonRecordRewardServiceTest
{
    private readonly Mock<IQuestCompletionService> mockQuestCompletionService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IAbilityCrestMultiplierService> mockAbilityCrestMultiplierService;
    private readonly Mock<IEventDropService> mockEventDropService;
    private readonly Mock<ILogger<DungeonRecordRewardService>> mockLogger;

    private readonly IDungeonRecordRewardService dungeonRecordRewardService;

    public DungeonRecordRewardServiceTest()
    {
        this.mockQuestCompletionService = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Strict);
        this.mockAbilityCrestMultiplierService = new(MockBehavior.Strict);
        this.mockEventDropService = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.dungeonRecordRewardService = new DungeonRecordRewardService(
            this.mockQuestCompletionService.Object,
            this.mockRewardService.Object,
            this.mockAbilityCrestMultiplierService.Object,
            this.mockEventDropService.Object,
            this.mockLogger.Object
        );
    }

    [Fact]
    public async Task ProcessQuestMissionCompletion_SetsEntityProperties()
    {
        int questId = 225021101;
        DbQuest questEntity =
            new()
            {
                DeviceAccountId = "id",
                QuestId = questId,
                PlayCount = 0,
                IsMissionClear1 = false,
                IsMissionClear2 = false,
                IsMissionClear3 = false,
            };

        List<AtgenFirstClearSet> firstClearRewards =
            new()
            {
                new()
                {
                    id = 0,
                    quantity = 2,
                    type = EntityTypes.Wyrmite
                }
            };

        PlayRecord playRecord = new();
        DungeonSession session =
            new() { QuestData = MasterAsset.QuestData[questId], Party = null! };
        QuestMissionStatus status =
            new(
                new[] { true, true, true },
                new List<AtgenMissionsClearSet>(),
                new List<AtgenFirstClearSet>()
            );

        this.mockQuestCompletionService
            .Setup(x => x.CompleteQuestMissions(session, new[] { false, false, false }, playRecord))
            .ReturnsAsync(status);
        this.mockQuestCompletionService
            .Setup(x => x.GrantFirstClearRewards(questId))
            .ReturnsAsync(firstClearRewards);

        (
            await this.dungeonRecordRewardService.ProcessQuestMissionCompletion(
                playRecord,
                session,
                questEntity
            )
        )
            .Should()
            .Be(status);

        questEntity.IsMissionClear1.Should().BeTrue();
        questEntity.IsMissionClear2.Should().BeTrue();
        questEntity.IsMissionClear3.Should().BeTrue();
    }
}
