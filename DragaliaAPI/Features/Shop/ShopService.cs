using DragaliaAPI.Features.Reward;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.Shop;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shop;

public class ShopService : IShopService
{
    private readonly IShopRepository shopRepository;
    private readonly IPaymentService paymentService;
    private readonly IRewardService rewardService;
    private readonly ILogger<ShopService> logger;
    private readonly IResetHelper resetHelper;

    public ShopService(
        IShopRepository shopRepository,
        IPaymentService paymentService,
        IRewardService rewardService,
        ILogger<ShopService> logger,
        IResetHelper resetHelper
    )
    {
        this.shopRepository = shopRepository;
        this.paymentService = paymentService;
        this.rewardService = rewardService;
        this.logger = logger;
        this.resetHelper = resetHelper;
    }

    public async Task<IEnumerable<ShopPurchaseList>> DoPurchase(
        ShopType shopType,
        PaymentTypes paymentType,
        int goodsId,
        int goodsQuantity
    )
    {
        logger.LogDebug(
            "Processing purchase {@purchase}",
            new
            {
                Type = shopType,
                Payment = paymentType,
                Id = goodsId,
                Quantity = goodsQuantity
            }
        );

        IShop shop = Shop.From(shopType, goodsId);

        if (shop.PaymentType != PaymentTypes.DiamantiumOrWyrmite && shop.PaymentType != paymentType)
            throw new DragaliaException(ResultCode.ShopPaymentTypeInvalid, "Payment type mismatch");

        if (
            shop.PaymentType == PaymentTypes.DiamantiumOrWyrmite
            && paymentType is not (PaymentTypes.Wyrmite or PaymentTypes.Diamantium)
        )
        {
            throw new DragaliaException(
                ResultCode.ShopPaymentTypeInvalid,
                "Invalid payment type, needs to be either Wyrmite or Diamantium"
            );
        }

        int price = shop.NeedCost * goodsQuantity;
        if (price < 0)
            throw new DragaliaException(ResultCode.CommonDataValidationError, "Price overflow");

        await this.paymentService.ProcessPayment(paymentType, expectedPrice: price);

        foreach (
            (
                EntityTypes type,
                int id,
                int quantity,
                int limitBreakCount
            ) in shop.DestinationEntities
        )
        {
            if (type != EntityTypes.None)
            {
                await this.rewardService.GrantReward(
                    new Entity(type, id, quantity * goodsQuantity, limitBreakCount)
                );
            }
        }

        // Now shop specific behavior
        if (shopType is ShopType.Normal or ShopType.Special)
        {
            throw new NotImplementedException("Normal or Special shop");

            NormalShop normalShop = (NormalShop)shop;
            if (normalShop.BonusGoodsType != 0)
            {
                logger.LogWarning(
                    "Tried to purchase shop item with bonus goods -- these are not yet supported."
                );
                throw new NotImplementedException("BonusGoodsType != 0 (NormalShop)");
            }

            if (normalShop.AddMaxDragonQuantity != 0) { }
            // TODO

            if (normalShop.StaminaSingleCount != 0) { }
            // TODO

            if (normalShop.StaminaMultiCount != 0) { }
            // TODO
        }

        DateTimeOffset purchaseTime = DateTimeOffset.UtcNow;

        (DateTimeOffset startTime, DateTimeOffset endTime) = GetEffectTimes(shopType);

        bool newlyAdded = await this.shopRepository.AddShopPurchase(
            shopType,
            goodsId,
            goodsQuantity,
            purchaseTime,
            startTime,
            endTime
        );

        PurchaseShopType purchaseType = shopType.ToPurchaseShopType();

        List<ShopPurchaseList> currentPurchases = (
            await this.shopRepository.Purchases.Where(x => x.ShopType == purchaseType).ToListAsync()
        )
            .Select(
                x =>
                    new ShopPurchaseList(
                        x.GoodsId,
                        x.LastBuyTime,
                        x.EffectStartTime,
                        x.EffectEndTime,
                        x.BuyCount
                    )
            )
            .ToList();

        if (newlyAdded) // Change tracker weirdness
        {
            currentPurchases.Add(
                new ShopPurchaseList(goodsId, purchaseTime, startTime, endTime, goodsQuantity)
            );
        }

        return currentPurchases;
    }

    public async Task<ILookup<PurchaseShopType, ShopPurchaseList>> GetPurchases()
    {
        await this.shopRepository.ClearExpiredShopPurchases();

        return (await this.shopRepository.Purchases.ToListAsync()).ToLookup(
            x => x.ShopType,
            x =>
                new ShopPurchaseList(
                    x.GoodsId,
                    x.LastBuyTime,
                    x.EffectStartTime,
                    x.EffectEndTime,
                    x.BuyCount
                )
        );
    }

    private (DateTimeOffset Start, DateTimeOffset End) GetEffectTimes(ShopType type)
    {
        return type switch
        {
            ShopType.MaterialDaily
                => (
                    this.resetHelper.LastDailyReset,
                    this.resetHelper.LastDailyReset.AddDays(1).AddSeconds(-1)
                ),
            ShopType.MaterialWeekly
                => (
                    this.resetHelper.LastWeeklyReset,
                    this.resetHelper.LastWeeklyReset.AddDays(7).AddSeconds(-1)
                ),
            ShopType.MaterialMonthly
                => (
                    this.resetHelper.LastMonthlyReset,
                    this.resetHelper.LastMonthlyReset.AddMonths(1).AddSeconds(-1)
                ),
            ShopType.Normal
            or ShopType.Special
                => (DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch),
            _ => throw new DragaliaException(ResultCode.CommonInvalidArgument, "Invalid ShopType")
        };
    }
}
