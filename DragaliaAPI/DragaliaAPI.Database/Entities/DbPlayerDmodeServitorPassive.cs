using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(PassiveId))]
public class DbPlayerDmodeServitorPassive : DbPlayerData
{
    [Column("PassiveId")]
    public required DmodeServitorPassiveType PassiveId { get; set; }

    [Column("Level")]
    public int Level { get; set; } = 1;
}
