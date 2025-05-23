﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.Shop;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shop;

public class ShopService(
    IShopRepository shopRepository,
    IPaymentService paymentService,
    IRewardService rewardService,
    ILogger<ShopService> logger,
    TimeProvider timeProvider,
    IUserService userService,
    IUserDataRepository userDataRepository
) : IShopService
{
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
                Quantity = goodsQuantity,
            }
        );

        IShop shop = Shop.From(shopType, goodsId);

        PaymentTypes shopPaymentType = shop.PaymentType;
        int price = shop.NeedCost * goodsQuantity;

        if (price < 0)
            throw new DragaliaException(ResultCode.CommonDataValidationError, "Price overflow");

        if (shopPaymentType == PaymentTypes.Other)
        {
            // In theory, we should now look up an entry in PriceChangeData using _OptionValue and determine what
            // 'step' we are at and adjust the cost appropriately. However, all rows in the table have _PaymentType = 4
            // and _Cost = 30, so we can just statically take those values.
            shopPaymentType = PaymentTypes.DiamantiumOrWyrmite;
            price = 30;
        }

        if (shopPaymentType != PaymentTypes.DiamantiumOrWyrmite && shopPaymentType != paymentType)
        {
            throw new DragaliaException(ResultCode.ShopPaymentTypeInvalid, "Payment type mismatch");
        }

        if (
            shopPaymentType == PaymentTypes.DiamantiumOrWyrmite
            && paymentType is not (PaymentTypes.Wyrmite or PaymentTypes.Diamantium)
        )
        {
            throw new DragaliaException(
                ResultCode.ShopPaymentTypeInvalid,
                "Invalid payment type, needs to be either Wyrmite or Diamantium"
            );
        }

        await paymentService.ProcessPayment(paymentType, expectedPrice: price);

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
                await rewardService.GrantReward(
                    new Entity(type, id, quantity * goodsQuantity, limitBreakCount)
                );
            }
        }

        // Now shop specific behavior
        if (shopType is ShopType.Special)
            throw new NotImplementedException("We dont take Diamantium nor real money");

        if (shopType is ShopType.Normal)
        {
            NormalShop normalShop = (NormalShop)shop;

            if (normalShop.AddMaxDragonQuantity != 0)
            {
                DbPlayerUserData userData = await userDataRepository.GetUserDataAsync();
                userData.MaxDragonQuantity += normalShop.AddMaxDragonQuantity;
            }

            if (normalShop.StaminaSingleCount != 0)
            {
                await userService.AddStamina(StaminaType.Single, normalShop.StaminaSingleCount);
            }

            if (normalShop.StaminaMultiCount != 0)
            {
                await userService.AddStamina(StaminaType.Multi, normalShop.StaminaMultiCount);
            }
        }

        DateTimeOffset purchaseTime = DateTimeOffset.UtcNow;

        (DateTimeOffset startTime, DateTimeOffset endTime) = GetEffectTimes(shopType);

        bool newlyAdded = await shopRepository.AddShopPurchase(
            shopType,
            goodsId,
            goodsQuantity,
            purchaseTime,
            startTime,
            endTime
        );

        PurchaseShopType purchaseType = shopType.ToPurchaseShopType();

        List<ShopPurchaseList> currentPurchases = (
            await shopRepository.Purchases.Where(x => x.ShopType == purchaseType).ToListAsync()
        )
            .Select(x => new ShopPurchaseList(
                x.GoodsId,
                x.LastBuyTime,
                x.EffectStartTime,
                x.EffectEndTime,
                x.BuyCount
            ))
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
        await shopRepository.ClearExpiredShopPurchases();

        return (await shopRepository.Purchases.ToListAsync()).ToLookup(
            x => x.ShopType,
            x => new ShopPurchaseList(
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
            ShopType.MaterialDaily => (
                timeProvider.GetLastDailyReset(),
                timeProvider.GetLastDailyReset().AddDays(1).AddSeconds(-1)
            ),
            ShopType.MaterialWeekly => (
                timeProvider.GetLastWeeklyReset(),
                timeProvider.GetLastWeeklyReset().AddDays(7).AddSeconds(-1)
            ),
            ShopType.MaterialMonthly => (
                timeProvider.GetLastMonthlyReset(),
                timeProvider.GetLastMonthlyReset().AddMonths(1).AddSeconds(-1)
            ),
            ShopType.Normal or ShopType.Special => (
                DateTimeOffset.UnixEpoch,
                DateTimeOffset.UnixEpoch
            ),
            _ => throw new DragaliaException(ResultCode.CommonInvalidArgument, "Invalid ShopType"),
        };
    }
}
