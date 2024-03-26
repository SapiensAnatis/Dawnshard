using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DragaliaAPI.MemoryPack;

public class MasterAssetWalker : CSharpSyntaxWalker
{
    public Dictionary<string, string> TypeJsonFileMapping = [];

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (node.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            base.VisitInvocationExpression(node);
            return;
        }

        if (node.ArgumentList.Arguments is not [ArgumentSyntax { Expression: { } stringExpr }, _])
        {
            base.VisitInvocationExpression(node);
            return;
        }

        string fileLiteral = stringExpr.ChildTokens().First().ValueText;

        if (
            memberAccessExpressionSyntax.Name
            is not GenericNameSyntax
            {
                TypeArgumentList: { Arguments: [_, NameSyntax recordTypeName] }
            }
        )
        {
            base.VisitInvocationExpression(node);
            return;
        }

        string recordTypeKey;

        if (recordTypeName is GenericNameSyntax genericNameSyntax)
        {
            string argName = genericNameSyntax.TypeArgumentList.Arguments[0].ToString();
            recordTypeKey = genericNameSyntax.Identifier.Text + argName;
        }
        else if (recordTypeName is SimpleNameSyntax simpleNameSyntax)
        {
            recordTypeKey = simpleNameSyntax.Identifier.Text;
        }
        else
        {
            throw new UnreachableException();
        }

        this.TypeJsonFileMapping[recordTypeKey] = fileLiteral;

        base.VisitInvocationExpression(node);
    }
}
