using DragaliaAPI.Models.Data;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Requests;

//TODO: HasUnknown
/// <summary>
/// A summoning request
/// </summary>
/// <param name="summon_id">Id of the summon banner</param>
/// <param name="exec_type">Unknown<br/>Maybe distinguishing 1x from 10x</param>
/// <param name="exec_count">Unknown<br/>Maybe for multiple single tickets</param>
/// <param name="payment_type">Probably type of currency used</param>
/// <param name="payment_target"><b>See: <see cref="PaymentTarget"/></b></param>
[MessagePackObject(true)]
public record SummonRequest(
    int summon_id,
    SummonExecTypes exec_type,
    int exec_count,
    PaymentTypes payment_type,
    PaymentTarget payment_target
);

/// <summary>
/// Contains the cost of the summon and the amount held of the relevant currency
/// Maybe required for Diamantium purchases
/// </summary>
/// <param name="target_hold_quantity">Total relevant currency held</param>
/// <param name="target_cost">Relevant currency cost</param>
[MessagePackObject(true)]
public record PaymentTarget(int target_hold_quantity, int target_cost);
