using System.Diagnostics;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Reward;

public class RewardService(
    ILogger<RewardService> logger,
    IInventoryRepository inventoryRepository,
    IUserDataRepository userDataRepository,
    IAbilityCrestRepository abilityCrestRepository,
    IUnitRepository unitRepository,
    IFortRepository fortRepository,
    IEventRepository eventRepository
) : IRewardService
{
    private readonly List<Entity> discardedEntities = new();
    private readonly List<Entity> presentEntites = new();
    private readonly List<Entity> presentLimitEntities = new();
    private readonly List<Entity> newEntities = new();
    private readonly List<ConvertedEntity> convertedEntities = new();

    public async Task<RewardGrantResult> GrantReward(Entity entity)
    {
        Debug.Assert(entity.Quantity > 0, "entity.Quantity > 0");

        logger.LogDebug("Granting reward {@rewardEntity}", entity);

        switch (entity.Type)
        {
            case EntityTypes.Chara:
                return await RewardCharacter(entity);
            case EntityTypes.Dragon:
                for (int i = 0; i < entity.Quantity; i++)
                    await unitRepository.AddDragons((Dragons)entity.Id);
                break;
            case EntityTypes.Dew:
                await userDataRepository.UpdateDewpoint(entity.Quantity);
                break;
            case EntityTypes.HustleHammer:
                (await userDataRepository.UserData.SingleAsync()).BuildTimePoint += entity.Quantity;
                break;
            case EntityTypes.Rupies:
                await userDataRepository.UpdateCoin(entity.Quantity);
                break;
            case EntityTypes.Wyrmite:
                await userDataRepository.GiveWyrmite(entity.Quantity);
                break;
            case EntityTypes.Wyrmprint:
                return await RewardAbilityCrest(entity);
            case EntityTypes.Material:
                (
                    await inventoryRepository.GetMaterial((Materials)entity.Id)
                    ?? inventoryRepository.AddMaterial((Materials)entity.Id)
                ).Quantity += entity.Quantity;
                break;
            case EntityTypes.Mana:
                (await userDataRepository.UserData.SingleAsync()).ManaPoint += entity.Quantity;
                break;
            case EntityTypes.FortPlant:
                await fortRepository.AddToStorage((FortPlants)entity.Id, quantity: entity.Quantity);
                break;
            case EntityTypes.BuildEventItem:
            case EntityTypes.Clb01EventItem:
            case EntityTypes.CollectEventItem:
            case EntityTypes.RaidEventItem:
            case EntityTypes.MazeEventItem:
            case EntityTypes.ExRushEventItem:
            case EntityTypes.SimpleEventItem:
            case EntityTypes.ExHunterEventItem:
            case EntityTypes.BattleRoyalEventItem:
            case EntityTypes.EarnEventItem:
            case EntityTypes.CombatEventItem:
                await eventRepository.AddItemQuantityAsync(entity.Id, entity.Quantity);
                break;
            default:
                logger.LogWarning("Tried to reward unsupported entity {@entity}", entity);
                return RewardGrantResult.FailError;
        }

        newEntities.Add(entity);
        return RewardGrantResult.Added;
    }

    private async Task<RewardGrantResult> RewardCharacter(Entity entity)
    {
        if (entity.Type != EntityTypes.Chara)
            throw new ArgumentException("Entity was not a character", nameof(entity));

        Charas chara = (Charas)entity.Id;

        if (await unitRepository.FindCharaAsync(chara) is not null)
        {
            // Is it the correct behaviour to discard gifted characters?
            // Not sure -- never had characters in my gift box
            logger.LogDebug("Discarded character entity: {@entity}.", entity);
            discardedEntities.Add(entity);
            return RewardGrantResult.Discarded;
        }

        // TODO: Support EntityLevel/LimitBreak/etc here

        logger.LogDebug("Granted new character entity: {@entity}", entity);
        await unitRepository.AddCharas(chara);
        newEntities.Add(entity);
        return RewardGrantResult.Added;
    }

    private async Task<RewardGrantResult> RewardAbilityCrest(Entity entity)
    {
        if (entity.Type != EntityTypes.Wyrmprint)
            throw new ArgumentException("Entity was not a wyrmprint", nameof(entity));

        AbilityCrests crest = (AbilityCrests)entity.Id;

        if (await abilityCrestRepository.FindAsync(crest) is not null)
        {
            Entity dewEntity = new(EntityTypes.Dew, Id: 0, Quantity: 4000);
            logger.LogDebug(
                "Converted ability crest entity: {@entity} to {@dewEntity}.",
                entity,
                dewEntity
            );

            await userDataRepository.UpdateDewpoint(dewEntity.Quantity);

            convertedEntities.Add(new ConvertedEntity(entity, dewEntity));
            return RewardGrantResult.Converted;
        }

        logger.LogDebug("Granted new ability crest entity: {@entity}", entity);
        await abilityCrestRepository.Add(
            crest,
            entity.LimitBreakCount,
            entity.BuildupCount,
            entity.EquipableCount
        );

        newEntities.Add(entity);
        return RewardGrantResult.Added;
    }

    public EntityResult GetEntityResult()
    {
        return new()
        {
            new_get_entity_list = newEntities.Select(x => x.ToDuplicateEntityList()),
            converted_entity_list = convertedEntities.Select(x => x.ToConvertedEntityList()),
            over_discard_entity_list = discardedEntities.Select(
                x => x.ToBuildEventRewardEntityList()
            ),
            over_present_entity_list = Enumerable.Empty<AtgenBuildEventRewardEntityList>(),
            over_present_limit_entity_list = Enumerable.Empty<AtgenBuildEventRewardEntityList>()
        };
    }
}
