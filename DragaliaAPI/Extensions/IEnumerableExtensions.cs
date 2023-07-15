using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DragaliaAPI.Extensions;

public static class IEnumerableExtensions
{
    public static bool TryGetElementAt<TElement>(
        this IEnumerable<TElement> enumerable,
        int index,
        [NotNullWhen(true)] out TElement? element
    )
        where TElement : class
    {
        element = enumerable.ElementAtOrDefault(index);

        return element is not null;
    }

    public static IEnumerable<TElement> PadWith<TElement>(
        this IEnumerable<TElement> source,
        Func<int, TElement> paddingFunc,
        int desiredLength
    )
    {
        int i = 0;
        foreach (TElement element in source)
        {
            yield return element;
            i++;
        }

        while (i <= desiredLength)
        {
            yield return paddingFunc.Invoke(i);
            i++;
        }
    }
}
