using System.Collections.Immutable;
using DragaliaAPI.Shared.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;

namespace DragaliaAPI.Shared.MasterAsset;

public class MasterAssetGroup<TKey, TItem>
    where TItem : class
    where TKey : notnull
{
    private const string JsonFolder = "Resources";

    private readonly string jsonFilename;
    private readonly Func<TItem, TKey> keySelector;
    private readonly Lazy<Dictionary<int, InternalKeyedCollection>> internalDictionary;

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
    public IDictionary<TKey, TItem> Get(int key) => this[key];

    /// <summary>
    /// Get a <typeparam name="TItem"> instance corresponding to the given <typeparam name="TKey"/> key.</typeparam>
    /// </summary>
    /// <param name="key">The key to index with.</param>
    /// <returns>The returned value.</returns>
    /// <exception cref="KeyNotFoundException">The given key was not present in the collection.</exception>
    public IDictionary<TKey, TItem> this[int key] =>
        this.internalDictionary.Value[key].AsImmutableDictionary();

    /// <summary>
    /// Attempts to get a <typeparam name="TItem"> instance corresponding to the given <typeparam name="TKey"/> key.</typeparam>
    /// </summary>
    /// <param name="key">The key to index with.</param>
    /// <param name="item">The returned value, if the master data contained it.</param>
    /// <returns>A bool indicating whether the value was successfully retrieved.</returns>
    public bool TryGetValue(int key, [NotNullWhen(true)] out IEnumerable<TItem>? item)
    {
        bool result = this.internalDictionary.Value.TryGetValue(
            key,
            out InternalKeyedCollection? entry
        );

        item = result ? entry!.AsEnumerable() : null;

        return result;
    }

    /// <summary>
    /// Creates a new instance of <see cref="MasterAssetGroup{TKey,TItem}"/>.
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

    private Dictionary<int, InternalKeyedCollection> DataFactory()
    {
        string path = Path.Join(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            JsonFolder,
            jsonFilename
        );

        Dictionary<int, IEnumerable<TItem>> items =
            JsonSerializer.Deserialize<Dictionary<int, IEnumerable<TItem>>>(
                File.ReadAllText(path),
                MasterAssetJsonOptions.Instance
            )
            ?? throw new JsonException("Deserialized Dictionary<int, IEnumerable<TItem>> was null");

        Dictionary<int, InternalKeyedCollection> dict = items.ToDictionary(
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

        public IDictionary<TKey, TItem> AsImmutableDictionary()
        {
            Debug.Assert(this.Dictionary != null, "this.Dictionary != null");

            return this.Dictionary.ToImmutableDictionary();
        }
    }
}
