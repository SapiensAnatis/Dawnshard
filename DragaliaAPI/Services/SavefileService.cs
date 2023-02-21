using System.Diagnostics;
using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Services;

public class SavefileService : ISavefileService
{
    private readonly ApiContext apiContext;
    private readonly IDistributedCache cache;
    private readonly IMapper mapper;
    private readonly ILogger<SavefileService> logger;

    private const int RecheckLockMs = 1000;
    private const int LockFailsafeExpiryMin = 5;

    public SavefileService(
        ApiContext apiContext,
        IDistributedCache cache,
        IMapper mapper,
        ILogger<SavefileService> logger
    )
    {
        this.apiContext = apiContext;
        this.cache = cache;
        this.mapper = mapper;
        this.logger = logger;
    }

    private static class RedisSchema
    {
        public static string PendingImport(string deviceAccountId) =>
            $":pending_save_import:{deviceAccountId}";
    }

    private static readonly DistributedCacheEntryOptions RedisOptions =
        new()
        {
            // Keys should be automatically removed, but just in case
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(LockFailsafeExpiryMin)
        };

    /// <summary>
    /// Thread safe version of <see cref="Import(string, LoadIndexData)"/>.
    /// </summary>
    /// <param name="deviceAccountId">The primary key to import for.</param>
    /// <param name="savefile">The savefile to import/</param>
    /// <returns>The task.</returns>
    public async Task ThreadSafeImport(string deviceAccountId, LoadIndexData savefile)
    {
        string key = RedisSchema.PendingImport(deviceAccountId);

        if (!string.IsNullOrEmpty(await this.cache.GetStringAsync(key)))
        {
            while (!string.IsNullOrEmpty(await this.cache.GetStringAsync(key)))
            {
                this.logger.LogInformation("Savefile import is locked, waiting...");
                await Task.Delay(RecheckLockMs);
            }

            this.logger.LogInformation("Savefile import lock released.");
            return;
        }

        try
        {
            await this.Import(deviceAccountId, savefile);
        }
        catch (Exception)
        {
            await this.cache.RemoveAsync(RedisSchema.PendingImport(deviceAccountId));
            throw;
        }
    }

    /// <summary>
    /// Import a savefile.
    /// <remarks>Not thread safe if called for the same account id from two different threads.</remarks>
    /// </summary>
    /// <param name="deviceAccountId">Primary key to import for.</param>
    /// <param name="savefile">Savefile data.</param>
    /// <returns>The task.</returns>
    public async Task Import(string deviceAccountId, LoadIndexData savefile)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Place a lock preventing any concurrent save imports
        await this.cache.SetStringAsync(
            RedisSchema.PendingImport(deviceAccountId),
            "true",
            RedisOptions
        );

        // Preserve the existing viewer ID if there is one.
        // Could reassign, but this makes it easier for people to remember their ID.
        long? oldViewerId = await this.apiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .Select(x => x.ViewerId)
            .Cast<long?>()
            .SingleOrDefaultAsync();

        this.logger.LogInformation(
            "Beginning savefile import for account {accountId}",
            deviceAccountId
        );

        await this.apiContext.Database.BeginTransactionAsync();

        try
        {
            this.Delete(deviceAccountId);

            await this.apiContext.Players.AddAsync(new DbPlayer() { AccountId = deviceAccountId });

            // This has JsonRequired so this should never be triggered
            ArgumentNullException.ThrowIfNull(savefile.user_data);

            apiContext.PlayerUserData.Add(
                this.mapper.Map<DbPlayerUserData>(
                    savefile.user_data,
                    opts =>
                        opts.AfterMap(
                            (_, dest) =>
                            {
                                dest.ViewerId = oldViewerId ?? default;
                                dest.DeviceAccountId = deviceAccountId;
                                dest.Crystal += 1_200_000;
                                dest.LastSaveImportTime = DateTimeOffset.UtcNow;
                                dest.LastLoginTime = DateTimeOffset.UnixEpoch;
                            }
                        )
                )
            );

            this.apiContext.PlayerCharaData.AddRange(
                this.MapWithDeviceAccount<DbPlayerCharaData>(savefile.chara_list, deviceAccountId)
            );

            this.apiContext.PlayerDragonReliability.AddRange(
                this.MapWithDeviceAccount<DbPlayerDragonReliability>(
                    savefile.dragon_reliability_list,
                    deviceAccountId
                )
            );

            // Build key id mappings for dragons and talismans
            Dictionary<long, DbPlayerDragonData> dragonKeyIds = new();

            foreach (DragonList d in savefile.dragon_list ?? new List<DragonList>())
            {
                ulong oldKeyId = d.dragon_key_id;
                DbPlayerDragonData dbEntry = MapWithDeviceAccount<DbPlayerDragonData>(
                    d,
                    deviceAccountId
                );
                DbPlayerDragonData addedEntry = (
                    await this.apiContext.PlayerDragonData.AddAsync(dbEntry)
                ).Entity;

                dragonKeyIds.Add((long)oldKeyId, addedEntry);
            }

            Dictionary<long, DbTalisman> talismanKeyIds = new();

            foreach (TalismanList t in savefile.talisman_list ?? new List<TalismanList>())
            {
                ulong oldKeyId = t.talisman_key_id;
                DbTalisman dbEntry = MapWithDeviceAccount<DbTalisman>(t, deviceAccountId);
                DbTalisman addedEntry = (
                    await this.apiContext.PlayerTalismans.AddAsync(dbEntry)
                ).Entity;

                talismanKeyIds.Add((long)oldKeyId, addedEntry);
            }

            // Must save changes for key ids to update
            await this.apiContext.SaveChangesAsync();

            if (savefile.party_list is not null)
            {
                // Update key ids in parties
                List<DbParty> parties = savefile.party_list
                    .Select(x => MapWithDeviceAccount<DbParty>(x, deviceAccountId))
                    .ToList();

                foreach (DbParty party in parties)
                {
                    foreach (DbPartyUnit unit in party.Units)
                    {
                        unit.EquipDragonKeyId = dragonKeyIds.TryGetValue(
                            unit.EquipDragonKeyId,
                            out DbPlayerDragonData? dragon
                        )
                            ? dragon.DragonKeyId
                            : 0;

                        unit.EquipTalismanKeyId = talismanKeyIds.TryGetValue(
                            unit.EquipTalismanKeyId,
                            out DbTalisman? talisman
                        )
                            ? talisman.TalismanKeyId
                            : 0;
                    }
                }

                this.apiContext.PlayerParties.AddRange(parties);
            }
            else
            {
                await this.AddDefaultParties(deviceAccountId);
            }

            this.apiContext.PlayerAbilityCrests.AddRange(
                MapWithDeviceAccount<DbAbilityCrest>(savefile.ability_crest_list, deviceAccountId)
            );

            this.apiContext.PlayerWeapons.AddRange(
                MapWithDeviceAccount<DbWeaponBody>(savefile.weapon_body_list, deviceAccountId)
            );

            this.apiContext.PlayerQuests.AddRange(
                MapWithDeviceAccount<DbQuest>(savefile.quest_list, deviceAccountId)
            );

            this.apiContext.PlayerStoryState.AddRange(
                MapWithDeviceAccount<DbPlayerStoryState>(savefile.quest_story_list, deviceAccountId)
            );

            this.apiContext.PlayerStoryState.AddRange(
                MapWithDeviceAccount<DbPlayerStoryState>(savefile.unit_story_list, deviceAccountId)
            );

            this.apiContext.PlayerStoryState.AddRange(
                MapWithDeviceAccount<DbPlayerStoryState>(
                    savefile.castle_story_list,
                    deviceAccountId
                )
            );

            this.apiContext.PlayerMaterials.AddRange(
                MapWithDeviceAccount<DbPlayerMaterial>(savefile.material_list, deviceAccountId)
            );

            this.apiContext.PlayerFortBuilds.AddRange(
                MapWithDeviceAccount<DbFortBuild>(savefile.build_list, deviceAccountId)
            );

            this.apiContext.PlayerWeaponSkins.AddRange(
                MapWithDeviceAccount<DbWeaponSkin>(savefile.weapon_skin_list, deviceAccountId)
            );

            this.apiContext.PlayerPassiveAbilities.AddRange(
                MapWithDeviceAccount<DbWeaponPassiveAbility>(
                    savefile.weapon_passive_ability_list,
                    deviceAccountId
                )
            );

            this.apiContext.PlayerDragonGifts.AddRange(
                MapWithDeviceAccount<DbPlayerDragonGift>(savefile.dragon_gift_list, deviceAccountId)
            );

            // TODO: unit sets
            // TODO much later: halidom, endeavours, kaleido data

            this.logger.LogInformation(
                "Mapping completed after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            await apiContext.SaveChangesAsync();
            await this.apiContext.Database.CommitTransactionAsync();

            // Remove lock
            await this.cache.RemoveAsync(RedisSchema.PendingImport(deviceAccountId));

            this.logger.LogInformation(
                "Saved changes after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );
        }
        catch
        {
            await this.apiContext.Database.RollbackTransactionAsync();
            throw;
        }
    }

    private void Delete(string deviceAccountId)
    {
        this.apiContext.Players.RemoveRange(
            this.apiContext.Players.Where(x => x.AccountId == deviceAccountId)
        );
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
        this.apiContext.PlayerMaterials.RemoveRange(
            this.apiContext.PlayerMaterials.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerTalismans.RemoveRange(
            this.apiContext.PlayerTalismans.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerFortBuilds.RemoveRange(
            this.apiContext.PlayerFortBuilds.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerWeaponSkins.RemoveRange(
            this.apiContext.PlayerWeaponSkins.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerPassiveAbilities.RemoveRange(
            this.apiContext.PlayerPassiveAbilities.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerDragonGifts.RemoveRange(
            this.apiContext.PlayerDragonGifts.Where(x => x.DeviceAccountId == deviceAccountId)
        );
    }

    public async Task Reset(string deviceAccountId)
    {
        this.Delete(deviceAccountId);

        // Unlike importing, this will not preserve the viewer id
        await this.Create(deviceAccountId);
        await this.apiContext.SaveChangesAsync();
    }

    public IQueryable<DbPlayer> Load(string deviceAccountId)
    {
        return this.apiContext.Players
            .Where(x => x.AccountId == deviceAccountId)
            .Include(x => x.UserData)
            .Include(x => x.AbilityCrestList)
            .Include(x => x.CharaList)
            .Include(x => x.Currencies)
            .Include(x => x.DragonList)
            .Include(x => x.DragonReliabilityList)
            .Include(x => x.DragonGiftList)
            .Include(x => x.BuildList)
            .Include(x => x.QuestList)
            .Include(x => x.StoryStates)
            .Include(x => x.PartyList)
            .ThenInclude(x => x.Units.OrderBy(x => x.UnitNo))
            .Include(x => x.TalismanList)
            .Include(x => x.WeaponBodyList)
            .Include(x => x.MaterialList)
            .Include(x => x.WeaponSkinList)
            .Include(x => x.WeaponPassiveAbilityList)
            .AsSplitQuery();
    }

    private IEnumerable<TDest> MapWithDeviceAccount<TDest>(
        IEnumerable<object>? source,
        string deviceAccountId
    )
        where TDest : IDbHasAccountId
    {
        return (source ?? new List<object>()).Select(
            x =>
                mapper.Map<TDest>(
                    x,
                    opts => opts.AfterMap((src, dest) => dest.DeviceAccountId = deviceAccountId)
                )
        );
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
        this.logger.LogInformation("Creating new savefile for account ID {id}", deviceAccountId);

        this.Delete(deviceAccountId);
        this.apiContext.Players.Add(new() { AccountId = deviceAccountId });

        DbPlayerUserData userData =
            new(deviceAccountId)
            {
#if DEBUG
                TutorialStatus = 10151,
#endif
                Crystal = 1_200_000
            };

        await apiContext.PlayerUserData.AddAsync(userData);
        await this.AddDefaultParties(deviceAccountId);
        await this.AddDefaultCharacters(deviceAccountId);

        // This needs to be in the save or the halidom screen will softlock
        // TODO: Move this to the tutorial step which gives you access to the Halidom,
        // so that saves imported without it will also avoid the softlock
        await apiContext.PlayerFortBuilds.AddAsync(
            new DbFortBuild()
            {
                DeviceAccountId = deviceAccountId,
                PlantId = FortPlants.TheHalidom,
                PositionX = 16, // Default Halidom position
                PositionZ = 17,
            }
        );

        await this.apiContext.SaveChangesAsync();
    }

    public async Task Create(string deviceAccountId)
    {
        await this.CreateBase(deviceAccountId);

        await this.AddDefaultWyrmprints(deviceAccountId);
        await this.AddDefaultDragons(deviceAccountId);
        await this.AddDefaultWeapons(deviceAccountId);
        await this.AddDefaultMaterials(deviceAccountId);

        await this.apiContext.SaveChangesAsync();
    }

    #region Default save data
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
                                new()
                                {
                                    DeviceAccountId = deviceAccountId,
                                    PartyNo = x,
                                    UnitNo = 1,
                                    CharaId = Charas.ThePrince
                                },
                                new()
                                {
                                    DeviceAccountId = deviceAccountId,
                                    PartyNo = x,
                                    UnitNo = 2,
                                    CharaId = Charas.Empty
                                },
                                new()
                                {
                                    DeviceAccountId = deviceAccountId,
                                    PartyNo = x,
                                    UnitNo = 3,
                                    CharaId = Charas.Empty
                                },
                                new()
                                {
                                    DeviceAccountId = deviceAccountId,
                                    PartyNo = x,
                                    UnitNo = 4,
                                    CharaId = Charas.Empty
                                }
                            }
                        }
                )
        );
    }

    private async Task AddDefaultCharacters(string deviceAccountId)
    {
        await this.apiContext.PlayerCharaData.AddRangeAsync(
            DefaultSavefileData.Characters.Select(x => new DbPlayerCharaData(deviceAccountId, x))
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
                    DefaultSavefileData.FreeDragonCount
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
        await this.apiContext.PlayerMaterials.AddRangeAsync(
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

    internal static class DefaultSavefileData
    {
        public static readonly IReadOnlyList<Charas> Characters = new List<Charas>()
        {
            Charas.ThePrince
        };

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

        public const int FreeDragonCount = 4;

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
            Materials.BronzeCrystal,
            Materials.LookingGlass
        };
    }
    #endregion
}
