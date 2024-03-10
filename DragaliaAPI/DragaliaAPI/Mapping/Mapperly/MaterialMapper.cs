using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class MaterialMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial MaterialList ToMaterialList(this DbPlayerMaterial dbEntity);
}
