using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using DragaliaAPI.MasterAssetConverter;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.Serialization;
using MessagePack;

string outputDir = args[0];
string resourcesPath = args[1];

// MSBuild will pass us the list of JSON files that are out of date.
// https://learn.microsoft.com/en-us/visualstudio/msbuild/incremental-builds?view=vs-2022
// "If only some files are up-to-date, MSBuild executes the target but skips the up-to-date items,
// and thereby brings all items up-to-date. This process is known as a partial incremental build."
string[] jsonFiles = args[2..];

Dictionary<string, GenerateMasterAssetAttributeInstance> attributeInstancesByPropertyName =
    typeof(MasterAsset)
        .GetCustomAttributes(typeof(GenerateMasterAssetAttribute<>))
        .Select(AttributeHelper.ParseGenerateMasterAssetAttribute)
        .ToDictionary(x => x.PropertyName, x => x);

Dictionary<string, ExtendMasterAssetAttribute> extensionAttributesByPath = typeof(MasterAsset)
    .Assembly.GetCustomAttributes<ExtendMasterAssetAttribute>()
    .ToDictionary(x => x.Filepath, x => x);

foreach (string fullJsonPath in jsonFiles)
{
    string propertyName = Path.GetFileName(fullJsonPath).Replace(".json", "");
    string relativePath = Path.GetRelativePath(resourcesPath, fullJsonPath);

    if (
        attributeInstancesByPropertyName.TryGetValue(
            propertyName,
            out GenerateMasterAssetAttributeInstance? instance
        )
    )
    {
        await Convert(fullJsonPath, instance);
    }
    else if (
        extensionAttributesByPath.TryGetValue(
            relativePath,
            out ExtendMasterAssetAttribute? extensionAttribute
        )
    )
    {
        GenerateMasterAssetAttributeInstance baseInstance = attributeInstancesByPropertyName[
            extensionAttribute.MasterAssetName
        ];

        await Convert(fullJsonPath, baseInstance);
    }
    else
    {
        throw new ArgumentException("Unable to find attribute instance for " + fullJsonPath);
    }
}

async Task Convert(string inputPath, GenerateMasterAssetAttributeInstance instance)
{
    string fullJsonPath = Path.Combine(resourcesPath, inputPath);
    string relativePath = Path.GetRelativePath(resourcesPath, fullJsonPath);
    string outputPath = Path.Combine(outputDir, relativePath.Replace(".json", ".msgpack"));

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
