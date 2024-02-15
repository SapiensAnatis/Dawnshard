using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(GameId), nameof(ViewerId), nameof(UnitNo))]
public class DbTimeAttackClearUnit : DbPartyUnitBase, IDbPlayerData
{
    public required string GameId { get; set; }

    public required long ViewerId { get; set; }

    public Dragons EquippedDragonEntityId { get; set; }

    public Talismans EquippedTalismanEntityId { get; set; }

    public int TalismanAbility1 { get; set; }

    public int TalismanAbility2 { get; set; }

    [ForeignKey($"{nameof(GameId)},{nameof(ViewerId)}")]
    public DbTimeAttackPlayer? Player { get; set; }
}
