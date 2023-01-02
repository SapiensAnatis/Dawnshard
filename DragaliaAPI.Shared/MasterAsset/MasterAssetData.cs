using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Shared.MasterAsset;

public class MasterAssetData<TKey, TItem>
    where TItem : class
    where TKey : notnull
{
    public class InternalKeyedCollection : KeyedCollection<TKey, TItem>
    {
        private Func<TItem, TKey> keySelector { get; }

        public InternalKeyedCollection(Func<TItem, TKey> keySelector)
        {
            this.keySelector = keySelector;
        }

        protected override TKey GetKeyForItem(TItem item)
        {
            return this.keySelector.Invoke(item);
        }
    }

    private readonly InternalKeyedCollection internalKeyCollection;

    private const string JsonFolder = "Resources";

    public MasterAssetData(string jsonFilename, Func<TItem, TKey> keySelector)
    {
        this.internalKeyCollection = new(keySelector);
        string path = Path.Join(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            JsonFolder,
            jsonFilename
        );

        JsonSerializerOptions jsonOpts =
            new() { PropertyNamingPolicy = new MasterAssetNamingPolicy() };
        jsonOpts.Converters.Add(new BoolIntJsonConverter());
        IEnumerable<TItem>? items = JsonSerializer.Deserialize<IEnumerable<TItem>>(
            File.ReadAllText(path),
            jsonOpts
        );

        if (items is null)
            return;

        foreach (TItem i in items)
        {
            this.internalKeyCollection.Add(i);
        }
    }

    public TItem Get(TKey key)
    {
        return this.internalKeyCollection[key];
    }

    public IEnumerable<TItem> Enumerable => this.internalKeyCollection.AsEnumerable();
}
