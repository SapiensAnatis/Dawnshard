using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Savefile.Mappers;

[Mapper]
public static partial class AbilityCrestSetMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial AbilityCrestSetList ToAbilityCrestSetList(
        this DbAbilityCrestSet dbEntity
    );
}
