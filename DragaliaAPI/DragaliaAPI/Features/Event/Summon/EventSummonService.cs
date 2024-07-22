using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ArgumentException = System.ArgumentException;

namespace DragaliaAPI.Features.Event.Summon;

internal class EventSummonService(
    ApiContext apiContext,
    IOptionsMonitor<EventSummonOptions> eventSummonOptionsMonitor,
    IRewardService rewardService,
    IPresentService presentService,
    IEnumerable<TwoStepItemGenerator> itemGenerators
)
{
    private const int MaxExecCount = 100;

    private readonly EventSummonOptions eventSummonOptions = eventSummonOptionsMonitor.CurrentValue;

    private readonly Dictionary<int, TwoStepItemGenerator> itemGeneratorLookup =
        itemGenerators.ToDictionary(x => x.Id, x => x);

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
                    Items = x.Items.Select(y => new { y.ItemId, y.TimesSummoned }),
                })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new DragaliaException(
                ResultCode.EntityRaidEventDataNotFound,
                "No event summon data found"
            );

        IReadOnlyDictionary<int, EventSummonItemConfiguration> itemDict = GetEventConfig(
            eventId
        ).Items;

        BoxSummonInfo data = EventSummonLogic.GetBoxSummonInfo(
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
        bool isEnableStopByTarget,
        CancellationToken cancellationToken
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
                .FirstOrDefaultAsync(x => x.EventId == eventId, cancellationToken)
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

        bool stoppedByTarget = false;

        for (int i = 0; ; i++)
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
            Entity configEntity = GenerateEntityFromPull(config);
            rewards.Add(i, configEntity);

            drawDetails.Add(
                new()
                {
                    Id = selectedId,
                    EntityId = configEntity.Id,
                    EntityQuantity = configEntity.Quantity,
                    EntityType = configEntity.Type,
                    IsNew = false,
                    ViewRarity = 0,
                }
            );

            if (idPool.Count == 0)
            {
                break;
            }

            if (userData.Points < boxSummonData.CostNum)
            {
                break;
            }

            if (isEnableStopByTarget && config.ResetItemFlag)
            {
                stoppedByTarget = true;
                break;
            }

            if (!isEnableStopByTarget && i + 1 >= numSummons)
            {
                break;
            }
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

        BoxSummonInfo newBoxSummonInfo = EventSummonLogic.GetBoxSummonInfo(
            userData.BoxNumber,
            userData.Items.ToDictionary(x => x.ItemId, x => x.TimesSummoned),
            itemDict
        );

        return new()
        {
            EventId = eventId,
            EventPoint = userData.Points,
            MaxExecCount = MaxExecCount,
            RemainingQuantity = newBoxSummonInfo.RemainingQuantity,
            ResetPossible = newBoxSummonInfo.ResetPossible,
            BoxSummonDetail = newBoxSummonInfo.DetailList,
            BoxSummonSeq = userData.BoxNumber,
            IsStoppedByTarget = stoppedByTarget,
            DrawDetails = drawDetails,
        };
    }

    public async Task ResetBoxSummon(int eventId, CancellationToken cancellationToken)
    {
        DbPlayerEventSummonData userData =
            await apiContext
                .PlayerEventSummonData.Include(x => x.Items)
                .AsTracking()
                .FirstOrDefaultAsync(x => x.EventId == eventId, cancellationToken)
            ?? throw new DragaliaException(
                ResultCode.EntityRaidEventDataNotFound,
                "No event summon data found"
            );

        IReadOnlyDictionary<int, EventSummonItemConfiguration> itemDict = GetEventConfig(
            eventId
        ).Items;

        (_, bool resetPossible, _) = EventSummonLogic.GetBoxSummonInfo(
            userData.BoxNumber,
            userData.Items.ToDictionary(x => x.ItemId, x => x.TimesSummoned),
            itemDict
        );

        if (!resetPossible)
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                "Box reset request is invalid"
            );
        }

        userData.BoxNumber += 1;
        userData.Items.Clear();
    }

    private EventSummonConfiguration GetEventConfig(int eventId) =>
        this.eventSummonOptions.EventSummons.FirstOrDefault(x => x.EventId == eventId)
        ?? throw new InvalidOperationException(
            $"No item summon table configured for event {eventId}!"
        );

    private Entity GenerateEntityFromPull(EventSummonItemConfiguration config)
    {
        if (config.TwoStepId is null)
        {
            return new Entity(config.EntityType, config.EntityId, config.EntityQuantity);
        }

        if (
            !this.itemGeneratorLookup.TryGetValue(
                config.TwoStepId.Value,
                out TwoStepItemGenerator? generator
            )
        )
        {
            throw new InvalidOperationException(
                $"TwoStepId {config.TwoStepId.Value} had no registered item generator"
            );
        }

        return generator.GenerateRandomEntity();
    }
}
