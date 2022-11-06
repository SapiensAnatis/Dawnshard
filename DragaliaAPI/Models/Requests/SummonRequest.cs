using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Models.Requests;

//TODO: HasUnknown
/// <summary>
/// A summoning request
/// </summary>
/// <param name="summon_id">Id of the summon banner</param>
/// <param name="exec_type">Distinguishing single(1) from tenfold(2) summons, maybe more</param>
/// <param name="exec_count">Seemingly only passed for multiple single summons, 0 for tenfold</param>
/// <param name="payment_type">Type of currency used</param>
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
/// Mostly likely only relevant for Diamantium
/// </summary>
/// <param name="target_hold_quantity">Total relevant currency held</param>
/// <param name="target_cost">Relevant currency cost</param>
[MessagePackObject(true)]
public record PaymentTarget(int target_hold_quantity, int target_cost);
