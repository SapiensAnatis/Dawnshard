// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using DragaliaAPI.MemoryPack;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using MemoryPack;
using MemoryPack.Compression;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

Console.WriteLine("Hello, World!");

string[] filenames = Directory.GetFiles(
    "/home/jay/RiderProjects/DragaliaAPI/DragaliaAPI/DragaliaAPI.Shared/Resources/",
    "*.json",
    SearchOption.AllDirectories
);

SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
    File.ReadAllText(
        "/home/jay/RiderProjects/DragaliaAPI/DragaliaAPI/DragaliaAPI.Shared/MasterAsset/MasterAsset.cs"
    )
);
CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

var walker = new MasterAssetWalker();
walker.Visit(root);

IEnumerable<Type> types = typeof(MasterAsset)
    .GetProperties(BindingFlags.Static | BindingFlags.Public)
    .Where(x => x.PropertyType.GetGenericTypeDefinition() == typeof(MasterAssetData<,>))
    .Select(x => x.PropertyType.GetGenericArguments().First(y => y != typeof(string) && y.IsClass));

using var compressor = new BrotliCompressor();

foreach (Type recordType in types)
{
    string typeName;
    if (recordType.IsGenericType)
    {
        typeName =
            $"{recordType.GetGenericTypeDefinition().Name[..^2]}{recordType.GetGenericArguments()[0].Name}";
    }
    else
    {
        typeName = recordType.Name;
    }

    string path = Path.Join(
        "/home/jay/RiderProjects/DragaliaAPI/DragaliaAPI/DragaliaAPI.Shared/Resources/"
            + walker.TypeJsonFileMapping[typeName]
    );

    string pathName = Path.GetFileNameWithoutExtension(path);
    string outputName = Path.Join(Path.GetDirectoryName(path), $"{pathName}.bin");
    Type listType = typeof(IList<>).MakeGenericType(recordType);

    await using FileStream fs = File.OpenRead(path);

    object deserialized =
        await JsonSerializer.DeserializeAsync(fs, listType)
        ?? throw new UnreachableException("Deserialization failure");

    MemoryPackSerializer.Serialize(deserialized.GetType(), compressor, deserialized);
    await File.WriteAllBytesAsync(outputName, compressor.ToArray());
}
