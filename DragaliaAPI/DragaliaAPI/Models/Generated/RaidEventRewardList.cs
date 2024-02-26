using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Event;

namespace DragaliaAPI.Models.Generated;

public partial class RaidEventRewardList : IEventRewardList<RaidEventRewardList>
{
    public static RaidEventRewardList FromDatabase(DbPlayerEventReward reward) =>
        new(reward.EventId, reward.RewardId);
}
