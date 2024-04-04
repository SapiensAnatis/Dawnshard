using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CharaNewCheckResult = (
    DragaliaAPI.Shared.Definitions.Enums.Charas Id,
    bool IsNew,
    bool IsStoryNew
);
using DragonNewCheckResult = (
    DragaliaAPI.Shared.Definitions.Enums.Dragons Id,
    bool IsNew,
    bool IsStoryNew
);

namespace DragaliaAPI.Database.Repositories;

public class UnitRepository : IUnitRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;

    public UnitRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
    }

    public IQueryable<DbPlayerDragonData> Dragons =>
        this.apiContext.PlayerDragonData.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbAbilityCrest> AbilityCrests =>
        this.apiContext.PlayerAbilityCrests.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbWeaponBody> WeaponBodies =>
        this.apiContext.PlayerWeapons.Where(x => x.ViewerId == this.playerIdentityService.ViewerId);

    public IQueryable<DbPlayerDragonReliability> DragonReliabilities =>
        this.apiContext.PlayerDragonReliability.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbTalisman> Talismans =>
        this.apiContext.PlayerTalismans.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId
        );

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

    /// <summary>
    /// Add a list of characters to the database. Will only add the first instance of any new character.
    /// </summary>
    /// <returns>A list of tuples which adds an additional dimension onto the input list,
    /// where the second item shows whether the given character id was a duplicate.</returns>
    public async Task<IEnumerable<(Charas id, bool isNew)>> AddCharas(IEnumerable<Charas> idList)
    {
        List<Charas> enumeratedIdList = idList.ToList();
        IEnumerable<int> storyIdList = GetFirstCharaStories(enumeratedIdList);

        // Generate result. The first occurrence of a character in the list should be new (if not in the DB)
        // but subsequent results should then not be labelled as new.
        List<Charas> ownedCharas = await this
            .apiContext.PlayerCharaData.Select(x => x.CharaId)
            .Where(x => enumeratedIdList.Contains(x))
            .ToListAsync();

        List<int> ownedCharaStories = await this
            .apiContext.PlayerStoryState.Where(x =>
                x.StoryType == StoryTypes.Chara && storyIdList.Contains(x.StoryId)
            )
            .Select(x => x.StoryId)
            .ToListAsync();

        // We also mark which stories are new. Ordinarily it is fine to assume if a character is new, then its story
        // should be new. However, this has encountered occasional primary key errors from players who import saves
        // after removing characters but not their story, which is not possible under normal circumstances.

        List<CharaNewCheckResult> newMapping = MarkNewCharas(
            ownedCharas,
            ownedCharaStories,
            enumeratedIdList
        );

        foreach (CharaNewCheckResult result in newMapping)
        {
            if (result.IsNew)
            {
                this.apiContext.PlayerCharaData.Add(
                    new DbPlayerCharaData(this.playerIdentityService.ViewerId, result.Id)
                );
            }

            if (
                result.IsStoryNew
                && MasterAsset.CharaStories.TryGetValue((int)result.Id, out StoryData? story)
            )
            {
                apiContext.PlayerStoryState.Add(
                    new DbPlayerStoryState
                    {
                        ViewerId = this.playerIdentityService.ViewerId,
                        StoryType = StoryTypes.Chara,
                        StoryId = story.StoryIds[0],
                        State = 0
                    }
                );
            }
        }

        return newMapping.Select(x => (x.Id, x.IsNew));

        static IEnumerable<int> GetFirstCharaStories(IEnumerable<Charas> charaIdList)
        {
            foreach (Charas c in charaIdList)
            {
                if (MasterAsset.CharaStories.TryGetValue((int)c, out StoryData? storyData))
                {
                    yield return storyData.StoryIds[0];
                }
            }
        }
    }

    public async Task<bool> AddCharas(Charas id)
    {
        return (await this.AddCharas(new[] { id })).First().isNew;
    }

    public async Task<IEnumerable<(Dragons Id, bool IsNew)>> AddDragons(IEnumerable<Dragons> idList)
    {
        List<Dragons> enumeratedIdList = idList.ToList();

        List<Dragons> ownedDragons = await Dragons
            .Select(x => x.DragonId)
            .Where(x => enumeratedIdList.Contains(x))
            .ToListAsync();
        List<Dragons> ownedReliabilities = await DragonReliabilities
            .Select(x => x.DragonId)
            .Where(x => enumeratedIdList.Contains(x))
            .ToListAsync();

        List<DragonNewCheckResult> newMapping = MarkNewDragons(
            ownedDragons,
            ownedReliabilities,
            enumeratedIdList
        );

        foreach ((Dragons id, _, bool isReliabilityNew) in newMapping)
        {
            // Not being in the dragon table doesn't mean a reliability doesn't exist
            // as the dragon could've been sold
            if (isReliabilityNew)
            {
                this.apiContext.Add(
                    DbPlayerDragonReliabilityFactory.Create(this.playerIdentityService.ViewerId, id)
                );
            }
        }

        this.apiContext.AddRange(
            enumeratedIdList.Select(id =>
                DbPlayerDragonDataFactory.Create(this.playerIdentityService.ViewerId, id)
            )
        );

        return newMapping.Select(x => (x.Id, x.IsNew));
    }

    public async Task<bool> AddDragons(Dragons id)
    {
        return (await this.AddDragons(new[] { id })).First().IsNew;
    }

    public async Task RemoveDragons(IEnumerable<long> keyIdList)
    {
        IEnumerable<DbPlayerDragonData> ownedDragons = await Dragons
            .Where(x =>
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
        return apiContext.PlayerSetUnits.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId && x.CharaId == charaId
        );
    }

    public async Task<IDictionary<Charas, IEnumerable<DbSetUnit>>> GetCharaSets(
        IEnumerable<Charas> charaIds
    )
    {
        return await apiContext
            .PlayerSetUnits.Where(x =>
                charaIds.Contains(x.CharaId) && x.ViewerId == this.playerIdentityService.ViewerId
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

    private static List<CharaNewCheckResult> MarkNewCharas(
        List<Charas> owned,
        List<int> ownedStories,
        List<Charas> idList
    )
    {
        List<CharaNewCheckResult> result = new();
        foreach (Charas c in idList)
        {
            bool isCharaNew = !(result.Any(x => x.Id.Equals(c)) || owned.Contains(c));
            bool isStoryNew =
                isCharaNew
                && MasterAsset.CharaStories.TryGetValue((int)c, out StoryData? storyData)
                && !ownedStories.Contains(storyData.StoryIds[0]);

            result.Add((c, isCharaNew, isStoryNew));
        }

        return result;
    }

    private static List<DragonNewCheckResult> MarkNewDragons(
        List<Dragons> owned,
        List<Dragons> ownedReliabilities,
        List<Dragons> idList
    )
    {
        List<DragonNewCheckResult> result = new();
        foreach (Dragons c in idList)
        {
            bool isDragonNew = !(result.Any(x => x.Id.Equals(c)) || owned.Contains(c));
            bool isReliabilityNew = isDragonNew && !ownedReliabilities.Contains(c);

            result.Add((c, isDragonNew, isReliabilityNew));
        }

        return result;
    }
}
