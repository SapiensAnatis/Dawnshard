using System.Xml.XPath;
using Basic.Reference.Assemblies;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DragaliaAPI.Shared.SourceGenerator.Test;

public class MasterAssetGeneratorTest
{
    [Fact]
    public async Task GeneratesMasterAssetCorrectly()
    {
        string source = """
            using DragaliaAPI.Shared.MasterAsset;
            using DragaliaAPI.Shared.MasterAsset.Models;
            using DragaliaAPI.Shared.MasterAsset.Models.TimeAttack;
            using DragaliaAPI.Shared.MasterAsset.Models.Event;

            [assembly: ExtendMasterAsset(nameof(MasterAsset.EventData), "Event/BuildEventReward.extension.json")]
            [assembly: ExtendMasterAsset(nameof(MasterAsset.DragonData), "DragonData.modded.json", FeatureFlag = "ModdedDragons")]

            namespace DragaliaAPI.Shared.MasterAsset;

            public enum Charas
            {
                Illia,
            }

            public record CharaData(Charas Id);
            public record RankingData(int QuestId);
            public record BuildEventReward(int Id);
            public record EventData(int Id);
            public record DragonData(int Id);

            [GenerateMasterAsset<CharaData>("CharaData.json")]
            [GenerateMasterAsset<RankingData>("TimeAttack/RankingData.json", Key = nameof(Models.TimeAttack.RankingData.QuestId))]
            [GenerateMasterAsset<BuildEventReward>("Event/BuildEventReward.json", Group = true)]
            [GenerateMasterAsset<EventData>("Event/EventData.json")]
            [GenerateMasterAsset<DragonData>("DragonData.json")]
            public static partial class MasterAsset
            {
            }
            """;

        GeneratorDriver driver = GetDriver(source);

        await Verify(driver);
    }

    private static GeneratorDriver GetDriver(string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        List<PortableExecutableReference> references = [.. ReferenceAssemblies.Net80];

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: [syntaxTree],
            references
        );

        MasterAssetGenerator generator = new();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        GeneratorDriverRunResult result = driver.GetRunResult();
        result.Diagnostics.Should().BeEmpty();

        return driver;
    }
}
