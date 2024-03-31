using System.Text;

namespace DragaliaAPI.Shared.SourceGenerator;

public class CodeStringBuilder
{
    private readonly StringBuilder stringBuilder = new();
    private int indentLevel;

    public void IncreaseIndent() => this.indentLevel++;

    public void DecreaseIndent()
    {
        if (this.indentLevel == 0)
            return;

        this.indentLevel--;
    }

    public void AppendLine() => this.stringBuilder.AppendLine();

    public void AppendLine(string value)
    {
        string indent = new string(' ', this.indentLevel * 4);
        string indentedValue = value.Replace("\n", $"\n{indent}");

        this.stringBuilder.AppendLine($"{indent}{indentedValue}");
    }

    public void Append(string value)
    {
        string indent = new string(' ', this.indentLevel * 4);
        string indentedValue = value.Replace("\n", $"\n{indent}");

        this.stringBuilder.Append($"{indent}{indentedValue}");
    }

    public override string ToString() => this.stringBuilder.ToString();
}
