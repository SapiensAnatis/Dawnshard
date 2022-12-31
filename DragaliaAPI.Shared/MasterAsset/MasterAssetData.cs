using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;

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

        IEnumerable<TItem>? items = JsonSerializer.Deserialize<IEnumerable<TItem>>(
            File.ReadAllText(path),
            new JsonSerializerOptions() { PropertyNamingPolicy = new MasterAssetNamingPolicy() }
        );

        if (items is null)
            return;

        int j = 0;
        foreach (TItem i in items)
        {
            j++;
            this.internalKeyCollection.Add(i);
        }
    }

    public TItem Get(TKey key)
    {
        return this.internalKeyCollection[key];
    }

    public IEnumerable<TItem> Data => this.internalKeyCollection.AsEnumerable();
}
