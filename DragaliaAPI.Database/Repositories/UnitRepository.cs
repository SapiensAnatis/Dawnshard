using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class UnitRepository : BaseRepository, IUnitRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerDetailsService playerDetailsService;

    public UnitRepository(ApiContext apiContext, IPlayerDetailsService playerDetailsService)
        : base(apiContext)
    {
        this.apiContext = apiContext;
        this.playerDetailsService = playerDetailsService;
    }

    public IQueryable<DbPlayerCharaData> GetAllCharaData(string deviceAccountId)
    {
        return apiContext.PlayerCharaData.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public IQueryable<DbPlayerDragonData> GetAllDragonData(string deviceAccountId)
    {
        return apiContext.PlayerDragonData.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public IQueryable<DbAbilityCrest> GetAllAbilityCrestData(string deviceAccountId)
    {
        return apiContext.PlayerAbilityCrests.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public IQueryable<DbWeaponBody> GetAllWeaponBodyData(string deviceAccountId)
    {
        return apiContext.PlayerWeapons.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public IQueryable<DbPlayerDragonReliability> GetAllDragonReliabilityData(string deviceAccountId)
    {
        return apiContext.PlayerDragonReliability.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public IQueryable<DbTalisman> GetAllTalismanData(string deviceAccountId)
    {
        return this.apiContext.PlayerTalismans.Where(x => x.DeviceAccountId == deviceAccountId);
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
    public async Task<IEnumerable<(Charas id, bool isNew)>> AddCharas(
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
                id => new DbPlayerCharaData(deviceAccountId, id)
            );

            await apiContext.PlayerCharaData.AddRangeAsync(dbEntries);
        }

        return newMapping;
    }

    public async Task<IEnumerable<(Charas id, bool isNew)>> AddCharas(IEnumerable<Charas> idList)
    {
        return await this.AddCharas(this.playerDetailsService.AccountId, idList);
    }

    public async Task<IEnumerable<(Charas id, bool isNew)>> AddCharas(Charas id)
    {
        return await this.AddCharas(new[] { id });
    }

    public async Task<IEnumerable<(Dragons id, bool isNew)>> AddDragons(
        string deviceAccountId,
        IEnumerable<Dragons> idList
    )
    {
        IEnumerable<Dragons> ownedDragons = await this.GetAllDragonData(deviceAccountId)
            .Select(x => x.DragonId)
            .ToListAsync();

        IEnumerable<(Dragons id, bool isNew)> newMapping = MarkNewIds(ownedDragons, idList);

        IEnumerable<DbPlayerDragonReliability> newReliabilities = newMapping.Select(
            x => DbPlayerDragonReliabilityFactory.Create(deviceAccountId, x.id)
        );

        foreach ((Dragons id, _) in newMapping.Where(x => x.isNew))
        {
            // Not being in the dragon table doesn't mean a reliability doesn't exist
            // as the dragon could've been sold
            if (
                await this.apiContext.PlayerDragonReliability.FindAsync(deviceAccountId, id) is null
            )
            {
                await apiContext.AddAsync(
                    DbPlayerDragonReliabilityFactory.Create(deviceAccountId, id)
                );
            }
        }

        await apiContext.AddRangeAsync(
            idList.Select(id => DbPlayerDragonDataFactory.Create(deviceAccountId, id))
        );

        return newMapping;
    }

    public async Task<IEnumerable<(Dragons id, bool isNew)>> AddDragons(IEnumerable<Dragons> idList)
    {
        return await this.AddDragons(this.playerDetailsService.AccountId, idList);
    }

    public async Task<IEnumerable<(Dragons id, bool isNew)>> AddDragons(Dragons id)
    {
        return await this.AddDragons(new[] { id });
    }

    public async Task RemoveDragons(string deviceAccountId, IEnumerable<long> keyIdList)
    {
        IEnumerable<DbPlayerDragonData> ownedDragons = await this.GetAllDragonData(deviceAccountId)
            .Where(x => x.DeviceAccountId == deviceAccountId && keyIdList.Contains(x.DragonKeyId))
            .ToListAsync();

        apiContext.PlayerDragonData.RemoveRange(ownedDragons);
    }

    public async Task<DbSetUnit?> GetCharaSetData(string deviceAccountId, Charas charaId, int setNo)
    {
        return await apiContext.PlayerSetUnits.FirstOrDefaultAsync(
            x =>
                x.DeviceAccountId == deviceAccountId && x.CharaId == charaId && x.UnitSetNo == setNo
        );
    }

    public DbSetUnit AddCharaSetData(string deviceAccountId, Charas charaId, int setNo)
    {
        return apiContext.PlayerSetUnits
            .Add(
                new DbSetUnit()
                {
                    DeviceAccountId = deviceAccountId,
                    CharaId = charaId,
                    UnitSetNo = setNo,
                    UnitSetName = $"Set {setNo}"
                }
            )
            .Entity;
    }

    public IEnumerable<DbSetUnit> GetCharaSets(string deviceAccountId, Charas charaId)
    {
        return apiContext.PlayerSetUnits.Where(
            x => x.DeviceAccountId == deviceAccountId && x.CharaId == charaId
        );
    }

    public async Task<IDictionary<Charas, IEnumerable<DbSetUnit>>> GetCharaSets(
        string deviceAccountId,
        IEnumerable<Charas> charaIds
    )
    {
        return await apiContext.PlayerSetUnits
            .Where(x => charaIds.Contains(x.CharaId) && x.DeviceAccountId == deviceAccountId)
            .GroupBy(x => x.CharaId)
            .ToDictionaryAsync(x => x.Key, x => x.AsEnumerable());
    }

    private static IEnumerable<(TEnum id, bool isNew)> MarkNewIds<TEnum>(
        IEnumerable<TEnum> owned,
        IEnumerable<TEnum> idList
    )
        where TEnum : Enum
    {
        List<(TEnum id, bool isNew)> result = new();
        foreach (TEnum c in idList)
        {
            bool isNew = !(result.Any(x => x.id.Equals(c)) || owned.Contains(c));
            result.Add(new(c, isNew));
        }

        return result;
    }

    public IQueryable<DbDetailedPartyUnit> BuildDetailedPartyUnit(
        string deviceAccountId,
        IQueryable<DbPartyUnit> input
    )
    {
        return from unit in input
        join chara in this.apiContext.PlayerCharaData
            on new { unit.DeviceAccountId, unit.CharaId } equals new
            {
                chara.DeviceAccountId,
                chara.CharaId
            }
        from dragon in this.apiContext.PlayerDragonData
            .Where(
                x =>
                    x.DragonKeyId == unit.EquipDragonKeyId
                    && x.DeviceAccountId == unit.DeviceAccountId
            )
            .DefaultIfEmpty()
        from dragonReliability in this.apiContext.PlayerDragonReliability
            .Where(x => x.DragonId == dragon.DragonId && x.DeviceAccountId == unit.DeviceAccountId)
            .DefaultIfEmpty()
        from weapon in this.apiContext.PlayerWeapons
            .Where(
                x =>
                    x.WeaponBodyId == unit.EquipWeaponBodyId
                    && x.DeviceAccountId == unit.DeviceAccountId
            )
            .DefaultIfEmpty()
        from crests11 in this.apiContext.PlayerAbilityCrests
            .Where(
                x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType1CrestId1
                    && x.DeviceAccountId == unit.DeviceAccountId
            )
            .DefaultIfEmpty()
        from crests12 in this.apiContext.PlayerAbilityCrests
            .Where(
                x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType1CrestId2
                    && x.DeviceAccountId == unit.DeviceAccountId
            )
            .DefaultIfEmpty()
        from crests13 in this.apiContext.PlayerAbilityCrests
            .Where(
                x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType1CrestId3
                    && x.DeviceAccountId == unit.DeviceAccountId
            )
            .DefaultIfEmpty()
        from crests21 in this.apiContext.PlayerAbilityCrests
            .Where(
                x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType2CrestId1
                    && x.DeviceAccountId == unit.DeviceAccountId
            )
            .DefaultIfEmpty()
        from crests22 in this.apiContext.PlayerAbilityCrests
            .Where(
                x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType2CrestId2
                    && x.DeviceAccountId == unit.DeviceAccountId
            )
            .DefaultIfEmpty()
        from crests31 in this.apiContext.PlayerAbilityCrests
            .Where(
                x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType3CrestId1
                    && x.DeviceAccountId == unit.DeviceAccountId
            )
            .DefaultIfEmpty()
        from crests32 in this.apiContext.PlayerAbilityCrests
            .Where(
                x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType3CrestId2
                    && x.DeviceAccountId == unit.DeviceAccountId
            )
            .DefaultIfEmpty()
        from charaEs1 in this.apiContext.PlayerCharaData
            .Where(
                x =>
                    x.CharaId == unit.EditSkill1CharaId
                    && x.DeviceAccountId == unit.DeviceAccountId
                    && x.IsUnlockEditSkill
            )
            .DefaultIfEmpty()
        from charaEs2 in this.apiContext.PlayerCharaData
            .Where(
                x =>
                    x.CharaId == unit.EditSkill2CharaId
                    && x.DeviceAccountId == unit.DeviceAccountId
                    && x.IsUnlockEditSkill
            )
            .DefaultIfEmpty()
        from talisman in this.apiContext.PlayerTalismans
            .Where(
                x =>
                    x.TalismanKeyId == unit.EquipTalismanKeyId
                    && x.DeviceAccountId == unit.DeviceAccountId
            )
            .DefaultIfEmpty()
        from skin in this.apiContext.PlayerWeaponSkins
            .Where(
                x =>
                    x.WeaponSkinId == unit.EquipWeaponSkinId
                    && x.DeviceAccountId == unit.DeviceAccountId
            )
            .DefaultIfEmpty()
        select new DbDetailedPartyUnit
        {
            DeviceAccountId = deviceAccountId,
            Position = unit.UnitNo,
            CharaData = chara,
            DragonData = dragon,
            DragonReliabilityLevel = (dragonReliability == null) ? 0 : dragonReliability.Level,
            WeaponBodyData = weapon,
            CrestSlotType1CrestList = new List<DbAbilityCrest>() { crests11, crests12, crests13 },
            CrestSlotType2CrestList = new List<DbAbilityCrest>() { crests21, crests22 },
            CrestSlotType3CrestList = new List<DbAbilityCrest>() { crests31, crests32 },
            EditSkill1CharaData =
                (charaEs1 == null)
                    ? null
                    : GetEditSkill(charaEs1.CharaId, charaEs1.Skill1Level, charaEs1.Skill2Level),
            EditSkill2CharaData =
                (charaEs2 == null)
                    ? null
                    : GetEditSkill(charaEs2.CharaId, charaEs2.Skill1Level, charaEs2.Skill2Level),
            TalismanData = talisman,
            WeaponSkinData = skin
        };
    }

    private static DbEditSkillData? GetEditSkill(Charas charaId, int skill1Level, int skill2Level)
    {
        // The method signature does not take a DbPlayerCharaData to limit the SELECT statement generated by ef
        if (charaId is Charas.Empty)
            return null;

        CharaData data = MasterAsset.CharaData.Get(charaId);
        bool isFirstSkill = data.EditSkillId == data.Skill1;

        return new DbEditSkillData()
        {
            CharaId = charaId,
            EditSkillLevel = isFirstSkill ? skill1Level : skill2Level,
        };
    }
}
