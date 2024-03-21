using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Target)]
public static partial class PartyMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(
        nameof(DbParty.Units),
        nameof(PartyList.PartySettingList),
        Use = nameof(MapToPartySettingListArray)
    )]
    public static partial PartyList ToPartyList(this DbParty dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial PartySettingList MapToPartySettingList(this DbPartyUnit dbEntity);

    private static PartySettingList[] MapToPartySettingListArray(ICollection<DbPartyUnit> source)
    {
        PartySettingList[] target = new PartySettingList[source.Count];
        int i = 0;
        foreach (DbPartyUnit item in source)
        {
            target[i] = MapToPartySettingList(item);
            i++;
        }
        return target;
    }
}
