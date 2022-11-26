using DragaliaAPI.Models.Generated;
using MessagePack;

namespace DragaliaAPI.Models;

[MessagePackObject(true)]
public class EntityResult
{
    public IEnumerable<AtgenBuildEventRewardEntityList>? over_discard_entity_list { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList>? over_present_entity_list { get; set; }
    public IEnumerable<AtgenBuildEventRewardEntityList>? over_present_limit_entity_list { get; set; }
    public IEnumerable<AtgenDuplicateEntityList>? new_get_entity_list { get; set; }
    public IEnumerable<ConvertedEntityList>? converted_entity_list { get; set; }
}
