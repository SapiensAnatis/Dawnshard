using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(WeaponPassiveAbilityId))]
public class DbWeaponPassiveAbility : DbPlayerData
{
    public required int WeaponPassiveAbilityId { get; set; }
}
