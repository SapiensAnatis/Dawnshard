using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DragaliaAPI.MemoryPack;

public record struct TypeIdentifier(string BaseTypeName, string? GenericArg);

/// <summary>
/// Analyzes the syntax tree of MasterAsset.cs to build associations between record types and the JSON file they model.
/// </summary>
public class MasterAssetWalker : CSharpSyntaxWalker
{
    public Dictionary<string, TypeIdentifier> JsonFileTypeMapping { get; } = [];

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (
            node.Expression
            is not MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax
                {
                    Identifier: { Text: "MasterAssetData" or "MasterAssetGroup" }
                }
            } memberAccessExpressionSyntax
        )
        {
            base.VisitInvocationExpression(node);
            return;
        }

        ExpressionSyntax stringExpr = node.ArgumentList.Arguments[0].Expression;
        string fileLiteral = stringExpr.ChildTokens().First().ValueText;

        NameSyntax recordTypeName = (NameSyntax)
            ((GenericNameSyntax)memberAccessExpressionSyntax.Name).TypeArgumentList.Arguments[^1];

        TypeIdentifier recordType;

        if (recordTypeName is GenericNameSyntax genericNameSyntax)
        {
            string argName = genericNameSyntax.TypeArgumentList.Arguments[0].ToString();
            recordType = new(genericNameSyntax.Identifier.Text, argName);
        }
        else if (recordTypeName is SimpleNameSyntax simpleNameSyntax)
        {
            recordType = new(simpleNameSyntax.Identifier.Text, null);
        }
        else
        {
            throw new UnreachableException();
        }

        string jsonFilePath = fileLiteral.Replace(".msgpack", ".json");
        this.JsonFileTypeMapping[jsonFilePath] = recordType;

        base.VisitInvocationExpression(node);
    }
}
