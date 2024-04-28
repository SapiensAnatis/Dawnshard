using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.PartyPower;

[Mapper]
public static partial class PartyPowerMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial PartyPowerData ToPartyPowerData(this DbPartyPower dbEntity);
}
