using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Shared.Reward.Handlers;
using DragaliaAPI.Features.Talisman;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Shared.Reward;

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
        await this.ProcessGrantResult(grantReturn, entity);

        logger.LogTrace("Result: {result}", grantReturn.Result);

        return grantReturn.Result;
    }

    public async Task GrantRewards(IEnumerable<Entity> entities)
    {
        foreach (Entity entity in entities)
        {
            IRewardHandler handler = this.GetHandler(entity.Type);

            GrantReturn grantReturn = await handler.Grant(entity);
            await this.ProcessGrantResult(grantReturn, entity);
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

                Debug.Assert(
                    batchResult.Count == dictionary.Count,
                    "Batch reward handler returned incorrect number of results"
                );

                foreach ((TKey key, GrantReturn grantReturn) in batchResult)
                {
                    await this.ProcessGrantResult(grantReturn, dictionary[key]);
                    result.Add(key, grantReturn.Result);
                }
            }
            else
            {
                IRewardHandler handler = this.GetHandler(type);

                foreach ((TKey key, Entity entity) in dictionary)
                {
                    GrantReturn grantReturn = await handler.Grant(entity);
                    await this.ProcessGrantResult(grantReturn, entity);
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
            {
                if (entity.Type != EntityTypes.Material)
                {
                    // Material exception stops every material showing as 'New!' in quest drops.
                    // I think new_get_entity_list is intended for entities that have never been seen before.
                    // TODO: We probably want a different RewardGrantResult so that materials that were unowned can show this.
                    this.newEntities.Add(entity);
                }

                break;
            }
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
            {
                this.discardedEntities.Add(entity);
                break;
            }
            case RewardGrantResult.Limit:
            {
                break;
            }
            case RewardGrantResult.FailError:
            {
                logger.LogError("Granting of entity {@entity} failed.", entity);
                throw new InvalidOperationException("Failed to grant reward");
            }
            default:
            {
                throw new InvalidOperationException(
                    $"RewardGrantResult {grantReturn.Result} out of range"
                );
            }
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

    public Task<(RewardGrantResult Result, DbTalisman? Talisman)> GrantTalisman(
        Talismans id,
        int abilityId1,
        int abilityId2,
        int abilityId3,
        int hp,
        int atk
    )
    {
        /*
        // TODO: currentCount >= TalismanService.TalismanMaxCount once we get presents working with it
        int currentCount = await unitRepository.Talismans.CountAsync();
        
        if (
            false
        )
        {
            Entity coinReward = new(EntityTypes.Rupies, 0, TalismanService.TalismanCoinReward);
            await this.GrantReward(coinReward);

            this.convertedEntities.Add(
                new ConvertedEntity(new Entity(EntityTypes.Talisman, (int)id), coinReward)
            );

            return (RewardGrantResult.Converted, null);
        }
        */

        DbTalisman talisman = unitRepository.AddTalisman(
            id,
            abilityId1,
            abilityId2,
            abilityId3,
            hp,
            atk
        );

        return Task.FromResult<(RewardGrantResult Result, DbTalisman? Talisman)>(
            (RewardGrantResult.Added, talisman)
        );
    }

    public EntityResult GetEntityResult()
    {
        return new()
        {
            NewGetEntityList = this.newEntities.Select(x => x.ToDuplicateEntityList()),
            ConvertedEntityList = this.convertedEntities.Select(x => x.ToConvertedEntityList()),
            OverDiscardEntityList = this.discardedEntities.Select(x =>
                x.ToBuildEventRewardEntityList()
            ),
        };
    }

    public IEnumerable<ConvertedEntity> GetConvertedEntityList() => this.convertedEntities.ToList();
}
