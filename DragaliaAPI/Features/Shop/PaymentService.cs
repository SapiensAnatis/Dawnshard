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
        PaymentTarget payment,
        int? expectedPrice = null
    )
    {
        if (expectedPrice == 0) // For free stuff, if needed.
            return;

        logger.LogDebug("Processing {paymentType} payment {@payment}.", type, payment);

        if (expectedPrice is not null && expectedPrice != payment.target_cost)
            throw new DragaliaException(ResultCode.CommonUserStatusError, "Price mismatch.");

        switch (type)
        {
            case PaymentTypes.Wyrmite:
                DbPlayerUserData userData = await this.userDataRepository.UserData.SingleAsync();
                if (userData.Crystal != payment.target_hold_quantity)
                    throw new DragaliaException(
                        ResultCode.CommonUserStatusError,
                        "Payment count mismatch."
                    );
                userData.Crystal -= payment.target_cost;
                break;
            case PaymentTypes.Diamantium:
                logger.LogDebug("Tried to pay with diamantium -- this is not supported.");
                throw new DragaliaException(
                    ResultCode.ShopPaymentTypeInvalid,
                    "Diamantium is not supported."
                );
            case PaymentTypes.HalidomHustleHammer:
                userData = await this.userDataRepository.UserData.SingleAsync();
                if (userData.BuildTimePoint != payment.target_hold_quantity)
                    throw new DragaliaException(
                        ResultCode.CommonUserStatusError,
                        "Payment count mismatch."
                    );
                userData.BuildTimePoint -= payment.target_cost;
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
    }
}
