using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.MasterAsset.Models.TimeAttack;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Tracks receipt of a <see cref="RankingTierReward"/>.
/// </summary>
[Index(nameof(QuestId))]
[PrimaryKey(nameof(ViewerId), nameof(RewardId))]
public class DbReceivedRankingTierReward : DbPlayerData
{
    public int RewardId { get; set; }

    public int QuestId { get; set; }
}
