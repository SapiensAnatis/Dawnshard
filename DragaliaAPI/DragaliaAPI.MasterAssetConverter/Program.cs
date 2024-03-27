using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using DragaliaAPI.MemoryPack;
using DragaliaAPI.Shared.Json;
using DragaliaAPI.Shared.MasterAsset;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

string sharedProjectPath = args[^1];

SyntaxTree testTree = CSharpSyntaxTree.ParseText(
    """
    ValueTask<MasterAssetData<int, NormalMission>> beginnerMission = MasterAssetData.LoadAsync<
           int,
           NormalMission
       >("Missions/MissionBeginnerData.msgpack", x => x.Id);
    """
);

CompilationUnitSyntax testRoot = testTree.GetCompilationUnitRoot();

MasterAssetWalker testWalker = new();
testWalker.Visit(testRoot);

SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
    File.ReadAllText(Path.Join(sharedProjectPath, "/MasterAsset/MasterAsset.cs"))
);
CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

MasterAssetWalker walker = new();
walker.Visit(root);

Dictionary<string, Type> typeLookup = typeof(MasterAsset)
    .Assembly.GetTypes()
    .ToDictionary(x => x.Name, x => x);

#if false
foreach ((string jsonFilename, TypeIdentifier typeName) in walker.JsonFileTypeMapping)
{
    Type itemType;
    if (typeName.GenericArg is not null)
    {
        string typeLookupKey = $"{typeName.BaseTypeName}`1";
        Type genericArgType = typeLookup[typeName.GenericArg];
        itemType = typeLookup[typeLookupKey].MakeGenericType(genericArgType);
    }
    else
    {
        itemType = typeLookup[typeName.BaseTypeName];
    }

    // I'm writing a fucking source generator so that I can analyze the attribute here
    // Can remove all the syntaxwalker shit if the filepath is in an attribute

    string jsonPath = Path.Combine(sharedProjectPath, "Resources", jsonFilename);

    string pathName = Path.GetFileNameWithoutExtension(jsonPath);
    string outputPath = Path.Join(Path.GetDirectoryName(jsonPath), $"{pathName}.msgpack");

    if (
        File.Exists(outputPath)
        && new FileInfo(outputPath).LastWriteTime >= new FileInfo(jsonPath).LastWriteTime
    )
    {
        Console.WriteLine($"Skipping conversion of {pathName} - binary converted file is newer");
        continue;
    }

    Type listType = typeof(IList<>).MakeGenericType(itemType);

    Type deserializationType =
        propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(MasterAssetGroup<,,>)
            ? typeof(IDictionary<,>).MakeGenericType(
                propertyInfo.PropertyType.GetGenericArguments()[0],
                listType
            )
            : listType;

    await using FileStream fs = File.OpenRead(jsonPath);

    object deserialized =
        await JsonSerializer.DeserializeAsync(
            fs,
            deserializationType,
            MasterAssetJsonOptions.Instance
        ) ?? throw new UnreachableException("Deserialization failure");

    await using FileStream outputFs = File.Create(outputPath);

    MessagePackSerializer.Serialize(
        deserialized.GetType(),
        outputFs,
        deserialized,
        new MessagePackSerializerOptions(ContractlessStandardResolver.Instance).WithCompression(
            MessagePackCompression.Lz4BlockArray
        )
    );

    Console.WriteLine($"Serialized {pathName} to binary.");
}
#endif
