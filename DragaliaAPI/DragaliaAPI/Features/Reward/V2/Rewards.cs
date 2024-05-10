using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Reward.V2;

public interface IReward;

public readonly record struct DragonReward(Dragons Id, int Level, int LimitBreakCount) : IReward;
