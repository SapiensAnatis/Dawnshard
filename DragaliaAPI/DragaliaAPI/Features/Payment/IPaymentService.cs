using DragaliaAPI.DTO;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Features.Payment;

public interface IPaymentService
{
    Task ProcessPayment(Entity entity, PaymentTarget? payment = null);

    Task ProcessPayment(
        PaymentTypes type,
        PaymentTarget? payment = null,
        int? expectedPrice = null
    );

    Task ProcessPayment(Materials id, int quantity);

    DeleteDataList GetDeleteDataList();
}
