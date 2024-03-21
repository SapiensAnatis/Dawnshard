using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(CharaId))]
public class DbPlayerDmodeChara : DbPlayerData
{
    [Column("CharaId")]
    public required Charas CharaId { get; set; }

    [Column("MaxFloor")]
    public int MaxFloor { get; set; }

    [Column("MaxScore")]
    public int MaxScore { get; set; }

    [Column("SelectedServitorId")]
    public int SelectedServitorId { get; set; }

    [Column("SelectEditSkillCharaId1")]
    public Charas SelectEditSkillCharaId1 { get; set; }

    [Column("SelectEditSkillCharaId2")]
    public Charas SelectEditSkillCharaId2 { get; set; }

    [Column("SelectEditSkillCharaId3")]
    public Charas SelectEditSkillCharaId3 { get; set; }
}
