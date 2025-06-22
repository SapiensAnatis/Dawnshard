using DragaliaAPI.Features.Fort;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Friends;

internal sealed class StaticHelperDataService(
    IBonusService bonusService,
    RealHelperDataService realHelperDataService
) : IHelperDataService
{
    public Task<QuestGetSupportUserListResponse> GetHelperList(CancellationToken cancellationToken)
    {
        return Task.FromResult(SupportListData);
    }

    public Task<UserSupportList?> GetHelper(
        long helperViewerId,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(
            SupportListData.SupportUserList.FirstOrDefault(x => x.ViewerId == (ulong)helperViewerId)
        );
    }

    public async Task<AtgenSupportUserDataDetail?> GetHelperDataDetail(
        long helperViewerId,
        CancellationToken cancellationToken
    )
    {
        UserSupportList? staticHelperInfo = SupportListData.SupportUserList.FirstOrDefault(x =>
            x.ViewerId == (ulong)helperViewerId
        );

        if (staticHelperInfo is null)
        {
            // When playing co-op, if you view details for another player, the client will make a call to
            // /friend/get_support_chara_detail with their ID. If you have static helpers enabled, this will fail. We
            // should fall back to real helper data to handle this case.
            // We expect this particular case to always hit this path rather than a static helper shadowing a real
            // player, because all the static helper IDs are near ulong.MaxValue. So there is a clear separation, unless
            // we get a LOT of new players.
            //
            // Note: it shouldn't be necessary to do this for GetHelper above as that is only called from
            // /friend/id_search.
            //
            return await realHelperDataService.GetHelperDataDetail(
                helperViewerId,
                cancellationToken
            );
        }

        FortBonusList bonusList = await bonusService.GetBonusList(cancellationToken);

        return new()
        {
            UserSupportData = staticHelperInfo,
            FortBonusList = bonusList,
            ManaCirclePieceIdList = Enumerable.Range(
                1,
                staticHelperInfo.SupportChara.AdditionalMaxLevel == 20 ? 70 : 50
            ),
            DragonReliabilityLevel = 30,
            IsFriend = true,
            ApplySendStatus = 0,
        };
    }

    public Task UseHelper(long helperViewerId, CancellationToken cancellationToken)
    {
        // No-op
        return Task.CompletedTask;
    }

    private static readonly QuestGetSupportUserListResponse SupportListData = new()
    {
        SupportUserList = new List<UserSupportList>()
        {
            new()
            {
                ViewerId = long.MaxValue - 1,
                Name = "dreadfullydistinct",
                Level = 400,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
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
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 2,
                Name = "Nightmerp",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
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
                    DragonKeyId = 0,
                    DragonId = DragonId.GalaBahamut,
                    Level = 100,
                    Hp = 368,
                    Attack = 128,
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
                    TalismanKeyId = 0,
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
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 3,
                Name = "Alicia",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10150503,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.Alberius,
                    Level = 80,
                    AdditionalMaxLevel = 0,
                    Rarity = 5,
                    Hp = 752,
                    Attack = 506,
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
                    DragonKeyId = 0,
                    DragonId = DragonId.Ramiel,
                    Level = 120,
                    Hp = 388,
                    Attack = 148,
                    Skill1Level = 2,
                    Ability1Level = 6,
                    Ability2Level = 6,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 5,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.PrimalHex,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.Alberius,
                    TalismanAbilityId1 = 340000030,
                    TalismanAbilityId2 = 340000132,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.WelcometotheOpera,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.AManUnchanging,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.PrayersUntoHim,
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
                        AbilityCrestId = AbilityCrestId.ChariotDrift,
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
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 4,
                Name = "alkaemist",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10850503,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.Grace,
                    Level = 80,
                    AdditionalMaxLevel = 0,
                    Rarity = 5,
                    Hp = 804,
                    Attack = 470,
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
                    DragonKeyId = 0,
                    DragonId = DragonId.Ramiel,
                    Level = 120,
                    Hp = 388,
                    Attack = 148,
                    Skill1Level = 2,
                    Ability1Level = 6,
                    Ability2Level = 6,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 5,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.ConsumingDarkness,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.Grace,
                    TalismanAbilityId1 = 340000070,
                    TalismanAbilityId2 = 340000134,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.GentleWinds,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.TheChocolatiers,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ProperMaintenance,
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
                        AbilityCrestId = AbilityCrestId.AWidowsLament,
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
                        AbilityCrestId = AbilityCrestId.RavenousFireCrownsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.PromisedPietyStaffsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 5,
                Name = "QwerbyKing",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10850502,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.SummerVerica,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 964,
                    Attack = 563,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 3,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.Ramiel,
                    Level = 120,
                    Hp = 388,
                    Attack = 148,
                    Skill1Level = 2,
                    Ability1Level = 6,
                    Ability2Level = 6,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 5,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.ConsumingDarkness,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.SummerVerica,
                    TalismanAbilityId1 = 340000010,
                    TalismanAbilityId2 = 340000134,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.CastleCheerCorps,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.StudyRabbits,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ProperMaintenance,
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
                        AbilityCrestId = AbilityCrestId.FromWhenceHeComes,
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
                        AbilityCrestId = AbilityCrestId.RavenousFireCrownsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.PromisedPietyStaffsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 6,
                Name = "Zappypants",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10550103,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.KimonoElisanne,
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
                    DragonKeyId = 0,
                    DragonId = DragonId.Horus,
                    Level = 100,
                    Hp = 369,
                    Attack = 127,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 0,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.OmniflameLance,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.KimonoElisanne,
                    TalismanAbilityId1 = 340000010,
                    TalismanAbilityId2 = 340000134,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.CastleCheerCorps,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.StudyRabbits,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ProperMaintenance,
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
                        AbilityCrestId = AbilityCrestId.FromWhenceHeComes,
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
                        AbilityCrestId = AbilityCrestId.RavenousFireCrownsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.PromisedPietyStaffsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 7,
                Name = "stairs",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10550103,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.Elisanne,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 902,
                    Attack = 551,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 2,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.GalaBahamut,
                    Level = 100,
                    Hp = 368,
                    Attack = 128,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 5,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.LimpidCascade,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.Elisanne,
                    TalismanAbilityId1 = 340000010,
                    TalismanAbilityId2 = 340000134,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.CastleCheerCorps,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.StudyRabbits,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ProperMaintenance,
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
                        AbilityCrestId = AbilityCrestId.FromWhenceHeComes,
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
                        AbilityCrestId = AbilityCrestId.RavenousFireCrownsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.PromisedPietyStaffsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 8,
                Name = "no",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10250302,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.Tobias,
                    Level = 80,
                    AdditionalMaxLevel = 0,
                    Rarity = 5,
                    Hp = 781,
                    Attack = 494,
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
                    DragonKeyId = 0,
                    DragonId = DragonId.GalaBahamut,
                    Level = 100,
                    Hp = 368,
                    Attack = 128,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 5,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.NobleHorizon,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.Tobias,
                    TalismanAbilityId1 = 340000010,
                    TalismanAbilityId2 = 340000134,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.CastleCheerCorps,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.StudyRabbits,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ProperMaintenance,
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
                        AbilityCrestId = AbilityCrestId.FromWhenceHeComes,
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
                        AbilityCrestId = AbilityCrestId.RavenousFireCrownsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.PromisedPietyStaffsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 9,
                Name = "Euden",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10850301,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.Akasha,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 967,
                    Attack = 561,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 3,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.GalaBahamut,
                    Level = 100,
                    Hp = 368,
                    Attack = 128,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 5,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.NobleHorizon,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.Akasha,
                    TalismanAbilityId1 = 340000010,
                    TalismanAbilityId2 = 340000134,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.CastleCheerCorps,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.StudyRabbits,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ProperMaintenance,
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
                        AbilityCrestId = AbilityCrestId.FromWhenceHeComes,
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
                        AbilityCrestId = AbilityCrestId.RavenousFireCrownsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.PromisedPietyStaffsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 10,
                Name = "Euden",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10850301,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.Patia,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 961,
                    Attack = 537,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 2,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.Ramiel,
                    Level = 120,
                    Hp = 388,
                    Attack = 148,
                    Skill1Level = 2,
                    Ability1Level = 6,
                    Ability2Level = 6,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 5,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.EbonPlagueLance,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.Patia,
                    TalismanAbilityId1 = 340000010,
                    TalismanAbilityId2 = 340000134,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.CastleCheerCorps,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.StudyRabbits,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ProperMaintenance,
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
                        AbilityCrestId = AbilityCrestId.FromWhenceHeComes,
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
                        AbilityCrestId = AbilityCrestId.RavenousFireCrownsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.PromisedPietyStaffsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 11,
                Name = "Leon",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10850301,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.Delphi,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 956,
                    Attack = 572,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 3,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.Ramiel,
                    Level = 120,
                    Hp = 388,
                    Attack = 148,
                    Skill1Level = 2,
                    Ability1Level = 6,
                    Ability2Level = 6,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 5,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.ShaderulersFang,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.Delphi,
                    TalismanAbilityId1 = 340000030,
                    TalismanAbilityId2 = 340000132,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.WelcometotheOpera,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.AManUnchanging,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.WorthyRivals,
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
                        AbilityCrestId = AbilityCrestId.ChariotDrift,
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
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 12,
                Name = "Crown",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10750201,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.Lily,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 899,
                    Attack = 613,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 3,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.GalaBahamut,
                    Level = 100,
                    Hp = 368,
                    Attack = 128,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.Lily,
                    TalismanAbilityId1 = 340000030,
                    TalismanAbilityId2 = 340000132,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.WingsofRebellionatRest,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.WorthyRivals,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.SeasidePrincess,
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
                        AbilityCrestId = AbilityCrestId.ChariotDrift,
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
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 13,
                Name = "Euden",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10150303,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.GalaLeif,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 1002,
                    Attack = 546,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 3,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.GalaBahamut,
                    Level = 100,
                    Hp = 368,
                    Attack = 128,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 5,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.PrimalTempest,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.GalaLeif,
                    TalismanAbilityId1 = 340000030,
                    TalismanAbilityId2 = 340000132,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.AManUnchanging,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.GoingUndercover,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.WorthyRivals,
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
                        AbilityCrestId = AbilityCrestId.ChariotDrift,
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
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 14,
                Name = "sockperson",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10350502,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.Delphi,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 956,
                    Attack = 572,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 3,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.Ramiel,
                    Level = 120,
                    Hp = 388,
                    Attack = 148,
                    Skill1Level = 2,
                    Ability1Level = 6,
                    Ability2Level = 6,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 5,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.ShaderulersFang,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.Delphi,
                    TalismanAbilityId1 = 340000030,
                    TalismanAbilityId2 = 340000132,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.WelcometotheOpera,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.AManUnchanging,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.WorthyRivals,
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
                        AbilityCrestId = AbilityCrestId.ChariotDrift,
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
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 15,
                Name = "Delpolo",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10840402,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.Vixel,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 943,
                    Attack = 542,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 2,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.GalaElysium,
                    Level = 100,
                    Hp = 371,
                    Attack = 124,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 5,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.CosmicRuler,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.Vixel,
                    TalismanAbilityId1 = 340000010,
                    TalismanAbilityId2 = 340000134,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.CastleCheerCorps,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.StudyRabbits,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ProperMaintenance,
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
                        AbilityCrestId = AbilityCrestId.FromWhenceHeComes,
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
                        AbilityCrestId = AbilityCrestId.RavenousFireCrownsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.PromisedPietyStaffsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 16,
                Name = "Euden",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10850402,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.GalaZena,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 553,
                    Attack = 350,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 3,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.GalaElysium,
                    Level = 100,
                    Hp = 371,
                    Attack = 124,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 5,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.CosmicRuler,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.GalaZena,
                    TalismanAbilityId1 = 340000010,
                    TalismanAbilityId2 = 340000134,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.CastleCheerCorps,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.StudyRabbits,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ProperMaintenance,
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
                        AbilityCrestId = AbilityCrestId.FromWhenceHeComes,
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
                        AbilityCrestId = AbilityCrestId.RavenousFireCrownsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.PromisedPietyStaffsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 17,
                Name = "Nahxela",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10350303,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.GalaNotte,
                    Level = 80,
                    AdditionalMaxLevel = 0,
                    Rarity = 5,
                    Hp = 760,
                    Attack = 499,
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
                    DragonKeyId = 0,
                    DragonId = DragonId.GalaBahamut,
                    Level = 100,
                    Hp = 368,
                    Attack = 128,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 5,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.WindrulersFang,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.GalaNotte,
                    TalismanAbilityId1 = 340000030,
                    TalismanAbilityId2 = 340000132,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.GoingUndercover,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.AManUnchanging,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.WorthyRivals,
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
                        AbilityCrestId = AbilityCrestId.ChariotDrift,
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
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 18,
                Name = "Shiny ☆",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = (Emblems)10350303,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.GalaMym,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 898,
                    Attack = 612,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 3,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.Horus,
                    Level = 100,
                    Hp = 368,
                    Attack = 128,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 5,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.OmniflameLance,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.GalaMym,
                    TalismanAbilityId1 = 340000030,
                    TalismanAbilityId2 = 340000132,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.MeandMyBestie,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.TheCutieCompetition,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.WorthyRivals,
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
                        AbilityCrestId = AbilityCrestId.ChariotDrift,
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
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 19,
                Name = "hateklauster",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = Emblems.HarvestGoddess,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.HumanoidZodiark,
                    Level = 80,
                    AdditionalMaxLevel = 0,
                    Rarity = 5,
                    Hp = 898,
                    Attack = 612,
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
                    DragonKeyId = 0,
                    DragonId = DragonId.Ramiel,
                    Level = 120,
                    Hp = 388,
                    Attack = 148,
                    Skill1Level = 2,
                    Ability1Level = 6,
                    Ability2Level = 6,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 5,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.DuskTrigger,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.HumanoidZodiark,
                    // Crit easy
                    TalismanAbilityId1 = 340000030,
                    TalismanAbilityId2 = 340000132,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.TheHeroesArrive,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ANewLook,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.UnconditionalLove,
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
                        AbilityCrestId = AbilityCrestId.BeautifulGunman,
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
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 20,
                Name = "g.",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = Emblems.MelodysMaster,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.Melody,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 888,
                    Attack = 563,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 3,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.GalaBahamut,
                    Level = 100,
                    Hp = 368,
                    Attack = 128,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 5,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.NobleHorizon,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.GalaMym,
                    TalismanAbilityId1 = 340000030, // Critical Rate +15%
                    TalismanAbilityId2 =
                        340000132 // Easy Hitter I
                    ,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.CastleCheerCorps,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.StudyRabbits,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ProperMaintenance,
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
                        AbilityCrestId = AbilityCrestId.FromWhenceHeComes,
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
                        AbilityCrestId = AbilityCrestId.RavenousFireCrownsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.PromisedPietyStaffsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 21,
                Name = "J. R. Oppenheimer",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = Emblems.MadScientist,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.HalloweenSylas,
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
                    DragonKeyId = 0,
                    DragonId = DragonId.Ramiel,
                    Level = 100,
                    Hp = 368,
                    Attack = 128,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 5,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.NightmareProphecy,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.GalaMym,
                    TalismanAbilityId1 = 340000030, // Critical Rate +15%
                    TalismanAbilityId2 =
                        340000132 // Easy Hitter I
                    ,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.AManUnchanging,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.WelcometotheOpera,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.WorthyRivals,
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
                        AbilityCrestId = AbilityCrestId.ChariotDrift,
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
                        AbilityCrestId = AbilityCrestId.RavenousFireCrownsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.PromisedPietyStaffsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 22,
                Name = "Ms. Flashburn",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = Emblems.PureHeartedAndroid,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.HalloweenLaxi,
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
                    DragonKeyId = 0,
                    DragonId = DragonId.GalaChronosNyx,
                    Level = 100,
                    Hp = 368,
                    Attack = 128,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 5,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.DivineTrigger,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.GalaMym,
                    TalismanAbilityId1 = 340000030, // Critical Rate +15%
                    TalismanAbilityId2 =
                        340000132 // Easy Hitter I
                    ,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.AWonderfulValentines,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ARainyDay,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ExtremeTeamwork,
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
                        AbilityCrestId = AbilityCrestId.ChariotDrift,
                        BuildupCount = 40,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.BeautifulGunman,
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
                        AbilityCrestId = AbilityCrestId.AKnightsDreamAxesBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
            new()
            {
                ViewerId = long.MaxValue - 23,
                Name = "boggers",
                Level = 250,
                LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                EmblemId = Emblems.SupremeBogfish,
                MaxPartyPower = 9999,
                SupportChara = new()
                {
                    CharaId = Charas.SummerCelliera,
                    Level = 100,
                    AdditionalMaxLevel = 20,
                    Rarity = 5,
                    Hp = 956,
                    Attack = 574,
                    HpPlusCount = 100,
                    AttackPlusCount = 100,
                    Ability1Level = 3,
                    Ability2Level = 3,
                    Ability3Level = 3,
                    ExAbilityLevel = 5,
                    ExAbility2Level = 5,
                    Skill1Level = 4,
                    Skill2Level = 3,
                    IsUnlockEditSkill = true,
                },
                SupportDragon = new()
                {
                    DragonKeyId = 0,
                    DragonId = DragonId.GalaBahamut,
                    Level = 100,
                    Hp = 368,
                    Attack = 128,
                    Skill1Level = 2,
                    Ability1Level = 5,
                    Ability2Level = 5,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                    LimitBreakCount = 4,
                },
                SupportWeaponBody = new()
                {
                    WeaponBodyId = WeaponBodies.PrimalAqua,
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
                    TalismanKeyId = 0,
                    TalismanId = Talismans.GalaMym,
                    TalismanAbilityId1 = 340000030, // Critical Rate +15%
                    TalismanAbilityId2 =
                        340000132 // Easy Hitter I
                    ,
                },
                SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                {
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.BlossomsontheWatersEdge,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.ThePetalQueen,
                        BuildupCount = 50,
                        LimitBreakCount = 4,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.MoonlightParty,
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
                        AbilityCrestId = AbilityCrestId.ASmallCourage,
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
                        AbilityCrestId = AbilityCrestId.MaskofDeterminationBowsBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                    new()
                    {
                        AbilityCrestId = AbilityCrestId.AKnightsDreamAxesBoon,
                        BuildupCount = 30,
                        LimitBreakCount = 4,
                        HpPlusCount = 40,
                        AttackPlusCount = 40,
                        EquipableCount = 4,
                    },
                },
                Guild = new() { GuildId = 0, GuildName = "Guild" },
            },
        },
        SupportUserDetailList = new List<AtgenSupportUserDetailList>()
        {
            new()
            {
                ViewerId = long.MaxValue - 1,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 2,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 3,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 4,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 5,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 6,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 7,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 8,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 9,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 10,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 11,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 12,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 13,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 14,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 15,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 16,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 17,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 18,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 19,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 20,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 21,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 22,
                GettableManaPoint = 50,
                IsFriend = true,
            },
            new()
            {
                ViewerId = long.MaxValue - 23,
                GettableManaPoint = 50,
                IsFriend = true,
            },
        },
    };
}
