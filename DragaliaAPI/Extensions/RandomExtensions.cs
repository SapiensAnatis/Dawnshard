using System;

namespace DragaliaAPI.Extensions;

public static class RandomExtensions
{
    public static T NextEnum<T>(this Random rdm) where T : struct, Enum
    {
        T[] values = Enum.GetValues<T>();

        return values[rdm.Next(values.Length)];
    }
}
