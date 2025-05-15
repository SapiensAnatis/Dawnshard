using DragaliaAPI.Database.Entities;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Shared.Options;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Integration.Test.Features.Friends;

public class HelperTest : TestFixture
{
    public HelperTest(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper) { }

    [Fact]
    public async Task GetSupportUserList_ReturnsList()
    {
        DbPlayer nonFriend = await this.AddNonFriendHelper();
        DbPlayer friend = await this.AddFriendHelper();

        DragaliaResponse<QuestGetSupportUserListResponse> response =
            await this.Client.PostMsgpack<QuestGetSupportUserListResponse>(
                "/quest/get_support_user_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.SupportUserList.Where(x =>
                // We don't reset the database between tests and rely on each test running under a new viewer ID
                // however, helpers are global data
                x.ViewerId == (ulong)nonFriend.ViewerId
                || x.ViewerId == (ulong)friend.ViewerId
            )
            .Should()
            .BeEquivalentTo(
                new List<UserSupportList>()
                {
                    new()
                    {
                        ViewerId = (ulong)nonFriend.ViewerId,
                        Name = "dreadfully",
                        Level = 50,
                        LastLoginDate = nonFriend.UserData!.LastLoginTime,
                        EmblemId = Emblems.TraitorousPrince,
                        MaxPartyPower = 9999,
                        SupportChara = new()
                        {
                            CharaId = Charas.DragonyuleIlia,
                            Level = 10,
                            AdditionalMaxLevel = 0,
                            Rarity = 5,
                            Hp = 60,
                            Attack = 40,
                            HpPlusCount = 0,
                            AttackPlusCount = 0,
                            Ability1Level = 0,
                            Ability2Level = 0,
                            Ability3Level = 0,
                            ExAbilityLevel = 1,
                            ExAbility2Level = 1,
                            Skill1Level = 1,
                            Skill2Level = 0,
                            IsUnlockEditSkill = true,
                        },
                        SupportDragon = new() { DragonKeyId = 0 },
                        SupportWeaponBody = new() { WeaponBodyId = 0 },
                        SupportTalisman = new() { TalismanKeyId = 0 },
                        SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new() { AbilityCrestId = 0 },
                            new() { AbilityCrestId = 0 },
                            new() { AbilityCrestId = 0 },
                        },
                        SupportCrestSlotType2List = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new() { AbilityCrestId = 0 },
                            new() { AbilityCrestId = 0 },
                        },
                        SupportCrestSlotType3List = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new() { AbilityCrestId = 0 },
                            new() { AbilityCrestId = 0 },
                        },
                        Guild = new() { GuildId = 0 },
                    },
                    new()
                    {
                        ViewerId = (ulong)friend.ViewerId,
                        Name = "Nightmerp",
                        Level = 250,
                        LastLoginDate = friend.UserData!.LastLoginTime,
                        EmblemId = (Emblems)10250305,
                        MaxPartyPower = 9999,
                        SupportChara = new()
                        {
                            CharaId = Charas.GalaEmile,
                            Level = 80,
                            AdditionalMaxLevel = 0,
                            Rarity = 5,
                            Hp = 789,
                            Attack = 486,
                            HpPlusCount = 100,
                            AttackPlusCount = 100,
                            Ability1Level = 2,
                            Ability2Level = 2,
                            Ability3Level = 2,
                            ExAbilityLevel = 5,
                            ExAbility2Level = 5,
                            Skill1Level = 3,
                            Skill2Level = 2,
                            IsUnlockEditSkill = true,
                        },
                        SupportDragon = new()
                        {
                            DragonKeyId = (ulong)friend.Helper!.EquipDragonKeyId!,
                            DragonId = DragonId.GalaBahamut,
                            Level = 100,
                            Hp = 0,
                            Attack = 0,
                            Skill1Level = 2,
                            Ability1Level = 5,
                            Ability2Level = 5,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            LimitBreakCount = 4,
                        },
                        SupportWeaponBody = new()
                        {
                            WeaponBodyId = WeaponBodies.AqueousPrison,
                            BuildupCount = 80,
                            LimitBreakCount = 8,
                            LimitOverCount = 1,
                            EquipableCount = 4,
                            AdditionalCrestSlotType1Count = 1,
                            AdditionalCrestSlotType2Count = 0,
                            AdditionalCrestSlotType3Count = 2,
                        },
                        SupportTalisman = new()
                        {
                            TalismanKeyId = (ulong)friend.Helper.EquipTalismanKeyId!,
                            TalismanId = Talismans.GalaEmile,
                            AdditionalAttack = 100,
                            AdditionalHp = 100,
                        },
                        SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                AbilityCrestId = AbilityCrestId.ARoyalTeaParty,
                                BuildupCount = 50,
                                LimitBreakCount = 4,
                                HpPlusCount = 50,
                                AttackPlusCount = 50,
                                EquipableCount = 4,
                            },
                            new()
                            {
                                AbilityCrestId = AbilityCrestId.QueenoftheBlueSeas,
                                BuildupCount = 50,
                                LimitBreakCount = 4,
                                HpPlusCount = 50,
                                AttackPlusCount = 50,
                                EquipableCount = 4,
                            },
                            new()
                            {
                                AbilityCrestId = AbilityCrestId.PeacefulWaterfront,
                                BuildupCount = 50,
                                LimitBreakCount = 4,
                                HpPlusCount = 50,
                                AttackPlusCount = 50,
                                EquipableCount = 4,
                            },
                        },
                        SupportCrestSlotType2List = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                AbilityCrestId = AbilityCrestId.HisCleverBrother,
                                BuildupCount = 40,
                                LimitBreakCount = 4,
                                HpPlusCount = 50,
                                AttackPlusCount = 50,
                                EquipableCount = 4,
                            },
                            new()
                            {
                                AbilityCrestId = AbilityCrestId.DragonsNest,
                                BuildupCount = 20,
                                LimitBreakCount = 4,
                                HpPlusCount = 50,
                                AttackPlusCount = 50,
                                EquipableCount = 4,
                            },
                        },
                        SupportCrestSlotType3List = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                AbilityCrestId = AbilityCrestId.TutelarysDestinyWolfsBoon,
                                BuildupCount = 30,
                                LimitBreakCount = 4,
                                HpPlusCount = 40,
                                AttackPlusCount = 40,
                                EquipableCount = 4,
                            },
                            new()
                            {
                                AbilityCrestId = AbilityCrestId.CrownofLightSerpentsBoon,
                                BuildupCount = 30,
                                LimitBreakCount = 4,
                                HpPlusCount = 40,
                                AttackPlusCount = 40,
                                EquipableCount = 4,
                            },
                        },
                        Guild = new() { GuildId = 0 },
                    },
                },
                opts => opts.WithDateTimeTolerance()
            );

        response
            .Data.SupportUserDetailList.Where(x =>
                x.ViewerId == (ulong)nonFriend.ViewerId || x.ViewerId == (ulong)friend.ViewerId
            )
            .Should()
            .BeEquivalentTo(
                new List<AtgenSupportUserDetailList>()
                {
                    new()
                    {
                        ViewerId = (ulong)nonFriend.ViewerId,
                        IsFriend = false,
                        GettableManaPoint = 25,
                    },
                    new()
                    {
                        ViewerId = (ulong)friend.ViewerId,
                        IsFriend = true,
                        GettableManaPoint = 50,
                    },
                }
            );
    }

    [Fact]
    public async Task GetSupportUserList_LegacyMode_ReturnsStaticList()
    {
        this.ApiContext.PlayerSettings.Add(
            new()
            {
                ViewerId = this.ViewerId,
                SettingsJson = new() { UseLegacyHelpers = true },
            }
        );
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DragaliaResponse<QuestGetSupportUserListResponse> response =
            await this.Client.PostMsgpack<QuestGetSupportUserListResponse>(
                "/quest/get_support_user_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.SupportUserList.Should()
            .AllSatisfy((helper) => helper.ViewerId.Should().BeGreaterThan(long.MaxValue - 100));
        response.Data.SupportUserDetailList.Should().HaveCount(23);

        response.Data.SupportUserDetailList.Should().AllSatisfy(x => x.IsFriend.Should().BeTrue());
        response.Data.SupportUserDetailList.Should().HaveCount(23);
    }

    [Fact]
    public async Task UseHelper_ReturnsCorrectRecordResponse()
    {
        DbPlayer nonFriend = await this.AddNonFriendHelper();

        DungeonRecordRecordResponse resp = await ClearDungeonWithHelper(nonFriend.ViewerId);

        resp.IngameResultData.HelperDetailList.Should()
            .BeEquivalentTo(
                new List<AtgenHelperDetailList>()
                {
                    new()
                    {
                        ViewerId = (ulong)nonFriend.ViewerId,
                        IsFriend = false,
                        ApplySendStatus = 0,
                        GetManaPoint = 25,
                    },
                }
            );
    }

    [Fact]
    public async Task UseHelper_FriendRequestSent_ReturnsCorrectRecordResponse()
    {
        DbPlayer nonFriend = await this.AddNonFriendHelper();

        this.ApiContext.PlayerFriendRequests.Add(
            new() { FromPlayerViewerId = this.ViewerId, ToPlayer = nonFriend }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DungeonRecordRecordResponse resp = await ClearDungeonWithHelper(nonFriend.ViewerId);

        resp.IngameResultData.HelperDetailList.Should()
            .BeEquivalentTo(
                new List<AtgenHelperDetailList>()
                {
                    new()
                    {
                        ViewerId = (ulong)nonFriend.ViewerId,
                        IsFriend = false,
                        ApplySendStatus = 1,
                        GetManaPoint = 25,
                    },
                }
            );
    }

    [Fact]
    public async Task UseHelper_CannotUseAgain()
    {
        DbPlayer nonFriend = await this.AddNonFriendHelper();

        _ = await ClearDungeonWithHelper(nonFriend.ViewerId);

        DragaliaResponse<QuestGetSupportUserListResponse> listResponse =
            await this.Client.PostMsgpack<QuestGetSupportUserListResponse>(
                "/quest/get_support_user_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        listResponse
            .Data.SupportUserList.Should()
            .NotContain(x => x.ViewerId == (ulong)nonFriend.ViewerId);
        listResponse
            .Data.SupportUserDetailList.Should()
            .NotContain(x => x.ViewerId == (ulong)nonFriend.ViewerId);
    }

    [Fact]
    public async Task UseHelper_ResetPasses_CanUseAgain()
    {
        DbPlayer nonFriend = await this.AddNonFriendHelper();

        _ = await ClearDungeonWithHelper(nonFriend.ViewerId);

        DragaliaResponse<QuestGetSupportUserListResponse> listResponse =
            await this.Client.PostMsgpack<QuestGetSupportUserListResponse>(
                "/quest/get_support_user_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        listResponse
            .Data.SupportUserList.Should()
            .NotContain(x => x.ViewerId == (ulong)nonFriend.ViewerId);
        listResponse
            .Data.SupportUserDetailList.Should()
            .NotContain(x => x.ViewerId == (ulong)nonFriend.ViewerId);

        this.MockTimeProvider.SetUtcNow(this.MockTimeProvider.GetLastDailyReset().AddDays(1));

        DragaliaResponse<QuestGetSupportUserListResponse> listResponse2 =
            await this.Client.PostMsgpack<QuestGetSupportUserListResponse>(
                "/quest/get_support_user_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        listResponse2
            .Data.SupportUserList.Should()
            .Contain(x => x.ViewerId == (ulong)nonFriend.ViewerId);
        listResponse2
            .Data.SupportUserDetailList.Should()
            .Contain(x => x.ViewerId == (ulong)nonFriend.ViewerId);
    }

    [Fact]
    public async Task UseHelper_OtherPlayerClearsQuest_CanUseAgain()
    {
        DbPlayer nonFriend = await this.AddNonFriendHelper();

        _ = await ClearDungeonWithHelper(nonFriend.ViewerId);

        DragaliaResponse<QuestGetSupportUserListResponse> listResponse =
            await this.Client.PostMsgpack<QuestGetSupportUserListResponse>(
                "/quest/get_support_user_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        listResponse
            .Data.SupportUserList.Should()
            .NotContain(x => x.ViewerId == (ulong)nonFriend.ViewerId);
        listResponse
            .Data.SupportUserDetailList.Should()
            .NotContain(x => x.ViewerId == (ulong)nonFriend.ViewerId);

        this.MockTimeProvider.Advance(TimeSpan.FromMinutes(1));

        _ = await ClearDungeonAsOtherPlayer(nonFriend);

        DragaliaResponse<QuestGetSupportUserListResponse> listResponse2 =
            await this.Client.PostMsgpack<QuestGetSupportUserListResponse>(
                "/quest/get_support_user_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        listResponse2
            .Data.SupportUserList.Should()
            .Contain(x => x.ViewerId == (ulong)nonFriend.ViewerId);
        listResponse2
            .Data.SupportUserDetailList.Should()
            .Contain(x => x.ViewerId == (ulong)nonFriend.ViewerId);
    }

    private async Task<DbPlayer> AddNonFriendHelper()
    {
        DbPlayer player = new()
        {
            AccountId = "helper1",
            UserData = new()
            {
                Name = "dreadfully",
                LastLoginTime = DateTimeOffset.UtcNow,
                Level = 50,
                EmblemId = Emblems.TraitorousPrince,
            },
            PartyPower = new() { MaxPartyPower = 9999 },
        };

        player.Helper = new()
        {
            EquippedChara = new()
            {
                CharaId = Charas.DragonyuleIlia,
                Level = 10,
                Rarity = 5,
                HpBase = 60,
                AttackBase = 40,
                HpPlusCount = 0,
                AttackPlusCount = 0,
                Ability1Level = 0,
                Ability2Level = 0,
                Ability3Level = 0,
                ExAbilityLevel = 1,
                ExAbility2Level = 1,
                Skill1Level = 1,
                Skill2Level = 0,
                IsUnlockEditSkill = true,
                Owner = player,
            },
        };

        this.ApiContext.Players.Add(player);

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return player;
    }

    private async Task<DbPlayer> AddFriendHelper()
    {
        DbPlayer player = new()
        {
            AccountId = "helper2",
            UserData = new()
            {
                Name = "Nightmerp",
                LastLoginTime = DateTimeOffset.UtcNow,
                Level = 250,
                EmblemId = (Emblems)10250305,
            },
            PartyPower = new() { MaxPartyPower = 9999 },
        };

        player.Helper = new()
        {
            EquippedChara = new()
            {
                CharaId = Charas.GalaEmile,
                Level = 80,
                Rarity = 5,
                HpBase = 789,
                AttackBase = 486,
                HpPlusCount = 100,
                AttackPlusCount = 100,
                Ability1Level = 2,
                Ability2Level = 2,
                Ability3Level = 2,
                ExAbilityLevel = 5,
                ExAbility2Level = 5,
                Skill1Level = 3,
                Skill2Level = 2,
                IsUnlockEditSkill = true,
                Owner = player,
            },
            EquippedDragon = new()
            {
                DragonKeyId = 0,
                DragonId = DragonId.GalaBahamut,
                Level = 100,
                Skill1Level = 2,
                Ability1Level = 5,
                Ability2Level = 5,
                HpPlusCount = 50,
                AttackPlusCount = 50,
                LimitBreakCount = 4,
                Owner = player,
            },
            EquippedWeaponBody = new()
            {
                WeaponBodyId = WeaponBodies.AqueousPrison,
                BuildupCount = 80,
                LimitBreakCount = 8,
                LimitOverCount = 1,
                EquipableCount = 4,
                AdditionalCrestSlotType1Count = 1,
                AdditionalCrestSlotType2Count = 0,
                AdditionalCrestSlotType3Count = 2,
                Owner = player,
            },
            EquippedTalisman = new()
            {
                TalismanKeyId = 0,
                TalismanId = Talismans.GalaEmile,
                AdditionalAttack = 100,
                AdditionalHp = 100,
                Owner = player,
            },
            EquippedCrestSlotType1Crest1 = new()
            {
                AbilityCrestId = AbilityCrestId.ARoyalTeaParty,
                BuildupCount = 50,
                LimitBreakCount = 4,
                HpPlusCount = 50,
                AttackPlusCount = 50,
                EquipableCount = 4,
                Owner = player,
            },
            EquippedCrestSlotType1Crest2 = new()
            {
                AbilityCrestId = AbilityCrestId.QueenoftheBlueSeas,
                BuildupCount = 50,
                LimitBreakCount = 4,
                HpPlusCount = 50,
                AttackPlusCount = 50,
                EquipableCount = 4,
                Owner = player,
            },
            EquippedCrestSlotType1Crest3 = new()
            {
                AbilityCrestId = AbilityCrestId.PeacefulWaterfront,
                BuildupCount = 50,
                LimitBreakCount = 4,
                HpPlusCount = 50,
                AttackPlusCount = 50,
                EquipableCount = 4,
                Owner = player,
            },
            EquippedCrestSlotType2Crest1 = new()
            {
                AbilityCrestId = AbilityCrestId.HisCleverBrother,
                BuildupCount = 40,
                LimitBreakCount = 4,
                HpPlusCount = 50,
                AttackPlusCount = 50,
                EquipableCount = 4,
                Owner = player,
            },
            EquippedCrestSlotType2Crest2 = new()
            {
                AbilityCrestId = AbilityCrestId.DragonsNest,
                BuildupCount = 20,
                LimitBreakCount = 4,
                HpPlusCount = 50,
                AttackPlusCount = 50,
                EquipableCount = 4,
                Owner = player,
            },
            EquippedCrestSlotType3Crest1 = new()
            {
                AbilityCrestId = AbilityCrestId.TutelarysDestinyWolfsBoon,
                BuildupCount = 30,
                LimitBreakCount = 4,
                HpPlusCount = 40,
                AttackPlusCount = 40,
                EquipableCount = 4,
                Owner = player,
            },
            EquippedCrestSlotType3Crest2 = new()
            {
                AbilityCrestId = AbilityCrestId.CrownofLightSerpentsBoon,
                BuildupCount = 30,
                LimitBreakCount = 4,
                HpPlusCount = 40,
                AttackPlusCount = 40,
                EquipableCount = 4,
                Owner = player,
            },
        };

        this.ApiContext.Players.Add(player);

        this.ApiContext.PlayerFriendships.Add(
            new()
            {
                PlayerFriendshipPlayers =
                [
                    new() { PlayerViewerId = this.ViewerId },
                    new() { Player = player },
                ],
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return player;
    }

    private async Task<DungeonRecordRecordResponse> ClearDungeonWithHelper(long helperViewerId)
    {
        // Ch. 5 / 4-3 Dark Terminus (Hard)
        int questId = 100050209;

        string dungeonKey = this.DungeonService.CreateSession(
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IList<AtgenEnemy>>(),
                SupportViewerId = (ulong)helperViewerId,
            }
        );

        await this.DungeonService.SaveSession(CancellationToken.None);

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

        return response;
    }

    private async Task<DungeonRecordRecordResponse> ClearDungeonAsOtherPlayer(DbPlayer player)
    {
        // Ch. 5 / 4-3 Dark Terminus (Hard)
        int questId = 100050209;

        this.ApiContext.PlayerQuests.Add(new() { ViewerId = player.ViewerId, QuestId = questId });
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        HttpClient client = this.CreateClientForOtherPlayer(player);

        IPlayerIdentityService stubPlayerIdentityService = new StubPlayerIdentityService(
            player.ViewerId
        );

        DungeonService dungeonService = new(
            this.Services.GetRequiredService<IDistributedCache>(),
            this.Services.GetRequiredService<IOptionsMonitor<RedisCachingOptions>>(),
            stubPlayerIdentityService,
            NullLogger<DungeonService>.Instance
        );

        string dungeonKey = dungeonService.CreateSession(
            new()
            {
                Party = new List<PartySettingList>()
                {
                    new() { CharaId = player.CharaList.First().CharaId },
                },
                QuestData = MasterAsset.QuestData.Get(questId),
                EnemyList = new Dictionary<int, IList<AtgenEnemy>>(),
            }
        );

        await dungeonService.SaveSession(CancellationToken.None);

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
            await client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        return response;
    }
}
