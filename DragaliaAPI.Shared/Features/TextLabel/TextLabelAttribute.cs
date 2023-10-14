namespace DragaliaAPI.Shared.Features.TextLabel;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class TextLabelAttribute : Attribute
{
    public TextLabelAttribute(string text)
    {
        this.Text = text;
    }

    public string Text { get; }
}
