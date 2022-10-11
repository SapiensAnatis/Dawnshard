using System;
using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Dragalia.Responses.Common;

namespace DragaliaAPI.Services;

public class SummonService : ISummonService
{
    public List<SummonEntity> GenerateSummonResult(int numSummons)
    {
        Random random = new((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        List<SummonEntity> resultList = new();

        for (int i = 0; i < numSummons; i++)
        {
            // TODO: Weight the RNG by rarity
            // TODO: Set rarity correctly
            bool isDragon = random.NextSingle() > 0.5;
            if (isDragon)
            {
                Dragons id = NextEnum<Dragons>(random);
                resultList.Add(new((int)EntityTypes.Dragon, (int)id, 5));
            }
            else
            {
                Charas id = NextEnum<Charas>(random);
                resultList.Add(new((int)EntityTypes.Chara, (int)id, 5));
            }
        }

        return resultList;
    }

    private T NextEnum<T>(Random random) where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        return (T)(
            values.GetValue(random.Next(values.Length))
            ?? throw new Exception("Invalid random value!")
        );
    }
}
