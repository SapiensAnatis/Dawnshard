using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DragaliaAPI.Shared.Serialization;
using MessagePack;

namespace DragaliaAPI.Shared.MasterAsset;

/// <summary>
/// Class that is composed of a <see cref="KeyedCollection{TKey,TItem}"/> implementation and populates it based on
/// supplied JSON data and a key selector argument. Exposes read-only methods of the <see cref="KeyedCollection{TKey,TItem}"/>
/// implementation.
/// </summary>
/// <remarks>
/// The population of the data via deserialization is lazy-loaded, and only performed on first access via one
/// of the public methods or properties.
/// JSON is deserialized using <see cref="MasterAssetJsonOptions"/> which notably includes the <see cref="MasterAssetNamingPolicy"/>.
/// </remarks>
/// <typeparam name="TKey">The type of the data's unique key.</typeparam>
/// <typeparam name="TItem">The type of the data models that will be returned. Should be a record or immutable class.</typeparam>
public sealed class MasterAssetData<TKey, TItem>
    where TItem : class
    where TKey : notnull
{
    private readonly FrozenDictionary<TKey, TItem> internalKeyCollection;

    /// <summary>
    /// Gets a <see cref="IEnumerable{TItem}"/> of all the collection's values.
    /// </summary>
    public IEnumerable<TItem> Enumerable => this.internalKeyCollection.Values;

    /// <summary>
    /// Gets the number of items in the collection.
    /// </summary>
    public int Count => this.internalKeyCollection.Count;

    /// <summary>
    /// Get a <typeparam name="TItem"> instance corresponding to the given <typeparam name="TKey"/> key.</typeparam>
    /// </summary>
    /// <param name="key">The key to index with.</param>
    /// <returns>The returned value.</returns>
    /// <exception cref="KeyNotFoundException">The given key was not present in the collection.</exception>
    public TItem Get(TKey key) => this[key];

    /// <summary>
    /// Try to get a <typeparamref name="TItem"/>, returning <see langword="null"/> if not found.
    /// </summary>
    /// <param name="key">The key to index with.</param>
    /// <returns>The returned value.</returns>
    public TItem? GetValueOrDefault(TKey key)
    {
        this.TryGetValue(key, out TItem? value);
        return value;
    }

    /// <summary>
    /// Get a <typeparam name="TItem"> instance corresponding to the given <typeparam name="TKey"/> key.</typeparam>
    /// </summary>
    /// <param name="key">The key to index with.</param>
    /// <returns>The returned value.</returns>
    /// <exception cref="KeyNotFoundException">The given key was not present in the collection.</exception>
    public TItem this[TKey key] => this.internalKeyCollection[key];

    /// <summary>
    /// Attempts to get a <typeparam name="TItem"> instance corresponding to the given <typeparam name="TKey"/> key.</typeparam>
    /// </summary>
    /// <param name="key">The key to index with.</param>
    /// <param name="item">The returned value, if the master data contained it.</param>
    /// <returns>A bool indicating whether the value was successfully retrieved.</returns>
    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TItem? item) =>
        this.internalKeyCollection.TryGetValue(key, out item);

    /// <summary>
    /// Checks whether the master data contains the given <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key to check the existence of.</param>
    /// <returns>A bool indicating whether that key is in the dictionary.</returns>
    public bool ContainsKey(TKey key) => this.internalKeyCollection.ContainsKey(key);

    public MasterAssetData(FrozenDictionary<TKey, TItem> frozenKeyedCollection)
    {
        this.internalKeyCollection = frozenKeyedCollection;
    }
}

public static class MasterAssetData
{
    private const string DataFolder = "Resources";

    public static async ValueTask<MasterAssetData<TKey, TItem>> LoadAsync<TKey, TItem>(
        string jsonFilename,
        Func<TItem, TKey> keySelector
    )
        where TItem : class
        where TKey : notnull
    {
        string path = Path.Join(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            DataFolder,
            jsonFilename
        );

        await using FileStream fs = File.OpenRead(path);

        List<TItem> items =
            await MessagePackSerializer.DeserializeAsync<List<TItem>>(
                fs,
                MasterAssetMessagePackOptions.Instance
            ) ?? throw new MessagePackSerializationException("Deserialized MasterAsset was null");

        FrozenDictionary<TKey, TItem> frozenDict = items
            .ToDictionary(keySelector, x => x)
            .ToFrozenDictionary();

        return new MasterAssetData<TKey, TItem>(frozenDict);
    }
}
