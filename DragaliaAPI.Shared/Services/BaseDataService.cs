using System.Reflection;
using System.Text.Json;
using DragaliaAPI.Shared.Definitions;

namespace DragaliaAPI.Shared.Services;

public abstract class BaseDataService<TData, TIndex> : IBaseDataService<TData, TIndex>
    where TData : IDataItem<TIndex>
    where TIndex : notnull
{
    private const string DataFolder = "Resources";

    private readonly Dictionary<TIndex, TData> dictionary;

    public IEnumerable<TData> AllData => dictionary.Values;

    public BaseDataService(string filename)
    {
        string json = File.ReadAllText(
            Path.Join(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                DataFolder,
                filename
            )
        );
        List<TData> deserialized =
            JsonSerializer.Deserialize<List<TData>>(json)
            ?? throw new JsonException("Deserialization failure");

        this.dictionary = deserialized
            .Select(x => new KeyValuePair<TIndex, TData>(x.Id, x))
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public TData GetData(TIndex id)
    {
        return this.dictionary[id];
    }
}
