using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Shop;

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
