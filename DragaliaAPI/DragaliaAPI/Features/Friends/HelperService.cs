using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Friends;

internal sealed partial class HelperService(
    IPartyRepository partyRepository,
    IDungeonRepository dungeonRepository,
    IUserDataRepository userDataRepository,
    IMapper mapper,
    IBonusService bonusService,
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    ILogger<HelperService> logger
) : IHelperService
{
    public async Task<QuestGetSupportUserListResponse> GetHelpers()
    {
        // TODO: Make this actually pull from database
        await Task.CompletedTask;

        return StubData.SupportListData;
    }

    public async Task<UserSupportList?> GetLegacyHelper(ulong viewerId)
    {
        UserSupportList? helper = (await this.GetHelpers()).SupportUserList.FirstOrDefault(x =>
            x.ViewerId == viewerId
        );

        logger.LogDebug("Retrieved support list {@helper}", helper);

        return helper;
    }

    public async Task<UserSupportList> GetLeadUnit(int partyNo)
    {
        DbPlayerUserData userData = await userDataRepository.GetUserDataAsync();

        IQueryable<DbPartyUnit> leadUnitQuery = partyRepository.GetPartyUnits(partyNo).Take(1);
        DbDetailedPartyUnit? detailedUnit = await dungeonRepository
            .BuildDetailedPartyUnit(leadUnitQuery, 0)
            .FirstAsync();

        UserSupportList supportList = new()
        {
            ViewerId = (ulong)userData.ViewerId,
            Name = userData.Name,
            LastLoginDate = userData.LastLoginTime,
            Level = userData.Level,
            EmblemId = userData.EmblemId,
            MaxPartyPower = 1000,
            Guild = new() { GuildId = 0 },
        };

        mapper.Map(detailedUnit, supportList);

        supportList.SupportCrestSlotType1List = supportList.SupportCrestSlotType1List.Where(x =>
            x != null
        );
        supportList.SupportCrestSlotType2List = supportList.SupportCrestSlotType2List.Where(x =>
            x != null
        );
        supportList.SupportCrestSlotType3List = supportList.SupportCrestSlotType3List.Where(x =>
            x != null
        );

        return supportList;
    }

    public AtgenSupportData BuildHelperData(
        UserSupportList helperInfo,
        AtgenSupportUserDetailList helperDetails
    )
    {
        return new AtgenSupportData()
        {
            ViewerId = helperInfo.ViewerId,
            Name = helperInfo.Name,
            IsFriend = helperDetails.IsFriend,
            CharaData = mapper.Map<CharaList>(helperInfo.SupportChara),
            DragonData = mapper.Map<DragonList>(helperInfo.SupportDragon),
            WeaponBodyData = mapper.Map<GameWeaponBody>(helperInfo.SupportWeaponBody),
            CrestSlotType1CrestList = helperInfo.SupportCrestSlotType1List.Select(
                mapper.Map<GameAbilityCrest>
            ),
            CrestSlotType2CrestList = helperInfo.SupportCrestSlotType2List.Select(
                mapper.Map<GameAbilityCrest>
            ),
            CrestSlotType3CrestList = helperInfo.SupportCrestSlotType3List.Select(
                mapper.Map<GameAbilityCrest>
            ),
            TalismanData = mapper.Map<TalismanList>(helperInfo.SupportTalisman),
        };
    }

    public async Task<AtgenSupportUserDataDetail> BuildStaticSupportUserDataDetail(
        UserSupportList staticHelperInfo
    )
    {
        AtgenSupportUserDetailList helperDetail = new()
        {
            IsFriend = true,
            ViewerId = staticHelperInfo.ViewerId,
            GettableManaPoint = 50,
        };

        FortBonusList bonusList = await bonusService.GetBonusList();

        return new()
        {
            UserSupportData = staticHelperInfo,
            FortBonusList = bonusList,
            ManaCirclePieceIdList = Enumerable.Range(
                1,
                staticHelperInfo?.SupportChara.AdditionalMaxLevel == 20 ? 70 : 50
            ),
            DragonReliabilityLevel = 30,
            IsFriend = helperDetail.IsFriend,
            ApplySendStatus = 0,
        };
    }

    internal static class StubData
    {
        public static readonly QuestGetSupportUserListResponse SupportListData = new()
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
                    ViewerId = long.MaxValue - 3,
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
                    ViewerId = long.MaxValue - 4,
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
                    ViewerId = long.MaxValue - 5,
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
                    ViewerId = long.MaxValue - 6,
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
                    ViewerId = long.MaxValue - 7,
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
                    ViewerId = long.MaxValue - 8,
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
                    ViewerId = long.MaxValue - 9,
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
                    ViewerId = long.MaxValue - 10,
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
                    ViewerId = long.MaxValue - 11,
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
                    ViewerId = long.MaxValue - 12,
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
                    ViewerId = long.MaxValue - 13,
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
                    ViewerId = long.MaxValue - 14,
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
                    ViewerId = long.MaxValue - 15,
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
                    ViewerId = long.MaxValue - 16,
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
                    ViewerId = long.MaxValue - 17,
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
                    ViewerId = long.MaxValue - 18,
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
                    ViewerId = long.MaxValue - 19,
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
                    ViewerId = long.MaxValue - 20,
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
                    ViewerId = long.MaxValue - 21,
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
                    ViewerId = long.MaxValue - 22,
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
                    ViewerId = long.MaxValue,
                    GettableManaPoint = 50,
                    IsFriend = true,
                },
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
            },
        };
    }
}
