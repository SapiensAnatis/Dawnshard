using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Trade;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Trade;

public class TreasureTradeService : ITreasureTradeService
{
    private readonly ITradeRepository tradeRepository;
    private readonly IRewardService rewardService;
    private readonly ILogger<TreasureTradeService> logger;

    public TreasureTradeService(
        ITradeRepository tradeRepository,
        IRewardService rewardService,
        ILogger<TreasureTradeService> logger
    )
    {
        this.tradeRepository = tradeRepository;
        this.rewardService = rewardService;
        this.logger = logger;
    }

    public IEnumerable<TreasureTradeList> GetCurrentTradeList()
    {
        DateTimeOffset current = DateTimeOffset.UtcNow;

        return MasterAsset.TreasureTrade.Enumerable
            .Where(x => x.CompleteDate == DateTimeOffset.UnixEpoch || x.CompleteDate > current)
            .Select(
                trade =>
                    new TreasureTradeList
                    {
                        treasure_trade_id = trade.Id,
                        priority = trade.Priority,
                        tab_group_id = trade.TabGroupId,
                        commence_date = trade.CommenceDate,
                        complete_date = trade.CompleteDate,
                        is_lock_view = trade.IsLockView,
                        reset_type = trade.ResetType,
                        limit = trade.Limit,
                        destination_entity_type = trade.DestinationEntityType,
                        destination_entity_id = trade.DestinationEntityId,
                        destination_entity_quantity = trade.DestinationEntityQuantity,
                        destination_limit_break_count = trade.DestinationLimitBreakCount,
                        need_trade_entity_list = trade.NeedEntities.Select(
                            x =>
                                new AtgenNeedTradeEntityList(
                                    x.Type,
                                    x.Id,
                                    x.Quantity,
                                    x.LimitBreakCount
                                )
                        )
                    }
            );
    }

    public async Task<IEnumerable<UserTreasureTradeList>> GetUserTradeList()
    {
        return (await this.tradeRepository.TreasureTrades.ToListAsync()).Select(
            x => new UserTreasureTradeList(x.Id, x.Count, x.LastTradeTime)
        );
    }

    public async Task DoTreasureTrade(
        int tradeId,
        int count,
        IEnumerable<AtgenNeedUnitList>? needUnitList
    )
    {
        TreasureTrade trade = MasterAsset.TreasureTrade[tradeId];

        logger.LogDebug("Processing {tradeQuantity}x treasure trade {tradeId}", count, tradeId);

        foreach (
            (EntityTypes type, int id, int quantity, int limitBreakCount) in trade.NeedEntities
        )
        {
            if (type == EntityTypes.None)
                continue;

            await this.rewardService.GrantReward(type, id, -(quantity * count), limitBreakCount);
        }

        await this.rewardService.GrantReward(
            trade.DestinationEntityType,
            trade.DestinationEntityId,
            trade.DestinationEntityQuantity,
            trade.DestinationLimitBreakCount
        );

        await this.tradeRepository.AddTrade(tradeId, count, DateTimeOffset.UtcNow);
    }
}
