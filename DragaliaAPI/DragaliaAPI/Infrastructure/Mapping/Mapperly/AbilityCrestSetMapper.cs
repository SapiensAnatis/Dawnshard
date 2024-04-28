using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Infrastructure.Mapping.Mapperly;

[Mapper]
public static partial class AbilityCrestSetMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial AbilityCrestSetList ToAbilityCrestSetList(
        this DbAbilityCrestSet dbEntity
    );
}
