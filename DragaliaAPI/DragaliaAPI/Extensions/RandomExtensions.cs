using System.Diagnostics;

namespace DragaliaAPI.Extensions;

public static class RandomExtensions
{
    public static T NextEnum<T>(this Random rdm)
        where T : struct, Enum
    {
        T[] values = Enum.GetValues<T>();

        return values[rdm.Next(values.Length)];
    }

    public static T Next<T>(this Random rdm, IReadOnlyList<T> values)
    {
        Debug.Assert(values.Count > 0, "values.Length > 0");

        return values[rdm.Next(values.Count)];
    }

    public static T Next<T>(this Random rdm, in Span<T> values)
    {
        Debug.Assert(values.Length > 0, "values.Length > 0");

        return values[rdm.Next(values.Length)];
    }
}
