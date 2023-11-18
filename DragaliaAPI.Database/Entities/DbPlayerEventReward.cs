using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(EventId), nameof(RewardId))]
public class DbPlayerEventReward : DbPlayerData
{
    [Column("EventId")]
    public required int EventId { get; set; }

    [Column("RewardId")]
    public required int RewardId { get; set; }

    [NotMapped]
    public bool IsLocationReward => RewardId > EventId * 100; // Location rewards use a combined id, meaning they are quite large (EventId | LocationRewardId:00)
}
