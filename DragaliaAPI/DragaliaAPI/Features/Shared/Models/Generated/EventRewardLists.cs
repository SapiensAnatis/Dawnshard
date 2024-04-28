using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Event;

namespace DragaliaAPI.Features.Shared.Models.Generated;

public partial class BuildEventRewardList : IEventRewardList<BuildEventRewardList>
{
    public static BuildEventRewardList FromDatabase(DbPlayerEventReward reward) =>
        new(reward.EventId, reward.RewardId);
}
