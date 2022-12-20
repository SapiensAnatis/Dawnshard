using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Database.Factories;

namespace DragaliaAPI.Database.Repositories;

/// <summary>
/// Repository to
/// </summary>
public class DeviceAccountRepository : BaseRepository, IDeviceAccountRepository
{
    private readonly ApiContext apiContext;
    private readonly ICharaDataService charaDataService;

    private const int PartySlotCount = 54;

    public DeviceAccountRepository(ApiContext apiContext, ICharaDataService charaDataService)
        : base(apiContext)
    {
        this.apiContext = apiContext;
        this.charaDataService = charaDataService;
    }

    public async Task AddNewDeviceAccount(string id, string hashedPassword)
    {
        await apiContext.DeviceAccounts.AddAsync(new DbDeviceAccount(id, hashedPassword));
    }

    public async Task<DbDeviceAccount?> GetDeviceAccountById(string id)
    {
        return await apiContext.DeviceAccounts.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task CreateNewSavefile(string deviceAccountId)
    {
        DbPlayerUserData userData = DbSavefileUserDataFactory.Create(deviceAccountId);
#if DEBUG
        userData.TutorialStatus = 10151;
#endif
        await apiContext.PlayerUserData.AddAsync(userData);

        await apiContext.PlayerCharaData.AddAsync(
            DbPlayerCharaDataFactory.Create(
                deviceAccountId,
                charaDataService.GetData(Charas.ThePrince)
            )
        );

        List<DbParty> defaultParties = new();
        for (int i = 1; i <= PartySlotCount; i++)
        {
            defaultParties.Add(
                new()
                {
                    DeviceAccountId = deviceAccountId,
                    PartyName = "Default",
                    PartyNo = i,
                    Units = new List<DbPartyUnit>()
                    {
                        new() { UnitNo = 1, CharaId = Charas.ThePrince },
                        new() { UnitNo = 2, CharaId = Charas.Empty },
                        new() { UnitNo = 3, CharaId = Charas.Empty },
                        new() { UnitNo = 4, CharaId = Charas.Empty }
                    }
                }
            );
        }

        await apiContext.PlayerParties.AddRangeAsync(defaultParties);

        await apiContext.PlayerWeapons.AddAsync(
            new DbWeaponBody()
            {
                DeviceAccountId = deviceAccountId,
                WeaponBodyId = WeaponBodies.PrimalCrimson,
                BuildupCount = 80,
                LimitBreakCount = 8,
                LimitOverCount = 1,
                IsNew = false,
                GetTime = DateTime.UtcNow,
            }
        );

        await this.AddDefaultWyrmprints(deviceAccountId);

        await this.AddDefaultDragons(deviceAccountId);
    }

    private async Task AddDefaultDragons(string deviceAccountId)
    {
        await this.apiContext.PlayerDragonData.AddRangeAsync(
            Enumerable
                .Repeat(
                    defaultDragons.Select(
                        x =>
                            new DbPlayerDragonData()
                            {
                                DeviceAccountId = deviceAccountId,
                                DragonId = x,
                                Level = 100,
                                LimitBreakCount = 4,
                                Ability1Level = 5,
                                Ability2Level = 5,
                                Skill1Level = 2,
                                AttackPlusCount = 50,
                                HpPlusCount = 50,
                                Exp = 1_240_020,
                                GetTime = DateTime.UtcNow,
                                IsLock = false,
                                IsNew = false,
                            }
                    ),
                    4
                )
                .SelectMany(x => x)
        );

        await this.apiContext.PlayerDragonReliability.AddRangeAsync(
            defaultDragons.Select(x => DbPlayerDragonReliabilityFactory.Create(deviceAccountId, x))
        );
    }

    private async Task AddDefaultWyrmprints(string deviceAccountId)
    {
        await this.apiContext.PlayerAbilityCrests.AddRangeAsync(
            default5StarCrests
                .Select(
                    x =>
                        new DbAbilityCrest()
                        {
                            AbilityCrestId = x,
                            BuildupCount = 50,
                            LimitBreakCount = 4,
                            DeviceAccountId = deviceAccountId,
                            AttackPlusCount = 50,
                            HpPlusCount = 50,
                            EquipableCount = 4,
                            GetTime = DateTime.UtcNow,
                            IsFavorite = false,
                            IsNew = false,
                        }
                )
                .Concat(
                    default4StarCrests.Select(
                        x =>
                            new DbAbilityCrest()
                            {
                                AbilityCrestId = x,
                                BuildupCount = 40,
                                LimitBreakCount = 4,
                                DeviceAccountId = deviceAccountId,
                                AttackPlusCount = 50,
                                HpPlusCount = 50,
                                EquipableCount = 4,
                                GetTime = DateTime.UtcNow,
                                IsFavorite = false,
                                IsNew = false,
                            }
                    )
                )
                .Concat(
                    default3StarCrests.Select(
                        x =>
                            new DbAbilityCrest()
                            {
                                AbilityCrestId = x,
                                BuildupCount = 10,
                                LimitBreakCount = 4,
                                DeviceAccountId = deviceAccountId,
                                AttackPlusCount = 50,
                                HpPlusCount = 50,
                                EquipableCount = 4,
                                GetTime = DateTime.UtcNow,
                                IsFavorite = false,
                                IsNew = false,
                            }
                    )
                )
                .Concat(
                    defaultSinDomCrests.Select(
                        x =>
                            new DbAbilityCrest()
                            {
                                AbilityCrestId = x,
                                BuildupCount = 30,
                                LimitBreakCount = 4,
                                DeviceAccountId = deviceAccountId,
                                AttackPlusCount = 40,
                                HpPlusCount = 40,
                                EquipableCount = 4,
                                GetTime = DateTime.UtcNow,
                                IsFavorite = false,
                                IsNew = false,
                            }
                    )
                )
        );
    }

    private static readonly IReadOnlyList<AbilityCrests> default5StarCrests =
        new List<AbilityCrests>()
        {
            // Generic SD
            AbilityCrests.ValiantCrown,
            AbilityCrests.HeraldsofHinomoto,
            // Generic strength
            AbilityCrests.PecorinesGrandAdventure,
            AbilityCrests.PrimalCrisis,
            AbilityCrests.MemoryofaFriend,
            // FS
            AbilityCrests.HereCometheSealers,
            // Generic crit
            AbilityCrests.ThirdAnniversary,
            AbilityCrests.LevinsChampion,
            // Punishers
            AbilityCrests.MeandMyBestie, // Burn
            AbilityCrests.IntheLimelight, // Scorchrend
            AbilityCrests.WingsofRebellionatRest, // Frostbite
            AbilityCrests.AManUnchanging, // Poison
            AbilityCrests.SweetSurprise, // Scorchrend
            AbilityCrests.SpiritoftheSeason, // Paralysis
            AbilityCrests.ExtremeTeamwork, // Flashburn
            AbilityCrests.WelcometotheOpera, // Shadowblight
            // Support
            AbilityCrests.JewelsoftheSun,
            AbilityCrests.StudyRabbits,
            AbilityCrests.GiveMeYourWounded,
            AbilityCrests.ProperMaintenance,
            AbilityCrests.CastleCheerCorps,
            // Misc
            AbilityCrests.TheChocolatiers,
            AbilityCrests.WorthyRivals,
            AbilityCrests.AnAncientOath,
        };

    private static readonly IReadOnlyList<AbilityCrests> default4StarCrests =
        new List<AbilityCrests>()
        {
            // Punishers
            AbilityCrests.ThePlaguebringer,
            AbilityCrests.HisCleverBrother,
            AbilityCrests.AButlersSmile,
            AbilityCrests.TheNoblesDayOff,
            // Misc
            AbilityCrests.FromWhenceHeComes,
            AbilityCrests.SnipersAllure,
            AbilityCrests.LunarFestivities,
            AbilityCrests.BeautifulNothingness,
        };

    private static readonly IReadOnlyList<AbilityCrests> default3StarCrests =
        new List<AbilityCrests>()
        {
            AbilityCrests.Bellathorna,
            AbilityCrests.DragonArcanum,
            AbilityCrests.DragonsNest
        };

    private static readonly IReadOnlyList<AbilityCrests> defaultSinDomCrests =
        new List<AbilityCrests>()
        {
            // SD
            AbilityCrests.TutelarysDestinyWolfsBoon,
            AbilityCrests.AppleliciousDreamsButterflysBoon,
            AbilityCrests.AnUnfreezingFlowerDeersBoon,
            AbilityCrests.AKnightsDreamAxesBoon,
            // Psalm
            AbilityCrests.PromisedPietyStaffsBoon,
            AbilityCrests.RavenousFireCrownsBoon,
            AbilityCrests.MaskofDeterminationBowsBoon
        };

    private static readonly IReadOnlyList<Dragons> defaultDragons = new List<Dragons>()
    {
        Dragons.GalaRebornAgni,
        Dragons.Horus,
        Dragons.GalaRebornPoseidon,
        Dragons.GaibhneCreidhne,
        Dragons.GalaRebornZephyr,
        Dragons.Freyja,
        Dragons.GalaRebornJeanne,
        Dragons.TieShanGongzhu,
        Dragons.GalaRebornNidhogg,
        Dragons.Azazel
    };
}
