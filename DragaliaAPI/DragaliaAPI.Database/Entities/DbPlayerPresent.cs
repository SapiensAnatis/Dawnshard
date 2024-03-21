using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerPresent")]
public class DbPlayerPresent : DbPlayerData
{
    [Column("PresentId")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long PresentId { get; set; }

    [Column("MasterId")]
    public long MasterId { get; set; }

    [Column("State")]
    public long State { get; set; }

    [Column("EntityType")]
    [TypeConverter(typeof(EnumConverter))]
    public EntityTypes EntityType { get; set; }

    [Column("EntityId")]
    public int EntityId { get; set; }

    [Column("EntityQuantity")]
    public int EntityQuantity { get; set; } = 1;

    [Column("EntityLevel")]
    public int EntityLevel { get; set; }

    [Column("EntityLimitBreakCount")]
    public int EntityLimitBreakCount { get; set; }

    [Column("EntityStatusPlusCount")]
    public int EntityStatusPlusCount { get; set; }

    [Column("MessageId")]
    public PresentMessage MessageId { get; set; }

    [Column("MessageParamValue1")]
    public int MessageParamValue1 { get; set; }

    [Column("MessageParamValue2")]
    public int MessageParamValue2 { get; set; }

    [Column("MessageParamValue3")]
    public int MessageParamValue3 { get; set; }

    [Column("MessageParamValue4")]
    public int MessageParamValue4 { get; set; }

    [Column("CreateTime")]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset CreateTime { get; set; } = DateTimeOffset.UtcNow;

    [Column("ReceiveLimitTime")]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset? ReceiveLimitTime { get; set; }
}
