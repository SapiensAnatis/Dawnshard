using System.Collections.ObjectModel;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.Summon;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Summon;

namespace DragaliaAPI.Features.Summoning;

public class SummonBannerOptions
{
    private readonly List<Banner> banners = [];

    /// <summary>
    /// Gets a list of active summoning banners.
    /// </summary>
    public required IReadOnlyList<Banner> Banners
    {
        get => this.banners;
        init => this.banners = value as List<Banner> ?? value.ToList();
    }

    /// <summary>
    /// Initializes the instance, intended for use with <see cref="Microsoft.Extensions.Options.OptionsBuilder{T}.PostConfigure"/>.
    /// </summary>
    public void PostConfigure()
    {
        this.banners.Add(SystemBanners.RerollBanner);
        this.banners.Add(SystemBanners.AdventurerSummonBanner);
        this.banners.Add(SystemBanners.DragonSummonBanner);
        this.banners.Add(SystemBanners.AdventurerSummonPlusBanner);
        this.banners.Add(SystemBanners.DragonSummonPlusBanner);

        foreach (Banner banner in this.banners)
        {
            banner.PostConfigure();
        }
    }
}

public class Banner
{
    private SummonTypes summonType;

    /// <summary>
    /// Gets the ID of the banner, as well as its associated summon point trade.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the start date of the banner.
    /// </summary>
    public DateTimeOffset Start { get; init; }

    /// <summary>
    /// Gets the end date of the banner.
    /// </summary>
    public DateTimeOffset End { get; init; }

    /// <summary>
    /// Gets a value indicating whether the banner uses Gala Dragalia rates and has Gala characters available.
    /// </summary>
    public bool IsGala { get; init; }

    /// <summary>
    /// Gets a value indicating whether the banner is a prize showcase and awards materials too.
    /// </summary>
    public bool IsPrizeShowcase { get; init; }

    /// <summary>
    /// Gets a list of characters on rate up.
    /// </summary>
    public IReadOnlyList<Charas> PickupCharas { get; init; } = [];

    /// <summary>
    /// Gets a list of dragons on rate up.
    /// </summary>
    public IReadOnlyList<Dragons> PickupDragons { get; init; } = [];

    /// <summary>
    /// Gets a list of limited characters that are available, but not on rate up.
    /// </summary>
    public IReadOnlyList<Charas> LimitedCharas { get; init; } = [];

    /// <summary>
    /// Gets a list of limited dragons that are available, but not on rate up.
    /// </summary>
    public IReadOnlyList<Dragons> LimitedDragons { get; init; } = [];

    /// <summary>
    /// Gets a list of characters that can be purchased using wyrmsigils.
    /// </summary>
    public IReadOnlyList<Charas> TradeCharas { get; init; } = [];

    /// <summary>
    /// Gets a list of dragons that can be purchased using wyrmsigils.
    /// </summary>
    public IReadOnlyList<Dragons> TradeDragons { get; init; } = [];

    /// <summary>
    /// Gets a dictionary of wyrmsigil trade ID to <see cref="AtgenSummonPointTradeList"/>.
    /// </summary>
    public IReadOnlyDictionary<int, AtgenSummonPointTradeList> TradeDictionary
    {
        get;
        private set;
    } = ReadOnlyDictionary<int, AtgenSummonPointTradeList>.Empty;

    /// <summary>
    /// Gets the special ticket required to summon on this banner.
    /// </summary>
    public SummonTickets? RequiredTicketId { get; init; }

    public SummonTypes SummonType
    {
        get => this.summonType;
        init => this.summonType = value;
    }

    /// <summary>
    /// Gets a list of characters that should override the default pool.
    /// </summary>
    /// <remarks>Only one override pool can be used. <see cref="OverrideCharaPool"/> takes precedence if both are specified.</remarks>
    public IReadOnlyList<Charas>? OverrideCharaPool { get; init; }

    /// <summary>
    /// Gets a list of dragons that should override the default pool.
    /// </summary>
    /// <remarks>Only one override pool can be used. <see cref="OverrideCharaPool"/> takes precedence if both are specified.</remarks>
    public IReadOnlyList<Dragons>? OverrideDragonPool { get; init; }

    /// <summary>
    /// Initializes the instance, intended for use with <see cref="Microsoft.Extensions.Options.OptionsBuilder{T}.PostConfigure"/>.
    /// </summary>
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

        // Get summon type from MasterAsset, if not explicitly specified
        if (
            this.summonType == default
            && MasterAsset.SummonData.TryGetValue(this.Id, out SummonData? summonData)
        )
        {
            this.summonType = summonData.SummonType;
        }
    }
}
