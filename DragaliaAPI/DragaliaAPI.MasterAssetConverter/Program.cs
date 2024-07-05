using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using DragaliaAPI.MasterAssetConverter;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.Serialization;
using MessagePack;

string outputDir = args[^1];
string resourcesPath = args[^2];

List<GenerateMasterAssetAttributeInstance> attributeInstances = typeof(MasterAsset)
    .GetCustomAttributes(typeof(GenerateMasterAssetAttribute<>))
    .Select(AttributeHelper.ParseGenerateMasterAssetAttribute)
    .ToList();

Dictionary<string, ExtendMasterAssetAttribute> extensionAttributes = typeof(MasterAsset)
    .Assembly.GetCustomAttributes<ExtendMasterAssetAttribute>()
    .ToDictionary(x => x.MasterAssetName, x => x);

foreach (GenerateMasterAssetAttributeInstance instance in attributeInstances)
{
    await Convert(instance.JsonPath, instance);

    if (
        extensionAttributes.TryGetValue(
            instance.PropertyName,
            out ExtendMasterAssetAttribute? extensionAttribute
        )
    )
    {
        await Convert(extensionAttribute.Filepath, instance);
    }
}

async Task Convert(string inputPath, GenerateMasterAssetAttributeInstance instance)
{
    string fullJsonPath = Path.Combine(resourcesPath, inputPath);
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
        return;
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
