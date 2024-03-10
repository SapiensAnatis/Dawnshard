using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class AbilityCrestMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(nameof(DbAbilityCrest.AbilityLevel), nameof(AbilityCrestList.Ability1Level))]
    [MapProperty(nameof(DbAbilityCrest.AbilityLevel), nameof(AbilityCrestList.Ability2Level))]
    public static partial AbilityCrestList ToAbilityCrestList(this DbAbilityCrest dbModel);
}
