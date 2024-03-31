using Basic.Reference.Assemblies;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DragaliaAPI.Shared.SourceGenerator.Test;

public class MasterAssetGeneratorTest
{
    [Fact]
    public async Task GeneratesMasterAssetCorrectly()
    {
        string source = """
            using DragaliaAPI.Shared.MasterAsset.Models;
            using DragaliaAPI.Shared.MasterAsset.Models.TimeAttack;
            using DragaliaAPI.Shared.MasterAsset.Models.Event;

            namespace DragaliaAPI.Shared.MasterAsset;

            [GenerateMasterAsset<CharaData>("CharaData.json")]
            [GenerateMasterAsset<RankingData>("TimeAttack/RankingData.json", Key = nameof(Models.TimeAttack.RankingData.QuestId))]
            [GenerateMasterAsset<BuildEventReward>("Event/BuildEventReward.json", Group = true)]
            public static partial class MasterAsset
            {
            }
            """;

        await Verify(source);
    }

    private static Task Verify(string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        List<PortableExecutableReference> references =
        [
            .. ReferenceAssemblies.Net80,
            MetadataReference.CreateFromFile(typeof(CharaData).Assembly.Location)
        ];

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references
        );

        MasterAssetGenerator generator = new();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        return Verifier.Verify(driver);
    }
}
