using DragaliaAPI.Features.Reward;

namespace DragaliaAPI.Features.Event.Summon;

/// <summary>
/// Represents an item generator for a blazon summon item such as "Void Battle Item" which is determined at random.
/// </summary>
internal abstract class TwoStepItemGenerator
{
    /// <summary>
    /// The <see cref="EventSummonItemConfiguration.TwoStepId"/> that this generator is compatible with.
    /// </summary>
    public abstract int Id { get; }

    protected abstract IReadOnlyList<Entity> EntityPool { get; }

    /// <summary>
    /// Generates a random entity.
    /// </summary>
    public Entity GenerateRandomEntity() => Random.Shared.Next(EntityPool);
}
