using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class FortBuildMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial BuildList ToBuildList(this DbFortBuild dbEntity);
}
