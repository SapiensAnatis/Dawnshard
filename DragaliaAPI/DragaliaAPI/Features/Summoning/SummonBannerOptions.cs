using DragaliaAPI.Models.Generated;
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

    public Dictionary<int, AtgenSummonPointTradeList> GetTradeDictionary()
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

        return tradeCharas.Concat(tradeDragons).ToDictionary(x => x.TradeId, x => x);
    }
}
