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
}
