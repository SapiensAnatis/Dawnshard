using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

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
            .Be((status, firstClearRewards));

        questEntity.IsMissionClear1.Should().BeTrue();
        questEntity.IsMissionClear2.Should().BeTrue();
        questEntity.IsMissionClear3.Should().BeTrue();
    }

    [Fact]
    public async Task ProcessEnemyDrops_RewardsCorrectDrops()
    {
        PlayRecord playRecord =
            new()
            {
                treasure_record = new List<AtgenTreasureRecord>()
                {
                    new()
                    {
                        area_idx = 0,
                        enemy = new List<int>() { 1, 0, 1 }
                    },
                    new()
                    {
                        area_idx = 1,
                        enemy = new List<int>() { 0, 1, 0 }
                    }
                }
            };

        DungeonSession session =
            new()
            {
                QuestData = null!,
                Party = null!,
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    {
                        0,
                        new List<AtgenEnemy>()
                        {
                            new()
                            {
                                enemy_drop_list = new List<EnemyDropList>()
                                {
                                    new()
                                    {
                                        mana = 10,
                                        coin = 10,
                                        drop_list = new List<AtgenDropList>()
                                        {
                                            new() { type = EntityTypes.Dew, quantity = 10 },
                                            new() { type = EntityTypes.HustleHammer, quantity = 10 }
                                        }
                                    },
                                }
                            },
                            new()
                            {
                                enemy_drop_list = new List<EnemyDropList>()
                                {
                                    new()
                                    {
                                        mana = 10,
                                        coin = 10,
                                        drop_list = new List<AtgenDropList>()
                                        {
                                            new() { type = EntityTypes.AstralItem, quantity = 10 }
                                        }
                                    },
                                }
                            },
                            new()
                            {
                                enemy_drop_list = new List<EnemyDropList>()
                                {
                                    new()
                                    {
                                        mana = 10,
                                        coin = 10,
                                        drop_list = new List<AtgenDropList>()
                                        {
                                            new() { type = EntityTypes.Wyrmite, quantity = 10 }
                                        }
                                    },
                                    new()
                                    {
                                        mana = 10,
                                        coin = 10,
                                        drop_list = new List<AtgenDropList>()
                                        {
                                            new() { type = EntityTypes.FafnirMedal, quantity = 10 }
                                        }
                                    },
                                }
                            }
                        }
                    },
                    {
                        1,
                        new List<AtgenEnemy>()
                        {
                            new(),
                            new()
                            {
                                enemy_drop_list = new List<EnemyDropList>()
                                {
                                    new() { coin = 10, mana = 10, }
                                }
                            }
                        }
                    }
                }
            };

        this.mockRewardService
            .Setup(x => x.GrantReward(It.Is<Entity>(e => e.Type == EntityTypes.Dew)))
            .ReturnsAsync(RewardGrantResult.Added);
        this.mockRewardService
            .Setup(x => x.GrantReward(It.Is<Entity>(e => e.Type == EntityTypes.HustleHammer)))
            .ReturnsAsync(RewardGrantResult.Added);
        this.mockRewardService
            .Setup(x => x.GrantReward(It.Is<Entity>(e => e.Type == EntityTypes.Wyrmite)))
            .ReturnsAsync(RewardGrantResult.Added);
        this.mockRewardService
            .Setup(x => x.GrantReward(It.Is<Entity>(e => e.Type == EntityTypes.FafnirMedal)))
            .ReturnsAsync(RewardGrantResult.Added);

        this.mockRewardService
            .Setup(
                x =>
                    x.GrantReward(
                        It.Is<Entity>(e => e.Type == EntityTypes.Mana && e.Quantity == 40)
                    )
            )
            .ReturnsAsync(RewardGrantResult.Added);
        this.mockRewardService
            .Setup(
                x =>
                    x.GrantReward(
                        It.Is<Entity>(e => e.Type == EntityTypes.Rupies && e.Quantity == 40)
                    )
            )
            .ReturnsAsync(RewardGrantResult.Added);

        (await this.dungeonRecordRewardService.ProcessEnemyDrops(playRecord, session))
            .Should()
            .BeEquivalentTo(
                (
                    new List<AtgenDropAll>()
                    {
                        new() { type = EntityTypes.Dew, quantity = 10 },
                        new() { type = EntityTypes.HustleHammer, quantity = 10 },
                        new() { type = EntityTypes.FafnirMedal, quantity = 10 },
                        new() { type = EntityTypes.Wyrmite, quantity = 10 }
                    },
                    40,
                    40
                )
            );

        this.mockRewardService.VerifyAll();
    }

    [Fact]
    public async Task ProcessEventRewards_CallsExpectedMethods()
    {
        List<PartySettingList> party = new();
        DungeonSession session =
            new() { QuestData = MasterAsset.QuestData[100010101], Party = party, };
        PlayRecord playRecord = new();

        List<AtgenScoreMissionSuccessList> scoreMissionSuccessLists =
            new()
            {
                new()
                {
                    score_mission_complete_type = QuestCompleteType.DragonElementLocked,
                    score_target_value = 2,
                }
            };

        List<AtgenEventPassiveUpList> passiveUpLists =
            new()
            {
                new() { passive_id = 3, progress = 10 }
            };

        List<AtgenDropAll> eventDrops =
            new()
            {
                new() { type = EntityTypes.Clb01EventItem, quantity = 100 }
            };

        int materialMultiplier = 2;
        int pointMultiplier = 3;
        int points = 10;
        int boostedPoints = 20;

        this.mockAbilityCrestMultiplierService
            .Setup(x => x.GetEventMultiplier(session.Party, session.QuestData.Gid))
            .ReturnsAsync((materialMultiplier, pointMultiplier));

        this.mockQuestCompletionService
            .Setup(x => x.CompleteQuestScoreMissions(session, playRecord, pointMultiplier))
            .ReturnsAsync((scoreMissionSuccessLists, points, boostedPoints));

        this.mockEventDropService
            .Setup(x => x.ProcessEventPassiveDrops(session.QuestData))
            .ReturnsAsync(passiveUpLists);
        this.mockEventDropService
            .Setup(
                x => x.ProcessEventMaterialDrops(session.QuestData, playRecord, materialMultiplier)
            )
            .ReturnsAsync(eventDrops);

        (await this.dungeonRecordRewardService.ProcessEventRewards(playRecord, session))
            .Should()
            .BeEquivalentTo(
                new DungeonRecordRewardService.EventRewardData(
                    scoreMissionSuccessLists,
                    points + boostedPoints,
                    boostedPoints,
                    passiveUpLists,
                    eventDrops
                )
            );

        this.mockAbilityCrestMultiplierService.VerifyAll();
        this.mockQuestCompletionService.VerifyAll();
        this.mockEventDropService.VerifyAll();
    }
}
