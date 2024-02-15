using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Event;

public interface IEventRewardList<out T>
{
    public static abstract T FromDatabase(DbPlayerEventReward reward);
}
