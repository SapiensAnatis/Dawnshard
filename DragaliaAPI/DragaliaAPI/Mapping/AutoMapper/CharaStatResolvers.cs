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
        CharaData adventurer = MasterAsset.CharaData.Get(source.CharaId);

        return (ushort)CharaUtils.CalculateBaseHp(adventurer, source.Level, source.Rarity);
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
        CharaData adventurer = MasterAsset.CharaData.Get(source.CharaId);

        return (ushort)CharaUtils.CalculateBaseAttack(adventurer, source.Level, source.Rarity);
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
        CharaData adventurer = MasterAsset.CharaData.Get(source.CharaId);

        int nodeHp =
            source.Hp - CharaUtils.CalculateBaseHp(adventurer, source.Level, source.Rarity);

        return (ushort)Math.Clamp(nodeHp, ushort.MinValue, ushort.MaxValue);
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
        CharaData adventurer = MasterAsset.CharaData.Get(source.CharaId);

        int nodeAtk =
            source.Attack - CharaUtils.CalculateBaseAttack(adventurer, source.Level, source.Rarity);

        return (ushort)Math.Clamp(nodeAtk, ushort.MinValue, ushort.MaxValue);
    }
}
