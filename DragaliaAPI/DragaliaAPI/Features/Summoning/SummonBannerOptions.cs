using System.Collections.ObjectModel;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Summoning;

public class SummonBannerOptions
{
    private static readonly Banner RerollBanner =
        new()
        {
            Id = SummonConstants.RedoableSummonBannerId,
            IsGala = true,
            Start = DateTimeOffset.MinValue,
            End = DateTimeOffset.MaxValue,
        };

    private readonly List<Banner> banners = [];

    public required IReadOnlyList<Banner> Banners
    {
        get => this.banners;
        init => this.banners = value as List<Banner> ?? value.ToList();
    }

    public void PostConfigure()
    {
        this.banners.Add(RerollBanner);

        foreach (Banner banner in this.banners)
        {
            banner.PostConfigure();
        }
    }
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

    public IReadOnlyDictionary<int, AtgenSummonPointTradeList> TradeDictionary
    {
        get;
        private set;
    } = ReadOnlyDictionary<int, AtgenSummonPointTradeList>.Empty;

    public void PostConfigure()
    {
        // Official trade IDs for a banner followed the format e.g. [1020203001, 1020203002, 1020203003, ...] for
        // summon_point_id = 1020203. The convention of including the entity type in the ID is our own and is used
        // to keep the LINQ pure here. Hopefully the IDs we use should not matter too much as long as they don't
        // collide.

        IEnumerable<AtgenSummonPointTradeList> tradeCharas = this.TradeCharas.Select(
            (x, index) =>
                new AtgenSummonPointTradeList()
                {
                    TradeId = (this.Id * 1000) + 100 + index,
                    EntityType = EntityTypes.Chara,
                    EntityId = (int)x,
                }
        );

        IEnumerable<AtgenSummonPointTradeList> tradeDragons = this.TradeDragons.Select(
            (x, index) =>
                new AtgenSummonPointTradeList()
                {
                    TradeId = (this.Id * 1000) + 700 + index,
                    EntityType = EntityTypes.Dragon,
                    EntityId = (int)x,
                }
        );

        this.TradeDictionary = tradeCharas
            .Concat(tradeDragons)
            .ToDictionary(x => x.TradeId, x => x);
    }
}
