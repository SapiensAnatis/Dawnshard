using System.Collections;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset.Models;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Dungeon;

[Mapper(
    RequiredMappingStrategy = RequiredMappingStrategy.Target,
    IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Target
)]
public static partial class DungeonMapper
{
    public static partial IEnumerable<PartyUnitList> MapToPartyUnitList(
        this IEnumerable<DbDetailedPartyUnit> detailedPartyUnits
    );

    public static partial IEnumerable<PartySettingList> MapToPartySettingList(
        this IEnumerable<DbPartyUnit> partyUnits
    );

    public static partial IEnumerable<AreaInfoList> MapToAreaInfoList(
        this IEnumerable<AreaInfo> areaInfo
    );

    [MapperIgnoreTarget(nameof(CharaList.StatusPlusCount))]
    private static partial CharaList MapToCharaList(DbPlayerCharaData charaData);

    [MapperIgnoreTarget(nameof(DragonList.StatusPlusCount))]
    private static partial DragonList MapToDragonList(DbPlayerDragonData charaData);

    [MapProperty(nameof(DbAbilityCrest.AbilityLevel), nameof(GameAbilityCrest.Ability1Level))]
    [MapProperty(nameof(DbAbilityCrest.AbilityLevel), nameof(GameAbilityCrest.Ability2Level))]
    private static partial GameAbilityCrest? MapToGameAbilityCrest(DbAbilityCrest? abilityCrest);
}
