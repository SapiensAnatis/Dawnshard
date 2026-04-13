using System.Collections.Immutable;
using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Features.CoOp.Stamps;
using DragaliaAPI.Features.Login.SavefileUpdate;
using DragaliaAPI.Infrastructure.Metrics;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Features.Login.Savefile;

internal sealed partial class SavefileService(
    ApiContext apiContext,
    IDistributedCache cache,
    ILogger<SavefileService> logger,
    IPlayerIdentityService playerIdentityService,
    IEnumerable<ISavefileUpdate> savefileUpdates,
    IDragaliaApiMetrics metrics
) : ISavefileService
{
    private const int RecheckLockMs = 1000;
    private const int LockFailsafeExpiryMin = 5;

    private readonly int maxSavefileVersion = savefileUpdates
        .Select(x => x.SavefileVersion)
        .DefaultIfEmpty()
        .Max();

    private const int MaxLevel = 250;
    private static readonly int MaxTotalExp = MasterAsset.UserLevel[MaxLevel].TotalExp;

    private static class RedisSchema
    {
        public static string PendingImport(string deviceAccountId) =>
            $":pending_save_import:{deviceAccountId}";
    }

    private static readonly DistributedCacheEntryOptions RedisOptions = new()
    {
        // Keys should be automatically removed, but just in case
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(LockFailsafeExpiryMin),
    };

    /// <summary>
    /// Thread safe version of <see cref="Import(LoadIndexResponse)"/>.
    /// </summary>
    /// <param name="savefile">The savefile to import/</param>
    /// <returns>The task.</returns>
    public async Task ThreadSafeImport(LoadIndexResponse savefile)
    {
        string key = RedisSchema.PendingImport(playerIdentityService.AccountId);

        if (!string.IsNullOrEmpty(await cache.GetStringAsync(key)))
        {
            while (!string.IsNullOrEmpty(await cache.GetStringAsync(key)))
            {
                Log.SavefileImportIsLockedWaiting(logger);
                await Task.Delay(RecheckLockMs);
            }

            Log.SavefileImportLockReleased(logger);
            return;
        }

        try
        {
            // Place a lock preventing any concurrent save imports
            await cache.SetStringAsync(
                RedisSchema.PendingImport(playerIdentityService.AccountId),
                "true",
                RedisOptions
            );

            await Import(savefile);
        }
        finally
        {
            await cache.RemoveAsync(RedisSchema.PendingImport(playerIdentityService.AccountId));
        }
    }

    /// <summary>
    /// Import a savefile.
    /// </summary>
    /// <param name="savefile">Savefile data.</param>
    /// <returns>The task.</returns>
    /// <remarks>Not thread safe if called for the same account id from two different threads.</remarks>
    public async Task Import(LoadIndexResponse savefile)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        Log.BeginningSavefileImportForAccount(logger, playerIdentityService.AccountId);

        if (!Validate(savefile))
        {
            Log.SavefileToBeImportedIsInvalid(logger);
            return;
        }

        Log.ValidatingSaveFileStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

        try
        {
            await using IDbContextTransaction transaction =
                await apiContext.Database.BeginTransactionAsync();

            await Delete();

            Log.DeletingSavedataStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            DbPlayer player = await apiContext.Players.FirstAsync(x =>
                x.ViewerId == playerIdentityService.ViewerId
            );

            player.SavefileVersion = 0;

            Log.MappingDbPlayerStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            // This has JsonRequired so this should never be triggered
            ArgumentNullException.ThrowIfNull(savefile.UserData);

            player.UserData = savefile.UserData.MapToDbPlayerUserData(
                playerIdentityService.ViewerId
            );

            // TODO: What was the actual maximum dragon storage you could get?
            int cappedDragonStorage = Math.Min(savefile.UserData.MaxDragonQuantity, 500);
            player.UserData.MaxDragonQuantity = cappedDragonStorage;

            // You can run into softlocks if your XP gets too high
            int cappedExp = Math.Min(savefile.UserData.Exp, MaxTotalExp);
            player.UserData.Exp = cappedExp;

            Log.MappingDbPlayerUserDataStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.CharaList = savefile
                .CharaList.ParallelMap(playerIdentityService.ViewerId, CharaMapper.ToDbPlayerChara)
                .ToList();

            // Characters imported with is_temporary = true become permanently inaccessible
            // when their event is not active, so clear the flag on savefile import.
            foreach (DbPlayerCharaData chara in player.CharaList)
            {
                chara.IsTemporary = false;
            }

            Log.MappingDbPlayerCharaDataStepDoneAfterMs(
                logger,
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.DragonReliabilityList = savefile
                .DragonReliabilityList.ParallelMap(
                    playerIdentityService.ViewerId,
                    DragonReliabilityMapper.ToDbPlayerDragonReliability
                )
                .ToList();

            Log.MappingDbPlayerDragonReliabilityStepDoneAfterMs(
                logger,
                stopwatch.Elapsed.TotalMilliseconds
            );

            // Build key id mappings for dragons and talismans
            Dictionary<long, DbPlayerDragonData> dragonKeyIds = new();

            foreach (DragonList d in savefile.DragonList?.Take(cappedDragonStorage) ?? [])
            {
                ulong oldKeyId = d.DragonKeyId;
                DbPlayerDragonData dbEntry = d.MapToDbPlayerDragonData(
                    playerIdentityService.ViewerId
                );
                dbEntry.DragonKeyId = 0; // Prevent EF from thinking this is an update
                player.DragonList.Add(dbEntry);

                dragonKeyIds.TryAdd((long)oldKeyId, dbEntry);
            }

            Log.MappingDbPlayerDragonDataStepDoneAfterMs(
                logger,
                stopwatch.Elapsed.TotalMilliseconds
            );

            Dictionary<long, DbTalisman> talismanKeyIds = new();

            foreach (TalismanList t in savefile.TalismanList ?? new List<TalismanList>())
            {
                ulong oldKeyId = t.TalismanKeyId;
                DbTalisman dbEntry = t.MapToDbTalisman(playerIdentityService.ViewerId);
                dbEntry.TalismanKeyId = 0;
                player.TalismanList.Add(dbEntry);

                talismanKeyIds.TryAdd((long)oldKeyId, dbEntry);
            }

            Log.MappingDbTalismanStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            // Must save changes for key ids to update
            await apiContext.SaveChangesAsync();

            Log.SaveChangesAsync1StepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            AddShopInfo(player);

            Log.AddingShopInfoStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            if (savefile.PartyList is not null)
            {
                // Update key ids in parties
                List<DbParty> parties = savefile
                    .PartyList.ParallelMap(playerIdentityService.ViewerId, PartyMapper.MapToDbParty)
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

                player.PartyList.AddRange(parties);
            }
            else
            {
                AddDefaultParties(player);
            }

            Log.MappingDbPartyStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.AbilityCrestList.AddRange(
                savefile.AbilityCrestList.Select(x =>
                    x.MapToDbAbilityCrest(playerIdentityService.ViewerId)
                )
            );

            Log.MappingDbAbilityCrestStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.WeaponBodyList.AddRange(
                savefile.WeaponBodyList.Select(x =>
                    x.MapToDbWeaponBody(playerIdentityService.ViewerId)
                )
            );

            Log.MappingDbWeaponBodyStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.QuestList.AddRange(
                savefile.QuestList.ParallelMap(
                    playerIdentityService.ViewerId,
                    QuestMapper.MapToDbQuest
                )
            );

            Log.MappingDbQuestStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.StoryStates.AddRange(
                savefile.QuestStoryList.ParallelMap(
                    playerIdentityService.ViewerId,
                    StoryMapper.MapToDbPlayerStoryState
                )
            );

            Log.MappingDbPlayerStoryStateQuestStoryListStepDoneAfterMs(
                logger,
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.StoryStates.AddRange(
                savefile.UnitStoryList.ParallelMap(
                    playerIdentityService.ViewerId,
                    StoryMapper.MapToDbPlayerStoryState
                )
            );

            Log.MappingDbPlayerStoryStateUnitStoryListStepDoneAfterMs(
                logger,
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.StoryStates.AddRange(
                savefile.CastleStoryList.ParallelMap(
                    playerIdentityService.ViewerId,
                    StoryMapper.MapToDbPlayerStoryState
                )
            );

            Log.MappingDbPlayerStoryStateCastleStoryListStepDoneAfterMs(
                logger,
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.MaterialList.AddRange(
                savefile.MaterialList.ParallelMap(
                    playerIdentityService.ViewerId,
                    MaterialMapper.MapToDbPlayerMaterial
                )
            );

            Log.MappingDbPlayerMaterialStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.BuildList.AddRange(
                savefile.BuildList.ParallelMap(
                    playerIdentityService.ViewerId,
                    FortBuildMapper.MapToDbFortBuild
                )
            );

            foreach (DbFortBuild fortBuild in player.BuildList)
            {
                fortBuild.BuildId = 0;
            }

            Log.MappingDbFortBuildStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.WeaponSkinList.AddRange(
                savefile.WeaponSkinList.ParallelMap(
                    playerIdentityService.ViewerId,
                    WeaponSkinMapper.MapToDbWeaponSkin
                )
            );

            Log.MappingDbWeaponSkinStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.WeaponPassiveAbilityList.AddRange(
                savefile.WeaponPassiveAbilityList.ParallelMap(
                    playerIdentityService.ViewerId,
                    WeaponPassiveAbilityMapper.MapToDbWeaponPassiveAbility
                )
            );

            Log.MappingDbWeaponPassibeAbilityStepDoneAfterMs(
                logger,
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.DragonGiftList.AddRange(
                savefile.DragonGiftList.ParallelMap(
                    playerIdentityService.ViewerId,
                    DragonGiftMapper.MapToDbPlayerDragonGift
                )
            );

            Log.MappingDbPlayerDragonGiftStepDoneAfterMs(
                logger,
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.EquippedStampList.AddRange(
                savefile.EquipStampList.ParallelMap(
                    playerIdentityService.ViewerId,
                    StampMapper.MapToDbEquippedStamp
                )
            );

            Log.MappingDbEquippedStampStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.Trades.AddRange(
                savefile.UserTreasureTradeList.ParallelMap(
                    playerIdentityService.ViewerId,
                    TreasureTradeMapper.MapToDbPlayerTrade
                )
            );

            Log.MappingDbPlayerTradeStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.SummonTickets.AddRange(
                savefile.SummonTicketList.ParallelMap(
                    playerIdentityService.ViewerId,
                    SummonMapper.MapToDbSummonTicket
                )
            );

            foreach (DbSummonTicket ticket in player.SummonTickets)
            {
                ticket.KeyId = 0;
            }

            Log.MappingDbSummonTicketStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            if (
                savefile.UserData.EmblemId != Emblems.DragonbloodPrince
                && await apiContext.Emblems.FindAsync(player.ViewerId, savefile.UserData.EmblemId)
                    == null
            )
            {
                player.Emblems.Add(
                    new()
                    {
                        EmblemId = savefile.UserData.EmblemId,
                        GetTime = DateTimeOffset.UnixEpoch,
                        IsNew = false,
                    }
                );
            }

            Log.AddingDbEmblemStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.PartyPower = savefile.PartyPowerData.MapToDbPartyPower(
                playerIdentityService.ViewerId
            );

            Log.MappingDbPartyPowerStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.QuestEvents = savefile
                .QuestEventList.ParallelMap(
                    playerIdentityService.ViewerId,
                    QuestEventMapper.MapToDbQuestEvent
                )
                .ToList();

            Log.MappingDbQuestEventStepDoneAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.QuestTreasureList = savefile
                .QuestTreasureList.ParallelMap(
                    playerIdentityService.ViewerId,
                    QuestTreasureMapper.MapToDbQuestTreasureList
                )
                .ToList();

            Log.MappingDbQuestTreasureListStepDoneAfterMs(
                logger,
                stopwatch.Elapsed.TotalMilliseconds
            );

            player.QuestWalls = savefile
                .QuestWallList.ParallelMap(
                    playerIdentityService.ViewerId,
                    WallMapper.MapToDbPlayerQuestWall
                )
                .ToList();

            Log.MappingDbPlayerQuestWallStepDoneAfterMs(
                logger,
                stopwatch.Elapsed.TotalMilliseconds
            );

            Log.MappingCompletedAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);

            player.LastSavefileImportTime = DateTimeOffset.UtcNow;
            player.SavefileOrigin = savefile.Origin;

            // Set helper to default. This will break if you try and delete Euden from your save, but we protect
            // against this in Validate()
            player.Helper = new() { CharaId = Charas.ThePrince };

            await apiContext.SaveChangesAsync();
            await transaction.CommitAsync();

            metrics.OnSaveImport(savefile);

            Log.SavedChangesAfterMs(logger, stopwatch.Elapsed.TotalMilliseconds);
        }
        catch
        {
            apiContext.ChangeTracker.Clear();
            throw;
        }
    }

    private bool Validate(LoadIndexResponse savefile)
    {
        // Ensure the save does not contain any invalid IDs for dragons and other entities - the server frequently
        // does dict[key] lookups in the master asset which throw errors for out-of-range IDs. Such invalid data should
        // be rejected outright rather than allowed to break later.

        bool hasEuden = false;

        foreach (CharaList chara in savefile.CharaList)
        {
            if (!MasterAsset.CharaData.ContainsKey(chara.CharaId))
            {
                Log.InvalidCharacterID(logger, chara.CharaId);
                return false;
            }

            if (chara.CharaId == Charas.ThePrince)
            {
                hasEuden = true;
            }
        }

        if (!hasEuden)
        {
            Log.SavefileDoesNotHaveThePrince(logger);
            return false;
        }

        foreach (DragonList dragon in savefile.DragonList)
        {
            if (!MasterAsset.DragonData.ContainsKey(dragon.DragonId))
            {
                Log.InvalidDragonID(logger, dragon.DragonId);
                return false;
            }
        }

        return true;
    }

    private async Task Delete()
    {
        long viewerId = playerIdentityService.ViewerId;

        // The usage of ExecuteDelete can leave the change tracker in a corrupted state where it attempts to
        // track rows that no longer exist. This can cause an error if you attempt to import a save on first
        // login, for example.
        // Since nothing in the tracker is going to be relevant in the face of total deletion, clear the
        // tracker entirely.
        apiContext.ChangeTracker.Clear();

        // Options commented out have been excluded from save import deletion process.
        // They will still be deleted by cascade delete when a player is actually deleted
        // without being re-added as they are in save imports.
        await apiContext
            .PlayerHelperUseDates.Where(x => x.HelperViewerId == viewerId)
            .ExecuteDeleteAsync();
        await apiContext.PlayerHelpers.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerUserData.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerCharaData.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext
            .PlayerDragonReliability.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await apiContext.PlayerDragonData.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext
            .PlayerAbilityCrests.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await apiContext.PlayerStoryState.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerQuests.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerParties.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerPartyUnits.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerWeapons.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerMaterials.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerTalismans.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerFortBuilds.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerWeaponSkins.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext
            .PlayerPassiveAbilities.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        await apiContext
            .PlayerDragonGifts.Where(x =>
                x.ViewerId == viewerId && x.DragonGiftId >= DragonGifts.FourLeafClover
            )
            .ExecuteDeleteAsync();
        await apiContext.PlayerMissions.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.EquippedStamps.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerShopInfos.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerTrades.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerEventData.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerEventItems.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerEventRewards.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext
            .PlayerEventPassives.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        // apiContext.PlayerDmodeInfos.RemoveRange(
        //     apiContext.PlayerDmodeInfos.Where(x => x.ViewerId == viewerId)
        // );
        // apiContext.PlayerDmodeCharas.RemoveRange(
        //     apiContext.PlayerDmodeCharas.Where(x => x.ViewerId == viewerId)
        // );
        // apiContext.PlayerDmodeDungeons.RemoveRange(
        //     apiContext.PlayerDmodeDungeons.Where(x => x.ViewerId == viewerId)
        // );
        // apiContext.PlayerDmodeServitorPassives.RemoveRange(
        //     apiContext.PlayerDmodeServitorPassives.Where(
        //         x => x.DeviceAccountId == deviceAccountId
        //     )
        // );
        // apiContext.PlayerDmodeExpeditions.RemoveRange(
        //     apiContext.PlayerDmodeExpeditions.Where(x => x.ViewerId == viewerId)
        // );
        await apiContext.PlayerUseItems.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext
            .PlayerSummonTickets.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        // apiContext.Emblems.RemoveRange(
        //     apiContext.Emblems.Where(x => x.ViewerId == viewerId)
        // );
        await apiContext.QuestEvents.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.QuestTreasureList.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PartyPowers.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext.PlayerQuestWalls.Where(x => x.ViewerId == viewerId).ExecuteDeleteAsync();
        await apiContext
            .CompletedDailyMissions.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
        // await this
        //     .apiContext.WallRewardDates.Where(x => x.ViewerId == viewerId)
        //     .ExecuteDeleteAsync();
        await apiContext
            .PlayerSummonHistory.Where(x => x.ViewerId == viewerId)
            .ExecuteDeleteAsync();
    }

    public async Task<DbPlayer> Create()
    {
        string deviceAccountId = playerIdentityService.AccountId;

        return await Create(deviceAccountId);
    }

    public async Task<DbPlayer> Create(string deviceAccountId)
    {
        Log.CreatingNewSavefileForAccountID(logger, deviceAccountId);

        await using IDbContextTransaction transaction =
            await apiContext.Database.BeginTransactionAsync();

        DbPlayer player = new()
        {
            AccountId = deviceAccountId,
            SavefileVersion = maxSavefileVersion,
            UserData = new(),
            DiamondData = new(),
            CreatedAt = DateTimeOffset.UtcNow,
        };

        apiContext.Players.Add(player);
        await apiContext.SaveChangesAsync();

        using IDisposable ctx = playerIdentityService.StartUserImpersonation(
            player.ViewerId,
            player.AccountId
        );

        AddDefaultParties(player);
        AddDefaultCharacters(player);
        AddDefaultEquippedStamps(player);
        AddShopInfo(player);
        AddDefaultEmblem(player);
        AddDefaultHelper(player);

        await apiContext.SaveChangesAsync();

        await transaction.CommitAsync();

        metrics.OnAccountCreated();

        return player;
    }

    private static void AddDefaultParties(DbPlayer player)
    {
        player.PartyList.AddRange(
            Enumerable
                .Range(1, DefaultSavefileData.PartySlotCount)
                .Select(x => new DbParty()
                {
                    PartyName = "Default",
                    PartyNo = x,
                    Units = new List<DbPartyUnit>()
                    {
                        new()
                        {
                            PartyNo = x,
                            UnitNo = 1,
                            CharaId = Charas.ThePrince,
                        },
                        new()
                        {
                            PartyNo = x,
                            UnitNo = 2,
                            CharaId = Charas.Empty,
                        },
                        new()
                        {
                            PartyNo = x,
                            UnitNo = 3,
                            CharaId = Charas.Empty,
                        },
                        new()
                        {
                            PartyNo = x,
                            UnitNo = 4,
                            CharaId = Charas.Empty,
                        },
                    },
                })
        );
    }

    private static void AddDefaultCharacters(DbPlayer player)
    {
        foreach (Charas c in DefaultSavefileData.Characters)
        {
            player.CharaList.Add(new(player.ViewerId, c));

            if (MasterAsset.CharaStories.TryGetValue((int)c, out StoryData? story))
            {
                player.StoryStates.Add(
                    new()
                    {
                        ViewerId = player.ViewerId,
                        StoryType = StoryTypes.Chara,
                        StoryId = story.StoryIds[0],
                        State = 0,
                    }
                );
            }
        }
    }

    private static void AddDefaultEquippedStamps(DbPlayer player)
    {
        player.EquippedStampList.AddRange(
            Enumerable
                .Range(1, StampService.EquipListSize)
                .Select(x => new DbEquippedStamp() { StampId = 0, Slot = x })
        );
    }

    private static void AddShopInfo(DbPlayer player)
    {
        player.ShopInfo = new();
    }

    private static void AddDefaultEmblem(DbPlayer player)
    {
        player.Emblems.Add(
            new()
            {
                EmblemId = DefaultSavefileData.DefaultEmblem,
                GetTime = DateTimeOffset.UnixEpoch,
                IsNew = false,
            }
        );
    }

    private static void AddDefaultHelper(DbPlayer player)
    {
        player.Helper = new() { CharaId = Charas.ThePrince };
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

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Savefile import is locked, waiting...")]
        public static partial void SavefileImportIsLockedWaiting(ILogger logger);

        [LoggerMessage(LogLevel.Information, "Savefile import lock released.")]
        public static partial void SavefileImportLockReleased(ILogger logger);

        [LoggerMessage(LogLevel.Information, "Beginning savefile import for account {AccountId}")]
        public static partial void BeginningSavefileImportForAccount(
            ILogger logger,
            string accountId
        );

        [LoggerMessage(LogLevel.Information, "Savefile to be imported is invalid")]
        public static partial void SavefileToBeImportedIsInvalid(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Validating save file step done after {t} ms")]
        public static partial void ValidatingSaveFileStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Deleting savedata step done after {t} ms")]
        public static partial void DeletingSavedataStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbPlayer step done after {t} ms")]
        public static partial void MappingDbPlayerStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbPlayerUserData step done after {t} ms")]
        public static partial void MappingDbPlayerUserDataStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbPlayerCharaData step done after {t} ms")]
        public static partial void MappingDbPlayerCharaDataStepDoneAfterMs(
            ILogger logger,
            double t
        );

        [LoggerMessage(LogLevel.Debug, "Mapping DbPlayerDragonReliability step done after {t} ms")]
        public static partial void MappingDbPlayerDragonReliabilityStepDoneAfterMs(
            ILogger logger,
            double t
        );

        [LoggerMessage(LogLevel.Debug, "Mapping DbPlayerDragonData step done after {t} ms")]
        public static partial void MappingDbPlayerDragonDataStepDoneAfterMs(
            ILogger logger,
            double t
        );

        [LoggerMessage(LogLevel.Debug, "Mapping DbTalisman step done after {t} ms")]
        public static partial void MappingDbTalismanStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "SaveChangesAsync() #1 step done after {t} ms")]
        public static partial void SaveChangesAsync1StepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Adding shop info step done after {t} ms")]
        public static partial void AddingShopInfoStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbParty step done after {t} ms")]
        public static partial void MappingDbPartyStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbAbilityCrest step done after {t} ms")]
        public static partial void MappingDbAbilityCrestStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbWeaponBody step done after {t} ms")]
        public static partial void MappingDbWeaponBodyStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbQuest step done after {t} ms")]
        public static partial void MappingDbQuestStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(
            LogLevel.Debug,
            "Mapping DbPlayerStoryState (QuestStoryList) step done after {t} ms"
        )]
        public static partial void MappingDbPlayerStoryStateQuestStoryListStepDoneAfterMs(
            ILogger logger,
            double t
        );

        [LoggerMessage(
            LogLevel.Debug,
            "Mapping DbPlayerStoryState (UnitStoryList) step done after {t} ms"
        )]
        public static partial void MappingDbPlayerStoryStateUnitStoryListStepDoneAfterMs(
            ILogger logger,
            double t
        );

        [LoggerMessage(
            LogLevel.Debug,
            "Mapping DbPlayerStoryState (CastleStoryList) step done after {t} ms"
        )]
        public static partial void MappingDbPlayerStoryStateCastleStoryListStepDoneAfterMs(
            ILogger logger,
            double t
        );

        [LoggerMessage(LogLevel.Debug, "Mapping DbPlayerMaterial step done after {t} ms")]
        public static partial void MappingDbPlayerMaterialStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbFortBuild step done after {t} ms")]
        public static partial void MappingDbFortBuildStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbWeaponSkin step done after {t} ms")]
        public static partial void MappingDbWeaponSkinStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbWeaponPassibeAbility step done after {t} ms")]
        public static partial void MappingDbWeaponPassibeAbilityStepDoneAfterMs(
            ILogger logger,
            double t
        );

        [LoggerMessage(LogLevel.Debug, "Mapping DbPlayerDragonGift step done after {t} ms")]
        public static partial void MappingDbPlayerDragonGiftStepDoneAfterMs(
            ILogger logger,
            double t
        );

        [LoggerMessage(LogLevel.Debug, "Mapping DbEquippedStamp step done after {t} ms")]
        public static partial void MappingDbEquippedStampStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbPlayerTrade step done after {t} ms")]
        public static partial void MappingDbPlayerTradeStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbSummonTicket step done after {t} ms")]
        public static partial void MappingDbSummonTicketStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Adding DbEmblem step done after {t} ms")]
        public static partial void AddingDbEmblemStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbPartyPower step done after {t} ms")]
        public static partial void MappingDbPartyPowerStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbQuestEvent step done after {t} ms")]
        public static partial void MappingDbQuestEventStepDoneAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Mapping DbQuestTreasureList step done after {t} ms")]
        public static partial void MappingDbQuestTreasureListStepDoneAfterMs(
            ILogger logger,
            double t
        );

        [LoggerMessage(LogLevel.Debug, "Mapping DbPlayerQuestWall step done after {t} ms")]
        public static partial void MappingDbPlayerQuestWallStepDoneAfterMs(
            ILogger logger,
            double t
        );

        [LoggerMessage(LogLevel.Information, "Mapping completed after {t} ms")]
        public static partial void MappingCompletedAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Information, "Saved changes after {t} ms")]
        public static partial void SavedChangesAfterMs(ILogger logger, double t);

        [LoggerMessage(LogLevel.Debug, "Invalid character ID: {CharaId}")]
        public static partial void InvalidCharacterID(ILogger logger, Charas charaId);

        [LoggerMessage(LogLevel.Debug, "Savefile does not have ThePrince")]
        public static partial void SavefileDoesNotHaveThePrince(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Invalid dragon ID: {DragonId}")]
        public static partial void InvalidDragonID(ILogger logger, DragonId dragonId);

        [LoggerMessage(LogLevel.Information, "Creating new savefile for account ID {id}")]
        public static partial void CreatingNewSavefileForAccountID(ILogger logger, string id);
    }
}

file static class Extensions
{
    public static IEnumerable<TDest> ParallelMap<TSource, TDest>(
        this IEnumerable<TSource>? source,
        long viewerId,
        Func<TSource, long, TDest> mapper
    )
        where TDest : IDbPlayerData
    {
        if (source == null)
        {
            return [];
        }

        return source
            .AsParallel()
            .WithMergeOptions(ParallelMergeOptions.NotBuffered)
            .Select(x => mapper(x, viewerId));
    }
}
