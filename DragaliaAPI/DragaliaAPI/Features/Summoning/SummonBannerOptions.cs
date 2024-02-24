using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Summoning;

public class SummonBannerOptions
{
    public required IReadOnlyList<Banner> Banners { get; init; }
}

public class Banner
{
    public int Id { get; init; }

    public DateTime Start { get; init; }

    public DateTime End { get; init; }

    public bool IsGala { get; init; }

    public bool IsPrizeShowcase { get; init; }

    public required IReadOnlyList<Charas> FeaturedAdventurers { get; init; }

    public required IReadOnlyList<Dragons> FeaturedDragons { get; init; }

    public required IReadOnlyList<Charas> LimitedAdventurers { get; init; }

    public required IReadOnlyList<Dragons> LimitedDragons { get; init; }

    public required IReadOnlyList<Charas> TradeAdventurers { get; init; }

    public required IReadOnlyList<Dragons> TradeDragons { get; init; }
}
