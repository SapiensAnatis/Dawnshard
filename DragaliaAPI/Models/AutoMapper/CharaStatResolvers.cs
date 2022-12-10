using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using AutoMapper.Execution;
using AutoMapper.Internal;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Services;

namespace DragaliaAPI.Models.AutoMapper;

/// <summary>
/// Resolvers to calculate base and node atk/hp for an input CharaList.
/// </summary>

public class CharaBaseHpResolver : IValueResolver<CharaList, DbPlayerCharaData, ushort>
{
    private readonly ICharaDataService charaDataService;

    public CharaBaseHpResolver(ICharaDataService charaDataService)
    {
        this.charaDataService = charaDataService;
    }

    public ushort Resolve(
        CharaList source,
        DbPlayerCharaData destination,
        ushort destMember,
        ResolutionContext context
    )
    {
        DataAdventurer adventurer = this.charaDataService.GetData(source.chara_id);

        return (ushort)CharaUtils.CalculateBaseHp(adventurer, source.level, source.rarity);
    }
}

public class CharaBaseAtkResolver : IValueResolver<CharaList, DbPlayerCharaData, ushort>
{
    private readonly ICharaDataService charaDataService;

    public CharaBaseAtkResolver(ICharaDataService charaDataService)
    {
        this.charaDataService = charaDataService;
    }

    public ushort Resolve(
        CharaList source,
        DbPlayerCharaData destination,
        ushort destMember,
        ResolutionContext context
    )
    {
        DataAdventurer adventurer = this.charaDataService.GetData(source.chara_id);

        return (ushort)CharaUtils.CalculateBaseAttack(adventurer, source.level, source.rarity);
    }
}

public class CharaNodeHpResolver : IValueResolver<CharaList, DbPlayerCharaData, ushort>
{
    private readonly ICharaDataService charaDataService;

    public CharaNodeHpResolver(ICharaDataService charaDataService)
    {
        this.charaDataService = charaDataService;
    }

    public ushort Resolve(
        CharaList source,
        DbPlayerCharaData destination,
        ushort destMember,
        ResolutionContext context
    )
    {
        DataAdventurer adventurer = this.charaDataService.GetData(source.chara_id);

        return (ushort)(
            source.hp - CharaUtils.CalculateBaseHp(adventurer, source.level, source.rarity)
        );
    }
}

public class CharaNodeAtkResolver : IValueResolver<CharaList, DbPlayerCharaData, ushort>
{
    private readonly ICharaDataService charaDataService;

    public CharaNodeAtkResolver(ICharaDataService charaDataService)
    {
        this.charaDataService = charaDataService;
    }

    public ushort Resolve(
        CharaList source,
        DbPlayerCharaData destination,
        ushort destMember,
        ResolutionContext context
    )
    {
        DataAdventurer adventurer = this.charaDataService.GetData(source.chara_id);

        return (ushort)(
            source.attack - CharaUtils.CalculateBaseHp(adventurer, source.level, source.rarity)
        );
    }
}
