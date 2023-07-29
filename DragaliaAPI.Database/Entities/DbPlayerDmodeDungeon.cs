using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(DeviceAccountId))]
public class DbPlayerDmodeDungeon : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    [Required]
    public required string DeviceAccountId { get; set; }

    [Column("CharaId")]
    public Charas CharaId { get; set; }

    [Column("Floor")]
    public int Floor { get; set; }

    [Column("QuestTime")]
    public int QuestTime { get; set; }

    [Column("DungeonScore")]
    public int DungeonScore { get; set; }

    [Column("IsPlayEnd")]
    public bool IsPlayEnd { get; set; }

    [Column("State")]
    public DungeonState State { get; set; } = DungeonState.Waiting;

    public void Clear()
    {
        CharaId = 0;
        Floor = 0;
        QuestTime = 0;
        DungeonScore = 0;
        IsPlayEnd = false;
        State = DungeonState.Waiting;
    }
}
