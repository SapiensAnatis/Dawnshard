using System.Diagnostics;
using AutoMapper;
using AutoMapper.Configuration.Conventions;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models.Generated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Services;

public class SavefileService : ISavefileService
{
    private readonly ApiContext apiContext;
    private readonly IMapper mapper;
    private readonly ILogger<SavefileService> logger;

    public SavefileService(ApiContext apiContext, IMapper mapper, ILogger<SavefileService> logger)
    {
        this.apiContext = apiContext;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task Import(string deviceAccountId, LoadIndexData savefile)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Preserve the existing viewer ID if there is one.
        // Could reassign, but this makes it easier for people to remember their ID.
        long? oldViewerId = await this.apiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .Select(x => x.ViewerId)
            .SingleOrDefaultAsync();

        this.logger.LogInformation(
            "Beginning savefile import for account {accountId}",
            deviceAccountId
        );

        this.Delete(deviceAccountId);

        apiContext.PlayerUserData.Add(
            this.mapper.Map<DbPlayerUserData>(
                savefile.user_data,
                opts =>
                    opts.AfterMap(
                        (_, dest) =>
                        {
                            dest.ViewerId = oldViewerId ?? default;
                            dest.DeviceAccountId = deviceAccountId;
                        }
                    )
            )
        );

        this.apiContext.PlayerCharaData.AddRange(
            savefile.chara_list.Select(
                x => MapWithDeviceAccount<DbPlayerCharaData>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerDragonReliability.AddRange(
            savefile.dragon_reliability_list.Select(
                x => MapWithDeviceAccount<DbPlayerDragonReliability>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerDragonData.AddRange(
            savefile.dragon_list.Select(
                x => MapWithDeviceAccount<DbPlayerDragonData>(x, deviceAccountId)
            )
        );

        // Zero out dragon and talisman key ids, as these won't exist in my database
        List<DbParty> parties = savefile.party_list
            .Select(x => MapWithDeviceAccount<DbParty>(x, deviceAccountId))
            .ToList();

        foreach (DbParty party in parties)
        {
            foreach (DbPartyUnit unit in party.Units)
            {
                unit.EquipDragonKeyId = 0;
                unit.EquipTalismanKeyId = 0;
            }
        }

        this.apiContext.PlayerParties.AddRange(parties);

        this.apiContext.PlayerAbilityCrests.AddRange(
            savefile.ability_crest_list.Select(
                x => MapWithDeviceAccount<DbAbilityCrest>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerWeapons.AddRange(
            savefile.weapon_body_list.Select(
                x => MapWithDeviceAccount<DbWeaponBody>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerQuests.AddRange(
            savefile.quest_list.Select(x => MapWithDeviceAccount<DbQuest>(x, deviceAccountId))
        );

        this.apiContext.PlayerStoryState.AddRange(
            savefile.quest_story_list.Select(
                x => MapWithDeviceAccount<DbPlayerStoryState>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerStoryState.AddRange(
            savefile.unit_story_list.Select(
                x => MapWithDeviceAccount<DbPlayerStoryState>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerStoryState.AddRange(
            savefile.castle_story_list.Select(
                x => MapWithDeviceAccount<DbPlayerStoryState>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerStorage.AddRange(
            savefile.material_list.Select(
                x => MapWithDeviceAccount<DbPlayerMaterial>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerTalismans.AddRange(
            savefile.talisman_list.Select(x => MapWithDeviceAccount<DbTalisman>(x, deviceAccountId))
        );

        // TODO: unit sets
        // TODO much later: halidom, endeavours, kaleido data

        this.logger.LogInformation(
            "Mapping completed after {seconds} s",
            stopwatch.Elapsed.TotalSeconds
        );

        await apiContext.SaveChangesAsync();

        this.logger.LogInformation(
            "Saved changes after {seconds} s",
            stopwatch.Elapsed.TotalSeconds
        );
    }

    private void Delete(string deviceAccountId)
    {
        this.apiContext.PlayerUserData.RemoveRange(
            this.apiContext.PlayerUserData.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerCharaData.RemoveRange(
            this.apiContext.PlayerCharaData.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerDragonReliability.RemoveRange(
            this.apiContext.PlayerDragonReliability.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerDragonData.RemoveRange(
            this.apiContext.PlayerDragonData.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerAbilityCrests.RemoveRange(
            this.apiContext.PlayerAbilityCrests.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerStoryState.RemoveRange(
            this.apiContext.PlayerStoryState.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerQuests.RemoveRange(
            this.apiContext.PlayerQuests.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerParties.RemoveRange(
            this.apiContext.PlayerParties.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerPartyUnits.RemoveRange(
            this.apiContext.PlayerPartyUnits.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerWeapons.RemoveRange(
            this.apiContext.PlayerWeapons.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerStorage.RemoveRange(
            this.apiContext.PlayerStorage.Where(x => x.DeviceAccountId == deviceAccountId)
        );
    }

    public async Task Reset(string deviceAccountId)
    {
        this.Delete(deviceAccountId);

        // Unlike importing, this will not preserve the viewer id
        await this.Create(deviceAccountId);
        await this.apiContext.SaveChangesAsync();
    }

    private TDest MapWithDeviceAccount<TDest>(object source, string deviceAccountId)
        where TDest : IDbHasAccountId
    {
        return mapper.Map<TDest>(
            source,
            opts => opts.AfterMap((src, dest) => dest.DeviceAccountId = deviceAccountId)
        );
    }

    public async Task CreateBase(string deviceAccountId)
    {
        DbPlayerUserData userData =
            new(deviceAccountId)
            {
#if DEBUG
                TutorialStatus = 10151,
#endif
                Crystal = 120_000
            };

        await apiContext.PlayerUserData.AddAsync(userData);
        await apiContext.PlayerCharaData.AddAsync(new(deviceAccountId, Charas.ThePrince));
        await this.AddDefaultParties(deviceAccountId);

        await this.apiContext.SaveChangesAsync();
    }

    public async Task Create(string deviceAccountId)
    {
        this.logger.LogInformation("Creating new savefile for account ID {id}", deviceAccountId);

        await this.CreateBase(deviceAccountId);

        await this.AddDefaultWyrmprints(deviceAccountId);
        await this.AddDefaultDragons(deviceAccountId);
        await this.AddDefaultWeapons(deviceAccountId);
        await this.AddDefaultMaterials(deviceAccountId);

        await this.apiContext.SaveChangesAsync();
    }

    private async Task AddDefaultParties(string deviceAccountId)
    {
        await this.apiContext.PlayerParties.AddRangeAsync(
            Enumerable
                .Range(1, DefaultSavefileData.PartySlotCount)
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
        public const int PartySlotCount = 54;

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
