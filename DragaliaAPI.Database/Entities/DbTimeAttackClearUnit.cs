using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(RoomName), nameof(DeviceAccountId), nameof(UnitNo))]
public class DbTimeAttackClearUnit : DbPartyUnitBase
{
    public required string RoomName { get; set; }

    public required string DeviceAccountId { get; set; }

    public Dragons EquippedDragonEntityId { get; set; }

    public Talismans EquippedTalismanEntityId { get; set; }

    public int TalismanAbility1 { get; set; }

    public int TalismanAbility2 { get; set; }

    [ForeignKey($"{nameof(RoomName)},{nameof(DeviceAccountId)}")]
    public DbTimeAttackPlayer? Player { get; set; }
}
