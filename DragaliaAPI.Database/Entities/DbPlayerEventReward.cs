using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId), nameof(EventId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(EventId), nameof(RewardId))]
public class DbPlayerEventReward : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("EventId")]
    public required int EventId { get; set; }

    [Column("RewardId")]
    public required int RewardId { get; set; }

    [NotMapped]
    public bool IsLocationReward => RewardId > EventId * 100; // Location rewards use a combined id, meaning they are quite large (EventId | LocationRewardId:00)
}
