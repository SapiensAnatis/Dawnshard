using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class UnitRepository : BaseRepository, IUnitRepository
{
    private readonly ApiContext apiContext;
    private readonly ICharaDataService charaDataService;

    public UnitRepository(ApiContext apiContext, ICharaDataService charaDataService)
        : base(apiContext)
    {
        this.apiContext = apiContext;
        this.charaDataService = charaDataService;
    }

    public IQueryable<DbPlayerCharaData> GetAllCharaData(string deviceAccountId)
    {
        return apiContext.PlayerCharaData.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public IQueryable<DbPlayerDragonData> GetAllDragonData(string deviceAccountId)
    {
        return apiContext.PlayerDragonData.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public async Task<bool> CheckHasCharas(string deviceAccountId, IEnumerable<Charas> idList)
    {
        IEnumerable<Charas> owned = await this.GetAllCharaData(deviceAccountId)
            .Select(x => x.CharaId)
            .ToListAsync();

        return owned.Intersect(idList).Count() == idList.Count();
    }

    public async Task<bool> CheckHasDragons(string deviceAccountId, IEnumerable<Dragons> idList)
    {
        IEnumerable<Dragons> owned = await this.GetAllDragonData(deviceAccountId)
            .Select(x => x.DragonId)
            .ToListAsync();

        return owned.Intersect(idList).Count() == idList.Count();
    }

    /// <summary>
    /// Add a list of characters to the database. Will only add the first instance of any new character.
    /// </summary>
    /// <param name="deviceAccountId"></param>
    /// <param name="idList"></param>
    /// <returns>A list of tuples which adds an additional dimension onto the input list,
    /// where the second item shows whether the given character id was a duplicate.</returns>
    public async Task<IEnumerable<DbPlayerCharaData>> AddCharas(
        string deviceAccountId,
        IEnumerable<Charas> idList
    )
    {
        // Generate result. The first occurrence of a character in the list should be new (if not in the DB)
        // but subsequent results should then not be labelled as new. No way to do that logic with LINQ afaik

        IEnumerable<Charas> ownedCharas = await this.GetAllCharaData(deviceAccountId)
            .Select(x => x.CharaId)
            .ToListAsync();

        IEnumerable<(Charas id, bool isNew)> newMapping = MarkNewIds(ownedCharas, idList);

        // Use result to inform additions to the DB
        IEnumerable<Charas> newCharas = newMapping.Where(x => x.isNew).Select(x => x.id);

        if (newCharas.Any())
        {
            IEnumerable<DbPlayerCharaData> dbEntries = newCharas.Select(
                id => DbPlayerCharaDataFactory.Create(deviceAccountId, charaDataService.GetData(id))
            );

            await apiContext.PlayerCharaData.AddRangeAsync(dbEntries);
        }

        IEnumerable<DbPlayerCharaData> result = apiContext.ChangeTracker
            .Entries<DbPlayerCharaData>()
            .Where(x => x.State == EntityState.Added)
            .Select(x => x.Entity)
            .ToList();

        return result;
    }

    public async Task<(
        IEnumerable<DbPlayerDragonData> newDragons,
        IEnumerable<DbPlayerDragonReliability> newReliabilities
    )> AddDragons(string deviceAccountId, IEnumerable<Dragons> idList)
    {
        IEnumerable<Dragons> ownedDragons = await this.GetAllDragonData(deviceAccountId)
            .Select(x => x.DragonId)
            .ToListAsync();

        IEnumerable<(Dragons id, bool isNew)> newMapping = MarkNewIds(ownedDragons, idList);

        IEnumerable<Dragons> newDragons = newMapping.Where(x => x.isNew).Select(x => x.id);

        if (newDragons.Any())
        {
            await apiContext.AddRangeAsync(
                newDragons.Select(
                    id => DbPlayerDragonReliabilityFactory.Create(deviceAccountId, id)
                )
            );
        }

        await apiContext.AddRangeAsync(
            idList.Select(id => DbPlayerDragonDataFactory.Create(deviceAccountId, id))
        );

        // Get after so that identity column is set
        IEnumerable<DbPlayerDragonData> addedDragons = apiContext.ChangeTracker
            .Entries<DbPlayerDragonData>()
            .Where(x => x.State == EntityState.Added)
            .Select(x => x.Entity)
            .ToList();

        IEnumerable<DbPlayerDragonReliability> addedReliability = apiContext.ChangeTracker
            .Entries<DbPlayerDragonReliability>()
            .Where(x => x.State == EntityState.Added)
            .Select(x => x.Entity)
            .ToList();

        return (addedDragons, addedReliability);
    }

    private static IEnumerable<(TEnum id, bool isNew)> MarkNewIds<TEnum>(
        IEnumerable<TEnum> owned,
        IEnumerable<TEnum> idList
    ) where TEnum : Enum
    {
        List<(TEnum id, bool isNew)> result = new();
        foreach (TEnum c in idList)
        {
            bool isNew = !(result.Any(x => x.id.Equals(c)) || owned.Contains(c));
            result.Add(new(c, isNew));
        }

        return result;
    }
}
