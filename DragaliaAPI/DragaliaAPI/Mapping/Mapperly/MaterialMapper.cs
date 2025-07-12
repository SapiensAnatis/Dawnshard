using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class MaterialMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial MaterialList MapToMaterialList(this DbPlayerMaterial dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DbPlayerMaterial.Owner))]
    public static partial DbPlayerMaterial MapToDbPlayerMaterial(
        this MaterialList materialList,
        long viewerId
    );
}
