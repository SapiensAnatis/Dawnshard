using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.AutoMapper;

/// <summary>
/// Resolvers to calculate base and node atk/hp for an input CharaList.
/// </summary>

public class CharaBaseHpResolver : IValueResolver<CharaList, DbPlayerCharaData, ushort>
{
    public ushort Resolve(
        CharaList source,
        DbPlayerCharaData destination,
        ushort destMember,
        ResolutionContext context
    )
    {
        CharaData adventurer = MasterAsset.CharaData.Get(source.chara_id);

        return (ushort)CharaUtils.CalculateBaseHp(adventurer, source.level, source.rarity);
    }
}

public class CharaBaseAtkResolver : IValueResolver<CharaList, DbPlayerCharaData, ushort>
{
    public ushort Resolve(
        CharaList source,
        DbPlayerCharaData destination,
        ushort destMember,
        ResolutionContext context
    )
    {
        CharaData adventurer = MasterAsset.CharaData.Get(source.chara_id);

        return (ushort)CharaUtils.CalculateBaseAttack(adventurer, source.level, source.rarity);
    }
}

public class CharaNodeHpResolver : IValueResolver<CharaList, DbPlayerCharaData, ushort>
{
    public ushort Resolve(
        CharaList source,
        DbPlayerCharaData destination,
        ushort destMember,
        ResolutionContext context
    )
    {
        CharaData adventurer = MasterAsset.CharaData.Get(source.chara_id);

        return (ushort)(
            source.hp - CharaUtils.CalculateBaseHp(adventurer, source.level, source.rarity)
        );
    }
}

public class CharaNodeAtkResolver : IValueResolver<CharaList, DbPlayerCharaData, ushort>
{
    public ushort Resolve(
        CharaList source,
        DbPlayerCharaData destination,
        ushort destMember,
        ResolutionContext context
    )
    {
        CharaData adventurer = MasterAsset.CharaData.Get(source.chara_id);

        return (ushort)(
            source.attack - CharaUtils.CalculateBaseAttack(adventurer, source.level, source.rarity)
        );
    }
}
