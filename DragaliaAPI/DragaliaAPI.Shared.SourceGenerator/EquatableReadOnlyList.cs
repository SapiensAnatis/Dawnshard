// Sourced from https://github.com/viceroypenguin/Immediate.Handlers/blob/master/src/Immediate.Handlers.Generators/EquatableReadOnlyList.cs
// The MIT License (MIT)
//
// Copyright (c) 2024 Immediate.Handlers developers
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections;

namespace DragaliaAPI.Shared.SourceGenerator;

public static class EquatableReadOnlyList
{
    public static EquatableReadOnlyList<T> ToEquatableReadOnlyList<T>(
        this IEnumerable<T> enumerable
    ) => new(enumerable.ToArray());
}

/// <summary>
///     A wrapper for IReadOnlyList that provides value equality support for the wrapped list.
/// </summary>
public readonly struct EquatableReadOnlyList<T>(IReadOnlyList<T>? collection)
    : IEquatable<EquatableReadOnlyList<T>>,
        IReadOnlyList<T>
{
    private IReadOnlyList<T> Collection => collection ?? [];

    public bool Equals(EquatableReadOnlyList<T> other) => this.SequenceEqual(other);

    public override bool Equals(object? obj) =>
        obj is EquatableReadOnlyList<T> other && this.Equals(other);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        foreach (var item in this.Collection)
            hashCode.Add(item);

        return hashCode.ToHashCode();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.Collection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.Collection.GetEnumerator();

    public int Count => this.Collection.Count;
    public T this[int index] => this.Collection[index];

    public static bool operator ==(EquatableReadOnlyList<T> left, EquatableReadOnlyList<T> right) =>
        left.Equals(right);

    public static bool operator !=(EquatableReadOnlyList<T> left, EquatableReadOnlyList<T> right) =>
        !left.Equals(right);
}
