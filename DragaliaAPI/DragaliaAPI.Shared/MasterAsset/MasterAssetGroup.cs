using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Shared.MasterAsset;

public class MasterAssetGroup<TGroupKey, TKey, TItem>
    where TItem : class
    where TKey : notnull
    where TGroupKey : notnull
{
    private const string JsonFolder = "Resources";

    private readonly string jsonFilename;
    private readonly Func<TItem, TKey> keySelector;
    private readonly Lazy<Dictionary<TGroupKey, InternalKeyedCollection>> internalDictionary;

    /// <summary>
    /// Gets a <see cref="IEnumerable{TItem}"/> of all the collection's values.
    /// </summary>
    public IEnumerable<IEnumerable<TItem>> Enumerable =>
        this.internalDictionary.Value.Values.Select(x => x.AsEnumerable());

    /// <summary>
    /// Get a <typeparam name="TItem"> instance corresponding to the given <typeparam name="TKey"/> key.</typeparam>
    /// </summary>
    /// <param name="key">The key to index with.</param>
    /// <returns>The returned value.</returns>
    /// <exception cref="KeyNotFoundException">The given key was not present in the collection.</exception>
    public IDictionary<TKey, TItem> Get(TGroupKey key) => this[key];

    /// <summary>
    /// Get a <typeparam name="TItem"> instance corresponding to the given <typeparam name="TKey"/> key.</typeparam>
    /// </summary>
    /// <param name="key">The key to index with.</param>
    /// <returns>The returned value.</returns>
    /// <exception cref="KeyNotFoundException">The given key was not present in the collection.</exception>
    public IDictionary<TKey, TItem> this[TGroupKey key] =>
        this.internalDictionary.Value[key].AsImmutableDictionary();

    /// <summary>
    /// Attempts to get a <typeparam name="TItem"> instance corresponding to the given <typeparam name="TKey"/> key.</typeparam>
    /// </summary>
    /// <param name="key">The key to index with.</param>
    /// <param name="item">The returned value, if the master data contained it.</param>
    /// <returns>A bool indicating whether the value was successfully retrieved.</returns>
    public bool TryGetValue(TGroupKey key, [NotNullWhen(true)] out IDictionary<TKey, TItem>? item)
    {
        bool result = this.internalDictionary.Value.TryGetValue(
            key,
            out InternalKeyedCollection? entry
        );

        item = result ? entry!.AsImmutableDictionary() : null;

        return result;
    }

    /// <summary>
    /// Creates a new instance of <see cref="MasterAssetGroup{TGroupKey, TKey,TItem}"/>.
    /// </summary>
    /// <param name="jsonFilename">The filename of the JSON in <see cref="JsonFolder"/>.</param>
    /// <param name="keySelector">A function that returns a unique <typeparamref name="TKey"/> value from a
    /// <typeparamref name="TItem"/>.</param>
    public MasterAssetGroup(string jsonFilename, Func<TItem, TKey> keySelector)
    {
        this.jsonFilename = jsonFilename;
        this.keySelector = keySelector;
        this.internalDictionary = new(DataFactory);
    }

    private Dictionary<TGroupKey, InternalKeyedCollection> DataFactory()
    {
        string path = Path.Join(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            JsonFolder,
            jsonFilename
        );

        Dictionary<TGroupKey, IEnumerable<TItem>> items =
            JsonSerializer.Deserialize<Dictionary<TGroupKey, IEnumerable<TItem>>>(
                File.ReadAllText(path),
                MasterAssetJsonOptions.Instance
            )
            ?? throw new JsonException("Deserialized Dictionary<int, IEnumerable<TItem>> was null");

        Dictionary<TGroupKey, InternalKeyedCollection> dict = items.ToDictionary(
            x => x.Key,
            x =>
            {
                InternalKeyedCollection collection = new(this.keySelector);
                foreach (TItem item in x.Value)
                    collection.Add(item);

                return collection;
            }
        );

        return dict;
    }

    private class InternalKeyedCollection : KeyedCollection<TKey, TItem>
    {
        private readonly Func<TItem, TKey> keySelector;

        public InternalKeyedCollection(Func<TItem, TKey> keySelector)
        {
            this.keySelector = keySelector;
        }

        protected override TKey GetKeyForItem(TItem item)
        {
            return this.keySelector.Invoke(item);
        }

        public Dictionary<TKey, TItem> AsImmutableDictionary()
        {
            Debug.Assert(this.Dictionary != null, "this.Dictionary != null");

            return this.Dictionary.ToDictionary();
        }
    }
}
