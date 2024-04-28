using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Infrastructure.Mapping.Mapperly;

[Mapper]
public static partial class AbilityCrestMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(nameof(DbAbilityCrest.AbilityLevel), nameof(AbilityCrestList.Ability1Level))]
    [MapProperty(nameof(DbAbilityCrest.AbilityLevel), nameof(AbilityCrestList.Ability2Level))]
    public static partial AbilityCrestList ToAbilityCrestList(this DbAbilityCrest dbModel);
}
