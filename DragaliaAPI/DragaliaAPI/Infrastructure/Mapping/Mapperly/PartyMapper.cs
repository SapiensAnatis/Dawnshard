using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Infrastructure.Mapping.Mapperly;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Target)]
public static partial class PartyMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(nameof(DbParty.Units), nameof(PartyList.PartySettingList))]
    public static partial PartyList ToPartyList(this DbParty dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial PartySettingList? MapToPartySettingList(this DbPartyUnit dbEntity);
}
