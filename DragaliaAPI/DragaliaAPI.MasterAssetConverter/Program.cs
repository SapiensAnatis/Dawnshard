using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using DragaliaAPI.MemoryPack;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.Serialization;
using MessagePack;
using MessagePack.Resolvers;

string resourcesPath = args[^1];

List<AttributeInstance> attributeInstances = typeof(MasterAsset)
    .GetCustomAttributes(typeof(GenerateMasterAssetAttribute<>))
    .Select(AttributeHelper.ParseAttribute)
    .ToList();

foreach (AttributeInstance instance in attributeInstances)
{
    string fullJsonPath = Path.Combine(resourcesPath, instance.JsonPath);

    string pathName = Path.GetFileNameWithoutExtension(fullJsonPath);
    string outputPath = Path.Combine(Path.GetDirectoryName(fullJsonPath)!, $"{pathName}.msgpack");

    if (
        File.Exists(outputPath)
        && new FileInfo(outputPath).LastWriteTime >= new FileInfo(fullJsonPath).LastWriteTime
    )
    {
        Console.WriteLine($"Skipping conversion of {pathName} - binary converted file is newer");
        continue;
    }

    Type listType = typeof(IList<>).MakeGenericType(instance.ItemType);

    Type deserializationType = instance.Group
        ? typeof(IDictionary<,>).MakeGenericType(instance.KeyType, listType)
        : listType;

    await using FileStream fs = File.OpenRead(fullJsonPath);

    object deserialized =
        await JsonSerializer.DeserializeAsync(
            fs,
            deserializationType,
            MasterAssetJsonOptions.Instance
        ) ?? throw new UnreachableException("Deserialization failure");

    await using FileStream outputFs = File.Create(outputPath);

    MessagePackSerializer.Serialize(
        deserializationType,
        outputFs,
        deserialized,
        MasterAssetMessagePackOptions.Instance
    );

    Console.WriteLine($"Serialized {pathName} to binary.");
}
