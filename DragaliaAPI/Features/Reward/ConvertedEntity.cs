using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Reward;

public record ConvertedEntity(Entity Before, Entity After)
{
    public ConvertedEntityList ToConvertedEntityList()
    {
        return new()
        {
            before_entity_type = Before.Type,
            before_entity_id = Before.Id,
            before_entity_quantity = Before.Quantity,
            after_entity_type = After.Type,
            after_entity_id = After.Id,
            after_entity_quantity = After.Quantity
        };
    }
};
