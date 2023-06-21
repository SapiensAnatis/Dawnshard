﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shop;

public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> logger;
    private readonly IUserDataRepository userDataRepository;

    public PaymentService(ILogger<PaymentService> logger, IUserDataRepository userDataRepository)
    {
        this.logger = logger;
        this.userDataRepository = userDataRepository;
    }

    public async Task ProcessPayment(
        PaymentTypes type,
        PaymentTarget? payment = null,
        int? expectedPrice = null
    )
    {
        if (payment == null && expectedPrice == null)
            throw new ArgumentNullException(nameof(payment), "No price provided");

        bool hasPaymentTarget = payment is not null;

        if (expectedPrice == 0) // For free stuff, if needed.
            return;

        if (hasPaymentTarget)
            logger.LogDebug("Processing {paymentType} payment {@payment}.", type, payment);
        else
            logger.LogDebug(
                "Processing {paymentType} payment {@payment}.",
                type,
                new { Price = expectedPrice }
            );

        if (hasPaymentTarget && expectedPrice is not null && expectedPrice != payment.target_cost)
            throw new DragaliaException(ResultCode.CommonUserStatusError, "Price mismatch.");

        int price = expectedPrice ?? payment!.target_cost;
        long quantity;
        Action updater;

        switch (type)
        {
            case PaymentTypes.Wyrmite:
                DbPlayerUserData userData = await this.userDataRepository.UserData.SingleAsync();
                quantity = userData.Crystal;
                updater = () => userData.Crystal -= price;
                break;
            case PaymentTypes.HalidomHustleHammer:
                userData = await this.userDataRepository.UserData.SingleAsync();
                quantity = userData.BuildTimePoint;
                updater = () => userData.BuildTimePoint -= price;
                break;
            case PaymentTypes.ManaPoint:
                userData = await this.userDataRepository.UserData.SingleAsync();
                quantity = userData.ManaPoint;
                updater = () => userData.ManaPoint -= price;
                break;
            case PaymentTypes.Coin:
                userData = await this.userDataRepository.UserData.SingleAsync();
                quantity = userData.Coin;
                updater = () => userData.Coin -= price;
                break;
            case PaymentTypes.Ticket:
                // TODO: Implement ticket payments.
                logger.LogWarning(
                    "Tried to pay with summon tickets - this is not (yet) supported!"
                );
                throw new DragaliaException(
                    ResultCode.ShopPaymentTypeInvalid,
                    "Tickets are not yet supported."
                );
                break;
            case PaymentTypes.Diamantium:
                logger.LogDebug("Tried to pay with diamantium -- this is not supported.");
                throw new DragaliaException(
                    ResultCode.ShopPaymentTypeInvalid,
                    "Diamantium is not supported."
                );
            default:
                logger.LogWarning("Unknown/invalid payment type.");
                throw new DragaliaException(
                    ResultCode.ShopPaymentTypeInvalid,
                    "Invalid payment type."
                );
        }

        if (hasPaymentTarget && quantity != payment!.target_hold_quantity)
        {
            this.logger.LogError(
                "Held quantity {quantity} does not match target of payment {@payment}",
                quantity,
                payment
            );

            throw new DragaliaException(
                ResultCode.CommonUserStatusError,
                "Payment count mismatch."
            );
        }

        if (quantity < price)
        {
            this.logger.LogError(
                "Held quantity {quantity} does not meet price {price}",
                quantity,
                price
            );

            throw new DragaliaException(
                ResultCode.CommonMaterialShort,
                "Insufficient quantity for payment."
            );
        }

        updater();
    }
}
