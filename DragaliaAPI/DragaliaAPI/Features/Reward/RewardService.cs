using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward.Handlers;
using DragaliaAPI.Features.Talisman;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Reward;

public class RewardService(
    ILogger<RewardService> logger,
    IUnitRepository unitRepository,
    IEnumerable<IRewardHandler> rewardHandlers
) : IRewardService
{
    private readonly List<Entity> discardedEntities = new();
    private readonly List<Entity> presentEntites = new();
    private readonly List<Entity> presentLimitEntities = new();
    private readonly List<Entity> newEntities = new();
    private readonly List<ConvertedEntity> convertedEntities = new();

    public async Task<RewardGrantResult> GrantReward(Entity entity)
    {
        if (entity.Quantity <= 0)
        {
            // NOTE: Should this be an invalid case?
            return RewardGrantResult.Added;
        }

        logger.LogTrace("Granting reward {@rewardEntity}", entity);

        RewardGrantResult result = await GrantRewardInternal(entity);

        logger.LogTrace("Result: {result}", result);

        return result;
    }

    public async Task GrantRewards(IEnumerable<Entity> entities)
    {
        entities = entities.ToList();
        logger.LogTrace("Granting rewards: {@rewards}", entities);

        if (!entities.TryGetNonEnumeratedCount(out int count))
            count = 0;

        List<RewardGrantResult> results = new(count);
        foreach (Entity entity in entities)
        {
            RewardGrantResult result = await GrantRewardInternal(entity);
            results.Add(result);
        }

        logger.LogTrace("Results: {@results}", results);
    }

    private async Task<RewardGrantResult> GrantRewardInternal(Entity entity)
    {
        IRewardHandler handler = this.GetHandler(entity.Type);
        GrantReturn grantReturn = await handler.Grant(entity);

        switch (grantReturn.Result)
        {
            case RewardGrantResult.Added:
                this.newEntities.Add(entity);
                break;
            case RewardGrantResult.Converted:
                ArgumentNullException.ThrowIfNull(grantReturn.ConvertedEntity);

                this.convertedEntities.Add(
                    new ConvertedEntity(entity, grantReturn.ConvertedEntity)
                );
                await this.GetHandler(grantReturn.ConvertedEntity.Type)
                    .Grant(grantReturn.ConvertedEntity);

                break;
            case RewardGrantResult.Discarded:
                this.discardedEntities.Add(entity);
                break;
            case RewardGrantResult.GiftBoxDiscarded:
                this.presentLimitEntities.Add(entity);
                break;
            case RewardGrantResult.GiftBox:
                this.presentEntites.Add(entity);
                break;
            case RewardGrantResult.Limit:
                break;
            case RewardGrantResult.FailError:
                logger.LogError("Granting of entity {@entity} failed.", entity);
                throw new InvalidOperationException("Failed to grant reward");
            default:
                throw new ArgumentOutOfRangeException(
                    string.Empty,
                    "RewardGrantResult out of range"
                );
        }

        return grantReturn.Result;
    }

    private IRewardHandler GetHandler(EntityTypes type)
    {
        IRewardHandler? handler = rewardHandlers.SingleOrDefault(x =>
            x.SupportedTypes.Contains(type)
        );

        if (handler is null)
        {
            logger.LogError("Failed to find reward handler for entity type {type}", type);
            throw new InvalidOperationException("Failed to grant reward");
        }

        return handler;
    }

    public async Task<(RewardGrantResult Result, DbTalisman? Talisman)> GrantTalisman(
        Talismans id,
        int abilityId1,
        int abilityId2,
        int abilityId3,
        int hp,
        int atk
    )
    {
        // int currentCount = await unitRepository.Talismans.CountAsync();

        if (
            false /*TODO: currentCount >= TalismanService.TalismanMaxCount once we get presents working with it*/
        )
        {
            Entity coinReward = new(EntityTypes.Rupies, 0, TalismanService.TalismanCoinReward);
            await GrantReward(coinReward);

            convertedEntities.Add(
                new ConvertedEntity(new Entity(EntityTypes.Talisman, (int)id), coinReward)
            );

            return (RewardGrantResult.Converted, null);
        }

        DbTalisman talisman = unitRepository.AddTalisman(
            id,
            abilityId1,
            abilityId2,
            abilityId3,
            hp,
            atk
        );

        return (RewardGrantResult.Added, talisman);
    }

    public EntityResult GetEntityResult()
    {
        return new()
        {
            NewGetEntityList = newEntities.Select(x => x.ToDuplicateEntityList()),
            ConvertedEntityList = convertedEntities.Select(x => x.ToConvertedEntityList()),
            OverDiscardEntityList = discardedEntities.Select(x => x.ToBuildEventRewardEntityList()),
            OverPresentEntityList = this.presentEntites.Select(x =>
                x.ToBuildEventRewardEntityList()
            ),
            OverPresentLimitEntityList = this.presentLimitEntities.Select(x =>
                x.ToBuildEventRewardEntityList()
            ),
        };
    }

    public IEnumerable<ConvertedEntity> GetConvertedEntityList() => this.convertedEntities.ToList();
}
