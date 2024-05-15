using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

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

    public IQueryable<DbPlayerDragonData> Dragons => this.apiContext.PlayerDragonData;

    public IQueryable<DbAbilityCrest> AbilityCrests =>
        this.apiContext.PlayerAbilityCrests.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbWeaponBody> WeaponBodies =>
        this.apiContext.PlayerWeapons.Where(x => x.ViewerId == this.playerIdentityService.ViewerId);

    public IQueryable<DbPlayerDragonReliability> DragonReliabilities =>
        this.apiContext.PlayerDragonReliability;

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

    public async Task RemoveDragons(IEnumerable<long> keyIdList)
    {
        List<DbPlayerDragonData> ownedDragons = await apiContext
            .PlayerDragonData.Where(x => keyIdList.Contains(x.DragonKeyId))
            .ToListAsync();

        apiContext.PlayerDragonData.RemoveRange(ownedDragons);
    }

    public void RemoveTalisman(DbTalisman talisman)
    {
        apiContext.PlayerTalismans.Remove(talisman);
    }
}
