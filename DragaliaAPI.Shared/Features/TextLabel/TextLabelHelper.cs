using System.Reflection;

namespace DragaliaAPI.Shared.Features.TextLabel;

public static class TextLabelHelper
{
    public static string? GetText<TEnum>(TEnum enumValue)
        where TEnum : struct, Enum
    {
        string? enumName = Enum.GetName(enumValue);

        if (enumName == null)
            return null;

        MemberInfo[] memberInfo = typeof(TEnum).GetMember(enumName);

        if (memberInfo.Length == 0)
            return null;

        object[] attributes = memberInfo[0].GetCustomAttributes(typeof(TextLabelAttribute), false);

        if (attributes.Length == 0)
            return null;

        return ((TextLabelAttribute)attributes[0]).Text;
    }
}
