using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Trade;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Trade;

public class TradeService(
    ITradeRepository tradeRepository,
    IRewardService rewardService,
    ILogger<TradeService> logger,
    IPaymentService paymentService,
    IMissionProgressionService missionProgressionService
) : ITradeService
{
    public IEnumerable<TreasureTradeList> GetCurrentTreasureTradeList()
    {
        DateTimeOffset current = DateTimeOffset.UtcNow;

        return MasterAsset
            .TreasureTrade.Enumerable.Where(
                x => x.CompleteDate == DateTimeOffset.UnixEpoch || x.CompleteDate > current
            )
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

    public IEnumerable<AbilityCrestTradeList> GetCurrentAbilityCrestTradeList()
    {
        DateTimeOffset current = DateTimeOffset.UtcNow;

        return MasterAsset
            .AbilityCrestTrade.Enumerable.Where(
                x => x.CompleteDate == DateTimeOffset.UnixEpoch || x.CompleteDate > current
            )
            .Select(
                trade =>
                    new AbilityCrestTradeList
                    {
                        ability_crest_trade_id = trade.Id,
                        ability_crest_id = trade.AbilityCrestId,
                        complete_date = trade.CompleteDate,
                        pickup_view_start_date = trade.PickupViewStartDate,
                        pickup_view_end_date = trade.PickupViewEndDate,
                        need_dew_point = trade.NeedDewPoint,
                        priority = trade.Priority
                    }
            );
    }

    public IEnumerable<EventTradeList> GetEventTradeList(int tradeGroupId)
    {
        return MasterAsset
            .EventTreasureTrade.Enumerable.Where(x => x.TradeGroupId == tradeGroupId)
            .Select(
                x =>
                    new EventTradeList
                    {
                        event_trade_id = x.Id,
                        trade_group_id = x.TradeGroupId,
                        tab_group_id = x.TabGroupId,
                        priority = x.Priority,
                        is_lock_view = x.IsLockView,
                        commence_date = x.CommenceDate,
                        complete_date = x.CompleteDate,
                        reset_type = x.ResetType,
                        limit = x.Limit,
                        destination_entity_type = x.DestinationEntityType,
                        destination_entity_id = x.DestinationEntityId,
                        destination_entity_quantity = x.DestinationEntityQuantity,
                        need_entity_list = x.NeedEntities.Where(y => y.Type != EntityTypes.None)
                            .Select(
                                z => new AtgenBuildEventRewardEntityList(z.Type, z.Id, z.Quantity)
                            ),
                        read_story_count = 0,
                        clear_target_quest_id = 0
                    }
            );
    }

    public async Task<IEnumerable<AtgenUserEventTradeList>> GetUserEventTradeList()
    {
        return await tradeRepository
            .Trades.Where(x => x.Type == TradeType.Event)
            .Select(x => new AtgenUserEventTradeList(x.Id, x.Count))
            .ToListAsync();
    }

    public async Task<IEnumerable<UserAbilityCrestTradeList>> GetUserAbilityCrestTradeList()
    {
        return await tradeRepository
            .Trades.Where(x => x.Type == TradeType.AbilityCrest)
            .Select(x => new UserAbilityCrestTradeList(x.Id, x.Count))
            .ToListAsync();
    }

    public async Task<IEnumerable<UserTreasureTradeList>> GetUserTreasureTradeList()
    {
        return (await tradeRepository.GetTradesByTypeAsync(TradeType.Treasure)).Select(
            x => new UserTreasureTradeList(x.Id, x.Count, x.LastTradeTime)
        );
    }

    public async Task DoTrade(
        TradeType tradeType,
        int tradeId,
        int count,
        IEnumerable<AtgenNeedUnitList>? needUnitList = null
    )
    {
        logger.LogDebug(
            "Processing {tradeQuantity}x {tradeId} of trade type {tradeType}",
            count,
            tradeId,
            tradeType
        );

        TreasureTrade trade = tradeType switch
        {
            TradeType.None
                => throw new DragaliaException(
                    ResultCode.CommonDataValidationError,
                    "Invalid trade type none"
                ),
            TradeType.Treasure => MasterAsset.TreasureTrade[tradeId],
            TradeType.Event => MasterAsset.EventTreasureTrade[tradeId],
            TradeType.AbilityCrest
                => throw new DragaliaException(
                    ResultCode.CommonInvalidArgument,
                    "Cannot process ability crest type in normal trade endpoint"
                ),
            _ => throw new DragaliaException(ResultCode.CommonInvalidArgument, "Invalid trade type")
        };

        foreach (
            (EntityTypes type, int id, int quantity, int limitBreakCount) in trade.NeedEntities
        )
        {
            if (type == EntityTypes.None)
                continue;

            await paymentService.ProcessPayment(
                new Entity(type, id, quantity * count, limitBreakCount)
            );
        }

        await rewardService.GrantReward(
            new(
                trade.DestinationEntityType,
                trade.DestinationEntityId,
                trade.DestinationEntityQuantity * count,
                trade.DestinationLimitBreakCount
            )
        );

        await tradeRepository.AddTrade(tradeType, tradeId, count, DateTimeOffset.UtcNow);

        int totalCount = (await tradeRepository.FindTrade(tradeId))?.Count ?? 0;

        missionProgressionService.OnTreasureTrade(
            tradeId,
            trade.DestinationEntityType,
            trade.DestinationEntityId,
            count,
            totalCount
        );
    }

    public async Task DoAbilityCrestTrade(int id, int count)
    {
        logger.LogDebug("Processing {tradeQuantity}x ability crest trade {tradeId}", count, id);

        if (count != 1)
            throw new DragaliaException(ResultCode.CommonDataValidationError, "Invalid count");

        AbilityCrestTrade trade = MasterAsset.AbilityCrestTrade[id];

        await paymentService.ProcessPayment(
            PaymentTypes.DewPoint,
            expectedPrice: trade.NeedDewPoint
        );

        await rewardService.GrantReward(
            new Entity(EntityTypes.Wyrmprint, (int)trade.AbilityCrestId)
        );

        await tradeRepository.AddTrade(TradeType.AbilityCrest, id, count);
    }
}
