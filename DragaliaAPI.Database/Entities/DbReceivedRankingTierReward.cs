using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Tracks receipt of a <see cref="Shared.MasterAsset.Models.RankingTierReward"/>.
/// </summary>
[Index(nameof(QuestId))]
[Index(nameof(DeviceAccountId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(RewardId))]
public class DbReceivedRankingTierReward
{
    [ForeignKey(nameof(Player))]
    public required string DeviceAccountId { get; set; }

    public int RewardId { get; set; }

    public int QuestId { get; set; }

    public DbPlayer? Player { get; set; }
}
