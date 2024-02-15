using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Event;

public interface IEventService
{
    Task<bool> GetCustomEventFlag(int eventId);

    Task<IEnumerable<T>> GetEventRewardList<T>(int eventId, bool isLocationReward = false)
        where T : IEventRewardList<T>;

    Task<EventPassiveList> GetEventPassiveList(int eventId);

    Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReceiveEventRewards(
        int eventId,
        IEnumerable<int>? rewardIds = null
    );

    Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReceiveEventLocationReward(
        int eventId,
        int locationId
    );

    Task CreateEventData(int eventId);

    #region User Data Providers

    Task<BuildEventUserList?> GetBuildEventUserData(int eventId);
    Task<RaidEventUserList?> GetRaidEventUserData(int eventId);
    Task<Clb01EventUserList?> GetClb01EventUserData(int eventId);
    Task<CollectEventUserList?> GetCollectEventUserData(int eventId);
    Task<CombatEventUserList?> GetCombatEventUserData(int eventId);
    Task<EarnEventUserList?> GetEarnEventUserData(int eventId);
    Task<ExHunterEventUserList?> GetExHunterEventUserData(int eventId);
    Task<ExRushEventUserList?> GetExRushEventUserData(int eventId);
    Task<MazeEventUserList?> GetMazeEventUserData(int eventId);
    Task<SimpleEventUserList?> GetSimpleEventUserData(int eventId);

    #endregion
}
