using System.Net.Http.Headers;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
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
                DeviceAccountId = DeviceAccountId
            }
        );

        DbPlayerUserData oldUserData = await ApiContext.PlayerUserData
            .AsNoTracking()
            .SingleAsync(x => x.DeviceAccountId == DeviceAccountId);

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

        response.ingame_result_data.reward_record.drop_all
            .Should()
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

        response.update_data_list.user_data.coin
            .Should()
            .BeInRange(oldUserData.Coin + 10, oldUserData.Coin + 10 + 1000); // +1000 because of temp. quest event group reward
        response.update_data_list.user_data.mana_point.Should().Be(oldUserData.ManaPoint + 10);

        response.update_data_list.material_list
            .Should()
            .Contain(x => x.material_id == Materials.Squishums);
        response.update_data_list.quest_list
            .Should()
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
    }

    [Fact]
    public async Task Record_Event_UsesMultiplierAndCompletesMissions()
    {
        int eventId = 20816;
        int questId = 208160502; // Flames of Reflection -- The Path To Mastery: Master

        await AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                State = 0,
                DeviceAccountId = DeviceAccountId
            }
        );

        await AddToDatabase(
            new DbAbilityCrest()
            {
                DeviceAccountId = DeviceAccountId,
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
    public async Task Record_HandlesNonExistentQuestData()
    {
        int questId = 219031102;

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
                DeviceAccountId = DeviceAccountId
            }
        );

        this.MockPhotonStateApi
            .Setup(x => x.GetGameByViewerId(this.ViewerId))
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

        DbTimeAttackClear? recordedClear = await this.ApiContext.TimeAttackClears
            .Include(x => x.Players)
            .ThenInclude(x => x.Units)
            .FirstAsync(x => x.GameId == gameId);

        recordedClear.Time.Should().Be(clearTime);
        recordedClear.QuestId.Should().Be(questId);
        recordedClear.Players.Should().ContainSingle(x => x.DeviceAccountId == DeviceAccountId);

        DbTimeAttackPlayer player = recordedClear.Players.First(
            x => x.DeviceAccountId == DeviceAccountId
        );

        player.PartyInfo.Should().NotBeNullOrEmpty();
        player.Units.Should().HaveCount(4);
    }

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
