using System.Diagnostics.CodeAnalysis;

namespace DragaliaAPI.Extensions;

public static class EnumerableExtensions
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
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);

        return Enumerable.Repeat(enumerable, count).SelectMany(x => x);
    }

    /// <summary>
    /// Pads a sequence to a length equal to or greater than the provided <paramref name="desiredLength"/>, by filling
    /// any short-fall with the default value for <typeparamref name="TElement"/>.
    /// </summary>
    /// <param name="enumerable">The input sequence.</param>
    /// <param name="desiredLength">The desired minimum length.</param>
    /// <typeparam name="TElement">The type of the elements of the input sequence.</typeparam>
    /// <returns>A new sequence with the padding applied.</returns>
    public static IEnumerable<TElement?> Pad<TElement>(
        this IEnumerable<TElement> enumerable,
        int desiredLength
    )
    {
        int count = 0;
        foreach (TElement item in enumerable)
        {
            yield return item;
            count++;
        }

        while (count < desiredLength)
        {
            yield return default;
            count++;
        }
    }
}
