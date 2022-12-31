using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class UnitRepository : BaseRepository, IUnitRepository
{
    private readonly ApiContext apiContext;

    public UnitRepository(ApiContext apiContext) : base(apiContext)
    {
        this.apiContext = apiContext;
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
                id =>
                    DbPlayerCharaDataFactory.Create(deviceAccountId, MasterAsset.CharaData.Get(id))
            );

            await apiContext.PlayerCharaData.AddRangeAsync(dbEntries);
        }

        return newMapping;
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

    public IDictionary<Charas, IEnumerable<DbSetUnit>> GetCharaSets(
        string deviceAccountId,
        IEnumerable<Charas> charaIds
    )
    {
        return apiContext.PlayerSetUnits
            .Where(x => charaIds.Contains(x.CharaId))
            .GroupBy(x => x.CharaId)
            .ToDictionary(x => x.Key, x => x.AsEnumerable());
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

    public async Task<DbDetailedPartyUnit> BuildDetailedPartyUnit(
        string deviceAccountId,
        DbPartyUnit input
    )
    {
        DbPlayerDragonData? dragonData = await this.GetAllDragonData(deviceAccountId)
            .SingleOrDefaultAsync(x => x.DragonKeyId == input.EquipDragonKeyId);

        IQueryable<DbPlayerCharaData> charaData = this.GetAllCharaData(deviceAccountId);

        IQueryable<DbAbilityCrest> crestData = this.GetAllAbilityCrestData(deviceAccountId);

        IQueryable<DbTalisman> talismanData = this.GetAllTalismanData(deviceAccountId);

        return new()
        {
            DeviceAccountId = deviceAccountId,
            Position = input.UnitNo,
            CharaData = await charaData.SingleAsync(x => x.CharaId == input.CharaId),
            DragonData = dragonData,
            DragonReliabilityLevel = 30, // TODO: implement dragon reliability nav property to get this from dragon data
            WeaponBodyData = await this.GetAllWeaponBodyData(deviceAccountId)
                .SingleOrDefaultAsync(x => x.WeaponBodyId == input.EquipWeaponBodyId),
            CrestSlotType1CrestList = await crestData
                .Where(
                    x =>
                        x.AbilityCrestId == input.EquipCrestSlotType1CrestId1
                        || x.AbilityCrestId == input.EquipCrestSlotType1CrestId2
                        || x.AbilityCrestId == input.EquipCrestSlotType1CrestId3
                )
                .ToListAsync(),
            CrestSlotType2CrestList = await crestData
                .Where(
                    x =>
                        x.AbilityCrestId == input.EquipCrestSlotType2CrestId1
                        || x.AbilityCrestId == input.EquipCrestSlotType2CrestId2
                )
                .ToListAsync(),
            CrestSlotType3CrestList = await crestData
                .Where(
                    x =>
                        x.AbilityCrestId == input.EquipCrestSlotType3CrestId1
                        || x.AbilityCrestId == input.EquipCrestSlotType3CrestId2
                )
                .ToListAsync(),
            TalismanData = await talismanData.SingleOrDefaultAsync(
                x => x.TalismanKeyId == input.EquipTalismanKeyId
            ),
            EditSkill1CharaData = await this.GetEditSkill(charaData, input.EditSkill1CharaId),
            EditSkill2CharaData = await this.GetEditSkill(charaData, input.EditSkill2CharaId)
        };
    }

    private async Task<DbEditSkillData?> GetEditSkill(
        IQueryable<DbPlayerCharaData> charaData,
        Charas id
    )
    {
        if (id == Charas.Empty)
            return null;

        CharaData data = MasterAsset.CharaData.Get(id);
        bool isFirstSkill = data.EditSkillId == data.Skill1ID;

        return await charaData
            .Where(x => x.CharaId == id && x.IsUnlockEditSkill)
            .Select(
                x =>
                    new DbEditSkillData()
                    {
                        CharaId = x.CharaId,
                        EditSkillLevel = isFirstSkill ? x.Skill1Level : x.Skill2Level,
                    }
            )
            .SingleOrDefaultAsync();
    }
}
