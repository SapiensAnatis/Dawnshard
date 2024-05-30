namespace DragaliaAPI.Features.Reward;

/// <summary>
/// Return type indicating the status of a reward being granted.
/// </summary>
public enum RewardGrantResult
{
    /// <summary>
    /// An empty reward result.
    /// </summary>
    /// <remarks>
    /// This value should never be returned from a <see cref="Handlers.IRewardHandler"/>.
    /// </remarks>
    None,

    /// <summary>
    /// The reward was successfully added.
    /// </summary>
    Added,

    /// <summary>
    /// The reward was not added, but was converted to another reward which was added.
    /// </summary>
    Converted,

    /// <summary>
    /// The reward was not added because the player did not have space to claim it.
    /// It may be able to be claimed later if the player makes space in their inventory.
    /// </summary>
    Limit,

    /// <summary>
    /// The reward was not added and will never be able to be claimed at a later date.
    /// </summary>
    Discarded,

    /// <summary>
    /// The reward was not added because of an error.
    /// </summary>
    FailError,
}
