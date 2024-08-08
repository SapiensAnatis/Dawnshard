using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ArgumentException = System.ArgumentException;

namespace DragaliaAPI.Features.Event.Summon;

public class EventSummonService(
    ApiContext apiContext,
    IOptionsMonitor<EventSummonOptions> eventSummonOptionsMonitor,
    IRewardService rewardService,
    IPresentService presentService
)
{
    private const int MaxExecCount = 10;

    private readonly EventSummonOptions eventSummonOptions = eventSummonOptionsMonitor.CurrentValue;

    public async Task<AtgenBoxSummonData> GetBoxSummonData(
        int eventId,
        CancellationToken cancellationToken
    )
    {
        var userData =
            await apiContext
                .PlayerEventSummonData.Where(x => x.EventId == eventId)
                .Select(x => new
                {
                    x.BoxNumber,
                    x.Points,
                    Items = x.Items.Select(y => new { y.ItemId, y.TimesSummoned })
                })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new DragaliaException(
                ResultCode.EntityRaidEventDataNotFound,
                "No event summon data found"
            );

        IReadOnlyDictionary<int, EventSummonItemConfiguration> itemDict = GetEventConfig(
            eventId
        ).Items;

        EventSummonLogic.BoxSummonInfo data = EventSummonLogic.GetBoxSummonInfo(
            userData.BoxNumber,
            userData.Items.ToDictionary(x => x.ItemId, x => x.TimesSummoned),
            itemDict
        );

        return new()
        {
            EventId = eventId,
            EventPoint = userData.Points,
            MaxExecCount = MaxExecCount,
            RemainingQuantity = data.RemainingQuantity,
            ResetPossible = data.ResetPossible,
            BoxSummonDetail = data.DetailList,
            BoxSummonSeq = userData.BoxNumber,
        };
    }

    public async Task<AtgenBoxSummonResult> ExecuteBoxSummon(
        int eventId,
        int numSummons,
        bool isEnableStopByTarget
    )
    {
        if (
            MasterAsset.BoxSummonData.Enumerable.FirstOrDefault(x => x.EventId == eventId)
            is not { } boxSummonData
        )
        {
            throw new ArgumentException($"Invalid event ID {eventId}: does not have a box summon");
        }

        DbPlayerEventSummonData userData =
            await apiContext
                .PlayerEventSummonData.Include(x => x.Items)
                .AsTracking()
                .FirstOrDefaultAsync()
            ?? throw new DragaliaException(
                ResultCode.EntityRaidEventDataNotFound,
                "No event summon data found"
            );

        IReadOnlyDictionary<int, EventSummonItemConfiguration> itemDict = GetEventConfig(
            eventId
        ).Items;

        if (userData.Points < numSummons * boxSummonData.CostNum)
        {
            throw new DragaliaException(
                ResultCode.SummonPointShort,
                $"Insufficient event summon points: need {numSummons * boxSummonData.CostNum}, have {userData.Points}"
            );
        }

        List<int> idPool = new(itemDict.Count);
        Dictionary<int, Entity> rewards = new(numSummons);
        List<AtgenDrawDetails> drawDetails = new(numSummons);

        foreach ((int id, EventSummonItemConfiguration config) in itemDict)
        {
            int baseQuantity = config.GetTotalCountInBox(userData.BoxNumber);
            int summonedQuantity =
                userData.Items.FirstOrDefault(x => x.ItemId == id)?.TimesSummoned ?? 0;
            int remainingQuantity = Math.Max(0, baseQuantity - summonedQuantity);

            idPool.AddRange(Enumerable.Repeat(id, remainingQuantity));
        }

        for (int i = 0; i < numSummons; i++)
        {
            int selectedIdIndex = Random.Shared.Next(idPool.Count);
            int selectedId = idPool[selectedIdIndex];

            if (userData.Items.FirstOrDefault(x => x.ItemId == selectedId) is not { } summonItem)
            {
                summonItem = new() { ItemId = selectedId, TimesSummoned = 0 };
                userData.Items.Add(summonItem);
            }

            summonItem.TimesSummoned += 1;
            userData.Points -= boxSummonData.CostNum;

            idPool.RemoveAt(selectedIdIndex);

            EventSummonItemConfiguration config = itemDict[selectedId];
            rewards.Add(i, new Entity(config.EntityType, config.EntityId, config.EntityQuantity));

            drawDetails.Add(
                new()
                {
                    Id = selectedId,
                    EntityId = config.EntityId,
                    EntityQuantity = config.EntityQuantity,
                    EntityType = config.EntityType,
                    IsNew = false,
                    ViewRarity = 0,
                }
            );
        }

        IDictionary<int, RewardGrantResult> resultDict = await rewardService.BatchGrantRewards(
            rewards
        );

        foreach ((int rewardId, RewardGrantResult grantResult) in resultDict)
        {
            if (grantResult == RewardGrantResult.Limit)
            {
                presentService.AddPresent(
                    new Present.Present(PresentMessage.EventSummonReward, rewards[rewardId])
                );
            }
        }

        EventSummonLogic.BoxSummonInfo data = EventSummonLogic.GetBoxSummonInfo(
            userData.BoxNumber,
            userData.Items.ToDictionary(x => x.ItemId, x => x.TimesSummoned),
            itemDict
        );

        return new()
        {
            EventId = eventId,
            EventPoint = userData.Points,
            MaxExecCount = MaxExecCount,
            RemainingQuantity = data.RemainingQuantity,
            ResetPossible = data.ResetPossible,
            BoxSummonDetail = data.DetailList,
            BoxSummonSeq = userData.BoxNumber,
            IsStoppedByTarget = false,
            DrawDetails = drawDetails
        };
    }

    private EventSummonConfiguration GetEventConfig(int eventId) =>
        this.eventSummonOptions.EventSummons.FirstOrDefault(x => x.EventId == eventId)
        ?? throw new InvalidOperationException(
            $"No item summon table configured for event {eventId}!"
        );
}
