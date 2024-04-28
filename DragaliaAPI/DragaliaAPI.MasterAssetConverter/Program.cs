using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using DragaliaAPI.MasterAssetConverter;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.Serialization;
using MessagePack;

string resourcesPath = args[^2];
string outputDir = args[^1];

List<AttributeInstance> attributeInstances = typeof(MasterAsset)
    .GetCustomAttributes(typeof(GenerateMasterAssetAttribute<>))
    .Select(AttributeHelper.ParseAttribute)
    .ToList();

foreach (AttributeInstance instance in attributeInstances)
{
    string fullJsonPath = Path.Combine(resourcesPath, instance.JsonPath);
    string relativePath = Path.GetRelativePath(resourcesPath, fullJsonPath);
    string outputPath = Path.Combine(outputDir, relativePath.Replace(".json", ".msgpack"));

    if (
        File.Exists(outputPath)
        && new FileInfo(outputPath).LastWriteTime >= new FileInfo(fullJsonPath).LastWriteTime
    )
    {
        Console.WriteLine(
            $"Skipping conversion of {relativePath} - binary converted file is newer"
        );
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

    if (Path.GetDirectoryName(outputPath) is { } nonNullDir)
    {
        Directory.CreateDirectory(nonNullDir);
    }

    await using FileStream outputFs = File.Create(outputPath);

    MessagePackSerializer.Serialize(
        deserializationType,
        outputFs,
        deserialized,
        MasterAssetMessagePackOptions.Instance
    );

    Console.WriteLine($"Serialized {relativePath} to binary at {outputPath}.");
}
