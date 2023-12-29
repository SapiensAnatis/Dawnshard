using System.Collections.Immutable;
using System.Diagnostics;
using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Features.Stamp;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using NuGet.Packaging;

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

        this.logger.LogInformation(
            "Beginning savefile import for account {accountId}",
            this.playerIdentityService.AccountId
        );

        try
        {
            await using IDbContextTransaction transaction =
                await this.apiContext.Database.BeginTransactionAsync();

            await this.Delete();

            this.logger.LogDebug(
                "Deleting savedata step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            DbPlayer player = await this.apiContext.Players.FirstAsync(
                x => x.ViewerId == this.playerIdentityService.ViewerId
            );

            player.SavefileVersion = 0;

            this.logger.LogDebug(
                "Mapping DbPlayer step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            // This has JsonRequired so this should never be triggered
            ArgumentNullException.ThrowIfNull(savefile.user_data);

            player.UserData = mapper.Map<DbPlayerUserData>(savefile.user_data);
            player.UserData.Crystal += 1_200_000;

            this.logger.LogDebug(
                "Mapping DbPlayerUserData step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.CharaList = savefile.chara_list.Map<DbPlayerCharaData>(mapper);

            this.logger.LogDebug(
                "Mapping DbPlayerCharaData step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.DragonReliabilityList =
                savefile.dragon_reliability_list.Map<DbPlayerDragonReliability>(mapper);

            this.logger.LogDebug(
                "Mapping DbPlayerDragonReliability step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            // Build key id mappings for dragons and talismans
            Dictionary<long, DbPlayerDragonData> dragonKeyIds = new();

            foreach (DragonList d in savefile.dragon_list ?? new List<DragonList>())
            {
                ulong oldKeyId = d.dragon_key_id;
                DbPlayerDragonData dbEntry = d.Map<DbPlayerDragonData>(mapper);
                player.DragonList.Add(dbEntry);

                dragonKeyIds.TryAdd((long)oldKeyId, dbEntry);
            }

            this.logger.LogDebug(
                "Mapping DbPlayerDragonData step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            Dictionary<long, DbTalisman> talismanKeyIds = new();

            foreach (TalismanList t in savefile.talisman_list ?? new List<TalismanList>())
            {
                ulong oldKeyId = t.talisman_key_id;
                DbTalisman dbEntry = t.Map<DbTalisman>(mapper);
                player.TalismanList.Add(dbEntry);

                talismanKeyIds.TryAdd((long)oldKeyId, dbEntry);
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

            this.AddShopInfo(player);

            this.logger.LogDebug(
                "Adding shop info step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            if (savefile.party_list is not null)
            {
                // Update key ids in parties
                List<DbParty> parties = savefile.party_list.Map<DbParty>(mapper);

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

                player.PartyList.AddRange(parties);
            }
            else
            {
                await this.AddDefaultParties(player);
            }

            this.logger.LogDebug(
                "Mapping DbParty step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.AbilityCrestList.AddRange(
                savefile.ability_crest_list.Map<DbAbilityCrest>(mapper)
            );

            this.logger.LogDebug(
                "Mapping DbAbilityCrest step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.WeaponBodyList.AddRange(savefile.weapon_body_list.Map<DbWeaponBody>(mapper));

            this.logger.LogDebug(
                "Mapping DbWeaponBody step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.QuestList.AddRange(savefile.quest_list.Map<DbQuest>(mapper));

            this.logger.LogDebug(
                "Mapping DbQuest step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.StoryStates.AddRange(savefile.quest_story_list.Map<DbPlayerStoryState>(mapper));

            this.logger.LogDebug(
                "Mapping DbPlayerStoryState (QuestStoryList) step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.StoryStates.AddRange(savefile.unit_story_list.Map<DbPlayerStoryState>(mapper));

            this.logger.LogDebug(
                "Mapping DbPlayerStoryState (UnitStoryList) step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.StoryStates.AddRange(savefile.castle_story_list.Map<DbPlayerStoryState>(mapper));

            this.logger.LogDebug(
                "Mapping DbPlayerStoryState (CastleStoryList) step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.MaterialList.AddRange(savefile.material_list.Map<DbPlayerMaterial>(mapper));

            this.logger.LogDebug(
                "Mapping DbPlayerMaterial step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.BuildList.AddRange(savefile.build_list.Map<DbFortBuild>(mapper));

            this.logger.LogDebug(
                "Mapping DbFortBuild step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.WeaponSkinList.AddRange(savefile.weapon_skin_list.Map<DbWeaponSkin>(mapper));

            this.logger.LogDebug(
                "Mapping DbWeaponSkin step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.WeaponPassiveAbilityList.AddRange(
                savefile.weapon_passive_ability_list.Map<DbWeaponPassiveAbility>(mapper)
            );

            this.logger.LogDebug(
                "Mapping DbWeaponPassibeAbility step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.DragonGiftList.AddRange(
                savefile.dragon_gift_list.Map<DbPlayerDragonGift>(mapper)
            );

            this.logger.LogDebug(
                "Mapping DbPlayerDragonGift step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.EquippedStampList.AddRange(
                savefile.equip_stamp_list.Map<DbEquippedStamp>(mapper)
            );

            this.logger.LogDebug(
                "Mapping DbEquippedStamp step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.Trades.AddRange(savefile.user_treasure_trade_list.Map<DbPlayerTrade>(mapper));

            this.logger.LogDebug(
                "Mapping DbPlayerTrade step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.SummonTickets.AddRange(savefile.summon_ticket_list.Map<DbSummonTicket>(mapper));

            this.logger.LogDebug(
                "Mapping DbSummonTicket step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            if (
                savefile.user_data.emblem_id != Emblems.DragonbloodPrince
                && await this.apiContext.Emblems.FindAsync(
                    player.ViewerId,
                    savefile.user_data.emblem_id
                ) == null
            )
            {
                player.Emblems.Add(
                    new DbEmblem
                    {
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

            player.PartyPower = savefile.party_power_data.Map<DbPartyPower>(mapper);

            this.logger.LogDebug(
                "Mapping DbPartyPower step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.QuestEvents = savefile.quest_event_list.Map<DbQuestEvent>(mapper);

            this.logger.LogDebug(
                "Mapping DbQuestEvent step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.QuestTreasureList = savefile.quest_treasure_list.Map<DbQuestTreasureList>(
                mapper
            );

            this.logger.LogDebug(
                "Mapping DbQuestTreasureList step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.QuestWalls = savefile.quest_wall_list.Map<DbPlayerQuestWall>(mapper);

            this.logger.LogDebug(
                "Mapping DbPlayerQuestWall step done after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            this.logger.LogInformation(
                "Mapping completed after {t} ms",
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.UserData.LastSaveImportTime = DateTimeOffset.UtcNow;

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
            this.apiContext.ChangeTracker.Clear();
            throw;
        }
    }

    private async Task Delete()
    {
        long viewerId = this.playerIdentityService.ViewerId;

        // Options commented out have been excluded from save import deletion process.
        // They will still be deleted by cascade delete when a player is actually deleted
        // without being re-added as they are in save imports.
        await this.apiContext.PlayerUserData.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerCharaData.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerDragonReliability.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerDragonData.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerAbilityCrests.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerStoryState.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerQuests.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await this.apiContext.PlayerParties.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await this.apiContext.PlayerPartyUnits.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerWeapons.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await this.apiContext.PlayerMaterials.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerTalismans.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerFortBuilds.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerWeaponSkins.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerPassiveAbilities.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerDragonGifts.Where(
            x => x.ViewerId == viewerId && x.DragonGiftId >= DragonGifts.FourLeafClover
        )
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerMissions.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.EquippedStamps.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerShopInfos.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerTrades.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await this.apiContext.PlayerEventData.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerEventItems.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerEventRewards.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerEventPassives.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        // this.apiContext.PlayerDmodeInfos.RemoveRange(
        //     this.apiContext.PlayerDmodeInfos.Where(x => x.ViewerId == viewerId)
        // );
        // this.apiContext.PlayerDmodeCharas.RemoveRange(
        //     this.apiContext.PlayerDmodeCharas.Where(x => x.ViewerId == viewerId)
        // );
        // this.apiContext.PlayerDmodeDungeons.RemoveRange(
        //     this.apiContext.PlayerDmodeDungeons.Where(x => x.ViewerId == viewerId)
        // );
        // this.apiContext.PlayerDmodeServitorPassives.RemoveRange(
        //     this.apiContext.PlayerDmodeServitorPassives.Where(
        //         x => x.DeviceAccountId == deviceAccountId
        //     )
        // );
        // this.apiContext.PlayerDmodeExpeditions.RemoveRange(
        //     this.apiContext.PlayerDmodeExpeditions.Where(x => x.ViewerId == viewerId)
        // );
        await this.apiContext.PlayerUseItems.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerSummonTickets.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        // this.apiContext.Emblems.RemoveRange(
        //     this.apiContext.Emblems.Where(x => x.ViewerId == viewerId)
        // );
        await this.apiContext.QuestEvents.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await this.apiContext.QuestTreasureList.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await this.apiContext.PartyPowers.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await this.apiContext.PlayerQuestWalls.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
    }

    public async Task Reset()
    {
        // Unlike importing, this will not preserve the viewer id
        await this.Create();
    }

    public IQueryable<DbPlayer> Load()
    {
        return this.apiContext.Players.Where(
            x => x.AccountId == this.playerIdentityService.AccountId
        )
            .Include(x => x.UserData)
            .Include(x => x.AbilityCrestList)
            .Include(x => x.CharaList)
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
            .Include(x => x.QuestTreasureList)
            .Include(x => x.QuestWalls)
            .AsSplitQuery();
    }

    public async Task<DbPlayer> Create()
    {
        string deviceAccountId = this.playerIdentityService.AccountId;

        return await this.Create(deviceAccountId);
    }

    public async Task<DbPlayer> Create(string deviceAccountId)
    {
        this.logger.LogInformation("Creating new savefile for account ID {id}", deviceAccountId);

        await using IDbContextTransaction transaction =
            await this.apiContext.Database.BeginTransactionAsync();

        DbPlayer player =
            new()
            {
                AccountId = deviceAccountId,
                SavefileVersion = this.maxSavefileVersion,
                UserData = new() { Crystal = 1_200_000 }
            };

        this.apiContext.Players.Add(player);
        await this.apiContext.SaveChangesAsync();

        using IDisposable ctx = this.playerIdentityService.StartUserImpersonation(
            player.ViewerId,
            player.AccountId
        );

        await this.AddDefaultParties(player);
        await this.AddDefaultCharacters();
        this.AddDefaultEquippedStamps(player);
        this.AddShopInfo(player);
        this.AddDefaultEmblem(player);

        await this.apiContext.SaveChangesAsync();

        await transaction.CommitAsync();

        return player;
    }

    private async Task AddDefaultParties(DbPlayer player)
    {
        player.PartyList.AddRange(
            Enumerable
                .Range(1, DefaultSavefileData.PartySlotCount)
                .Select(
                    x =>
                        new DbParty()
                        {
                            PartyName = "Default",
                            PartyNo = x,
                            Units = new List<DbPartyUnit>()
                            {
                                new()
                                {
                                    PartyNo = x,
                                    UnitNo = 1,
                                    CharaId = Charas.ThePrince
                                },
                                new()
                                {
                                    PartyNo = x,
                                    UnitNo = 2,
                                    CharaId = Charas.Empty
                                },
                                new()
                                {
                                    PartyNo = x,
                                    UnitNo = 3,
                                    CharaId = Charas.Empty
                                },
                                new()
                                {
                                    PartyNo = x,
                                    UnitNo = 4,
                                    CharaId = Charas.Empty
                                }
                            }
                        }
                )
        );
    }

    private async Task AddDefaultCharacters()
    {
        await this.unitRepository.AddCharas(DefaultSavefileData.Characters);
    }

    private void AddDefaultEquippedStamps(DbPlayer player)
    {
        player.EquippedStampList.AddRange(
            Enumerable
                .Range(1, StampService.EquipListSize)
                .Select(x => new DbEquippedStamp() { StampId = 0, Slot = x })
        );
    }

    private void AddShopInfo(DbPlayer player)
    {
        player.ShopInfo = new DbPlayerShopInfo();
    }

    private void AddDefaultEmblem(DbPlayer player)
    {
        player.Emblems.Add(
            new DbEmblem
            {
                EmblemId = DefaultSavefileData.DefaultEmblem,
                GetTime = DateTimeOffset.UnixEpoch,
                IsNew = false
            }
        );
    }

    internal static class DefaultSavefileData
    {
        public static readonly ImmutableList<Charas> Characters = MasterAsset
            .CharaData.Enumerable.Where(x => x.Rarity == 3 && x.IsPlayable)
            .Select(x => x.Id)
            .Append(Charas.ThePrince)
            .ToImmutableList();

        public const int PartySlotCount = 54;

        public const Emblems DefaultEmblem = Emblems.DragonbloodPrince;
    }
}

file static class Extensions
{
    public static List<TDest> Map<TDest>(this IEnumerable<object>? source, IMapper mapper)
        where TDest : IDbPlayerData =>
        (source ?? new List<object>())
            .AsParallel()
            .WithMergeOptions(ParallelMergeOptions.NotBuffered)
            .Select(mapper.Map<TDest>)
            .ToList();

    public static TDest Map<TDest>(this object? source, IMapper mapper)
        where TDest : IDbPlayerData
    {
        return mapper.Map<TDest>(source);
    }
}
