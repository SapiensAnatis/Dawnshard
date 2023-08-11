using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(QuestEventId))]
public class DbQuestEvent : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("QuestEventId")]
    public required int QuestEventId { get; set; }

    [Column("DailyPlayCount")]
    public int DailyPlayCount { get; set; }

    [Column("LastDailyResetTime")]
    public DateTimeOffset LastDailyResetTime { get; set; } = DateTimeOffset.UnixEpoch;

    [Column("WeeklyPlayCount")]
    public int WeeklyPlayCount { get; set; }

    [Column("LastWeeklyResetTime")]
    public DateTimeOffset LastWeeklyResetTime { get; set; } = DateTimeOffset.UnixEpoch;

    [Column("QuestBonusReceiveCount")]
    public int QuestBonusReceiveCount { get; set; }

    [Column("QuestBonusStackCount")]
    public int QuestBonusStackCount { get; set; }

    [Column("QuestBonusStackTime")]
    public DateTimeOffset QuestBonusStackTime { get; set; } = DateTimeOffset.UnixEpoch;

    [Column("QuestBonusReserveCount")]
    public int QuestBonusReserveCount { get; set; }

    [Column("QuestBonusReserveTime")]
    public DateTimeOffset QuestBonusReserveTime { get; set; } = DateTimeOffset.UnixEpoch;
}
