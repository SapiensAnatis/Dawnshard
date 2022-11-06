using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Database.Entities;

public class DbQuest
{
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = string.Empty;

    [Required]
    public int QuestId { get; set; }

    [Required]
    [Range(2, 3)]
    public byte State { get; set; }

    [Required]
    public bool IsMissionClear1 { get; set; }

    [Required]
    public bool IsMissionClear2 { get; set; }

    [Required]
    public bool IsMissionClear3 { get; set; }

    [Required]
    public int PlayCount { get; set; }

    [Required]
    public int DailyPlayCount { get; set; }

    [Required]
    public int WeeklyPlayCount { get; set; }

    [Required]
    public int LastDailyResetTime { get; set; }

    [Required]
    public int LastWeeklyResetTime { get; set; }

    [Required]
    public bool IsAppear { get; set; }

    [Required]
    public float BestClearTime { get; set; }
}
