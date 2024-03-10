using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class PartyPowerMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial PartyPowerData ToPartyPowerData(this DbPartyPower dbEntity);
}
