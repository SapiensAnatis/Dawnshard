using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Features.Summoning;

public class UnitRate
{
    /// <summary>
    /// The factor to use when converting the <see cref="decimal"/> rate to a <see cref="int"/> weighting for
    /// FluentRandomPicker.
    /// </summary>
    /// <remarks>
    /// A value of 100_000 means that a rate of 41.125% / 0.41125 does not lose any precision and  is converted to
    /// 41125. 3 decimal places of precision on the percentage is most likely 'good enough'.
    /// </remarks>
    private const int RateConversionFactor = 100_000;

    public int Id { get; }

    public EntityTypes EntityType { get; }

    public int Rarity { get; }

    public decimal Rate { get; }

    public int Weighting => (int)(this.Rate * RateConversionFactor);

    public UnitRate(Dragons dragon, decimal rate)
    {
        this.Id = (int)dragon;
        this.EntityType = EntityTypes.Dragon;
        this.Rarity = MasterAsset.DragonData[dragon].Rarity;
        this.Rate = rate;
    }

    public UnitRate(Charas chara, decimal rate)
    {
        this.Id = (int)chara;
        this.EntityType = EntityTypes.Chara;
        this.Rarity = MasterAsset.CharaData[chara].Rarity;
        this.Rate = rate;
    }

    public AtgenUnitList ToAtgenUnitList()
    {
        return new AtgenUnitList() { Id = this.Id, Rate = this.Rate.ToPercentageString3Dp() };
    }
}
