using System;
using System.Collections.Immutable;
using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using DragaliaAPI.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services;

public class SavefileWriteService : ISavefileWriteService
{
    private readonly IApiRepository apiRepository;
    private readonly ApiContext apiContext;
    private readonly IUnitDataService unitDataService;

    public SavefileWriteService(
        IApiRepository apiRepository,
        ApiContext apiContext,
        IUnitDataService unitDataService
    )
    {
        this.apiRepository = apiRepository;
        this.apiContext = apiContext;
        this.unitDataService = unitDataService;
    }

    public async Task<UpdateDataList> CommitSummonResult(
        List<SimpleSummonReward> summonResult,
        string deviceAccountId,
        bool giveDew = true
    )
    {
        Dictionary<Charas, Chara> newChars = new Dictionary<Charas, Chara>();
        List<Dragon> newDragons = new List<Dragon>();
        Dictionary<Dragons, DragonReliability> newUniqueDragons =
            new Dictionary<Dragons, DragonReliability>();

        DbSet<DbPlayerCharaData> playerCharaData = apiContext.PlayerCharaData;
        DbSet<DbPlayerDragonData> playerDragonData = apiContext.PlayerDragonData;

        ImmutableDictionary<Charas, DbPlayerCharaData> playerCharas = playerCharaData
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ToImmutableDictionary(chara => chara.CharaId);
        ImmutableDictionary<Dragons, DbPlayerDragonReliability> playerDragonsReliability =
            apiContext.PlayerDragonReliability
                .Where(x => x.DeviceAccountId == deviceAccountId)
                .ToImmutableDictionary(dragon => dragon.DragonId);

        //TODO: storage size limit for dragons
        int dragonStorageMaxSize = await apiRepository
            .GetPlayerInfo(deviceAccountId)
            .Select(x => x.MaxDragonQuantity)
            .FirstAsync();
        int dragonStorageSize = playerDragonData
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .Count();

        foreach (SimpleSummonReward e in summonResult)
        {
            switch ((EntityTypes)e.entity_type)
            {
                case EntityTypes.Chara:
                    Charas charId = (Charas)e.id;
                    if (!newChars.ContainsKey(charId) && !playerCharas.ContainsKey(charId))
                    {
                        DbPlayerCharaData newChar = DbPlayerCharaDataFactory.Create(
                            deviceAccountId,
                            unitDataService.GetData(e.id),
                            (byte)e.rarity
                        );
                        //TODO: Chara stories
                        newChar = playerCharaData.Add(newChar).Entity;
                        newChars.Add(charId, CharaFactory.Create(newChar));
                    }
                    break;
                case EntityTypes.Dragon:
                    if (!(dragonStorageMaxSize > dragonStorageSize))
                    {
                        //TODO: Send to gift box
                        continue;
                    }
                    Dragons dragonId = (Dragons)e.id;
                    if (
                        !newUniqueDragons.ContainsKey(dragonId)
                        && !playerDragonsReliability.ContainsKey(dragonId)
                    )
                    {
                        DbPlayerDragonReliability newDragonReliability =
                            DbPlayerDragonReliabilityFactory.Create(deviceAccountId, dragonId);
                        //TODO: Dragon stories
                        newDragonReliability = apiContext.PlayerDragonReliability
                            .Add(newDragonReliability)
                            .Entity;
                        newUniqueDragons.Add(
                            dragonId,
                            DragonReliabilityFactory.Create(newDragonReliability)
                        );
                    }

                    DbPlayerDragonData newDragon = DbPlayerDragonDataFactory.Create(
                        deviceAccountId,
                        dragonId
                    );
                    //DbPlayerDragonData newDragon =
                    //    new()
                    //    {
                    //        DeviceAccountId = deviceAccountId,
                    //        DragonKeyId = newDragons.Count,
                    //        DragonId = dragonId,
                    //        Exp = 0,
                    //        Level = 1,
                    //        HpPlusCount = 0,
                    //        AttackPlusCount = 0,
                    //        LimitBreakCount = 0,
                    //        IsLocked = false,
                    //        IsNew = true,
                    //        FirstSkillLevel = (byte)1,
                    //        FirstAbilityLevel = (byte)1,
                    //        SecondAbilityLevel = (byte)1,
                    //        GetTime = DateTimeOffset.UtcNow
                    //    };
                    playerDragonData.Add(newDragon);
                    newDragons.Add(DragonFactory.Create(newDragon));
                    dragonStorageSize++;
                    break;
                default:
                    throw new InvalidDataException($"Unsupported Entity Type Id {e.entity_type}");
            }
        }

        await apiContext.SaveChangesAsync();
        DbPlayerUserData userData = await apiRepository.GetPlayerInfo(deviceAccountId).FirstAsync();
        UpdateDataList result =
            new()
            {
                chara_list = newChars.Values.ToList(),
                dragon_list = newDragons,
                dragon_reliability_list = newUniqueDragons.Values.ToList(),
                user_data = SavefileUserDataFactory.Create(userData)
            };

        return result;
    }

    public async Task<int> CreateSummonHistory(IEnumerable<DbPlayerSummonHistory> entries)
    {
        apiContext.PlayerSummonHistory.AddRange(entries);
        return await apiContext.SaveChangesAsync();
    }
}
