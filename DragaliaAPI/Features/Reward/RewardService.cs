using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DragaliaAPI.Features.Reward;

public class RewardService : IRewardService
{
    private readonly ILogger<RewardService> logger;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IAbilityCrestRepository abilityCrestRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IFortRepository fortRepository;

    private List<Entity> discardedEntities = new();
    private List<Entity> presentEntites = new();
    private List<Entity> presentLimitEntities = new();
    private List<Entity> newEntities = new();
    private List<ConvertedEntity> convertedEntities = new();

    public RewardService(
        ILogger<RewardService> logger,
        IInventoryRepository inventoryRepository,
        IUserDataRepository userDataRepository,
        IAbilityCrestRepository abilityCrestRepository,
        IUnitRepository unitRepository,
        IFortRepository fortRepository
    )
    {
        this.logger = logger;
        this.inventoryRepository = inventoryRepository;
        this.userDataRepository = userDataRepository;
        this.abilityCrestRepository = abilityCrestRepository;
        this.unitRepository = unitRepository;
        this.fortRepository = fortRepository;
    }

    public async Task<RewardGrantResult> GrantReward(Entity entity)
    {
        switch (entity.Type)
        {
            case EntityTypes.Chara:
                return await this.RewardCharacter(entity);
            case EntityTypes.Dragon:
                await this.unitRepository.AddDragons((Dragons)entity.Id);
                break;
            case EntityTypes.Dew:
                await this.userDataRepository.UpdateDewpoint(entity.Quantity);
                break;
            case EntityTypes.HustleHammer:
                (await this.userDataRepository.UserData.SingleAsync()).BuildTimePoint +=
                    entity.Quantity;
                break;
            case EntityTypes.Rupies:
                await this.userDataRepository.UpdateCoin(entity.Quantity);
                break;
            case EntityTypes.Wyrmite:
                await this.userDataRepository.GiveWyrmite(entity.Quantity);
                break;
            case EntityTypes.Wyrmprint:
                return await this.RewardAbilityCrest(entity);
            case EntityTypes.Material:
                (
                    await this.inventoryRepository.GetMaterial((Materials)entity.Id)
                    ?? this.inventoryRepository.AddMaterial((Materials)entity.Id)
                ).Quantity += entity.Quantity;
                break;
            case EntityTypes.Mana:
                (await this.userDataRepository.UserData.SingleAsync()).ManaPoint += entity.Quantity;
                break;
            case EntityTypes.FortPlant:
                await this.fortRepository.AddToStorage((FortPlants)entity.Id, 1);
                break;
            default:
                logger.LogWarning("Tried to reward unsupported entity {@entity}", entity);
                return RewardGrantResult.FailError;
        }

        this.newEntities.Add(entity);
        return RewardGrantResult.Added;
    }

    private async Task<RewardGrantResult> RewardCharacter(Entity entity)
    {
        if (entity.Type != EntityTypes.Chara)
            throw new ArgumentException("Entity was not a character", nameof(entity));

        Charas chara = (Charas)entity.Id;

        if (await this.unitRepository.FindCharaAsync(chara) is not null)
        {
            // Is it the correct behaviour to discard gifted characters?
            // Not sure -- never had characters in my gift box
            this.logger.LogDebug("Discarded character entity: {@entity}.", entity);
            discardedEntities.Add(entity);
            return RewardGrantResult.Discarded;
        }

        // TODO: Support EntityLevel/LimitBreak/etc here

        this.logger.LogDebug("Granted new character entity: {@entity}", entity);
        await this.unitRepository.AddCharas(chara);
        newEntities.Add(entity);
        return RewardGrantResult.Added;
    }

    private async Task<RewardGrantResult> RewardAbilityCrest(Entity entity)
    {
        if (entity.Type != EntityTypes.Wyrmprint)
            throw new ArgumentException("Entity was not a wyrmprint", nameof(entity));

        AbilityCrests crest = (AbilityCrests)entity.Id;

        if (await this.abilityCrestRepository.FindAsync(crest) is not null)
        {
            Entity dewEntity = new(EntityTypes.Dew, Id: 0, Quantity: 4000);
            this.logger.LogDebug(
                "Converted ability crest entity: {@entity} to {@dewEntity}.",
                entity,
                dewEntity
            );

            await this.userDataRepository.UpdateDewpoint(dewEntity.Quantity);

            convertedEntities.Add(new ConvertedEntity(entity, dewEntity));
            return RewardGrantResult.Converted;
        }

        this.logger.LogDebug("Granted new ability crest entity: {@entity}", entity);
        await this.abilityCrestRepository.Add(
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
            new_get_entity_list = this.newEntities.Select(x => x.ToDuplicateEntityList()),
            converted_entity_list = this.convertedEntities.Select(x => x.ToConvertedEntityList()),
            over_discard_entity_list = this.discardedEntities.Select(
                x => x.ToBuildEventRewardEntityList()
            ),
            over_present_entity_list = Enumerable.Empty<AtgenBuildEventRewardEntityList>(),
            over_present_limit_entity_list = Enumerable.Empty<AtgenBuildEventRewardEntityList>()
        };
    }
}
