using System;
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

    public static T NextElement<T>(this Random rdm, List<T> values)
    {
        Debug.Assert(values.Count != 0, "values.Count != 0");

        return values[rdm.Next(values.Count - 1)];
    }
}
