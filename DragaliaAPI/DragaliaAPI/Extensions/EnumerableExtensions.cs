using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

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

    public static async Task<HashSet<TElement>> ToHashSetAsync<TElement>(
        this IQueryable<TElement> enumerable,
        IEqualityComparer<TElement>? comparer = null,
        CancellationToken cancellationToken = default
    )
        where TElement : struct
    {
        comparer ??= EqualityComparer<TElement>.Default;

        HashSet<TElement> set = new(comparer);
        await foreach (
            TElement element in enumerable.AsAsyncEnumerable().WithCancellation(cancellationToken)
        )
        {
            set.Add(element);
        }

        return set;
    }
}
