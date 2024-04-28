using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Savefile.Mappers;

[Mapper]
public static partial class MaterialMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial MaterialList ToMaterialList(this DbPlayerMaterial dbEntity);
}
