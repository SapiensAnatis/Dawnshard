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

    public async Task CreateNewSavefileBase(string deviceAccountId)
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

        await this.AddDefaultParties(deviceAccountId);
    }

    public async Task CreateNewSavefile(string deviceAccountId)
    {
        await this.CreateNewSavefileBase(deviceAccountId);

        await this.AddDefaultWyrmprints(deviceAccountId);
        await this.AddDefaultDragons(deviceAccountId);
        await this.AddDefaultWeapons(deviceAccountId);
        await this.AddDefaultMaterials(deviceAccountId);
    }

    private async Task AddDefaultParties(string deviceAccountId)
    {
        await this.apiContext.PlayerParties.AddRangeAsync(
            Enumerable
                .Range(1, PartySlotCount)
                .Select(
                    x =>
                        new DbParty()
                        {
                            DeviceAccountId = deviceAccountId,
                            PartyName = "Default",
                            PartyNo = x,
                            Units = new List<DbPartyUnit>()
                            {
                                new() { UnitNo = 1, CharaId = Charas.ThePrince },
                                new() { UnitNo = 2, CharaId = Charas.Empty },
                                new() { UnitNo = 3, CharaId = Charas.Empty },
                                new() { UnitNo = 4, CharaId = Charas.Empty }
                            }
                        }
                )
        );
    }

    private async Task AddDefaultWyrmprints(string deviceAccountId)
    {
        await this.apiContext.PlayerAbilityCrests.AddAsync(
            new DbAbilityCrest()
            {
                DeviceAccountId = deviceAccountId,
                AbilityCrestId = AbilityCrests.ManaFount
            }
        );

        await this.apiContext.PlayerAbilityCrests.AddRangeAsync(
            DefaultSavefileData.FiveStarCrests
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
                    DefaultSavefileData.FourStarCrests.Select(
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
                    DefaultSavefileData.ThreeStarCrests.Select(
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
                    DefaultSavefileData.SinDomCrests.Select(
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

    private async Task AddDefaultDragons(string deviceAccountId)
    {
        await this.apiContext.PlayerDragonData.AddRangeAsync(
            Enumerable
                .Repeat(
                    DefaultSavefileData.Dragons.Select(
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
            DefaultSavefileData.Dragons.Select(
                x => DbPlayerDragonReliabilityFactory.Create(deviceAccountId, x)
            )
        );
    }

    private async Task AddDefaultWeapons(string deviceAccountId)
    {
        await this.apiContext.PlayerWeapons.AddRangeAsync(
            DefaultSavefileData.Weapons.Select(
                x =>
                    new DbWeaponBody()
                    {
                        DeviceAccountId = deviceAccountId,
                        WeaponBodyId = x,
                        BuildupCount = 80,
                        LimitBreakCount = 8,
                        LimitOverCount = 1,
                        EquipableCount = 4,
                        AdditionalCrestSlotType1Count = 1,
                        AdditionalCrestSlotType2Count = 0,
                        AdditionalCrestSlotType3Count = 2,
                        FortPassiveCharaWeaponBuildupCount = 1,
                        IsNew = true,
                        GetTime = DateTime.UtcNow,
                    }
            )
        );
    }

    private async Task AddDefaultMaterials(string deviceAccountId, int defaultQuantity = 10000)
    {
        await this.apiContext.PlayerStorage.AddRangeAsync(
            DefaultSavefileData.UpgradeMaterials.Select(
                x =>
                    new DbPlayerMaterial()
                    {
                        DeviceAccountId = deviceAccountId,
                        MaterialId = x,
                        Quantity = defaultQuantity
                    }
            )
        );
    }

    private static class DefaultSavefileData
    {
        public static readonly IReadOnlyList<AbilityCrests> FiveStarCrests =
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

        public static readonly IReadOnlyList<AbilityCrests> FourStarCrests =
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

        public static readonly IReadOnlyList<AbilityCrests> ThreeStarCrests =
            new List<AbilityCrests>()
            {
                AbilityCrests.Bellathorna,
                // Technically Dragon Arcanum is 2 star but whatever
                AbilityCrests.DragonArcanum,
                AbilityCrests.DragonsNest
            };

        public static readonly IReadOnlyList<AbilityCrests> SinDomCrests = new List<AbilityCrests>()
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

        public static readonly IReadOnlyList<Dragons> Dragons = new List<Dragons>()
        {
            Shared.Definitions.Enums.Dragons.GalaRebornAgni,
            Shared.Definitions.Enums.Dragons.Horus,
            Shared.Definitions.Enums.Dragons.GalaRebornPoseidon,
            Shared.Definitions.Enums.Dragons.GaibhneCreidhne,
            Shared.Definitions.Enums.Dragons.GalaRebornZephyr,
            Shared.Definitions.Enums.Dragons.Freyja,
            Shared.Definitions.Enums.Dragons.GalaRebornJeanne,
            Shared.Definitions.Enums.Dragons.TieShanGongzhu,
            Shared.Definitions.Enums.Dragons.GalaRebornNidhogg,
            Shared.Definitions.Enums.Dragons.Azazel
        };

        public static readonly IReadOnlyList<WeaponBodies> Weapons = new List<WeaponBodies>()
        {
            // Flame
            WeaponBodies.PrimalCrimson,
            WeaponBodies.RagingConflagration,
            WeaponBodies.FlamerulersFang,
            WeaponBodies.NobleCrimsonHeat,
            WeaponBodies.OmniflameLance,
            WeaponBodies.ValkyriesHellfire,
            WeaponBodies.Hellblaze,
            WeaponBodies.Flamerollick,
            WeaponBodies.BigBangTrigger,
            // Water
            WeaponBodies.PrimalAqua,
            WeaponBodies.CalamitousTorrent,
            WeaponBodies.TiderulersFang,
            WeaponBodies.LimpidCascade,
            WeaponBodies.SapphireMercurius,
            WeaponBodies.AqueousPrison,
            WeaponBodies.RuleroftheJeweledTide,
            WeaponBodies.AquamarineTrigger,
            // Wind
            WeaponBodies.PrimalTempest,
            WeaponBodies.NobleHorizon,
            WeaponBodies.WindrulersFang,
            WeaponBodies.TempestsGuide,
            WeaponBodies.GalesAid,
            WeaponBodies.JormungandsWrath,
            WeaponBodies.StormChaser,
            WeaponBodies.Squallruler,
            WeaponBodies.CycloneTrigger,
            // Light
            WeaponBodies.PrimalLightning,
            WeaponBodies.DauntingFlash,
            WeaponBodies.FulminatorsFang,
            WeaponBodies.IndomitableThundercrash,
            WeaponBodies.RadiantLightflash,
            WeaponBodies.JupitersShimmer,
            WeaponBodies.ElectronBurst,
            WeaponBodies.CosmicRuler,
            WeaponBodies.DivineTrigger,
            // Shadow
            WeaponBodies.PrimalHex,
            WeaponBodies.EternalAbyss,
            WeaponBodies.ShaderulersFang,
            WeaponBodies.NightfallsDarkbiteAxe,
            WeaponBodies.EbonPlagueLance,
            WeaponBodies.NightmareProphecy,
            WeaponBodies.UmbralChaser,
            WeaponBodies.ConsumingDarkness,
            WeaponBodies.DuskTrigger
        };

        public static readonly IReadOnlyList<Materials> UpgradeMaterials = new List<Materials>()
        {
            Materials.GoldCrystal,
            Materials.SilverCrystal,
            Materials.BronzeCrystal
        };
    }
}
