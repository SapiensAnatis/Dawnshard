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
        where TElement : class?
    {
        element = enumerable.ElementAtOrDefault(index);

        return element is not null;
    }

    public static IEnumerable<TElement> NotNull<TElement>(this IEnumerable<TElement?> enumerable)
    {
        foreach (TElement? element in enumerable)
        {
            if (element is not null)
            {
                yield return element;
            }
        }
    }

    /// <summary>
    /// Repeats the input <paramref name="enumerable"/> after itself <paramref name="count"/> times.
    /// </summary>
    /// <param name="enumerable">Input enumerable.</param>
    /// <param name="count">Number of times to repeat.</param>
    /// <typeparam name="TElement">The element type of <paramref name="enumerable"/>.</typeparam>
    /// <returns>The repeated enumerable.</returns>
    public static IEnumerable<TElement> Repeat<TElement>(
        this IEnumerable<TElement> enumerable,
        int count
    )
        where TElement : class?
    {
        if (count < 1)
            throw new ArgumentOutOfRangeException(nameof(count));

        return Enumerable.Repeat(enumerable, count).SelectMany(x => x);
    }
}
