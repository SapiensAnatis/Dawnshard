using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Fort;

[Mapper]
public static partial class FortBuildMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial BuildList ToBuildList(this DbFortBuild dbEntity);
}
