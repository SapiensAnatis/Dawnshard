using System.Net.Http.Headers;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

public class DungeonRecordTest : TestFixture
{
    public DungeonRecordTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions(2);

        this.ApiContext.PlayerUserData.ExecuteUpdate(
            p => p.SetProperty(e => e.StaminaSingle, e => 100)
        );
        this.ApiContext.PlayerUserData.ExecuteUpdate(
            p => p.SetProperty(e => e.StaminaMulti, e => 100)
        );

        this.ApiContext.PlayerQuests.ExecuteDelete();
        this.ApiContext.PlayerMissions.ExecuteDelete();
        this.ApiContext.PlayerAbilityCrests.ExecuteDelete();
        this.ApiContext.PlayerEventData.ExecuteDelete();
        this.ApiContext.PlayerEventItems.ExecuteDelete();
        this.ApiContext.PlayerEventPassives.ExecuteDelete();
        this.ApiContext.PlayerEventRewards.ExecuteDelete();
        this.ApiContext.CompletedDailyMissions.ExecuteDelete();
    }

    [Fact]
    public async Task Record_ReturnsExpectedResponse()
    {
        int questId = 227100106;
        await AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                State = 0,
                ViewerId = ViewerId
            }
        );

        DbPlayerUserData oldUserData = await ApiContext
            .PlayerUserData.AsNoTracking()
            .SingleAsync(x => x.ViewerId == ViewerId);

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    {
                        1,
                        new List<AtgenEnemy>()
                        {
                            new()
                            {
                                enemy_idx = 0,
                                enemy_drop_list = new List<EnemyDropList>()
                                {
                                    new()
                                    {
                                        coin = 10,
                                        mana = 10,
                                        drop_list = new List<AtgenDropList>()
                                        {
                                            new()
                                            {
                                                type = EntityTypes.Material,
                                                id = (int)Materials.Squishums,
                                                quantity = 1
                                            }
                                        }
                                    }
                                }
                            },
                            new()
                            {
                                enemy_idx = 0,
                                enemy_drop_list = new List<EnemyDropList>()
                                {
                                    new()
                                    {
                                        coin = 10,
                                        mana = 10,
                                        drop_list = new List<AtgenDropList>()
                                        {
                                            new()
                                            {
                                                type = EntityTypes.Material,
                                                id = (int)Materials.ImitationSquish,
                                                quantity = 1
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        DungeonRecordRecordData response = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = key,
                    play_record = new PlayRecord
                    {
                        time = 10,
                        treasure_record = new List<AtgenTreasureRecord>()
                        {
                            new()
                            {
                                area_idx = 1,
                                enemy = new List<int>() { 1, 0 }
                            }
                        },
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord()
                    }
                }
            )
        ).data;

        response.ingame_result_data.dungeon_key.Should().Be(key);
        response.ingame_result_data.quest_id.Should().Be(227100106);

        response
            .ingame_result_data.reward_record.drop_all.Should()
            .BeEquivalentTo(
                new List<AtgenDropAll>()
                {
                    new()
                    {
                        type = EntityTypes.Material,
                        id = (int)Materials.Squishums,
                        quantity = 1
                    }
                }
            );
        response.ingame_result_data.reward_record.take_coin.Should().Be(10);
        response.ingame_result_data.grow_record.take_mana.Should().Be(10);

        response.ingame_result_data.grow_record.take_player_exp.Should().NotBe(0);

        response
            .update_data_list.user_data.coin.Should()
            .BeInRange(oldUserData.Coin + 10, oldUserData.Coin + 10 + 1000); // +1000 because of temp. quest event group reward
        response.update_data_list.user_data.mana_point.Should().Be(oldUserData.ManaPoint + 10);

        response
            .update_data_list.material_list.Should()
            .Contain(x => x.material_id == Materials.Squishums);
        response
            .update_data_list.quest_list.Should()
            .ContainEquivalentOf(
                new QuestList()
                {
                    quest_id = questId,
                    state = 3,
                    daily_play_count = 1,
                    play_count = 1,
                    weekly_play_count = 1,
                    is_appear = 1,
                    is_mission_clear_1 = 1,
                    is_mission_clear_2 = 1,
                    is_mission_clear_3 = 1,
                    best_clear_time = 10,
                    last_daily_reset_time = DateTimeOffset.UtcNow,
                    last_weekly_reset_time = DateTimeOffset.UtcNow
                }
            );

        response.repeat_data.Should().BeNull();
    }

    [Fact]
    public async Task Record_Event_UsesMultiplierAndCompletesScoreMissions()
    {
        int eventId = 20816;
        int questId = 208160502; // Flames of Reflection -- The Path To Mastery: Master

        await AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                State = 0,
                ViewerId = ViewerId
            }
        );

        await AddToDatabase(
            new DbAbilityCrest()
            {
                ViewerId = ViewerId,
                AbilityCrestId = AbilityCrests.SistersDayOut,
            }
        );

        await Client.PostMsgpack<MemoryEventActivateData>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { event_id = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>()
                {
                    new()
                    {
                        chara_id = Charas.ThePrince,
                        equip_crest_slot_type_1_crest_id_1 = AbilityCrests.SistersDayOut
                    }
                },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        DungeonRecordRecordData response = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = key,
                    play_record = new PlayRecord
                    {
                        time = 10,
                        treasure_record = new List<AtgenTreasureRecord>(),
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord(),
                        wave = 3
                    }
                }
            )
        ).data;

        response.ingame_result_data.score_mission_success_list.Should().NotBeEmpty();
        response.ingame_result_data.reward_record.take_accumulate_point.Should().NotBe(0);
        response.ingame_result_data.reward_record.take_boost_accumulate_point.Should().NotBe(0);
    }

    [Fact]
    public async Task Record_Event_BossBattle_CompletesMissions()
    {
        int questId = 208450301; // The Stirring Abyss: Beginner
        int eventId = 20845; // Toll of the Deep

        await Client.PostMsgpack<MemoryEventActivateData>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { event_id = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>()
                {
                    new()
                    {
                        chara_id = Charas.ThePrince,
                        equip_crest_slot_type_2_crest_id_1 = AbilityCrests.HavingaSummerBall
                    }
                },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordData response = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = key,
                    play_record = new PlayRecord
                    {
                        time = 10,
                        treasure_record = new List<AtgenTreasureRecord>(),
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord(),
                        wave = 3
                    }
                }
            )
        ).data;

        response
            .update_data_list.mission_notice.memory_event_mission_notice.new_complete_mission_id_list.Should()
            .Contain(10220201) // Clear a Boss Battle
            .And.Contain(10220401) // Clear a "Toll of the Deep" Quest with Having a Summer Ball Equipped
            .And.Contain(10220501); // Collect 100 Oceanic Resonance in One Go
    }

    [Fact]
    public async Task Record_Event_ChallengeBattle_CompletesMissions()
    {
        int questId = 208450502; // Tempestuous Assault: Master
        int eventId = 20845; // Toll of the Deep

        await Client.PostMsgpack<MemoryEventActivateData>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { event_id = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordData response = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = key,
                    play_record = new PlayRecord
                    {
                        time = 10,
                        treasure_record = new List<AtgenTreasureRecord>(),
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord(),
                        wave = 5 // Final wave
                    }
                }
            )
        ).data;

        response
            .update_data_list.mission_notice.memory_event_mission_notice.new_complete_mission_id_list.Should()
            .Contain(10221001) // Completely Clear a Challenge Battle on Master
            .And.Contain(10221301); // Earn the "Light of the Deep" Epithet

        // Clear Three Challenge Battles
        this.ApiContext.PlayerMissions.First(x => x.Id == 10220801).Progress.Should().Be(1);
    }

    [Fact]
    public async Task Record_Event_ChallengeBattle_Coop_CompletesMissions()
    {
        int questId = 229030201; // Repelling the Frosty Fiends: Standard (Co-Op)
        int eventId = 22903; // One Starry Dragonyule

        await Client.PostMsgpack(
            "/earn_event/entry",
            new EarnEventEntryRequest() { event_id = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordData response = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = key,
                    play_record = new PlayRecord
                    {
                        time = 10,
                        treasure_record =
                        [
                            new()
                            {
                                area_idx = 0,
                                enemy = [],
                                enemy_smash = []
                            }
                        ],
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord(),
                        wave = 2
                    }
                }
            )
        ).data;

        AtgenNormalMissionNotice? missionNotice = response
            .update_data_list
            .mission_notice
            ?.period_mission_notice;

        missionNotice.Should().NotBeNull();
        missionNotice!.new_complete_mission_id_list.Should().Contain(11650501); // Clear an Invasion on Standard
    }

    [Fact]
    public async Task Record_Event_Trial_CompletesMissions()
    {
        int questId = 208450702; // Wrath of Leviathan: Expert
        int eventId = 20845; // Toll of the Deep

        await Client.PostMsgpack<MemoryEventActivateData>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { event_id = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordData response = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = key,
                    play_record = new PlayRecord
                    {
                        time = 10,
                        treasure_record = new List<AtgenTreasureRecord>(),
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord(),
                        wave = 3
                    }
                }
            )
        ).data;

        response
            .update_data_list.mission_notice.memory_event_mission_notice.new_complete_mission_id_list.Should()
            .Contain(10221201); // Clear a "Toll of the Deep" Trial on Expert
    }

    [Fact]
    public async Task Record_Event_Trial_Coop_CompletesMissions()
    {
        int questId = 229030303; // The Angelic Herald: Master (Co-Op)
        int eventId = 22903; // One Starry Dragonyule

        await Client.PostMsgpack<EarnEventEntryData>(
            "/earn_event/entry",
            new EarnEventEntryRequest() { event_id = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordData response = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = key,
                    play_record = new PlayRecord
                    {
                        time = 10,
                        treasure_record =
                        [
                            new()
                            {
                                area_idx = 0,
                                drop_obj = [],
                                enemy = [],
                                enemy_smash = []
                            }
                        ],
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord(),
                        wave = 3
                    }
                }
            )
        ).data;

        AtgenNormalMissionNotice? missionNotice = response
            .update_data_list
            .mission_notice
            ?.period_mission_notice;

        missionNotice.Should().NotBeNull();
        missionNotice!.new_complete_mission_id_list.Should().Contain(11651001); // Clear a "One Starry Dragonyule" Trial on Master
    }

    [Fact]
    public async Task Record_EarnEvent_GrantsEnemyScore()
    {
        int questId = 229031201; // Repelling the Frosty Fiends: Standard (Solo)
        int eventId = 22903; // One Starry Dragonyule
        int enemyCount = 31; // Count of enemies returned from /dungeon_start/start

        await Client.PostMsgpack<MemoryEventActivateData>(
            "/earn_event/entry",
            new EarnEventEntryRequest() { event_id = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    {
                        1,

                        [
                            new()
                            {
                                param_id = 229030211,
                                enemy_idx = 0, // Meadow Rat (10 points)
                                enemy_drop_list = []
                            },
                            new()
                            {
                                param_id = 229030217,
                                enemy_idx = 1, // Wind Manticore (200 points)
                                enemy_drop_list = []
                            },
                            new()
                            {
                                param_id = 229030215,
                                enemy_idx = 2, // Arrow Raptor (40 points)
                                enemy_drop_list = []
                            }
                        ]
                    }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordData response = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = key,
                    play_record = new PlayRecord
                    {
                        time = 10,
                        treasure_record =
                        [
                            new()
                            {
                                area_idx = 1,
                                enemy_smash =
                                [
                                    new() { count = 1 },
                                    new() { count = 2 },
                                    new() { count = 3 }
                                ]
                            }
                        ],
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord(),
                        wave = 3
                    }
                }
            )
        ).data;

        response
            .ingame_result_data.reward_record.take_accumulate_point.Should()
            .Be((10 * 1) + (200 * 2) + (40 * 3));
        response.ingame_result_data.scoring_enemy_point_list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Record_HandlesNonExistentQuestData()
    {
        int questId = 219031102;

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince, } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        DragaliaResponse<DungeonRecordRecordData> response =
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = key,
                    play_record = new PlayRecord
                    {
                        time = 10,
                        treasure_record = new List<AtgenTreasureRecord>(),
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord(),
                        wave = 3
                    }
                }
            );

        response.data_headers.result_code.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task RecordTimeAttack_RegistersClear()
    {
        this.ImportSave();
        this.SetupPhotonAuthentication();

        int questId = 227010104; // Volk's Wrath TA Solo
        string roomName = Guid.NewGuid().ToString();
        string roomId = "1234";
        string gameId = $"{roomName}_{roomId}";

        this.Client.DefaultRequestHeaders.Add("RoomName", roomName);
        this.Client.DefaultRequestHeaders.Add("RoomId", roomId);

        await this.AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                State = 0,
                ViewerId = ViewerId
            }
        );

        this.MockPhotonStateApi.Setup(x => x.GetGameByViewerId(this.ViewerId))
            .ReturnsAsync(new Photon.Shared.Models.ApiGame() { Name = roomName });

        DungeonStartStartMultiData startResponse = (
            await this.Client.PostMsgpack<DungeonStartStartMultiData>(
                "/dungeon_start/start_multi",
                new DungeonStartStartMultiRequest()
                {
                    party_no_list = new int[] { 4 }, // Flame team
                    quest_id = questId
                }
            )
        ).data;

        string dungeonKey = startResponse.ingame_data.dungeon_key;
        float clearTime = 10.4f;

        await this.Client.PostMsgpack(
            "/dungeon_record/record_time_attack",
            new DungeonRecordRecordMultiRequest()
            {
                dungeon_key = dungeonKey,
                play_record = new PlayRecord
                {
                    time = clearTime,
                    treasure_record = new List<AtgenTreasureRecord>(),
                    live_unit_no_list = new List<int>(),
                    damage_record = new List<AtgenDamageRecord>(),
                    dragon_damage_record = new List<AtgenDamageRecord>(),
                    battle_royal_record = new AtgenBattleRoyalRecord(),
                    wave = 3
                }
            }
        );

        this.ApiContext.TimeAttackClears.Should().ContainSingle(x => x.GameId == gameId);

        DbTimeAttackClear? recordedClear = await this.ApiContext.TimeAttackClears.Include(
            x => x.Players
        )
            .ThenInclude(x => x.Units)
            .FirstAsync(x => x.GameId == gameId);

        recordedClear.Time.Should().Be(clearTime);
        recordedClear.QuestId.Should().Be(questId);
        recordedClear.Players.Should().ContainSingle(x => x.ViewerId == ViewerId);

        DbTimeAttackPlayer player = recordedClear.Players.First(x => x.ViewerId == ViewerId);

        player.PartyInfo.Should().NotBeNullOrEmpty();
        player.Units.Should().HaveCount(4);
    }

    [Fact]
    public async Task Record_InsufficientStamina_ReturnsError()
    {
        int questId = 100020203;
        await AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                State = 3,
                ViewerId = ViewerId
            }
        );

        this.ApiContext.PlayerUserData.ExecuteUpdate(
            p => p.SetProperty(e => e.StaminaSingle, e => 0)
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>() { }
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = key,
                    play_record = new PlayRecord
                    {
                        time = 10,
                        treasure_record = new List<AtgenTreasureRecord>()
                        {
                            new()
                            {
                                area_idx = 1,
                                enemy = new List<int>() { 1, 0 }
                            }
                        },
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord()
                    }
                },
                ensureSuccessHeader: false
            )
        ).data_headers.result_code.Should().Be(ResultCode.QuestStaminaSingleShort);
    }

    [Fact]
    public async Task Record_InsufficientStamina_FirstClear_ReturnsSuccess()
    {
        int questId = 100020301;
        await AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                State = 0,
                ViewerId = ViewerId
            }
        );

        this.ApiContext.PlayerUserData.ExecuteUpdate(
            p => p.SetProperty(e => e.StaminaSingle, e => 0)
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>() { }
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = key,
                    play_record = new PlayRecord
                    {
                        time = 10,
                        treasure_record = new List<AtgenTreasureRecord>()
                        {
                            new()
                            {
                                area_idx = 1,
                                enemy = new List<int>() { 1, 0 }
                            }
                        },
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord()
                    }
                },
                ensureSuccessHeader: false
            )
        ).data_headers.result_code.Should().Be(ResultCode.Success);
    }

    private async Task<string> StartDungeon(DungeonSession session) =>
        await Services.GetRequiredService<IDungeonService>().StartDungeon(session);

    private void SetupPhotonAuthentication()
    {
        Environment.SetEnvironmentVariable("PHOTON_TOKEN", "supersecrettoken");
        this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            "supersecrettoken"
        );
        this.Client.DefaultRequestHeaders.Add("Auth-ViewerId", this.ViewerId.ToString());
    }
}
