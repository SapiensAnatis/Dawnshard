﻿using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CharasEnum = DragaliaAPI.Shared.Definitions.Enums.Charas;

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
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public IQueryable<DbPlayerDragonData> Dragons =>
        this.apiContext.PlayerDragonData.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public IQueryable<DbAbilityCrest> AbilityCrests =>
        this.apiContext.PlayerAbilityCrests.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public IQueryable<DbWeaponBody> WeaponBodies =>
        this.apiContext.PlayerWeapons.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public IQueryable<DbPlayerDragonReliability> DragonReliabilities =>
        this.apiContext.PlayerDragonReliability.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public IQueryable<DbTalisman> Talismans =>
        this.apiContext.PlayerTalismans.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public async Task<bool> CheckHasCharas(IEnumerable<Charas> idList)
    {
        IEnumerable<Charas> owned = await Charas.Select(x => x.CharaId).ToListAsync();

        return owned.Intersect(idList).Count() == idList.Count();
    }

    public async Task<DbPlayerCharaData?> FindCharaAsync(Charas chara)
    {
        return await this.apiContext.PlayerCharaData.FindAsync(
            this.playerIdentityService.AccountId,
            chara
        );
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
                id => new DbPlayerCharaData(this.playerIdentityService.AccountId, id)
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
                        new DbPlayerStoryState()
                        {
                            DeviceAccountId = this.playerIdentityService.AccountId,
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
            x => DbPlayerDragonReliabilityFactory.Create(this.playerIdentityService.AccountId, x.id)
        );

        foreach ((Dragons id, _) in newMapping.Where(x => x.isNew))
        {
            // Not being in the dragon table doesn't mean a reliability doesn't exist
            // as the dragon could've been sold
            if (
                await this.apiContext.PlayerDragonReliability.FindAsync(
                    this.playerIdentityService.AccountId,
                    id
                )
                is null
            )
            {
                await apiContext.AddAsync(
                    DbPlayerDragonReliabilityFactory.Create(
                        this.playerIdentityService.AccountId,
                        id
                    )
                );
            }
        }

        await apiContext.AddRangeAsync(
            idList.Select(
                id => DbPlayerDragonDataFactory.Create(this.playerIdentityService.AccountId, id)
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
                    x.DeviceAccountId == this.playerIdentityService.AccountId
                    && keyIdList.Contains(x.DragonKeyId)
            )
            .ToListAsync();

        apiContext.PlayerDragonData.RemoveRange(ownedDragons);
    }

    public async Task<DbSetUnit?> GetCharaSetData(Charas charaId, int setNo)
    {
        return await apiContext.PlayerSetUnits.FirstOrDefaultAsync(
            x =>
                x.DeviceAccountId == this.playerIdentityService.AccountId
                && x.CharaId == charaId
                && x.UnitSetNo == setNo
        );
    }

    public DbSetUnit AddCharaSetData(Charas charaId, int setNo)
    {
        return apiContext.PlayerSetUnits
            .Add(
                new DbSetUnit()
                {
                    DeviceAccountId = this.playerIdentityService.AccountId,
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
            x => x.DeviceAccountId == this.playerIdentityService.AccountId && x.CharaId == charaId
        );
    }

    public async Task<IDictionary<Charas, IEnumerable<DbSetUnit>>> GetCharaSets(
        IEnumerable<Charas> charaIds
    )
    {
        return await apiContext.PlayerSetUnits
            .Where(
                x =>
                    charaIds.Contains(x.CharaId)
                    && x.DeviceAccountId == this.playerIdentityService.AccountId
            )
            .GroupBy(x => x.CharaId)
            .ToDictionaryAsync(x => x.Key, x => x.AsEnumerable());
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
