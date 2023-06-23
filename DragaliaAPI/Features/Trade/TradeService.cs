using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Trade;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Trade;

public class TradeService : ITradeService
{
    private readonly ITradeRepository tradeRepository;
    private readonly IRewardService rewardService;
    private readonly ILogger<TradeService> logger;
    private readonly IUserDataRepository userDataRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IPaymentService paymentService;

    public TradeService(
        ITradeRepository tradeRepository,
        IRewardService rewardService,
        ILogger<TradeService> logger,
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        IPaymentService paymentService
    )
    {
        this.tradeRepository = tradeRepository;
        this.rewardService = rewardService;
        this.logger = logger;
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.paymentService = paymentService;
    }

    public IEnumerable<TreasureTradeList> GetCurrentTreasureTradeList()
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

    public IEnumerable<AbilityCrestTradeList> GetCurrentAbilityCrestTradeList()
    {
        DateTimeOffset current = DateTimeOffset.UtcNow;

        return MasterAsset.AbilityCrestTrade.Enumerable
            .Where(x => x.CompleteDate == DateTimeOffset.UnixEpoch || x.CompleteDate > current)
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

    public async Task<IEnumerable<AtgenUserEventTradeList>> GetUserEventTradeList()
    {
        return await this.tradeRepository.Trades
            .Where(x => x.Type == TradeType.Event)
            .Select(x => new AtgenUserEventTradeList(x.Id, x.Count))
            .ToListAsync();
    }

    public async Task<IEnumerable<UserAbilityCrestTradeList>> GetUserAbilityCrestTradeList()
    {
        return await this.tradeRepository.Trades
            .Where(x => x.Type == TradeType.AbilityCrest)
            .Select(x => new UserAbilityCrestTradeList(x.Id, x.Count))
            .ToListAsync();
    }

    public async Task<IEnumerable<UserTreasureTradeList>> GetUserTreasureTradeList()
    {
        return (await this.tradeRepository.GetTradesByTypeAsync(TradeType.Treasure)).Select(
            x => new UserTreasureTradeList(x.Id, x.Count, x.LastTradeTime)
        );
    }

    public async Task DoTreasureTrade(
        int tradeId,
        int count,
        IEnumerable<AtgenNeedUnitList>? needUnitList
    )
    {
        logger.LogDebug("Processing {tradeQuantity}x treasure trade {tradeId}", count, tradeId);

        TreasureTrade trade = MasterAsset.TreasureTrade[tradeId];

        foreach ((EntityTypes type, int id, int quantity, _) in trade.NeedEntities)
        {
            if (type == EntityTypes.None)
                continue;

            switch (type)
            {
                case EntityTypes.Material
                or EntityTypes.FafnirMedal:
                    DbPlayerMaterial? material = await this.inventoryRepository.GetMaterial(
                        (Materials)id
                    );
                    if (material == null || material.Quantity < quantity)
                    {
                        throw new DragaliaException(
                            ResultCode.CommonMaterialShort,
                            "Not enough material"
                        );
                    }

                    material.Quantity -= quantity;
                    break;
                case EntityTypes.Mana:
                    DbPlayerUserData? userData =
                        await this.userDataRepository.UserData.SingleAsync();
                    if (userData == null || userData.ManaPoint < quantity)
                    {
                        throw new DragaliaException(
                            ResultCode.CommonMaterialShort,
                            "Not enough mana"
                        );
                    }

                    userData.ManaPoint -= quantity;
                    break;
                case EntityTypes.DmodePoint:
                    throw new NotImplementedException("DmodePoint treasure trade");
                    break;
                default:
                    throw new DragaliaException(
                        ResultCode.CommonDataValidationError,
                        "Invalid EntityType in TreasureTrade"
                    );
            }
        }

        await this.rewardService.GrantReward(
            new(
                trade.DestinationEntityType,
                trade.DestinationEntityId,
                trade.DestinationEntityQuantity,
                trade.DestinationLimitBreakCount
            )
        );

        await this.tradeRepository.AddTrade(
            TradeType.Treasure,
            tradeId,
            count,
            DateTimeOffset.UtcNow
        );
    }

    public async Task DoAbilityCrestTrade(int id, int count)
    {
        logger.LogDebug("Processing {tradeQuantity}x ability crest trade {tradeId}", count, id);

        if (count != 1)
            throw new DragaliaException(ResultCode.CommonDataValidationError, "Invalid count");

        AbilityCrestTrade trade = MasterAsset.AbilityCrestTrade[id];

        await this.paymentService.ProcessPayment(
            PaymentTypes.DewPoint,
            expectedPrice: trade.NeedDewPoint
        );
        await this.rewardService.GrantReward(
            new Entity(EntityTypes.Wyrmprint, (int)trade.AbilityCrestId)
        );

        await this.tradeRepository.AddTrade(TradeType.AbilityCrest, id, count);
    }
}
