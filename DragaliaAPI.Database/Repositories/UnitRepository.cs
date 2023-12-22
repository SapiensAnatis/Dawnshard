using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

public class UnitRepository : IUnitRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<UnitRepository> logger;

    public UnitRepository(
        ApiContext apiContext,
        IPlayerIdentityService playerIdentityService,
        ILogger<UnitRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    public IQueryable<DbPlayerCharaData> Charas =>
        this.apiContext.PlayerCharaData.Where(
            x => x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbPlayerDragonData> Dragons =>
        this.apiContext.PlayerDragonData.Where(
            x => x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbAbilityCrest> AbilityCrests =>
        this.apiContext.PlayerAbilityCrests.Where(
            x => x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbWeaponBody> WeaponBodies =>
        this.apiContext.PlayerWeapons.Where(x => x.ViewerId == this.playerIdentityService.ViewerId);

    public IQueryable<DbPlayerDragonReliability> DragonReliabilities =>
        this.apiContext.PlayerDragonReliability.Where(
            x => x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbTalisman> Talismans =>
        this.apiContext.PlayerTalismans.Where(
            x => x.ViewerId == this.playerIdentityService.ViewerId
        );

    public async Task<bool> CheckHasCharas(IEnumerable<Charas> idList)
    {
        IEnumerable<Charas> owned = await Charas.Select(x => x.CharaId).ToListAsync();

        return owned.Intersect(idList).Count() == idList.Count();
    }

    public async Task<DbPlayerCharaData?> FindCharaAsync(Charas chara)
    {
        return await this.apiContext.PlayerCharaData.FindAsync(
            this.playerIdentityService.ViewerId,
            chara
        );
    }

    public async Task<DbPlayerDragonData?> FindDragonAsync(long dragonKeyId)
    {
        return await apiContext.PlayerDragonData.FindAsync(dragonKeyId);
    }

    public async Task<DbPlayerDragonReliability?> FindDragonReliabilityAsync(Dragons dragon)
    {
        return await apiContext.PlayerDragonReliability.FindAsync(
            playerIdentityService.ViewerId,
            dragon
        );
    }

    public async Task<DbTalisman?> FindTalismanAsync(long talismanKeyId)
    {
        return await apiContext.PlayerTalismans.FindAsync(talismanKeyId);
    }

    public async Task<DbWeaponBody?> FindWeaponBodyAsync(WeaponBodies weaponBody)
    {
        return await apiContext.PlayerWeapons.FindAsync(playerIdentityService.ViewerId, weaponBody);
    }

    public async Task<bool> CheckHasDragons(IEnumerable<Dragons> idList)
    {
        IEnumerable<Dragons> owned = await Dragons.Select(x => x.DragonId).ToListAsync();

        return owned.Intersect(idList).Count() == idList.Count();
    }

    /// <summary>
    /// Add a list of characters to the database. Will only add the first instance of any new character.
    /// </summary>
    /// <param name="this.playerIdentityService.AccountId"></param>
    /// <param name="idList"></param>
    /// <returns>A list of tuples which adds an additional dimension onto the input list,
    /// where the second item shows whether the given character id was a duplicate.</returns>
    public async Task<IEnumerable<(Charas id, bool isNew)>> AddCharas(IEnumerable<Charas> idList)
    {
        List<Charas> addedChars = idList.ToList();

        // Generate result. The first occurrence of a character in the list should be new (if not in the DB)
        // but subsequent results should then not be labelled as new. No way to do that logic with LINQ afaik

        ICollection<Charas> ownedCharas = await Charas.Select(x => x.CharaId).ToListAsync();

        List<(Charas id, bool isNew)> newMapping = MarkNewIds(ownedCharas, addedChars);

        // Use result to inform additions to the DB
        IEnumerable<Charas> newCharas = newMapping.Where(x => x.isNew).Select(x => x.id).ToList();

        if (newCharas.Any())
        {
            IEnumerable<DbPlayerCharaData> dbEntries = newCharas.Select(
                id => new DbPlayerCharaData(this.playerIdentityService.ViewerId, id)
            );

            await apiContext.PlayerCharaData.AddRangeAsync(dbEntries);

            List<DbPlayerStoryState> newCharaStories = new List<DbPlayerStoryState>(
                newCharas.Count()
            );
            for (int i = 0; i < newCharas.Count(); i++)
            {
                if (
                    MasterAsset.CharaStories.TryGetValue(
                        (int)newCharas.ElementAt(i),
                        out StoryData? story
                    )
                )
                {
                    newCharaStories.Add(
                        new DbPlayerStoryState
                        {
                            ViewerId = this.playerIdentityService.ViewerId,
                            StoryType = StoryTypes.Chara,
                            StoryId = story.storyIds[0],
                            State = 0
                        }
                    );
                }
                else
                {
                    logger.LogInformation(
                        "Unable to find any storyIds for Character: {Chara}",
                        newCharas.ElementAt(i)
                    );
                }
            }
            apiContext.PlayerStoryState.AddRange(newCharaStories);
        }

        return newMapping;
    }

    public async Task<bool> AddCharas(Charas id)
    {
        return (await this.AddCharas(new[] { id })).First().isNew;
    }

    public async Task<IEnumerable<(Dragons id, bool isNew)>> AddDragons(IEnumerable<Dragons> idList)
    {
        IEnumerable<Dragons> ownedDragons = await Dragons.Select(x => x.DragonId).ToListAsync();

        IEnumerable<(Dragons id, bool isNew)> newMapping = MarkNewIds(ownedDragons, idList);

        IEnumerable<DbPlayerDragonReliability> newReliabilities = newMapping.Select(
            x => DbPlayerDragonReliabilityFactory.Create(this.playerIdentityService.ViewerId, x.id)
        );

        foreach ((Dragons id, _) in newMapping.Where(x => x.isNew))
        {
            // Not being in the dragon table doesn't mean a reliability doesn't exist
            // as the dragon could've been sold
            if (
                await this.apiContext.PlayerDragonReliability.FindAsync(
                    this.playerIdentityService.ViewerId,
                    id
                )
                is null
            )
            {
                await apiContext.AddAsync(
                    DbPlayerDragonReliabilityFactory.Create(this.playerIdentityService.ViewerId, id)
                );
            }
        }

        await apiContext.AddRangeAsync(
            idList.Select(
                id => DbPlayerDragonDataFactory.Create(this.playerIdentityService.ViewerId, id)
            )
        );

        return newMapping;
    }

    public async Task<bool> AddDragons(Dragons id)
    {
        return (await this.AddDragons(new[] { id })).First().isNew;
    }

    public async Task RemoveDragons(IEnumerable<long> keyIdList)
    {
        IEnumerable<DbPlayerDragonData> ownedDragons = await Dragons
            .Where(
                x =>
                    x.ViewerId == this.playerIdentityService.ViewerId
                    && keyIdList.Contains(x.DragonKeyId)
            )
            .ToListAsync();

        apiContext.PlayerDragonData.RemoveRange(ownedDragons);
    }

    public async Task<DbSetUnit?> GetCharaSetData(Charas charaId, int setNo)
    {
        return await apiContext.PlayerSetUnits.FindAsync(
            playerIdentityService.ViewerId,
            charaId,
            setNo
        );
    }

    public DbSetUnit AddCharaSetData(Charas charaId, int setNo)
    {
        return apiContext
            .PlayerSetUnits.Add(
                new DbSetUnit
                {
                    ViewerId = this.playerIdentityService.ViewerId,
                    CharaId = charaId,
                    UnitSetNo = setNo,
                    UnitSetName = $"Set {setNo}"
                }
            )
            .Entity;
    }

    public IEnumerable<DbSetUnit> GetCharaSets(Charas charaId)
    {
        return apiContext.PlayerSetUnits.Where(
            x => x.ViewerId == this.playerIdentityService.ViewerId && x.CharaId == charaId
        );
    }

    public async Task<IDictionary<Charas, IEnumerable<DbSetUnit>>> GetCharaSets(
        IEnumerable<Charas> charaIds
    )
    {
        return await apiContext
            .PlayerSetUnits.Where(
                x =>
                    charaIds.Contains(x.CharaId)
                    && x.ViewerId == this.playerIdentityService.ViewerId
            )
            .GroupBy(x => x.CharaId)
            .ToDictionaryAsync(x => x.Key, x => x.AsEnumerable());
    }

    public DbTalisman AddTalisman(
        Talismans id,
        int abilityId1,
        int abilityId2,
        int abilityId3,
        int additionalHp,
        int additionalAttack
    )
    {
        return apiContext
            .PlayerTalismans.Add(
                new DbTalisman
                {
                    ViewerId = playerIdentityService.ViewerId,
                    TalismanId = id,
                    TalismanAbilityId1 = abilityId1,
                    TalismanAbilityId2 = abilityId2,
                    TalismanAbilityId3 = abilityId3,
                    AdditionalHp = additionalHp,
                    AdditionalAttack = additionalAttack,
                    GetTime = DateTimeOffset.UtcNow
                }
            )
            .Entity;
    }

    public void RemoveTalisman(DbTalisman talisman)
    {
        apiContext.PlayerTalismans.Remove(talisman);
    }

    private static List<(TEnum id, bool isNew)> MarkNewIds<TEnum>(
        IEnumerable<TEnum> owned,
        IEnumerable<TEnum> idList
    )
        where TEnum : Enum
    {
        List<(TEnum id, bool isNew)> result = new();
        foreach (TEnum c in idList)
        {
            bool isNew = !(result.Any(x => x.id.Equals(c)) || owned.Contains(c));
            result.Add((c, isNew));
        }

        return result;
    }
}
