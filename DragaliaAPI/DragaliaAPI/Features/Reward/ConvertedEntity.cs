using DragaliaAPI.DTO;

namespace DragaliaAPI.Features.Reward;

public record ConvertedEntity(Entity Before, Entity After)
{
    public ConvertedEntityList ToConvertedEntityList()
    {
        return new()
        {
            BeforeEntityType = Before.Type,
            BeforeEntityId = Before.Id,
            BeforeEntityQuantity = Before.Quantity,
            AfterEntityType = After.Type,
            AfterEntityId = After.Id,
            AfterEntityQuantity = After.Quantity
        };
    }
};
