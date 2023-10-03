using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
public class DbQuest : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Required]
    public int QuestId { get; set; }

    [Required]
    [Range(2, 3)]
    public byte State { get; set; } = 0;

    public bool IsMissionClear1 { get; set; } = false;

    public bool IsMissionClear2 { get; set; } = false;

    public bool IsMissionClear3 { get; set; } = false;

    public int PlayCount { get; set; } = 0;

    public int DailyPlayCount { get; set; } = 0;

    public int WeeklyPlayCount { get; set; } = 0;

    public DateTimeOffset LastDailyResetTime { get; set; } = DateTimeOffset.UnixEpoch;

    public DateTimeOffset LastWeeklyResetTime { get; set; } = DateTimeOffset.UnixEpoch;

    public bool IsAppear { get; set; } = true;

    public float BestClearTime { get; set; } = -1.0f;
}
