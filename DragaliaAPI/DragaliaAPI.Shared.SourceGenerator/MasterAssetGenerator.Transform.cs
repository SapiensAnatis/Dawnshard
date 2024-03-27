using Microsoft.CodeAnalysis;

namespace DragaliaAPI.Shared.SourceGenerator;

public partial class MasterAssetGenerator
{
    private static EquatableReadOnlyList<MasterAssetDeclaration> TransformMasterAssetDeclarations(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        return ProcessDeclarations(context, cancellationToken).ToEquatableReadOnlyList();

        static IEnumerable<MasterAssetDeclaration> ProcessDeclarations(
            GeneratorAttributeSyntaxContext context,
            CancellationToken cancellationToken
        )
        {
            foreach (AttributeData? attribute in context.Attributes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (
                    attribute.AttributeClass
                    is not { TypeArguments: [{ } keyTypeSymbol, { } itemTypeSymbol] }
                )
                {
                    throw new InvalidOperationException("Invalid type arguments");
                }

                if (
                    attribute.ConstructorArguments
                    is not [{ Value: string jsonPath }, { Value: string keyName }]
                )
                {
                    throw new InvalidOperationException("Invalid constructor arguments");
                }

                yield return new MasterAssetDeclaration(
                    keyTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    itemTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    jsonPath,
                    keyName
                );
            }
        }
    }

    private readonly record struct MasterAssetDeclaration(
        string FullyQualifiedKeyTypeName,
        string FullyQualifiedItemTypeName,
        string JsonPath,
        string KeyName
    );
}
