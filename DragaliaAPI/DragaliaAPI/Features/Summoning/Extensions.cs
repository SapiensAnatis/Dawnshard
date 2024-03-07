using System.Globalization;

namespace DragaliaAPI.Features.Summoning;

public static class Extensions
{
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
