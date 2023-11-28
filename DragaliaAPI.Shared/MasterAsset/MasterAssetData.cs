using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using DragaliaAPI.Shared.Json;

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
public class MasterAssetData<TKey, TItem>
    where TItem : class
    where TKey : notnull
{
    private const string JsonFolder = "Resources";

    private readonly string jsonFilename;
    private readonly Func<TItem, TKey> keySelector;
    private readonly FrozenKeyedCollection internalKeyCollection;

    /// <summary>
    /// Gets a <see cref="IEnumerable{TItem}"/> of all the collection's values.
    /// </summary>
    public IEnumerable<TItem> Enumerable => this.internalKeyCollection.Items;

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
    /// Creates a new instance of <see cref="MasterAssetData{TKey,TItem}"/>.
    /// </summary>
    /// <param name="jsonFilename">The filename of the JSON in <see cref="JsonFolder"/>.</param>
    /// <param name="keySelector">A function that returns a unique <typeparamref name="TKey"/> value from a
    /// <typeparamref name="TItem"/>.</param>
    public MasterAssetData(string jsonFilename, Func<TItem, TKey> keySelector)
    {
        this.jsonFilename = jsonFilename;
        this.keySelector = keySelector;
        this.internalKeyCollection = DataFactory();
    }

    private FrozenKeyedCollection DataFactory()
    {
        string path = Path.Join(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            JsonFolder,
            jsonFilename
        );

        using FileStream fs = File.OpenRead(path);

        IReadOnlyCollection<TItem> items =
            JsonSerializer.Deserialize<IReadOnlyCollection<TItem>>(
                fs,
                MasterAssetJsonOptions.Instance
            ) ?? throw new JsonException("Deserialized IEnumerable was null");

        return new FrozenKeyedCollection(items, this.keySelector);
    }

    private class FrozenKeyedCollection
    {
        public readonly ImmutableArray<TItem> Items;
        private readonly FrozenDictionary<TKey, TItem> dictionary;

        public FrozenKeyedCollection(IReadOnlyCollection<TItem> items, Func<TItem, TKey> selector)
        {
            this.dictionary = items.ToFrozenDictionary(selector, x => x);
            this.Items = this.dictionary.Values;
        }

        public TItem this[TKey key] => this.dictionary[key];

        public bool TryGetValue(TKey key, [NotNullWhen(true)] out TItem? item) =>
            this.dictionary.TryGetValue(key, out item);
    }
}
