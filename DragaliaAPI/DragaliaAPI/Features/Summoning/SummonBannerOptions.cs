using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Summoning;

public class SummonBannerOptions
{
    public required List<Banner> Banners { get; init; }
}

public class Banner
{
    public int Id { get; init; }

    public DateTimeOffset Start { get; init; }

    public DateTimeOffset End { get; init; }

    public bool IsGala { get; init; }

    public bool IsPrizeShowcase { get; init; }

    public IReadOnlyList<Charas> PickupCharas { get; init; } = [];

    public IReadOnlyList<Dragons> PickupDragons { get; init; } = [];

    public IReadOnlyList<Charas> LimitedCharas { get; init; } = [];

    public IReadOnlyList<Dragons> LimitedDragons { get; init; } = [];

    public IReadOnlyList<Charas> TradeCharas { get; init; } = [];

    public IReadOnlyList<Dragons> TradeDragons { get; init; } = [];
}
