using System.Collections.Immutable;
using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Item;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shared.Reward.Handlers;

// TODO: Move types out of this class into their own dedicated handlers.

public class GenericRewardHandler(
    ILogger<RewardService> logger,
    IInventoryRepository inventoryRepository,
    IUserDataRepository userDataRepository,
    IFortRepository fortRepository,
    IEventRepository eventRepository,
    IDmodeRepository dmodeRepository,
    IItemRepository itemRepository,
    IUserService userService
) : IRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; } =
        ImmutableArray.Create(
            EntityTypes.Item,
            EntityTypes.Dew,
            EntityTypes.Rupies,
            EntityTypes.SkipTicket,
            EntityTypes.Material,
            EntityTypes.Mana,
            EntityTypes.FortPlant,
            EntityTypes.BuildEventItem,
            EntityTypes.Clb01EventItem,
            EntityTypes.CollectEventItem,
            EntityTypes.RaidEventItem,
            EntityTypes.MazeEventItem,
            EntityTypes.ExRushEventItem,
            EntityTypes.SimpleEventItem,
            EntityTypes.ExHunterEventItem,
            EntityTypes.BattleRoyalEventItem,
            EntityTypes.EarnEventItem,
            EntityTypes.CombatEventItem
        );

    public async Task<GrantReturn> Grant(Entity entity)
    {
        switch (entity.Type)
        {
            case EntityTypes.Item:
                await itemRepository.AddItemQuantityAsync((UseItem)entity.Id, entity.Quantity);
                break;
            case EntityTypes.Dew:
                await userDataRepository.UpdateDewpoint(entity.Quantity);
                break;
            case EntityTypes.Rupies:
                await userDataRepository.UpdateCoin(entity.Quantity);
                break;
            case EntityTypes.SkipTicket:
                await userService.AddQuestSkipPoint(entity.Quantity);
                break;
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
            default:
                logger.LogWarning("Tried to reward unsupported entity {@entity}", entity);
                return new(RewardGrantResult.FailError);
        }

        return new(RewardGrantResult.Added);
    }
}
