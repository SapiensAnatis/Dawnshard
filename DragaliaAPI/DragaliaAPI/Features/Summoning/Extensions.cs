using System.Globalization;

namespace DragaliaAPI.Features.Summoning;

public static class Extensions
{
    private static readonly NumberFormatInfo PercentFormatInfo = new() { PercentDecimalDigits = 3 };

    public static string ToPercentageString(this decimal d) => d.ToString("P", PercentFormatInfo);
}
