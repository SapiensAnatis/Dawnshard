using DragaliaAPI.Shared.Definitions.Enums;
using JetBrains.Annotations;

namespace DragaliaAPI.Features.Shared.Reward.Handlers;

[UsedImplicitly]
public partial class FafnirMedalRewardHandler(ILogger<FafnirMedalRewardHandler> logger) : IRewardHandler
{
    private readonly ILogger<FafnirMedalRewardHandler> logger = logger;

    public IReadOnlyList<EntityTypes> SupportedTypes { get; } = [EntityTypes.FafnirMedal];

    public Task<GrantReturn> Grant(Entity entity)
    {
        Log.DiscardingFafnirMedalEntity(this.logger, entity);
        return Task.FromResult(new GrantReturn(RewardGrantResult.Discarded));
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Discarding fafnir medal entity: {@entity}")]
        public static partial void DiscardingFafnirMedalEntity(ILogger logger, Entity entity);
    }
}
