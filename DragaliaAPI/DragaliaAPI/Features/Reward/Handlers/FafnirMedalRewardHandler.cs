using DragaliaAPI.Shared.Definitions.Enums;
using JetBrains.Annotations;

namespace DragaliaAPI.Features.Reward.Handlers;

[UsedImplicitly]
public class FafnirMedalRewardHandler(ILogger<FafnirMedalRewardHandler> logger) : IRewardHandler
{
    private readonly ILogger<FafnirMedalRewardHandler> logger = logger;

    public IReadOnlyList<EntityTypes> SupportedTypes { get; } = [EntityTypes.FafnirMedal];

    public Task<GrantReturn> Grant(Entity entity)
    {
        this.logger.LogInformation("Discarding fafnir medal entity: {@entity}", entity);
        return Task.FromResult(new GrantReturn(RewardGrantResult.Discarded));
    }
}
