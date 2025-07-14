using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Target)]
public static partial class PartyMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(nameof(DbParty.Units), nameof(PartyList.PartySettingList))]
    public static partial PartyList MapToPartyList(this DbParty dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial PartySettingList MapToPartySettingList(this DbPartyUnit dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial PartySettingList MapToPartySettingList(
        this DbQuestClearPartyUnit dbEntity
    );

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DbPartyUnit.Id))]
    [MapperIgnoreTarget(nameof(DbPartyUnit.Party))]
    public static partial DbPartyUnit MapToDbPartyUnit(
        this PartySettingList partySettingList,
        long viewerId,
        int partyNo
    );

    public static DbParty MapToDbParty(this PartyList partyList, long viewerId)
    {
        return new DbParty()
        {
            ViewerId = viewerId,
            PartyNo = partyList.PartyNo,
            PartyName = partyList.PartyName,
            Units = partyList
                .PartySettingList.Select(x => x.MapToDbPartyUnit(viewerId, partyList.PartyNo))
                .ToList(),
        };
    }
}
