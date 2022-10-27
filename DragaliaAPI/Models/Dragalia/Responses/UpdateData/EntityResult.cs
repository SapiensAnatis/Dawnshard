using DragaliaAPI.Models.Dragalia.Responses.Common;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.UpdateData;

[MessagePackObject(true)]
public record EntityResult(
    IEnumerable<BaseNewEntity> converted_entity_list,
    IEnumerable<BaseNewEntity> new_get_entity_list
);

public static class EntityResultStatic
{
    public static readonly EntityResult Empty =
        new(new List<BaseNewEntity>(), new List<BaseNewEntity>());
}
