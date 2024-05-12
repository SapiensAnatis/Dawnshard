using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IUnitRepository
{
    IQueryable<DbPlayerDragonData> Dragons { get; }
    IQueryable<DbPlayerDragonReliability> DragonReliabilities { get; }
    IQueryable<DbWeaponBody> WeaponBodies { get; }
    IQueryable<DbAbilityCrest> AbilityCrests { get; }
    IQueryable<DbTalisman> Talismans { get; }

    Task RemoveDragons(IEnumerable<long> keyIdList);

    Task<DbSetUnit?> GetCharaSetData(Charas charaId, int setNo);

    DbSetUnit AddCharaSetData(Charas charaId, int setNo);

    IEnumerable<DbSetUnit> GetCharaSets(Charas charaId);

    Task<IDictionary<Charas, IEnumerable<DbSetUnit>>> GetCharaSets(IEnumerable<Charas> charaId);

    Task<DbPlayerCharaData?> FindCharaAsync(Charas chara);
    Task<DbPlayerDragonData?> FindDragonAsync(long dragonKeyId);
    Task<DbPlayerDragonReliability?> FindDragonReliabilityAsync(Dragons dragon);
    Task<DbTalisman?> FindTalismanAsync(long talismanKeyId);
    Task<DbWeaponBody?> FindWeaponBodyAsync(WeaponBodies weaponBody);

    DbTalisman AddTalisman(
        Talismans id,
        int abilityId1,
        int abilityId2,
        int abilityId3,
        int additionalHp,
        int additionalAttack
    );

    void RemoveTalisman(DbTalisman talisman);
}
