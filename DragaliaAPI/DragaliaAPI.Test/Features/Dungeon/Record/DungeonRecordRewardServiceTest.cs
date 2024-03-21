using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Missions;
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
    private readonly Mock<IMissionProgressionService> mockMissionProgressionService;
    private readonly Mock<IQuestRepository> mockQuestRepository;
    private readonly Mock<ILogger<DungeonRecordRewardService>> mockLogger;

    private readonly IDungeonRecordRewardService dungeonRecordRewardService;

    public DungeonRecordRewardServiceTest()
    {
        this.mockQuestCompletionService = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Strict);
        this.mockAbilityCrestMultiplierService = new(MockBehavior.Strict);
        this.mockEventDropService = new(MockBehavior.Strict);
        this.mockMissionProgressionService = new(MockBehavior.Strict);
        this.mockQuestRepository = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.dungeonRecordRewardService = new DungeonRecordRewardService(
            this.mockQuestCompletionService.Object,
            this.mockRewardService.Object,
            this.mockAbilityCrestMultiplierService.Object,
            this.mockEventDropService.Object,
            this.mockMissionProgressionService.Object,
            this.mockQuestRepository.Object,
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
                ViewerId = 1,
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
                    Id = 0,
                    Quantity = 2,
                    Type = EntityTypes.Wyrmite
                }
            };

        PlayRecord playRecord = new();
        DungeonSession session =
            new() { QuestData = MasterAsset.QuestData[questId], Party = null! };
        QuestMissionStatus status =
            new(
                [true, true, true],
                new List<AtgenMissionsClearSet>(),
                new List<AtgenFirstClearSet>()
            );

        this.mockQuestRepository.Setup(x => x.GetQuestDataAsync(questId)).ReturnsAsync(questEntity);

        this.mockQuestCompletionService.Setup(x =>
            x.CompleteQuestMissions(session, new[] { false, false, false }, playRecord)
        )
            .ReturnsAsync(status);
        this.mockQuestCompletionService.Setup(x => x.GrantFirstClearRewards(questId))
            .ReturnsAsync(firstClearRewards);

        (await this.dungeonRecordRewardService.ProcessQuestMissionCompletion(playRecord, session))
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
                TreasureRecord = new List<AtgenTreasureRecord>()
                {
                    new()
                    {
                        AreaIdx = 0,
                        Enemy = new List<int>() { 1, 0, 1 }
                    },
                    new()
                    {
                        AreaIdx = 1,
                        Enemy = new List<int>() { 0, 1, 0 }
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
                                EnemyDropList = new List<EnemyDropList>()
                                {
                                    new()
                                    {
                                        Mana = 10,
                                        Coin = 10,
                                        DropList = new List<AtgenDropList>()
                                        {
                                            new() { Type = EntityTypes.Dew, Quantity = 10 },
                                            new() { Type = EntityTypes.HustleHammer, Quantity = 10 }
                                        }
                                    },
                                }
                            },
                            new()
                            {
                                EnemyDropList = new List<EnemyDropList>()
                                {
                                    new()
                                    {
                                        Mana = 10,
                                        Coin = 10,
                                        DropList = new List<AtgenDropList>()
                                        {
                                            new() { Type = EntityTypes.AstralItem, Quantity = 10 }
                                        }
                                    },
                                }
                            },
                            new()
                            {
                                EnemyDropList = new List<EnemyDropList>()
                                {
                                    new()
                                    {
                                        Mana = 10,
                                        Coin = 10,
                                        DropList = new List<AtgenDropList>()
                                        {
                                            new() { Type = EntityTypes.Wyrmite, Quantity = 10 }
                                        }
                                    },
                                    new()
                                    {
                                        Mana = 10,
                                        Coin = 10,
                                        DropList = new List<AtgenDropList>()
                                        {
                                            new() { Type = EntityTypes.FafnirMedal, Quantity = 10 }
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
                                EnemyDropList = new List<EnemyDropList>()
                                {
                                    new() { Coin = 10, Mana = 10, }
                                }
                            }
                        }
                    }
                }
            };

        this.mockRewardService.Setup(x => x.GrantRewards(It.IsAny<List<Entity>>()))
            .Returns(Task.CompletedTask);
        this.mockRewardService.Setup(x =>
            x.GrantReward(It.Is<Entity>(e => e.Type == EntityTypes.Mana && e.Quantity == 40))
        )
            .ReturnsAsync(RewardGrantResult.Added);
        this.mockRewardService.Setup(x =>
            x.GrantReward(It.Is<Entity>(e => e.Type == EntityTypes.Rupies && e.Quantity == 40))
        )
            .ReturnsAsync(RewardGrantResult.Added);

        (await this.dungeonRecordRewardService.ProcessEnemyDrops(playRecord, session))
            .Should()
            .BeEquivalentTo(
                (
                    new List<AtgenDropAll>()
                    {
                        new() { Type = EntityTypes.Dew, Quantity = 10 },
                        new() { Type = EntityTypes.HustleHammer, Quantity = 10 },
                        new() { Type = EntityTypes.FafnirMedal, Quantity = 10 },
                        new() { Type = EntityTypes.Wyrmite, Quantity = 10 }
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
                    ScoreMissionCompleteType = QuestCompleteType.DragonElementLocked,
                    ScoreTargetValue = 2,
                }
            };

        List<AtgenEventPassiveUpList> passiveUpLists =
            new()
            {
                new() { PassiveId = 3, Progress = 10 }
            };

        List<AtgenDropAll> eventDrops =
            new()
            {
                new() { Type = EntityTypes.Clb01EventItem, Quantity = 100 }
            };

        List<AtgenScoringEnemyPointList> enemyScoring =
        [
            new()
            {
                ScoringEnemyId = 100,
                Point = 1,
                SmashCount = 2
            }
        ];

        int materialMultiplier = 2;
        int pointMultiplier = 3;
        int points = 10;
        int boostedPoints = 20;
        int enemyPoints = 30;

        this.mockAbilityCrestMultiplierService.Setup(x =>
            x.GetEventMultiplier(session.Party, session.QuestData.Gid)
        )
            .ReturnsAsync((materialMultiplier, pointMultiplier));

        this.mockQuestCompletionService.Setup(x =>
            x.CompleteQuestScoreMissions(session, playRecord, pointMultiplier)
        )
            .ReturnsAsync((scoreMissionSuccessLists, points, boostedPoints));
        this.mockQuestCompletionService.Setup(x =>
            x.CompleteEnemyScoreMissions(session, playRecord)
        )
            .ReturnsAsync((enemyScoring, enemyPoints));

        this.mockEventDropService.Setup(x => x.ProcessEventPassiveDrops(session.QuestData))
            .ReturnsAsync(passiveUpLists);
        this.mockEventDropService.Setup(x =>
            x.ProcessEventMaterialDrops(session.QuestData, playRecord, materialMultiplier)
        )
            .ReturnsAsync(eventDrops);

        this.mockMissionProgressionService.Setup(x =>
            x.OnEventPointCollected(
                session.QuestData.Gid,
                session.QuestVariation,
                points + boostedPoints
            )
        );

        (await this.dungeonRecordRewardService.ProcessEventRewards(playRecord, session))
            .Should()
            .BeEquivalentTo(
                new DungeonRecordRewardService.EventRewardData(
                    scoreMissionSuccessLists,
                    enemyScoring,
                    points + boostedPoints + enemyPoints,
                    boostedPoints,
                    passiveUpLists,
                    eventDrops
                )
            );

        this.mockAbilityCrestMultiplierService.VerifyAll();
        this.mockQuestCompletionService.VerifyAll();
        this.mockEventDropService.VerifyAll();
        this.mockMissionProgressionService.VerifyAll();
    }
}
