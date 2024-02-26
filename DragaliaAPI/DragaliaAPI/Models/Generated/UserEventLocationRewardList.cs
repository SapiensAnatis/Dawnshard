using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Event;

namespace DragaliaAPI.Models.Generated;

public partial class UserEventLocationRewardList : IEventRewardList<UserEventLocationRewardList>
{
    public static UserEventLocationRewardList FromDatabase(DbPlayerEventReward reward) =>
        new(reward.EventId, reward.RewardId);
}
