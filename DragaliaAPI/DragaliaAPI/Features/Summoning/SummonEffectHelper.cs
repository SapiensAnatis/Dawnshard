using DragaliaAPI.Shared.Definitions.Enums.Summon;

namespace DragaliaAPI.Features.Summoning;

public static class SummonEffectHelper
{
    public static SummonEffect CalculateEffect(SummonService.SummonResultMetaInfo metaInfo)
    {
        int reversalIndex = metaInfo.LastIndexOfRare5;
        if (reversalIndex != -1 && new Random().NextSingle() < 0.95)
            reversalIndex = -1;

        int sageEffect;
        int circleEffect;
        int rarityDisplayModifier = reversalIndex == -1 ? 0 : 1;
        if (metaInfo.CountOfRare5Char + metaInfo.CountOfRare5Dragon > 0 + rarityDisplayModifier)
        {
            sageEffect =
                metaInfo.CountOfRare5Dragon > metaInfo.CountOfRare5Char
                    ? (int)SummonEffectsSage.GoldFafnirs
                    : (int)SummonEffectsSage.RainbowCrystal;
            circleEffect = (int)SummonEffectsSky.Rainbow;
        }
        else
        {
            circleEffect = (int)SummonEffectsSky.Yellow;
            switch (
                metaInfo.CountOfRare4
                + ((metaInfo.CountOfRare5Char + metaInfo.CountOfRare5Dragon) * 2)
            )
            {
                case > 1:
                    sageEffect = (int)SummonEffectsSage.MultiDoves;
                    break;
                case > 0:
                    sageEffect = (int)SummonEffectsSage.SingleDove;
                    break;
                default:
                    sageEffect = (int)SummonEffectsSage.Dull;
                    circleEffect = (int)SummonEffectsSky.Blue;
                    break;
            }
        }

        return new SummonEffect(
            SageEffect: sageEffect,
            CircleEffect: circleEffect,
            ReversalIndex: reversalIndex
        );
    }
}

public readonly record struct SummonEffect(int SageEffect, int CircleEffect, int ReversalIndex);
