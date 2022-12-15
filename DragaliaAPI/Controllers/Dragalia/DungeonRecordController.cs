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
    private readonly IUpdateDataService updateDataService;

    public DungeonRecordController(
        IQuestRepository questRepository,
        IDungeonService dungeonService,
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        IUpdateDataService updateDataService
    )
    {
        this.questRepository = questRepository;
        this.dungeonService = dungeonService;
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.updateDataService = updateDataService;
    }

    [HttpPost("record")]
    public async Task<DragaliaResult> Record(DungeonRecordRecordRequest request)
    {
        DungeonSession session = await this.dungeonService.FinishDungeon(request.dungeon_key);

        await this.userDataRepository.AddTutorialFlag(this.DeviceAccountId, 1022);
        await this.questRepository.CompleteQuest(this.DeviceAccountId, session.DungeonId);

        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .SingleAsync();

        userData.Exp += 1;
        userData.ManaPoint += 1000;
        userData.Coin += 1000;

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.questRepository.SaveChangesAsync();

        return this.Ok(
            new DungeonRecordRecordData()
            {
                ingame_result_data = new()
                {
                    dungeon_key = request.dungeon_key,
                    play_type = 1,
                    quest_id = session.DungeonId,
                    reward_record = new()
                    {
                        drop_all = new List<AtgenDropAll>()
                        {
                            new()
                            {
                                type = (int)EntityTypes.Material,
                                id = 201014003, // Squishum
                                quantity = 1,
                                place = 0,
                                factor = 0
                            }
                        },
                        first_clear_set = new List<AtgenFirstClearSet>()
                        {
                            new()
                            {
                                type = (int)EntityTypes.Wyrmite,
                                id = 0,
                                quantity = 5
                            }
                        },
                        take_coin = 1000,
                        take_astral_item_quantity = 300,
                        missions_clear_set = Enumerable
                            .Range(1, 3)
                            .Select(
                                x =>
                                    new AtgenMissionsClearSet()
                                    {
                                        type = 23,
                                        id = 0,
                                        quantity = 5,
                                        mission_no = x
                                    }
                            ),
                        mission_complete = new List<AtgenFirstClearSet>()
                        {
                            new()
                            {
                                type = (int)EntityTypes.Wyrmite,
                                id = 0,
                                quantity = 1234
                            }
                        },
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
                        take_mana = 1000,
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
                    clear_time = 70,
                    is_best_clear_time = true,
                    converted_entity_list = new List<ConvertedEntityList>(),
                    dungeon_skip_type = 0,
                    total_play_damage = 0,
                },
                update_data_list = updateDataList,
                entity_result = new(),
            }
        );
    }

    [HttpPost("record_multi")]
    public async Task<DragaliaResult> RecordMulti(DungeonRecordRecordMultiRequest request)
    {
        DungeonSession session = await this.dungeonService.FinishDungeon(request.dungeon_key);

        await this.userDataRepository.AddTutorialFlag(this.DeviceAccountId, 1022);
        await this.questRepository.CompleteQuest(this.DeviceAccountId, session.DungeonId);

        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .SingleAsync();

        userData.Exp += 1;
        userData.ManaPoint += 1000;
        userData.Coin += 1000;

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.questRepository.SaveChangesAsync();

        return this.Ok(
            new DungeonRecordRecordMultiData()
            {
                ingame_result_data = new()
                {
                    dungeon_key = request.dungeon_key,
                    play_type = 1,
                    quest_id = session.DungeonId,
                    reward_record = new()
                    {
                        drop_all = new List<AtgenDropAll>()
                        {
                            new()
                            {
                                type = (int)EntityTypes.Material,
                                id = 201014003, // Squishum
                                quantity = 1,
                                place = 0,
                                factor = 0
                            }
                        },
                        first_clear_set = new List<AtgenFirstClearSet>()
                        {
                            new()
                            {
                                type = (int)EntityTypes.Wyrmite,
                                id = 0,
                                quantity = 5
                            }
                        },
                        take_coin = 1000,
                        take_astral_item_quantity = 300,
                        missions_clear_set = Enumerable
                            .Range(1, 3)
                            .Select(
                                x =>
                                    new AtgenMissionsClearSet()
                                    {
                                        type = 23,
                                        id = 0,
                                        quantity = 5,
                                        mission_no = x
                                    }
                            ),
                        mission_complete = new List<AtgenFirstClearSet>()
                        {
                            new()
                            {
                                type = (int)EntityTypes.Wyrmite,
                                id = 0,
                                quantity = 1234
                            }
                        },
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
                        take_mana = 1000,
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
                    clear_time = 70,
                    is_best_clear_time = true,
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
