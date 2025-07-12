using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class SupportMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(AtgenSupportChara.StatusPlusCount))]
    public static partial AtgenSupportChara ToSupportChara(this DbPlayerCharaData charaData);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial AtgenSupportWeaponBody ToSupportWeaponBody(this DbWeaponBody dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(AtgenSupportDragon.Hp))]
    [MapperIgnoreTarget(nameof(AtgenSupportDragon.Attack))]
    [MapperIgnoreTarget(nameof(AtgenSupportDragon.StatusPlusCount))]
    public static partial AtgenSupportDragon ToSupportDragon(this DbPlayerDragonData dbModel);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial AtgenSupportCrestSlotType1List? MapToSupportAbilityCrestList(
        this DbAbilityCrest? dbEntity
    );

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial AtgenSupportTalisman ToSupportTalisman(this DbTalisman? dbEntity);
}
