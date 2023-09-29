using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Item;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Talisman;
using DragaliaAPI.Features.Tickets;
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
    IEventRepository eventRepository,
    IDmodeRepository dmodeRepository,
    IItemRepository itemRepository,
    IUserService userService,
    ITicketRepository ticketRepository
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

        if (result == RewardGrantResult.Added)
            newEntities.Add(entity);

        return result;
    }

    public async Task GrantRewards(IEnumerable<Entity> entities)
    {
        entities = entities.ToList();
        logger.LogTrace("Granting rewards: {@rewards}", entities);

        foreach (Entity entity in entities)
        {
            RewardGrantResult result = await GrantRewardInternal(entity);
            if (result == RewardGrantResult.Added)
                newEntities.Add(entity);
        }
    }

    private async Task<RewardGrantResult> GrantRewardInternal(Entity entity)
    {
        switch (entity.Type)
        {
            case EntityTypes.Chara:
                return await RewardCharacter(entity);
            case EntityTypes.Item:
                await itemRepository.AddItemQuantityAsync((UseItem)entity.Id, entity.Quantity);
                break;
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
            case EntityTypes.SkipTicket:
                await userService.AddQuestSkipPoint(entity.Quantity);
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
            case EntityTypes.DmodePoint:
                DbPlayerDmodeInfo info = await dmodeRepository.GetInfoAsync();
                if (entity.Id == (int)DmodePoint.Point1)
                    info.Point1Quantity += entity.Quantity;
                else if (entity.Id == (int)DmodePoint.Point2)
                    info.Point2Quantity += entity.Quantity;
                else
                    throw new UnreachableException("Invalid dmode point id");
                break;
            case EntityTypes.SummonTicket:
                ticketRepository.AddTicket((SummonTickets)entity.Id, entity.Quantity);
                break;
            default:
                logger.LogWarning("Tried to reward unsupported entity {@entity}", entity);
                return RewardGrantResult.FailError;
        }

        return RewardGrantResult.Added;
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
