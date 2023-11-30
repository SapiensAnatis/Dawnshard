using FluentRandomPicker.FluentInterfaces.General;

namespace DragaliaAPI.Features.Dungeon;

public class SingleValuePicker<T> : IPick<T>
{
    private readonly T singleElement;

    public SingleValuePicker(T singleElement)
    {
        this.singleElement = singleElement;
    }

    public IEnumerable<T> Pick(int n) => Enumerable.Repeat(this.singleElement, n);

    public T PickOne() => this.singleElement;

    public IEnumerable<T> PickDistinct(int n) => throw new NotSupportedException();
}
