using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Event;

public interface IEventRepository
{
    IQueryable<DbPlayerEventData> EventData { get; }
    IQueryable<DbPlayerEventReward> Rewards { get; }
    IQueryable<DbPlayerEventItem> Items { get; }

    Task<DbPlayerEventData?> GetEventDataAsync(int eventId);

    Task<IEnumerable<DbPlayerEventReward>> GetEventRewardsAsync(
        int eventId,
        bool isLocationReward = false
    );
    Task<IEnumerable<int>> GetEventRewardIdsAsync(int eventId, bool isLocationReward = false);

    Task<DbPlayerEventItem?> GetEventItemAsync(int itemId);
    Task<DbPlayerEventItem> GetEventItemAsync(int eventId, int itemType);
    Task<IEnumerable<DbPlayerEventItem>> GetEventItemsAsync(int eventId);
    Task<Dictionary<int, int>> GetEventItemQuantityAsync(int eventId);

    DbPlayerEventData CreateEventData(int eventId, bool customEventFlag = false);
    DbPlayerEventReward CreateEventReward(int eventId, int rewardId);
    IEnumerable<DbPlayerEventItem> CreateEventItems(
        int eventId,
        IEnumerable<(int Id, int Type)> itemIds
    );

    Task AddItemQuantityAsync(int itemId, int quantity);
    Task RemoveItemQuantityAsync(int itemId, int quantity);
}
