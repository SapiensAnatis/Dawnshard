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

    private readonly IDungeonRecordService dungeonRecordService;

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
        PlayRecord playRecord = new() { time = 10, };

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
                    id = (int)Materials.FirestormRuby,
                    quantity = 10,
                    type = EntityTypes.Material
                }
            };

        List<AtgenDropAll> eventDrops =
            new()
            {
                new()
                {
                    id = (int)Materials.WoodlandHerbs,
                    quantity = 20,
                    type = EntityTypes.Material
                }
            };

        List<AtgenScoreMissionSuccessList> scoreMissionSuccessLists =
            new()
            {
                new()
                {
                    score_mission_complete_type = QuestCompleteType.LimitFall,
                    score_target_value = 100,
                }
            };

        List<AtgenEventPassiveUpList> passiveUpLists =
            new()
            {
                new() { passive_id = 1, progress = 2 }
            };

        List<AtgenMissionsClearSet> missionsClearSets = new List<AtgenMissionsClearSet>()
        {
            new()
            {
                type = EntityTypes.CollectEventItem,
                id = 1,
                quantity = 2
            }
        };

        List<AtgenFirstClearSet> missionCompleteSets =
            new()
            {
                new()
                {
                    type = EntityTypes.ExchangeTicket,
                    id = 2,
                    quantity = 3
                }
            };

        List<AtgenFirstClearSet> firstClearSets =
            new()
            {
                new()
                {
                    type = EntityTypes.RaidEventItem,
                    id = 4,
                    quantity = 5
                }
            };

        List<AtgenScoringEnemyPointList> enemyScoring =
        [
            new()
            {
                scoring_enemy_id = 100,
                point = 1,
                smash_count = 2
            }
        ];

        QuestMissionStatus missionStatus =
            new(new bool[] { }, missionsClearSets, missionCompleteSets);

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

        this.mockDungeonRewardService.Setup(
            x => x.ProcessQuestMissionCompletion(playRecord, session)
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
                    dungeon_key = "dungeonKey",
                    play_type = QuestPlayType.Default,
                    quest_id = lSurtrSoloId,
                    is_host = true,
                    quest_party_setting_list = session.Party,
                    start_time = session.StartTime,
                    end_time = DateTimeOffset.UtcNow,
                    reborn_count = playRecord.reborn_count,
                    total_play_damage = playRecord.total_play_damage,
                    is_clear = true,
                    current_play_count = 1,
                    reward_record = new()
                    {
                        drop_all = dropList.Concat(eventDrops).ToList(),
                        take_boost_accumulate_point = takeBoostAccumulatePoint,
                        take_accumulate_point = takeAccumulatePoint,
                        take_coin = takeCoin,
                        take_astral_item_quantity = 0,
                        player_level_up_fstone = 50,
                        first_clear_set = firstClearSets,
                        mission_complete = missionCompleteSets,
                        missions_clear_set = missionsClearSets,
                    },
                    grow_record = new()
                    {
                        take_mana = takeMana,
                        take_player_exp = 400,
                        take_chara_exp = 0,
                        bonus_factor = 1,
                        mana_bonus_factor = 1,
                        chara_grow_record = new List<AtgenCharaGrowRecord>()
                    },
                    event_passive_up_list = passiveUpLists,
                    score_mission_success_list = scoreMissionSuccessLists,
                    scoring_enemy_point_list = enemyScoring,
                    is_best_clear_time = true,
                    clear_time = playRecord.time,
                    converted_entity_list = new List<ConvertedEntityList>()
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
