using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(Id), nameof(Type))]
public class DbPlayerMission : DbPlayerData
{
    [Column("MissionId")]
    [Required]
    public int Id { get; set; }

    [Column("Progress")]
    public int Progress { get; set; }

    [Column("State")]
    [TypeConverter(typeof(EnumConverter))]
    public MissionState State { get; set; } = MissionState.Invalid;

    [Column("StartDate")]
    public DateTimeOffset Start { get; set; } = DateTimeOffset.UnixEpoch;

    [Column("EndDate")]
    public DateTimeOffset End { get; set; } = DateTimeOffset.UnixEpoch;

    [Column("Type")]
    [TypeConverter(typeof(EnumConverter))]
    [Required]
    public MissionType Type { get; set; } = MissionType.Invalid;

    [Column("Pickup")]
    public bool Pickup { get; set; }

    [Column("GroupId")]
    public int? GroupId { get; set; }
}
