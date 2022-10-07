using System.Collections.Immutable;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Enums;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Enums;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services;

/// <summary>
/// Abstraction upon the context to access the database without exposing all the methods of DbContext.
/// Use this instead of directly injecting the context.
/// </summary>
public class ApiRepository : IApiRepository
{
    private readonly ApiContext _apiContext;

    public ApiRepository(ApiContext context)
    {
        _apiContext = context;
    }

    public virtual async Task AddNewDeviceAccount(string id, string hashedPassword)
    {
        await _apiContext.DeviceAccounts.AddAsync(new DbDeviceAccount(id, hashedPassword));
        await _apiContext.SaveChangesAsync();
    }

    public virtual async Task<DbDeviceAccount?> GetDeviceAccountById(string id)
    {
        return await _apiContext.DeviceAccounts.FirstOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task AddNewPlayerInfo(string deviceAccountId)
    {
        await _apiContext.SavefileUserData.AddAsync(
            DbSavefileUserDataFactory.Create(deviceAccountId)
        );
        await _apiContext.SaveChangesAsync();
    }

    public virtual IQueryable<DbSavefileUserData> GetPlayerInfo(string deviceAccountId)
    {
        IQueryable<DbSavefileUserData> infoQuery = _apiContext.SavefileUserData.Where(
            x => x.DeviceAccountId == deviceAccountId
        );

        if (infoQuery.Count() != 1)
            // Returning an empty IQueryable will almost certainly cause errors down the line.
            // Better stop here instead, where it's easier to debug with access to ApiContext.
            throw new InvalidOperationException(
                $"PlayerInfo query with id {deviceAccountId} returned {infoQuery.Count()} results."
            );

        return infoQuery;
    }

    public virtual async Task<ISet<int>> getTutorialFlags(string deviceAccountId)
    {
        DbSavefileUserData? saveFileUserData = await _apiContext.SavefileUserData.FindAsync(
            deviceAccountId
        );
        if (saveFileUserData == null)
        {
            throw new Exception($"No SaveFileData found for Account-Id: {deviceAccountId}");
        }
        int flags_ = saveFileUserData.TutorialFlag;
        return TutorialFlagUtil.ConvertIntToFlagIntList(flags_);
    }

    public virtual async Task setTutorialFlags(string deviceAccountId, ISet<int> tutorialFlags)
    {
        DbSavefileUserData? saveFileUserData = await _apiContext.SavefileUserData.FindAsync(
            deviceAccountId
        );
        if (saveFileUserData == null)
        {
            throw new Exception($"No SaveFileData found for Account-Id: {deviceAccountId}");
        }
        saveFileUserData.TutorialFlag = TutorialFlagUtil.ConvertFlagIntListToInt(
            tutorialFlags,
            saveFileUserData.TutorialFlag
        );
        int udpatedRows = await _apiContext.SaveChangesAsync();
    }

    public virtual async Task<
        Tuple<IEnumerable<Entity>, IEnumerable<Entity>, IEnumerable<Entity>>
    > commitSummonResults(string deviceAccountId, IEnumerable<SummonEntity> summonResult)
    {
        DbSavefileUserData? saveFileUserData = await _apiContext.SavefileUserData.FindAsync(
            deviceAccountId
        );
        if (saveFileUserData == null)
        {
            throw new Exception($"No SaveFileData found for Account-Id: {deviceAccountId}");
        }
        List<SummonEntity> convertedEntities = new List<SummonEntity>();
        List<Entity> newEntities = new List<Entity>();
        List<Entity> sentToGiftsEntities = new List<Entity>();

        DbSet<DbPlayerCharaData> playerCharaData = _apiContext.PlayerCharaData;
        DbSet<DbPlayerDragonData> playerDragonData = _apiContext.PlayerDragonData;

        ImmutableDictionary<Charas, DbPlayerCharaData> playerCharas = playerCharaData
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ToImmutableDictionary(chara => chara.CharaId);
        ImmutableDictionary<Dragons, DbPlayerDragonData> playerDragons = playerDragonData
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ToImmutableDictionary(dragon => dragon.DragonId);
        ImmutableDictionary<Dragons, DbPlayerDragonReliability> playerDragonsReliability =
            _apiContext.PlayerDragonReliability
                .Where(x => x.DeviceAccountId == deviceAccountId)
                .ToImmutableDictionary(dragon => dragon.DragonId);

        //TODO: storage size limit for dragons
        int dragonStorageSize = -1;
        int dragonStorageCount = playerDragons.Count;

        foreach (SummonEntity e in summonResult)
        {
            switch ((EntityTypes)e.entity_type)
            {
                case EntityTypes.CHARA:
                    if (
                        newEntities.Exists(
                            x => x.entity_id == e.entity_id && x.entity_type == e.entity_type
                        ) || playerCharas.ContainsKey((Charas)e.entity_id)
                    )
                    {
                        DbPlayerCharaData newChar = new DbPlayerCharaData()
                        {
                            DeviceAccountId = deviceAccountId,
                            CharaId = (Charas)e.entity_id
                        };
                        playerCharaData.Add(newChar);
                        newEntities.Add(e);
                    }
                    else
                    {
                        convertedEntities.Add(e);
                    }
                    break;
                case EntityTypes.DRAGON:
                    if (!(dragonStorageSize < dragonStorageCount))
                    {
                        sentToGiftsEntities.Add(e);
                        continue;
                    }
                    DbPlayerDragonData newDragon = new DbPlayerDragonData()
                    {
                        DeviceAccountId = deviceAccountId,
                        DragonId = (Dragons)e.entity_id
                    };
                    if (playerDragonsReliability.ContainsKey((Dragons)e.entity_id))
                    {
                        DbPlayerDragonReliability newDragonReliability =
                            new DbPlayerDragonReliability()
                            {
                                DeviceAccountId = deviceAccountId,
                                DragonId = (Dragons)e.entity_id
                            };
                        _apiContext.PlayerDragonReliability.Add(newDragonReliability);
                    }
                    playerDragonData.Add(newDragon);
                    newEntities.Add(e);
                    dragonStorageCount++;
                    break;
                default:
                    throw new InvalidDataException($"Unknown Entity Type Id {e.entity_type}");
            }
        }

        convertedEntities.ForEach(e =>
        {
            int amount = 0;
            switch (e.rarity)
            {
                case 5:
                    amount = (int)DupeReturnBaseValues.RARITY_5;
                    break;
                case 4:
                    amount = (int)DupeReturnBaseValues.RARITY_4;
                    break;
                case 3:
                    amount = (int)DupeReturnBaseValues.RARITY_3;
                    break;
            }
            saveFileUserData.DewPoint += amount;
        });

        await _apiContext.SaveChangesAsync();

        return new Tuple<IEnumerable<Entity>, IEnumerable<Entity>, IEnumerable<Entity>>(
            convertedEntities,
            newEntities,
            sentToGiftsEntities
        );
    }
}
