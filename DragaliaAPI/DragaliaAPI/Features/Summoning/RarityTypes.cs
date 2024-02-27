using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Summoning;

public record RarityGroup(int Rarity, double TotalRate, double CharaRate, double DragonRate);

public class UnitRarity
{
    public int Id { get; }

    public double Rate { get; }

    public UnitRarity(Dragons dragon, double rate)
    {
        this.Id = (int)dragon;
        this.Rate = rate;
    }

    public UnitRarity(Charas chara, double rate)
    {
        this.Id = (int)chara;
        this.Rate = rate;
    }
}
