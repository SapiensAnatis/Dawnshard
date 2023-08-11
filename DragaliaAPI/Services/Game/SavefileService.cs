using System.Collections.Immutable;
using System.Diagnostics;
using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Features.Stamp;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Services.Game;

public class SavefileService : ISavefileService
{
    private readonly ApiContext apiContext;
    private readonly IDistributedCache cache;
    private readonly IMapper mapper;
    private readonly ILogger<SavefileService> logger;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly IUnitRepository unitRepository;

    private const int RecheckLockMs = 1000;
    private const int LockFailsafeExpiryMin = 5;

    private readonly int maxSavefileVersion;

    public SavefileService(
        ApiContext apiContext,
        IDistributedCache cache,
        IMapper mapper,
        ILogger<SavefileService> logger,
        IPlayerIdentityService playerIdentityService,
        IEnumerable<ISavefileUpdate> savefileUpdates,
        IUnitRepository unitRepository
    )
    {
        this.apiContext = apiContext;
        this.cache = cache;
        this.mapper = mapper;
        this.logger = logger;
        this.playerIdentityService = playerIdentityService;
        this.unitRepository = unitRepository;

        this.maxSavefileVersion =
            savefileUpdates.MaxBy(x => x.SavefileVersion)?.SavefileVersion ?? 0;
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
    /// Thread safe version of <see cref="Import(LoadIndexData)"/>.
    /// </summary>
    /// <param name="savefile">The savefile to import/</param>
    /// <returns>The task.</returns>
    public async Task ThreadSafeImport(LoadIndexData savefile)
    {
        string key = RedisSchema.PendingImport(this.playerIdentityService.AccountId);

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
            await this.Import(savefile);
        }
        catch (Exception)
        {
            await this.cache.RemoveAsync(
                RedisSchema.PendingImport(this.playerIdentityService.AccountId)
            );
            throw;
        }
    }

    /// <summary>
    /// Import a savefile.
    /// </summary>
    /// <param name="savefile">Savefile data.</param>
    /// <returns>The task.</returns>
    /// <remarks>Not thread safe if called for the same account id from two different threads.</remarks>
    public async Task Import(LoadIndexData savefile)
    {
        string deviceAccountId = this.playerIdentityService.AccountId;
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
            this.playerIdentityService.AccountId
        );

        try
        {
            this.apiContext.ChangeTracker.AutoDetectChangesEnabled = false;
            await using IDbContextTransaction transaction =
                await this.apiContext.Database.BeginTransactionAsync();

            this.Delete();

            this.logger.LogDebug(
                "Deleting savedata step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.Players.Add(
                new DbPlayer { AccountId = this.playerIdentityService.AccountId }
            );

            this.logger.LogDebug(
                "Mapping DbPlayer step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

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
                                dest.ActiveMemoryEventId = 0;
                            }
                        )
                )
            );

            this.logger.LogDebug(
                "Mapping DbPlayerUserData step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerCharaData.AddRange(
                savefile.chara_list.MapWithDeviceAccount<DbPlayerCharaData>(mapper, deviceAccountId)
            );

            this.logger.LogDebug(
                "Mapping DbPlayerCharaData step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerDragonReliability.AddRange(
                savefile.dragon_reliability_list.MapWithDeviceAccount<DbPlayerDragonReliability>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbPlayerDragonReliability step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            // Build key id mappings for dragons and talismans
            Dictionary<long, DbPlayerDragonData> dragonKeyIds = new();

            foreach (DragonList d in savefile.dragon_list ?? new List<DragonList>())
            {
                ulong oldKeyId = d.dragon_key_id;
                DbPlayerDragonData dbEntry = d.MapWithDeviceAccount<DbPlayerDragonData>(
                    mapper,
                    deviceAccountId
                );
                DbPlayerDragonData addedEntry = (
                    this.apiContext.PlayerDragonData.Add(dbEntry)
                ).Entity;

                dragonKeyIds.TryAdd((long)oldKeyId, addedEntry);
            }

            this.logger.LogDebug(
                "Mapping DbPlayerDragonData step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            Dictionary<long, DbTalisman> talismanKeyIds = new();

            foreach (TalismanList t in savefile.talisman_list ?? new List<TalismanList>())
            {
                ulong oldKeyId = t.talisman_key_id;
                DbTalisman dbEntry = t.MapWithDeviceAccount<DbTalisman>(mapper, deviceAccountId);
                DbTalisman addedEntry = this.apiContext.PlayerTalismans.Add(dbEntry).Entity;

                talismanKeyIds.TryAdd((long)oldKeyId, addedEntry);
            }

            this.logger.LogDebug(
                "Mapping DbTalisman step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            // Must save changes for key ids to update
            await this.apiContext.SaveChangesAsync();

            this.logger.LogDebug(
                "SaveChangesAsync() #1 step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.AddShopInfo();

            this.logger.LogDebug(
                "Adding shop info step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            if (savefile.party_list is not null)
            {
                // Update key ids in parties
                List<DbParty> parties = savefile.party_list
                    .MapWithDeviceAccount<DbParty>(mapper, deviceAccountId)
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

            this.logger.LogDebug(
                "Mapping DbParty step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerAbilityCrests.AddRange(
                savefile.ability_crest_list.MapWithDeviceAccount<DbAbilityCrest>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbAbilityCrest step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerWeapons.AddRange(
                savefile.weapon_body_list.MapWithDeviceAccount<DbWeaponBody>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbWeaponBody step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerQuests.AddRange(
                savefile.quest_list.MapWithDeviceAccount<DbQuest>(mapper, deviceAccountId)
            );

            this.logger.LogDebug(
                "Mapping DbQuest step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerStoryState.AddRange(
                savefile.quest_story_list.MapWithDeviceAccount<DbPlayerStoryState>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbPlayerStoryState (QuestStoryList) step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerStoryState.AddRange(
                savefile.unit_story_list.MapWithDeviceAccount<DbPlayerStoryState>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbPlayerStoryState (UnitStoryList) step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerStoryState.AddRange(
                savefile.castle_story_list.MapWithDeviceAccount<DbPlayerStoryState>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbPlayerStoryState (CastleStoryList) step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerMaterials.AddRange(
                savefile.material_list.MapWithDeviceAccount<DbPlayerMaterial>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbPlayerMaterial step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerFortBuilds.AddRange(
                savefile.build_list.MapWithDeviceAccount<DbFortBuild>(mapper, deviceAccountId)
            );

            this.logger.LogDebug(
                "Mapping DbFortBuild step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerWeaponSkins.AddRange(
                savefile.weapon_skin_list.MapWithDeviceAccount<DbWeaponSkin>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbWeaponSkin step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerPassiveAbilities.AddRange(
                savefile.weapon_passive_ability_list.MapWithDeviceAccount<DbWeaponPassiveAbility>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbWeaponPassibeAbility step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerDragonGifts.AddRange(
                savefile.dragon_gift_list.MapWithDeviceAccount<DbPlayerDragonGift>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbPlayerDragonGift step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.EquippedStamps.AddRange(
                savefile.equip_stamp_list.MapWithDeviceAccount<DbEquippedStamp>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbEquippedStamp step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerTrades.AddRange(
                savefile.user_treasure_trade_list.MapWithDeviceAccount<DbPlayerTrade>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbPlayerTrade step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.PlayerSummonTickets.AddRange(
                savefile.summon_ticket_list.MapWithDeviceAccount<DbSummonTicket>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbSummonTicket step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            if (savefile.user_data.emblem_id != Emblems.DragonbloodPrince)
            {
                this.apiContext.Emblems.Add(
                    new DbEmblem
                    {
                        DeviceAccountId = deviceAccountId,
                        EmblemId = savefile.user_data.emblem_id,
                        GetTime = DateTimeOffset.UnixEpoch,
                        IsNew = false
                    }
                );
            }

            this.logger.LogDebug(
                "Adding DbEmblem step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            apiContext.PartyPowers.Add(
                savefile.party_power_data.MapWithDeviceAccount<DbPartyPower>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbPartyPower step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.apiContext.QuestEvents.AddRange(
                savefile.quest_event_list.MapWithDeviceAccount<DbQuestEvent>(
                    mapper,
                    deviceAccountId
                )
            );

            this.logger.LogDebug(
                "Mapping DbQuestEvent step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.logger.LogInformation(
                "Mapping completed after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            await apiContext.SaveChangesAsync();
            await transaction.CommitAsync();

            // Remove lock
            await this.cache.RemoveAsync(RedisSchema.PendingImport(deviceAccountId));

            this.logger.LogInformation(
                "Saved changes after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );
        }
        catch
        {
            this.apiContext.ChangeTracker.AutoDetectChangesEnabled = true;
            this.apiContext.ChangeTracker.Clear();
            throw;
        }
    }

    private void Delete()
    {
        string deviceAccountId = this.playerIdentityService.AccountId;

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
        this.apiContext.PlayerMissions.RemoveRange(
            this.apiContext.PlayerMissions.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.EquippedStamps.RemoveRange(
            this.apiContext.EquippedStamps.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerShopInfos.RemoveRange(
            this.apiContext.PlayerShopInfos.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerTrades.RemoveRange(
            this.apiContext.PlayerTrades.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerEventData.RemoveRange(
            this.apiContext.PlayerEventData.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerEventItems.RemoveRange(
            this.apiContext.PlayerEventItems.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerEventRewards.RemoveRange(
            this.apiContext.PlayerEventRewards.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerEventPassives.RemoveRange(
            this.apiContext.PlayerEventPassives.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerDmodeInfos.RemoveRange(
            this.apiContext.PlayerDmodeInfos.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerDmodeCharas.RemoveRange(
            this.apiContext.PlayerDmodeCharas.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerDmodeDungeons.RemoveRange(
            this.apiContext.PlayerDmodeDungeons.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerDmodeServitorPassives.RemoveRange(
            this.apiContext.PlayerDmodeServitorPassives.Where(
                x => x.DeviceAccountId == deviceAccountId
            )
        );
        this.apiContext.PlayerDmodeExpeditions.RemoveRange(
            this.apiContext.PlayerDmodeExpeditions.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerUseItems.RemoveRange(
            this.apiContext.PlayerUseItems.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PlayerSummonTickets.RemoveRange(
            this.apiContext.PlayerSummonTickets.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.Emblems.RemoveRange(
            this.apiContext.Emblems.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.QuestEvents.RemoveRange(
            this.apiContext.QuestEvents.Where(x => x.DeviceAccountId == deviceAccountId)
        );
        this.apiContext.PartyPowers.RemoveRange(
            this.apiContext.PartyPowers.Where(x => x.DeviceAccountId == deviceAccountId)
        );
    }

    public async Task Reset()
    {
        // Unlike importing, this will not preserve the viewer id
        await this.Create();
    }

    public IQueryable<DbPlayer> Load()
    {
        return this.apiContext.Players
            .Where(x => x.AccountId == this.playerIdentityService.AccountId)
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
            .Include(x => x.EquippedStampList)
            .Include(x => x.QuestEvents)
            .Include(x => x.PartyPower)
            .AsSplitQuery();
    }

    public async Task Create()
    {
        string deviceAccountId = this.playerIdentityService.AccountId;

        this.logger.LogInformation("Creating new savefile for account ID {id}", deviceAccountId);

        await using IDbContextTransaction transaction =
            await this.apiContext.Database.BeginTransactionAsync();

        this.Delete();
        await this.apiContext.SaveChangesAsync();

        this.apiContext.Players.Add(
            new() { AccountId = deviceAccountId, SavefileVersion = this.maxSavefileVersion }
        );

        DbPlayerUserData userData =
            new(deviceAccountId) {
#if DEBUG
                TutorialStatus = 10151,
#endif
                Crystal = 1_200_000 };

        apiContext.PlayerUserData.Add(userData);
        await this.AddDefaultParties(deviceAccountId);
        await this.AddDefaultCharacters(deviceAccountId);
        this.AddDefaultEquippedStamps();
        this.AddShopInfo();
        this.AddDefaultEmblem();

        await this.apiContext.SaveChangesAsync();

        await transaction.CommitAsync();
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
        await this.unitRepository.AddCharas(DefaultSavefileData.Characters);
    }

    private void AddDefaultEquippedStamps()
    {
        this.apiContext.EquippedStamps.AddRange(
            Enumerable
                .Range(1, StampService.EquipListSize)
                .Select(
                    x =>
                        new DbEquippedStamp()
                        {
                            DeviceAccountId = this.playerIdentityService.AccountId,
                            StampId = 0,
                            Slot = x
                        }
                )
        );
    }

    private void AddShopInfo()
    {
        this.apiContext.PlayerShopInfos.Add(
            new DbPlayerShopInfo() { DeviceAccountId = this.playerIdentityService.AccountId, }
        );
    }

    private void AddDefaultEmblem()
    {
        this.apiContext.Emblems.Add(
            new DbEmblem
            {
                DeviceAccountId = playerIdentityService.AccountId,
                EmblemId = DefaultSavefileData.DefaultEmblem,
                GetTime = DateTimeOffset.UnixEpoch,
                IsNew = false
            }
        );
    }

    internal static class DefaultSavefileData
    {
        public static readonly ImmutableList<Charas> Characters = MasterAsset.CharaData.Enumerable
            .Where(x => x.Rarity == 3 && x.IsPlayable)
            .Select(x => x.Id)
            .Append(Charas.ThePrince)
            .ToImmutableList();

        public const int PartySlotCount = 54;

        public const Emblems DefaultEmblem = Emblems.DragonbloodPrince;
    }
}

file static class Extensions
{
    public static IEnumerable<TDest> MapWithDeviceAccount<TDest>(
        this IEnumerable<object>? source,
        IMapper mapper,
        string deviceAccountId
    )
        where TDest : IDbHasAccountId
    {
        return (source ?? new List<object>())
            .AsParallel()
            .WithMergeOptions(ParallelMergeOptions.NotBuffered)
            .Select(
                x =>
                    mapper.Map<TDest>(
                        x,
                        opts => opts.AfterMap((src, dest) => dest.DeviceAccountId = deviceAccountId)
                    )
            )
            .AsEnumerable();
    }

    public static TDest MapWithDeviceAccount<TDest>(
        this object? source,
        IMapper mapper,
        string deviceAccountId
    )
        where TDest : IDbHasAccountId
    {
        return mapper.Map<TDest>(
            source,
            opts => opts.AfterMap((_, dest) => dest.DeviceAccountId = deviceAccountId)
        );
    }
}
