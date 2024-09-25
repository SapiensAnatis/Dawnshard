using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Event.Summon;

internal class EventSummonOptions
{
    public required IReadOnlyList<EventSummonConfiguration> EventSummons { get; init; }
}

internal class EventSummonConfiguration
{
    public int EventId { get; init; }

    public required IReadOnlyDictionary<int, EventSummonItemConfiguration> Items { get; init; }
}

internal class EventSummonItemCount
{
    public int Box1Count { get; init; }

    public int Box2Count { get; init; }

    public int Box3Count { get; init; }

    public int Box4Count { get; init; }

    public int FinalCount { get; init; }
}

internal class EventSummonItemConfiguration
{
    public EntityTypes EntityType { get; init; }

    public int EntityId { get; init; }

    public int EntityQuantity { get; init; }

    /// <summary>
    /// The order this item should appear in within the summary of featured items.
    /// </summary>
    public int PickupItemState { get; init; }

    public bool ResetItemFlag { get; init; }

    public required EventSummonItemCount TotalCounts { get; init; }
    public int? TwoStepId { get; init; }

    public int GetTotalCountInBox(int boxNumber) =>
        boxNumber switch
        {
            < 1 => throw new ArgumentOutOfRangeException(nameof(boxNumber)),
            1 => this.TotalCounts.Box1Count,
            2 => this.TotalCounts.Box2Count,
            3 => this.TotalCounts.Box3Count,
            4 => this.TotalCounts.Box4Count,
            >= 5 => this.TotalCounts.FinalCount,
        };
}
