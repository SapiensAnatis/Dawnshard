using System.Diagnostics;
using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon;

[Route("dungeon_record")]
public class DungeonRecordController : DragaliaControllerBase
{
    private readonly IQuestRepository questRepository;
    private readonly IDungeonService dungeonService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly ITutorialService tutorialService;
    private readonly IMissionProgressionService missionProgressionService;
    private readonly ILogger<DungeonRecordController> logger;
    private readonly IQuestCompletionService questCompletionService;

    private const int QuestCoin = 10_000_000;
    private const int QuestMana = 20_000;
    private const int QuestDropQuantity = 100;

    public DungeonRecordController(
        IQuestRepository questRepository,
        IDungeonService dungeonService,
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        IUpdateDataService updateDataService,
        ITutorialService tutorialService,
        IMissionProgressionService missionProgressionService,
        ILogger<DungeonRecordController> logger,
        IQuestCompletionService questCompletionService
    )
    {
        this.questRepository = questRepository;
        this.dungeonService = dungeonService;
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.updateDataService = updateDataService;
        this.tutorialService = tutorialService;
        this.missionProgressionService = missionProgressionService;
        this.logger = logger;
        this.questCompletionService = questCompletionService;
    }

    [HttpPost("record")]
    [HttpPost("record_multi")]
    public async Task<DragaliaResult> Record(DungeonRecordRecordRequest request)
    {
        // TODO: Turn this method into a service call
        DungeonSession session = await dungeonService.FinishDungeon(request.dungeon_key);

        DbQuest? oldQuestData = await questRepository.Quests.SingleOrDefaultAsync(
            x => x.QuestId == session.QuestData.Id
        );

        bool isFirstClear = oldQuestData is null || oldQuestData?.PlayCount == 0;

        float clear_time = request.play_record?.time ?? -1.0f;

        await tutorialService.AddTutorialFlag(1022);

        // oldQuestData and newQuestData actually reference the same object so this is somewhat redundant
        // keeping it for clarity and because oldQuestData is null in some tests
        DbQuest newQuestData = await questRepository.CompleteQuest(
            session.QuestData.Id,
            clear_time
        );

        // Void battle moment :(
        if (session.QuestData.IsPartOfVoidBattleGroups)
            missionProgressionService.OnVoidBattleCleared();

        missionProgressionService.OnQuestCleared(session.QuestData.Id);

        DbPlayerUserData userData = await userDataRepository.UserData.SingleAsync();

        if (isFirstClear)
        {
            userData.Crystal += 5;
        }

        userData.Exp += 1;
        userData.ManaPoint += QuestMana;
        userData.Coin += QuestCoin;

        bool[] oldMissionStatus =
        {
            newQuestData.IsMissionClear1,
            newQuestData.IsMissionClear2,
            newQuestData.IsMissionClear3
        };

        QuestMissionStatus status = await this.questCompletionService.CompleteQuestMissions(
            session.QuestData.Id,
            oldMissionStatus,
            request.play_record!,
            session
        );

        newQuestData.IsMissionClear1 = status.Missions[0];
        newQuestData.IsMissionClear2 = status.Missions[1];
        newQuestData.IsMissionClear3 = status.Missions[2];

        List<AtgenDropAll> drops = new();
        int manaDrop = 0;
        int coinDrop = 0;

        foreach (
            AtgenTreasureRecord record in request.play_record?.treasure_record
                ?? Enumerable.Empty<AtgenTreasureRecord>()
        )
        {
            if (
                !session.EnemyList.TryGetValue(
                    record.area_idx,
                    out IEnumerable<AtgenEnemy>? enemyList
                )
            )
            {
                this.logger.LogWarning(
                    "Could not retrieve enemy list for area_idx {idx}",
                    record.area_idx
                );
                continue;
            }

            // Sometimes record.enemy is null for boss stages. Give all drops in this case.
            IEnumerable<int> enemyRecord = record.enemy ?? Enumerable.Repeat(1, enemyList.Count());

            foreach (
                EnemyDropList dropList in enemyList
                    .Zip(enemyRecord)
                    .Where(x => x.Second == 1)
                    .SelectMany(x => x.First.enemy_drop_list)
            )
            {
                manaDrop += dropList.mana;
                coinDrop += dropList.coin;
                drops.AddRange(
                    dropList.drop_list.Select(
                        x =>
                            new AtgenDropAll()
                            {
                                type = EntityTypes.Material,
                                id = x.id,
                                quantity = x.quantity,
                            }
                    )
                );
            }
        }

        await inventoryRepository.UpdateQuantity(
            drops.Select(x => new KeyValuePair<Materials, int>((Materials)x.id, x.quantity))
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        return Ok(
            new DungeonRecordRecordData()
            {
                ingame_result_data = new()
                {
                    dungeon_key = request.dungeon_key,
                    play_type = 1,
                    quest_id = session.QuestData.Id,
                    reward_record = new()
                    {
                        drop_all = drops,
                        first_clear_set = isFirstClear
                            ? new List<AtgenFirstClearSet>()
                            {
                                new()
                                {
                                    type = EntityTypes.Wyrmite,
                                    id = 0,
                                    quantity = 5
                                }
                            }
                            : new List<AtgenFirstClearSet>(),
                        take_coin = coinDrop,
                        take_astral_item_quantity = 300,
                        missions_clear_set = status.MissionsClearSet,
                        mission_complete = status.MissionCompleteSet,
                        enemy_piece = new List<AtgenEnemyPiece>(),
                        reborn_bonus = new List<AtgenFirstClearSet>(),
                        quest_bonus_list = new List<AtgenFirstClearSet>(),
                        carry_bonus = new List<AtgenFirstClearSet>(),
                        challenge_quest_bonus_list = new List<AtgenFirstClearSet>(),
                        campaign_extra_reward_list = new List<AtgenFirstClearSet>(),
                        weekly_limit_reward_list = new List<AtgenFirstClearSet>(),
                    },
                    grow_record = new()
                    {
                        take_player_exp = 1,
                        take_chara_exp = 4000,
                        take_mana = manaDrop,
                        bonus_factor = 1,
                        mana_bonus_factor = 1,
                        chara_grow_record = session.Party.Select(
                            x =>
                                new AtgenCharaGrowRecord()
                                {
                                    chara_id = (int)x.chara_id,
                                    take_exp = 240
                                }
                        ),
                        chara_friendship_list = new List<CharaFriendshipList>()
                    },
                    start_time = DateTimeOffset.UtcNow,
                    end_time = DateTimeOffset.FromUnixTimeSeconds(0),
                    current_play_count = 1,
                    is_clear = false,
                    state = 1,
                    is_host = true,
                    reborn_count = 0,
                    helper_list = new List<UserSupportList>(),
                    helper_detail_list = new List<AtgenHelperDetailList>(),
                    quest_party_setting_list = session.Party,
                    bonus_factor_list = new List<AtgenBonusFactorList>(),
                    scoring_enemy_point_list = new List<AtgenScoringEnemyPointList>(),
                    score_mission_success_list = new List<AtgenScoreMissionSuccessList>(),
                    event_passive_up_list = new List<AtgenEventPassiveUpList>(),
                    clear_time = clear_time,
                    is_best_clear_time = clear_time == newQuestData.BestClearTime,
                    converted_entity_list = new List<ConvertedEntityList>(),
                    dungeon_skip_type = 0,
                    total_play_damage = 0,
                },
                update_data_list = updateDataList,
                entity_result = new(),
            }
        );
    }
}
