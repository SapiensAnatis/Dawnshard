using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace DragaliaAPI.Shared.SourceGenerator;

public static class Transforms
{
    public static EquatableReadOnlyList<MasterAssetDeclaration> TransformMasterAssetDeclarations(
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

                if (attribute.AttributeClass is not { TypeArguments: [{ } itemTypeSymbol] })
                {
                    continue;
                }

                if (
                    attribute.ConstructorArguments.Length < 1
                    || attribute.ConstructorArguments[0].Value is not string jsonPath
                )
                {
                    continue;
                }

                if (!attribute.TryGetNamedArgument<string>("Key", out string? keyName))
                {
                    keyName = "Id";
                }

                bool isGroup =
                    attribute.TryGetNamedArgument("Group", out bool groupValue) && groupValue;

                if (
                    itemTypeSymbol.GetMembers(keyName).FirstOrDefault()
                    is not IPropertySymbol keyPropertySymbol
                )
                {
                    continue;
                }

                ITypeSymbol keyTypeSymbol = keyPropertySymbol.Type;

                yield return new MasterAssetDeclaration(
                    keyTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    itemTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    jsonPath,
                    keyName,
                    isGroup
                );
            }
        }
    }

    public static EquatableReadOnlyList<MasterAssetExtensionDeclaration> TransformExtensionDeclarations(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        return ProcessDeclarations(context, cancellationToken).ToEquatableReadOnlyList();

        static IEnumerable<MasterAssetExtensionDeclaration> ProcessDeclarations(
            GeneratorAttributeSyntaxContext context,
            CancellationToken cancellationToken
        )
        {
            foreach (AttributeData? attribute in context.Attributes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (
                    attribute.ConstructorArguments
                    is not [{ Value: string masterAssetName }, { Value: string jsonPath }]
                )
                {
                    continue;
                }

                attribute.TryGetNamedArgument("FeatureFlag", out string? featureFlag);

                yield return new MasterAssetExtensionDeclaration(
                    masterAssetName,
                    jsonPath,
                    featureFlag
                );
            }
        }
    }
}

file static class AttributeDataExtensions
{
    public static bool TryGetNamedArgument<TValue>(
        this AttributeData attributeData,
        string argumentName,
        [MaybeNullWhen(false)] out TValue value
    )
    {
        value = default;

        TypedConstant? matchingArgumentValue = null;

        foreach (KeyValuePair<string, TypedConstant> kvp in attributeData.NamedArguments)
        {
            if (kvp.Key == argumentName)
            {
                matchingArgumentValue = kvp.Value;
            }
        }

        if (matchingArgumentValue is not { Value: TValue foundValue })
        {
            return false;
        }

        value = foundValue;
        return true;
    }
}
