using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerPresent")]
[PrimaryKey(nameof(DeviceAccountId), nameof(PresentId))]
[Index(nameof(DeviceAccountId))]
public class DbPlayerPresent : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("PresentId")]
    public long PresentId { get; set; }

    [Column("MasterId")]
    public long MasterId { get; set; }

    [Column("State")]
    public long State { get; set; }

    [Column("EntityType")]
    [TypeConverter(typeof(EnumConverter))]
    public EntityTypes EntityType { get; set; }

    [Column("EntityId")]
    public long EntityId { get; set; }

    [Column("EntityQuantity")]
    public int EntityQuantity { get; set; }

    [Column("EntityLevel")]
    public int EntityLevel { get; set; }

    [Column("EntityLimitBreakCount")]
    public int EntityLimitBreakCount { get; set; }

    [Column("EntityStatusPlusCount")]
    public int EntityStatusPlusCount { get; set; }

    [Column("MessageId")]
    public long MessageId { get; set; }

    [Column("MessageParamValue1")]
    public long MessageParamValue1 { get; set; }

    [Column("MessageParamValue2")]
    public long MessageParamValue2 { get; set; }

    [Column("MessageParamValue3")]
    public long MessageParamValue3 { get; set; }

    [Column("MessageParamValue4")]
    public long MessageParamValue4 { get; set; }

    [Column("CreateTime")]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset CreateTime { get; set; }

    [Column("ReceiveLimitTime")]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset? ReceiveLimitTime { get; set; }
}
