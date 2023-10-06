using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(GameId), nameof(DeviceAccountId), nameof(UnitNo))]
public class DbTimeAttackClearUnit : DbPartyUnitBase
{
    public required string GameId { get; set; }

    public required string DeviceAccountId { get; set; }

    public Dragons EquippedDragonEntityId { get; set; }

    public Talismans EquippedTalismanEntityId { get; set; }

    public int TalismanAbility1 { get; set; }

    public int TalismanAbility2 { get; set; }

    [ForeignKey($"{nameof(GameId)},{nameof(DeviceAccountId)}")]
    public DbTimeAttackPlayer? Player { get; set; }
}
