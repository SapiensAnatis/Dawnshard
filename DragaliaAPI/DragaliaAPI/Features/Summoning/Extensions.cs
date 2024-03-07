using System.Globalization;

namespace DragaliaAPI.Features.Summoning;

public static class Extensions
{
    private static readonly NumberFormatInfo PercentFormatInfo =
        new() { PercentDecimalDigits = 3, PercentSymbol = "%" };

    public static string ToPercentageString(this decimal d, int decimalPlaces) =>
        d.ToString(
            "P",
            new NumberFormatInfo()
            {
                PercentDecimalDigits = decimalPlaces,
                PercentPositivePattern = 1
            }
        );
}
