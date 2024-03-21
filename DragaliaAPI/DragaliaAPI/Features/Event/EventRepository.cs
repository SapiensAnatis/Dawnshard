using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Event;

public class EventRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    : IEventRepository
{
    public IQueryable<DbPlayerEventData> EventData =>
        apiContext.PlayerEventData.Where(x => x.ViewerId == playerIdentityService.ViewerId);

    public IQueryable<DbPlayerEventReward> Rewards =>
        apiContext.PlayerEventRewards.Where(x => x.ViewerId == playerIdentityService.ViewerId);

    public IQueryable<DbPlayerEventItem> Items =>
        apiContext.PlayerEventItems.Where(x => x.ViewerId == playerIdentityService.ViewerId);

    public IQueryable<DbPlayerEventPassive> Passives =>
        apiContext.PlayerEventPassives.Where(x => x.ViewerId == playerIdentityService.ViewerId);

    public async Task<DbPlayerEventData?> GetEventDataAsync(int eventId)
    {
        return await apiContext.PlayerEventData.FindAsync(playerIdentityService.ViewerId, eventId);
    }

    public async Task<bool> HasEventDataAsync(int eventId) =>
        await this.EventData.Where(x => x.EventId == eventId).AnyAsync();

    public async Task<IEnumerable<DbPlayerEventReward>> GetEventRewardsAsync(
        int eventId,
        bool isLocationReward = false
    )
    {
        List<DbPlayerEventReward> rewards = await Rewards
            .Where(x => x.EventId == eventId)
            .ToListAsync();

        return isLocationReward ? rewards.Where(x => x.IsLocationReward) : rewards;
    }

    public async Task<IEnumerable<int>> GetEventRewardIdsAsync(
        int eventId,
        bool isLocationReward = false
    )
    {
        List<DbPlayerEventReward> rewards = await Rewards
            .Where(x => x.EventId == eventId)
            .ToListAsync();

        return (isLocationReward ? rewards.Where(x => x.IsLocationReward) : rewards).Select(x =>
            x.RewardId
        );
    }

    public async Task<DbPlayerEventItem?> GetEventItemAsync(int itemId)
    {
        return await apiContext.PlayerEventItems.FindAsync(playerIdentityService.ViewerId, itemId);
    }

    public async Task<DbPlayerEventItem> GetEventItemAsync(int eventId, int itemType)
    {
        return await Items.SingleAsync(x => x.EventId == eventId && x.Type == itemType);
    }

    public async Task<IEnumerable<DbPlayerEventItem>> GetEventItemsAsync(int eventId)
    {
        return await Items.Where(x => x.EventId == eventId).ToListAsync();
    }

    public async Task<Dictionary<int, int>> GetEventItemQuantityAsync(int eventId)
    {
        return await Items
            .Where(x => x.EventId == eventId)
            .ToDictionaryAsync(x => x.Type, x => x.Quantity);
    }

    public async Task<IEnumerable<DbPlayerEventPassive>> GetEventPassivesAsync(int eventId)
    {
        return await Passives.Where(x => x.EventId == eventId).ToListAsync();
    }

    public async Task<DbPlayerEventPassive?> GetEventPassiveAsync(int eventId, int passiveId)
    {
        return await apiContext.PlayerEventPassives.FindAsync(
            playerIdentityService.ViewerId,
            eventId,
            passiveId
        );
    }

    public DbPlayerEventData CreateEventData(int eventId, bool customEventFlag = false)
    {
        return apiContext
            .PlayerEventData.Add(
                new DbPlayerEventData
                {
                    ViewerId = playerIdentityService.ViewerId,
                    EventId = eventId,
                    CustomEventFlag = customEventFlag
                }
            )
            .Entity;
    }

    public DbPlayerEventReward CreateEventReward(int eventId, int rewardId)
    {
        return apiContext
            .PlayerEventRewards.Add(
                new DbPlayerEventReward
                {
                    ViewerId = playerIdentityService.ViewerId,
                    EventId = eventId,
                    RewardId = rewardId
                }
            )
            .Entity;
    }

    public IEnumerable<DbPlayerEventItem> CreateEventItems(
        int eventId,
        IEnumerable<(int Id, int Type)> itemIds
    )
    {
        List<DbPlayerEventItem> items = new();

        foreach ((int itemId, int itemType) in itemIds)
        {
            items.Add(
                apiContext
                    .PlayerEventItems.Add(
                        new DbPlayerEventItem
                        {
                            ViewerId = playerIdentityService.ViewerId,
                            EventId = eventId,
                            Id = itemId,
                            Type = itemType
                        }
                    )
                    .Entity
            );
        }

        return items;
    }

    public IEnumerable<DbPlayerEventPassive> CreateEventPassives(
        int eventId,
        IEnumerable<int> passiveIds
    )
    {
        List<DbPlayerEventPassive> passives = new();

        string accountId = playerIdentityService.AccountId;

        foreach (int passiveId in passiveIds)
        {
            passives.Add(
                apiContext
                    .PlayerEventPassives.Add(
                        new DbPlayerEventPassive
                        {
                            ViewerId = playerIdentityService.ViewerId,
                            EventId = eventId,
                            PassiveId = passiveId
                        }
                    )
                    .Entity
            );
        }

        return passives;
    }

    public async Task AddItemQuantityAsync(int itemId, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);

        DbPlayerEventItem item =
            await GetEventItemAsync(itemId)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "Event item not found");

        item.Quantity += quantity;
    }

    public async Task RemoveItemQuantityAsync(int itemId, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);

        DbPlayerEventItem item =
            await GetEventItemAsync(itemId)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "Event item not found");

        item.Quantity -= quantity;
    }

    public async Task AddEventPassiveProgressAsync(int eventId, int passiveId, int progress)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(progress);

        DbPlayerEventPassive passive =
            await GetEventPassiveAsync(eventId, passiveId)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "Event passive not found");

        passive.Progress += progress;
    }
}
