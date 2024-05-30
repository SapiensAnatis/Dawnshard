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
    IEnumerable<IRewardHandler> rewardHandlers,
    IEnumerable<IBatchRewardHandler> batchRewardHandlers
) : IRewardService
{
    private readonly List<Entity> newEntities = [];
    private readonly List<ConvertedEntity> convertedEntities = [];
    private readonly List<Entity> discardedEntities = [];

    public async Task<RewardGrantResult> GrantReward(Entity entity)
    {
        if (entity.Quantity <= 0)
        {
            // NOTE: Should this be an invalid case?
            return RewardGrantResult.Added;
        }

        logger.LogTrace("Granting reward {@rewardEntity}", entity);

        IRewardHandler handler = this.GetHandler(entity.Type);

        GrantReturn grantReturn = await handler.Grant(entity);
        await ProcessGrantResult(grantReturn, entity);

        logger.LogTrace("Result: {result}", grantReturn.Result);

        return grantReturn.Result;
    }

    public async Task GrantRewards(IEnumerable<Entity> entities)
    {
        foreach (Entity entity in entities)
        {
            IRewardHandler handler = this.GetHandler(entity.Type);

            GrantReturn grantReturn = await handler.Grant(entity);
            await ProcessGrantResult(grantReturn, entity);
        }
    }

    public async Task<IDictionary<TKey, RewardGrantResult>> BatchGrantRewards<TKey>(
        IDictionary<TKey, Entity> entities
    )
        where TKey : struct
    {
        Dictionary<TKey, RewardGrantResult> result = [];

        IEnumerable<(EntityTypes type, Dictionary<TKey, Entity>)> grouping = entities.GroupBy(
            x => x.Value.Type,
            (type, group) => (type, group.ToDictionary())
        );

        foreach ((EntityTypes type, Dictionary<TKey, Entity> dictionary) in grouping)
        {
            if (
                batchRewardHandlers.FirstOrDefault(x => x.SupportedTypes.Contains(type)) is
                { } batchRewardHandler
            )
            {
                IDictionary<TKey, GrantReturn> batchResult = await batchRewardHandler.GrantRange(
                    dictionary
                );

                foreach ((TKey key, GrantReturn grantReturn) in batchResult)
                {
                    await ProcessGrantResult(grantReturn, dictionary[key]);
                    result.Add(key, grantReturn.Result);
                }
            }
            else
            {
                IRewardHandler handler = this.GetHandler(type);

                foreach ((TKey key, Entity entity) in dictionary)
                {
                    GrantReturn grantReturn = await handler.Grant(entity);
                    await ProcessGrantResult(grantReturn, entity);
                    result.Add(key, grantReturn.Result);
                }
            }
        }

        return result;
    }

    private async Task ProcessGrantResult(GrantReturn grantReturn, Entity entity)
    {
        switch (grantReturn.Result)
        {
            case RewardGrantResult.Added:
                this.newEntities.Add(entity);
                break;
            case RewardGrantResult.Converted:
            {
                if (grantReturn.ConvertedEntity is null)
                {
                    throw new InvalidOperationException(
                        "RewardGrantResult.Converted was returned, but converted entity was null!"
                    );
                }

                this.convertedEntities.Add(
                    new ConvertedEntity(entity, grantReturn.ConvertedEntity)
                );
                await this.GetHandler(grantReturn.ConvertedEntity.Type)
                    .Grant(grantReturn.ConvertedEntity);

                break;
            }
            case RewardGrantResult.Discarded:
                this.discardedEntities.Add(entity);
                break;
            case RewardGrantResult.Limit:
                break;
            case RewardGrantResult.FailError:
                logger.LogError("Granting of entity {@entity} failed.", entity);
                throw new InvalidOperationException("Failed to grant reward");
            default:
                throw new InvalidOperationException(
                    $"RewardGrantResult {grantReturn.Result} out of range"
                );
        }
    }

    private IRewardHandler GetHandler(EntityTypes type)
    {
        IRewardHandler? handler = rewardHandlers.FirstOrDefault(x =>
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
            OverDiscardEntityList = this.discardedEntities.Select(x =>
                x.ToBuildEventRewardEntityList()
            )
        };
    }

    public IEnumerable<ConvertedEntity> GetConvertedEntityList() => this.convertedEntities.ToList();
}
