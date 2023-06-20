using DragaliaAPI.Database.Entities;
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
        int quantity;
        Action updater;

        switch (type)
        {
            case PaymentTypes.Wyrmite:
                DbPlayerUserData userData = await this.userDataRepository.UserData.SingleAsync();
                quantity = userData.Crystal;
                updater = () => userData.Crystal -= price;
                break;
            case PaymentTypes.Diamantium:
                logger.LogDebug("Tried to pay with diamantium -- this is not supported.");
                throw new DragaliaException(
                    ResultCode.ShopPaymentTypeInvalid,
                    "Diamantium is not supported."
                );
            case PaymentTypes.HalidomHustleHammer:
                userData = await this.userDataRepository.UserData.SingleAsync();
                quantity = userData.BuildTimePoint;
                updater = () => userData.BuildTimePoint -= price;
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
            default:
                logger.LogWarning("Unknown/invalid payment type.");
                throw new DragaliaException(
                    ResultCode.ShopPaymentTypeInvalid,
                    "Invalid payment type."
                );
        }

        if ((hasPaymentTarget && quantity != payment!.target_hold_quantity) || quantity < price)
        {
            throw new DragaliaException(
                ResultCode.CommonUserStatusError,
                "Payment count mismatch."
            );
        }

        updater();
    }
}
