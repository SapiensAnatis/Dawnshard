using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using DragaliaAPI.Shared.Serialization;
using MessagePack;
using MessagePack.Resolvers;

namespace DragaliaAPI.Shared.MasterAsset;

public class MasterAssetGroup<TGroupKey, TKey, TItem>
    where TItem : class
    where TKey : notnull
    where TGroupKey : notnull
{
    private readonly FrozenDictionary<TGroupKey, FrozenDictionary<TKey, TItem>> internalDictionary;

    /// <summary>
    /// Gets a <see cref="IEnumerable{TItem}"/> of all the collection's values.
    /// </summary>
    public IEnumerable<IEnumerable<TItem>> Enumerable =>
        this.internalDictionary.Values.Select(x => x.Values.AsEnumerable());

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
    public IDictionary<TKey, TItem> this[TGroupKey key] => this.internalDictionary[key];

    /// <summary>
    /// Attempts to get a <typeparam name="TItem"> instance corresponding to the given <typeparam name="TKey"/> key.</typeparam>
    /// </summary>
    /// <param name="key">The key to index with.</param>
    /// <param name="item">The returned value, if the master data contained it.</param>
    /// <returns>A bool indicating whether the value was successfully retrieved.</returns>
    public bool TryGetValue(TGroupKey key, [NotNullWhen(true)] out IDictionary<TKey, TItem>? item)
    {
        bool result = this.internalDictionary.TryGetValue(
            key,
            out FrozenDictionary<TKey, TItem>? entry
        );

        item = result ? entry!.ToDictionary() : null;

        return result;
    }

    internal MasterAssetGroup(FrozenDictionary<TGroupKey, FrozenDictionary<TKey, TItem>> data)
    {
        this.internalDictionary = data;
    }
}

public static class MasterAssetGroup
{
    private const string DataFolder = "Resources";

    private static readonly MessagePackSerializerOptions MsgpackOptions =
        MessagePackSerializerOptions
            .Standard.WithResolver(ContractlessStandardResolver.Instance)
            .WithCompression(MessagePackCompression.Lz4BlockArray);

    public static async ValueTask<MasterAssetGroup<TGroupKey, TKey, TItem>> LoadAsync<
        TGroupKey,
        TKey,
        TItem
    >(string jsonFilename, Func<TItem, TKey> keySelector)
        where TItem : class
        where TKey : notnull
        where TGroupKey : notnull
    {
        string path = Path.Join(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            DataFolder,
            jsonFilename
        );

        await using FileStream fs = File.OpenRead(path);

        Dictionary<TGroupKey, List<TItem>> items =
            await MessagePackSerializer.DeserializeAsync<Dictionary<TGroupKey, List<TItem>>>(
                fs,
                MasterAssetMessagePackOptions.Instance
            ) ?? throw new JsonException("Deserialized IEnumerable was null");

        FrozenDictionary<TGroupKey, FrozenDictionary<TKey, TItem>> dict = items
            .ToDictionary(
                x => x.Key,
                x => x.Value.ToDictionary(keySelector, y => y).ToFrozenDictionary()
            )
            .ToFrozenDictionary();

        return new MasterAssetGroup<TGroupKey, TKey, TItem>(dict);
    }
}
