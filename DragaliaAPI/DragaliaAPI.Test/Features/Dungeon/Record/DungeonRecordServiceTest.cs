using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Chara;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Test.Utils;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Features.Dungeon.Record;

public class DungeonRecordServiceTest
{
    private readonly Mock<IDungeonRecordRewardService> mockDungeonRewardService;
    private readonly Mock<IQuestService> mockQuestService;
    private readonly Mock<IUserService> mockUserService;
    private readonly Mock<ITutorialService> mockTutorialService;
    private readonly Mock<ICharaService> mockCharaService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<ILogger<DungeonRecordService>> mockLogger;

    private readonly DungeonRecordService dungeonRecordService;

    public DungeonRecordServiceTest()
    {
        this.mockDungeonRewardService = new(MockBehavior.Strict);
        this.mockQuestService = new(MockBehavior.Strict);
        this.mockUserService = new(MockBehavior.Strict);
        this.mockTutorialService = new(MockBehavior.Strict);
        this.mockCharaService = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.dungeonRecordService = new DungeonRecordService(
            this.mockDungeonRewardService.Object,
            this.mockQuestService.Object,
            this.mockUserService.Object,
            this.mockTutorialService.Object,
            this.mockCharaService.Object,
            this.mockRewardService.Object,
            this.mockLogger.Object
        );

        this.mockTutorialService.Setup(x => x.AddTutorialFlag(1022)).ReturnsAsync(new List<int>());

        CommonAssertionOptions.ApplyTimeOptions();
    }

    [Fact]
    public async Task GenerateIngameResultData_CallsExpectedMethods()
    {
        int lSurtrSoloId = 232031101;

        DungeonSession session =
            new()
            {
                QuestData = MasterAsset.QuestData[lSurtrSoloId],
                Party = new List<PartySettingList>(),
                StartTime = DateTimeOffset.UtcNow
            };
        PlayRecord playRecord = new() { Time = 10, };

        DbQuest mockQuest =
            new()
            {
                ViewerId = 1,
                QuestId = lSurtrSoloId,
                State = 0,
                BestClearTime = 999
            };

        List<AtgenDropAll> dropList =
            new()
            {
                new()
                {
                    Id = (int)Materials.FirestormRuby,
                    Quantity = 10,
                    Type = EntityTypes.Material
                }
            };

        List<AtgenDropAll> eventDrops =
            new()
            {
                new()
                {
                    Id = (int)Materials.WoodlandHerbs,
                    Quantity = 20,
                    Type = EntityTypes.Material
                }
            };

        List<AtgenScoreMissionSuccessList> scoreMissionSuccessLists =
            new()
            {
                new()
                {
                    ScoreMissionCompleteType = QuestCompleteType.LimitFall,
                    ScoreTargetValue = 100,
                }
            };

        List<AtgenEventPassiveUpList> passiveUpLists =
            new()
            {
                new() { PassiveId = 1, Progress = 2 }
            };

        List<AtgenMissionsClearSet> missionsClearSets = new List<AtgenMissionsClearSet>()
        {
            new()
            {
                Type = EntityTypes.CollectEventItem,
                Id = 1,
                Quantity = 2
            }
        };

        List<AtgenFirstClearSet> missionCompleteSets =
            new()
            {
                new()
                {
                    Type = EntityTypes.ExchangeTicket,
                    Id = 2,
                    Quantity = 3
                }
            };

        List<AtgenFirstClearSet> firstClearSets =
            new()
            {
                new()
                {
                    Type = EntityTypes.RaidEventItem,
                    Id = 4,
                    Quantity = 5
                }
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

        QuestMissionStatus missionStatus = new([], missionsClearSets, missionCompleteSets);

        int takeCoin = 10;
        int takeMana = 20;
        int takeAccumulatePoint = 30;
        int takeBoostAccumulatePoint = 40;

        this.mockQuestService.Setup(x => x.ProcessQuestCompletion(session, playRecord))
            .ReturnsAsync((true, new List<AtgenFirstClearSet>()));

        this.mockUserService.Setup(x => x.RemoveStamina(StaminaType.Single, 40))
            .Returns(Task.CompletedTask);
        this.mockUserService.Setup(x => x.AddExperience(400))
            .ReturnsAsync(new PlayerLevelResult(true, 100, 50));

        this.mockDungeonRewardService.Setup(x =>
            x.ProcessQuestMissionCompletion(playRecord, session)
        )
            .ReturnsAsync((missionStatus, firstClearSets));
        this.mockDungeonRewardService.Setup(x => x.ProcessEnemyDrops(playRecord, session))
            .ReturnsAsync((dropList, takeMana, takeCoin));
        this.mockDungeonRewardService.Setup(x => x.ProcessEventRewards(playRecord, session))
            .ReturnsAsync(
                new DungeonRecordRewardService.EventRewardData(
                    scoreMissionSuccessLists,
                    enemyScoring,
                    takeAccumulatePoint,
                    takeBoostAccumulatePoint,
                    passiveUpLists,
                    eventDrops
                )
            );

        this.mockQuestService.Setup(x => x.GetQuestStamina(lSurtrSoloId, StaminaType.Single))
            .ReturnsAsync(40);

        this.mockRewardService.Setup(x => x.GetConvertedEntityList())
            .Returns(new List<ConvertedEntity>());

        IngameResultData ingameResultData =
            await this.dungeonRecordService.GenerateIngameResultData(
                "dungeonKey",
                playRecord,
                session
            );

        ingameResultData
            .Should()
            .BeEquivalentTo(
                new IngameResultData()
                {
                    DungeonKey = "dungeonKey",
                    PlayType = QuestPlayType.Default,
                    QuestId = lSurtrSoloId,
                    IsHost = true,
                    QuestPartySettingList = session.Party,
                    StartTime = session.StartTime,
                    EndTime = DateTimeOffset.UtcNow,
                    RebornCount = playRecord.RebornCount,
                    TotalPlayDamage = playRecord.TotalPlayDamage,
                    IsClear = true,
                    CurrentPlayCount = 1,
                    RewardRecord = new()
                    {
                        DropAll = dropList.Concat(eventDrops).ToList(),
                        TakeBoostAccumulatePoint = takeBoostAccumulatePoint,
                        TakeAccumulatePoint = takeAccumulatePoint,
                        TakeCoin = takeCoin,
                        TakeAstralItemQuantity = 0,
                        PlayerLevelUpFstone = 50,
                        FirstClearSet = firstClearSets,
                        MissionComplete = missionCompleteSets,
                        MissionsClearSet = missionsClearSets,
                    },
                    GrowRecord = new()
                    {
                        TakeMana = takeMana,
                        TakePlayerExp = 400,
                        TakeCharaExp = 0,
                        BonusFactor = 1,
                        ManaBonusFactor = 1,
                        CharaGrowRecord = new List<AtgenCharaGrowRecord>()
                    },
                    EventPassiveUpList = passiveUpLists,
                    ScoreMissionSuccessList = scoreMissionSuccessLists,
                    ScoringEnemyPointList = enemyScoring,
                    IsBestClearTime = true,
                    ClearTime = playRecord.Time,
                    ConvertedEntityList = new List<ConvertedEntityList>()
                }
            );

        this.mockDungeonRewardService.VerifyAll();
        this.mockQuestService.VerifyAll();
        this.mockUserService.VerifyAll();
        this.mockTutorialService.VerifyAll();
        this.mockLogger.VerifyAll();
        this.mockQuestService.VerifyAll();
    }
}
