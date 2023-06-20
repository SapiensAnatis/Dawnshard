using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Shop;

public interface IPaymentService
{
    Task ProcessPayment(PaymentTypes type, PaymentTarget payment, int? expectedPrice = null);
}
