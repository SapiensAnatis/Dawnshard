﻿using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Tutorial;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

[SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments")]
public class DungeonRecordTest : TestFixture
{
    public DungeonRecordTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
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
                ViewerId = ViewerId,
            }
        );

        DbPlayerUserData oldUserData = await ApiContext
            .PlayerUserData.AsNoTracking()
            .SingleAsync(
                x => x.ViewerId == ViewerId,
                cancellationToken: TestContext.Current.CancellationToken
            );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>()
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
                                            Quantity = 1,
                                        },
                                    },
                                },
                            },
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
                                            Quantity = 1,
                                        },
                                    },
                                },
                            },
                        },
                    }
                },
            },
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
                        TreasureRecord = new List<AtgenTreasureRecord>()
                        {
                            new()
                            {
                                AreaIdx = 1,
                                Enemy = new List<int>() { 1, 0 },
                            },
                        },
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
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
                        Quantity = 1,
                    },
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
                    LastWeeklyResetTime = DateTimeOffset.UtcNow,
                },
                opts => opts.WithDateTimeTolerance(TimeSpan.FromMinutes(5))
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
                ViewerId = ViewerId,
            }
        );

        await AddToDatabase(
            new DbAbilityCrest()
            {
                ViewerId = ViewerId,
                AbilityCrestId = AbilityCrestId.SistersDayOut,
            }
        );

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>()
            {
                new()
                {
                    CharaId = Charas.ThePrince,
                    EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut,
                },
            },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>() { { 1, [] } },
        };

        string key = await this.StartDungeon(
            new DungeonStartStartAssignUnitRequest()
            {
                RequestPartySettingList = new List<PartySettingList>()
                {
                    new()
                    {
                        CharaId = Charas.ThePrince,
                        EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut,
                    },
                },
                QuestId = questId,
            }
        );

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
                        Wave = 3,
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
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
        await AddToDatabase(new DbQuest() { QuestId = questId, State = 0 });

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>() { { 1, [] } },
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
                        TreasureRecord = [],
                        LiveUnitNoList = [],
                        DamageRecord = [],
                        DragonDamageRecord = [],
                        Wave = wave,
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
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
                new DbAbilityCrest() { AbilityCrestId = AbilityCrestId.HavingaSummerBall },
                new DbAbilityCrest() { AbilityCrestId = AbilityCrestId.SuperSoakingAndroids },
            ]
        );

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        string key = await this.StartDungeon(
            new DungeonStartStartAssignUnitRequest()
            {
                QuestId = questId,
                RequestPartySettingList = new List<PartySettingList>()
                {
                    new()
                    {
                        CharaId = Charas.ThePrince,
                        EquipCrestSlotType1CrestId1 = AbilityCrestId.SuperSoakingAndroids,
                        EquipCrestSlotType2CrestId1 = AbilityCrestId.HavingaSummerBall,
                    },
                },
            }
        );

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
                        Wave = 3,
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
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
            new MemoryEventActivateRequest() { EventId = eventId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>() { { 1, [] } },
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
                        Wave = 3,
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .UpdateDataList.QuestList!.Should()
            .ContainEquivalentOf(
                new QuestList() { QuestId = exQuestId, IsAppear = true },
                opts => opts.Including(x => x.QuestId).Including(x => x.IsAppear)
            );
    }

    [Fact]
    public async Task Record_Event_BossBattle_NoExBattleForEvent_DoesNotUnlockExBattle()
    {
        int questId = 208450301; // The Stirring Abyss: Beginner
        int exQuestId = 208450401; // Not a valid quest
        int eventId = 20845; // Toll of the Deep

        await this.AddToDatabase(new DbQuest() { QuestId = questId, PlayCount = 2 });

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>() { { 1, [] } },
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
                        Wave = 3,
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
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
            new MemoryEventActivateRequest() { EventId = eventId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(exQuestId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>() { { 1, [] } },
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
                        Wave = 3,
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .UpdateDataList.QuestList!.Should()
            .ContainEquivalentOf(
                new QuestList() { QuestId = exQuestId, IsAppear = false },
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
            new MemoryEventActivateRequest() { EventId = eventId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>() { { 1, [] } },
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
                        Wave =
                            5 // Final wave
                        ,
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .UpdateDataList.MissionNotice.MemoryEventMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(10221001) // Completely Clear a Challenge Battle on Master
            .And.Contain(10221301); // Earn the "Light of the Deep" Epithet

        // Clear Three Challenge Battles
        this.ApiContext.PlayerMissions.Where(x => x.ViewerId == this.ViewerId)
            .First(x => x.Id == 10220801)
            .Progress.Should()
            .Be(1);
    }

    [Fact]
    public async Task Record_Event_ChallengeBattle_Coop_CompletesMissions()
    {
        int questId = 229030201; // Repelling the Frosty Fiends: Standard (Co-Op)
        int eventId = 22903; // One Starry Dragonyule

        await Client.PostMsgpack(
            "/earn_event/entry",
            new EarnEventEntryRequest() { EventId = eventId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>() { { 1, [] } },
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
                                EnemySmash = [],
                            },
                        ],
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 2,
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
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
            new MemoryEventActivateRequest() { EventId = eventId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>() { { 1, [] } },
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
                        Wave = 3,
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
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
            new EarnEventEntryRequest() { EventId = eventId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>() { { 1, [] } },
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
                                EnemySmash = [],
                            },
                        ],
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 3,
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
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
            new EarnEventEntryRequest() { EventId = eventId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>()
            {
                {
                    1,
                    [
                        new()
                        {
                            ParamId = 229030211,
                            EnemyIdx = 0, // Meadow Rat (10 points)
                            EnemyDropList = [],
                        },
                        new()
                        {
                            ParamId = 229030217,
                            EnemyIdx = 1, // Wind Manticore (200 points)
                            EnemyDropList = [],
                        },
                        new()
                        {
                            ParamId = 229030215,
                            EnemyIdx = 2, // Arrow Raptor (40 points)
                            EnemyDropList = [],
                        },
                    ]
                },
            },
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
                                    new() { Count = 3 },
                                ],
                            },
                        ],
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                        Wave = 3,
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
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

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>() { { 1, [] } },
        };

        string key = await this.StartDungeon(mockSession);

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
                        Wave = 3,
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task RecordTimeAttack_RegistersClear()
    {
        await this.ImportSave();
        this.SetupPhotonAuthentication();

        int questId = 227080106; // Asura's Blinding Light (Ranked)
        string roomName = Guid.NewGuid().ToString();
        string roomId = "1234";
        string gameId = $"{roomName}_{roomId}";

        this.Client.DefaultRequestHeaders.Add("RoomName", roomName);
        this.Client.DefaultRequestHeaders.Add("RoomId", roomId);

        DungeonStartStartMultiResponse startResponse = (
            await this.Client.PostMsgpack<DungeonStartStartMultiResponse>(
                "/dungeon_start/start_multi",
                new DungeonStartStartMultiRequest()
                {
                    PartyNoList = [2], // Shadow team
                    QuestId = questId,
                },
                cancellationToken: TestContext.Current.CancellationToken
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
                    Wave = 3,
                },
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        this.ApiContext.TimeAttackClears.Should().ContainSingle(x => x.GameId == gameId);

        DbTimeAttackClear recordedClear = await this
            .ApiContext.TimeAttackClears.Include(x => x.Players)
            .ThenInclude(x => x.Units)
            .FirstAsync(
                x => x.GameId == gameId,
                cancellationToken: TestContext.Current.CancellationToken
            );

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
                ViewerId = ViewerId,
            }
        );

        this.ApiContext.PlayerUserData.ExecuteUpdate(p =>
            p.SetProperty(e => e.StaminaSingle, e => 0)
        );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>(),
        };

        string key = await this.StartDungeon(mockSession);

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
                                Enemy = new List<int>() { 1, 0 },
                            },
                        },
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                    },
                },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
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
                ViewerId = ViewerId,
            }
        );

        this.ApiContext.PlayerUserData.ExecuteUpdate(p =>
            p.SetProperty(e => e.StaminaSingle, e => 0)
        );

        DungeonSession mockSession = new()
        {
            Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new Dictionary<int, IList<AtgenEnemy>>(),
        };

        string key = await this.StartDungeon(mockSession);

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
                                Enemy = new List<int>() { 1, 0 },
                            },
                        },
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                    },
                },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
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
            new DungeonSession()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IList<AtgenEnemy>>(),
            }
        );

        DungeonRecordRecordRequest request = new()
        {
            DungeonKey = dungeonKey,
            PlayRecord = new PlayRecord
            {
                Time = 10,
                TreasureRecord = new List<AtgenTreasureRecord>()
                {
                    new() { AreaIdx = 1, Enemy = [] },
                },
                LiveUnitNoList = new List<int>(),
                DamageRecord = [],
                DragonDamageRecord = [],
                BattleRoyalRecord = new AtgenBattleRoyalRecord(),
            },
        };

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .IngameResultData.RewardRecord.FirstClearSet.Should()
            .BeEquivalentTo(
                [
                    new AtgenFirstClearSet()
                    {
                        Type = EntityTypes.Material,
                        Id = (int)Materials.DestituteOnesMaskFragment,
                        Quantity = 80,
                    },
                    new AtgenFirstClearSet()
                    {
                        Type = EntityTypes.Material,
                        Id = (int)Materials.PlaguedOnesMaskFragment,
                        Quantity = 30,
                    },
                    new AtgenFirstClearSet() { Type = EntityTypes.Wyrmite, Quantity = 5 },
                ]
            );

        request.DungeonKey = await this.StartDungeon(
            new DungeonSession()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IList<AtgenEnemy>>(),
            }
        );

        DungeonRecordRecordResponse response2 = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response2.IngameResultData.RewardRecord.FirstClearSet.Should().BeEmpty();
    }

    [Fact]
    public async Task Record_IsCoopTutorial_AdvancesTutorialStatus()
    {
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            e =>
                e.SetProperty(
                    p => p.TutorialStatus,
                    TutorialService.TutorialStatusIds.CoopTutorial
                ),
            cancellationToken: TestContext.Current.CancellationToken
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
            new DungeonSession()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IList<AtgenEnemy>>(),
            }
        );

        DungeonRecordRecordRequest request = new()
        {
            DungeonKey = dungeonKey,
            PlayRecord = new PlayRecord
            {
                Time = 10,
                TreasureRecord = new List<AtgenTreasureRecord>()
                {
                    new() { AreaIdx = 1, Enemy = [] },
                },
                LiveUnitNoList = new List<int>(),
                DamageRecord = [],
                DragonDamageRecord = [],
                BattleRoyalRecord = new AtgenBattleRoyalRecord(),
            },
        };

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.UpdateDataList.UserData.Should().NotBeNull();
        response.UpdateDataList.UserData.TutorialStatus.Should().Be(20501);
    }

    [Fact]
    public async Task Record_Multi_GrantsFirstMeetingReward()
    {
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
            new DungeonSession()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IList<AtgenEnemy>>(),
            }
        );

        DungeonRecordRecordMultiRequest request = new()
        {
            DungeonKey = dungeonKey,
            PlayRecord = new PlayRecord
            {
                Time = 10,
                TreasureRecord = new List<AtgenTreasureRecord>()
                {
                    new() { AreaIdx = 1, Enemy = [] },
                },
                LiveUnitNoList = new List<int>(),
                DamageRecord = [],
                DragonDamageRecord = [],
                BattleRoyalRecord = new AtgenBattleRoyalRecord(),
            },
            ConnectingViewerIdList = [1, 2],
        };

        DungeonRecordRecordMultiResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordMultiResponse>(
                "/dungeon_record/record_multi",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .IngameResultData.RewardRecord.FirstMeeting.Should()
            .BeEquivalentTo(
                new AtgenFirstMeeting()
                {
                    Headcount = 2,
                    Id = 0,
                    TotalQuantity = 200,
                    Type = EntityTypes.FreeDiamantium,
                }
            );
        response.UpdateDataList.PresentNotice.PresentCount.Should().Be(1);

        PresentGetPresentListResponse presentResponse = (
            await this.Client.PostMsgpack<PresentGetPresentListResponse>(
                "present/get_present_list",
                new PresentGetPresentListRequest(),
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        presentResponse
            .PresentList.Should()
            .Contain(x => x.MessageId == PresentMessage.SocialReward && x.MessageParamValue1 == 2);
    }

    [Fact]
    public async Task Record_DragonEssencesAvailable_GrantsEssences()
    {
        // Ch. 5 / 4-3 Dark Terminus (Hard)
        int questId = 100050209;
        int existingEssenceQuantity = this
            .ApiContext.PlayerMaterials.Where(x => x.ViewerId == this.ViewerId)
            .First(x => x.MaterialId == Materials.ChthoniussEssence)
            .Quantity;

        await this.AddToDatabase(new DbQuest() { QuestId = questId, DailyPlayCount = 0 });

        string dungeonKey = await this.StartDungeon(
            new DungeonSession()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IList<AtgenEnemy>>(),
            }
        );

        DungeonRecordRecordRequest request = new()
        {
            DungeonKey = dungeonKey,
            PlayRecord = new PlayRecord
            {
                Time = 10,
                TreasureRecord = new List<AtgenTreasureRecord>()
                {
                    new() { AreaIdx = 1, Enemy = [] },
                },
                LiveUnitNoList = new List<int>(),
                DamageRecord = [],
                DragonDamageRecord = [],
                BattleRoyalRecord = new AtgenBattleRoyalRecord(),
            },
        };

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.UpdateDataList.MaterialList.Should().NotBeNull();
        response
            .UpdateDataList.MaterialList.Should()
            .Contain(x => x.MaterialId == Materials.ChthoniussEssence)
            .Which.Quantity.Should()
            .Be(existingEssenceQuantity + 1);
    }

    [Fact]
    public async Task Record_DragonEssencesNotAvailable_StopsGivingEssence()
    {
        // Ch. 5 / 4-3 Dark Terminus (Hard)
        int questId = 100050209;
        int existingEssenceQuantity = this
            .ApiContext.PlayerMaterials.Where(x => x.ViewerId == this.ViewerId)
            .First(x => x.MaterialId == Materials.ChthoniussEssence)
            .Quantity;

        await this.AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                DailyPlayCount = 2,
                LastDailyResetTime = DateTimeOffset.UtcNow,
            }
        );

        string dungeonKey = await this.StartDungeon(
            new DungeonSession()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IList<AtgenEnemy>>(),
            }
        );

        DungeonRecordRecordRequest request = new()
        {
            DungeonKey = dungeonKey,
            PlayRecord = new PlayRecord
            {
                Time = 10,
                TreasureRecord = new List<AtgenTreasureRecord>()
                {
                    new() { AreaIdx = 1, Enemy = [] },
                },
                LiveUnitNoList = new List<int>(),
                DamageRecord = [],
                DragonDamageRecord = [],
                BattleRoyalRecord = new AtgenBattleRoyalRecord(),
            },
        };

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.UpdateDataList.MaterialList.Should().NotBeNull();
        response
            .UpdateDataList.MaterialList.Should()
            .Contain(x => x.MaterialId == Materials.ChthoniussEssence)
            .Which.Quantity.Should()
            .Be(existingEssenceQuantity + 1);

        request.DungeonKey = await this.StartDungeon(
            new DungeonSession()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IList<AtgenEnemy>>(),
            }
        );

        DungeonRecordRecordResponse secondResponse = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        secondResponse.UpdateDataList.MaterialList.Should().BeNull();
    }

    [Fact]
    public async Task Record_DragonEssencesEarnedBeforeReset_ResetsAndGrantsEssence()
    {
        // Ch. 5 / 4-3 Dark Terminus (Hard)
        int questId = 100050209;
        int existingEssenceQuantity = this
            .ApiContext.PlayerMaterials.Where(x => x.ViewerId == this.ViewerId)
            .First(x => x.MaterialId == Materials.ChthoniussEssence)
            .Quantity;

        await this.AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                DailyPlayCount = 3,
                LastDailyResetTime = DateTimeOffset.UtcNow.AddDays(-2),
            }
        );

        string dungeonKey = await this.StartDungeon(
            new DungeonSession()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IList<AtgenEnemy>>(),
            }
        );

        DungeonRecordRecordRequest request = new()
        {
            DungeonKey = dungeonKey,
            PlayRecord = new PlayRecord
            {
                Time = 10,
                TreasureRecord = new List<AtgenTreasureRecord>()
                {
                    new() { AreaIdx = 1, Enemy = [] },
                },
                LiveUnitNoList = new List<int>(),
                DamageRecord = [],
                DragonDamageRecord = [],
                BattleRoyalRecord = new AtgenBattleRoyalRecord(),
            },
        };

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.UpdateDataList.MaterialList.Should().NotBeNull();
        response
            .UpdateDataList.MaterialList.Should()
            .Contain(x => x.MaterialId == Materials.ChthoniussEssence)
            .Which.Quantity.Should()
            .Be(existingEssenceQuantity + 1);
    }

    [Fact]
    public async Task Record_Fafnirs_BoostsRupiesAndMana()
    {
        int avenueToFortuneQuestId = 202060104;

        DbPlayerDragonData goldFafnir = new() { DragonId = DragonId.GoldFafnir, Ability1Level = 5 };
        DbPlayerDragonData silverFafnir = new()
        {
            DragonId = DragonId.SilverFafnir,
            Ability1Level = 5,
        };

        await AddRangeToDatabase(
            [
                new DbQuest()
                {
                    QuestId = avenueToFortuneQuestId,
                    State = 0,
                    ViewerId = ViewerId,
                },
                goldFafnir,
                silverFafnir,
            ]
        );

        DragaliaResponse<DungeonStartStartAssignUnitResponse> startResponse =
            await this.Client.PostMsgpack<DungeonStartStartAssignUnitResponse>(
                "/dungeon_start/start_assign_unit",
                new DungeonStartStartAssignUnitRequest()
                {
                    SupportViewerId = 0,
                    RequestPartySettingList = new List<PartySettingList>()
                    {
                        new()
                        {
                            CharaId = Charas.ThePrince,
                            EquipDragonKeyId = (ulong)goldFafnir.DragonKeyId,
                        },
                        new()
                        {
                            CharaId = Charas.Marty,
                            EquipDragonKeyId = (ulong)silverFafnir.DragonKeyId,
                        },
                    },
                    QuestId = avenueToFortuneQuestId,
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        string key = startResponse.Data.IngameData.DungeonKey;

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
                                AreaIdx = 0,
                                Enemy = Enumerable.Repeat(
                                    1,
                                    startResponse.Data.OddsInfo.Enemy.Count
                                ),
                            },
                        },
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord(),
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        int rupiesFromEnemies = startResponse
            .Data.OddsInfo.Enemy.SelectMany(x => x.EnemyDropList)
            .Sum(x => x.Coin);
        int manaFromEnemies = startResponse
            .Data.OddsInfo.Enemy.SelectMany(x => x.EnemyDropList)
            .Sum(x => x.Mana);

        response
            .IngameResultData.RewardRecord.TakeCoin.Should()
            .Be((int)Math.Round(rupiesFromEnemies * 1.5));
        response
            .IngameResultData.GrowRecord.TakeMana.Should()
            .Be((int)Math.Round(manaFromEnemies * 1.5));
    }

    private async Task<string> StartDungeon(DungeonSession session)
    {
        string key = this.DungeonService.CreateSession(session);
        await this.DungeonService.SaveSession(CancellationToken.None);

        return key;
    }

    private async Task<string> StartDungeon(DungeonStartStartAssignUnitRequest request)
    {
        DragaliaResponse<DungeonStartStartAssignUnitResponse> response =
            await this.Client.PostMsgpack<DungeonStartStartAssignUnitResponse>(
                "/dungeon_start/start_assign_unit",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            );

        return response.Data.IngameData.DungeonKey;
    }

    private void SetupPhotonAuthentication()
    {
        this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            "supersecrettoken"
        );
        this.Client.DefaultRequestHeaders.Add("Auth-ViewerId", this.ViewerId.ToString());
    }
}
