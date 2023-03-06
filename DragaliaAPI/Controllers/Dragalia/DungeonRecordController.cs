using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("dungeon_record")]
public class DungeonRecordController : DragaliaControllerBase
{
    private readonly IQuestRepository questRepository;
    private readonly IDungeonService dungeonService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IQuestRewardService questRewardService;
    private readonly IUpdateDataService updateDataService;

    private const int QuestCoin = 10_000_000;
    private const int QuestMana = 20_000;
    private const int QuestDropQuantity = 100;

    public DungeonRecordController(
        IQuestRepository questRepository,
        IDungeonService dungeonService,
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        IQuestRewardService questRewardService,
        IUpdateDataService updateDataService
    )
    {
        this.questRepository = questRepository;
        this.dungeonService = dungeonService;
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.questRewardService = questRewardService;
        this.updateDataService = updateDataService;
    }

    [HttpPost("record")]
    [HttpPost("record_multi")]
    public async Task<DragaliaResult> Record(DungeonRecordRecordRequest request)
    {
        // TODO: Turn this method into a service call
        DungeonSession session = await this.dungeonService.FinishDungeon(request.dungeon_key);

        DbQuest? oldQuestData = await this.questRepository
            .GetQuests(this.DeviceAccountId)
            .SingleOrDefaultAsync(x => x.QuestId == session.QuestData.Id);

        bool isFirstClear = oldQuestData is null || oldQuestData?.PlayCount == 0;
        bool oldMissionClear1 = oldQuestData?.IsMissionClear1 ?? false;
        bool oldMissionClear2 = oldQuestData?.IsMissionClear2 ?? false;
        bool oldMissionClear3 = oldQuestData?.IsMissionClear3 ?? false;

        float clear_time = request.play_record?.time ?? -1.0f;

        await this.userDataRepository.AddTutorialFlag(this.DeviceAccountId, 1022);

        // oldQuestData and newQuestData actually reference the same object so this is somewhat redundant
        // keeping it for clarity and because oldQuestData is null in some tests
        DbQuest newQuestData = await this.questRepository.CompleteQuest(
            this.DeviceAccountId,
            session.QuestData.Id,
            clear_time
        );

        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .SingleAsync();

        userData.Exp += 1;
        userData.ManaPoint += QuestMana;
        userData.Coin += QuestCoin;

        bool[] clearedMissions = new bool[3]
        {
            newQuestData.IsMissionClear1 && !oldMissionClear1,
            newQuestData.IsMissionClear2 && !oldMissionClear2,
            newQuestData.IsMissionClear3 && !oldMissionClear3,
        };

        bool allMissionsCleared =
            clearedMissions.Any(x => x)
            && newQuestData.IsMissionClear1
            && newQuestData.IsMissionClear2
            && newQuestData.IsMissionClear3;

        userData.Crystal +=
            (isFirstClear ? 5 : 0)
            + (clearedMissions.Where(x => x).Count() * 5)
            + (allMissionsCleared ? 5 : 0);

        IEnumerable<Materials> drops = this.questRewardService.GetDrops(session.QuestData.Id);
        await this.inventoryRepository.UpdateQuantity(drops, QuestDropQuantity);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return this.Ok(
            new DungeonRecordRecordData()
            {
                ingame_result_data = new()
                {
                    dungeon_key = request.dungeon_key,
                    play_type = 1,
                    quest_id = session.QuestData.Id,
                    reward_record = new()
                    {
                        drop_all = drops.Select(
                            x =>
                                new AtgenDropAll()
                                {
                                    id = (int)x,
                                    type = EntityTypes.Material,
                                    quantity = QuestDropQuantity
                                }
                        ),
                        first_clear_set = isFirstClear
                            ? new List<AtgenFirstClearSet>()
                            {
                                new()
                                {
                                    type = (int)EntityTypes.Wyrmite,
                                    id = 0,
                                    quantity = 5
                                }
                            }
                            : new List<AtgenFirstClearSet>(),
                        take_coin = QuestCoin,
                        take_astral_item_quantity = 300,
                        missions_clear_set = clearedMissions
                            .Select((x, index) => new { x, index })
                            .Where(x => x.x)
                            .Select(
                                x =>
                                    new AtgenMissionsClearSet()
                                    {
                                        type = (int)EntityTypes.Wyrmite,
                                        id = 0,
                                        quantity = 5,
                                        mission_no = x.index + 1
                                    }
                            ),
                        mission_complete = allMissionsCleared
                            ? new List<AtgenFirstClearSet>()
                            {
                                new()
                                {
                                    type = (int)EntityTypes.Wyrmite,
                                    id = 0,
                                    quantity = 5
                                }
                            }
                            : new List<AtgenFirstClearSet>(),
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
                        take_mana = QuestMana,
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
