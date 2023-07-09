using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shop;

public class PaymentService(
    ILogger<PaymentService> logger,
    IUserDataRepository userDataRepository,
    IInventoryRepository inventoryRepository
) : IPaymentService
{
    private readonly List<AtgenDeleteDragonList> dragonList = new();
    private readonly List<AtgenDeleteAmuletList> amuletList = new();
    private readonly List<AtgenDeleteTalismanList> talismanList = new();
    private readonly List<AtgenDeleteWeaponList> weaponList = new();

    public async Task ProcessPayment(Entity entity, PaymentTarget? payment = null)
    {
        if (entity.Type == EntityTypes.None)
            throw new ArgumentException("Invalid entity type", nameof(entity.Type));

        bool hasPaymentTarget = payment is not null;

        if (entity.Quantity == 0) // For free stuff, if needed.
            return;

        logger.LogDebug(
            "Processing {paymentType} payment {@payment}.",
            entity.Type,
            hasPaymentTarget ? payment : entity
        );

        if (hasPaymentTarget && entity.Quantity != payment!.target_cost)
            throw new DragaliaException(ResultCode.CommonUserStatusError, "Price mismatch.");

        int price = entity.Quantity;
        long? quantity;
        Action updater;

        switch (entity.Type)
        {
            case EntityTypes.Wyrmite:
                DbPlayerUserData userData = await userDataRepository.UserData.SingleAsync();
                quantity = userData.Crystal;
                updater = () => userData.Crystal -= price;
                break;
            case EntityTypes.HustleHammer:
                userData = await userDataRepository.UserData.SingleAsync();
                quantity = userData.BuildTimePoint;
                updater = () => userData.BuildTimePoint -= price;
                break;
            case EntityTypes.Mana:
                userData = await userDataRepository.UserData.SingleAsync();
                quantity = userData.ManaPoint;
                updater = () => userData.ManaPoint -= price;
                break;
            case EntityTypes.Rupies:
                userData = await userDataRepository.UserData.SingleAsync();
                quantity = userData.Coin;
                updater = () => userData.Coin -= price;
                break;
            case EntityTypes.Dew:
                userData = await userDataRepository.UserData.SingleAsync();
                quantity = userData.DewPoint;
                updater = () => userData.DewPoint -= price;
                break;
            case EntityTypes.SkipTicket:
                userData = await userDataRepository.UserData.SingleAsync();
                quantity = userData.QuestSkipPoint;
                updater = () => userData.QuestSkipPoint -= price;
                break;
            case EntityTypes.Material:
                DbPlayerMaterial? material = await inventoryRepository.GetMaterial(
                    (Materials)entity.Id
                );
                quantity = material?.Quantity;
                updater = () => material!.Quantity -= price;
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
                throw new NotImplementedException();
            case EntityTypes.SummonTicket:
                // TODO: Implement ticket payments.
                logger.LogWarning(
                    "Tried to pay with summon tickets - this is not (yet) supported!"
                );
                throw new DragaliaException(
                    ResultCode.ShopPaymentTypeInvalid,
                    "Tickets are not yet supported."
                );
            case EntityTypes.FreeDiamantium:
            case EntityTypes.PaidDiamantium:
                logger.LogDebug("Tried to pay with diamantium -- this is not supported.");
                throw new DragaliaException(
                    ResultCode.ShopPaymentTypeInvalid,
                    "Diamantium is not supported."
                );
            default:
                logger.LogWarning("Unknown/invalid entity type for payment.");
                throw new DragaliaException(
                    ResultCode.ShopPaymentTypeInvalid,
                    "Invalid payment type."
                );
        }

        if (quantity == null)
        {
            logger.LogError("Player does not own any of the entity.");

            throw new DragaliaException(ResultCode.CommonMaterialShort, "No entity owned.");
        }

        if (hasPaymentTarget && quantity != payment!.target_hold_quantity)
        {
            logger.LogError(
                "Held quantity {quantity} does not match target of payment {@payment}",
                quantity,
                payment
            );

            throw new DragaliaException(
                ResultCode.CommonUserStatusError,
                "Payment count mismatch."
            );
        }

        if (quantity < price)
        {
            logger.LogError(
                "Held quantity {quantity} does not meet price {price}",
                quantity,
                price
            );

            throw new DragaliaException(
                ResultCode.CommonMaterialShort,
                "Insufficient quantity for payment."
            );
        }

        updater();
    }

    public async Task ProcessPayment(
        PaymentTypes type,
        PaymentTarget? payment = null,
        int? expectedPrice = null
    )
    {
        if (type == PaymentTypes.DiamantiumOrWyrmite)
            throw new ArgumentException(
                "Cannot process DiamantiumOrWyrmite payment type",
                nameof(type)
            );

        switch (type)
        {
            default:
                EntityTypes entityType = type.ToEntityType();
                if (entityType == EntityTypes.None)
                    throw new ArgumentException(
                        $"Cannot process payment type {type}",
                        nameof(type)
                    );

                await ProcessPayment(new Entity(entityType, Quantity: expectedPrice ?? 0), payment);
                break;
        }
    }

    public Task ProcessPayment(Materials id, int quantity)
    {
        return ProcessPayment(new Entity(EntityTypes.Material, (int)id, quantity));
    }

    public DeleteDataList GetDeleteDataList()
    {
        return new DeleteDataList(dragonList, talismanList, weaponList, amuletList);
    }
}
