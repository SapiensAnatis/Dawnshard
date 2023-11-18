using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(QuestEventId))]
public class DbQuestEvent : DbPlayerData
{
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
