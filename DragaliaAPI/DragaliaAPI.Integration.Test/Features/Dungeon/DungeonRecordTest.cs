using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

[SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments")]
public class DungeonRecordTest : TestFixture
{
    public DungeonRecordTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions(2);

        this.ApiContext.PlayerUserData.ExecuteUpdate(p =>
            p.SetProperty(e => e.StaminaSingle, e => 100)
        );
        this.ApiContext.PlayerUserData.ExecuteUpdate(p =>
            p.SetProperty(e => e.StaminaMulti, e => 100)
        );

        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);
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
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    {
                        1,
                        new List<AtgenEnemy>()
                        {
                            new()
                            {
                                EnemyIdx = 0,
                                EnemyDropList = new List<EnemyDropList>()
                                {
                                    new()
                                    {
                                        Coin = 10,
                                        Mana = 10,
                                        DropList = new List<AtgenDropList>()
                                        {
                                            new()
                                            {
                                                Type = EntityTypes.Material,
                                                Id = (int)Materials.Squishums,
                                                Quantity = 1
                                            }
                                        }
                                    }
                                }
                            },
                            new()
                            {
                                EnemyIdx = 0,
                                EnemyDropList = new List<EnemyDropList>()
                                {
                                    new()
                                    {
                                        Coin = 10,
                                        Mana = 10,
                                        DropList = new List<AtgenDropList>()
                                        {
                                            new()
                                            {
                                                Type = EntityTypes.Material,
                                                Id = (int)Materials.ImitationSquish,
                                                Quantity = 1
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

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord = new List<AtgenTreasureRecord>()
                        {
                            new()
                            {
                                AreaIdx = 1,
                                Enemy = new List<int>() { 1, 0 }
                            }
                        },
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord()
                    }
                }
            )
        ).Data;

        response.IngameResultData.DungeonKey.Should().Be(key);
        response.IngameResultData.QuestId.Should().Be(227100106);

        response
            .IngameResultData.RewardRecord.DropAll.Should()
            .BeEquivalentTo(
                new List<AtgenDropAll>()
                {
                    new()
                    {
                        Type = EntityTypes.Material,
                        Id = (int)Materials.Squishums,
                        Quantity = 1
                    }
                }
            );
        response.IngameResultData.RewardRecord.TakeCoin.Should().Be(10);
        response.IngameResultData.GrowRecord.TakeMana.Should().Be(10);

        response.IngameResultData.GrowRecord.TakePlayerExp.Should().NotBe(0);

        response
            .UpdateDataList.UserData.Coin.Should()
            .BeInRange(oldUserData.Coin + 10, oldUserData.Coin + 10 + 1000); // +1000 because of temp. quest event group reward
        response.UpdateDataList.UserData.ManaPoint.Should().Be(oldUserData.ManaPoint + 10);

        response
            .UpdateDataList.MaterialList.Should()
            .Contain(x => x.MaterialId == Materials.Squishums);
        response
            .UpdateDataList.QuestList!.Should()
            .ContainEquivalentOf(
                new QuestList()
                {
                    QuestId = questId,
                    State = 3,
                    DailyPlayCount = 1,
                    PlayCount = 1,
                    WeeklyPlayCount = 1,
                    IsAppear = true,
                    IsMissionClear1 = true,
                    IsMissionClear2 = true,
                    IsMissionClear3 = true,
                    BestClearTime = 10,
                    LastDailyResetTime = DateTimeOffset.UtcNow,
                    LastWeeklyResetTime = DateTimeOffset.UtcNow
                }
            );

        response.RepeatData.Should().BeNull();
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

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>()
                {
                    new()
                    {
                        CharaId = Charas.ThePrince,
                        EquipCrestSlotType1CrestId1 = AbilityCrests.SistersDayOut
                    }
                },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord = new List<AtgenTreasureRecord>(),
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 3
                    }
                }
            )
        ).Data;

        response.IngameResultData.ScoreMissionSuccessList.Should().NotBeEmpty();
        response.IngameResultData.RewardRecord.TakeAccumulatePoint.Should().NotBe(0);
        response.IngameResultData.RewardRecord.TakeBoostAccumulatePoint.Should().NotBe(0);
    }

    [Theory]
    [InlineData(22220, 222200201, 0)] // Twinkling Twilight - Skirmish: Standard
    [InlineData(22220, 222200404, 0)] // Twinkling Twilight - All-Out Assault EX
    [InlineData(22223, 222230404, 5)] // Shadow of the Mukuroshu - Defensive Battle EX
    public async Task Record_CombatEvent_GrantsScore(int eventId, int questId, int wave)
    {
        await AddToDatabase(new DbQuest() { QuestId = questId, State = 0, });

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord = [],
                        LiveUnitNoList = [],
                        DamageRecord = [],
                        DragonDamageRecord = [],
                        Wave = wave,
                    }
                }
            )
        ).Data;

        response.IngameResultData.RewardRecord.TakeAccumulatePoint.Should().NotBe(0);
        response.IngameResultData.RewardRecord.TakeBoostAccumulatePoint.Should().Be(0);
    }

    [Fact]
    public async Task Record_Event_BossBattle_CompletesMissions()
    {
        int questId = 208450301; // The Stirring Abyss: Beginner
        int eventId = 20845; // Toll of the Deep

        await this.AddRangeToDatabase(
            [
                new DbAbilityCrest() { AbilityCrestId = AbilityCrests.HavingaSummerBall },
                new DbAbilityCrest() { AbilityCrestId = AbilityCrests.SuperSoakingAndroids }
            ]
        );

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>()
                {
                    new()
                    {
                        CharaId = Charas.ThePrince,
                        EquipCrestSlotType1CrestId1 = AbilityCrests.SuperSoakingAndroids,
                        EquipCrestSlotType2CrestId1 = AbilityCrests.HavingaSummerBall,
                    }
                },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord = new List<AtgenTreasureRecord>(),
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 3
                    }
                }
            )
        ).Data;

        response.IngameResultData.RewardRecord.TakeAccumulatePoint.Should().NotBe(0);
        response.IngameResultData.RewardRecord.TakeBoostAccumulatePoint.Should().NotBe(0);
        response.IngameResultData.ScoreMissionSuccessList.Should().NotBeEmpty();

        response
            .UpdateDataList.MissionNotice.MemoryEventMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(10220201) // Clear a Boss Battle
            .And.Contain(10220401) // Clear a "Toll of the Deep" Quest with Having a Summer Ball Equipped
            .And.Contain(10220501); // Collect 100 Oceanic Resonance in One Go
    }

    [Fact]
    public async Task Record_Event_BossBattle_UnlocksExBattle()
    {
        int questId = 208260303; // Squash the Pumpking: Expert
        int exQuestId = 208260401; // Revenge of the Pumpking
        int eventId = 20826; // Trick or Treasure

        await this.AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                PlayCount = 2, // EX quests are forcibly triggered on every third clear
            }
        );

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince, } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord = new List<AtgenTreasureRecord>(),
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 3
                    }
                }
            )
        ).Data;

        response
            .UpdateDataList.QuestList!.Should()
            .ContainEquivalentOf(
                new QuestList() { QuestId = exQuestId, IsAppear = true, },
                opts => opts.Including(x => x.QuestId).Including(x => x.IsAppear)
            );
    }

    [Fact]
    public async Task Record_Event_BossBattle_NoExBattleForEvent_DoesNotUnlockExBattle()
    {
        int questId = 208450301; // The Stirring Abyss: Beginner
        int exQuestId = 208450401; // Not a valid quest
        int eventId = 20845; // Toll of the Deep

        await this.AddToDatabase(new DbQuest() { QuestId = questId, PlayCount = 2, });

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince, } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord = new List<AtgenTreasureRecord>(),
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 3
                    }
                }
            )
        ).Data;

        response.UpdateDataList.QuestList!.Should().NotContain(x => x.QuestId == exQuestId);
    }

    [Fact]
    public async Task Record_Event_ExBossBattle_SetsIsAppearFalse()
    {
        int exQuestId = 208260401; // Revenge of the Pumpking
        int eventId = 20826; // Trick or Treasure

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince, } },
                QuestData = MasterAsset.QuestData.Get(exQuestId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord = new List<AtgenTreasureRecord>(),
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 3
                    }
                }
            )
        ).Data;

        response
            .UpdateDataList.QuestList!.Should()
            .ContainEquivalentOf(
                new QuestList() { QuestId = exQuestId, IsAppear = false, },
                opts => opts.Including(x => x.QuestId).Including(x => x.IsAppear)
            );
    }

    [Fact]
    public async Task Record_Event_ChallengeBattle_CompletesMissions()
    {
        int questId = 208450502; // Tempestuous Assault: Master
        int eventId = 20845; // Toll of the Deep

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord = new List<AtgenTreasureRecord>(),
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 5 // Final wave
                    }
                }
            )
        ).Data;

        response
            .UpdateDataList.MissionNotice.MemoryEventMissionNotice.NewCompleteMissionIdList.Should()
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
            new EarnEventEntryRequest() { EventId = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord =
                        [
                            new()
                            {
                                AreaIdx = 0,
                                Enemy = [],
                                EnemySmash = []
                            }
                        ],
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 2
                    }
                }
            )
        ).Data;

        AtgenNormalMissionNotice? missionNotice = response
            .UpdateDataList
            .MissionNotice
            ?.PeriodMissionNotice;

        missionNotice.Should().NotBeNull();
        missionNotice!.NewCompleteMissionIdList.Should().Contain(11650501); // Clear an Invasion on Standard
    }

    [Fact]
    public async Task Record_Event_Trial_CompletesMissions()
    {
        int questId = 208450702; // Wrath of Leviathan: Expert
        int eventId = 20845; // Toll of the Deep

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord = new List<AtgenTreasureRecord>(),
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 3
                    }
                }
            )
        ).Data;

        response
            .UpdateDataList.MissionNotice.MemoryEventMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(10221201); // Clear a "Toll of the Deep" Trial on Expert
    }

    [Fact]
    public async Task Record_Event_Trial_Coop_CompletesMissions()
    {
        int questId = 229030303; // The Angelic Herald: Master (Co-Op)
        int eventId = 22903; // One Starry Dragonyule

        await Client.PostMsgpack<EarnEventEntryResponse>(
            "/earn_event/entry",
            new EarnEventEntryRequest() { EventId = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord =
                        [
                            new()
                            {
                                AreaIdx = 0,
                                DropObj = [],
                                Enemy = [],
                                EnemySmash = []
                            }
                        ],
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 3
                    }
                }
            )
        ).Data;

        AtgenNormalMissionNotice? missionNotice = response
            .UpdateDataList
            .MissionNotice
            ?.PeriodMissionNotice;

        missionNotice.Should().NotBeNull();
        missionNotice!.NewCompleteMissionIdList.Should().Contain(11651001); // Clear a "One Starry Dragonyule" Trial on Master
    }

    [Fact]
    public async Task Record_EarnEvent_GrantsEnemyScore()
    {
        int questId = 229031201; // Repelling the Frosty Fiends: Standard (Solo)
        int eventId = 22903; // One Starry Dragonyule

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/earn_event/entry",
            new EarnEventEntryRequest() { EventId = eventId }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    {
                        1,

                        [
                            new()
                            {
                                ParamId = 229030211,
                                EnemyIdx = 0, // Meadow Rat (10 points)
                                EnemyDropList = []
                            },
                            new()
                            {
                                ParamId = 229030217,
                                EnemyIdx = 1, // Wind Manticore (200 points)
                                EnemyDropList = []
                            },
                            new()
                            {
                                ParamId = 229030215,
                                EnemyIdx = 2, // Arrow Raptor (40 points)
                                EnemyDropList = []
                            }
                        ]
                    }
                }
            };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord =
                        [
                            new()
                            {
                                AreaIdx = 1,
                                EnemySmash =
                                [
                                    new() { Count = 1 },
                                    new() { Count = 2 },
                                    new() { Count = 3 }
                                ]
                            }
                        ],
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 3
                    }
                }
            )
        ).Data;

        response
            .IngameResultData.RewardRecord.TakeAccumulatePoint.Should()
            .Be((10 * 1) + (200 * 2) + (40 * 3));
        response.IngameResultData.ScoringEnemyPointList.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Record_HandlesNonExistentQuestData()
    {
        int questId = 219031102;

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince, } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
                {
                    { 1, Enumerable.Empty<AtgenEnemy>() }
                }
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        DragaliaResponse<DungeonRecordRecordResponse> response =
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord = new List<AtgenTreasureRecord>(),
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 3
                    }
                }
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task RecordTimeAttack_RegistersClear()
    {
        await this.ImportSave();
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

        DungeonStartStartMultiResponse startResponse = (
            await this.Client.PostMsgpack<DungeonStartStartMultiResponse>(
                "/dungeon_start/start_multi",
                new DungeonStartStartMultiRequest()
                {
                    PartyNoList = new[] { 4 }, // Flame team
                    QuestId = questId
                }
            )
        ).Data;

        string dungeonKey = startResponse.IngameData.DungeonKey;
        float clearTime = 10.4f;

        await this.Client.PostMsgpack(
            "/dungeon_record/record_time_attack",
            new DungeonRecordRecordMultiRequest()
            {
                DungeonKey = dungeonKey,
                PlayRecord = new PlayRecord
                {
                    Time = clearTime,
                    TreasureRecord = new List<AtgenTreasureRecord>(),
                    LiveUnitNoList = new List<int>(),
                    DamageRecord = new List<AtgenDamageRecord>(),
                    DragonDamageRecord = new List<AtgenDamageRecord>(),
                    BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                    Wave = 3
                }
            }
        );

        this.ApiContext.TimeAttackClears.Should().ContainSingle(x => x.GameId == gameId);

        DbTimeAttackClear recordedClear = await this
            .ApiContext.TimeAttackClears.Include(x => x.Players)
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

        this.ApiContext.PlayerUserData.ExecuteUpdate(p =>
            p.SetProperty(e => e.StaminaSingle, e => 0)
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord = new List<AtgenTreasureRecord>()
                        {
                            new()
                            {
                                AreaIdx = 1,
                                Enemy = new List<int>() { 1, 0 }
                            }
                        },
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord()
                    }
                },
                ensureSuccessHeader: false
            )
        ).DataHeaders.ResultCode.Should().Be(ResultCode.QuestStaminaSingleShort);
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

        this.ApiContext.PlayerUserData.ExecuteUpdate(p =>
            p.SetProperty(e => e.StaminaSingle, e => 0)
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new PlayRecord
                    {
                        Time = 10,
                        TreasureRecord = new List<AtgenTreasureRecord>()
                        {
                            new()
                            {
                                AreaIdx = 1,
                                Enemy = new List<int>() { 1, 0 }
                            }
                        },
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord()
                    }
                },
                ensureSuccessHeader: false
            )
        ).DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task Record_FirstClear_GrantsFirstClearRewards()
    {
        int questId = 219011101; // Volk's Wrath: Standard (Solo)

        await this.AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                State = 0,
                PlayCount = 0,
            }
        );

        string dungeonKey = await this.StartDungeon(
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
            }
        );

        DungeonRecordRecordRequest request =
            new()
            {
                DungeonKey = dungeonKey,
                PlayRecord = new PlayRecord
                {
                    Time = 10,
                    TreasureRecord = new List<AtgenTreasureRecord>()
                    {
                        new() { AreaIdx = 1, Enemy = [] }
                    },
                    LiveUnitNoList = new List<int>(),
                    DamageRecord = [],
                    DragonDamageRecord = [],
                    BattleRoyalRecord = new AtgenBattleRoyalRecord()
                }
            };

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>("/dungeon_record/record", request)
        ).Data;

        response
            .IngameResultData.RewardRecord.FirstClearSet.Should()
            .BeEquivalentTo(
                [
                    new AtgenFirstClearSet()
                    {
                        Type = EntityTypes.Material,
                        Id = (int)Materials.DestituteOnesMaskFragment,
                        Quantity = 80
                    },
                    new AtgenFirstClearSet()
                    {
                        Type = EntityTypes.Material,
                        Id = (int)Materials.PlaguedOnesMaskFragment,
                        Quantity = 30
                    },
                    new AtgenFirstClearSet() { Type = EntityTypes.Wyrmite, Quantity = 5 }
                ]
            );

        request.DungeonKey = await this.StartDungeon(
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
            }
        );

        DungeonRecordRecordResponse response2 = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>("/dungeon_record/record", request)
        ).Data;

        response2.IngameResultData.RewardRecord.FirstClearSet.Should().BeEmpty();
    }

    [Fact]
    public async Task Record_IsCoopTutorial_AdvancesTutorialStatus()
    {
        await this
            .ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdateAsync(e =>
                e.SetProperty(p => p.TutorialStatus, TutorialService.TutorialStatusIds.CoopTutorial)
            );

        int questId = TutorialService.TutorialQuestIds.AvenueToPowerBeginner;

        await this.AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                State = 0,
                PlayCount = 0,
            }
        );

        string dungeonKey = await this.StartDungeon(
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IEnumerable<AtgenEnemy>>()
            }
        );

        DungeonRecordRecordRequest request =
            new()
            {
                DungeonKey = dungeonKey,
                PlayRecord = new PlayRecord
                {
                    Time = 10,
                    TreasureRecord = new List<AtgenTreasureRecord>()
                    {
                        new() { AreaIdx = 1, Enemy = [] }
                    },
                    LiveUnitNoList = new List<int>(),
                    DamageRecord = [],
                    DragonDamageRecord = [],
                    BattleRoyalRecord = new AtgenBattleRoyalRecord()
                }
            };

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>("/dungeon_record/record", request)
        ).Data;

        response.UpdateDataList.UserData.Should().NotBeNull();
        response.UpdateDataList.UserData.TutorialStatus.Should().Be(20501);
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
