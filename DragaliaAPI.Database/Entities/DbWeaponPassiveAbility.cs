using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(DeviceAccountId), nameof(WeaponPassiveAbilityId))]
[Index(nameof(DeviceAccountId))]
public class DbWeaponPassiveAbility : IDbHasAccountId
{
    public DbPlayer? Owner { get; set; }

    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    public required int WeaponPassiveAbilityId { get; set; }
}
