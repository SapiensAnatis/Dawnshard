using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId))]
public class DbPlayerDmodeDungeon : DbPlayerData
{
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
