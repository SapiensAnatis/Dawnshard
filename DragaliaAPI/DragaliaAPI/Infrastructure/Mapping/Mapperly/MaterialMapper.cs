using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Infrastructure.Mapping.Mapperly;

[Mapper]
public static partial class MaterialMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial MaterialList ToMaterialList(this DbPlayerMaterial dbEntity);
}
