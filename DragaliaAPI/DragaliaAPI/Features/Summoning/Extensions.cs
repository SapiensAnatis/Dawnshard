using System.Globalization;

namespace DragaliaAPI.Features.Summoning;

public static class Extensions
{
    public static string ToPercentageString2Dp(this decimal d) => d.ToString("P", TwoDpFormat);

    public static string ToPercentageString3Dp(this decimal d) => d.ToString("P", ThreeDpFormat);

    private static readonly NumberFormatInfo TwoDpFormat =
        new() { PercentDecimalDigits = 2, PercentPositivePattern = 1 };

    private static readonly NumberFormatInfo ThreeDpFormat =
        new() { PercentDecimalDigits = 3, PercentPositivePattern = 1 };
}
